using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using SlotMaker;
using SlotMaker.Json;
using NodeCanvas.Framework;
using System.Text.RegularExpressions;

namespace BagelCode
{
    public class PendingActionUriOperator
    {
        public System.Uri uri;

        public string origUri;
        public string scheme;
        public string host;
        public string query;

        public bool ParseUri(string strUri)
        {
            try
            {
                System.Uri uri = new System.Uri(strUri);
                scheme = uri.Scheme;

                if(ProductSettings.Instance.deeplinkUriScheme != scheme) return false;

                origUri = strUri;
                host = uri.Authority;
                query = uri.Query;
                if (query.Length > 0)
                    query = query.Substring(1);

                if(string.IsNullOrEmpty(host)) return false;
                if(string.IsNullOrEmpty(query)) return false;

                Debug.Log("OpenUrl scheme:" + scheme + ", host:" + host + ", query:" + query);
            }
            catch (System.UriFormatException)
            {
                Debug.Log("OpenUrl error:UriFormatException");
                return false;
            }

            return true;
        }

        public void Operate()
        {
            if(string.IsNullOrEmpty(query)) return;

            if(host == "adjust")
            {
                OperateAction();
            }
            else if(host == "email_validation")
            {
                OperateEmailVerification();
            }
            else if(host == "vipsurvey")
            {
                OperatorVipSurvey();
            }
        }

        private void OperateAction()
        {
            GameObject deepLinkGO = GameObject.Find("DeepLink");

            string[] queryParams = query.Split('&');
            string key = "action";

            for (int i = 0; i < queryParams.Length; ++i)
            {
                int index = 0;
                index = queryParams[i].IndexOf('=', index);

                if (index < 0) continue;

                string keyString  = queryParams[i].Substring(0, index);
                string dataString = queryParams[i].Substring(index+1);

                //urldecoding for facebook
                // dataString = dataString.Replace("%3D", "=").Replace("20%", "+").Replace("%2F", "/");
                dataString = UnityEngine.WWW.UnEscapeURL(dataString, System.Text.Encoding.UTF8).Replace(" ", "+");
                if (keyString.Equals(key) && !string.IsNullOrEmpty(dataString))
                {
                    Debug.Log(keyString);
                    Debug.Log(dataString);

                    byte[] parsedData = System.Convert.FromBase64String(dataString);   
                    Action action = Action.Deserialize(parsedData);

                    if(action.type == ActionType.UNKNOWN || action.type == ActionType.NONE) 
                        break;

                    var deeplinkBB = BlackboardUtils.GetOrCreateBlackboard(deepLinkGO.GetComponent<Blackboard>() as IBlackboard, "_action");
                    ClientAPI2Blackboard.Serialize(deeplinkBB, action);

                    var uriVariable = BlackboardUtils.GetOrCreateVariable<string>(deeplinkBB, "uri");
                    uriVariable.value = origUri;

                    var e = new ParadoxNotion.EventData("DoAction");
                    deepLinkGO.GetComponent<NodeCanvas.Framework.GraphOwner>().SendEvent(e, null);
                }
            }
        }

        private void OperateEmailVerification()
        {
            string[] queryParams = query.Split('&');
            string key = "code";
            for (int i = 0; i < queryParams.Length; ++i)
            {
                int index = 0;
                index = queryParams[i].IndexOf('=', index);

                if (index < 0) continue;

                string keyString  = queryParams[i].Substring(0, index);
                string dataString = queryParams[i].Substring(index+1);
                
                if(!string.IsNullOrEmpty(dataString))
                    dataString = dataString.Trim();

                if (keyString.Equals(key) && !string.IsNullOrEmpty(dataString))
                {
                    var e = new ParadoxNotion.EventData<string>("OnFillEmailVerificationCode", dataString);
                    MessageDispatcher.Dispatch("OnMetaUIEvent", e);
                    break;
                }
            }
        }

        private void OperatorVipSurvey()
        {
            string[] queryParams = query.Split('&');
            string key = "action";
            for (int i = 0; i < queryParams.Length; ++i)
            {
                int index = 0;
                index = queryParams[i].IndexOf('=', index);

                if (index < 0) continue;

                string keyString  = queryParams[i].Substring(0, index);
                string dataString = queryParams[i].Substring(index+1);
                
                if(!string.IsNullOrEmpty(dataString))
                    dataString = dataString.Trim();

                if (keyString.Equals(key) && dataString.Equals("success"))
                {
                    EventSender.SendGlobalEvent("OnSuccessVipSurvey");
                }
            }
        }
    }
}
