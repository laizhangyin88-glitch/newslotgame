using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideGameObjects : MonoBehaviour
{
    public GameObject[] gameObjects;

    private void Start()
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i].gameObject.SetActive(false);
        }
    }
}
