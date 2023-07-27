using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NodeCanvas.Framework;
[ExecuteInEditMode]
public class DissolveController : MonoBehaviour
{
    private Material material;
    [Range(0, 1)]
    public float alphaCut;

    public Color setColor = new Color();

    public float AlphaCut
    {
        get
        {
            return alphaCut;
        }
        set
        {
            material.SetFloat("_Level", value);
        }
    }

    public Color SetColor
    {
        get
        {
            return setColor;
        }
        set
        {
            material.SetColor("_Color", value);
        }
    }



    //public Variable<float> alpha = new Variable<float>();
    //private void UpdateAlpha(string name, object value)
    //{
    //    AlphaCut = (float)value;

    //}
    //private void OnEnable()
    //{
    //    alpha.onValueChanged += UpdateAlpha;
    //}
    //private void OnDisable()
    //{
    //    alpha.onValueChanged -= UpdateAlpha;
    //}


    void Awake()
    {
        GetMaterial();
        //Debug.Log("Alpha Cut : " + alphaCut);
    }


    void Update()
    {
        AlphaCut = alphaCut;
        SetColor = setColor;

    }


    void GetMaterial()
    {
        if (material == null)
            material = transform.GetComponent<Image>()?.material;
        if (material != null)
        {
            alphaCut = material.GetFloat("_Level");
            setColor = material.GetColor("_Color");
        }

    }
}
