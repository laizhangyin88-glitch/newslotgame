using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode {
    public class AutoDestroyer : MonoBehaviour {
        private GameObject target;
        private bool isInit = false;

        public static AutoDestroyer ApplyChaining(GameObject owner,GameObject chainTarget) {
            var destroyer=owner.AddComponent<AutoDestroyer>();
            destroyer.target = chainTarget;
            destroyer.isInit = true;
            return destroyer;
        }
        public void ChangeTarget(GameObject chainTarget) {
            target = chainTarget;
            isInit = true;
        }

        private void Update() {
            if (isInit && target == null) {
                Destroy(gameObject);
            }
        }
    }
}
