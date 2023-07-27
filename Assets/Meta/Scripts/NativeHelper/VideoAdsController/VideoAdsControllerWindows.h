#if GENERATED_PROJECT
#define PLUGIN_API __declspec(dllimport)
#else
#define PLUGIN_API __declspec(dllexport)
#endif

typedef void(_stdcall *action_str_t)(const wchar_t *);
typedef void(_stdcall *action_str_str_t)(const wchar_t *, const wchar_t *);
typedef bool(_stdcall *func_bool_action_str_t)(const wchar_t *);
typedef long(_stdcall *func_long_action_str_t)(const wchar_t *);

extern PLUGIN_API action_str_t initializeVideoAdsControllerFn;
extern PLUGIN_API action_str_t loadPlacementFn;
extern PLUGIN_API func_bool_action_str_t isVideoAdsAvailableFn;
extern PLUGIN_API action_str_str_t showRewardedVideoFn;
extern PLUGIN_API func_long_action_str_t getPlacementRewardFn;
