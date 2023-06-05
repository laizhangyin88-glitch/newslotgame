# Build Script for devices

## Build Scripts for Specific Platforms

### Common Prerequisites

* Unity 2018.3.8f1
  * Default Unity Binary is on /Applications/Unity/Unity.app
  * If you want to use another Unity Binary, set ``UNITY_PATH`` environment variable
  * Unity Android(Amazon) / iOS / WebGL / OSX Standalone build support (install what you want to build)

### Android

#### Dependencies
* Android SDK
  * https://developer.android.com/studio/index.html#downloads
  * Platform API 27 should be installed.
  * ``ANDROID_HOME`` environment variable is set.
  * JDK 1.8.0u101 or above (old document. need updates)
* Android NDK r16b
* Fastlane (optional)

#### Usage

*sample usage:*

- Generate Android project file for DEV on ``/hello/world``:
  ```bash
  bash generate_android_project.sh -o /hello/world DEV
  ```

Gradle project will be exported to path given by -o option. Then you can build the project with gradle or Fastlane.

### iOS

#### Dependencies

* Xcode 10.1
* Fastlane (optional)

#### Usage

- Generate iOS project file for DEV on ``/my/project/path``:
  ```
  bash generate_ios_project.sh -o /my/project/path DEV
  ```

Xcode proejct will be exported to path given by -o option. Then you can build the project with xcode or Fastlane.

### WebGL & OSX Standalone

The same as Android & iOS. Just execute `generate_*_project.sh` replacing * with your target platform.

WebGL and OSX Standalone do not need consequtive build processes after `generate_*_project.sh` ends.

**NOTE:** All other settings will be set by default, but some default options might be broken as `generate_*_project.sh` has not been managed for local build without options. See `generate_*_project.sh` and make adequate changes if error occurs.

## Local Build Support for Android & iOS

There has been needs for local build support, but `generate_*_project.sh` now has too many options to support local build and consecutive native build requires some dependencies and knowledges. Thus, I made scripts specially designed for local build support: `install_build_dependencies.sh`, `local_build_android.sh` and `local_build_ios.sh`

**NOTE:** These scripts are currently not well tested and need continous maintenance. Some bugs can occur while building and dependency installer might not cover all dependencies.

**NOTE2:** These scripts only supports **Mac OS**

### 1. `install_build_dependencies.sh`

If you execute this script, dependencies for build will be installed automatically. This script does not allow custom installation path and installer, and it forces dependencies to be installed in specific path or with specific installer to prevent users from being confused by too many options. Thus even when you already installed some dependency, they might be shown as not installed in this script. Also, dependency versions are haracoded that it needs to be continuously updated.

#### Usage
```bash
bash install_build_dependencies.sh
```

#### Target Dependencies
1. Unity (Installation Not Supported)
2. Xcode (Installation Not Supported)
3. JDK (Installation Not Supported)
4. Android SDK
5. Android NDK
6. Command Line Tools
7. Brew
8. Fastlane
9. 1Password CLI
10. jq
11. python requests library

**NOTE:** However, auto install for some dependencies has not been implemeneted yet: `Unity`, `Xcode` and `JDK`. These dependencies should be installed manually in specified path or by specific installer.

**TROUBLESHOOT:** If thes script claims some dependencies which you already installed are not installed, you should check how `install_build_dependencies.sh` script checks dependency installation and your current installation fit that.

### 2. `local_build_android.sh`

This script does not allow any options to prevent users from being confused by too much arguments for exports paths and build options. Just execute the script to get built apk files.

#### Prerequisite

- dependencies installed by `install_build_dependencies.sh` script
- access to `Vault-CV-Dev` to upload assetbundle (`Assetbundle Upload x-bypass-key`)

#### Build Process
1. Unity Build
2. Android Build
3. Upload Assetbundle (Optional)

#### Usage
```bash
bash local_build_android.sh
```

### 3. `local_build_ios.sh`

This script does not allow any options to prevent users from being confused by too much arguments for exports paths and build options. Just execute the script to get built ipa file.

#### Prerequisite

- dependencies installed by `install_build_dependencies.sh` script
- access to `Vault-Common-Dev` for match repository clone & Apple 2FA authentication (`Beta Build Machine SSH Private Key`)
- access to `Vault-Common-Dev` for match repository decryption (`Fastlane Match`)
- access to `Vault-Common-AppStore` for fastlane xcode build (`App Store Connect`)
- access to `Vault-CV-Dev` to upload assetbundle (`Assetbundle Upload x-bypass-key`)
- (Only for QA, PROD build) access to `Vault-Common-Dev` for fastlane testflight upload (`Apple app-specific password`)

#### Build Process
1. Unity Build
2. iOS Build
3. Upload Assetbundle (Optional)

#### Usage
```bash
bash local_build_ios.sh
```
