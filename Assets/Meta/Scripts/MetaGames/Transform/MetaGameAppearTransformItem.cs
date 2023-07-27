using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class MetaGameAppearTransformItem : MonoBehaviour
    {
        public string id;

        private void OnEnable()
        {
            if(string.IsNullOrEmpty(id)) return;

            MetaGameAppearTransformManager.Instance.Regist(id, transform);
        }

        private void OnDisable()
        {
            if(string.IsNullOrEmpty(id)) return;
            if(MetaGameAppearTransformManager.Instance == null) return;

            MetaGameAppearTransformManager.Instance.UnRegist(id, transform);
        }
    }
}
