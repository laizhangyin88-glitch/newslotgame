# coding: utf-8
module Fastlane
  module Actions
    class UpdateIconFilesAction < Action
      def self.run(params)
        require 'plist'
        identifier_key = 'CFBundleIconFiles'
        folder = Dir['*.xcodeproj'].first
        info_plist_path = params[:plist_path]
        raise "Couldn't find info plist file at path '#{params[:plist_path]}'".red unless File.exist?(info_plist_path)
        plist = Plist.parse_xml(info_plist_path)
        appIconFiles = plist[identifier_key]
        if appIconFiles.nil?
          UI.message "No Icon File Setting found on #{params[:plist_path]} .".green
          return
        end
        plist[identifier_key] = appIconFiles.map { |fileName| fileName.sub(/^AppIcon(\d)/, "AppIcon" + params[:icon_name_suffix] + "\\1") }
        plist_string = Plist::Emit.dump(plist)
        File.write(info_plist_path, plist_string)
        UI.message "Updated #{params[:plist_path]} .".green
      end

      #####################################################
      # @!group Documentation
      #####################################################

      def self.is_supported?(platform)
        [:ios].include?(platform)
      end

      def self.description
        'Update CFBundleIconFiles'
      end

      def self.available_options
        [
          FastlaneCore::ConfigItem.new(key: :plist_path,
                                       env_name: "FL_UPDATE_APP_IDENTIFIER_PLIST_PATH",
                                       description: "Path to info plist, relative to your Xcode project",
                                       verify_block: proc do |value|
                                         raise "Invalid plist file".red unless value[-6..-1].downcase == ".plist"
                                       end),
          FastlaneCore::ConfigItem.new(key: :icon_name_suffix,
                                       env_name: 'FL_UPDATE_ICON_NAME_SUFFIX',
                                       description: 'App IconSet name',
                                       verify_block: proc do |value|
                                          raise "No icon_name_suffix".red unless (value and not value.empty?)
                                       end)
        ]
      end

      def self.authors
        ['@jbseo']
      end
    end
  end
end
