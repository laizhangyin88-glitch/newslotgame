using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode
{
    public class FishDrillSkillItem
    {
        public enum DrillSkillStats
        {
            Born = 1,
            Idle = 2,
            Shoot = 3,
        }
        string Skill_Drill_Res = "Skill_Drill";
        string Gun_Drill_Res = "Gun_Drill";
        string Skill_CountdownTimer_Res = "Skill_CountdownTimer";
        string Bullet_Drill_Res = "Bullet_Drill";
        string[] AnimParams = { "DrillGun_Idle" };
        int bulletUID = 1;
        DrillSkillStats curSkillStats = DrillSkillStats.Born;
        Vector3 beginPos;
        Vector3 targetPos;
        public bool isCanDestroy;
        public bool isPlayingAnim;
        float shootTotalTime = 13.6f;
        float idleTotalTime = 32;
        float countDownTime = 30;
        float currentTime = 0;
        int score = 0;
        int multiple = 0;
        Transform skillGunParent;
        Transform skillPanelParent;
        public SkillVo skillVo;
        FishGameData gameData;
        Action callBack;
        GameObject Skill_Drill;
        GameObject Skill_LogoItem;
        GameObject Skill_ShootBtnObj;
        GameObject countdownGameObject;
        GameObject Gun_Drill;
        GameObject Gun_Sphere_Sprite;
        Transform Gun_BulletFirePos;
        Animator Gun_Drill_Anim;
        List<FishDrillBullet> AllUseDrillBulletList = new List<FishDrillBullet> ();
        Dictionary<int, FishDrillBullet> CurrentUseDrillBulletInsList = new Dictionary<int, FishDrillBullet>();
        Text countdownLabel;

        public FishDrillSkillItem()
        {
            gameData = FishGameUIManager.Instance.gameData;
        }

        public void ResetSkillVo(SkillVo vo)
        {
            skillVo = vo;
            isCanDestroy = false;
        }

        public void ResetSkillState(Vector3 beginPos, int skillStatus, float skillTime, Action callBack = null)
        {
            isCanDestroy = false;
            score = 0;
            multiple = 0;
            skillGunParent = skillVo.PlayerIns.Panel.SkillGun;
            skillPanelParent = skillVo.PlayerIns.Panel.SKillPanel;
            this.beginPos = beginPos;
            targetPos = skillGunParent.position;
            this.callBack = callBack;
            ChangePlayerView();
            GetCurrentDrillStep(skillStatus, skillTime);
            ShowDrillSkill(skillTime * 0.001f);
            isPlayingAnim = true;
        }

        public void ChangePlayerView()
        {
            if (skillVo.IsMe)
            {
                skillVo.PlayerIns.UploadAutoLockFish(false);
                skillVo.PlayerIns.Panel.IsShowLockFishPanel(false);
                //todo gameset
            }
            skillVo.PlayerIns.SetCanShootBullet(false);
        }

        public void GetCurrentDrillStep(int skillStatus, float skillTime)
        {
            if (skillStatus == 1)
            {
                if (skillTime <= 0.1f)
                {
                    curSkillStats = DrillSkillStats.Born;
                }
                else
                {
                    curSkillStats = DrillSkillStats.Idle;
                }
            }
            else if(skillStatus == 2)
            {
                curSkillStats = DrillSkillStats.Shoot;
            }
            currentTime = skillTime * 0.001f;
        }

        public void ShowDrillSkill(float timeElapsed)
        {
            if (curSkillStats == DrillSkillStats.Born)
            {
                GetDrillResItem(beginPos);
                GetCountdownTimerResItem();
                MoveDrillItemToGun();
            }
            else if (curSkillStats == DrillSkillStats.Idle)
            {
                GetDrillResItem(targetPos);
                GetCountdownTimerResItem();
                ShowGunDrill();
            }
            else if (curSkillStats == DrillSkillStats.Shoot)
            {
                GetDrillResItem(targetPos);
                ShowGunDrill();
                ShootGunDrill(skillVo.PlayerIns.GetPlayerChairId(), 0, timeElapsed);
            }
        }

        public bool IsDrillShoot()
        {
            return curSkillStats == DrillSkillStats.Shoot;
        }

        public void GetDrillResItem(Vector3 pos)
        {
            Skill_Drill = FishGameObjectPoolManager.Instance.GetGameObject(Skill_Drill_Res, PoolType.EffectPool);
            var trans = Skill_Drill.transform;
            trans.SetParent(skillPanelParent);
            trans.position = pos;
            trans.localRotation = Quaternion.identity;
            trans.localScale = Vector3.one;
            Skill_LogoItem = trans.Find("DrillObject").gameObject;
            Skill_ShootBtnObj = trans.Find("Fire").gameObject;
            Button btn = Skill_ShootBtnObj.GetComponent<Button>();
            btn.onClick.AddListener(OnClickShootFire);
            IsShowSkillLogoItem(true);
            IsShowSkillShootBtnObj(false);
        }

        public void GetCountdownTimerResItem()
        {
            if (skillVo.IsMe)
            {
                countdownGameObject = FishGameObjectPoolManager.Instance.GetGameObject(Skill_CountdownTimer_Res, PoolType.EffectPool);
                var trans = countdownGameObject.transform;
                trans.localPosition = new Vector3(0, -170, 0);
                trans.localRotation = Quaternion.Euler(0, 0, 0);
                trans.localScale = Vector3.one;
                countdownLabel = trans.Find("Text").GetComponent<Text>();
                countdownGameObject.SetActive(false);
            }
        }

        public void MoveDrillItemToGun()
        {
            Transform trans = Skill_Drill.transform;
            Vector3 orginalPos = trans.localPosition;
            Vector3 targetPos1 = new Vector3(trans.localPosition.x, trans.localPosition.y + 100, trans.localPosition.z);
            AsyncActionUtils.ApplyScaling(FishElectricSkillManager.Instance, trans, trans.localScale, Vector3.one * 1.5f, 0.3f, TweenUtils.VectorTweenOutCubic);
            AsyncActionUtils.ApplyLocalMovement(FishElectricSkillManager.Instance, trans, trans.localPosition, targetPos1, 0.3f, TweenUtils.VectorTweenOutCubic, 0, () =>
            {
                AsyncActionUtils.ApplyLocalMovement(FishElectricSkillManager.Instance, trans, trans.localPosition, orginalPos, 0.2f, TweenUtils.VectorTweenInCubic, 0, () =>
                {
                    targetPos1 = new Vector3(trans.localPosition.x, trans.localPosition.y + 40, trans.localPosition.z);
                    AsyncActionUtils.ApplyLocalMovement(FishElectricSkillManager.Instance, trans, trans.localPosition, targetPos1, 0.2f, TweenUtils.VectorTweenOutCubic, 0, () =>
                    {
                        AsyncActionUtils.ApplyLocalMovement(FishElectricSkillManager.Instance, trans, trans.localPosition, orginalPos, 0.1f, TweenUtils.VectorTweenInCubic, 0, () =>
                        {
                            AsyncActionUtils.ApplyScaling(FishElectricSkillManager.Instance, trans, trans.localScale, Vector3.one, 0.7f, TweenUtils.VectorTweenInCubic);
                            AsyncActionUtils.ApplyMovement(FishElectricSkillManager.Instance, trans, trans.position, targetPos, 0.7f, TweenUtils.VectorTweenInCubic, 0, ShowGunDrill);
                        });
                    });
                });
            });
            if (skillVo.IsMe)
                FishAudioManager.Instance.PlayNormalAudio(41);
        }

        public void ShowGunDrill()
        {
            if (skillVo.IsMe)
            {
                FishAudioManager.Instance.StopNormalAudio(43);
                FishAudioManager.Instance.PlayNormalAudio(43, 1, true);
                IsShowSkillShootBtnObj(true);
            }
            IsShowSkillLogoItem(false);
            Gun_Drill = FishGameObjectPoolManager.Instance.GetGameObject(Gun_Drill_Res, PoolType.EffectPool);
            var trans = Gun_Drill.transform;
            trans.SetParent(skillGunParent);
            trans.position = targetPos;
            trans.localRotation = Quaternion.Euler(0, 0, 0);
            trans.localScale = Vector3.one;
            skillVo.PlayerIns.SetMuzzleEulerAngles(0 / skillVo.PlayerIns.PrecisionValue);
            Gun_Drill_Anim = trans.Find("TransLocation/Gun_Drill").GetComponent<Animator>();
            Gun_Sphere_Sprite = trans.Find("TransLocation/Sphere_Sprite").gameObject;
            Gun_BulletFirePos = trans.Find("TransLocation/BulletFirePos").transform;
            IsShowPlayerGunPanel(false);
            curSkillStats = DrillSkillStats.Idle;
            IsShowGunSphere(true);
            PlayGunDrillAnim(AnimParams[0]);
        }

        public void IsShowPlayerGunPanel(bool isDisplay)
        {
            var playerIns = skillVo.PlayerIns;
            if (isDisplay)
            {
                FishGameUIManager.Instance.IsGamePress(true);
                if (skillVo.IsMe)
                {
                    skillVo.PlayerIns.Panel.IsShowLockFishPanel(true);
                    //todo gameset
                    //GameSetManager.GetInstance().PlayerSetPanel.SetLockFishBtnDisable(true);
                }
                if (skillVo.IsMe)
                {
                    playerIns.Panel.IsShowBetPanel(true);
                }
                playerIns.SetCanShootBullet(true);
                playerIns.SetGunLevelValue(playerIns.CurrentGunLevel);
            }
            else
            {
                playerIns.Panel.IsShowBetPanel(false);
                playerIns.Panel.HideGunPanel();
            }
        }

        public void PlayGunDrillAnim(string animName)
        {
            Gun_Drill_Anim.Play(animName, 0, 0);
        }

        public void DrillAimOnClick()
        {
            if (curSkillStats == DrillSkillStats.Idle && skillVo.IsMe)
            {
                skillVo.PlayerIns.NormalRotationGun();
                var UID = skillVo.UID;
                var chairId = skillVo.PlayerIns.GetPlayerChairId();
                var aimAngle = Mathf.FloorToInt(skillVo.PlayerIns.gunTeamObj.transform.eulerAngles.z * skillVo.PlayerIns.PrecisionValue);
                FishDrillSkillManager.Instance.RequestDrillAimMsg(chairId, aimAngle, UID);
            }
        }

        public void ChangeDrillSkillAim(int angle)
        {
            skillVo.PlayerIns.SetMuzzleEulerAngles(angle / skillVo.PlayerIns.PrecisionValue);
        }

        public void OnClickShootFire()
        {
            int UID = skillVo.UID;
            int chairID = gameData.playerChairId;
            var aimAngle = Mathf.FloorToInt(skillVo.PlayerIns.gunTeamObj.transform.eulerAngles.z * skillVo.PlayerIns.PrecisionValue);
            FishDrillSkillManager.Instance.RequestDrillShootMsg(chairID, aimAngle, UID);
        }

        public void ShootGunDrill(int chairId, float angle, float timeElapsed)
        {
            currentTime = timeElapsed;
            IsShowSkillLogoItem(false);
            IsShowSkillShootBtnObj(false);
            if (countdownGameObject != null)
            {
                countdownGameObject.SetActive(false);
            }
            curSkillStats = DrillSkillStats.Shoot;
            skillVo.PlayerIns.SetMuzzleEulerAngles(angle / skillVo.PlayerIns.PrecisionValue);
            IsShowGunSphere(false);
            ShowDrillBullet(skillVo.traceId, skillVo.startPoint);
            Gun_Drill.transform.localPosition = new Vector3(10000, 10000, 0);
            if (skillVo.IsMe)
            {
                FishAudioManager.Instance.StopNormalAudio(43);
                FishAudioManager.Instance.PlayNormalAudio(42);
            }
        }

        public void ShowDrillBullet(int traceId, int traceStartPoint)
        {
            var vo = GetDrillBulletVo(traceId, traceStartPoint);
            var tempDrillBulletIns = GetDrillBullet(vo);
            if (tempDrillBulletIns != null)
            {
                tempDrillBulletIns.ResetState(this);
            }
            else
            {
                Debug.LogError("获取DrillBullet失败==>");
            }
        }

        public int GetDrillBulletUID()
        {
            bulletUID++;
            return bulletUID;
        }

        public DrillBulletVo GetDrillBulletVo(int traceId, int startPoint)
        {
            DrillBulletVo vo = new DrillBulletVo {
                traceId = traceId,
                startPoint = startPoint,
                UID = GetDrillBulletUID(),
            };
            return vo;
        }

        public FishDrillBullet GetDrillBullet(DrillBulletVo drillBulletVo)
        {
            if (AllUseDrillBulletList != null && AllUseDrillBulletList.Count > 0)
            {
                var tempDrillBulletIns = AllUseDrillBulletList[0];
                AllUseDrillBulletList.RemoveAt(0);
                if (tempDrillBulletIns != null)
                {
                    tempDrillBulletIns.ResetDrillBulletVo(drillBulletVo);
                    CurrentUseDrillBulletInsList[drillBulletVo.UID] = tempDrillBulletIns;
                    return tempDrillBulletIns;
                }
            }
            else
            {
                var go = FishGameObjectPoolManager.Instance.GetGameObject(Bullet_Drill_Res, PoolType.BulletPool);
                var tempDrillBulletIns = new FishDrillBullet(go);
                if (tempDrillBulletIns != null)
                {
                    tempDrillBulletIns.ResetDrillBulletVo(drillBulletVo);
                    CurrentUseDrillBulletInsList[drillBulletVo.UID] = tempDrillBulletIns;
                    return tempDrillBulletIns;
                }
            }
            return null;
        }

        public void SendHitFishInfo(int fishUID)
        {
            if (skillVo.IsMe || (skillVo.usProcUserChairId != -1 && skillVo.usProcUserChairId == gameData.playerChairId))
            {
                int usRobotChairId = -1;
                if (skillVo.usProcUserChairId != -1 && skillVo.usProcUserChairId == gameData.playerChairId)
                {
                    usRobotChairId = skillVo.chairId;
                }

                List<int> hitFishTable = new List<int>();
                int chairID = gameData.playerChairId;
                int UID = skillVo.UID;
                hitFishTable.Add(fishUID);
                FishDrillSkillManager.Instance.RequestDrillHitFishMsg(chairID, UID, hitFishTable, usRobotChairId);
            }
        }

        public void BombDrillSkill(int score, int mul)
        {
            this.score = score;
            this.multiple = mul;
            if (CurrentUseDrillBulletInsList != null && CurrentUseDrillBulletInsList.Count > 0)
            {
                foreach (var kvp in CurrentUseDrillBulletInsList)
                {
                    kvp.Value.BombDrill();
                }
            }
        }

        void ClearAllDrillBullet()
        {
            if (CurrentUseDrillBulletInsList != null)
            {
                foreach (var kvp in CurrentUseDrillBulletInsList)
                {
                    kvp.Value.isCanDestroy = true;
                }
                UpdateRemoveDrillBullet();
            }
            CurrentUseDrillBulletInsList.Clear();
        }

        void RecycleDrillBullet(DrillBulletVo drillBulletVo)
        {
            if (CurrentUseDrillBulletInsList.ContainsKey(drillBulletVo.UID))
            {
                var tempDrillSkillIns = CurrentUseDrillBulletInsList[drillBulletVo.UID];
                AllUseDrillBulletList.Add(tempDrillSkillIns);
                CurrentUseDrillBulletInsList.Remove(drillBulletVo.UID);
            }
            else
            {
                Debug.LogError("回收钻头蟹子弹失败==>" + drillBulletVo.UID);
            }
        }

        public void UpdateRemoveDrillBullet()
        {
            if (CurrentUseDrillBulletInsList != null && CurrentUseDrillBulletInsList.Count > 0)
            {
                List<int> removeKeyCatch = new List<int>();
                foreach (var kvp in CurrentUseDrillBulletInsList)
                {
                    if (kvp.Value.isCanDestroy)
                    {
                        kvp.Value.Destroy();
                        removeKeyCatch.Add(kvp.Key);
                    }
                }
                for (int i = 0; i < removeKeyCatch.Count; i++)
                {
                    RecycleDrillBullet(CurrentUseDrillBulletInsList[removeKeyCatch[i]].drillBulletVo);
                }
            }
        }

        public void ShootCountdownLable()
        {
            if (skillVo.IsMe)
            {
                if ((idleTotalTime - currentTime) <= countDownTime)
                {
                    if (!countdownGameObject.activeSelf)
                    {
                        countdownGameObject.SetActive(true);
                    }
                    int time = Mathf.CeilToInt(idleTotalTime - currentTime);
                    countdownLabel.text = "调整发射方向并点击发射按钮，或<color=red>" + time.ToString() + "</color>S后自动发射！";
                }
                else
                {
                    if (countdownGameObject.activeSelf)
                    {
                        countdownGameObject.SetActive(false);
                    }
                    FishAudioManager.Instance.StopNormalAudio(43);
                }
            }
        }

        public void Update()
        {
            if (isPlayingAnim)
            {
                if (curSkillStats == DrillSkillStats.Born || curSkillStats == DrillSkillStats.Idle)
                {
                    currentTime += Time.deltaTime;
                    ShootCountdownLable();
                    if (currentTime >= idleTotalTime)
                    {
                        currentTime = 0;
                    }
                }
                else if (curSkillStats == DrillSkillStats.Shoot)
                {
                    currentTime += Time.deltaTime;
                    if (currentTime >= shootTotalTime)
                    {
                        currentTime = 0;
                        isPlayingAnim = false;
                        isCanDestroy = true;
                        IsShowPlayerGunPanel(true);
                        FishDrillSkillManager.Instance.ShowSpecialDeclareScore(skillVo.UID, score, multiple);
                        callBack?.Invoke();
                    }
                }
            }
            foreach (var item in CurrentUseDrillBulletInsList.Values)
            {
                item.Update();
            }
        }

        public void IsShowSkillLogoItem(bool isDisplay)
        {
            Skill_LogoItem.SetActive(isDisplay);
        }

        public void IsShowSkillShootBtnObj(bool isDisplay)
        {
            Skill_ShootBtnObj.SetActive(isDisplay);
        }

        public void IsShowGunSphere(bool isDisplay)
        {
            Gun_Sphere_Sprite.SetActive(isDisplay);
        }

        public void Destroy()
        {
            isCanDestroy = false;
            isPlayingAnim = false;
            callBack = null;
            if (countdownGameObject != null)
            {
                FishGameObjectPoolManager.Instance.ReCycleToGameObject(countdownGameObject, PoolType.EffectPool);
            }
            countdownGameObject = null;

            if (Skill_Drill != null)
            {
                FishGameObjectPoolManager.Instance.ReCycleToGameObject(Skill_Drill, PoolType.EffectPool);
            }
            Skill_Drill = null;

            if (Gun_Drill != null)
            {
                FishGameObjectPoolManager.Instance.ReCycleToGameObject(Gun_Drill, PoolType.EffectPool);
            }
            Gun_Drill = null;
            ClearAllDrillBullet();
        }
    }
}
