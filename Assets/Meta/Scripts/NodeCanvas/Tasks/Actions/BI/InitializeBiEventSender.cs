using UnityEngine;
using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class InitializeBiEventSender : ActionTask
{
    protected override void OnExecute()
    {
        var awsAccessKey = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "/values/misc/CLIENT_ANALYTIC_EVENT_SENDER_AWS_ACCESS_KEY");
        var awsSecretKey = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "/values/misc/CLIENT_ANALYTIC_EVENT_SENDER_AWS_SECRET_KEY");
        var awsHost      = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "/values/misc/CLIENT_ANALYTIC_EVENT_SENDER_AWS_HOST");
        var appName      = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "/values/misc/CLIENT_ANALYTIC_EVENT_SENDER_APP_NAME");
        var env          = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "/values/misc/CLIENT_ANALYTIC_EVENT_SENDER_ENV");

        if(awsAccessKey == null || awsSecretKey == null || awsHost == null || appName == null || env == null)
        {
            Debug.LogError("[Blackboard] Null variable founded in InitializeKey in misc");
            EndAction(true);    
        }

        BiEventSender.Instance.Initialize(awsAccessKey.value, awsSecretKey.value, awsHost.value, appName.value, env.value);
        EndAction();
    }
}

}
