/// <summary>
/// チームメニュー
/// 
/// 2014/12/10
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Packet;
using Scm.Common.GameParameter;

public class GUITeamMenu : Singleton<GUITeamMenu>
{
#if OLD_TEAM_LOGIC
#region 定義
    public enum TeamMenuMode
	{
		None,

		SingleMenu   = 381,
		Create       = 382,
		Join         = 383,

		LeaderMenu   = 384,
		Invitation   = 385,
		LeaderChange = 386,
		//CreatePullDown = 387,
	}
	// パスワード入力ウィンドウ処理用クラス.
	class TeamJoinPasswordInput
	{
		long teamId;
		string password;
		
		public TeamJoinPasswordInput(long teamId)
		{
			this.teamId = teamId;
		}
		public void OpenInputWindow()
		{
			GUIMessageWindow.SetModeInput(MasterData.GetText(TextType.TX089_TeamJoin_PasswordInputTitle), string.Empty, string.Empty, TrySendTeamJoin ,null, SetPassword, null);
		}
		private void SetPassword(string password)
		{
			this.password = password;
		}
		private void TrySendTeamJoin()
		{
			string errorMessage;
			if(CommonPacket.TrySendTeamJoin(this.teamId, this.password, out errorMessage))
			{
				GUITeamMenu.SetMode(TeamMenuMode.None);
			}
			else
			{
				GUIMessageWindow.SetModeOK(errorMessage, OpenInputWindow);
			}
		}
	}

	const float SendTeamSearchTime = 5f;
	#endregion

	#region フィールド＆プロパティ

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UIPlayTween rootTween;

		public UIPlayTween singleMenuTween;
		public UIPlayTween makeTween;
		public UIPlayTween joinTween;

		public UIPlayTween leaderMenuTween;
		public UIPlayTween invitationTween;
		public UIPlayTween leaderChangeTween;

		public XUIButton teamCreateButton;
		public XUIButton teamSearchButton;

