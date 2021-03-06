/// <summary>
/// 共通パケット解析
/// 
/// 2012/12/10
/// </summary>

#define NEW_MOVE_MOTION

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ExitGames.Client.Photon;
using Asobimo.Photon.Packet;
using Scm.Common.Master;
using Scm.Common.Packet;
using Scm.Common.PacketCode;
using Scm.Common.GameParameter;
using Scm.Common.NGWord;
using Scm.Client;
using System;

public class CommonPacket {
    // Move パケット送信時のY位置のクランプ値（プラスマイナス）
    const float MovePositionHeightClamp = 1000f;
    // SendEntrantの多重送信防止用タイマー.
    const float SendEntrantInterval = 2;
    const float SendEntrantAllInterval = 10;
    static private float nextSendEntrantTime;

    #region EntrantAll パケット
    /// <summary>
    /// EntrantAll 送信
    /// フィールド内の全プレイヤーを取得する.
    /// クライアントでシーン移行が完了した通知でもあるので,EnterFieldRes,EnterLobbyRes後は必ず送信する.
    /// </summary>
    /// <param name="fieldId"></param>
    public static void SendEntrantAll(int fieldId) {
        EntrantAllReq packet = new EntrantAllReq();
        //packet.FieldId = fieldId;
        GameListener.Send(packet);
        nextSendEntrantTime = Time.time + SendEntrantAllInterval;
    }
    /// <summary>
    /// EntrantAll 受信応答
    /// </summary>
    public void OperationResponseEntrantAll(EntrantAllRes packet) {
        // パケットエラー.
        if (packet == null)
            return;

        var entrantResArray = packet.GetEntrantResArray();
        foreach (var entrantRes in entrantResArray) {
            OperationResponseEntrant(entrantRes);
        }

        foreach (var recruitment in packet.GetRecruitments()) {
            Entrant.UpdateRecruitment(recruitment);
        }
    }
    #endregion

    #region Entrant パケット
    /// <summary>
    /// Entrant 送信
    /// 他プレイヤーの情報要求
    /// </summary>
    /// <param name="inFieldId"></param>
    static void SendEntrant(short inFieldId) {
        if (nextSendEntrantTime < Time.time) {
            EntrantReq packet = new EntrantReq();
            packet.InFieldId = inFieldId;
            GameListener.Send(packet);
            nextSendEntrantTime = Time.time + SendEntrantInterval;
        }
    }
    /// <summary>
    /// Entrant 受信応答
    /// プレイヤーログイン時の他プレイヤー生成
    /// </summary>
    /// <param name="packet"></param>
    public void OperationResponseEntrant(EntrantRes packet) {
        // パケッドが違う
        if (packet == null)
            return;

        // タイプ別生成
        var info = EntrantInfo.Create(packet, false);
        info.CreateObject();
    }
    /// <summary>
    /// Entrant 受信通知
    /// 他プレイヤーログイン時の他プレイヤー生成
    /// </summary>
    /// <param name="packet"></param>
    public void EventEntrant(EntrantEvent packet) {
        // パケッドが違う
        if (packet == null)
            return;

        var info = EntrantInfo.Create(packet);

        // 存在するマップが違う
        if (!MapManager.Instance.IsSameMap(info.AreaType, info.FieldId))
            return;

        // タイプ別生成
        info.CreateObject();
    }
    #endregion

    #region Echo パケット
    /// <summary>
    /// Echo 送信.
    /// チャットの文字列を送信する.
    /// </summary>
    /// <param name='msg'>
    [System.Obsolete]
    public static void SendEcho(string msg) {
        EchoReq packet = new EchoReq();
        packet.Text = msg;
        GameListener.Send(packet);
    }
    /// <summary>
    /// 文字列　受信応答.
    /// </summary>
    /// <param name='packet'>
    public void OperationResponseEcho(EchoRes packet) {
        // パケッドが違う
        if (packet == null)
            return;
    }
    #endregion

    #region Move パケット
#if NEW_MOVE_MOTION
    /// <summary>
    /// Move 送信
    /// プレイヤー位置情報の送信
    /// </summary>
    public static void SendMove(Vector3 oldPosition, Vector3 position, Quaternion rotation) {
        MoveReq packet = new MoveReq();
        packet.OldPosition = new float[] { oldPosition.x, oldPosition.y, oldPosition.z };
        packet.Position = new float[] { position.x, Mathf.Clamp(position.y, -MovePositionHeightClamp, MovePositionHeightClamp), position.z };
        packet.Rotation = rotation.eulerAngles.y;
        GameListener.Send(packet);
    }

    public static void ProxySendMove(int inFieldId, Vector3 oldPosition, Vector3 position, Quaternion rotation) {
        MoveReq packet = new MoveReq();
        packet.OldPosition = new float[] { oldPosition.x, oldPosition.y, oldPosition.z };
        packet.Position = new float[] { position.x, Mathf.Clamp(position.y, -MovePositionHeightClamp, MovePositionHeightClamp), position.z };
        packet.Rotation = rotation.eulerAngles.y;
        ProxyReq req = new ProxyReq() {
            ActualCode = packet.Code,
            ActualPacket = packet.GetPacket(),
            InFieldId = inFieldId
        };

        GameListener.Send(req);
    }

