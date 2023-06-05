# coding: utf-8
module Fastlane
  module Actions
    class WaitForBuildAction < Action

      def self.find_build_from_all_builds_for_train(target_build_version, target_build_number, app)
        # begin
          matched_builds = app.all_builds_for_train(train: target_build_version)
          find_result = matched_builds.find { |build| build.build_version == target_build_number.to_s }
          return find_result
        # rescue Exception => e
        #   UI.message "Exception occured while finding uploaded build"
        #   UI.message e.message
        #   UI.message e.backtrace.inspect
        #   return nil
        # end
      end

      def self.report_status(build: nil)
        if build.nil?
          UI.message("Build doesn't show up in the build list anymore, waiting for it to appear again (check your email for processing issues if this continues)")
        elsif build.active?
          UI.success("Build #{build.train_version} - #{build.build_version} is already being tested")
        elsif build.ready_to_submit? || build.export_compliance_missing? || build.review_rejected?
          UI.success("Successfully finished processing the build #{build.train_version} - #{build.build_version}")
        else
          UI.message("Waiting for iTunes Connect to finish processing the new build (#{build.train_version} - #{build.build_version})")
        end
      end

      def self.loginAndGetApp(appIdentifier)
        Spaceship::Tunes::login()
        return Spaceship::Tunes::Application.find(appIdentifier)
      end

      def self.run(params)
        app_identifier = params[:app_identifier]
        target_version = params[:version]
        target_version_number = params[:version_number]

        app = loginAndGetApp(app_identifier)

        try_count = 0
        max_tries = 100
        while try_count < max_tries do
          try_count += 1

          begin
            build = find_build_from_all_builds_for_train(target_version, target_version_number, app)
            report_status(build: build)
            if build && build.processed?
              return build
            end
            UI.message "waiting for build #{target_version} ##{target_version_number} to be processed"
          rescue Exception => e
            UI.message "Try number #{try_count}. Exception occured while finding uploaded build"
            UI.message e.message
            UI.message e.backtrace.inspect
            if e.message == "Unauthorized Access"
              UI.message "Logged out. Try after loggin in again."
              app = loginAndGetApp(app_identifier)
            end
          end
          sleep 60
        end

        UI.abort_with_message!("Failed to get build from testflight.")
      end

      #####################################################
      # @!group Documentation
      #####################################################

      def self.is_supported?(platform)
        [:ios].include?(platform)
      end

      def self.description
        'wait_for_build'
      end

      def self.available_options
        [
          FastlaneCore::ConfigItem.new(key: :app_identifier,
                                       description: "The bundle identifier of your app",
                                       default_value: CredentialsManager::AppfileConfig.try_fetch_value(:app_identifier)),
          FastlaneCore::ConfigItem.new(key: :version,
                                       description: "The version whose latest build number we want"),
          FastlaneCore::ConfigItem.new(key: :version_number,
                                       type: Fixnum,
                                       description: "The version number whose latest build number we want")
        ]
      end

      def self.authors
        ['@jbseo', '@moonchang']
      end
    end
  end
end
