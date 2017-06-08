using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Packet;

public class GUITeamMatch : MonoBehaviour
{
    public GameObject GUIInvite;
    public GameObject GUIMatchInfo;
    public UISlider InviteSlider;
    public List<TeamMatchSlot> Slots;

    public static GUITeamMatch Instance;

    // キャラアイコン
    CharaIcon CharaIcon { get { return ScmParam.Battle.CharaIcon; } }

    [SerializeField]
    private InviteAttach inviteAttach;

    [System.Serializable]
    public class InviteAttach {
        public UILabel inviteText;
        public UISprite inviterHead;
    }

    // Use this for initialization
    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
        Slots.ForEachI((v, i) =>
        {
            if (i % 2 == 0)
            {
                v.mRemove.transform.localPosition = new Vector3(0, -56.1f, 0);
                v.mRemove.transform.localScale = new Vector3(1, 1, 1);
                v.mRemoveLabel.transform.localScale = new Vector3(1, 1, 1);
                v.transform.localPosition = new Vector3(v.transform.localPosition.x, 0f, v.transform.localPosition.z);
            }
            else
            {
                v.mRemove.transform.localPosition = new Vector3(0, 56.9f, 0);
                v.mRemove.transform.localScale = new Vector3(1, -1, 1);
                v.mRemoveLabel.transform.localScale = new Vector3(1, -1, 1);
                v.transform.localPosition = new Vector3(v.transform.localPosition.x, -41f, v.transform.localPosition.z);
            }
        });
    }

    public void Show(bool pActive)
    {
        gameObject.SetActive(pActive);
    }

    public void ShowInvite(bool pActive, TeamInviteEvent inviteEvent)
    {
        Show(pActive);
        if (pActive)
        {
            SetupTeamInviteInfo(inviteEvent);
            if (GUIInvite.activeSelf)
            {
                return;
            }
            GUIMatchInfo.SetActive(false);
            StartCoroutine(StartCountDown());
        }
        GUIInvite.SetActive(pActive);
    }

    private void SetupTeamInviteInfo(TeamInviteEvent inviteEvent) {
        inviteAttach.inviteText.text = MasterData.GetText(TextType.TX617_TeamInvite, inviteEvent.PlayerName);
        CharaIcon.GetIcon((AvatarType)inviteEvent.CharacterId, inviteEvent.SkinId, false, (UIAtlas atlas, string sprite) => {
            inviteAttach.inviterHead.atlas = atlas;
            inviteAttach.inviterHead.spriteName = sprite;
        });
    }

    public void ShowTeamMatchInfo(bool pActive)
    {
        Show(pActive);
        if (pActive)
        {
            TeamMatchSlot.SetDetail(TeamMatchSlot.MemberParameters);
            GUIInvite.SetActive(false);
        }
        GUIMatchInfo.SetActive(pActive);
    }

    IEnumerator StartCountDown()
    {
        float cd = 19f;
        float t = Time.time + cd;
        while (t > Time.time)
        {
            InviteSlider.value = (t - Time.time) / cd;
            yield return new WaitForSeconds(0.05f);
        }

        ShowInvite(false, null);
        yield return 0;
    }

    #region Button Event
    public void OnInviteAccept()
    {
        CommonPacket.SendInviteAccOrRej(TeamInviteAckReq.OperationType.Accept);
        StopAllCoroutines();
        TeamMatchSlot.MemberParameters.Clear();
        ShowTeamMatchInfo(true);
    }

    public void OnInviteReject()
    {
        CommonPacket.SendInviteAccOrRej(TeamInviteAckReq.OperationType.Reject);
        StopAllCoroutines();
        ShowInvite(false, null);
    }
    #endregion

    public static void OnRefrshTeam(List<GroupMemberParameter> pMemberParameters)
    {
        TeamMatchSlot.MemberParameters = pMemberParameters;
        GUITeamMatch.Instance.ShowTeamMatchInfo(true);
    }

    public static void LeaveTeam()
    {
        TeamMatchSlot.MemberParameters.Clear();
        Instance.Slots.ForEachI((v, i) =>
        {
            v.SetSlotDetail(null);
        });
    }
}
