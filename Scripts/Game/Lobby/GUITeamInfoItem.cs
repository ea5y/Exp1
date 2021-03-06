/// <summary>
/// チーム情報アイテム
/// 
/// 2014/12/12
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.Packet;
using Scm.Common.GameParameter;

public class GUITeamInfoItem : MonoBehaviour
{
	#region フィールド＆プロパティ
	private TeamParameter teamParam;
	public long TeamId { get { return teamParam != null ? teamParam.TeamId : 0; } }
	public string TeamName { get { return teamParam != null ? teamParam.TeamName : string.Empty; } }
	public bool IsPassword { get { return teamParam != null ? teamParam.IsPassword : false; } }
	public bool IsJoin { get { return !this.attach.group_InBattle.gameObject.activeSelf; } }

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	private Attach _attach;
	private Attach attach { get { return _attach; } }
	[System.Serializable]
	public class Attach
	{
		public UILabel nameLabel;
		public UISprite icon_Password;
		public UILabel enrollment;
		public UILabel commentLabel;

		public UIWidget icon_LeaderParent;
		public UISprite icon_Leader;
		private UISprite icon_Loading;
		public UIWidget selectFrame;
		public Transform group_InBattle;
		public UILabel label_InBattle;
	}
	#endregion

	public void SetTeamParam(TeamParameter teamParam)
	{
		this.teamParam = teamParam;
		this.DisplayTeamParam();
	}
	public void DisplayTeamParam()
	{
		this.attach.selectFrame.alpha = 0;
		this.attach.icon_Leader.enabled = false;

		if(this.teamParam != null)
		{
			this.attach.nameLabel.text = this.teamParam.TeamName;
			this.attach.icon_Password.enabled = this.teamParam.IsPassword;
			this.attach.enrollment.text = string.Format("{0}/{1}", this.teamParam.NumOfMember, GameConstant.TeamMemberMax);
			this.attach.commentLabel.text = ObsolateSrc.TeamComment.GetTeamComment(this.teamParam.CommentId);

			// 戦闘中,マッチング中.
			switch(this.teamParam.LeaderMatchingStatus)
			{
			case MatchingStatus.Normal:
				this.attach.group_InBattle.gameObject.SetActive(false);
				break;
			case MatchingStatus.InBattle:
				this.attach.group_InBattle.gameObject.SetActive(true);
				this.attach.label_InBattle.text = MasterData.GetText(TextType.TX104_GUITeamInfoItem_InBattle);
				break;
			default:
				this.attach.group_InBattle.gameObject.SetActive(true);
				this.attach.label_InBattle.text = MasterData.GetText(TextType.TX105_GUITeamInfoItem_InMatching);
				break;
			}

			CharaIcon charaIcon = ScmParam.Lobby.CharaIcon;
			if (charaIcon != null) charaIcon.GetIcon((AvatarType)this.teamParam.LeaderAvatar, this.teamParam.LeaderSkinId, false, this.SetLeaderIcon);
		}
	}
	private void SetLeaderIcon(UIAtlas atlas, string spriteName)
	{
		var sprite = this.attach.icon_Leader;
		if (sprite == null)
			return;

		// スプライト設定.
		sprite.atlas = atlas;
		sprite.spriteName = spriteName;
		sprite.enabled = true;

		// スプライトが読み込めたかどうかチェック.
		if (sprite.GetAtlasSprite() == null)
		{
			// アトラスとスプライト名が両方共設定されていてスプライトがない場合はエラー扱いｌ
			if (atlas != null && !string.IsNullOrEmpty(spriteName))
			{
				BugReportController.SaveLogFile(string.Format("SetLeaderIcon:\r\nSprite Not Found!! AvatarType = {0} SpriteName = {1}", this.teamParam.LeaderAvatar, spriteName));
			}
		}
	}
	public void SetSelectFrameActive(bool active)
	{
		TweenAlpha.Begin(this.attach.selectFrame.gameObject, 0.15f, active ? 1f : 0f);
	}
	#region NGUIリフレクション
	public void OnClick()
	{
#if OLD_TEAM_LOGIC
        GUITeamMenu.OnSearchSelect(this);
#endif
	}
	#endregion
}
