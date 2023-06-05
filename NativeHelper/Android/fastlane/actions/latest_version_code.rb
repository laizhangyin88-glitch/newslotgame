require 'googleauth'
require 'google/apis/androidpublisher_v3'
Androidpublisher = Google::Apis::AndroidpublisherV3
CredentialsLoader = Google::Auth::CredentialsLoader

module Fastlane
  module Actions

    class Client
      # Connecting with Google
      attr_accessor :android_publisher

      # Editing something
      # Reference to the entry we're currently editing. Might be nil if don't have one open
      attr_accessor :current_edit
      # Package name of the currently edited element
      attr_accessor :current_package_name

      #####################################################
      # @!group Login
      #####################################################

      # Initializes the android_publisher and its auth_client using the specified information
      # @param service_account_json: The raw service account Json data
      def initialize(service_account_json: nil)
        scope = Androidpublisher::AUTH_ANDROIDPUBLISHER
        key_io = service_account_json
        auth_client = Google::Auth::ServiceAccountCredentials.make_creds(json_key_io: key_io, scope: scope)

        UI.verbose("Fetching a new access token from Google...")

        auth_client.fetch_access_token!

        Google::Apis::ClientOptions.default.application_name = "fastlane - supply"
        Google::Apis::ClientOptions.default.application_version = Fastlane::VERSION
        Google::Apis::ClientOptions.default.read_timeout_sec = 300
        Google::Apis::ClientOptions.default.open_timeout_sec = 300
        Google::Apis::RequestOptions.default.retries = 5

        self.android_publisher = Androidpublisher::AndroidPublisherService.new
        self.android_publisher.authorization = auth_client
      end

      #####################################################
      # @!group Handling the edit lifecycle
      #####################################################

      # Begin modifying a certain package
      def begin_edit(package_name: nil)
        UI.user_error!("You currently have an active edit") if @current_edit

        self.current_edit = call_google_api { android_publisher.insert_edit(package_name) }

        self.current_package_name = package_name
      end

      # Aborts the current edit deleting all pending changes
      def abort_current_edit
        ensure_active_edit!

        call_google_api { android_publisher.delete_edit(current_package_name, current_edit.id) }

        self.current_edit = nil
        self.current_package_name = nil
      end

      #####################################################
      # @!group Getting data
      #####################################################

      # Get a list of all apks verion codes - returns the list of version codes
      def apks_version_codes
        ensure_active_edit!

        result = call_google_api { android_publisher.list_edit_apks(current_package_name, current_edit.id) }

        return result.apks.map(&:version_code)
      end
      
      # Get a list of all bundles verion codes - returns the list of version codes
      def bundles_version_codes
        ensure_active_edit!

        result = call_google_api { android_publisher.list_edit_bundles(current_package_name, current_edit.id) }

        return result.bundles.map(&:version_code)
      end

      private

      def ensure_active_edit!
        UI.user_error!("You need to have an active edit, make sure to call `begin_edit`") unless @current_edit
      end

      def call_google_api
        yield if block_given?
      rescue Google::Apis::ClientError => e
        UI.user_error! "Google Api Error: #{e.message}"
      end
    end
    
    class LatestVersionCodeAction < Action
      def self.run(params)
        service_account_json = File.open(File.expand_path(params[:json_key]))
        client = Client.new(service_account_json: service_account_json)
        client.begin_edit(package_name: params[:package_name])
        # Use bundles to get version code instead of apks
        version_codes = client.bundles_version_codes()
        client.abort_current_edit

        latest_version_code = version_codes.max
        return latest_version_code == nil ? 0 : latest_version_code
      end

      #####################################################
      # @!group Documentation
      #####################################################

      def self.description
        "Get latest version code from Google PlayStore"
      end

      def self.available_options
        @options ||= [
          FastlaneCore::ConfigItem.new(key: :package_name,
                                       env_name: "SUPPLY_PACKAGE_NAME",
                                       short_option: "-p",
                                       description: "The package name of the Application to modify",
                                       default_value: CredentialsManager::AppfileConfig.try_fetch_value(:package_name)),

          FastlaneCore::ConfigItem.new(key: :json_key,
                                       env_name: "SUPPLY_JSON_KEY",
                                       short_option: "-j",
                                       description: "The service account json file used to authenticate with Google",
                                       default_value: CredentialsManager::AppfileConfig.try_fetch_value(:json_key_file),
                                       verify_block: proc do |value|
                                         UI.user_error! "'#{value}' doesn't seem to be a JSON file" unless FastlaneCore::Helper.json_file?(File.expand_path(value))
                                         UI.user_error! "Could not find service account json file at path '#{File.expand_path(value)}'" unless File.exist?(File.expand_path(value))
                                       end)
        ]
      end

      def self.authors
        ["@jbseo"]
      end

      def self.is_supported?(platform)
        platform == :android
      end
    end
  end
end
