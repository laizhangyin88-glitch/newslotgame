using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker.Slots.Strategy;
using SlotMaker.IoC.Strategy.Tween;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    public class WinLineBorder : MonoBehaviour
    {
        public RectTransform rectTransform;
        public Animator animator;

        [InlineEditor]
        public SymbolPositionStrategy positionStrategy;

        [InlineEditor]
        public TweenStrategy tween;

        public float duration;

        protected Coroutine coroutine;
        protected float playback;
        protected Vector2 from;
        protected Vector2 to;

        protected const string ACTIVE_BORDER = "Active";

        public void ShowLineBorder(int columnCount, int rowCount, int x, int y, bool activeBorder, bool reset)
        {
            animator.SetBool(ACTIVE_BORDER, activeBorder);
            
            var newPosition = positionStrategy.GetPosition(columnCount, rowCount, x, y, 1, 1, 0, 0);
            if (coroutine != null) StopCoroutine(coroutine);
            if (!reset && duration > 0f)
                coroutine = StartCoroutine(Tween(newPosition));
            else
                rectTransform.anchoredPosition = newPosition;
        }

        protected IEnumerator Tween(Vector2 targetPosition)
        {
            playback = 0f;
            from = rectTransform.anchoredPosition;
            to = targetPosition;

            while (playback < duration)
            {
                yield return null;
                
                playback = Mathf.Clamp(playback + Time.deltaTime, 0f, duration);
                rectTransform.anchoredPosition = tween.UpdateTween(from, to, playback / duration);
            }
            rectTransform.anchoredPosition = to;
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        /// EDITOR
        //////////////////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
            if (animator == null) animator = GetComponent<Animator>();
        }
#endif
    }
}