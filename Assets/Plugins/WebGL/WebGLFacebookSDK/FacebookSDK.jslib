// ALREADY DEFINED VARIABLES:
// 1) FACEBOOK_ID (FacebookSDK.js)
// 2) FACEBOOK_ACCESS_TOKEN (FacebookSDK.js)
// 3) FB (FacebookSDK.js)
// 4) gameInstance (index.html)

var FacebookSDKPlugin = {
    InitializeFacebook: function () {
        // assume initialized already
    },
    LoginFacebook: function (callbackObjectNamePtr, callbackMethodNamePtr) {
        var callbackObjectName = Pointer_stringify(callbackObjectNamePtr);
        var callbackMethodName = Pointer_stringify(callbackMethodNamePtr);
        var passLoginResult = function (facebookAccessToken) {
            var PROFILE_IMAGE_MIN_SIDE = 152 * 2; // see NativeHelper.cs for sync with other platforms
            var profileParam = { fields: 'picture.height(304).width(304){url,is_silhouette},gender,name,id,email,friends' };
            FB.api('/me', 'get', profileParam, function (profileResponse) {
                if (!profileResponse || profileResponse.error) {
                    SendMessage(callbackObjectName, callbackMethodName, 'Error');
                    return;
                }
                var loginResult = profileResponse;
                if (profileResponse.picture.data.is_silhouette) {
                    loginResult.picture = '';
                } else {
                    loginResult.picture = profileResponse.picture.data.url;
                }
                var friendIdObjList = profileResponse.friends.data;
                var friendIdList = [];
                for (var i = 0; i < friendIdObjList.length; ++i) {
                    friendIdList.push(friendIdObjList.id);
                }
                loginResult.friendList = friendIdList;
                loginResult.accessToken = facebookAccessToken;

                SendMessage(callbackObjectName, callbackMethodName, JSON.stringify(loginResult));
            });
        };
        var loginCallback = function (loginResponse) {
            if (!loginResponse.authResponse) {
                console.log('User cancelled login or did not fully authorize.');
                SendMessage(callbackObjectName, callbackMethodName, 'Error');
                return;
            }
            FACEBOOK_ID = loginResponse.authResponse.userID;
            FACEBOOK_ACCESS_TOKEN = loginResponse.authResponse.accessToken;

            passLoginResult(loginResponse.authResponse.accessToken);
        };
        FB.getLoginStatus(function (statusResponse) {
            if (statusResponse.status === 'connected') {
                FACEBOOK_ID = statusResponse.authResponse.userID;
                FACEBOOK_ACCESS_TOKEN = statusResponse.authResponse.accessToken;

                passLoginResult(statusResponse.authResponse.accessToken);
            } else if (statusResponse.status === 'not_authorized') {
                FB.login(loginCallback, {
                    scope: 'public_profile,user_friends,email'
                });
            } else {
                // the user isn't logged in to Facebook.
                SendMessage(callbackObjectName, callbackMethodName, 'Error');
            }
        });
    },
    ShareFacebook: function (callbackObjectNamePtr, callbackMethodNamePtr, linkUrlPtr) {
        var callbackObjectName = Pointer_stringify(callbackObjectNamePtr);
        var callbackMethodName = Pointer_stringify(callbackMethodNamePtr);
        var linkUrl = Pointer_stringify(linkUrlPtr);
        FB.ui({
            method: 'share',
            href: linkUrl,
        }, function (shareResponse) {
            if (!shareResponse || shareResponse.error_code) {
                SendMessage(callbackObjectName, callbackMethodName, 'Error');
                return;
            }
            SendMessage(callbackObjectName, callbackMethodName, 'Success');
        });
    },
    InviteFacebook: function (callbackObjectNamePtr, callbackMethodNamePtr, messagePtr, titlePtr) {
        var callbackObjectName = Pointer_stringify(callbackObjectNamePtr);
        var callbackMethodName = Pointer_stringify(callbackMethodNamePtr);
        var message = Pointer_stringify(messagePtr);
        var title = Pointer_stringify(titlePtr);

        FB.ui({
            method: 'apprequests',
            title: title,
            message: message,
            filters: ['app_non_users']
        }, function (apprequestsResponse) {
            if (!apprequestsResponse || apprequestsResponse.error_code) {
                SendMessage(callbackObjectName, callbackMethodName, 'Error');
                return;
            }
            SendMessage(callbackObjectName, callbackMethodName, 'Success');
        });
    },
    GetLoginCachedFacebookId: function () {
        var value = FACEBOOK_ID;
        var size = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(size);
        stringToUTF8(value, buffer, size);
        return buffer;
    },
    GetLoginCachedFacebookAccessToken: function () {
        var value = FACEBOOK_ACCESS_TOKEN;
        var size = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(size);
        stringToUTF8(value, buffer, size);
        return buffer;
    },
    PayWithProductIdFacebook: function (callbackObjectNamePtr, callbackMethodNamePtr, serverProductId, skuPtr, isSubscription) {
        var callbackObjectName = Pointer_stringify(callbackObjectNamePtr);
        var callbackMethodName = Pointer_stringify(callbackMethodNamePtr);
        var sku = Pointer_stringify(skuPtr);
        var startPay = function () {
            FB.ui({
                method: 'pay',
                action: 'purchaseiap',
                product_id: sku
            }, function (purchaseResponse) {
                if (!purchaseResponse || purchaseResponse.error_code) {
                    if (purchaseResponse) {
                        SendMessage(callbackObjectName, callbackMethodName, 'Error' + purchaseResponse.error_code);
                    } else {
                        SendMessage(callbackObjectName, callbackMethodName, 'Error' + 'INTERNAL_ERROR');
                    }
                    return;
                }
                var purchaseRecord = purchaseResponse;
                purchaseRecord.server_product_id = serverProductId;
                try {
                    if (typeof(Storage) !== "undefined") {
                        var key = 'purchaseHistory';
                        var value = JSON.stringify(purchaseRecord) + '\n';
                        var originalItem = localStorage.getItem(key);
                        var notNullOriginalItem = originalItem || "";
                        localStorage.setItem(key, notNullOriginalItem + value);
                    } else {
                        console.log("local storage not supported");
                    }
                } catch (err) {
                    console.log(err);
                }

                var purchaseResult = {
                    receipt: purchaseResponse.signed_request,
                    // pass purchase token and access token to signature param not to add additional params
                    signature: JSON.stringify({
                        access_token: FACEBOOK_ACCESS_TOKEN
                    })
                };
                var b64EncodeUnicode = function (str) {
                    // first we use encodeURIComponent to get percent-encoded UTF-8,
                    // then we convert the percent encodings into raw bytes which
                    // can be fed into btoa.
                    return btoa(encodeURIComponent(str).replace(/%([0-9A-F]{2})/g,
                        function toSolidBytes(match, p1) {
                            return String.fromCharCode('0x' + p1);
                        }));
                };
                var resultString = b64EncodeUnicode(JSON.stringify(purchaseResult));
                SendMessage(callbackObjectName, callbackMethodName, resultString);
            });
        };
        var checkLoginBeforePayThenStart = function () {
            FB.getLoginStatus(function (statusResponse) {
                if (statusResponse.status === 'connected') {
                    FACEBOOK_ID = statusResponse.authResponse.userID;
                    FACEBOOK_ACCESS_TOKEN = statusResponse.authResponse.accessToken;
                    startPay();
                } else if (statusResponse.status === 'not_authorized') {
                    FB.login(checkLoginBeforePayThenStart, {
                        scope: 'public_profile,user_friends,email'
                    });
                } else {
                    // the user isn't logged in to Facebook.
                    SendMessage(callbackObjectName, callbackMethodName, 'Error' + statusResponse.status);
                }
            });
        };

        if (gameInstance && gameInstance.SetFullscreen) {
            gameInstance.SetFullscreen(0);
        }
        checkLoginBeforePayThenStart();
    },
    ConsumeUnclaimedPurchaseFacebook: function (callbackObjectNamePtr, callbackMethodNamePtr) {
        // skip checkLoginBefore- function because this is called right after UpdateFacebookId.cs refresh login status
        var callbackObjectName = Pointer_stringify(callbackObjectNamePtr);
        var callbackMethodName = Pointer_stringify(callbackMethodNamePtr);

        var startConsumeUnclaimedPurchase = function () {
            FB.api('/app/purchases', 'get', {}, function (purchasesResponse) {
                if (!purchasesResponse || purchasesResponse.error) {
                    SendMessage(callbackObjectName, callbackMethodName, 'NO_UNCLAIMED_PURCHASE');
                    return;
                }
                var unconsumedPurchasesList = purchasesResponse['data'];
                if (unconsumedPurchasesList.length === 0) {
                    SendMessage(callbackObjectName, callbackMethodName, 'NO_UNCLAIMED_PURCHASE');
                    return;
                }
                var purchaseTokenList = [];
                for (var i = 0; i < unconsumedPurchasesList.length; ++i) {
                    purchaseTokenList.push(unconsumedPurchasesList[i]['purchase_token']);
                }
    
                // process only one unconsumed purchase for each call
                var targetPurchaseToken = purchaseTokenList[0];
                var serverProductId = null;
                var signedRequest = null;
                var productId = null;
                var purchaseHistory = "";
                if (typeof(Storage) !== "undefined") {
                    var key = 'purchaseHistory';
                    var item = localStorage.getItem(key);
                    purchaseHistory = item ? item : "";
                } else {
                    console.log("local storage not supported");
                }
                var purchaseRecordList = purchaseHistory ? purchaseHistory.trim().split('\n') : [];
                try {
                    for (var i = 0; i < purchaseRecordList.length; ++i) {
                        var purchaseRecord = JSON.parse(purchaseRecordList[i]);
                        var recordedServerProductId = purchaseRecord.server_product_id;
                        var recordedPurchaseToken = purchaseRecord.purchase_token;
                        var recordedSignedRequest = purchaseRecord.signed_request;
                        if (targetPurchaseToken == recordedPurchaseToken) {
                            serverProductId = recordedServerProductId;
                            signedRequest = recordedSignedRequest;
                            productId = purchaseRecord.product_id;
                            break;
                        }
                    }
                } catch (err) {
                    console.error(err);
                    if (typeof(Storage) !== "undefined") {
                        localStorage.removeItem('purchaseHistory');
                    }
                    FB.api('/' + targetPurchaseToken + '/consume', 'post', {}, function (consumeResponse) {
                        SendMessage(callbackObjectName, callbackMethodName, 'NO_MATCHING_UNCLAIMED_PURCHASE');
                    });
                    return;
                }
                if (serverProductId == null) {
                    FB.api('/' + targetPurchaseToken + '/consume', 'post', {}, function (consumeResponse) {
                        SendMessage(callbackObjectName, callbackMethodName, 'NO_MATCHING_UNCLAIMED_PURCHASE');
                    });
                    return;
                }

                var consumeUnclaimedResult = {
                    serverProductId: serverProductId,
                    receipt: signedRequest,
                    sku : productId,
                    // TODO: set signature to empty string after removing server-side consume
                    signature: JSON.stringify({
                        access_token: FACEBOOK_ACCESS_TOKEN
                    })
                };
                var b64EncodeUnicode = function (str) {
                    // first we use encodeURIComponent to get percent-encoded UTF-8,
                    // then we convert the percent encodings into raw bytes which
                    // can be fed into btoa.
                    return btoa(encodeURIComponent(str).replace(/%([0-9A-F]{2})/g,
                        function toSolidBytes(match, p1) {
                            return String.fromCharCode('0x' + p1);
                        }));
                };
                var resultString = b64EncodeUnicode(JSON.stringify(consumeUnclaimedResult));
                SendMessage(callbackObjectName, callbackMethodName, resultString);
                return;
            });
        };

        var checkLoginBeforeConsumeUnclaimedThenStart = function () {
            FB.getLoginStatus(function (statusResponse) {
                if (statusResponse.status === 'connected') {
                    FACEBOOK_ID = statusResponse.authResponse.userID;
                    FACEBOOK_ACCESS_TOKEN = statusResponse.authResponse.accessToken;
                    startConsumeUnclaimedPurchase();
                } else if (statusResponse.status === 'not_authorized') {
                    FB.login(checkLoginBeforeConsumeUnclaimedThenStart, {
                        scope: 'public_profile,user_friends,email'
                    });
                } else {
                    // the user isn't logged in to Facebook.
                    SendMessage(callbackObjectName, callbackMethodName, 'Error');
                }
            });
        };

        checkLoginBeforeConsumeUnclaimedThenStart();
    },
    ConsumePurchase: function (callbackObjectNamePtr, callbackMethodNamePtr, decodedSignedRequestPtr) {
        var callbackObjectName = Pointer_stringify(callbackObjectNamePtr);
        var callbackMethodName = Pointer_stringify(callbackMethodNamePtr);
        var decodedSignedRequest = Pointer_stringify(decodedSignedRequestPtr);
        var parsedSignedRequest = null;
        try {
            parsedSignedRequest = JSON.parse(decodedSignedRequest);
        } catch (err) {
            SendMessage(callbackObjectName, callbackMethodName, 'Fail');
            return;
        }
        FB.api('/' + parsedSignedRequest.purchase_token + '/consume', 'post', {}, function (consumeResponse) {
            if (!consumeResponse || consumeResponse.error) {
                SendMessage(callbackObjectName, callbackMethodName, 'Fail');
                return;
            }
            SendMessage(callbackObjectName, callbackMethodName, 'Success');
        });
    },
    LogAppEventFacebook: function (eventIdPtr) {
        var APP_EVENT_MAP = {
            'level_up:20': FB.AppEvents.EventNames.ACHIEVED_LEVEL,
            'slot_enter': FB.AppEvents.EventNames.VIEWED_CONTENT,
            'spin_funnel:100': FB.AppEvents.EventNames.SPENT_CREDITS
        };
        var eventId = Pointer_stringify(eventIdPtr);
        if (eventId in APP_EVENT_MAP) {
            var appEventName = APP_EVENT_MAP[eventId];
            FB.AppEvents.logEvent(appEventName);
        }
    },
    LogPurchaseFacebook: function (purchaseIdPtr, revenue) {
        var purchaseId = Pointer_stringify(purchaseIdPtr);
        FB.AppEvents.logPurchase(revenue, 'USD', {purchase_id: purchaseId});
    }
};

mergeInto(LibraryManager.library, FacebookSDKPlugin);
