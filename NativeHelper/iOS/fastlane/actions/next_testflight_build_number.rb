# coding: utf-8
module Fastlane
  module Actions
    class NextTestflightBuildNumberAction < Action

      # def self.find_build_from_latest_version_candidate_builds(target_build_version, target_build_number, app)
      #   build = nil
      #   app.latest_version.candidate_builds.each do |b|
      #     build = b if b.build_version == target_build_number.to_s && b.train_version == target_build_version
      #   end
      #   return build
      # end

      # def self.loginAndGetApp(appIdentifier)
      #   Spaceship::Tunes::login()
      #   return Spaceship::Tunes::Application.find(appIdentifier)
      # end

      def self.run(params)
        app_identifier = params[:app_identifier]
        target_build_version = params[:version]

        latest_build_number = other_action.latest_testflight_build_number(
          app_identifier: app_identifier,
          version: target_build_version,
          initial_build_number: 0
        )

        return latest_build_number + 1

        # app = loginAndGetApp(app_identifier)

      #   try_count = 0
      #   max_tries = 100
      #   while try_count < max_tries do
      #     try_count += 1

      #     begin
      #       the_same_build = find_build_from_latest_version_candidate_builds(target_build_version, target_build_number, app)
      #       if the_same_build == nil
      #         return target_build_number
      #       end
      #       UI.message "Try number #{try_count}. Already uploaded build #{target_build_version} ##{target_build_number} found. Increase build number"
      #       target_build_number = target_build_number + 1
      #     rescue Exception => e
      #       UI.message "Try number #{try_count}. Exception occured while finding uploaded build"
      #       UI.message e.message
      #       UI.message e.backtrace.inspect
      #       if e.message == "Unauthorized Access"
      #         UI.message "Logged out. Try after loggin in again."
      #         app = loginAndGetApp(app_identifier)
      #       end
      #     end
      #   end

      #   UI.abort_with_message!("Failed to get build number")
      end

      #####################################################
      # @!group Documentation
      #####################################################

      def self.is_supported?(platform)
        [:ios].include?(platform)
      end

      def self.description
        'next_testflight_build_number'
      end

      def self.available_options
        [
          FastlaneCore::ConfigItem.new(key: :app_identifier,
                                       description: "The bundle identifier of your app",
                                       default_value: CredentialsManager::AppfileConfig.try_fetch_value(:app_identifier)),
          FastlaneCore::ConfigItem.new(key: :version,
                                       description: "The version number whose latest build number we want",
                                       optional: true)
        ]
      end

      def self.authors
        ['@jbseo', '@moonchang']
      end
    end
  end
end
