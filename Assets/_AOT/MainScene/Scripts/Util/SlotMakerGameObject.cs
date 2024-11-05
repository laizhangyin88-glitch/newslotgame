using UnityEngine;

namespace SlotMaker
{
    public static class SlotMakerGameObject
    {
        public static GameObject AddChild(this GameObject parent, GameObject child)
        {
            child.transform.SetParent(parent.transform, false);
            return child;
        }

        public static GameObject AddChild(this GameObject parent, string name)
        {
            GameObject child = new GameObject();
            child.name = name;
            parent.AddChild(child);
            return child;
        }

        public static GameObject CopyChild(this GameObject parent, string name)
        {
            GameObject obj = parent.AddChild(new GameObject(name));
            return obj;
        }

        public static GameObject CopyChild(this GameObject parent, GameObject child)
        {
            GameObject obj = Object.Instantiate(child) as GameObject;
            obj.name = child.name;
            parent.AddChild(obj);
            return obj;
        }

        public static bool HasParent(this GameObject obj)
        {
            return obj != null && obj.transform.parent != null;
        }

        public static GameObject GetParent(this GameObject obj)
        {
            return obj.transform.parent.gameObject;
        }

        public static T _GetComponentInParent<T>(this Transform transform) where T : Component
        {
            if (transform == null) return null;

            T found = (T)transform.GetComponent(typeof(T));
            if (found == null && transform.parent != null)
                return _GetComponentInParent<T>(transform.parent);

            return found;
        }

        public static GameObject GetRoot(this GameObject obj)
        {
            return obj.transform.root.gameObject;
        }

        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            T comp = obj.GetComponent<T>();
            if (comp == null) comp = obj.AddComponent<T>();
            return comp;
        }

        public static void DestroyImmediateChildren(this GameObject go)
        {
            Transform transform = go.transform;
            int childCount = transform.childCount;
            Transform[] objsToDestroy = new Transform[childCount];

            for (int i = 0; i < childCount; ++i)
            {
                var child = transform.GetChild(i);
#if UNITY_EDITOR
                if (UnityEditor.PrefabUtility.IsAnyPrefabInstanceRoot(child.gameObject) || !UnityEditor.PrefabUtility.IsPartOfAnyPrefab(child.gameObject))
                    objsToDestroy[i] = child;
                else
                    Debug.Log($"\'{child.name}\' is a part of a prefab, so this cannot be destroyed.", child);
#else
                objsToDestroy[i] = child;
#endif
            }

            for (int i = 0; i < childCount; ++i)
            {
                if (objsToDestroy[i] != null && objsToDestroy[i].gameObject != null)
                    GameObject.DestroyImmediate(objsToDestroy[i].gameObject);
            }
        }

        public static void DestroyChildren(this GameObject go)
        {
            Transform transform = go.transform;
            int childCount = transform.childCount;
            Transform[] objsToDestroy = new Transform[childCount];

            for (int i = 0; i < childCount; ++i)
            {
                objsToDestroy[i] = transform.GetChild(i);
            }

            for (int i = 0; i < childCount; ++i)
            {
                objsToDestroy[i].gameObject.DestroyThis();
            }
        }

        public static void DestroyThis(this Object obj)
        {
#if UNITY_EDITOR
                if (Application.isPlaying)
                    GameObject.Destroy(obj);
                else
                    GameObject.DestroyImmediate(obj);
#else
                GameObject.Destroy(obj);
#endif
        }

        public static void Visit(this GameObject go, System.Action<GameObject> visitor)
        {
            for (int i = 0; i < go.transform.childCount; ++i)
            {
                GameObject child = go.transform.GetChild(i).gameObject;
                visitor(child);
                child.Visit(visitor);
            }
        }
    }
}
