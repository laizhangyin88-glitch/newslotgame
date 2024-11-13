using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

    [Category("★ BagelCode/Utils")]
    public class MakeSlotThumbnail : ActionTask<Blackboard>
    {
        public BBParameter<string> gameTitleValue;
        public BBParameter<Transform> root;
        public BBParameter<string> parentName;

        public BBParameter<bool> isLong;
        public BBParameter<bool> useBG;

        public BBParameter<GameObject> saveAs;

        public BBParameter<bool> isDestroyPrevObject;

        protected override string info
        {
            get
            {
                return string.Format("{0} = Load Slot Thumbnail(Long = {1})", saveAs, isLong);
            }
        }

        protected override void OnExecute()
        {
            if(saveAs.value != null && isDestroyPrevObject.value)
                GameObject.Destroy(saveAs.value);

            var gameTitle = BlackboardUtils.FindVariable<string>(agent, gameTitleValue.value);

            //SLOT_THUMBNAIL_SQUARE

            if(gameTitle != null)
            {
                if(root.value == null)
                    root.value = agent.transform;

                saveAs.value = MetaIconUtils.MakeSlotImageObjectFromGameTitle(gameTitle.value, isLong.value, useBG.value, root.value, parentName.value);
            }
            else
            {
                saveAs.value = null;
                Debug.Log("GameTitle is Null");
            }


            EndAction();
        }
    }

}
