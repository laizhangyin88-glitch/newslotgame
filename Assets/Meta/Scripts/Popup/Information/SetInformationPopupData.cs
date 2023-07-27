using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode.MetaGame.Tasks.Actions
{
    [Category("★ BagelCode/Popup")]
    public class SetInformationPopupData : ActionTask<Blackboard>
    {
        public BBParameter<GameObject> targetPopupObj;

        public BBParameter<string> bundle;
        public BBParameter<bool> combineApplicationType;

        public BBParameter<int> pageCount;
        public BBParameter<string> dotPrefabFormat;
        public BBParameter<string> infoPrefabFormat;
        public BBParameter<string> infoTextFormat;
        public BBParameter<List<List<string>>> argsValue;
        public BBParameter<List<List<string>>> datasValue;

        protected override string info
        {
            get { return "Set Information Popup Data"; }
        }

        protected override void OnExecute()
        {
            if(targetPopupObj != null)
            {
                var bb = targetPopupObj.value.GetComponent<Blackboard>();
                if(bb != null)
                {
                    BlackboardUtils.SetOrCreateValue<int>(bb, "pageCount", pageCount.value);
                    BlackboardUtils.SetOrCreateValue<string>(bb, "bundle", GetBundleName());
                    BlackboardUtils.SetOrCreateValue<string>(bb, "dotPrefabFormat", dotPrefabFormat.value);
                    BlackboardUtils.SetOrCreateValue<string>(bb, "infoPrefabFormat", infoPrefabFormat.value);
                    BlackboardUtils.SetOrCreateValue<string>(bb, "infoTextFormat", infoTextFormat.value);


                    // Set Args List
                    var argsList = BlackboardUtils.GetOrCreateVariable<List<object[]>>(bb, "argsList");
                    SetArgsList(argsList, argsValue);
                    //if (argsList != null && argsValue.value.Count > 0)
                    //{
                    //    argsList.value = new List<object[]>();

                    //    for(int i=0; i < argsValue.value.Count; ++i)
                    //    {
                    //        object[] args = new object[argsValue.value[i].Count];
                    //        for(int k=0; k < argsValue.value[i].Count; ++k)
                    //        {
                    //            var arg = agent.GetVariable( argsValue.value[i][k]);
                    //            args[k] = arg == null ? null :arg.value;
                    //        }
                    //        argsList.value.Add(args);
                    //    }
                    //}
                    // Set Datas List
                    var datasList = BlackboardUtils.GetOrCreateVariable<List<object[]>>(bb, "datasList");
                    SetArgsList(datasList, datasValue);
                }
            }

            EndAction();
        }

        protected string GetBundleName()
        {
            return combineApplicationType.value ? ApplicationSettings.MakeApplicationBundleName(bundle.value) : bundle.value;
        }

        protected void SetArgsList(Variable<List<object[]>> argsList, BBParameter<List<List<string>>> _argsValue)
        {
            if (argsList != null && _argsValue != null && _argsValue.value != null && _argsValue.value.Count > 0)
            {
                argsList.value = new List<object[]>();

                for (int i = 0; i < _argsValue.value.Count; ++i)
                {
                    object[] args = new object[_argsValue.value[i].Count];
                    for (int k = 0; k < _argsValue.value[i].Count; ++k)
                    {
                        var arg = agent.GetVariable(_argsValue.value[i][k]);
                        args[k] = arg == null ? null : arg.value;
                    }
                    argsList.value.Add(args);
                }
            }
        }
    }
}
