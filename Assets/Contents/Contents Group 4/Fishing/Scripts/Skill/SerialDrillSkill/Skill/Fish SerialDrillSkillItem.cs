using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishSerialDrillSkillItem
    {
        string Skill_SerialDrill_Res = "Skill_SerialDrill";
        string Gun_SerialDrill_Res = "Gun_SerialDrill";
        string Bullet_SerialDrill_Res = "Bullet_SerialDrill";
        string[] AnimParams = { "GunSerialDrill_Idle", "GunSerialDrill_Shoot", "GunSerialDrill_Bomb" };
        Vector3 beginPos;
        Vector3 targetPos;
        public bool isCanDestroy;
        public bool isPlayingAnim;
        float currentTime = 0;
        float totalTime = 30;
        List<FishSerialDrillBullet> AllUseSerialDrillBulletList = new List<FishSerialDrillBullet>();
        Dictionary<int, FishSerialDrillBullet> CurrentUseSerialDrillBulletInsList = new Dictionary<int, FishSerialDrillBullet>();
        Action callBack;
        GameObject Skill_Drill;
        GameObject Skill_LogoItem;
        GameObject Skill_ShootBtnObj;
        GameObject countdownGameObject;
        GameObject Gun_SerialDrill;
        GameObject Gun_Sphere_Sprite;
        Transform Gun_BulletFirePos;
        Animator Gun_SerialDrill_Anim;
        int score = 0;
        int multiple = 0;
        Transform skillGunParent;
        Transform skillPanelParent;
        public SkillVo skillVo;
        FishGameData gameData;

        public void ResetSkillVo(SkillVo vo)
        {
            skillVo = vo;
            isCanDestroy = false;
        }

        public void UpdateSkillVo(int bulletId, SerialDrillBulletVo bulletInfo)
        {
            if (skillVo.serialDrillBulletList == null)
            {
                skillVo.serialDrillBulletList = new Dictionary<int, SerialDrillBulletVo> ();
            }
            skillVo.serialDrillBulletList[bulletId] = bulletInfo;

        }

        public void ResetSkillState(Vector3 beginPos, int skillStatus, float skillTime, Action callBack = null)
        {
            isCanDestroy = false;
            skillGunParent = skillVo.PlayerIns.Panel.SkillGun;
            skillPanelParent = skillVo.PlayerIns.Panel.SKillPanel;
            this.beginPos = beginPos;
            targetPos = new Vector3(beginPos.x, beginPos.y, 0);
            currentTime = skillTime;
            this.callBack = callBack;
            ShowSerialGunDrill();
        }

        public void ShowSerialGunDrill()
        {
            Gun_SerialDrill = FishGameObjectPoolManager.Instance.GetGameObject(Gun_SerialDrill_Res, PoolType.EffectPool);
            var trans = Gun_SerialDrill.transform;
            trans.SetParent(skillGunParent);
            trans.position = targetPos;
            trans.localRotation = Quaternion.identity;
            trans.localScale = Vector3.one;
            Gun_SerialDrill_Anim = trans.Find("TransLocation/Gun_Drill").GetComponent<Animator>();
            Gun_BulletFirePos = trans.Find("TransLocation/BulletFirePos").transform;
            PlayGunSerialDrillAnim(AnimParams[0]);
            isPlayingAnim = true;
        }

        public void ShootSerialGunDrill(int serialBulletID, int angle)
        {
            Gun_SerialDrill.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            PlayGunSerialDrillAnim(AnimParams[1]);
            ShowSerialDrillBullet(serialBulletID);
        }

        public void ShowSerialDrillBullet(int serialBulletID)
        {
            var vo = GetSerialDrillBulletVo(serialBulletID);
            var tempSerialDrillBulletIns = GetSerialDrillBullet(vo);
            if (tempSerialDrillBulletIns != null)
            {
                tempSerialDrillBulletIns.ResetState(this);
            }
            else
                Debug.LogError("获取SerialDrillBullet失败==>");
        }

        public SerialDrillBulletVo GetSerialDrillBulletVo(int serialBulletID)
        {
            SerialDrillBulletVo vo = new SerialDrillBulletVo();
            if (skillVo.serialDrillBulletList[serialBulletID] != null)
            {
                vo = skillVo.serialDrillBulletList[serialBulletID];
            }
            return vo;
        }

        public FishSerialDrillBullet GetSerialDrillBullet(SerialDrillBulletVo serialDrillBulletVo)
        {
            if (AllUseSerialDrillBulletList != null && AllUseSerialDrillBulletList.Count > 0)
            {
                var tempSerialDrillBulletIns = AllUseSerialDrillBulletList[0];
                AllUseSerialDrillBulletList.RemoveAt(0);
                if (tempSerialDrillBulletIns != null)
                {
                    tempSerialDrillBulletIns.ResetSerialDrillBulletVo(serialDrillBulletVo);
                    CurrentUseSerialDrillBulletInsList[serialDrillBulletVo.serialDrillBulletID] = tempSerialDrillBulletIns;
                    return tempSerialDrillBulletIns;
                }
                else
                {
                    Debug.LogError("Failed to create SerialDrillBullet instance: " + serialDrillBulletVo.serialDrillBulletID);
                }
            }
            else
            {
                GameObject go = FishGameObjectPoolManager.Instance.GetGameObject(Bullet_SerialDrill_Res, PoolType.BulletPool);
                var tempSerialDrillBulletIns = new FishSerialDrillBullet(go);
                if (tempSerialDrillBulletIns != null)
                {
                    tempSerialDrillBulletIns.ResetSerialDrillBulletVo(serialDrillBulletVo);
                    CurrentUseSerialDrillBulletInsList[serialDrillBulletVo.serialDrillBulletID] = tempSerialDrillBulletIns;
                    return tempSerialDrillBulletIns;
                }
            }
            return null;
        }

        public void SendHitFishInfo(int fishUID, int bulletUID)
        {
            List<int> hitFishTable = new List<int>();
            int chairID = skillVo.PlayerIns.GetPlayerChairId();
            int serialDrillUID = skillVo.UID;
            int drillBulletUID = bulletUID;
            hitFishTable.Add(fishUID);
            FishSerialDrillSkillManager.Instance.RequestSerialDrillHitFishMsg(chairID, serialDrillUID, drillBulletUID, hitFishTable);
        }

        public void BombSerialDrillSkill()
        {
            PlayGunSerialDrillAnim(AnimParams[0]);
        }

        public void PlayGunSerialDrillAnim(string animName)
        {
            Gun_SerialDrill_Anim.Play(animName, 0, 0);
        }

        public void ClearAllSerialDrillBullet()
        {
            foreach (var item in CurrentUseSerialDrillBulletInsList.Values)
            {
                item.isCanDestroy = true;
            }
            UpdateRemoveSerialDrillBullet();
        }

        public void RecycleSerialDrillBullet(SerialDrillBulletVo serialDrillBulletVo)
        {
            var tempSerialDrillBulletIns = CurrentUseSerialDrillBulletInsList[serialDrillBulletVo.serialDrillBulletID];
            if (tempSerialDrillBulletIns != null)
            {
                AllUseSerialDrillBulletList.Add(tempSerialDrillBulletIns);
                CurrentUseSerialDrillBulletInsList.Remove(serialDrillBulletVo.serialDrillBulletID);
            }
            else
            {
                Debug.LogError("回收连环钻头蟹子弹失败==>" + serialDrillBulletVo.serialDrillBulletID);
            }
        }

        public void UpdateRemoveSerialDrillBullet()
        {
            if (CurrentUseSerialDrillBulletInsList != null && CurrentUseSerialDrillBulletInsList.Count > 0)
            {
                List<int> removeKeyCatch = new List<int>();
                foreach (var kvp in CurrentUseSerialDrillBulletInsList)
                {
                    if (kvp.Value.isCanDestroy)
                    {
                        kvp.Value.Destroy();
                        removeKeyCatch.Add(kvp.Key);
                        RecycleSerialDrillBullet(kvp.Value.serialDrillBulletVo);
                    }
                }
                for (int i = 0; i < removeKeyCatch.Count; i++)
                {
                    RecycleSerialDrillBullet(CurrentUseSerialDrillBulletInsList[removeKeyCatch[i]].serialDrillBulletVo);
                }
            }
        }

        public void Update()
        {
            if (isPlayingAnim)
            {
                UpdateRemoveSerialDrillBullet();
                currentTime += Time.deltaTime;
                if (currentTime >= totalTime)
                {
                    currentTime = 0;
                    isCanDestroy = true;
                    isPlayingAnim = false;
                }
            }
        }

        public void Destroy()
        {
            isCanDestroy = false;
            isPlayingAnim = false;
            callBack = null;
            FishGameObjectPoolManager.Instance.ReCycleToGameObject(Gun_SerialDrill, PoolType.EffectPool);
            ClearAllSerialDrillBullet();
        }
    }
}