    public static void SendConfirmMove(int inFieldId, Vector3 position, bool collision) {
        ConfirmMoveReq packet = new ConfirmMoveReq() {
            Position = new float[] { position.x, Mathf.Clamp(position.y, -MovePositionHeightClamp, MovePositionHeightClamp), position.z },
            Collision = collision
        };
        ProxyReq req = new ProxyReq() {
            ActualCode = packet.Code,
            ActualPacket = packet.GetPacket(),
            InFieldId = inFieldId
        };

        GameListener.Send(req);
    }

    public static void ProxySendMotionFinished(int inFieldId, Vector3 position, Quaternion rotation) {
        MotionStatusReq packet = new MotionStatusReq() {
            Finished = true,
            Position = new float[] { position.x, Mathf.Clamp(position.y, -MovePositionHeightClamp, MovePositionHeightClamp), position.z },
            Rotation = rotation.eulerAngles.y
        };
        ProxyReq req = new ProxyReq() {
            ActualCode = packet.Code,
            ActualPacket = packet.GetPacket(),
            InFieldId = inFieldId
        };

        GameListener.Send(req);
    }

    public static void ProxySendMotionStarted(int inFieldId, Vector3 position, Quaternion rotation) {
        MotionStatusReq packet = new MotionStatusReq() {
            Finished = false,
            Position = new float[] { position.x, Mathf.Clamp(position.y, -MovePositionHeightClamp, MovePositionHeightClamp), position.z },
            Rotation = rotation.eulerAngles.y
        };
        ProxyReq req = new ProxyReq() {
            ActualCode = packet.Code,
            ActualPacket = packet.GetPacket(),
            InFieldId = inFieldId
        };

        GameListener.Send(req);
    }
#else
	/// <summary>
	/// Move 送信
	/// プレイヤー位置情報の送信
	/// </summary>
	/// <param name="position"></param>
	/// <param name="rotation"></param>
	public static void SendMove(Vector3 position, Vector3 nextPosition, Quaternion nextRotation)
	{
		MoveReq packet = new MoveReq();
		packet.OldPosition = new float[]{position.x, position.y, position.z};
		packet.Position = new float[]{nextPosition.x, nextPosition.y, nextPosition.z};
		packet.Rotation = nextRotation.eulerAngles.y;
		GameListener.Send(packet);
	}
#endif
    /// <summary>
    /// Move 受信通知
    /// 他プレイヤーの移動処理
    /// </summary>
    /// <param name="packet"></param>
    public void EventMove(MoveEvent packet)
	{
		// パケッドが違う
		if (packet == null)
			return;

		EntrantInfo entrant;
		if(Entrant.TryGetEntrant(packet.InFieldId, out entrant))
		{
			entrant.Move(
				new Vector3(packet.OldPosition[0], packet.OldPosition[1], packet.OldPosition[2]),
				new Vector3(packet.Position[0], packet.Position[1], packet.Position[2]),
				Quaternion.Euler(0.0f, packet.Rotation, 0.0f),
                packet.ForceGrounded
			);
		}
		else
		{
			// 存在していない
			SendEntrant(packet.InFieldId);
		}
	}
	#endregion

	#region Chat パケット
	/// <summary>
	/// チャット送信.
	/// </summary>
	public static void SendChat(ChatType chatType, string text, long whisperPlayerID)
	{
		ChatReq packet = new ChatReq();
		packet.ChatType = chatType;
		packet.Text = text;
		packet.PlayerId = whisperPlayerID;
		GameListener.Send(packet);
	}
	/// <summary>
	/// チャット受信通知.
	/// </summary>
	public void EventChat(ChatEvent packet)
	{
		// パケット違う
		if (packet == null)
			return;

		if (GMCommand.IsGMCommand(packet.Text))
		{
#if XW_DEBUG
			// デバッグアプリ版はGMコマンドを処理する
			// それ以外は何もしない
			GMCommand.CommandAll(packet.Text);
			GUIDebugLog.AddMessage(packet.Text);
			Debug.Log(packet.Text);
#endif
			return;
		}

		// チャットメッセージ
		var chatInfo = new ChatInfo(packet);
		//GUIChat.AddMessage(chatInfo);
        XUI.GUIChatFrameController.AddMessage(chatInfo);
	}
	#endregion

	#region PlayerDisplayStatus パケット
	/// <summary>
	/// プレイヤー表示状態受信通知.
	/// </summary>
	public void EventPlayerDisplayState(PlayerDisplayStateEvent packet)
	{
		// UNDONE: 参戦予約をチャットウィンドウに表示するための暫定実装
		// パケット違う
		if (packet == null)
			return;

		ObjectBase objectBase = GameController.SearchInFieldID(packet.InFieldId);

		if (objectBase != null)
		{
			// プレイヤー自身の表示は行わない
			if (objectBase is Player)
				return;

			switch (packet.AutoState)
			{
			case PlayerDisplayState.None:
				//GUIChat.AddMessage(string.Format("{0}が参戦予約をキャンセルしました", objectBase.UserName));
				break;

			case PlayerDisplayState.BattleEntry:
				//GUIChat.AddMessage(string.Format("{0}が参戦予約しました", objectBase.UserName));
				//GUIChat.AddMessage(string.Format("{0}が参戦状態を変更しました", objectBase.UserName));
				break;
			}
		}
	}

	#endregion

