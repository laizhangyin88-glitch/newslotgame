using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BSS.Utils {
    public static class Vector3Utility 
    {
        public static Vector3 SetX(this Vector3 vector, float x) {
            return new Vector3(x, vector.y, vector.z);
        }
        public static Vector3 SetY(this Vector3 vector, float y) {
            return new Vector3(vector.x, y, vector.z);
        }
        public static Vector3 SetZ(this Vector3 vector, float z) {
            return new Vector3(vector.x, vector.y, z);
        }
        public static Vector3 AddX(this Vector3 vector, float x) {
            return vector + new Vector3(x, 0f, 0f);
        }
        public static Vector3 AddY(this Vector3 vector, float y) {
            return vector + new Vector3(0f, y, 0f);
        }
        public static Vector3 AddZ(this Vector3 vector, float z) {
            return vector + new Vector3(0f, 0f, z);
        }

        public static Vector3 Multiply(this Vector3 origin, Vector3 other) {
            return new Vector3(origin.x * other.x, origin.y * other.y, origin.z * other.z);
        }
        public static Vector3 Devide(this Vector3 origin, Vector3 other) {
            return new Vector3(origin.x / other.x, origin.y / other.y, origin.z / other.z);
        }
    }

    public static class Vector2Utility 
    {
        public static Vector2 SetX(this Vector2 vector, float x) {
            return new Vector2(x, vector.y);
        }
        public static Vector2 SetY(this Vector2 vector, float y) {
            return new Vector2(vector.x, y);
        }
        public static Vector2 AddX(this Vector2 vector, float x) {
            return new Vector2(x, vector.y);
        }
        public static Vector2 AddY(this Vector2 vector, float y) {
            return new Vector2(vector.x, y);
        }
        public static Vector2 Multiply(this Vector2 origin, Vector2 other) {
            return new Vector2(origin.x * other.x, origin.y * other.y);
        }
        public static Vector2 Devide(this Vector2 origin, Vector2 other) {
            return new Vector2(origin.x / other.x, origin.y / other.y);
        }
    }
}
