using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace BagelCode
{

// TODO: Replace this with proper tweener after SlotMaker 2019.3
public class FourGodsFlagController : MonoBehaviour
{
    public Slider flagSlider;
    public AnimationCurve flagCurve = AnimationCurve.Constant(0f, 1f, 0.5f);
    public float flagDuration;
    private float flagTarget;

    public Slider markerSlider;
    public AnimationCurve markerCurve = AnimationCurve.Constant(0f, 1f, 0.5f);
    public float markerInitialDirection = 1f;
    private float markerDirection = 0f;

    public float markerDuration;
    public int markerAdditionalMoveCount = 1;

    public float markerIntroDuration;

    public UnityEvent onProbabilatyUpdated;
    public UnityEvent onMarkerStopped;

    public void UpdateProbability(float probability)
    {
        flagSlider.value = 0f;
        flagTarget = probability;

        StartCoroutine(UpdateProbabilityCoroutine());
    }

    private IEnumerator UpdateProbabilityCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < flagDuration)
        {
            flagSlider.value = flagTarget * flagCurve.Evaluate(elapsedTime / flagDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        flagSlider.value = flagTarget;
        onProbabilatyUpdated.Invoke();
    }

    public void MoveMarker(float targetValue) 
    {
        if (markerDirection == 0f)
            markerDirection = markerInitialDirection;

        float currentValue = markerSlider.value;

        float startMoveDistance = (markerDirection > 0f) ? 1.0f - currentValue : currentValue;
        float lastMoveDistance = ((markerDirection > 0f) == (markerAdditionalMoveCount % 2 == 0)) ? 1.0f - targetValue : targetValue;
        float moveDistance = startMoveDistance + (float)markerAdditionalMoveCount + lastMoveDistance;

        StartCoroutine(MoveMarkerCoroutine(markerDirection, moveDistance, markerDuration, markerAdditionalMoveCount));
    }

    private IEnumerator MoveMarkerCoroutine(float direction, float distance, float duration, int additionalMoveCount)
    {
        float elapsedTime = 0f;
        float startValue = markerSlider.value;

        while (elapsedTime < duration)
        {
            float movedDistance = distance * markerCurve.Evaluate(elapsedTime / duration);
            markerSlider.value = Mathf.PingPong(startValue + direction * movedDistance, 1.0f);
           
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        markerSlider.value = Mathf.PingPong(startValue + direction * distance, 1.0f);
        
        // Update next marker direction
        markerDirection = (additionalMoveCount % 2 != 0) ? direction : -direction;
        onMarkerStopped.Invoke();
    }

    public void IntroMarkerMovement(float targetValue)
    {
        float currentValue = markerSlider.value;
        float moveDistance = currentValue - targetValue;

        StartCoroutine(MoveMarkerCoroutine(-1.0f, moveDistance, markerIntroDuration, 0));
    }
}

}
