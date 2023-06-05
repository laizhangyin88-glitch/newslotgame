# coding: utf-8
module Fastlane
  module Actions
    class FacebookPlistHotfixAction < Action
      def self.run(params)
        require 'plist'
        info_plist_path = params[:plist_path]
        raise "Couldn't find info plist file at path '#{params[:plist_path]}'".red unless File.exist?(info_plist_path)
        plist = Plist.parse_xml(info_plist_path)

        facebookAppId = params[:fb_app_id]
        plist['FacebookAppID'] = facebookAppId
        
        fbCFBundleURLSchemes = 'fb' + facebookAppId
        cfBundleUrlTypes = plist['CFBundleURLTypes']
        # append deep link part
        if cfBundleUrlTypes.nil?
          plist['CFBundleURLTypes'] = [
            {'CFBundleURLName' => '$(PRODUCT_BUNDLE_IDENTIFIER)',
              'CFBundleURLSchemes' => [fbCFBundleURLSchemes, params[:url_scheme]]}
            
          ]      
        else
          if cfBundleUrlTypes.any? { |i| !(i['CFBundleURLSchemes'].nil?) && i['CFBundleURLSchemes'].include?(fbCFBundleURLSchemes) }
            UI.message "CFBundleURLSchemes is already properly set."
            return
          else
            # TBD: has to append to existing CFBundleURLSchemes element?
            cfBundleUrlTypes.push({'CFBundleURLSchemes' => [fbCFBundleURLSchemes]})
          end
        end
        plist_string = Plist::Emit.dump(plist)
        File.write(info_plist_path, plist_string)
        UI.message "Updated #{params[:plist_path]} 💾.".green
      end

      #####################################################
      # @!group Documentation
      #####################################################

      def self.is_supported?(platform)
        [:ios].include?(platform)
      end

      def self.description
        'Update an app name'
      end

      def self.available_options
        [
          FastlaneCore::ConfigItem.new(key: :plist_path,
                                       env_name: "FL_UPDATE_APP_IDENTIFIER_PLIST_PATH",
                                       description: "Path to info plist, relative to your Xcode project",
                                       verify_block: proc do |value|
                                         raise "Invalid plist file".red unless value[-6..-1].downcase == ".plist"
                                       end),
          FastlaneCore::ConfigItem.new(key: :fb_display_name,
                                       env_name: "FL_UPDATE_FB_DISPLAY_NAME",
                                       description: "FB Display name"),
          FastlaneCore::ConfigItem.new(key: :fb_app_id,
                                       env_name: "FL_UPDATE_FB_APP_ID",
                                       description: "FB App ID"),
          FastlaneCore::ConfigItem.new(key: :url_scheme,
                                       env_name: "FL_UPDATE_URL_SCHEME",
                                       description: "Url scheme")
        ]
      end

      def self.authors
        ['@jbseo']
      end
    end
  end
end
