using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BagelCode
{
    public class FishElectricSkillItem
    {
        enum ElcSkillStats
        {
            Born = 1,
            Idle = 3,
            Shoot = 4,
        }
        string Skill_Electric_Res = "Skill_Electric";
        string Gun_Electric_Res = "Gun_Electric";
        string Skill_CountdownTimer_Res = "Skill_CountdownTimer";
        string FishTag = "Fish";
        string[] AnimParams = new string[] { "EletricGun_Idle", "EletricGun_Fire" };
        bool isPlayingAnim = false;
        ElcSkillStats curSkillStats = ElcSkillStats.Born;
        Vector3 beginPos;
        Vector3 targetPos;
        public bool isCanDestroy;
        float shootTotalTime = 3.15f;
        float idleTotalTime = 30;
        float countDownTime = 30;
        float currentTime = 0;
        Transform skillGunParent;
        Transform skillPanelParent;
        public SkillVo skillVo;
        FishGameData gameData;
        Action callBack;
        GameObject Skill_Electric;
        GameObject Skill_LogoItem;
        GameObject Skill_ShootBtnObj;
        GameObject countdownGameObject;
        GameObject Gun_Electric;
        Animator Gun_Electric_Anim;
        Text countdownLabel;
        FishTrigger trigger;
        public FishElectricSkillItem()
        {
            gameData = FishGameUIManager.Instance.gameData;
        }

        public void ResetSkillVo(SkillVo vo)
        {
            skillVo = vo;
            isCanDestroy = false;
            isPlayingAnim = false;
        }

        public void ResetSkillState(Vector3 beginPos, int skillStatus, float skillTime, Action callBack = null)
        {
            isCanDestroy = false;
            skillGunParent = skillVo.PlayerIns.Panel.SkillGun;
            skillPanelParent = skillVo.PlayerIns.Panel.SKillPanel;
            this.beginPos = beginPos;
            targetPos = skillGunParent.position;
            this.callBack = callBack;
            ChangePlayerView();
            GetCurrentElectricStep(skillStatus, skillTime);
            ShowElectricSkill(skillTime);
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

        public void GetCurrentElectricStep(int skillStatus, float skillTime)
        {
            if (skillStatus == 1)
            {
                if (skillTime <= 0.05f)
                {
                    curSkillStats = ElcSkillStats.Born;
                }
                else
                    curSkillStats = ElcSkillStats.Idle;
            }
            else if (skillStatus == 2)
            {
                curSkillStats = ElcSkillStats.Shoot;
            }
            currentTime = skillTime * 0.001f;
        }

        public void ShowElectricSkill(float timeElapsed)
        {
            if (curSkillStats == ElcSkillStats.Born)
            {
                GetElectricResItem(beginPos);
                GetCountdownTimerResItem();
                MoveElectricItemToGun();
            }
            else if (curSkillStats == ElcSkillStats.Idle)
            {
                GetElectricResItem(targetPos);
                GetCountdownTimerResItem();
                ShowGunElectric();
            }
            else if (curSkillStats == ElcSkillStats.Shoot)
            {
                GetElectricResItem(targetPos);
                ShowGunElectric();
                ShootGunElectric( 0, timeElapsed);
            }
        }

        public void GetElectricResItem(Vector3 pos)
        {
            Skill_Electric = FishGameObjectPoolManager.Instance.GetGameObject(Skill_Electric_Res, PoolType.EffectPool);
            Transform trans = Skill_Electric.transform;
            trans.SetParent(skillPanelParent);
            trans.position = pos;
            trans.localRotation = Quaternion.Euler(0, 0, 0);
            trans.localScale = Vector3.one;
            Skill_LogoItem = trans.Find("FakeGun").gameObject;
            Skill_ShootBtnObj = trans.Find("Fire").gameObject;
            Button btn = Skill_ShootBtnObj.GetComponent<Button>();
            btn.onClick.AddListener(OnclickShootFire);

            IsShowSkillLogoItem(true);
            IsShowSkillShootBtnObj(false);
        }


        public void GetCountdownTimerResItem()
        {
            if (skillVo.IsMe)
            {
                countdownGameObject = FishGameObjectPoolManager.Instance.GetGameObject(Skill_CountdownTimer_Res, PoolType.EffectPool);
                Transform trans = countdownGameObject.transform;
                trans.position = new Vector3(0, -170f, 0);
                trans.localRotation = Quaternion.Euler(0, 0, 0);
                trans.localScale = Vector3.one;
                countdownLabel = trans.Find("Text").GetComponent<Text>();
                countdownGameObject.SetActive(false);
            }
        }

        public void MoveElectricItemToGun()
        {
            Transform trans = Skill_Electric.transform;
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
                            AsyncActionUtils.ApplyScaling(FishElectricSkillManager.Instance, trans, trans.localScale, Vector3.one , 0.7f, TweenUtils.VectorTweenInCubic);
                            AsyncActionUtils.ApplyMovement(FishElectricSkillManager.Instance, trans, trans.position, targetPos, 0.7f, TweenUtils.VectorTweenInCubic, 0, ShowGunElectric);
                        });
                    });
                });
            });

            if (skillVo.IsMe)
                FishAudioManager.Instance.PlayNormalAudio(41);
        }

        public void ShowGunElectric()
        {
            IsShowSkillLogoItem(false);
            if (skillVo.IsMe)
            {
                FishAudioManager.Instance.StopNormalAudio(43);
                FishAudioManager.Instance.PlayNormalAudio(43, 1,true);
                IsShowSkillShootBtnObj(true);
            }
            Gun_Electric = FishGameObjectPoolManager.Instance.GetGameObject(Gun_Electric_Res, PoolType.EffectPool);
            Transform trans = Gun_Electric.transform;
            trans.SetParent(skillGunParent);
            trans.position = targetPos;
            trans.localRotation = Quaternion.Euler(0, 0, 0);
            trans.localScale = Vector3.one;
            skillVo.PlayerIns.SetMuzzleEulerAngles(0 / skillVo.PlayerIns.PrecisionValue);
            Gun_Electric_Anim = trans.Find("GunAnimator").GetComponent<Animator>();
            GameObject Sphere_Obj = trans.Find("GunAnimator/Sphere_Obj").gameObject;
            trigger = Sphere_Obj.GetComponent<FishTrigger>();
            if (trigger == null)
            {
                trigger = Sphere_Obj.AddComponent<FishTrigger>();
            }
            trigger.onTriggerCallBack = OnTriggerEnter;
            IsShowPlayerGunPanel(false);
            curSkillStats = ElcSkillStats.Idle;
            PlayGunElcAnim(AnimParams[0]);
        }

        public void IsShowPlayerGunPanel(bool isDelay)
        {
            FishPlayerInfo playerIns = skillVo.PlayerIns;
            if(isDelay)
            {
                FishGameUIManager.Instance.IsGamePress(true);
                if (skillVo.IsMe)
                {
                    playerIns.Panel.IsShowLockFishPanel(true);
                    playerIns.Panel.IsShowBetPanel(true);
                    //todo Set
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

        private void OnTriggerEnter(Collider other)
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
                    SendHitFishInfo(hitFish.FishVo.UID);
                }
            }
        }

        private void SendHitFishInfo(int fishUID)
        {
            if (skillVo.IsMe || (skillVo.usProcUserChairId != -1 && skillVo.usProcUserChairId == gameData.playerChairId))
            {
                int usRobotChairId;
                if (skillVo.usProcUserChairId != -1 && skillVo.usProcUserChairId == gameData.playerChairId)
                {
                    usRobotChairId = skillVo.chairId;
                }
                else
                    usRobotChairId = -1;
                FishElectricSkillManager.Instance.RequestDianCiCannonHitFishMsg(gameData.playerChairId, skillVo.UID, fishUID, usRobotChairId);
            }
        }

        public void ShootGunElectric(int angle, float timeElapsed)
        {
            currentTime = timeElapsed;
            IsShowSkillLogoItem(false);
            IsShowSkillShootBtnObj(false);
            if (countdownGameObject != null)
            {
                countdownGameObject.SetActive(false);
            }
            curSkillStats = ElcSkillStats.Shoot;
            skillVo.PlayerIns.SetMuzzleEulerAngles(angle / skillVo.PlayerIns.PrecisionValue);
            PlayGunElcAnim(AnimParams[1]);
            FishAudioManager.Instance.StopNormalAudio(43);
            FishAudioManager.Instance.PlayNormalAudio(42);
            FishGameUIManager.Instance.SetShake(false);
        }

        public void PlayGunElcAnim(string animName)
        {
            Gun_Electric_Anim.Play(animName, 0, 0);
        }

        public void OnclickShootFire()
        {
            int angle = Mathf.FloorToInt(skillVo.PlayerIns.gunTeamObj.transform.eulerAngles.z * skillVo.PlayerIns.PrecisionValue);
            FishElectricSkillManager.Instance.RequestDianCiCannonShootMsg(skillVo.PlayerIns.GetPlayerChairId(), angle, skillVo.UID);
        }

        public void ElectricAimOnClick()
        {
            if (curSkillStats == ElcSkillStats.Idle && skillVo.IsMe)
            {
                skillVo.PlayerIns.NormalRotationGun();
                int angle = Mathf.FloorToInt(skillVo.PlayerIns.gunTeamObj.transform.eulerAngles.z * skillVo.PlayerIns.PrecisionValue);
                FishElectricSkillManager.Instance.RequestDianCiCannonAimMsg(skillVo.PlayerIns.GetPlayerChairId(), angle, skillVo.UID);
            }
        }

        public void ChangeElectricSkillAim(int angel)
        {
            skillVo.PlayerIns.SetMuzzleEulerAngles(angel / skillVo.PlayerIns.PrecisionValue);
        }

        public void IsShowSkillLogoItem(bool isDisPlay)
        {
            Skill_LogoItem.SetActive(isDisPlay);
        }

        public void IsShowSkillShootBtnObj(bool isDisPlay)
        {
            Skill_ShootBtnObj.SetActive(isDisPlay);
        }

        public void ShootCountdownLable()
        {
            if (skillVo.IsMe)
            {
                if ((idleTotalTime - currentTime) <= countDownTime)
                {
                    if (!countdownGameObject.activeSelf)
                        countdownGameObject.SetActive(true);
                    int time = Mathf.CeilToInt(idleTotalTime - currentTime);
                    countdownLabel.text = "调整发射方向并点击发射按钮，或 < color = red > " + time.ToString() + "s </ color > 后自动发射！";
                }
                else
                {
                    if (countdownGameObject.activeSelf)
                        countdownGameObject.SetActive(false);
                    FishAudioManager.Instance.StopNormalAudio(43);
                }
            }
        }

        public void Update()
        {
            if (isPlayingAnim)
            {
                if (curSkillStats == ElcSkillStats.Idle || curSkillStats == ElcSkillStats.Born)
                {
                    currentTime += Time.deltaTime;
                    ShootCountdownLable();
                    if (currentTime >= idleTotalTime)
                    {
                        currentTime = 0;
                    }
                }
                else if (curSkillStats == ElcSkillStats.Shoot)
                {
                    currentTime += Time.deltaTime;
                    if (currentTime >= shootTotalTime)
                    {
                        currentTime = 0;
                        isPlayingAnim = false;
                        isCanDestroy = true;
                        IsShowPlayerGunPanel(true);
                        callBack?.Invoke();
                    }
                }
            }
        }

        public void Destroy()
        {
            FishAudioManager.Instance.StopNormalAudio(43);
            if (countdownGameObject != null)
                FishGameObjectPoolManager.Instance.ReCycleToGameObject(countdownGameObject, PoolType.EffectPool);
            countdownGameObject = null;

            if (Skill_Electric != null)
                FishGameObjectPoolManager.Instance.ReCycleToGameObject(Skill_Electric, PoolType.EffectPool);
            Skill_Electric = null;

            if (Gun_Electric != null)
                FishGameObjectPoolManager.Instance.ReCycleToGameObject(Gun_Electric, PoolType.EffectPool);
            Gun_Electric = null;

            isCanDestroy = false;
            isPlayingAnim = false;
            callBack = null;
        }
    }
}
