using UnityEngine;

namespace SlotMaker
{
    public class AnimatorButton : MonoBehaviour
    {   
        public void OnButtonEnable(bool Enabled)
        {
            GetComponent<Animator>().SetBool("Enabled", Enabled);
        }

        public void OnButtonClick()
        {
            GetComponent<Animator>().SetTrigger("Click");
        }

        public void OnButtonPress(bool pressed)
        {
            GetComponent<Animator>().SetBool("Pressed", pressed);
        }
    }
}