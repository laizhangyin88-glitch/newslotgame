// ALREADY DEFINED VARIABLES:
// 1) function requestFbAuthBIEvent (BiEvent.js)
// 2) instantiateGame (index.html)
// 3) fixLoadingScreenWidthHeightCssProperties, startScreenResizeInterval (UnityProgress.js)
// 4) FACEBOOK_APP_ID (config.js)

// global variable to share with unity
var FACEBOOK_ID = '';
var FACEBOOK_ACCESS_TOKEN = '';

function initialLoginCallback(loginResponse) {
  if (!loginResponse.authResponse) {
    console.log('User cancelled login or did not fully authorize.');
    FB.login(initialLoginCallback, {
      scope: 'public_profile,user_friends,email',
      auth_type: 'rerequest'
    });
    return;
  }

  FACEBOOK_ID = loginResponse.authResponse.userID;
  FACEBOOK_ACCESS_TOKEN = loginResponse.authResponse.accessToken;
  requestFbAuthBIEvent();
  instantiateGame();
}

window.fbAsyncInit = function() {
  FB.init({
    appId            : FACEBOOK_APP_ID,
    autoLogAppEvents : true,
    xfbml            : true,
    version          : 'v14.0'
  });
  FB.AppEvents.activateApp();

  FB.Canvas.setUrlHandler(function (data) {
    // window.location = data.path;
  });

  FB.getLoginStatus(function(statusResponse) {
    if (!statusResponse || statusResponse.error) {
      alert("FATAL ERROR: failed to get login status");
      return;
    }

    if (statusResponse.status === 'connected') {
      // the user is logged in and has authenticated your
      // app, and response.authResponse supplies
      // the user's ID, a valid access token, a signed
      // request, and the time the access token
      // and signed request each expire
      FACEBOOK_ID = statusResponse.authResponse.userID;
      FACEBOOK_ACCESS_TOKEN = statusResponse.authResponse.accessToken;
      requestFbAuthBIEvent();
      instantiateGame();
    } else if (statusResponse.status === 'not_authorized') {
      // the user is logged in to Facebook,
      // but has not authenticated your app
      FB.login(initialLoginCallback, {
        scope: 'public_profile,user_friends,email'
      });
    } else {
      // the user isn't logged in to Facebook.
      console.log('status unknown. Ignore it. 2018/05/03');
      FB.login(initialLoginCallback, {
        scope: 'public_profile,user_friends,email'
      });
    }
  });
};

(function(d, s, id){
   var js, fjs = d.getElementsByTagName(s)[0];
   if (d.getElementById(id)) {return;}
   js = d.createElement(s); js.id = id;
   js.src = "https://connect.facebook.net/en_US/sdk.js";
   fjs.parentNode.insertBefore(js, fjs);
 }(document, 'script', 'facebook-jssdk'));

