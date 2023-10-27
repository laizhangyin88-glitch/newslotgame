using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var test = Resources.Load("ApplicationSettings");
        Debug.Log(test);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
