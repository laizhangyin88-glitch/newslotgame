using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BSS.Utils {
    public static class CameraUtility 
    {
        public static Rect GetWorldRect(this Camera camera) {
            float height = 2f * Camera.main.orthographicSize;
            float width = height * Camera.main.aspect;
            return new Rect(camera.transform.position-new Vector3(width*0.5f,height*0.5f,0f), new Vector2(width, height));
        }
    }
}