		public AttachMake make;
		public AttachJoin join;
	}
	[System.Serializable]
	public class AttachMake
	{
		public UIInput teamName;
		public UIInput password;
		public byte commentId;
		public UILabel comment;

		public UIPlayTween pullDown;
		public UITable uiTable;
		public GUITeamCommentItem baseItem;
		private List<GUITeamCommentItem> itemList;

		public void CreateCommentItem()
		{
			if(itemList == null)
			{
				itemList = new List<GUITeamCommentItem>();
				for(byte i = 0; i < ObsolateSrc.TeamComment.TeamCommentLength; ++i)
				{
					// Item生成.
					var item = SafeObject.Instantiate(this.baseItem) as GUITeamCommentItem;
					item.name = string.Format("Item{0:00}", i);
					item.transform.parent = uiTable.transform;
					item.transform.localPosition = this.baseItem.transform.localPosition;
					item.transform.localRotation = this.baseItem.transform.localRotation;
					item.transform.localScale = this.baseItem.transform.localScale;
					item.SetComment(i);
					item.gameObject.SetActive(true);
					itemList.Add(item);
				}
				uiTable.Reposition();
			}
		}
	}
	[System.Serializable]
	public class AttachJoin
	{
		public int pageItems = 15;
		private int startIndex;
		public UIScrollView uiScrollView;
		public UITable uiTable;
		public GUITeamInfoItem baseItem;

		private List<TeamParameter> teamParamList;
		private List<GUITeamInfoItem> itemList = new List<GUITeamInfoItem>();
		public GUITeamInfoItem SelectedItem { get; private set; }

		public Transform group_memberList;
		public UILabel select_teamName;
		public GUITeamMemberItem[] select_memberItems;

		public UILabel pageLabel;
		public Transform group_Next;
		public Transform group_Back;
		public UIButton okButton;

		private bool CanBackTeamList { get { return 0 < this.startIndex; } }
		private bool CanNextTeamList { get { return this.startIndex + this.pageItems < this.teamParamList.Count; } }

		/// <summary>
		/// 初期化.
		/// </summary>
		public void Init()
		{
			this.SetSelectedItem(null);
		}

		/// <summary>
		/// 通信で受け取ったTeamParameterをセットする.
		/// </summary>
		public void SetTeamParam(List<TeamParameter> teamParams)
		{
			this.teamParamList = teamParams;
			this.SetSelectedItem(null);

			this.startIndex = 0;
			this.CreateTeamListItem();
		}
		/// <summary>
		/// TeamParameterを元に規定数のチームボタンリストを作る.
		/// </summary>
		private void CreateTeamListItem()
		{
			// itemのActiveをfalseに.
			for(int i = 0; i < itemList.Count; ++i)
			{
				if(itemList[i] != null)
				{
					itemList[i].gameObject.SetActive(false);
				}
				else
				{
					// 外的要因で消滅した場合の保険.
					itemList.RemoveAt(i);
					--i;
				}
			}

			// TeamParameterをセット.
			if(this.teamParamList != null)
			{
				int max = Mathf.Min(this.pageItems, this.teamParamList.Count - this.startIndex);
				for(int i = 0; i < max; ++i)
				{
					GUITeamInfoItem item = i < itemList.Count ? itemList[i] : null;
					if(item == null)
					{
						// Item生成.
						item = SafeObject.Instantiate(this.baseItem) as GUITeamInfoItem;
						item.name = string.Format("Item{0:00}", itemList.Count);
						item.transform.parent = uiTable.transform;
						item.transform.localPosition = this.baseItem.transform.localPosition;
						item.transform.localRotation = this.baseItem.transform.localRotation;
						item.transform.localScale = this.baseItem.transform.localScale;
						itemList.Add(item);
					}
					item.gameObject.SetActive(true);
					item.SetTeamParam(this.teamParamList[this.startIndex + i]);
				}
			}
			// UITable,UIScrollViewをリセット.
			uiTable.Reposition();
			uiScrollView.ResetPosition();
			this.UpdatePageItem();
		}
		/// <summary>
		/// チームボタンリストを前ページに戻す.
		/// </summary>
		public void ShowBackTeamList()
		{
			if(this.CanBackTeamList)
			{
				this.startIndex = Mathf.Max(this.startIndex - this.pageItems, 0);
				this.CreateTeamListItem();
			}
		}
		/// <summary>
		/// チームボタンリストを次ページに送る.
		/// </summary>
		public void ShowNextTeamList()
		{
			if(this.CanNextTeamList)
			{
				this.startIndex += this.pageItems;
				this.CreateTeamListItem();
			}
		}
		private void UpdatePageItem()
		{
			int nowPage, maxPage;
			if(this.teamParamList != null)
			{
				this.group_Back.gameObject.SetActive(this.CanBackTeamList);
				this.group_Next.gameObject.SetActive(this.CanNextTeamList);

				// pageItems=15,startIndex=2,teamParamList.Count=10の場合など,
				// 本来は1ページに収まる量でも<-が押せることを示すために2/2と表示する.
				nowPage = (this.startIndex + this.pageItems - 1) / this.pageItems + 1;
				maxPage = Mathf.Max((this.teamParamList.Count - 1) / this.pageItems + 1, nowPage);
			}
			else
			{
				this.group_Back.gameObject.SetActive(false);
				this.group_Next.gameObject.SetActive(false);
				nowPage = maxPage = 1;
			}
			this.pageLabel.text = string.Format("{0}/{1}", nowPage, maxPage);
		}
		/// <summary>
		/// 選択したチームの情報を表示する.
		/// </summary>
		public void SetMemberParam(long teamId, List<GroupMemberParameter> members)
		{
			if(this.SelectedItem != null && this.SelectedItem.TeamId == teamId)
			{
				this.ResetMemberParam();
				this.select_teamName.text = this.SelectedItem.TeamName;
				for(int i = 0; i < members.Count; ++i)
				{
					this.select_memberItems[i].SetMemberParam(members[i], false);
				}
				this.group_memberList.gameObject.SetActive(true);
			}
		}
		/// <summary>
		/// 選択したチームの情報領域を非表示にする.
		/// </summary>
		public void ResetMemberParam()
		{
			this.group_memberList.gameObject.SetActive(false);
			this.select_teamName.text = string.Empty;
			foreach(var item in this.select_memberItems)
			{
				item.SetMemberParam(null, false);
			}
		}
		/// <summary>
		/// 選択したチームを記録し,選択枠を付ける.
		/// </summary>
		public void SetSelectedItem(GUITeamInfoItem item)
		{
			if(this.SelectedItem != null)
			{
				this.SelectedItem.SetSelectFrameActive(false);
			}
			this.SelectedItem = item;
			// 戦闘中,マッチング中のチームは選択不可.
			if(this.SelectedItem != null)
			{
				this.okButton.isEnabled = item.IsJoin;
				this.SelectedItem.SetSelectFrameActive(true);
			}
			else
			{
				this.okButton.isEnabled = false;
			}
		}
	}

	// 現在のモード
	TeamMenuMode Mode { get; set; }
	bool createPullDownForward = false;
	List<TeamParameter> joinTeamParamList;
	Dictionary<long, CacheData<List<GroupMemberParameter>>> teamMemberDic = new Dictionary<long, CacheData<List<GroupMemberParameter>>>();
	float sendTeamSearchNextTime;

	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();
		this.Init();
	}
	private void Init()
	{
		// 表示設定
		this._SetMode(TeamMenuMode.None);
		// 初期状態にする
		this._PlayTween(TeamMenuMode.None, true);
		this._PlayTween(TeamMenuMode.SingleMenu, false);
		this._PlayTween(TeamMenuMode.Create, false);
		this._PlayTween(TeamMenuMode.Join, false);
		this._PlayTween(TeamMenuMode.LeaderMenu, false);
		this._PlayTween(TeamMenuMode.Invitation, false);
		this._PlayTween(TeamMenuMode.LeaderChange, false);
		this.OnCreatePullDown(false);
	}
	private void ResetCreateField()
	{
		if(this.Attach.make.teamName != null) { this.Attach.make.teamName.value = string.Empty; }
		if(this.Attach.make.password != null) { this.Attach.make.password.value = string.Empty; }
		this.Attach.make.commentId = 0;
		if(this.Attach.make.comment != null) { this.Attach.make.comment.text = string.Empty; }
		this.Attach.make.CreateCommentItem();
		this._SetComment(0);
	}
	#endregion

	#region モード切替
	static public void SetMode(TeamMenuMode mode)
	{
		if(GUITeamMenu.Instance != null)
		{
			GUITeamMenu.Instance._SetMode(mode);
		}
	}
	private void _SetMode(TeamMenuMode mode)
	{
		if(this.Mode != mode)
		{
			this._PlayTween(this.Mode, false);
			this.Mode = mode;
			this._PlayTween(this.Mode, true);
		}
		if(this.Mode == TeamMenuMode.None)
		{
			//GUILobbyMenu.Toggle();
			GUILobbyResident.SetActive(true);
			GUIScreenTitle.Play(false);
			GUIHelpMessage.Play(false);
		}
		else
		{
			GUILobbyMenu.SetMode(GUILobbyMenu.MenuMode.None);
			GUILobbyResident.SetActive(false);
			GUIScreenTitle.Play(true, MasterData.GetText(TextType.TX078_TeamMenu_ScreenTitle));
			GUIHelpMessage.Play(true, MasterData.GetText(TextType.TX079_TeamMenu_HelpMessage));
		}
	}
	private void _PlayTween(TeamMenuMode mode, bool isForward)
	{
		UIPlayTween playTween;
		switch(mode)
		{
		case TeamMenuMode.None:
			playTween = this.Attach.rootTween;
			break;
		case TeamMenuMode.SingleMenu:
			playTween = this.Attach.singleMenuTween;
			break;
		case TeamMenuMode.Create:
			playTween = this.Attach.makeTween;
			this.OnCreatePullDown(false);
			break;
		case TeamMenuMode.Join:
			this.Attach.join.Init();
			playTween = this.Attach.joinTween;
			break;
		case TeamMenuMode.LeaderMenu:
			playTween = this.Attach.leaderMenuTween;
			break;
		case TeamMenuMode.Invitation:
			playTween = this.Attach.invitationTween;
			break;
		case TeamMenuMode.LeaderChange:
			playTween = this.Attach.leaderChangeTween;
			break;
		default:
			BugReportController.SaveLogFile("unknown mode = " + mode);
			this._SetMode(TeamMenuMode.None);
			return;
		}

		if(playTween != null)
		{
			playTween.Play(isForward);
		}
	}
	#endregion

	#region 情報セット
	static public void SetSearchTeam(IEnumerable<TeamParameter> teamParams)
	{
		if(GUITeamMenu.Instance != null) { GUITeamMenu.Instance._SetSearchTeam(teamParams); }
	}
	private void _SetSearchTeam(IEnumerable<TeamParameter> teamParams)
	{
		this.joinTeamParamList = new List<TeamParameter>(teamParams);
		this._attach.join.SetTeamParam(this.joinTeamParamList);
	}
	static public void SetSearchTeamMember(long teamId, List<GroupMemberParameter> members)
	{
		if(GUITeamMenu.Instance != null) { GUITeamMenu.Instance._SetSearchTeamMember(teamId, members); }
	}
	private void _SetSearchTeamMember(long teamId, List<GroupMemberParameter> members)
	{
		this.teamMemberDic[teamId] = new CacheData<List<GroupMemberParameter>>(Time.time, members);
		this._attach.join.SetMemberParam(teamId, members);
	}
	static public void UpdateSingleButtonEnable()
	{
		if(GUITeamMenu.Instance != null) { GUITeamMenu.Instance._UpdateSingleButtonEnable(); }
	}
	private void _UpdateSingleButtonEnable()
	{
		this._attach.teamCreateButton.isEnabled = !GUIMatchingState.IsMatching;
		this._attach.teamSearchButton.isEnabled = !GUIMatchingState.IsMatching;
	}
	#endregion

	#region 情報処理
	static public void OnResponseTeamCreate(TeamEditResult result)
	{
		switch(result)
		{
		case TeamEditResult.Success:
			break;
		case TeamEditResult.AlreadyEnteredTeam:
			GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX083_TeamCreateRes_AlreadyEnter), null);
			break;
		case TeamEditResult.NameDuplicate:
			GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX084_TeamCreateRes_NameDuplicate), null);
			GUITeamMenu.SetMode(GUITeamMenu.TeamMenuMode.Create);
			break;
		case TeamEditResult.NGName:
			GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX085_TeamCreateRes_NGName), null);
			GUITeamMenu.SetMode(GUITeamMenu.TeamMenuMode.Create);
			break;
		case TeamEditResult.OverLengthName:
			GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX086_TeamCreateRes_OverLengthName), null);
			GUITeamMenu.SetMode(GUITeamMenu.TeamMenuMode.Create);
			break;
		case TeamEditResult.SenderInMatching:
			GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX087_TeamCreateRes_SenderInMatching), null);
			break;
		case TeamEditResult.UnjustPassword:
			GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX082_TeamCreate_PasswordError), null);
			GUITeamMenu.SetMode(GUITeamMenu.TeamMenuMode.Create);
			BugReportController.SaveLogFile(result.ToString());
			break;
		default:
			GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX088_TeamCreateRes_Fail), null);
			BugReportController.SaveLogFile(result.ToString());
			break;
		}
	}
	static public void OnResponseTeamJoin(TeamEditResult result)
	{
		switch(result)
		{
		case TeamEditResult.Success:
			break;
		case TeamEditResult.MemberOver:
			GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX090_TeamJoinRes_MemberOver), () => {});
			break;
		case TeamEditResult.UnjustPassword:
			GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX091_TeamJoinRes_UnjustPassword), () => {});
			break;
		case TeamEditResult.InMatching:
			GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX094_TeamJoinRes_InMatching), () => {});
			break;
		case TeamEditResult.InBattle:
			GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX095_TeamJoinRes_InBattle), () => {});
			break;
		case TeamEditResult.TeamNotExist:
			GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX092_TeamJoinRes_TeamNotExist), () => {});
			break;
		case TeamEditResult.SenderInMatching:
			GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX093_TeamJoinRes_SenderInMatching), null);
			break;
		default:
			GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX096_TeamJoinRes_Fail), () => {});
			BugReportController.SaveLogFile(result.ToString());
			break;
		}
	}
	static public void OnResponseTeamRemoveMember(TeamEditResult result)
	{
		switch(result)
		{
		case TeamEditResult.Success:
			break;
		case TeamEditResult.NotFoundTarget:
			GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX099_TeamRemoveMemberRes_NotFoundTarget), () => {});
			break;
		case TeamEditResult.NoAuthority:
			GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX100_TeamRemoveMemberRes_NoAuthority), () => {});
			break;
		case TeamEditResult.TeamNotExist:
			GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX092_TeamJoinRes_TeamNotExist), () => {});
			break;
		default:
			GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX101_TeamRemoveMemberRes_Fail), () => {});
			BugReportController.SaveLogFile(result.ToString());
			break;
		}
	}
	static public void OnResponseTeamDissolve(TeamEditResult result)
	{
		switch(result)
		{
		case TeamEditResult.Success:
			break;
		case TeamEditResult.NoAuthority:
			GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX102_TeamDissolveRes_NoAuthority), () => {});
			break;
		case TeamEditResult.TeamNotExist:
			GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX092_TeamJoinRes_TeamNotExist), () => {});
			break;
		default:
			GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX103_TeamDissolveRes_Fail), () => {});
			BugReportController.SaveLogFile(result.ToString());
			break;
		}
	}
	#endregion

	#region NGUIリフレクション
	public void OnCreate()
	{
		this.ResetCreateField();
		this._SetMode(TeamMenuMode.Create);
	}
	public void OnSearch()
	{
		if(this.joinTeamParamList == null ||
			sendTeamSearchNextTime < Time.time)
		{
			CommonPacket.SendTeamSearch();
			sendTeamSearchNextTime = Time.time + SendTeamSearchTime;
			this.joinTeamParamList = null;
			this.teamMemberDic.Clear();
		}
		this._SetMode(TeamMenuMode.Join);
		this._attach.join.SetTeamParam(this.joinTeamParamList);
		this._attach.join.ResetMemberParam();
	}
	public void OnInvitation()
	{
		this._SetMode(TeamMenuMode.Invitation);
	}
	public void OnLeaderChange()
	{
		this._SetMode(TeamMenuMode.LeaderChange);
	}

	public void OnHome()
	{
		this._SetMode(TeamMenuMode.None);
	}
	public void OnClose()
	{
		switch(this.Mode)
		{
		case TeamMenuMode.None:
			break;
		case TeamMenuMode.Create:
		case TeamMenuMode.Join:
			this._SetMode(TeamMenuMode.SingleMenu);
			break;
		case TeamMenuMode.Invitation:
		case TeamMenuMode.LeaderChange:
			this._SetMode(TeamMenuMode.LeaderMenu);
			break;
		case TeamMenuMode.SingleMenu:
		case TeamMenuMode.LeaderMenu:
		default:
			this._SetMode(TeamMenuMode.None);
			break;
		}
	}

	public void OnCreateOk()
	{
		if(this.Attach.make.teamName != null && this.Attach.make.password != null)
		{
			string errorMessage;
			if(CommonPacket.TrySendTeamCreate(this.Attach.make.teamName.label.text, this.Attach.make.password.label.text, this.Attach.make.commentId, out errorMessage))
			{
				this._SetMode(TeamMenuMode.None);
			}
			else
			{
				GUIMessageWindow.SetModeOK(errorMessage, null);
			}
		}
	}
	public void OnCreatePullDownToggle()
	{
		OnCreatePullDown(!this.createPullDownForward);
	}
	private void OnCreatePullDown(bool isForward)
	{
		this.createPullDownForward = isForward;
		if(this.Attach.make.pullDown != null) { this.Attach.make.pullDown.Play(isForward); }
	}
	static public void SetComment(GUITeamCommentItem item)
	{
		if(GUITeamMenu.Instance != null) { GUITeamMenu.Instance._SetComment(item.CommentId); }
	}
	private void _SetComment(byte commentId)
	{
		this.Attach.make.commentId = commentId;
		if(this.Attach.make.comment != null) { this.Attach.make.comment.text = ObsolateSrc.TeamComment.GetTeamComment(commentId); }
		this.OnCreatePullDown(false);
	}
	public void OnDissolve()
	{
		CommonPacket.SendTeamRemoveMember(0);
		this._SetMode(TeamMenuMode.None);
	}
	static public void OnSearchSelect(GUITeamInfoItem item)
	{
		if(GUITeamMenu.Instance != null) { GUITeamMenu.Instance._OnSearchSelect(item); }
	}
	private void _OnSearchSelect(GUITeamInfoItem item)
	{
		this.Attach.join.SetSelectedItem(item);

		if(item != null)
		{
			// キャッシュ内に既にメンバー情報が有り,期限内ならそれをセットする.
			CacheData<List<GroupMemberParameter>> membersCache;
			if(this.teamMemberDic.TryGetValue(item.TeamId, out membersCache))
			{
				if(Time.time < membersCache.Time + SendTeamSearchTime)
				{
					this._attach.join.SetMemberParam(item.TeamId, membersCache.Data);
					return;
				}
			}
			// キャッシュヒットせず,あるいは有効期限切れなら情報請求.
			CommonPacket.SendTeamMember(item.TeamId);
		}
		// 理由はどうあれセットする情報が無ければ見た目をクリーンに.
		this._attach.join.ResetMemberParam();
	}
	public void OnSearchJoin()
	{
		if(this.Attach.join.SelectedItem != null &&
		   this.Attach.join.SelectedItem.TeamId != 0)
		{
			if(this.Attach.join.SelectedItem.IsPassword)
			{
				// パスワード入力処理へ.
				var pInput = new TeamJoinPasswordInput(this.Attach.join.SelectedItem.TeamId);
				pInput.OpenInputWindow();
			}
			else
			{
				CommonPacket.SendTeamJoin(this.Attach.join.SelectedItem.TeamId);
				this._SetMode(TeamMenuMode.None);
			}
		}
	}
	public void OnBackTeamList()
	{
		this.Attach.join.ShowBackTeamList();
	}
	public void OnNextTeamList()
	{
		this.Attach.join.ShowNextTeamList();
	}
	#endregion

	#region デバッグ