	#region CharacterDeckNum パケット
	/// <summary>
	/// CharacterDeckNum 送信
	/// </summary>
	public static void SendCharacterDeckNum()
	{
		CharacterDeckNumReq packet = new CharacterDeckNumReq();
		GameListener.Send(packet);
	}
	/// <summary>
	/// CharacterDeckNumRes 受信通知
	/// </summary>
	public void OperationResponseCharacterDeckNum(CharacterDeckNumRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

//Num	int	キャラデッキ所有数
//CurrentDeckId	int	現在選択中のデッキ番号(1～デッキ所有数)
		// ページ設定
//		GUIDeckEdit.SetPage(packet.CurrentDeckId, packet.Num);
		// 現在のデッキ内容を問い合わせる
		CommonPacket.SendCharacterDeck(packet.CurrentDeckId);
	}
	#endregion

	#region CharacterDeckList パケット
	/// <summary>
	/// CharacterDeckList 送信
	/// </summary>
	public static void SendCharacterDeckList(int startDeckID, int count)
	{
		CharacterDeckListReq packet = new CharacterDeckListReq();
		packet.StartDeckId = startDeckID;
		packet.Count = count;
//StartDeckId	int	開始デッキID（1～）
//Count	int	取得数
		GameListener.Send(packet);
	}
	/// <summary>
	/// CharacterDeckListRes 受信通知
	/// </summary>
	public void OperationResponseCharacterDeckList(CharacterDeckListRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

//Count	int	実際に取得出来た数
//Parameters	Dictionary<byte,object>	取得出来たCharacterDeckResの集合
		//packet.Count;
		//foreach (var t in packet.GetDeckListPackets())
		{
			//t.DeckId;
			//t.Name;
			//t.GetOwnCharacterPacketParameterArray();
		}

	}
	#endregion

	#region CharacterDeck パケット
	/// <summary>
	/// CharacterDeck 送信
	/// </summary>
	public static void SendCharacterDeck(int deckID)
	{
		CharacterDeckReq packet = new CharacterDeckReq();
		packet.DeckId = deckID;
//DeckId	int	デッキID(1～デッキ所有数)
		GameListener.Send(packet);
	}
	/// <summary>
	/// CharacterDeckRes 受信通知
	/// </summary>
	public void OperationResponseCharacterDeck(CharacterDeckRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

//DeckId	int	デッキID(1～デッキ所有数)
//Name	string	デッキ名
//Parameters	Dictionary<byte,object>	OwnCharacterPacketParameterの集合
		// デッキ情報を解析
		var deckInfo = new DeckInfo(packet);
		// デッキ設定

	    if (GUIDeckEdit.NowMode == GUIDeckEdit.DeckMode.CharaSelect)
	    {
	        GUIDeckEdit.SetDeck(deckInfo, deckInfo.CurrentSlotIndex);
	    }
	    else
	    {
            DeckEdit.SetDetail(deckInfo);
	    }
	}
	#endregion

	#region SetCharacterDeck パケット
	/// <summary>
	/// SetCharacterDeck 送信
	/// </summary>
	public static void SendSetCharacterDeck(int deckID, string name, ulong[] charaIDs)
	{
		SetCharacterDeckReq packet = new SetCharacterDeckReq();
		packet.DeckId = deckID;
		packet.Name = name;
		packet.PlayerCharacterUuids = Scm.Common.Utility.ToLongArray(charaIDs);
//DeckId	int	デッキID(1～デッキ所有数)
//Name	string	デッキ名
//PlayerCharacterUuids	long[]	所有キャラID（デッキのスロット順）
		GameListener.Send(packet);
	}
	/// <summary>
	/// SetCharacterDeckRes 受信通知
	/// </summary>
	public void OperationResponseSetCharacterDeck(SetCharacterDeckRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

//DeckId    int	デッキID(1～デッキ所有数)
//SetCharacterDeckResult	SetCharacterDeckResult	設定結果(成功、失敗、コストオーバー)
		switch (packet.SetCharacterDeckResult)
		{
		case SetCharacterDeckResult.Success:
			GUIChat.AddSystemMessage(false, MasterData.GetText(TextType.TX131_SetCharacterDeckRes_Success));
			break;

		case SetCharacterDeckResult.Fail:
			GUIChat.AddSystemMessage(true, MasterData.GetText(TextType.TX132_SetCharacterDeckRes_Fail));
			break;

		case SetCharacterDeckResult.CostOver:
			GUIChat.AddSystemMessage(true, MasterData.GetText(TextType.TX133_SetCharacterDeckRes_CostOver));
			break;
		}
	}
	#endregion

