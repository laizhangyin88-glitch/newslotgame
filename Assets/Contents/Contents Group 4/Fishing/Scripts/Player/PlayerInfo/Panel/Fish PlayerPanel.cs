using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode
{
    public class FishPlayerPanel
    {
        public int PreCount = 30;
        public List<GameObject> LockPointList;
        public int GunLevelCount = 3;
        public int CatchFishPosIndex = 0;
        public int BigAwardPosCount = 3;
        public int CatchFishPosCount = 3;
        public List<Transform> CatchFishPosList;
        public Transform SwrilPanel;
        public Transform SpecialDeclarePanel;
        public Timer ImHereTimer = null;
        public Transform SkillGun;
        public Transform SKillPanel = null;
        public GameObject GunTeamObj = null;
        public Transform BulletPos;
        public GameObject GunLockPoint;
        public GameObject ImHere;
        public Animator ImHereAnimator;
        public GameObject LockFishPanel;
        public GameObject LockRotation;
        public Animator LockRotationAnim;
        public GameObject LockPoint;
        public GameObject LockTips;
        public Animator LockTipsAnim;
        public GameObject BetPanel;
        public Button AddBetBtn;
        public Button ReduceBetBtn;
        public Transform FlyScorePos;
        public Transform FlyCoinPos;
        public Text BetScoreLabel;
        public Text PlayerMoneyLabel;
        public Text PlayerNameLabel;
        public bool showImHere;

        //跑升级特效
        private GameObject GunParticleEffect;
        private List<Animator> GunLevelAnimList;
        private string[] GunAnimPramsList = { "shot01", "shot02", "shot03", "FireStormGun_Shoot" };

        private Animator gunBaseAnimator;
        private Animator gunNormalAnimator;
        private Animator gunSpeedAnimator;
        private Animator ammunitionAnimator;
        private Animator fireStormAnimator;
        public bool isSpeed;
        private bool isFireStorm;

        public FishPlayerPanel(GameObject gameObj)
        {
            InitData();
            InitView(gameObj);
            InitViewData();
        }

        private void InitData()
        {
            PreCount = 30;
            LockPointList = new List<GameObject>();
            GunLevelCount = 3;
            CatchFishPosIndex = 0;
            BigAwardPosCount = 3;
            CatchFishPosCount = 3;
            CatchFishPosList = new List<Transform>();
            SwrilPanel = null;
            SpecialDeclarePanel = null;
            GunLevelAnimList = new List<Animator>();
            GunAnimPramsList = new string[] { "shot01", "shot02", "shot03", "FireStormGun_Shoot" };
            ImHereTimer = null;
            SkillGun = null;
            SKillPanel = null;
        }

        private void InitView(GameObject gameObj)
        {
            FindView(gameObj);
        }

        private void FindView(GameObject gameObj)
        {
            FindGunView(gameObj.transform);
            FindLockFishView(gameObj.transform);
            FindPlayerInfoView(gameObj.transform);
            AddNodePanel(gameObj.transform);
        }

        private void InitViewData()
        {
            InitLockPoint();
            SetShowPanel(false);
        }

        public void SetShowPanel(bool isDisplay)
        {
            IsShowlockRotation(isDisplay);
            IsShowLockPoint(isDisplay);
            IsShowLockTips(isDisplay);
            IsShowGunLockPoint(isDisplay);
        }

        private void FindGunView(Transform trans)
        {
            GunTeamObj = trans.Find("Gun_Team").gameObject;
            BulletPos = trans.Find("Gun_Team/BulletPos");
            GunLockPoint = trans.Find("Gun_Team/LockPonitTeam").gameObject;
            ImHere = trans.Find("ImHere").gameObject;
            ImHereAnimator = ImHere.GetComponent<Animator>();
            ImHere.SetActive(false);
            GunParticleEffect = trans.Find("Gun_Team/GunParticleEffect").gameObject;
            //for (int i = 1; i <= GunLevelCount; i++)
            //{
            //    Animator tempGunAnim = trans.Find("Gun_Team/GunType0" + i).GetComponent<Animator>();
            //    GunLevelAnimList.Add(tempGunAnim);
            //}
            //Animator gunFireStorm = trans.Find("Gun_Team/GunFireStorm").GetComponent<Animator>();
            //GunLevelAnimList.Add(gunFireStorm);

            fireStormAnimator = trans.Find("Gun_Team/GunFireStorm").GetComponent<Animator>();
            gunBaseAnimator = trans.Find("GunBase").GetComponent<Animator>();
            gunNormalAnimator = trans.Find("Gun_Team/GunNormal").GetComponent <Animator>();
            gunSpeedAnimator = trans.Find("Gun_Team/GunSpeed").GetComponent<Animator>();
            ammunitionAnimator = trans.Find("Ammunition").GetComponent<Animator>();
        }

        public void ChangeGunModeAnim(bool isToSpeed)
        {
            if (isToSpeed)
            {
                gunBaseAnimator.SetFloat("AnimSpeed", 1.0f);
                gunBaseAnimator.Play("GunChange", 0, 0);
                AsyncActionUtils.DelayedAction(FishPlayerManager.Instance, 1, () =>
                {
                    isSpeed = true;
                    gunNormalAnimator.gameObject.SetActive(false);
                    gunSpeedAnimator.gameObject.SetActive(true);
                    ammunitionAnimator.gameObject.SetActive(true);
                    AsyncActionUtils.ApplyScaling(FishPlayerManager.Instance, gunSpeedAnimator.transform, Vector3.one * 1.5f, Vector3.one, 0.5f, TweenUtils.VectorTweenLinear);
                    AsyncActionUtils.ApplyScaling(FishPlayerManager.Instance, ammunitionAnimator.transform, Vector3.one * 1.5f, Vector3.one, 0.5f, TweenUtils.VectorTweenLinear);
                });
                AsyncActionUtils.DelayedAction(FishPlayerManager.Instance, 2.4f, () => { SetShowGunPanel(); });
            }
            else
            {
                gunBaseAnimator.SetFloat("AnimSpeed", -1.0f);
                gunBaseAnimator.Play("GunChange",0, 1);

                AsyncActionUtils.DelayedAction(FishPlayerManager.Instance, 1, () =>
                {
                    isSpeed = false;
                    gunNormalAnimator.gameObject.SetActive(true);
                    gunSpeedAnimator.gameObject.SetActive(false);
                    ammunitionAnimator.gameObject.SetActive(false);
                    AsyncActionUtils.ApplyScaling(FishPlayerManager.Instance, gunNormalAnimator.transform, Vector3.one * 1.5f, Vector3.one, 0.5f, TweenUtils.VectorTweenLinear);
                });
                AsyncActionUtils.DelayedAction(FishPlayerManager.Instance, 2.4f, () => { SetShowGunPanel(); });
            }
        }

        public void FindLockFishView(Transform mTF)
        {
            LockFishPanel = mTF.Find("LockFishPanel").gameObject;
            LockRotation = mTF.Find("LockFishPanel/SkillLockRotation").gameObject;
            LockRotationAnim = LockRotation.GetComponent<Animator>();
            LockPoint = mTF.Find("LockFishPanel/LockPoint").gameObject;
            LockTips = mTF.Find("LockFishPanel/LockTips").gameObject;
            LockTipsAnim = LockTips.GetComponent<Animator>();
        }

        public void FindPlayerInfoView(Transform mTF)
        {
            BetPanel = mTF.Find("BetPanel").gameObject;
            AddBetBtn = mTF.Find("BetPanel/Add").GetComponent<Button>();
            ReduceBetBtn = mTF.Find("BetPanel/Reduce").GetComponent<Button>();
            FlyScorePos = mTF.Find("PlayerInfoPanel/ScoreImage/ScorePoint");
            FlyCoinPos = mTF.Find("PlayerInfoPanel/ScoreImage/CoinPos");
            BetScoreLabel = mTF.Find("PlayerInfoPanel/BetText").GetComponent<Text>();
            PlayerMoneyLabel = mTF.Find("PlayerInfoPanel/ScoreImage/Text").GetComponent<Text>();
            PlayerNameLabel = mTF.Find("PlayerInfoPanel/Name").GetComponent<Text>();
        }

        public void AddNodePanel(Transform mTF)
        {
            Transform catchFishPanel = mTF.Find("AddNodePanel/CatchFishPanel");
            for (int i = 1; i <= CatchFishPosCount; i++)
            {
                Transform tempPos = catchFishPanel.Find("Pos" + i);
                CatchFishPosList.Add(tempPos);
            }
            SwrilPanel = mTF.Find("AddNodePanel/SwrilPanel");
            SKillPanel = mTF.Find("AddNodePanel/SKillPanel");
            SkillGun = mTF.Find("Gun_Team/Skill_Gun");
            SpecialDeclarePanel = mTF.Find("AddNodePanel/SpecialDeclarePanel");
        }

        public void InitLockPoint()
        {
            for (int i = 1; i <= PreCount; i++)
            {
                GameObject tempPoint = GameObject.Instantiate(LockPoint);
                tempPoint.gameObject.SetActive(false);
                tempPoint.transform.SetParent(GunLockPoint.transform);
                tempPoint.transform.localScale = Vector3.one;
                tempPoint.transform.localPosition = Vector3.zero;
                tempPoint.transform.localRotation = Quaternion.identity;
                LockPointList.Add(tempPoint);
            }
        }

        public Vector2 GetPlayerCatchFishPos(int index = 0)
        {
            CatchFishPosIndex = index > 0 ? index : CatchFishPosIndex + 1;
            if (CatchFishPosIndex > 3)
                CatchFishPosIndex = 1;
            Vector2 catchFishScreenPos = FishGameManager.Instance.UICamera.WorldToScreenPoint(CatchFishPosList[CatchFishPosIndex - 1].position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(FishGameManager.Instance.fishCanvas.GetComponent<RectTransform>(), catchFishScreenPos, FishGameManager.Instance.UICamera, out Vector2 targetPos);
            return targetPos;
        }


        public void RemovePlayerCatchFishPos()
        {
            CatchFishPosIndex--;
            if (CatchFishPosIndex < 0)
                CatchFishPosIndex = 0;
        }

        public void IsShowBetPanel(bool isDisPlay)
        {
            BetPanel.SetActive(isDisPlay);
        }

        public void IsShowLockFishPanel(bool isDisPlay)
        {
            LockFishPanel.SetActive(isDisPlay);
        }

        public void IsShowGunLockPoint(bool isDisPlay)
        {
            GunLockPoint.SetActive(isDisPlay);
        }

        public void IsShowlockRotation(bool isDisPlay)
        {
            LockRotation.SetActive(isDisPlay);
        }

        public void IsShowLockPoint(bool isDisPlay)
        {
            LockPoint.SetActive(isDisPlay);
        }

        public void IsShowLockTips(bool isDisPlay)
        {
            LockTips.SetActive(isDisPlay);
        }

        public void PlayLockTipsAnim()
        {
            LockTipsAnim.Play("LockFishTipsAnim", 0, 0);
        }

        public void SetLockTipsPos(Vector3 targetPos)
        {
            LockTips.transform.position = targetPos;
        }

        public void PlayGunShotAnim()
        {
            if (isFireStorm)
                fireStormAnimator.Play("GunShoot", 0, 0);
            else if (isSpeed)
            {
                gunSpeedAnimator.Play("GunShoot", 0, 0);
                ammunitionAnimator.Play("Ammunition", 0, 0);
            }
            else
                gunNormalAnimator.Play("GunShoot", 0, 0);
        }

        public void PlayFireStormShootAnim(string animName)
        {
            if (!string.IsNullOrEmpty(animName))
                fireStormAnimator.Play(animName, 0, 0);
        }

        public void SetShowGunPanel(bool isFireStorm = false)
        {
            this.isFireStorm = isFireStorm;
            if (isFireStorm)
            {
                gunNormalAnimator.gameObject.SetActive(false);
                gunSpeedAnimator.gameObject.SetActive(false);
                ammunitionAnimator.gameObject.SetActive(false);
                fireStormAnimator.gameObject.SetActive(isFireStorm);
            }
            else
            {
                string animName = isSpeed ? "SpeedGunIdel" : "NormalGunIdel";
                gunBaseAnimator.Play(animName, 0, 0);
                gunNormalAnimator.gameObject.SetActive(!isSpeed);
                gunSpeedAnimator.gameObject.SetActive(isSpeed);
                ammunitionAnimator.gameObject.SetActive(isSpeed);
                fireStormAnimator.gameObject.SetActive(isFireStorm);
            }
        }

        /// <summary>
        /// 炮升级特效
        /// </summary>
        public void IsShowGunParticleEffect()
        {
            //GunParticleEffect.SetActive(false);
            //GunParticleEffect.SetActive(true);
        }

        public void HideGunPanel()
        {
            gunNormalAnimator.gameObject.SetActive (false);
            gunSpeedAnimator.gameObject.SetActive(false);
            fireStormAnimator.gameObject.SetActive(false);
            ammunitionAnimator.gameObject.SetActive(false);
            //for (int i = 0; i < GunLevelAnimList.Count; i++)
            //{
            //    GunLevelAnimList[i].gameObject.SetActive(false);
            //}
        }

        public void IsShowImHere(bool isDisplay)
        {
            ImHere.SetActive(isDisplay);
            ImHereTimer?.Dispose();
            ImHereTimer = new Timer(3000);
            ImHereTimer.Elapsed += HideImHere;
            ImHereTimer.Start();
            showImHere = isDisplay;
        }

        private void HideImHere(object sender, ElapsedEventArgs e)
        {
            showImHere = false;
            ImHereTimer.Stop();
            ImHereTimer?.Dispose();
        }
    }
}

