# coding: utf-8
module Fastlane
  module Actions
    class SetInfoPlistAction < Action
      def self.run(params)
        require 'plist'
        identifier_key = params[:key]
        folder = Dir['*.xcodeproj'].first
        info_plist_path = params[:plist_path]
        raise "Couldn't find info plist file at path '#{params[:plist_path]}'".red unless File.exist?(info_plist_path)
        plist = Plist.parse_xml(info_plist_path)
        plist[identifier_key] = params[:value]
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
        'Update Info.plist'
      end

      def self.available_options
        [
          FastlaneCore::ConfigItem.new(key: :plist_path,
                                       env_name: "FL_UPDATE_APP_IDENTIFIER_PLIST_PATH",
                                       description: "Path to info plist, relative to your Xcode project",
                                       verify_block: proc do |value|
                                         raise "Invalid plist file".red unless value[-6..-1].downcase == ".plist"
                                       end),
          FastlaneCore::ConfigItem.new(key: :key,
                                       env_name: 'FL_UPDATE_KEY',
                                       description: 'The key to set in Info.plist',
                                       verify_block: proc do |value|
                                          raise "No key".red unless (value and not value.empty?)
                                       end),
          FastlaneCore::ConfigItem.new(key: :value,
                                       env_name: 'FL_UPDATE_VALUE',
                                       description: 'The value to set in Info.plist',
                                       verify_block: proc do |value|
                                          raise "No value".red unless (value and not value.empty?)
                                       end)
        ]
      end

      def self.authors
        ['@snogar']
      end
    end
  end
end
