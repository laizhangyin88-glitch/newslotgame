using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NodeCanvas.Framework;
[ExecuteInEditMode]
public class GrayScaleController : MonoBehaviour
{
    private Material material;
    [Range(0, 1)]
    public float grayScale;

    public float GrayScale
    {
        get
        {
            return grayScale;
        }
        set
        {
            material.SetFloat("_EffectAmount", value);
        }
    }


    void Awake()
    {
        GetMaterial();
    }


    void Update()
    {
        GrayScale = grayScale;

    }


    void GetMaterial()
    {
        if (material == null)
            material = transform.GetComponent<Image>()?.material;
        if (material != null)
        {
            grayScale = material.GetFloat("_EffectAmount");
        }

    }
}
