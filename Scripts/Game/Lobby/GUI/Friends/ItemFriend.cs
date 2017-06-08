using UnityEngine;
using System.Collections;
using Scm.Common.Packet;
using Scm.Common.Master;

namespace XUI
{
    public class ItemFriend : CustomControl.ScrollViewItem
    {
        public UISprite icon;
        public UILabel username;
        public UILabel lv;
        public UILabel winTimes;
        public UILabel rank;
        public UILabel InviteLabel;
        public UIAtlas rankAtlas;
        public UISprite rankIcon;

        public UIButton btnLeft;
        public UIButton btnRight;
        public UIButton btnDelete;
        private FriendParameter data;

        public void CountDown()
        {
            StartCoroutine(StartCountDown());
        }

        private bool IsCountDown = false;
        IEnumerator StartCountDown()
        {
            if (IsCountDown)
            {
                yield break;
            }
            IsCountDown = true;
            InviteLabel.text = "已邀请";
            float cd = 19f;
            float t = Time.time + cd;
            while (t > Time.time)
            {
                yield return new WaitForSeconds(0.05f);
            }
            InviteLabel.text = "邀请";
            IsCountDown = false;
            yield return 0;
        }

        override public void FillItem(IList datas, int index)
        {
            base.FillItem(datas, index);
            this.data = (FriendParameter)datas[index];

            this.SetIcon();
            this.SetName();
            this.SetStatus();
            this.SetWinTimes();
            this.SetRank();
            
            this.InviteLabel.text = "邀请";

            var friendId = this.data.FriendId;
            var friendName = this.data.Name;

            this.btnLeft.onClick.Clear();
            this.btnRight.onClick.Clear();
            EventDelegate.Add(this.btnLeft.onClick, () => {
                    this.OnBtnTeamClick(friendId);
                    this.CountDown();
                    });
            EventDelegate.Add(this.btnRight.onClick, () => {
                    this.OnBtnChatClick(friendId, friendName);
                    });
            EventDelegate.Add(this.btnDelete.onClick, () => {
                    this.OnBtnDeleteClick(friendId, friendName);
                    });
        }

        private void SetIcon()
        {
            ScmParam.Lobby.CharaIcon.GetIcon((AvatarType)this.data.CharacterId, this.data.SkinId, false, 
                    (a, s) =>{
                     this.icon.atlas = a;
                     this.icon.spriteName = s;
                    });
        }

        private void SetName()
        {
            this.username.text = this.data.Name;
        }

        private void SetStatus()
        {
            if (this.data.LastLogoutSeconds > 0)
                this.lv.text = "离线";
            else
                this.lv.text = "在线";

            this.SetButton(this.btnLeft, !(this.data.LastLogoutSeconds > 0));
            this.SetButton(this.btnRight, !(this.data.LastLogoutSeconds > 0));
        }

        private void SetButton(UIButton btn, bool isEnable)
        {
            btn.enabled = isEnable;
            if(isEnable)
                btn.SetState(UIButton.State.Normal, true);
            else
                btn.SetState(UIButtonColor.State.Disabled, true);
        }

        private void SetWinTimes()
        {
            this.winTimes.text = this.data.WinCount + "";
        }

        private void SetRank()
        {
            this.rank.text = PlayerRankMaster.Instance.GetRankByScore(this.data.Score) + "";
            this.rankIcon.atlas = this.rankAtlas;
            this.rankIcon.spriteName = this.rank.text;
        }

        private void SetEvent()
        {

        }

        private void OnBtnTeamClick(long friendId)
        {
            CommonPacket.SendTeamInvite(new[] { friendId });
        }

        private void OnBtnChatClick(long friendId, string name)
        {
            GUIChatFrameController.Instance.Show();
            GUIChatFrameController.Instance.OnChatTypeWis(friendId, name);
        }

        private void OnBtnDeleteClick(long friendId, string name) {
            GUISystemMessage.SetModeYesNo(MasterData.GetText(TextType.TX619_DeleteFriendWarningTitle),
                MasterData.GetText(TextType.TX618_DeleteFriendWarning, name),
                () => {
                    GUISystemMessage.Close();
                    CommonPacket.SendDeleteFriend(friendId);
                },
                () => {
                    GUISystemMessage.Close();
                });
                
        }
    }
}

