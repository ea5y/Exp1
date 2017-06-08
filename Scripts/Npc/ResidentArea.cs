/// <summary>
/// ResidentArea, 
/// 
/// 2016/09/07
/// </summary>

using System;
using UnityEngine;
using System.Collections;

using Scm.Common.Master;
using Scm.Common.GameParameter;
using System.Collections.Generic;

public class ResidentArea : Gadget
{
    public static Dictionary<int, ResidentArea> mCachDic = new Dictionary<int, ResidentArea>();
    //服务器数据是否显示
    public static Dictionary<int, bool> mCachActive = new Dictionary<int, bool>();

    //if the resident can show at current round, set the resident active
    public static void OnActiveRefresh()
    {
        //old
        return;
        foreach (var item in mCachDic)
        {
            item.Value.ActiveRefresh();
        }
    }

    public static void OnActiveRefresh(bool pActive, int pField)
    {
        NetworkController.InvokeAsync(() =>
        {
            mCachActive[pField] = pActive;
            foreach (var item in mCachDic)
            {
                item.Value.ActiveRefresh(pActive, pField);
            }
        });
    }

    //onReact to the resident`s hold percent
    public static void OnHoldRefresh(Scm.Common.Packet.ResidentAreaSideGaugeEvent packet)
    {
        if (mCachDic.ContainsKey(packet.InFieldId))
        {
            mCachDic[packet.InFieldId].HoldRefresh(packet);
        }
    }

    public static void CacheResidentArea(ResidentArea pResidentArea)
    {
        if (!mCachDic.ContainsKey(pResidentArea.InFieldId))
        {
            mCachDic.Add(pResidentArea.InFieldId, pResidentArea);
            foreach (var v in mCachActive)
            {
                OnActiveRefresh(v.Value, v.Key);
            }
        }
    }

    protected override void Setup(Manager manager, ObjectMasterData objectData, EntrantInfo info, AssetReference assetReference)
    {
        base.Setup(manager, objectData, info, assetReference);
        gameObject.transform.localScale = new Vector3(objectData.CollisionRatio, 1, objectData.CollisionRatio);
    }

    protected override void Destroy()
    {
        ObjectUIRoot.RemoveResidentProcess();
        if (mCachDic.ContainsKey(InFieldId))
        {
            mCachDic.Remove(InFieldId);
        }
        base.Destroy();
    }

    private void Show(bool pActive)
    {
        if (!pActive)
        {
            if (!gameObject.activeSelf)
            {
                Debug.Log("Not Want happen");
                mCachDic.Remove(FieldId);
                return;
            }
            if (null == GUIBattleDirectionTip.Instance)
            {
                return;
            }
            GUIBattleDirectionTip.Instance.Remove(transform);
            ObjectUIRoot.RemoveResidentProcess();
        }
        gameObject.SetActive(pActive);
    }

    private void ActiveRefresh()
    {
        bool pCur = gameObject.activeSelf;
        if (GUITacticalGauge.RoundIndex == this.ObjectData.ActiveRound)
        {
            if (pCur)
            {
                return;
            }
            else
            {
                //keep order
                gameObject.SetActive(true);
                Gadget.DelateSetUp[InFieldId]();

                GUIBattleDirectionTip.Instance.AddTarget(transform);
            }
        }
        else
        {
            if (pCur)
            {
                //                gameObject.SetActive(false);
                //                ObjectUIRoot.RemoveResidentProcess();
                //Remove or not
                //                Destroy();
            }
        }
    }

    private void ActiveRefresh(bool pActive, int pField)
    {
        if (pField != InFieldId)
        {
            return;
        }
        if (!pActive)
        {
            gameObject.SetActive(false);
            return;
        }
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            Gadget.DelateSetUp[InFieldId]();
            GUIBattleDirectionTip.Instance.AddTarget(transform);
        }
    }

    private void HoldRefresh(Scm.Common.Packet.ResidentAreaSideGaugeEvent packet)
    {
        Debug.LogWarning("===> " + packet.InFieldId + " " + packet.RedRemain + " " + packet.BlueRemain);
        Debug.LogError("===> " + packet.InFieldId + " " + packet.RedStandBy + " " + packet.BlueStandBy);

        if (GUITacticalGauge.RoundIndex != 3)
        {
            if (packet.RedRemain > 0)
            {
                if (packet.RedRemain == packet.RedTotal)
                {

                    Show(false);
                    mCachDic.Remove(packet.InFieldId);
                    return;
                }
                ObjectUIRoot.UpdateResidentProcess(packet, false);
                return;
            }

            if (packet.BlueRemain > 0)
            {
                if (packet.BlueRemain == packet.BlueTotal)
                {

                    Show(false);
                    mCachDic.Remove(packet.InFieldId);
                    return;
                }
                ObjectUIRoot.UpdateResidentProcess(packet, true);
            }
        }
        else
        {
            if (packet.RedStandBy > 0)
            {
                if (packet.RedRemain == packet.RedTotal)
                {

                    Show(false);
                    mCachDic.Remove(packet.InFieldId);
                    return;
                }
                ObjectUIRoot.UpdateResidentProcess(packet, false);
                return;
            }

            if (packet.BlueStandBy > 0)
            {
                if (packet.BlueRemain == packet.BlueTotal)
                {

                    Show(false);
                    mCachDic.Remove(packet.InFieldId);
                    return;
                }
                ObjectUIRoot.UpdateResidentProcess(packet, true);
            }
        }
    }
}
