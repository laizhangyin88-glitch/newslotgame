package com.bagelcode.v3;

import android.net.Uri;
import android.os.Bundle;
import android.util.Log;

import com.facebook.AccessToken;
import com.facebook.CallbackManager;
import com.facebook.GraphRequest;
import com.facebook.GraphRequestBatch;
import com.facebook.GraphResponse;
import com.facebook.HttpMethod;
import com.facebook.login.LoginManager;
import com.facebook.login.LoginResult;
import com.facebook.FacebookCallback;
import com.facebook.FacebookException;
import com.facebook.FacebookSdk;
import com.facebook.share.Sharer;
import com.facebook.share.model.ShareLinkContent;
import com.facebook.share.model.GameRequestContent;
import com.facebook.share.widget.ShareDialog;
import com.facebook.share.widget.GameRequestDialog;
import com.google.zxing.common.StringUtils;
import com.unity3d.player.UnityPlayer;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.Map;
import java.util.Set;

import static java.lang.System.in;

public class FacebookManager
{
  private String unityObjectName = null;
  private String unityMethodName = null;

  private static FacebookManager _instance;
  public static FacebookManager Instance()
  {
    if (_instance == null)
      _instance = new FacebookManager();

    return _instance;
  }
  public CallbackManager GetCallbackManager()
  {
    return callbackManager;
  }

  CallbackManager callbackManager;
  ShareDialog shareDialog;
  GameRequestDialog gameRequestDialog;

  public static void Initialize(String name)   { FacebookManager.Instance().Init(name); }
  public static void LoginFB(String callback)  { FacebookManager.Instance().Login(callback);  }
  public static void LogoutFB()                { FacebookManager.Instance().Logout(); }
  public static void ShareFB(String linkUrl, String title, String description, String imageUrl, String encodedAction, String callback) { FacebookManager.Instance().Share(linkUrl, title, description, imageUrl, encodedAction, callback); }
  public static void GameRequestFB(String message, String title, String callback) { FacebookManager.Instance().GameRequest(message, title, callback); }
  public static String GetAccessTokenFB() { return FacebookManager.Instance().GetAccessToken(); }
  public static boolean HasPermissionFB(String permission) { return FacebookManager.Instance().HasPermission(permission); }
  public static void GetPublishPermissionFB(String callback) { FacebookManager.Instance().GetPublishPermission(callback); }

  public void Init(String name)
  {
    UnityPlayerActivity currentInstance = UnityPlayerActivity.CurrentInstance();
    if (currentInstance == null) {
      return ;
    }

    unityObjectName = name;

    callbackManager = CallbackManager.Factory.create();
    LoginManager.getInstance().registerCallback(callbackManager, loginCallback);

    shareDialog = new ShareDialog(currentInstance);
    shareDialog.registerCallback(callbackManager, shareCallback);

    gameRequestDialog = new GameRequestDialog(currentInstance);
    gameRequestDialog.registerCallback(callbackManager, gameRequestCallback);
  }

  public String parseURL(String url, Map<String, String> params)
  {
    Uri.Builder builder = Uri.parse(url).buildUpon();
    for (String key : params.keySet())
    {
      builder.appendQueryParameter(key, params.get(key));
    }
    return builder.build().toString();
  }

  public void Share(String linkUrl, String title, String description, String imageUrl, String encodedAction, String callback)
  {
    UnityPlayerActivity currentInstance = UnityPlayerActivity.CurrentInstance();
    if (currentInstance == null) {
      UnityPlayer.UnitySendMessage(unityObjectName, callback, "Error");
      return ;
    }
    unityMethodName = callback;

    Map<String, String> queryParams = new HashMap<String, String>();
    queryParams.put("image", imageUrl);
    queryParams.put("title", title);
    queryParams.put("description", description);

    if (encodedAction != null && !encodedAction.isEmpty())
    {
      queryParams.put("encoded_action", encodedAction);
    }

    String contentUrl = parseURL(linkUrl, queryParams);

    if (ShareDialog.canShow(ShareLinkContent.class))
    {
      ShareLinkContent content = new ShareLinkContent.Builder()
        .setContentUrl(Uri.parse(contentUrl))
        .build();

      shareDialog.show(content);
    }
    else
    {
      UnityPlayer.UnitySendMessage(unityObjectName, callback, "Error");
    }
  }

  public void Login(String callback)
  {
    UnityPlayerActivity currentInstance = UnityPlayerActivity.CurrentInstance();
    if (currentInstance == null) {
      UnityPlayer.UnitySendMessage(unityObjectName, callback, "Error");
      return ;
    }
    unityMethodName = callback;
    LoginManager.getInstance().logInWithReadPermissions(currentInstance, Arrays.asList("public_profile"));
  }

  public void Logout()
  {
    UnityPlayerActivity currentInstance = UnityPlayerActivity.CurrentInstance();
    if (currentInstance == null) {
      return ;
    }
    LoginManager.getInstance().logOut();
  }

  public void GameRequest(String message, String title, String callback)
  {
    UnityPlayerActivity currentInstance = UnityPlayerActivity.CurrentInstance();
    if (currentInstance == null) {
      UnityPlayer.UnitySendMessage(unityObjectName, callback, "Error");
      return ;
    }

    unityMethodName = callback;
    if (gameRequestDialog.canShow()) {
      GameRequestContent content = new GameRequestContent.Builder()
        .setMessage(message)
        .setTitle(title)
        .build();

      gameRequestDialog.show(currentInstance, content);
    }
    else
    {
      UnityPlayer.UnitySendMessage(unityObjectName, unityMethodName, "Error");
    }
  }

