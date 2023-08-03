using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishSerialDrillBullet
    {
        GameObject gameObject;
        string FishTag = "Fish";
        string[] animParams = { "SerialDrill_Bullet_Move" };
        float currentTime = 0;
        float totalTime = 19;
        FishSerialDrillSkillItem serialDrillSkillItem;
        public int fishWidth = 120;
        public int fishHeight = 340;
        public bool isCanSeen = false;
        public bool isCanMove = false;
        public bool isCanDestroy = false;
        bool isPlayingAnim = false;
        Animator Bullet_SerialDrill_Anim;
        FishBehaviour fishBehavior;
        Transform transform;
        public SerialDrillBulletVo serialDrillBulletVo;
        public FishSerialDrillBullet(GameObject obj)
        {
            gameObject = obj;
            transform = gameObject.transform;
            FindView();
        }

        private void FindView()
        {
            Bullet_SerialDrill_Anim = transform.Find("Animator_Object").GetComponent<Animator>();
            fishBehavior = gameObject.GetComponent<FishBehaviour>();
            if (fishBehavior == null)
                fishBehavior = gameObject.GetComponent<FishBehaviour>();
            fishBehavior.onTriggerCallBack = OnTriggerEnter;
        }

        private void PlayAnim(string animName)
        {
            Bullet_SerialDrill_Anim.Play(animName, 0, 0);
        }

        public void ResetSerialDrillBulletVo(SerialDrillBulletVo vo)
        {
            serialDrillBulletVo = vo;
            isCanDestroy = false;
            isPlayingAnim = false;
        }

        public void ResetState(FishSerialDrillSkillItem skillItem)
        {
            serialDrillSkillItem = skillItem;
            int curPointIndex = serialDrillBulletVo.startPoint;
            fishBehavior.FishBeginMove(serialDrillBulletVo.traceId, 0, 0, fishWidth, fishHeight, curPointIndex, 0);
            fishBehavior.curFishStatus = FishBehaviour.FishStatus.Move;
            PlayAnim(animParams[0]);
            isPlayingAnim = true;
            currentTime = curPointIndex * 0.1f;
        }

        public void Update()
        {
            if (isPlayingAnim)
            {
                currentTime += Time.deltaTime;
                if (currentTime >= totalTime)
                {
                    currentTime = 0;
                    isCanDestroy = true;
                    isPlayingAnim = false;
                    fishBehavior.curFishStatus = FishBehaviour.FishStatus.Stop;
                }
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(FishTag))
            {
                FishFishBase hitFish = other.transform.parent.parent.GetComponent<FishBehaviour>().fishIns;
                if (hitFish != null)
                {
                    if (hitFish.GetIsDie())
                    {
                        return;
                    }
                    serialDrillSkillItem.SendHitFishInfo(hitFish.FishVo.UID, serialDrillBulletVo.serialDrillBulletID);
                }
            }
        }

        public void Destroy()
        {
            isCanDestroy = false;
            isPlayingAnim = false;
            fishBehavior.curFishStatus = FishBehaviour.FishStatus.Stop;
            gameObject.transform.localPosition = new Vector3(10000, 10000, 0);
        }
    }
}
