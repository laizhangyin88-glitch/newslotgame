using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode
{
    public class LobbyBottomBadgeListener : MonoBehaviour
    {
        private Animator _animator;
        private Animator animator
        {
            get
            {
                if (_animator == null) _animator = GetComponent<Animator>();
                return _animator;
            }
        }

        public bool IsBadgeAvailable()
        {
            if (animator == null) return gameObject.activeSelf;
            return animator.GetInteger("value") != 0;
        }
    }
}