	#region SetSymbolPlayerCharacter パケット
	/// <summary>
	/// SetSymbolPlayerCharacter 受信引数
	/// </summary>
	public class SetSymbolPlayerCharacterResArgs : EventArgs
	{
		public bool Result { get; set; }
		public ulong SymbolPlayerCharacterUUID { get; set; }
	}
	static event System.Action<SetSymbolPlayerCharacterResArgs> SetSymbolPlayerCharacterResponse = null;
	/// <summary>
	/// SetSymbolPlayerCharacter 送信
	/// </summary>
	public static void SendSetSymbolPlayerCharacterReq(ulong characterUuID, System.Action<SetSymbolPlayerCharacterResArgs> response)
	{
		if (SetSymbolPlayerCharacterResponse == null)
		{
			SetSymbolPlayerCharacterReq packet = new SetSymbolPlayerCharacterReq();
			packet.SymbolPlayerCharacterUuid = (long)characterUuID;

			Player player = GameController.GetPlayer();
			if (player)
			{
				player.SetBlockSendMoveTime(GameConstant.BlockSendMoveTime_Respawn);
			}

			GameListener.Send(packet);
		}

		SetSymbolPlayerCharacterResponse += response;
		// RespanwEvent パケットも同時に送信される
	}
	/// <summary>
	/// SetSymbolPlayerCharacter 受信通知
	/// </summary>
	public void OperationSetSymbolPlayerCharacterRes(SetSymbolPlayerCharacterRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// パケット変換
		var eventArgs = new SetSymbolPlayerCharacterResArgs();
		eventArgs.Result = packet.Result;
		eventArgs.SymbolPlayerCharacterUUID = (ulong)packet.SymbolPlayerCharacterUuid;

		if (!eventArgs.Result)
		{
			GUIChat.AddSystemMessage(true, MasterData.GetText(TextType.TX137_SetMainOwnCharacterRes_Fail));
		}

		// 結果を通知する
		if (SetSymbolPlayerCharacterResponse != null)
		{
			SetSymbolPlayerCharacterResponse(eventArgs);
			SetSymbolPlayerCharacterResponse = null;
		}
	}
	#endregion

	#region SelectCharacterDeck パケット
	/// <summary>
	/// SelectCharacterDeck 送信
	/// </summary>
	public static void SendSelectCharacterDeck(int deckID)
	{
		SelectCharacterDeckReq packet = new SelectCharacterDeckReq();
		packet.DeckId = deckID;
//DeckId	int	デッキID(1～デッキ所有数)
		GameListener.Send(packet);
	}
	/// <summary>
	/// SelectCharacterDeckRes 受信通知
	/// </summary>
	public void OperationResponseSelectCharacterDeck(SelectCharacterDeckRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

//DeckId	int	デッキID(1～デッキ所有数)
//Result	bool	選択結果(true=成功、false=失敗)
	}
    #endregion

    //  UNDONE: Common.DLL: 使用しなくなったパケットの関連コードをコメントアウト
    /*
	#region SelectCharacterTimeUp パケット
	/// <summary>
	/// SelectCharacterTimeUpEvent 受信通知
	/// </summary>
	public void EventSelectCharacterTimeUp(SelectCharacterTimeUpEvent packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// UNDONE:実装するのか未定のためPacketHandlerで受け取っていない
//Index	int	デッキのスロットIndex(0～3)
	}
	#endregion
    */
#if OLD_TEAM_LOGIC
    #region チーム
    #region TeamInfo パケット
    public void EventTeamInfo(TeamInfoEvent packet)
	{
		// パケットが違う
		if (packet == null)
			{ return; }

		// 値を保存.
		NetworkController.ServerValue.SetTeamInfo(packet);
	}
    #endregion
    #region TeamCreate パケット
    static public bool TrySendTeamCreate(string teamName, string password, byte commentId, out string errorMessage)
	{
		if(string.IsNullOrEmpty(teamName))
		{
			errorMessage = MasterData.GetText(TextType.TX080_TeamCreate_NameEmpty);
			return false;
		}
		else if(10 < teamName.Length)
		{
			errorMessage = MasterData.GetText(TextType.TX081_TeamCreate_NameOver);
			return false;
		}
		else if(!string.IsNullOrEmpty(password) &&
				(password.Length != 4 || !NGWord.IsAlphanumeric(password)))
		{
			errorMessage = MasterData.GetText(TextType.TX082_TeamCreate_PasswordError);
			return false;
		}

		TeamCreateReq packet = new TeamCreateReq();
		packet.TeamName = teamName;
		packet.Password = password;
		packet.CommentId = commentId;

		GameListener.Send(packet);
		errorMessage = null;
		return true;
	}
	public void OperationResponseTeamCreate(TeamCreateRes packet)
	{
		// パケットが違う
		if (packet == null)
			{ return; }

		GUITeamMenu.OnResponseTeamCreate((TeamEditResult)packet.Result);
	}
    #endregion
    #region TeamRemoveMember パケット
	static public void SendTeamRemoveMember(long playerId)
	{
		TeamRemoveMemberReq packet = new TeamRemoveMemberReq();
		packet.PlayerId = playerId;

		GameListener.Send(packet);
	}
	public void OperationResponseTeamRemoveMember(TeamRemoveMemberRes packet)
	{
		// パケットが違う
		if (packet == null)
			{ return; }

		if(packet.PlayerId == 0)
		{
			GUITeamMenu.OnResponseTeamDissolve((TeamEditResult)packet.Result);
		}
		else
		{
			GUITeamMenu.OnResponseTeamRemoveMember((TeamEditResult)packet.Result);
		}
	}
    #endregion
    #region TeamSearch パケット
	static public void SendTeamSearch()
	{
		TeamSearchReq packet = new TeamSearchReq();

		GameListener.Send(packet);
	}
	public void OperationResponseTeamSearch(TeamSearchRes packet)
	{
		// パケットが違う
		if (packet == null)
			{ return; }

		GUITeamMenu.SetSearchTeam(packet.GetTeamParameters());
	}
    #endregion
    #region TeamJoin パケット
	static public void SendTeamJoin(long teamId)
	{
		TeamJoinReq packet = new TeamJoinReq();
		packet.TeamId = teamId;

		GameListener.Send(packet);
	}
	static public bool TrySendTeamJoin(long teamId, string password, out string errorMessage)
	{
		if(string.IsNullOrEmpty(password) ||
			password.Length != 4 ||
			!NGWord.IsAlphanumeric(password))
		{
			errorMessage = MasterData.GetText(TextType.TX091_TeamJoinRes_UnjustPassword);
			return false;
		}
		
		TeamJoinReq packet = new TeamJoinReq();
		packet.TeamId = teamId;
		packet.Password = password;
		
		GameListener.Send(packet);
		errorMessage = null;
		return true;
	}
	public void OperationResponseTeamJoin(TeamJoinRes packet)
	{
		// パケットが違う
		if (packet == null)
			{ return; }

		GUITeamMenu.OnResponseTeamJoin((TeamEditResult)packet.Result);
	}
    #endregion
    #region TeamMember パケット
	static public void SendTeamMember(long teamId)
	{
		TeamMemberReq packet = new TeamMemberReq();
		packet.TeamId = teamId;

		GameListener.Send(packet);
	}
	public void OperationResponseTeamMember(TeamMemberRes packet)
	{
		// パケットが違う
		if (packet == null)
			{ return; }

		GUITeamMenu.SetSearchTeamMember(packet.TeamId, packet.GetMembers());
	}
    #endregion
    #endregion
#endif