  public String GetAccessToken()
  {
    UnityPlayerActivity currentInstance = UnityPlayerActivity.CurrentInstance();
    if (currentInstance == null) {
      return "";
    }

    AccessToken token = AccessToken.getCurrentAccessToken();
    if (token != null && !token.isExpired())
      return token.getToken();

    return "";
  }

  public boolean HasPermission(String permission)
  {
    Set<String> permissionList = AccessToken.getCurrentAccessToken().getPermissions();

    for (String ownedPermission : permissionList)
    {
      if (ownedPermission.equals(permission))
      {
        return true;
      }
    }

    return false;
  }

  public void GetPublishPermission(String callback)
  {
    UnityPlayerActivity currentInstance = UnityPlayerActivity.CurrentInstance();
    if (currentInstance == null) {
      UnityPlayer.UnitySendMessage(unityObjectName, callback, "Error");
      return ;
    }

    unityMethodName = callback;
    LoginManager.getInstance().logInWithPublishPermissions(currentInstance, Arrays.asList(("publish_actions")));
  }

  FacebookCallback<GameRequestDialog.Result> gameRequestCallback = new FacebookCallback<GameRequestDialog.Result>() {
    @Override
    public void onSuccess(GameRequestDialog.Result result) {
      UnityPlayer.UnitySendMessage(unityObjectName, unityMethodName, "Success");
      // Log.d("gameRequestCallback", result.toString());
    }

    @Override
    public void onCancel() {
      UnityPlayer.UnitySendMessage(unityObjectName, unityMethodName, "Cancel");
      // Log.d("gameRequestCallback", "onCancel");
    }

    @Override
    public void onError(FacebookException error) {
      UnityPlayer.UnitySendMessage(unityObjectName, unityMethodName, "Error");
      // Log.d("gameRequestCallback", error.toString());
    }
  };

  FacebookCallback<Sharer.Result> shareCallback = new FacebookCallback<Sharer.Result>()
  {
    @Override
    public void onSuccess(Sharer.Result result)
    {
      UnityPlayer.UnitySendMessage(unityObjectName, unityMethodName, "Success");
      // Log.d("shareCallback", result.toString());
    }

    @Override
    public void onCancel()
    {
      UnityPlayer.UnitySendMessage(unityObjectName, unityMethodName, "Error");
      // Log.d("shareCallback", "onCancel");
    }

    @Override
    public void onError(FacebookException error)
    {
      UnityPlayer.UnitySendMessage(unityObjectName, unityMethodName, "Error");
      // Log.d("shareCallback", error.toString());
    }
  };

  FacebookCallback<LoginResult> loginCallback = new FacebookCallback<LoginResult>()
  {
    @Override
    public void onSuccess(LoginResult loginResult)
    {
      // App code
      // Log.d("loginCallback", "onSuccess");

      final JSONObject json = new JSONObject();

      // newMeRequest
      GraphRequest newMeRequest = GraphRequest.newMeRequest
        (
          loginResult.getAccessToken(),
          new GraphRequest.GraphJSONObjectCallback()
          {
            @Override
            public void onCompleted(JSONObject object, GraphResponse response)
            {
              if (response.getError() == null)
              {
                //  Log.d("GraphRequest", response.toString());
                try
                {
                  json.put("id", object.getString("id"));
                }
                catch (JSONException e)
                {
                  UnityPlayer.UnitySendMessage(unityObjectName, unityMethodName, "Error");
                  e.printStackTrace();
                }
              }
              else
              {
                Log.e("GraphRequest", "Error in Response "+ response);
                UnityPlayer.UnitySendMessage(unityObjectName, unityMethodName, "Error");
              }
            }
          }
        );
      Bundle parameters = new Bundle();
      parameters.putString("fields", "id");
      newMeRequest.setParameters(parameters);

      // batch newMe and newFriend request
      GraphRequestBatch batch = new GraphRequestBatch(newMeRequest);
      batch.addCallback(new GraphRequestBatch.Callback()
      {
        @Override
        public void onBatchCompleted(GraphRequestBatch graphRequests)
        {
          // Log.d("batchCompleted", graphRequests.toString());
          // Log.d("Json", json.toString());
          UnityPlayer.UnitySendMessage(unityObjectName, unityMethodName, json.toString());
          // Application code for when the batch finishes
        }
      });

      batch.executeAsync();
    }
    @Override
    public void onCancel()
    {
      // App code
      LoginManager.getInstance().logOut();
      // Log.d("loginCallback", "onCancel");
      UnityPlayer.UnitySendMessage(unityObjectName, unityMethodName, "Error");
    }
    @Override
    public void onError(FacebookException exception)
    {
      // App code
      // Log.d("loginCallback", "onError");
      // Log.d("loginCallback", exception.toString());

      // if (exception instanceof FacebookAuthorizationException) {
        // if (AccessToken.getCurrentAccessToken() != null) {
          LoginManager.getInstance().logOut();
        // }
      // }

      UnityPlayer.UnitySendMessage(unityObjectName, unityMethodName, "Error");
    }
  };
}
