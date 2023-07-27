using UnityEngine;
using System.Collections;

public class AnchoredPooledObject : SlotMaker.PooledObject
{
    public Transform anchor;
    private bool isAnchored = false;
    public float distance = 1;

    public void Awake()
    {
        if (anchor != null) SetAnchor(anchor);
    }

    public void LateUpdate()
    {
        if (isAnchored)
        {
            if (anchor == null || !anchor.gameObject.activeInHierarchy)
            {
                isAnchored = false;
                ReturnToPool();
            }
            else
            {
                transform.position = new Vector3(anchor.position.x - distance, transform.position.y, transform.position.z);    
            }
        }
    }

    public void SetAnchor(Transform anchor)
    {
        this.anchor = anchor;
        isAnchored = true;
    }
}