	#region GmCommand パケット
	static public void SendGmCommand(string command)
	{
		GmCommandReq packet = new GmCommandReq();
		packet.Command = command;

		GameListener.Send(packet);
	}
	public void OperationResponseGmCommand(GmCommandRes packet)
	{
		// パケットが違う
		if (packet == null)
			{ return; }

        XUI.GUIChatFrameController.AddSystemMessage(false, packet.Text);
        //GUIChatFrameController.AddMessage(fals)
	}
	#endregion

	#region Auth パケット
	static IPacketResponse<AuthRes> AuthResponse { get; set; }
	/// <summary>
	/// Auth 送信
	/// </summary>
	public static void SendAuth(PlatformType platformType, MarketType marketType, Language language, byte authType, byte distributionCode, string asobimoToken, IPacketResponse<AuthRes> response)
	{
		AuthReq packet = new AuthReq();
		packet.PlatformType = platformType;
		packet.MarketType = marketType;
		packet.Language = language;
		packet.AuthType = authType;
        packet.DistributionCode = distributionCode;
        packet.Token = asobimoToken;
		GameListener.SendConnected(packet);

		AuthResponse = response;
	}
	/// <summary>
	/// Auth 受信通知
	/// </summary>
	public void OperationResponseAuth(AuthRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// 結果を通知する
		AuthResponse.Response(packet);
		AuthResponse = null;
	}
    #endregion
    #region Server Mapping Packet
    static Action<ServerMappingRes> ServerMappingResponse { get; set; }

    /// <summary>
    /// Send server mapping request
    /// </summary>
    /// <param name="callback"></param>
    public static void SendServerMapping(Action<ServerMappingRes> callback) {
        ServerMappingResponse = callback;
        var p = new ServerMappingReq();
#if XW_DEBUG
        p.Environment = ScmParam.Debug.File.Environment;
#else
        p.Environment = Scm.Common.GameParameter.EnvironmentType.BetaTest;
#endif
        var dict = p.GetPacket();
        Debug.Log("dict.keys=" + dict.Count);
        Debug.Log("errorMessage=" + p.GetErrorMessage());
        GameListener.SendConnected(p);
    }

    /// <summary>
    /// Should be called when server mapping response is reached
    /// </summary>
    /// <param name="res"></param>
    public void OperationResponseServerMapping(ServerMappingRes res) {
        if (res == null || ServerMappingResponse == null) {
            return;
        }

        ServerMappingResponse.Invoke(res);
        ServerMappingResponse = null;
    }

#endregion

#region SetCurrentAvatar 设定角色当前皮肤
    private static event Action<SetCurrentAvatarRes> SetCurrentAvatar = null;
    /// <summary>
    /// 请求设定角色当前皮肤
    /// </summary>
    public static void SendSetCurrentAvatar(long characterUuid, int avatarId, Action<SetCurrentAvatarRes> response)
    {
        var packet = new SetCurrentAvatarReq()
        {
            CharacterUuid = characterUuid,
            AvatarId = avatarId,
        };
        GameListener.Send(packet);

        SetCurrentAvatar += response;
    }

    /// <summary>
    /// 响应设定角色当前皮肤
    /// </summary>
    public void OperationResponseSetCurrentAvatar(SetCurrentAvatarRes packet)
    {
        if (packet == null) return;

        var args = new SetCurrentAvatarRes()
        {
            Result = packet.Result,
        };

        if (SetCurrentAvatar != null)
        {
            SetCurrentAvatar(args);
            SetCurrentAvatar = null;
        }
    }
#endregion

#region SetCurrentAvatar 获取角色所有皮肤
    private static event Action<List<CharacterAvatarParameter>> GetCharacterAvatarAll = null;
    /// <summary>
    /// 请求获取角色所有皮肤
    /// </summary>
    public static void SendGetCharacterAvatarAll(long characterUuid, Action<List<CharacterAvatarParameter>> response)
    {
        var packet = new GetCharacterAvatarAllReq()
        {
            CharacterUuid = characterUuid,
        };
        GameListener.Send(packet);

        GetCharacterAvatarAll += response;
    }

