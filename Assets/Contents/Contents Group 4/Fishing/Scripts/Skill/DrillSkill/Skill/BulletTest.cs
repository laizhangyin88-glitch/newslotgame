using BagelCode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class BulletTest : MonoBehaviour
{
    public Canvas canvas;
    public GameObject trailPrefab;
    Vector3 m_direction;
    float m_moveSpeed;
    CanvasScaler m_canvasScaler;
    float ResolutionWidthHalf;
    float ResolutionHeightHalf;
    float hightOffset = 170;

    void Start()
    {
        m_moveSpeed = 1000;
        m_canvasScaler = canvas.GetComponent<CanvasScaler>();
        ResolutionHeightHalf = m_canvasScaler.referenceResolution.y * 0.5f;
        ResolutionWidthHalf = m_canvasScaler.referenceResolution.x * 0.5f;
    }

    void Update()
    {
        BulletMoving();
    }

    public static float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
    {
        return Vector3.Angle(dirA, dirB) * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0 ? -1 : 1);
    }

    public void CreateDrillTrail(Vector3 pos)
    {
        GameObject trail = Instantiate(trailPrefab);
        trail.transform.parent = transform.parent;
        trail.transform.localScale = Vector3.one;
        trail.transform.localPosition = pos;
        trail.transform.localEulerAngles = transform.localEulerAngles;
    }

    public void BulletMoving()
    {
        m_direction = transform.TransformDirection(Vector3.up);
        m_direction = m_direction.normalized;
        float tempMoveSpeed = m_moveSpeed * (ResolutionWidthHalf / 1920);
        Vector3 pos = transform.localPosition + new Vector3(m_direction.x, m_direction.y, 0) * Time.deltaTime * tempMoveSpeed;
        if (pos.x > ResolutionWidthHalf)
        {
            Vector3 re = Vector3.Reflect(m_direction, -Vector3.right);
            transform.localEulerAngles = new Vector3(0, 0, AngleAroundAxis(Vector3.up, re, Vector3.forward));
            pos = new Vector3(ResolutionWidthHalf, transform.localPosition.y, transform.localPosition.z);
            m_direction = transform.TransformDirection(Vector3.up);
        }
        if (pos.x < -ResolutionWidthHalf)
        {
            Vector3 re = Vector3.Reflect(m_direction, Vector3.right);
            transform.localEulerAngles = new Vector3(0, 0, AngleAroundAxis(Vector3.up, re, Vector3.forward));
            pos = new Vector3(-ResolutionWidthHalf, transform.localPosition.y, transform.localPosition.z);
            m_direction = transform.TransformDirection(Vector3.up);
        }
        if (pos.y + hightOffset > ResolutionHeightHalf)
        {
            CreateDrillTrail(pos);
            Vector3 re = Vector3.Reflect(m_direction, -Vector3.up);
            transform.localEulerAngles = new Vector3(0, 0, AngleAroundAxis(Vector3.up, re, Vector3.forward));
            pos = new Vector3(transform.localPosition.x, ResolutionHeightHalf - hightOffset, transform.localPosition.z);
            m_direction = transform.TransformDirection(Vector3.up);
        }
        if (pos.y - hightOffset < -ResolutionHeightHalf)
        {
            CreateDrillTrail(pos);
            Vector3 re = Vector3.Reflect(m_direction, Vector3.up);
            transform.localEulerAngles = new Vector3(0, 0, AngleAroundAxis(Vector3.up, re, Vector3.forward));
            pos = new Vector3(transform.localPosition.x, -ResolutionHeightHalf + hightOffset, transform.localPosition.z);
            m_direction = transform.TransformDirection(Vector3.up);
        }

        transform.localPosition = pos;
    }
}
