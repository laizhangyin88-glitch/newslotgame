using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Meta.Scripts.UI
{
    public class TMP_PlaceholderController : MonoBehaviour
    {
        public TMP_InputField textInputField;
        public Graphic placeholder;

        public void OnSelect()
        {
            placeholder.enabled = false;
        }

        public void OnDeselect()
        {
            placeholder.enabled = string.IsNullOrEmpty(textInputField.text);
        }
    }
}