    /// <summary>
    /// 响应获取角色所有皮肤
    /// </summary>
    public void OperationResponseGetCharacterAvatarAll(GetCharacterAvatarAllRes packet)
    {
        if (packet == null) return;
        var args = new List<CharacterAvatarParameter>();
        args.AddRange(packet.GetCharacterAvatarParameters());

        if (GetCharacterAvatarAll != null)
        {
            GetCharacterAvatarAll(args);
            GetCharacterAvatarAll = null;
        }
    }
#endregion

    #region SetCurrentAvatar 获取玩家所有皮肤
    private static event Action<List<ObtainedCharacterAvatarParameter>> ObtainedCharacterAvatarAll = null;
    /// <summary>
    /// 请求获取角色所有皮肤
    /// </summary>
    public static void SendObtainedCharacterAvatarAll(int[] avatarIds, Action<List<ObtainedCharacterAvatarParameter>> response)
    {
        var packet = new ObtainedCharacterAvatarAllReq()
        {
            AvatarIds = avatarIds,
        };
        GameListener.Send(packet);

        ObtainedCharacterAvatarAll += response;
    }

    /// <summary>
    /// 响应获取玩家所有皮肤
    /// </summary>
    public void OperationResponseObtainedCharacterAvatarAll(ObtainedCharacterAvatarAllRes packet)
    {
        if (packet == null) return;
        var args = new List<ObtainedCharacterAvatarParameter>();
        args.AddRange(packet.GetObtainedCharacterAvatarParameter());

        if (ObtainedCharacterAvatarAll != null)
        {
            ObtainedCharacterAvatarAll(args);
            ObtainedCharacterAvatarAll = null;
        }
    }
    #endregion

#region SetCurrentAvatar 获取角色所有声优
    private static event Action<List<ReplayVoiceParameter>> GetCharacterReplayVoiceAll = null;
    /// <summary>
    /// 请求获取角色所有声优
    /// </summary>
    public static void SendGetCharacterReplayVoiceAll(long characterUuid, Action<List<ReplayVoiceParameter>> response)
    {
        var packet = new GetCharacterReplayVoiceAllReq()
        {
            CharacterUuid = characterUuid,
        };
        GameListener.Send(packet);

        GetCharacterReplayVoiceAll += response;
    }

    /// <summary>
    /// 响应获取角色所有声优
    /// </summary>
    public void OperationResponseGetCharacterReplayVoiceAll(GetCharacterReplayVoiceAllRes packet)
    {
        if (packet == null) return;
        var args = new List<ReplayVoiceParameter>();
        args.AddRange(packet.GetCharacterReplayVoiceParameters());

        if (GetCharacterReplayVoiceAll != null)
        {
            GetCharacterReplayVoiceAll(args);
            GetCharacterReplayVoiceAll = null;
        }
    }
#endregion

#region SetCurrentAvatar 获取角色所有剧情
    private static event Action<List<CharacterStoryParameter>> GetCharacterStoryAll = null;
    /// <summary>
    /// 请求获取角色所有剧情
    /// </summary>
    public static void SendGetCharacterStoryAll(long characterUuid, Action<List<CharacterStoryParameter>> response)
    {
        var packet = new GetCharacterStoryAllReq()
        {
            CharacterUuid = characterUuid,
        };
        GameListener.Send(packet);

        GetCharacterStoryAll += response;
    }

    /// <summary>
    /// 响应获取角色所有剧情
    /// </summary>
    public void OperationResponseGetCharacterStoryAll(GetCharacterStoryAllRes packet)
    {
        if (packet == null) return;
        var args = new List<CharacterStoryParameter>();
        args.AddRange(packet.GetCharacterStoryParameters());

        if (GetCharacterStoryAll != null)
        {
            GetCharacterStoryAll(args);
            GetCharacterStoryAll = null;
        }
    }
#endregion

#region SetCurrentAvatar 获取角色所有壁纸
    private static event Action<List<CharacterWallpaperParameter>> GetCharacterWallpaperAll = null;
    /// <summary>
    /// 请求获取角色所有壁纸
    /// </summary>
    public static void SendGetCharacterWallpaperAll(long characterUuid, Action<List<CharacterWallpaperParameter>> response)
    {
        var packet = new GetCharacterWallpaperAllReq()
        {
            CharacterUuid = characterUuid,
        };
        GameListener.Send(packet);

        GetCharacterWallpaperAll += response;
    }

    /// <summary>
    /// 响应获取角色所有壁纸
    /// </summary>
    public void OperationResponseGetCharacterWallpaperAll(GetCharacterWallpaperAllRes packet)
    {
        if (packet == null) return;
        var args = new List<CharacterWallpaperParameter>();
        args.AddRange(packet.GetCharacterWallpaperParameters());

        if (GetCharacterWallpaperAll != null)
        {
            GetCharacterWallpaperAll(args);
            GetCharacterWallpaperAll = null;
        }
    }
#endregion
#region 组队
    //邀请===================================================================================
    public static void SendTeamInvite(long[] pIds)
    {
        var packet = new TeamInviteReq();
        packet.PlayerIds = pIds;
        GameListener.Send(packet, 10, (code, response) =>
        {
            Debug.Log("===> Send " + code);
            OnResponseTeamInvite(response);
        });
    }

    public static void OnResponseTeamInvite(TeamInviteRes pInviteRes)
    {
         Debug.Log("===> 未处理 " + pInviteRes.Code);
//        Debug.Log(pInviteRes.Code);
    }

