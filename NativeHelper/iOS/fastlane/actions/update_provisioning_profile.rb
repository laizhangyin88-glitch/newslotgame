# coding: utf-8
module Fastlane
  module Actions
    class UpdateProvisioningProfileAction < Action
      def self.run(params)
        require 'plist'
        require 'xcodeproj'

        info_plist_key = 'INFOPLIST_FILE'
        developer_team_key = 'DEVELOPMENT_TEAM'
        provisiong_profile_key = 'PROVISIONING_PROFILE'
        provisiong_profile_specifier_key = 'PROVISIONING_PROFILE_SPECIFIER'
        codesign_identity_key = 'CODE_SIGN_IDENTITY[sdk=iphoneos*]'

        # Load .xcodeproj
        project_path = params[:xcodeproj]
        project = Xcodeproj::Project.open(project_path)

        # Fetch the build configuration objects
        configs = project.objects.select { |obj| obj.isa == 'XCBuildConfiguration'}
        UI.user_error!("Not found XCBuildConfiguration from xcodeproj") unless configs.count > 0

        configs = configs.select { |obj| obj.build_settings[info_plist_key] == params[:plist_path] }
        UI.user_error!("Xcodeproj doesn't have configuration with info plist #{params[:plist_path]}.") unless configs.count > 0

        # For each of the build configurations, set app identifier
        configs.each do |c|
          c.build_settings[developer_team_key] = params[:team_id]
          c.build_settings[provisiong_profile_key] = params[:provisiong_profile]
          c.build_settings[provisiong_profile_specifier_key] = params[:provisiong_profile_specifier]
          c.build_settings[codesign_identity_key] = params[:codesign_identity]
        end

        # Write changes to the file
        project.save

        UI.success("Updated #{params[:xcodeproj]} 💾.")
      end

      #####################################################
      # @!group Documentation
      #####################################################

      def self.is_supported?(platform)
        [:ios].include?(platform)
      end

      def self.description
        "Update the project's bundle identifier"
      end

      def self.details
        "Update an app identifier by either setting `CFBundleIdentifier` or `PRODUCT_BUNDLE_IDENTIFIER`, depending on which is already in use."
      end

      def self.available_options
        [
          FastlaneCore::ConfigItem.new(key: :xcodeproj,
                                       env_name: "FL_UPDATE_PROVISIONG_PROFILE_PROJECT_PATH",
                                       description: "Path to your Xcode project",
                                       default_value: Dir['*.xcodeproj'].first,
                                       verify_block: proc do |value|
                                         UI.user_error!("Please pass the path to the project, not the workspace") unless value.end_with?(".xcodeproj")
                                         UI.user_error!("Could not find Xcode project") unless File.exist?(value)
                                       end),
          FastlaneCore::ConfigItem.new(key: :plist_path,
                                       env_name: "FL_UPDATE_PROVISIONG_PROFILE_PLIST_PATH",
                                       description: "Path to info plist, relative to your Xcode project",
                                       verify_block: proc do |value|
                                         UI.user_error!("Invalid plist file") unless value[-6..-1].casecmp(".plist").zero?
                                       end),
          FastlaneCore::ConfigItem.new(key: :provisiong_profile,
                                       env_name: 'FL_UPDATE_PROVISIONG_PROFILE',
                                       description: 'The app Identifier you want to set',
                                       default_value: ENV['PRODUCE_PROVISIONG_PROFILE'] || CredentialsManager::AppfileConfig.try_fetch_value(:provisiong_profile)),
          FastlaneCore::ConfigItem.new(key: :provisiong_profile_specifier,
                                       env_name: 'FL_UPDATE_PROVISIONG_PROFILE_SPECIFIER',
                                       description: 'The app Identifier you want to set',
                                       default_value: ENV['PRODUCE_PROVISIONG_PROFILE_SPECIFIER'] || CredentialsManager::AppfileConfig.try_fetch_value(:provisiong_profile_specifier)),
          FastlaneCore::ConfigItem.new(key: :team_id,
                                       env_name: 'FL_UPDATE_TEAM_ID',
                                       description: 'Team ID you want to set',
                                       default_value: ENV['PRODUCE_TEAM_ID'] || CredentialsManager::AppfileConfig.try_fetch_value(:team_id)),
          FastlaneCore::ConfigItem.new(key: :codesign_identity,
                                       env_name: 'FL_UPDATE_CODESIGN_IDENTITY',
                                       description: 'Codesign identity you want to set',
                                       default_value: ENV['PRODUCE_CODESIGN_IDENTITY'])
        ]
      end

      def self.authors
        ['squarefrog', 'tobiasstrebitzer']
      end

      def self.example_code
        [
          'update_provisiong_profile(
            xcodeproj: "Example.xcodeproj", # Optional path to xcodeproj, will use the first .xcodeproj if not set
            provisiong_profile: "com.test.example" # The provisioning profile
          )'
        ]
      end

      def self.category
        :project
      end
    end
  end
end
