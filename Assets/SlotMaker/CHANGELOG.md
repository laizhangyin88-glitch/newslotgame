# Changelog
Please refer to the [link](http://doc.pages.bagelcode.com/slotmaker) for the complete document. 

## Guiding
* Changelogs are for humans, not machines. 
* There should be an entry for every single version.
* The same types of changes should be grouped.
* The latest version comes first. 
* The release date of each version is displayed. 

## Types of changes
* **Added** for new features.
* **Changed** for changes in existing functionality.
* **Deprecated** for soon-to-be removed features.
* **Removed** for now removed features.
* **Fixed** for any bug fixes.

## 2022.2.4

### Changed
* Fixed Duplicated throw exception handling in AssetBundleLoadOperation.cs

## 2022.2.3

### Added
* Added CanvasOrderChecker Tool

### Changed
* Fixed Assetbundle Loading Error
* Fixed Debug Spin Testsuite
* Fixed Duplicated throw exception handling

## 2022.2.2

### Added
* Added Meta Music Mixer.mixer
* Added ActionTask CalcClassicLineWin1

### Changed
* Fixed GSHandler.cs
* Fixed GSManager.cs
* Fixed Mixer.prefab
* Fixed Master Mixer.mixer
* Changed SymbolBehaviour to use cache
* Changed DebugSpin feature

## 2022.2.1

### Added
* Added CalcMixedLineWinWithMultiplier ActionTask
* Added CurveVelocity.cs ActionTask

### Changed
* Changed AssetBundleManager inProgressBundleOperations logic

## 2022.2.0

### Added
* New **Symbol Logic**
    * Replace SymbolController to SymbolEventHandler
    * Replace Symbol_FSM to SymbolBehaviour
    * Replace SymbolAsset to SymbolBehaviourAsset
* New **Feature Template**
    * Added FeatureController, FeatureModule, GamePopup
* Added HighestSymbolWinSoundView.cs
* Added InitializeSlotSymbol.cs

### Changed
* Update ApplyContentsStore to MetaSystem

## 2022.1.1

### Added
* Added ReplaceReelStripsByFinalOutputList.cs

### Changed
* Update FormatUntility.cs for increasing the range of SimpleNumber format
* Update DebugSpin window for resizing and filtering

## 2022.1.0

### Added
* Added SendEventWithGameObject.cs
* Added decryption flag to ContentsSerializer
* Added WheelSetupTools.cs

### Changed
* Unity updated to **2019.4.34f1(LTS)**
* Update DeepCopyAsset
* Update AssetDependencyChecker(migrated from lab repository)

### Fixed
* Fixed GameObjectId to be removed on clear and return to pool

## 2021.2.3

### Changed
* Apply threading task to LoadSymbolGraphs.cs

## 2021.2.2

### Added
* Added SetInt64Modulo
* OverrideRelativeMeshRendererSortingLayer
* OverrideRelativeSkeletonAnimationSortingLayer
* Added InternalEventRouter

### Changed
* Changed Popup.cs. Inherited IEventRouterDataObject.
* Changed PopupManager.cs. Use InternalEventRouter from PopupManager.

## 2021.2.1

### Changed
* Change UpdateExpectationSpotReelCustomOrder.cs for applying ignore option
* Refactor Keno logic for claim bonus

## 2021.2.0

### Added
* Added unity **Spine** package
* Added ContextTextTMPList
* Added CopyBlackboardToTargetGameObject
* Added LoadSymbolGraphs.ts

### Changed
* Change DeepCopyAssets for nested prefab

### Fixed
* Fixed UpdateExpectationSpotReelCustomOrder stop effect ignore condition

## 2021.1.6

### Added
* Added CalcNumberCombinationPerRowWin.ts
* Added CreateClassicSpotSlotMachine.ts

### Changes
* Added function StringTableObjects.CompleteImport. for post process.

## 2021.1.5

### Added
* Added MeshSymbolAsset and BaseSymbolAsset
* Added SymbolSetMaterialByIndex

## 2021.1.4

### Changes
* Change ContentsSerializer for decoding games server response

## 2021.1.3

### Added
* Added ReplaceableSpritePanel, ReplaceableSpriteImage

### Changed
* Changed Google Service Url
* Changed GoogleSheet.cs error logic

## 2021.1.2

### Changed
* Changed Analytics for applying keno AE(client_pick, client_auto_change)

### Added
* Added AE_keno_click_auto_change, AE_keno_pick
* Added CheckSpotEvent, Keno_GetSpotNumber

## 2021.1.1

### Fixed
* Fix UpdateExpectationFullStackReel continuous option

## [2021.1.0](https://git.bagelcode.com/v3-client/slotmaker/-/merge_requests/170)

### Added
* Released **KENO** feature
    * Added KenoInstance, BallInstance
    * Added KenoMediator with EventBehaviour
    * Added CustomKenoCommands

## 2019.4.26

### Added
* Added UpdateExpectationFullStackReel ActionTask
* Added MultiReelLayoutGroup and SetReelGroupOffsetList
* Added GetBlackboardValueUsingPath
* Added NonUniformPayLineEditorConfig

## 2019.4.25

### Added
* Added PayTableLineByCellSegment and interface
* Added CalcWinLineByCell ActionTask
* Add UpdateExpectationSpotReelCustomReel for customized spin order in spot reel game.

### Fixed
* Fixed NullReferenceException in the MetaSystem.SubscribeBackButton

## 2019.4.24

### Fixed
* Fixed SetContextSlider BBParameter editor inspector
* Added ReplaceCustomDatasOnReelStrip

## 2019.4.23

### Changed
* In UpdateExpectationStack, use last symbol in found stack instead of fixed row size.
* Add ignoreSymbolAttribute on CalcWayWin.

### Fixed
* Fixed NullReferenceException in the MetaSystem.UnSubscribeBackButton

## 2019.4.22

### Added
* Added VerticalReelMovement

### Changed
* Change ContentsManifest to refresh GameInfos
* Refactored deserialize schema map for non primitive
* Fix bundle deps ArgumentException

## 2019.4.21

### Fixed
* Fixed SymbolJackpotCoinBase to have public setter on coinCredit property

## 2019.4.20

### Added
* Added ContentsVersionManager
* Added `gameVersion` field in `contents` of EnterGame request body
* Added LoadContentsGameVersion action

## 2019.4.19

### Changed
* Update ContentsModels.txt
    * Add jackpotInfo to TicketedBonus schema
    * ContentsModels.json should be applied for each serviced apps

### Fixed
* Fixed ContentJackpotCredit that is not updated to target on first join
* Fixed SpriteImageMPB by reverting the commit that prematurely optimized MaterialPropertyBlock usage in that component.
* Fixed WebImageDownloader invalid cast exception.

## 2019.4.18

### Added
* Added Sprite-Default(Distortion) Shader and SpriteImageMPB component
* Added AnimationLinearWheelRandomCurvePicker Component

### Changed
* Changed SymbolJackpotCoinBase class and its subclasses (SymbolJackpotCoin, SymbolJackpotCoinDouble, MultipleSymbolJackpotCoinDouble)
    * Separated coin calculation and text setting
    * Added global symbol coin base bet that can override the base bet for SymbolJackpotCoin
    * Added coinCredit property for bounding it in Blackboard
* Added includeWild option in CalcScatterDiscontinousWayWin action

## 2019.4.17

### Added
* Added CheckSymbolWin action
* Added CalcScatterDiscontinuousWayWin action

### Changed
* Considering subsymbol to get multiplier in calc classic line win

### Fixed
* Fixed ChangePayLinesSortingLayer NodeCanvas action by adding agent type
* Fixed UpdateExpectationSpotBySubSymbol so it doesn't check subsymbol of Reject symbol.

## 2019.4.16

### Added
* Merged 2019.3.9
* Added CalcScatterAdjacentLineWin NodeCanvas action
* Added UpdateExpectationSpotAdjacentLine NodeCanvas action

### Fixed
* Fixed the legacy contents loader by ignoring child objects in a prefab, from SlotMakerGameObject.DestroyImmediateChildren()
* Fixed Multiple Weight Random Generator
* Revert previous changes in 2019.4.15
    * Added adjacent option to calc scatter line win
    * Added adjacent option to expecation spot line

## 2019.4.15

### Added
* Added GetReelStrip action
* Added SphereCollider class to the link.xml to make it not to be stripped (IFB support)
* Added SetReelStrip action
* Added Symbol Jackpot Coin that uses double instead of float
* Added WindZone and Projectile that receive forces from WindZones
* Added Inferno Ball shader that supports rim light and fresnel effect
* Added PauseEditor action for debugging
* Added MultipleWeightRandomGenerator that can have multiple probability lists and select one by its index

### Fixed
* Fixed update expectation spot line
* Fixed ContentBlackboardUtils.GetBonusResponse that can cause Null Ref Exception in the case that './spin' doesn't have a response

### Changed
* Added adjacent option to calc scatter line win
* Added adjacent option to expecation spot line
* Added Action Task calculating adjacent line win
* Refactored the SymbolJackpotCoin class to be extendable by inheritance and to be able to have different WeightRandomGenerator implementations

## 2019.4.14

### Fixed
* Fixed volatility schema
* Fixed web image texture leak(only fixed pre-download file image)

## 2019.4.13

### Changed
* Optimized duplicating win calls on OnTotalWin

### Fixed
* Fixed ContentJackpotCredit reset logic bug that isn't updated instantly at the bet change

## 2019.4.12

### Added
* Merged 2019.3.8 

## 2019.4.11

### Added
* Added Action Task sorting Win List by several options.

### Fixed
* Fixed PerSymbolWinSoundView bug that didn't work with multiple slots
* JackpotBoard Reset logic omitting newPrev calculation

### Changed
* Changed ReplaceReelStripWithCustomData component for handling local reelStrips object.

## 2019.4.10

### Added
* ContentsLoader now supports play mode in editor
* TMP_Patcher editor tool for migration from TMP 1.3.0 to TMP 2.1.0-preview8

## 2019.4.9

### Fixed
* Fixed EllipsisText bug in Unity 2019.2.21f1
* Revert auto release audio optimization code - Added to 2019.3.4

## 2019.4.8

### Changed
* Changed BgmSoundView component so that it can choose whether setting BGM null for not specified Bonus or not.
* Changed Symbol Pays Static Field component so that it can show a pay of symbols that has only 1 pay count

### Fixed
* Fixed Expandable Animation Linear Wheel bugs (2019.3.6)

## 2019.4.7

### Fixed
* Fixed SceneInfo bug that sometimes does not work.

## 2019.4.6

### Added
* ChangePayLinesSortingLayer action added
    * This action changes the sorting layer of every payline to the given one as an argument
    * https://git.bagelcode.com/v3-client/slotmaker/-/merge_requests/83

## 2019.4.5

### Added
* Merged 2019.3.3

### Fixed
* Editor-only using statements in ContentsManifest.cs are fixed
* Reverted TestSuiteEnterGameApp.cs to import Contents.json instead of ContentsManifest which is currently unstable

## 2019.4.4

### Added
* Merged 2019.3.1-2 

## 2019.4.3

### Changed
* Unity updated to 2019.2.19f1
* New Contents Loader is be now 'Verified'.

### Fixed
* Changing storedHash issue has been fixed.

## 2019.4.2

### Added
* Added CrashReport User Meta Data("SlotMaker Version", "Contents Version") for filtering CrashReports.

## 2019.4.1

### Changed
* [gameInfo.json format is changed](http://doc.pages.bagelcode.com/slotmaker/en/slotmaker/editor/contentsloader/).

## [2019.4.0](http://doc.pages.bagelcode.com/slotmaker/en/release-notes/2019.4/) - 2020.1.6

### Added
* [New Contents Loader](http://doc.pages.bagelcode.com/slotmaker/en/slotmaker/editor/contentsloader) is released.  

### Changed
* Unity updated to 2019.2.17f1
* NodeCanvas updated to 2.9.7

## 2019.3.9

### Added
* Copied SimplePlayGameSoundRandom action from CVS meta

## 2019.3.8

### Added
* Actions that split calculating and applying earn credit of symbol win list

## 2019.3.7

### Fixed
* Revert auto release audio optimization code - Added to 2019.3.4

## 2019.3.6

### Fixed
* Expandable wheel distance miscalculation and omitted segment fixed

## 2019.3.5

### Added
* ScreenUtils. GetScaleSize function is added. using for get perfect fit size.

## 2019.3.4

### Added 

* Unload unused audio clips for more than one minute 

## 2019.3.3

### Added
* HSL shader for UI Images is added
* Grayscale shaders for Sprites and UI Images are added
    * https://git.bagelcode.com/v3-client/slotmaker/merge_requests/81

### Changed
* Move shaders which were at Shaders/v4 into Packages/Shaders

## 2019.3.2

### Added
* SpotDirectWinHandler action added
    * This component acts like SpotWinHandler, but it introduces "Direct Win" symbol animation.
    * "Direct Win" symbol animation does not redirected to pivot symbols.
    * https://git.bagelcode.com/v3-client/slotmaker/merge_requests/79

## 2019.3.1

### Added
* LinkSuperStackingSlotSymbolSafe action added
    * This action checks slot border and prevents to link a symbol that is not
      in the slot machine currently.

### Changed
* When the SingleWin event is dispatched with the SymbolWin of `0` lineIndex,
  `WinLineBordersAdaptor.SingleWin` does nothing from now on.
    * It's because the zero lineIndex in SymbolWin means it is either a scatter
      win or a way win, in most cases. 

## [2019.3.0](http://doc.pages.bagelcode.com/slotmaker/en/release-notes/2019.3/) - 2020.1.6

### Added
* Preferred types added
    * SlotMaker.VariableAsset
    * SlotMaker.Slots.SlotMediator
    * SlotMaker.Slots.SpinOutput
    * SlotMaker.Slots.SpinOutputSubset
    * SlotMaker.Slots.SymbolRemap

### Changed
* AudioMixer is moved into SlotMaker.

## [2019.2](http://doc.pages.bagelcode.com/slotmaker/en/release-notes/2019.2/)

### Added
* Suport LZMA StreamingAssets build. If you want to use this, add USE_ASSETBUNDLE_FILECACHE define flag to your project defines. 
* OdinInspector added.

### Changed
* NodeCanvas updated to 2.9.2
* Split StringTable importer from StringTableObject. 

## [2019.1](http://doc.pages.bagelcode.com/slotmaker/en/release-notes/2019.1/)

### Changed
* [Restore Anima2D pre-build process](http://doc.pages.bagelcode.com/slotmaker/en/release-notes/2019.1/anima2d-pre-build-process)
* Unity updated to 2018.3.8f1
* ContextAniamtor is renamed to ContextAnimator
* IAnalytics.buyin interface is changed(string productId).
* IAnalytics.big_win_gamble interface is add.

### Deprecated
* LoadAssetBundles will be removed, use LoadAssetBundles1
* BlackboardUtils.CopyBlackboardVariables will be removed, use BlackboardUtils.CopyBlackboard
