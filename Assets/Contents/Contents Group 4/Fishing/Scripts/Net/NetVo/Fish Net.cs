using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishNet
    {
        private GameObject _gameObject;
        private bool IsPlayAnim;
        private float currentUpdateTime;
        private float NetInteralTime;
        private Animator Animator;
        public FishNet(GameObject gameObj)
        {
            _gameObject = gameObj;
            Init();
        }

        private void Init()
        {
            InitData();
            FindView();
            InitViewData();
        }

        private void InitData()
        {
            IsPlayAnim = false;
            currentUpdateTime = 0;
            NetInteralTime = 1.5f;
        }

        private void FindView()
        {
            Animator = _gameObject.GetComponent<Animator>();
        }

        private void InitViewData()
        {
            ResetNetState();
        }

        public void ResetNetState()
        {
            currentUpdateTime = 0;
            SetPlayAnimState(false);
        }

        public void PlayNetAudio()
        {

        }

        public void SetNetPosition(Vector3 transPos)
        {
            _gameObject.transform.localPosition = transPos;
        }

        public void IsEnableAnimator(bool isEnable)
        {
            if (Animator != null)
            {
                Animator.enabled = isEnable;
            }
        }

        public void SetPlayAnimState(bool isEnable)
        {
            IsPlayAnim = isEnable;
        }

        public void PlayNetAnimator(string animName)
        {
            if (Animator != null)
            {
                Animator.Play(animName, 0, 0);
            }
        }

        public void UpdateAnimaState()
        {
            if (IsPlayAnim)
            {
                currentUpdateTime += Time.deltaTime;
                if (currentUpdateTime >= NetInteralTime)
                {
                    IsPlayAnim = false;
                    currentUpdateTime = 0;
                    FishNetManager.Instance.AddNet(this);
                }
            }
        }
        public void Update()
        {
            UpdateAnimaState();
        }

        public void Destroy()
        {
            IsPlayAnim = false;
            _gameObject.transform.localPosition = new Vector3(10000, 10000);
        }
    }
}
