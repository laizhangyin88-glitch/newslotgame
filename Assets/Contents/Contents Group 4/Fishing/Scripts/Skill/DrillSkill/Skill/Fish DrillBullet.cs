using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class DrillBulletVo
    {
        public int UID;
        public int startPoint;
        public int traceId;
    }

    public class FishDrillBullet
    {
        enum DrillBulletStats
        {
            DrillBullet_Normal = 1,
            DrillBullet_Rotation = 2,
            DrillBullet_Bomb = 3,
        }
        string FishTag = "Fish";
        string[] animParams = { "Drill_BulletNormal", "Drill_BulletRotation", "Drill_BulletlBomb" };
        DrillBulletStats currentDrillBulletStats = DrillBulletStats.DrillBullet_Normal;
        bool isPlayingAnim = false;
        float currentTime = 0;
        float normalTotalTime = 10;
        float RotationTotalTime = 2;
        float bombTotalTime = 1.6f;
        public Vector3 beginPos = Vector3.zero;
        FishDrillSkillItem drillSkillItem;
        public int fishWidth = 120;
        public int fishHeight = 340;
        public bool isCanSeen = false;
        public bool isCanMove = false;
        public bool isCanDestroy = false;
        bool isPlayHitAudio = false;
        Animator bullet_Drill_Anim;
        Collider boxCollider;
        FishBehaviour fishBehavior;
        GameObject gameObject;
        Transform transform;
        public DrillBulletVo drillBulletVo;
        public FishDrillBullet(GameObject obj)
        {
            gameObject = obj;
            transform = gameObject.transform;
            FindView();
        }

        private void FindView()
        {
            bullet_Drill_Anim = transform.Find("Animator_Object").GetComponent<Animator>();
            boxCollider = transform.GetComponent<Collider>();
            fishBehavior = gameObject.GetComponent<FishBehaviour>();
            if (fishBehavior == null)
                fishBehavior = gameObject.AddComponent<FishBehaviour>();
            fishBehavior.onTriggerCallBack = OnTriggerEnter;
        }

        private void PlayAnim(string animName)
        {
            bullet_Drill_Anim.Play(animName, 0, 0);
        }

        public void ResetDrillBulletVo(DrillBulletVo vo)
        {
            drillBulletVo = vo;
            isCanDestroy = false;
            isPlayingAnim = false;
            isPlayHitAudio = false;
        }

        public void ResetState(FishDrillSkillItem skillItem)
        {
            drillSkillItem = skillItem;
            int curPointIndex = drillBulletVo.startPoint;
            fishBehavior.FishBeginMove(drillBulletVo.traceId, 0, 0, fishWidth, fishHeight, curPointIndex, 0);
            fishBehavior.curFishStatus = FishBehaviour.FishStatus.Move;
            PlayAnim(animParams[0]);
            GetDrillStats(curPointIndex * 0.1f);
            isPlayingAnim = true;
            isPlayHitAudio = true;
            boxCollider.enabled = true;
        }

        public void GetDrillStats(float timeElapsed)
        {
            if (timeElapsed <= normalTotalTime)
            {
                PlayAnim(animParams[0]);
                currentTime = timeElapsed;
                currentDrillBulletStats = DrillBulletStats.DrillBullet_Normal;
            }
            else if (timeElapsed >= normalTotalTime && timeElapsed <= (normalTotalTime + RotationTotalTime))
            {
                PlayAnim(animParams[1]);
                currentTime = timeElapsed - normalTotalTime;
                currentDrillBulletStats = DrillBulletStats.DrillBullet_Rotation;
            }
        }

        public void BombDrill()
        {
            boxCollider.enabled = false;
            currentDrillBulletStats = DrillBulletStats.DrillBullet_Bomb;
            currentTime = 0;
            fishBehavior.curFishStatus = FishBehaviour.FishStatus.Stop;
            PlayAnim(animParams[2]);
            FishGameUIManager.Instance.SetShake(false);
            FishAudioManager.Instance.PlayNormalAudio(45);
        }

        public void Update()
        {
            if (isPlayingAnim)
            {
                if (currentDrillBulletStats == DrillBulletStats.DrillBullet_Normal)
                {
                    currentTime += Time.deltaTime;
                    if (currentTime >= normalTotalTime)
                    {
                        currentTime = 0;
                        PlayAnim(animParams[1]);
                        currentDrillBulletStats = DrillBulletStats.DrillBullet_Rotation;
                    }
                }
                else if (currentDrillBulletStats == DrillBulletStats.DrillBullet_Rotation)
                {
                    currentTime += Time.deltaTime;
                    if (currentTime >= RotationTotalTime)
                    {
                        currentTime = 0;
                    }
                }
                else if (currentDrillBulletStats == DrillBulletStats.DrillBullet_Bomb)
                {
                    currentTime += Time.deltaTime;
                    if (currentTime >= bombTotalTime)
                    {
                        currentTime = 0;
                        isPlayingAnim = false;
                        isCanDestroy = true;
                    }
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
                    PlayHitAudio();
                    ShowHitEffect();
                    drillSkillItem.SendHitFishInfo(hitFish.fishVo.UID);
                }
            }
        }

        public void ShowHitEffect()
        {

        }

        public void PlayHitAudio()
        {
            if (isPlayHitAudio)
            {
                isPlayHitAudio = false;
                FishAudioManager.Instance.PlayNormalAudio(44);
                AsyncActionUtils.DelayedAction(FishDrillSkillManager.Instance, 0.7f, () => { isPlayHitAudio = true; });
            }
        }

        public void Destroy()
        {
            isCanDestroy = false;
            isPlayingAnim = false;
            fishBehavior.curFishStatus = FishBehaviour.FishStatus.Stop;
            currentDrillBulletStats = DrillBulletStats.DrillBullet_Normal;
            gameObject.transform.localPosition = new Vector3(10000, 10000, 0);
        }

    }
}
