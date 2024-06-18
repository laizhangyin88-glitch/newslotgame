using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.Json;
using static UnityEngine.UI.GridLayoutGroup;
#if DEV
using SlotMaker.TestSuite;
#endif

namespace BagelCode.Tasks.Actions
{
	[Category("★ BagelCode/System")]
	public class InitializeSlotMaker : ActionTask
	{
	    protected override void OnExecute()
        {
            MetaSystem.InitializeMetaSystem(new V3MetaSystem());
			Analytics.InitializeAnalytics(new V3Analytics());

			StringTableUtils.customProvider = new BagelCodeFormatProvider();

			// Schema
            BlackboardJson.CurrentNamingStrategy = new SnakeToCamelCaseNamingStrategy();
            BlackboardJson.CurrentJsonSerializerStrategy = new BlackboardJsonSerializerStrategy { 
                NamingStrategy = BlackboardJson.CurrentNamingStrategy 
            };

            BlackboardJson.LoadSchema(AssetBundleManager.LoadAsset<TextAsset>("models", "ContentsModels").text);

#if DEV && !NEW_NET
			TestSuiteServer.InitializeServer(new TestSuitePlayFabServer());
#endif

            EndAction();
	    }
	}
}
