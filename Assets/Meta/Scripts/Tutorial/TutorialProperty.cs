using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

namespace BagelCode
{

public class TutorialProperty : MonoBehaviour
{
    public Blackboard tutorial { get; set; }
    public string key { get { return GetTutorialProperty<string>("key"); } }
    public string category { get { return GetTutorialProperty<string>("category"); } }
    public Blackboard tutorialBB { get { return GetTutorialProperty<Blackboard>("tutorialBB"); } }
    public RectTransform rectTransform { get { return GetTutorialProperty<RectTransform>("rectTransform"); } }
    public string eventName { get { return GetTutorialProperty<string>("event"); } }
    public bool wait { get { return GetTutorialProperty<bool>("wait"); } }
    public string waitEvent { get { return GetTutorialProperty<string>("waitEvent"); } }
    public string text { get { return GetTutorialProperty<string>("text"); } }
    public TextAnchor anchor { get { return GetTutorialProperty<TextAnchor>("anchor"); } }
    public Vector2 offset { get { return GetTutorialProperty<Vector2>("offset"); } }
    public TextAnchor pointerAnchor { get { return GetTutorialProperty<TextAnchor>("pointerAnchor"); } }
    public Vector2 pointerOffset { get { return GetTutorialProperty<Vector2>("pointerOffset"); } }
    public Vector3 pointerRotation { get { return GetTutorialProperty<Vector3>("pointerRotation"); } }
    public bool rayMask { get { return GetTutorialProperty<bool>("rayMask"); } }
    public float showDelay { get { return GetTutorialProperty<float>("showDelay"); } }
    public float popupDelay { get { return GetTutorialProperty<float>("popupDelay"); } }
    public float pointerDelay { get { return GetTutorialProperty<float>("pointerDelay"); } }

    private T GetTutorialProperty<T>(string key)
    {
        return tutorial.GetValue<T>(key);
    }
}

}
