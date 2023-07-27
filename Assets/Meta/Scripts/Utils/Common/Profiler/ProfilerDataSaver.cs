using UnityEngine;
using System.Text;
using System.Collections;

namespace BagelCode
{
    public class ProfilerDataSaver
    {
        public void StartSave(MonoBehaviour agent, int frames)
        {
            agent.StartCoroutine(SaveProfilerDataCoroutine(frames));
        }

        private IEnumerator SaveProfilerDataCoroutine(int frames)
        {
            // todo shk
            yield break;
        }

    }
}