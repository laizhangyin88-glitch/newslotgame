using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SlotMaker
{
    public class SymbolCachingObjectRestorer : MonoBehaviour
    {
        public bool ignoreSymbolsInReelBuffer = true;

        public void RestoreCachingObject(BaseSymbol dst, BaseSymbol src)
        {
            if (ignoreSymbolsInReelBuffer && !dst.reel.ContainsSymbol(dst.column, dst.row))
                return;

            var dstSymbolController = dst.GetComponent<SymbolController>();
            var srcSymbolController = src.GetComponent<SymbolController>();

            if (srcSymbolController == null)
                return;

            List<GameObject> dstCachingObjects = dstSymbolController.cachingObjects;
            List<GameObject> srcCachingObjects = srcSymbolController.cachingObjects;

            dstCachingObjects.Clear();

            for (int i = 0; i < srcCachingObjects.Count; i++)
            {
                var srcCachingObject = srcCachingObjects[i];

                dstCachingObjects.Add(srcCachingObject);

                if (srcCachingObject != null)
                {
                    srcCachingObject.transform.SetParent(dst.transform, false);
                }

                // set it as null instead of Clear() to keep length of cachingObjects list so symbols still can reference to it.
                srcCachingObjects[i] = null;
            }
        }
    }
}
