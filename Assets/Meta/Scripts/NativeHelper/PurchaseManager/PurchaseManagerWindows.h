#if GENERATED_PROJECT
#define PLUGIN_API __declspec(dllimport)
#else
#define PLUGIN_API __declspec(dllexport)
#endif

typedef void(_stdcall *action_str_t)(const wchar_t *);
typedef void(_stdcall *action_str_str_t)(const wchar_t *, const wchar_t *);
typedef void(_stdcall *action_str_int_str_t)(const wchar_t *, int, const wchar_t *);
typedef void(_stdcall *action_str_str_str_t)(const wchar_t *, const wchar_t *, const wchar_t *);

extern PLUGIN_API action_str_t initializePurchaseFn;
extern PLUGIN_API action_str_int_str_t purchaseFn;
extern PLUGIN_API action_str_t consumeUnclaimedPurchaseFn;
extern PLUGIN_API action_str_str_t reportConsumableProductsAsFulfilledFn;
extern PLUGIN_API action_str_str_str_t getAndUpdateMicrosoftStoreIdKeyFn;
