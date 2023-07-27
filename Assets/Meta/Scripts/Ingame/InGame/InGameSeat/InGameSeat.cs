using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ParadoxNotion;
using ParadoxNotion.Services;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{

public class InGameSeat : MonoBehaviour
{
    public int seatIndex;
    public string userId;
    public bool seatEnable;
    public bool seatEventEnable;
    
    public Button emptyButton;
    public Button seatButton;
    public Button likeButton;
    public Blackboard seatBlackboard;
    public Animator seatAnimator;
    public Transform eventAnchor;
    public Transform eventList;
    public Transform likeAnchor;

    private static readonly int ANIMATOR_ACTIVATE = Animator.StringToHash("Activate");
    
    private bool _interactable = true;
    public bool interactable 
    {
        set 
        {
            _interactable = value;
            UpdateButtonInteractable();
        }
    }

    private bool _likeButtonInteractable = true;
    public bool likeButtonInteractable 
    {
        set 
        {
            _likeButtonInteractable = value;
            UpdateButtonInteractable();
        }
    }

    private bool seatLikeOnce = false;

    private void Awake()
    {
        seatIndex = -1;
    }

    private void Start()
    {
        var seatButtonElement = seatButton.gameObject.GetComponent<ContextElement>();
        seatButtonElement.UpdateContext(false);

        var likeButtonElement = ContextUtils.FindElement(seatButtonElement, "Button Area/Button Like Seat", ContextSearchingType.FullNameSearch);
        MetaContextElementUtils.SimpleSetText(likeButtonElement, "Text", StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_LIKE"));

        likeButton = likeButtonElement.gameObject.GetComponent<Button>();
        likeButton.onClick.AddListener(OnLike);
    }

    public void BeginSeatEvent()
    {
        seatEventEnable = false;
        likeButtonInteractable = true;
        // UpdateButtonInteractable();
    }

    public void EndSeatEvent()
    {
        seatEventEnable = true;
        likeButtonInteractable = false;
        // UpdateButtonInteractable();
        seatAnimator.SetBool("IsLikeButton", false);
    }

    public void UpdateButtonInteractable()
    {
        bool buttonInteractable = _interactable && seatEventEnable;
        emptyButton.interactable = buttonInteractable;
        seatButton.interactable = buttonInteractable;

        if(likeButton != null)
            likeButton.interactable = _likeButtonInteractable;
    }

    private void OnEnable()
    {
        StartCoroutine(UpdateEventListCo());
    }
    
    public void PushSeatEvent(Blackboard bb)
    {
        var newBB = (Blackboard)BlackboardUtils.CreateBlackboard("seatEvent");
        BlackboardUtils.CopyBlackboardVariables(bb, newBB);
        newBB.transform.parent = eventList;
        newBB.transform.SetAsLastSibling();
    }

    public void ClearSeatEvent()
    {
        eventList.gameObject.DestroyChildren();
        eventAnchor.gameObject.DestroyChildren();
        likeAnchor.gameObject.DestroyChildren();

        EndSeatEvent();
        seatAnimator.SetBool(ANIMATOR_ACTIVATE, true);
        seatAnimator.SetBool("IsLikeButton", false);
    }
    
    private IEnumerator UpdateEventListCo()
    {
        while (true)
        {
            if (seatEnable && seatEventEnable && eventList.childCount > 0)
            {
                BeginSeatEvent();

                Blackboard eventData = eventList.GetChild(0).GetComponent<Blackboard>();
                
                string assetName;
                var eventType = eventData.GetValue<PollType>("__event__");
                if (eventType == PollType.WIN)
                    assetName = string.Format("Seat Event {0} {1} Scene", eventType, eventData.GetValue<WinType>("winType"));
                else 
                    assetName = string.Format("Seat Event {0} Scene", eventType);
                
                GameObject loadedScene = null;
                yield return StartCoroutine(SceneUtils.LoadSceneAsync(MetaStringDefine.LOBBY_BUNDLE_NAME, assetName, eventAnchor, true, 
                    (result) => { loadedScene = result; }));

                if (!seatEventEnable)
                {
                    var sceneBB = loadedScene.GetComponent<Blackboard>();
                    sceneBB.SetValue("userProfile", seatBlackboard.GetValue<Blackboard>("userProfile"));
                    sceneBB.SetValue("eventData", eventData);
                    eventData.transform.parent = loadedScene.transform;
                    sceneBB.SetValue("seatAnimator", seatAnimator);
                    loadedScene.SetActive(true);
                }
                else// Clear
                {
                    GameObject.Destroy(loadedScene);
                    EndSeatEvent();
                }
            }
            
            yield return new WaitForSeconds(0.2f);
        }
    }
    
    public void JoinUser()
    {
        StartCoroutine(JoinUserCo());
    }
    
    private IEnumerator JoinUserCo()
    {
        BeginSeatEvent();
        
        string assetName = "Seat Event Join Scene";
        
        GameObject loadedScene = null;
        yield return StartCoroutine(SceneUtils.LoadSceneAsync(MetaStringDefine.LOBBY_BUNDLE_NAME, assetName, eventAnchor, true, 
            (result) => { loadedScene = result; }));
            
        if (!seatEventEnable)
        {
            var sceneBB = loadedScene.GetComponent<Blackboard>();
            sceneBB.SetValue("userProfile", seatBlackboard.GetValue<Blackboard>("userProfile"));
            sceneBB.SetValue("seatAnimator", seatAnimator);
            loadedScene.SetActive(true);
        }
        else// Clear
        {
            GameObject.Destroy(loadedScene);
            EndSeatEvent();
        }
    }

    public void OnLike()
    {
        if(_likeButtonInteractable)
        {
            // var userProfileBB = seatBlackboard.GetValue<Blackboard>("userProfile");
            // Debug.LogError(userProfileBB);
            // if(userProfileBB != null)
            if(!string.IsNullOrEmpty(userId))
            {
                likeButtonInteractable = false;

                // Send Like
                BagelCodeClientAPI.UserLike(userId, null, null);
                // Send BI
                Analytics.CustomEvent("client_like", new Dictionary<string, object>
                {
                    { "type", "in_game" },
                    { "target_user_id", userId },
                    { "slot_enter_context_id", BiEventUtils.GetSlotEnterContextID() }
                });

                MessageDispatcher.Dispatch("OnMetaUIEvent", new EventData<string>("SeatLike", userId));
            }
        }
    }
}

}
