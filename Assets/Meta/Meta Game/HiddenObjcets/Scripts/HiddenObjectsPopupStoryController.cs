using System.Collections;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using System.Collections.Generic;
using System.Linq;

namespace BagelCode.HiddenObjects
{
    public class HiddenObjectsPopupStoryController : MonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private int sceneIndex = 0;

        private bool fromInGame;

        private bool isActive = false;
        private bool isLeftActive = false;
        private bool isRightActive = false;

        private List<CutSceneData> sceneDataList;

        private ContextElement messageElement;

        private ContextElement leftNameElement;
        private ContextElement leftCharacterAnchorElement;

        private ContextElement rightNameElement;
        private ContextElement rightCharacterAnchorElement;

        private Dictionary<string, GameObject> characterObjDict = new Dictionary<string, GameObject>();

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        public void InitProperty()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            sceneDataList = bb.GetValue<List<CutSceneData>>("sceneDataList");
            fromInGame = bb.GetValue<bool>("fromInGame");

            BlackboardUtils.SetOrCreateValue(bb, "sceneIndex", 0);
            BlackboardUtils.SetOrCreateValue(bb, "lastIndex", sceneDataList.Count - 1);

            anim.SetTrigger("Board Active");

            MetaContextElementUtils.SetClickable(root, gameObject, HiddenObjects.Events.ON_CLICK, false);

            // Bottom Text
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Bottom Text", "HIDDEN_OBJECTS_POPUP_STORY_TAP_TO_CONTINUE", CHILDREN);

            // Elements
            messageElement = ContextUtils.FindElement(root, "Story Area/Text", FULL);
            leftNameElement = ContextUtils.FindElement(root, "Left Character Area/Text Name", FULL);
            leftCharacterAnchorElement = ContextUtils.FindElement(root, "Left Character Area/Character Image Anchor", FULL);
            rightNameElement = ContextUtils.FindElement(root, "Right Character Area/Text Name", FULL);
            rightCharacterAnchorElement = ContextUtils.FindElement(root, "Right Character Area/Character Image Anchor", FULL);

            // Blur on
            if(fromInGame) BlurManager.SetBlur(true);
        }

        public IEnumerator PlayCoroutine()
        {
            var sceneData = sceneDataList[sceneIndex];
            bool isLeft = sceneData.position != "right";

            // Name
            string characterName = sceneData.name;
            bool isNameExist = !string.IsNullOrEmpty(characterName);
            MetaContextElementUtils.SetText(isLeft ? leftNameElement : rightNameElement, characterName);

            // Character
            string character = sceneData.character;

            foreach (var pair in characterObjDict)
            {
                if (pair.Key != character)
                {
                    pair.Value.SetActive(false);
                }
            }

            bool isCharacterExist = !string.IsNullOrEmpty(character);
            if (isCharacterExist)
            {
                if (characterObjDict.ContainsKey(character))
                {
                    TriggerAnimation(characterObjDict[character].GetComponent<Animator>(), "Speak");
                    characterObjDict[character].gameObject.SetActive(true);
                }
                else
                {
                    string bundle = HiddenObjects.Defines.CONTENTS_BUNDLE;
                    string asset = string.Format("Hidden Objects Story Character {0}", character);
                    Transform parent = isLeft ? leftCharacterAnchorElement.transform : rightCharacterAnchorElement.transform;

                    var characterObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
                    if(characterObj != null)
                    {
                        characterObjDict.Add(character, characterObj);

                        if (characterObj != null)
                        {
                            var charAnim = characterObj.GetComponent<Animator>();
                            TriggerAnimation(charAnim, "Speak");

                            OverrideSortingLayer[] overriders = characterObj.GetComponentsInChildren<OverrideSortingLayer>(true);
                            for (int i = 0; i < overriders.Length; ++i)
                                overriders[i].UpdateSortingLayer();

                            // Character Anim
                            string triggerAnim = sceneData.triggerAnim;
                            if (!string.IsNullOrEmpty(triggerAnim))
                            {
                                if(charAnim != null)
                                    charAnim.SetBool(triggerAnim, true);
                            }
                        }
                    }
                }
            }

            // Message
            string message = sceneData.message;
            MetaContextElementUtils.SetText(messageElement, message);

            // Message Anim
            if(!isNameExist)
            {
                if (isActive)
                {
                    anim.SetTrigger("Board Reappear");
                    anim.SetBool("Left Character Active", false);
                    anim.SetBool("Right Character Active", false);
                }
            }
            else if (isLeft)
            {
                if (isLeftActive)
                {
                    // anim.SetTrigger("Left Reappear");
                }
                else if(isRightActive)
                {
                    anim.SetBool("Right Character Active", false);
                    anim.SetBool("Left Character Active", true);
                }
                else
                {
                    anim.SetBool("Left Character Active", true);
                }
            }
            else // right
            {
                if (isRightActive)
                {
                    // anim.SetTrigger("Right Reappear");
                }
                else if (isLeftActive)
                {
                    anim.SetBool("Left Character Active", false);
                    anim.SetBool("Right Character Active", true);
                }
                else
                {
                    anim.SetBool("Right Character Active", true);
                }
            }

            isActive = true;
            isLeftActive = isLeft;
            isRightActive = !isLeft;

            BlackboardUtils.SetOrCreateValue(bb, "sceneIndex", ++sceneIndex);

            yield return new WaitForSeconds(0.5f);
        }

        private void TriggerAnimation(Animator anim, string key)
        {
            if(anim == null) return;
            anim.SetTrigger(key);
        }

        public void OnClose()
        {
            // Animator
            anim.SetTrigger("Board Close");
            anim.SetBool("Left Character Active", false);
            anim.SetBool("Right Character Active", false);
            PopupManager.Instance.Close(gameObject);
            // Blur off
            if (fromInGame) BlurManager.SetBlur(false);
            EventSender.SendCalleeCallback(gameObject);
        }
    }
}