#if UNITY_EDITOR && XW_DEBUG
	[SerializeField]
	DebugParameter _debugParam = new DebugParameter();
	DebugParameter DebugParam { get { return _debugParam; } }
	[System.Serializable]
	public class DebugParameter
	{
		public ModeChange modeChange;
		public TeamList teamList;
	}
	[System.Serializable]
	public class ModeChange
	{
		public bool execute;
		public TeamMenuMode mode;
	}
	[System.Serializable]
	public class TeamList
	{
		public bool execute;
		public int count;
	}
	
	void DebugUpdate()
	{
		var t = this.DebugParam;
		if (t.modeChange.execute)
		{
			t.modeChange.execute = false;
			this._SetMode(t.modeChange.mode);
		}
		if (t.teamList.execute)
		{
			t.teamList.execute = false;
			List<TeamParameter> paramList = new List<TeamParameter>();
			for(int i = 0; i < t.teamList.count; ++i)
			{
				TeamParameter param = new TeamParameter();
				param.TeamId = i;
				param.TeamName = "Team" + i;
				param.NumOfMember = 1;
				param.LeaderAvatar = i % 10 + 1;
				param.LeaderMatchingStatus = (MatchingStatus)(i % 6);
				param.CommentId = (byte)(i % 8);
				param.IsPassword = i % 2 == 0 ? false : true;
				paramList.Add(param);
			}
			this._attach.join.SetTeamParam(paramList);
		}
	}
	void OnValidate()
	{
		if (Application.isPlaying)
			this.DebugUpdate();
	}
#endif
	#endregion
#endif
}