    public static void OnResponseTeamInvite(OperationResponse pResponse)
    {
        if ((short)Scm.Common.ReturnCode.Ok == pResponse.ReturnCode)
        {
            GameListener.GameCommunication.OnOperationResponse(pResponse);
            return;
        }
        if ((short)Scm.Common.ReturnCode.OperationInProgress == pResponse.ReturnCode)
        {
           Debug.Log("===> 请不要平凡请求");
           return;
        }
        if ((short) Scm.Common.ReturnCode.CannotEnter == pResponse.ReturnCode)
        {
            Debug.Log("===> 处于队伍中");
            return;
        }
        if ((short) Scm.Common.ReturnCode.Fatal == pResponse.ReturnCode)
        {
            Debug.Log("===> 致命（对方可能离线）");
            return;
        }
        Debug.Log("===> 未处理 " + pResponse.ReturnCode);
    }

    private static long TeamId;
    public static void OnEventTeamInvite(TeamInviteEvent pEvent)
    {
        //        Debug.Log(pEvent.Code);
        Debug.Log("===> Receive " + pEvent.PlayerName + " Invite");
        TeamId = pEvent.TeamId;
        GUITeamMatch.Instance.ShowInvite(true, pEvent);
    }
    //End邀请=====================================================================================

    //邀请应答====================================================================================
    public static void SendInviteAccOrRej(TeamInviteAckReq.OperationType pType)
    {
        var packet = new TeamInviteAckReq();
        packet.TeamId = TeamId;
        packet.Operation = pType;
        GameListener.Send(packet, 10, (code, response) =>
        {
            Debug.Log("===> Send " + code);
            OnResponseInviteAccOrRej(response);
        });
    }

    public static void SendJoinTeamReq(int teamId) {
        var packet = new TeamInviteAckReq() {
            Operation = TeamInviteAckReq.OperationType.JoinRecruitment,
            TeamId = teamId
        };
        GameListener.Send(packet, 10, (code, response) => {
            Debug.Log("===> Send " + code);
            OnResponseInviteAccOrRej(response);
        });
    }

    public static void OnResponseInviteAccOrRej(OperationResponse pResponse)
    {
        if ((ushort)Scm.Common.ReturnCode.Ok == pResponse.ReturnCode)
        {
            Debug.Log(pResponse.ReturnCode);
            Debug.Log(pResponse.DebugMessage);
            return;
        }
        Debug.Log("===> 未处理 " + pResponse.ReturnCode);
    }

    public static void OnEventInviteAccOrRej(TeamMemberEvent pEvent)
    {
        //        Debug.Log(pEvent.Code);
//        Debug.Log("===> Receive " + pEvent.Event + " Invite");
//        Scm.Common.PacketCode.EventCode
        if (TeamMemberEvent.EventType.Reject == pEvent.Event)
        {
            Debug.Log("===> " + pEvent.GetPlayerParameter().PlayerName + " 拒绝邀请");
        }else if (TeamMemberEvent.EventType.Join == pEvent.Event)
        {
            var Player = pEvent.GetPlayerParameter();
            Debug.Log("===> " + Player.PlayerName + " 接受邀请");
        }
        else if (TeamMemberEvent.EventType.Dismiss == pEvent.Event)
        {
            Debug.Log("===> 队伍解散");
            GUITeamMatch.LeaveTeam();
        }
        else if (TeamMemberEvent.EventType.Remove == pEvent.Event)
        {
            Debug.Log("===> " + pEvent.GetPlayerParameter().PlayerName + " 退出组队");

            if (NetworkController.ServerValue.PlayerId == pEvent.GetPlayerParameter().PlayerId)
            {
                GUITeamMatch.LeaveTeam();
            }
        }
        else if (TeamMemberEvent.EventType.Logout == pEvent.Event)
        {
            Debug.Log("===> " + pEvent.GetPlayerParameter().PlayerName + " 退出客户端");
        }
        else
        {
            Debug.Log("===> " + " 未处理 " + pEvent.Event);
        }
    }
    //End邀请应答=================================================================================

    //组队状态====================================================================================
    /// <param name="pTeamId">0表示当前所在队伍</param>
    public static void SendGetTeamInfo(int pTeamId = 0)
    {
        var packet = new TeamMemberReq();
        packet.TeamId = pTeamId;
        GameListener.Send(packet, 10, (code, response) =>
        {
            Debug.Log("===> Send " + code);
            GameListener.GameCommunication.OnOperationResponse(response);
        });
    }

    public static void OnResponseGetTeamInfo(TeamMemberRes pTeamMemberRes)
    {
        Debug.Log("===> 未处理 " + pTeamMemberRes.Code);
//        pTeamMemberRes.
    }
    //End组队状态=================================================================================

    //组队操作====================================================================================
    /// <param name="pTeamId">0表示当前所在队伍</param>
    public static void SendTeamOperation(TeamOperationReq.OperationType pOperationType, long pRemovePlayerId = 0)
    {
        var packet = new TeamOperationReq();
        packet.Operation = pOperationType;
        packet.PlayerId = pRemovePlayerId;
        GameListener.Send(packet, 10, (code, response) =>
        {
            Debug.Log("===> Send " + code);
            Debug.Log("===> responsecode " + response.ReturnCode);
            if ((short)Scm.Common.ReturnCode.Ok == response.ReturnCode)
            {
                if (pOperationType == TeamOperationReq.OperationType.Exit)
                {
                    Debug.Log("===> 不需处理 交给OnEventInviteAccOrRej处理 ");
                }
                else if (pOperationType == TeamOperationReq.OperationType.RemoveMember)
                {
                    Debug.Log("===> 不需处理 交给OnEventInviteAccOrRej处理 ");
                }
                else if (pOperationType == TeamOperationReq.OperationType.Dismiss)
                {
                    Debug.Log("===> 不需处理 交给OnEventInviteAccOrRej处理 ");
                }
                else
                {
                    Debug.Log("===> 未处理 " + pOperationType);
                }
            }
            else
            {
                Debug.Log("===> Error " + code);
            }
        });
    }

