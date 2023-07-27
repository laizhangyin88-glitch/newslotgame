using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions.Math
{
    [Category("✶ Slots/Math")]
    public class MaxInt : ActionTask
    {
        public BBParameter<int> a;
        public BBParameter<int> b;

        [BlackboardOnly]
        public BBParameter<int> saveAs;

        protected override string info
        {
            get { return string.Format("{0} = Max({1}, {2})", saveAs, a, b); }
        }

        protected override void OnExecute()
        {
            saveAs.value = Mathf.Max(a.value, b.value);

            EndAction();
        }
    }

    [Category("✶ Slots/Math")]
    public class MaxFloat : ActionTask
    {
        public BBParameter<float> a;
        public BBParameter<float> b;

        [BlackboardOnly]
        public BBParameter<float> saveAs;

        protected override string info
        {
            get { return string.Format("{0} = Max({1}, {2})", saveAs, a, b); }
        }

        protected override void OnExecute()
        {
            saveAs.value = Mathf.Max(a.value, b.value);

            EndAction();
        }
    }

    [Category("✶ Slots/Math")]
    public class MinInt : ActionTask
    {
        public BBParameter<int> a;
        public BBParameter<int> b;

        [BlackboardOnly]
        public BBParameter<int> saveAs;

        protected override string info
        {
            get { return string.Format("{0} = Min({1}, {2})", saveAs, a, b); }
        }

        protected override void OnExecute()
        {
            saveAs.value = Mathf.Min(a.value, b.value);

            EndAction();
        }
    }

    [Category("✶ Slots/Math")]
    public class MinFloat : ActionTask
    {
        public BBParameter<float> a;
        public BBParameter<float> b;

        [BlackboardOnly]
        public BBParameter<float> saveAs;

        protected override string info
        {
            get { return string.Format("{0} = Min({1}, {2})", saveAs, a, b); }
        }

        protected override void OnExecute()
        {
            saveAs.value = Mathf.Min(a.value, b.value);

            EndAction();
        }
    }
}