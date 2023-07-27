using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker
{
    public class AniPreserveParam
    {
        public AnimatorControllerParameterType type;
        public string paramName;
        public object data;

        public void SetParam(Animator anim, string paramName, AnimatorControllerParameterType type)
        {
            this.type = type;
            this.paramName = paramName;
            switch (type)
            {
                case AnimatorControllerParameterType.Int:
                    this.data = (int)anim.GetInteger(paramName);
                    break;
                case AnimatorControllerParameterType.Float:
                    this.data = (float)anim.GetFloat(paramName);
                    break;
                case AnimatorControllerParameterType.Bool:
                    this.data = (bool)anim.GetBool(paramName);
                    break;
            }
        }
    }

    public class AnimationPreserve
    {
        private Animator animator;
        private List<AniPreserveParam> paramList = new List<AniPreserveParam>();

        public void PrintParams()
        {
#if UNITY_EDITOR
            if(paramList.Count == 0)
            {
                Debug.Log("No Params");
                return;
            }

            for(int i=0; i<paramList.Count; ++i)
            {
                Debug.Log(string.Format("{0} : {1}", paramList[i].paramName, paramList[i].data));
            }
#endif
        }

        public void Init(Animator ani)
        {
            animator = ani;
            paramList.Clear();
        }

        public void SaveState()
        {
            if(animator == null || !animator.isActiveAndEnabled) return;

            paramList.Clear();
            for (int i = 0; i < animator.parameters.Length; i++)
            {
                AnimatorControllerParameter param = animator.parameters[i];

                AniPreserveParam preserveParam = new AniPreserveParam();
                preserveParam.SetParam(animator, param.name, param.type);

                paramList.Add(preserveParam);
            }
        }

        public void Preserve()
        {
            if(animator == null || !animator.isActiveAndEnabled) return;

            foreach(AniPreserveParam p in paramList)
            {
                switch (p.type)
                {
                    case AnimatorControllerParameterType.Int:
                        animator.SetInteger(p.paramName,(int)p.data);
                        break;
                    case AnimatorControllerParameterType.Float:
                        animator.SetFloat(p.paramName,(float)p.data);
                        break;
                    case AnimatorControllerParameterType.Bool:
                        animator.SetBool(p.paramName,(bool)p.data);
                        break;
                }
            }
        }
    }

/*
    public class TestAniPreserveParam
    {
        public string paramName;
        public object data;

        public void SetParam(string paramName, object value)
        {
            this.paramName = paramName;
            this.data = value;
        }
    }

    public class TestAnimationPreserve
    {
        private Animator animator;
        private List<TestAniPreserveParam> paramList = new List<TestAniPreserveParam>();

        public void PrintParams()
        {
#if UNITY_EDITOR
            if(paramList.Count == 0)
            {
                Debug.Log("No Params");
                return;
            }

            for(int i=0; i<paramList.Count; ++i)
            {
                Debug.Log(string.Format("{0} : {1}", paramList[i].paramName, paramList[i].data));
            }
#endif
        }

        public void Init(Animator ani)
        {
            animator = ani;
            paramList.Clear();
        }

        public void SaveState(string name, object value)
        {
            if(animator == null) return;

            paramList.Clear();

            TestAniPreserveParam preserveParam = new TestAniPreserveParam();
            preserveParam.SetParam(name, value);
            paramList.Add(preserveParam);
        }

        public void Preserve()
        {
            if(animator == null) return;

            foreach(TestAniPreserveParam p in paramList)
            {
                if(p.data is int)
                {
                    animator.SetInteger(p.paramName,(int)p.data);
                }
                else if(p.data is float)
                {
                    animator.SetFloat(p.paramName,(float)p.data);
                }
                else
                {
                    animator.SetBool(p.paramName,(bool)p.data);
                }
            }
        }
    }
    */
}
