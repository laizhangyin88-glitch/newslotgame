using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        // Spin
        if (Input.GetKeyDown(KeyCode.F5))
        {
            GameObject go = GameObject.Find("Door Chose/Animator");
            Animator animator = go.GetComponent<Animator>();
            animator.SetBool("Next", false);

            Debug.Log("i am F5");
        }
        // Spin
        if (Input.GetKeyUp(KeyCode.F6))
        {
            GameObject go = GameObject.Find("Door Chose/Animator");
            Animator animator = go.GetComponent<Animator>();
            animator.SetInteger("ChoseDoor", 0);

            Debug.Log("i am F6");
        }


        // K2
        if ((Input.GetKeyDown(KeyCode.F7)))
        {


        }

    }
}
