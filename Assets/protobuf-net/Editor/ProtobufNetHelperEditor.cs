using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;
using ProtoBuf.Meta;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
using Assembly = System.Reflection.Assembly;

public class ProtobufNetHelperEditor
{
    public const string protobufNetDirRoot = "Assets/protobuf-net";
    public const string protobuf_net_dll_path = "Assets/protobuf-net/Plugins/protobuf-net.dll";

    /// <summary>
    /// 示例类型,用于从该类型所在的Assembly中获取到所有protobuf能够使用的类型,每个相关dll中只需要填一个即可
    /// </summary>
    public static List<Type> exampleTypes = new List<Type>()
    {
        typeof(config.Test),
        typeof(rpc.RpcPacket),
        typeof(hall.HeartHeat),
        typeof(hall.RoomStatusInfo),
        typeof(hall.LoginHallReq),
        typeof(hall.LoginHallRsp),
        typeof(hall.EnterGameReq),
        typeof(hall.EnterGameRsp),
        typeof(hall.ExitGameReq),
        typeof(hall.ExitGameRsp),
        typeof(fishMsg.GameStatusReq),
        typeof(fishMsg.GameStatusRsp),
        typeof(fishMsg.PromptInfoRsp),
        typeof(fishMsg.DoubleGunOnOffRsp),
        typeof(fishMsg.FishInfo),
        typeof(fishMsg.FishListRsp),
        typeof(fishMsg.ShootBulletReq),
        typeof(fishMsg.ShootBulletRsp),
        typeof(fishMsg.HitfishReq),
        typeof(fishMsg.KillFishRsp),
        typeof(fishMsg.AutoShootReq),
        typeof(fishMsg.LockOnOffReq),
        typeof(fishMsg.LockOnOffRsp),
        typeof(fishMsg.LockFishReq),
        typeof(fishMsg.LockFishRsp),
        typeof(fishMsg.BulletSpeedReq),
        typeof(fishMsg.BulletSpeedRsp),
        typeof(fishMsg.CannonInfo),
        typeof(fishMsg.UserInfo),
        typeof(fishMsg.ChangeCannonReq),
        typeof(fishMsg.ChangeCannonRsp),
        typeof(fishMsg.UserMoneyRsp),
        typeof(fishMsg.UserEnterDeskRsp),
        typeof(fishMsg.UserLeaveDeskRsp),
        typeof(fishMsg.ChangeSceneRsp),
        typeof(fishMsg.FreezeFish),
        typeof(fishMsg.FreezeFishesRsp),
        typeof(fishMsg.EnterGameReq),
        typeof(fishMsg.EnterGameRsp),
        typeof(fishMsg.UserStatus),
        typeof(fishMsg.UserStatusRsp),
        typeof(fishMsg.ExitGameReq),
        typeof(fishMsg.ExitGameRsp),
        typeof(fishMsg.CreateDianCiCannonRsp),
        typeof(fishMsg.DianCiCannonAimReq),
        typeof(fishMsg.DianCiCannonAimRsp),
        typeof(fishMsg.DianCiCannonShootReq),
        typeof(fishMsg.DianCiCannonShootRsp),
        typeof(fishMsg.DianCiCannonHitFishReq),
        typeof(fishMsg.DianCiCannonDestroyRsp),
        typeof(fishMsg.CreateZuanTouRsp),
        typeof(fishMsg.ZuanTouAimReq),
        typeof(fishMsg.ZuanTouAimRsp),
        typeof(fishMsg.ZuanTouShootReq),
        typeof(fishMsg.ZuanTouShootRsp),
        typeof(fishMsg.ZuanTouHitFishReq),
        typeof(fishMsg.ZuanTouBombRsp),
        typeof(fishMsg.SomeZuanTouInfo),
        typeof(fishMsg.CreateSomeZuanTouRsp),
        typeof(fishMsg.SomeZuanTouShootRsp),
        typeof(fishMsg.SomeZuanTouHitFishReq),
        typeof(fishMsg.SomeZuanTouBombRsp),
        typeof(fishMsg.CreateFireStormRsp),
        typeof(fishMsg.DestoryFireStormRsp),
        typeof(fishMsg.FireStormStatusShootRsp),
        typeof(fishMsg.FireStormScoreRsp),
        typeof(fishMsg.CreateMadCowRsp),
        typeof(fishMsg.DestoryMadCowRsp),
        typeof(fishMsg.MadCowStatusRsp),
        typeof(fishMsg.MadCowScoreRsp),
        typeof(fishMsg.CreateDelayBombRsp),
        typeof(fishMsg.DelayBomb_Bomb_Rsp),
        typeof(fishMsg.CreateSerialBombCrabRsp),
        typeof(fishMsg.SerialBombCrabBombRsp),
        typeof(fishMsg.DestorySerialBombCrabRsp),
        typeof(fishMsg.HaiWangCrabHitPartReq),
        typeof(fishMsg.CrabPart),
        typeof(fishMsg.HaiWangCrabKilledPartRsp),
        typeof(fishMsg.HaiWangCrabKilledDeadRsp),
        typeof(fishMsg.CreateThunderHammerRsp),
        typeof(fishMsg.ThunderHammer_Bomb_Rsp),
        typeof(fishMsg.CreateGhostShipRsp),
        typeof(fishMsg.DestoryGhostShipRsp),
        typeof(fishMsg.GhostShipStatusRsp),
        typeof(fishMsg.GhostShipScoreRsp),
        typeof(fishMsg.CreateAnglerFishRsp),
        typeof(fishMsg.AnglerFishBombRsp),
        typeof(fishMsg.DestoryAnglerFishRsp),
    };

    [MenuItem("Tools/重建protobuf-model.dll")]
    private static void RebuildProtobufModelForProject()
    {
        RuntimeTypeModel typeModel = GetModel(out string typeNames);
        if (typeModel == null)
        {
            return;
        }
        typeModel.Compile("ProjectModel", "protobuf-model.dll");
        if (!Directory.Exists(protobufNetDirRoot + "/Plugins"))
        {
            Directory.CreateDirectory(protobufNetDirRoot + "/Plugins");
        }
        File.Copy("protobuf-model.dll", protobufNetDirRoot + "/Plugins/protobuf-model.dll", true);
        File.Delete("protobuf-model.dll");
        UnityEngine.Debug.Log("为以下类型重建protobuf-model.dll\r\n" + typeNames);
        AssetDatabase.Refresh();
    }

    private static RuntimeTypeModel GetModel(out string typeNames)
    {
        List<Type> types = GetAllRelatedTypeList();
        RuntimeTypeModel typeModel = RuntimeTypeModel.Create();
        StringBuilder stringBuilder = new StringBuilder();
        List<Type> list = new List<Type>();
        foreach (var t in types)
        {
            var contract = t.GetCustomAttributes(typeof(ProtoContractAttribute), false);
            if (contract.Length > 0 && !list.Contains(t))
            {
                typeModel.Add(t, true);
                stringBuilder.Append(t.ToString());
                stringBuilder.Append("\r\n");
                list.Add(t);
            }
        }
        typeNames = stringBuilder.ToString();
        return typeModel;
    }

    private static List<Type> GetAllRelatedTypeList()
    {
        List<Type> list = new List<Type>();
        List<string> assemblyNames = new List<string>();
        for (int i = 0; i < exampleTypes.Count; i++)
        {
            var assembly = Assembly.GetAssembly(exampleTypes[i]);
            if (assemblyNames.Contains(assembly.FullName))
            {
                continue;
            }
            assemblyNames.Add(assembly.FullName);
            list.AddRange(assembly.GetTypes());
        }
        return list;
    }
}