    public static void OnResponseTeamOperation(TeamOperationRes pTeamOperationRes)
    {
        Debug.Log("===> 未处理 " + pTeamOperationRes.Code);
        //        pTeamMemberRes.
    }
    //End组队操作=================================================================================

    //队伍信息====================================================================================
    /// <param name="pTeamId">0表示当前所在队伍</param>
//    public static void SendTeamOperation(TeamOperationReq.OperationType pOperationType, long pRemovePlayerId = 0)
//    {
//        var packet = new TeamOperationReq();
//        packet.Operation = pOperationType;
//        packet.PlayerId = pRemovePlayerId;
//        GameListener.Send(packet, 10, (code, response) =>
//        {
//            Debug.Log("===> Send " + code);
//            Debug.Log("===> responsecode " + response.ReturnCode);
//            GameListener.GameCommunication.OnOperationResponse(response);
//        });
//    }

    //队伍发生变动时候调用，但是当自己主动退出或者被房主移出时候无法接收到，这个时候要通过OnEventInviteAccOrRej来判断处理
    public static void OnEventTeamInfo(TeamInfoEvent pTeamInfoEvent)
    {
//        Debug.Log("===> 未处理 " + pTeamInfoEvent.Code);
        var MemberList =     pTeamInfoEvent.GetMembers();
        var TeamInfo = pTeamInfoEvent.GetTeamParameter();
        GUITeamMatch.OnRefrshTeam(MemberList);

        //        pTeamMemberRes.
    }
    //End队伍信息=================================================================================
    /// <summary>
    /// Gold or coin Changed
    /// </summary>
    /// <param name="statusChangedEvent"></param>
    public static void OnEventStatusChanged(StatusChangedEvent statusChangedEvent)
    {
        Debug.Log("OnEventStatusChanged===>Gold:" + statusChangedEvent.Gold + "Coin:" + statusChangedEvent.Coin + "Energy:" + statusChangedEvent.Energy);
        XDATA.PlayerData.Instance.Gold = statusChangedEvent.Gold;
        XDATA.PlayerData.Instance.Coin = statusChangedEvent.Coin;
    }
#endregion

#region Get guide
    /// <summary>
    /// get guide 
    /// </summary>
    public class GetGuideResArgs : EventArgs
    {
        public bool result { get; set; }
        public int step { get; set; }

    }
    static event System.Action<GetGuideResArgs> GetGuideResponse = null;

    /// <summary>
    /// get guide
    /// </summary>
    /// <param name="response"></param>
    public static void SendGetGuide(System.Action<GetGuideResArgs> response)
    {
        var packet = new GetGuideReq();
        GameListener.Send(packet);

        GetGuideResponse += response;
    }

    /// <summary>
    /// response get guide
    /// </summary>
    /// <param name="packet"></param>
    public static void OperationResponseGetGuide(GetGuideRes packet)
    {
        if (packet == null)
            return;

        var eventArgs = new GetGuideResArgs();
        eventArgs.step = packet.Step;
        eventArgs.result = packet.Result;
        if (GetGuideResponse != null)
        {
            GetGuideResponse(eventArgs);
            GetGuideResponse = null;
        }
    }
#endregion

#region Set guide
    /// <summary>
    /// get guide 
    /// </summary>
    public class SetGuideResArgs : EventArgs
    {
        public bool result { get; set; }
        public int step { get; set; }
    }
    static event System.Action<SetGuideResArgs> SetGuideResponse = null;

    /// <summary>
    /// get guide
    /// </summary>
    /// <param name="step"></param>
    /// <param name="response"></param>
    public static void SendSetGuide(int step, System.Action<SetGuideResArgs> response)
    {
        var packet = new SetGuideReq();
        packet.Step = step;
        GameListener.Send(packet);

        SetGuideResponse += response;
    }

    /// <summary>
    /// response get guide
    /// </summary>
    /// <param name="packet"></param>
    public static void OperationResponseSetGuide(SetGuideRes packet)
    {
        if (packet == null)
            return;

        var eventArgs = new SetGuideResArgs();
        eventArgs.result = packet.Result;
        eventArgs.step = packet.Step;

        if (SetGuideResponse != null)
        {
            SetGuideResponse(eventArgs);
            SetGuideResponse = null;
        }
    }
    #endregion

    #region Friend
    public static void SendDeleteFriend(long friendId) {
        var p = new FriendReq();
        p.SetDeleteFriendParameter(friendId);
        GameListener.Send<FriendRes>(p, 10, (code, res, rc) => {
            if (code != ResponseResultCode.Success) {
                Debug.LogError("Failed to send delete friend req:" + code);
                return;
            }
            Net.Network.Instance.StartCoroutine(XDATA.PlayerData.Instance.GetFriendsList());
        });
    }
    #endregion
}
