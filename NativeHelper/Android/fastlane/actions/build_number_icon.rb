# Based on: https://gist.github.com/rgm/5377144
# Requires ImageMagick: `brew install imagemagick ghostscript`

module Fastlane
  module Actions

    class NoSourceFileError < ArgumentError; end

    class Icon
      attr_accessor :src, :dst
      def src= path
        if File.exist? path
          @src = path
        else
          raise NoSourceFileError, "no icon file at #{path}"
        end
      end
      def stamp_icon_with str
        `convert -background '#0008' -fill white -font Helvetica -density 300 -gravity Center -size #{banner_dims} caption:"#{str}" "#{@src}" +swap -gravity North -composite "#{@dst}"`
      end
      def banner_dims
        banner_ht = (dims[:ht] * 0.5).floor
        "#{dims[:wd]}x#{banner_ht}" # imagemagick-style format string
      end
      def dims
        return @dims if @dims
        data = `identify -format "%w %h" "#{@src}"`.strip.split.map(&:to_i)
        @dims = {:wd => data[0], :ht => data[1]}
      end
    end

    class BuildNumberIconAction < Action
      def self.run(params)
        if `which convert`.strip == "" || `which identify` == ""
          STDERR.puts "Warning: either 'convert' or 'identify' not found"
          return
        end
        icons = sh("find #{params[:res_dir]} -iname '#{params[:filename]}'")
        icons.each_line do |stem|
          begin
            source = stem.strip
            i = Icon.new
            i.src = source
            i.dst = "#{source}-edit"
            version = ENV["BUILD_NUMBER"] || "1234"
            i.stamp_icon_with "##{version}"
            FileUtils.cp i.dst, i.src
            FileUtils.rm i.dst
          rescue NoSourceFileError => msg
            STDERR.puts "Warning: #{msg}"
          end
        end
      end

      #####################################################
      # @!group Documentation
      #####################################################

      def self.description
        "Adds the build number to the Icons"
      end

      def self.available_options
        @options ||= [
          FastlaneCore::ConfigItem.new(key: :filename,
                                       env_name: "BUILD_NUMBER_FILENAME",
                                       short_option: "-f",
                                       description: "The name of the app icon file to modify",
                                       default_value: "ic_launcher.png"),
          FastlaneCore::ConfigItem.new(key: :res_dir,
                                       env_name: "BUILD_RESOURCE_DIR_BASEPATH",
                                       short_option: "-b",
                                       description: "The base directory path to search icon files",
                                       default_value: ".")
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
