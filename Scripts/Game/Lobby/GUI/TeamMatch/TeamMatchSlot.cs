using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Scm.Common.Packet;

public class TeamMatchSlot : MonoBehaviour
{
    public UISprite mBg;
    public GameObject mHead;
    public UISprite mIcon;
    public UILabel Label;
    public GameObject mAdd;
    public GameObject mJiesan;
    public GameObject mRemove;
    public UILabel mRemoveLabel;

    private static List<TeamMatchSlot> SlotList
    {
        get { return GUITeamMatch.Instance.Slots; }
    }
    private CharaIcon CharaIcon { get { return ScmParam.Battle.CharaIcon; } }
    public static List<GroupMemberParameter> MemberParameters = new List<GroupMemberParameter>();
    private GroupMemberParameter MemberParameter = null;
    private static bool Leader = false;
    public static void SetDetail(List<GroupMemberParameter> pMemberParameters)
    {
        Leader = false;
        MemberParameters = pMemberParameters;
        SlotList.ForEachI((v, i) =>
        {
            v.SetSlotDetail(null);
        });

        if (null == MemberParameters || MemberParameters.Count == 0)
        {
            return;
        }

        if (MemberParameters[0].PlayerId == NetworkController.ServerValue.PlayerId)
        {
            Leader = true;
            MemberParameters.ForEachI((v, i) =>
            {
                if (0 == i)
                {

                }
                SlotList[i].SetSlotDetail(v);
            });
        }
        else
        {
            MemberParameters.ForEachI((v, i) =>
            {
                if (0 == i)
                {

                }
                SlotList[i].SetSlotDetail(v);
            });
        }
    }

    public void SetSlotDetail(GroupMemberParameter pMemberParameter)
    {
        MemberParameter = pMemberParameter;
        if (null == pMemberParameter)
        {
            mHead.SetActive(false);
            if (null != mAdd)
            {
                mAdd.SetActive(false);
            }
            if (null != mJiesan)
            {
                mJiesan.SetActive(false);
            }
            mRemove.SetActive(false);
            mBg.enabled = true;
        }
        else
        {
            if (Leader)
            {
                if (null != mJiesan)
                {
                    mJiesan.SetActive(false);
                }
                if (null != mAdd)
                {
                    mAdd.SetActive(false);
                }
                if (pMemberParameter.PlayerId == NetworkController.ServerValue.PlayerId)
                {
                    if (null != mJiesan)
                    {
                        mJiesan.SetActive(true);
                    }
                    mRemoveLabel.text = "退出";
                }
                else
                {
                    mRemoveLabel.text = "移除";
                }
                mRemove.SetActive(true);
            }
            else
            {
                if (null != mAdd)
                {
                    mAdd.SetActive(false);
                }
                mRemove.SetActive(false);
                if (pMemberParameter.PlayerId == NetworkController.ServerValue.PlayerId)
                {
                    mRemove.SetActive(true);
                    mRemoveLabel.text = "退出";
                }
            }
            mHead.SetActive(true);
            mBg.enabled = false;
            ShowJoinerInfo((AvatarType)pMemberParameter.CharacterId, pMemberParameter.SkinId);
        }
    }

    public void ShowJoinerInfo(AvatarType pAvatarType, int pSkinId)
    {
        CharaIcon.GetIcon(pAvatarType, pSkinId, false, (a, s) =>
        {
            mIcon.atlas = a;
            mIcon.spriteName = s;
        });
    }

    #region Button Event
    public void OnZuduiSlot()
    {
        //        Debug.Log(transform.name);
        if (Leader || MemberParameters.Count == 0)
        {
            //if (!XUI.Friends.FriendsController.Instance.gameObject.activeSelf)
            //    XUI.Friends.FriendsController.Instance.Show();
            XUI.GUIFriends.Instance.Open();
        }
    }

    public void OnRemove()
    {
        if (null == MemberParameter)
        {
            return;
        }
        CommonPacket.SendTeamOperation(TeamOperationReq.OperationType.RemoveMember, MemberParameter.PlayerId);
    }

    public void OnJieSan()
    {
        CommonPacket.SendTeamOperation(TeamOperationReq.OperationType.Dismiss);
    }
    #endregion
}
