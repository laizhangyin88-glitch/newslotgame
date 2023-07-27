#if GENERATED_PROJECT
#define PLUGIN_API __declspec(dllimport)
#else
#define PLUGIN_API __declspec(dllexport)
#endif

typedef const wchar_t* (_stdcall* func_str_t)();
typedef void(_stdcall *action_t)();
typedef void(_stdcall *action_int_t)(int);
typedef void(_stdcall *action_str_t)(const wchar_t *);
typedef void(_stdcall *action_str_str_t)(const wchar_t *, const wchar_t *);
typedef void(_stdcall *action_str_double_str_t)(const wchar_t *, double, const wchar_t *);
typedef void(_stdcall *action_int_str_str_int_bool_t)(int, const wchar_t *, const wchar_t *, int, bool);
typedef void(_stdcall *action_str_str_str_t)(const wchar_t *, const wchar_t *, const wchar_t *);
typedef void(_stdcall *action_uint_str_t)(unsigned int, const wchar_t *);
typedef bool(_stdcall *func_bool)();

extern PLUGIN_API action_str_t initializeFn;
extern PLUGIN_API action_t registerForNotificationFn;
extern PLUGIN_API func_str_t getNotificationTokenFn;
extern PLUGIN_API func_str_t getDeviceIdFn;
extern PLUGIN_API func_str_t getServerBaseUrlFn;
extern PLUGIN_API func_str_t getChattingUrlFn;
extern PLUGIN_API func_str_t getAppDownloadUrlFn;
extern PLUGIN_API action_int_str_str_int_bool_t setLocalPushFn;
extern PLUGIN_API action_int_t deleteLocalPushFn;
extern PLUGIN_API action_t togglePushFn;
extern PLUGIN_API func_str_t getAdjustEnvFn;
extern PLUGIN_API func_str_t getAdjustAppTokenFn;
extern PLUGIN_API func_str_t getAdjustAppSecretFn;
extern PLUGIN_API func_str_t getAdjustInfo1Fn;
extern PLUGIN_API func_str_t getAdjustInfo2Fn;
extern PLUGIN_API func_str_t getAdjustInfo3Fn;
extern PLUGIN_API func_str_t getAdjustInfo4Fn;
extern PLUGIN_API action_uint_str_t getProfileImageFn;
extern PLUGIN_API func_bool isPinningAllowedFn;
extern PLUGIN_API action_str_t isPinnedFn;
extern PLUGIN_API action_str_t setPinFn;
extern "C" PLUGIN_API void _stdcall unitySendMessage(const wchar_t *object, const wchar_t *method, const wchar_t *arg);
