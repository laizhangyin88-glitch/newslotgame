using System.Collections;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class Ping : MonoBehaviour
    {
        public static GameObject mainPingObj;

        public float duration;

        private void Awake()
        {
            if (mainPingObj == null)
                mainPingObj = gameObject;


#if NEW_NET
            return;
#else
            StopPing();
#endif
        }

        private void OnEnable()
        {
            if (gameObject != mainPingObj && mainPingObj != null)
            {
                mainPingObj.SetActive(false);
            }
        }

        private void OnDisable()
        {
            if (gameObject != mainPingObj && mainPingObj != null)
            {
                mainPingObj.SetActive(true);
            }
        }

        private void StopPing()
        {
            StopCoroutine(SendPing());
            StartCoroutine(CheckSessionAlive());
        }

        private IEnumerator SendPing()
        {
            while (true)
            {
                yield return new WaitForSeconds(duration);

                BagelCodeClientAPI.Ping
                (
                    (response) =>
                    {
                    },
                    (error) =>
                    {
                        StopPing();
                    }
                );
            }
        }

        private IEnumerator CheckSessionAlive()
        {
            while (true)
            {
                var isAlive = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "sessionAlive");

                if (isAlive != null && isAlive.value == true)
                {
                    StartCoroutine(SendPing());
                    break;
                }

                yield return new WaitForSeconds(duration);
            }
        }
    }
}
