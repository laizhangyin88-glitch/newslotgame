#if GENERATED_PROJECT
#define PLUGIN_API __declspec(dllimport)
#else
#define PLUGIN_API __declspec(dllexport)
#endif

typedef void (_stdcall *action_str_t)(const wchar_t *);
typedef void (_stdcall *action_str_str_str_t)(const wchar_t *, const wchar_t *, const wchar_t *);
typedef void (_stdcall *action_str_str_str_str_str_t)(const wchar_t *, const wchar_t *, const wchar_t *, const wchar_t *, const wchar_t *);
typedef void (_stdcall *action_t)();
typedef const wchar_t* (_stdcall* func_str_t)();

extern PLUGIN_API action_str_t initializeSocialFn;
extern PLUGIN_API action_str_t loginFBFn;
extern PLUGIN_API action_str_str_str_str_str_t shareFBFn;
extern PLUGIN_API action_str_str_str_t inviteFBFn;
extern PLUGIN_API action_t logoutFBFn;
extern PLUGIN_API func_str_t getFacebookAppIdFn;
extern PLUGIN_API func_str_t getFacebookAppIdNameFn;
extern PLUGIN_API func_str_t getFacebookIdFn;
extern PLUGIN_API func_str_t getFacebookAccessTokenFn;
