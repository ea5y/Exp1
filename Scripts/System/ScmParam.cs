/// <summary>
/// サイコマ内パラメータ
/// 
/// 2014/05/27
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.GameParameter;
using Scm.Common.Master;

public static class ScmParam
{
    /// <summary>
    /// Master server's host address
    /// </summary>
	public static string MasterHost
	{
		get
		{
#if BETA_SERVER
            if (_masterHost == null) {
                List<ServerMappingMasterData> candidate = new List<ServerMappingMasterData>();
                foreach (var server in ServerMappingMaster.Instance.Servers) {
                    if (server.Environment == EnvironmentType.BetaTest) {
                        candidate.Add(server);
                    }
                }
                if (candidate.Count == 0) {
                    UnityEngine.Debug.LogError("No beta server in master data");
                    _masterHost = ObsolateSrc.BetaGameServerHost;
                } else {
                    _masterHost = candidate[Random.Range(0, candidate.Count)].Address;
                }
            }
            return _masterHost;
#elif TEST_SERVER
            return ObsolateSrc.TestGameServerHost;
#elif XW_DEBUG
            if (ScmParam.Debug.File.IsDebugMode)
			{
				return ScmParam.Debug.File.Host;
			} else {
                return ObsolateSrc.BetaGameServerHost;
            }
#else
            return ObsolateSrc.BetaGameServerHost;
#endif
        }
    }

    private static string _masterHost = null;

    /// <summary>
    /// Host to be connected to
    /// </summary>
    public static string ConnectHost {
        get; set;
    }

#region フィールド&プロパティ
	/// <summary>
	/// ネットワーク専用
	/// </summary>
	public static NetworkParam Net = new NetworkParam();
	[System.Serializable]
	public class NetworkParam
	{
		public NetworkParam Clone() { return (NetworkParam)MemberwiseClone(); }

		/// <summary>
		/// ユーザーアカウント
		/// </summary>
		[SerializeField]
		private string userName;
		public string UserName { get { return userName; } set { userName = value; } }
	}

	/// <summary>
	/// 共通
	/// </summary>
	public static CommonParam Common = new CommonParam();
	[System.Serializable]
	public class CommonParam
	{
		public CommonParam Clone()
		{
			var t = (CommonParam)MemberwiseClone();
			if (this.ChatLog != null)
				t._chatLog = this.ChatLog.Clone();
			if (this.ChatPopupQueue != null)
				t._chatPopupQueue = this.ChatPopupQueue.Clone();
			return t;
		}

		/// <summary>
		/// チャットログ
		/// </summary>
		[SerializeField]
		ChatLog _chatLog = new ChatLog();
		public ChatLog ChatLog { get { return _chatLog; } }
		/// <summary>
		/// チャットキュー
		/// </summary>
		[SerializeField]
		ChatPopupQueue _chatPopupQueue = new ChatPopupQueue();
		public ChatPopupQueue ChatPopupQueue { get { return _chatPopupQueue; } }
		/// <summary>
		/// チャットタイプ
		/// </summary>
		[SerializeField]
		ChatType _chatType = ChatType.Say;
		public ChatType ChatType { get { return _chatType; } set { _chatType = value; } }
		/// <summary>
		/// 現在のエリア
		/// </summary>
		[SerializeField]
		AreaType _areaType = AreaType.Lobby;
		public AreaType AreaType { get { return _areaType; } set { _areaType = value; } }
	}

	/// <summary>
	/// ロビー専用
	/// </summary>
	public static LobbyParam Lobby = new LobbyParam();
	[System.Serializable]
	public class LobbyParam
	{
		public LobbyParam Clone() { return (LobbyParam)MemberwiseClone(); }

		/// <summary>
		/// 現在のロビー
		/// </summary>
		[SerializeField]
		private LobbyType _lobbyType;
		public LobbyType LobbyType
		{
			get { return _lobbyType; }
			set
			{
				_lobbyType = value;
				// UI更新
				GUILobbyResident.SetLobbyNo((int)value);
			}
		}

		/// <summary>
		/// キャラアイコン
		/// </summary>
		[SerializeField]
		private CharaIcon _charaIcon = new CharaIcon();
		public CharaIcon CharaIcon { get { return _charaIcon; } }

		/// <summary>
		/// キャラボード
		/// </summary>
		[SerializeField]
		private CharaBoard _charaBoard = new CharaBoard();
		public CharaBoard CharaBoard { get { return _charaBoard; } }

		/// <summary>
		/// アイテムアイコン
		/// </summary>
		[SerializeField]
		private ItemIcon _itemIcon = new ItemIcon();
		public ItemIcon ItemIcon { get { return _itemIcon; } }

		/// <summary>
		/// 共通アイコン
		/// </summary>
		[SerializeField]
		private CommonIcon _commonIcon = new CommonIcon();
		public CommonIcon CommonIcon { get { return _commonIcon; } }
	}

	/// <summary>
	/// バトル専用
	/// </summary>
	public static BattleParam Battle = new BattleParam();
	[System.Serializable]
	public class BattleParam
	{
		public BattleParam Clone() { return (BattleParam)MemberwiseClone(); }

		/// <summary>
		/// バトルフィールドタイプ
		/// </summary>
		[SerializeField]
		private BattleFieldType battleFieldType;
		public BattleFieldType BattleFieldType { get { return battleFieldType; } set { battleFieldType = value; } }

        [SerializeField]
        private ScoreType scoreType;
        public ScoreType ScoreType { get { return scoreType; } set { scoreType = value; } }

		/// <summary>
		/// キャラアイコン
		/// </summary>
		[SerializeField]
		private CharaIcon _charaIcon = new CharaIcon();
		public CharaIcon CharaIcon { get { return _charaIcon; } }

		/// <summary>
		/// キャラボード
		/// </summary>
		[SerializeField]
		private CharaBoard _charaBoard = new CharaBoard();
		public CharaBoard CharaBoard { get { return _charaBoard; } }

		/// <summary>
		/// スキルアイコン
		/// </summary>
		[SerializeField]
		private SkillIcon _skillIcon = new SkillIcon();
		public SkillIcon SkillIcon { get { return _skillIcon; } }
	}

    public static bool SelectBattleField {
        get {
#if XW_DEBUG
            return Debug.File.SelectBattleField;
#else
            return _selectBattleField;
#endif
        }
        set {
#if XW_DEBUG
            Debug.File.SelectBattleField = value;
#else
            _selectBattleField = value;
#endif
        }
    }

    private static bool _selectBattleField = false;

#if XW_DEBUG
	/// <summary>
	/// デバッグ専用
	/// </summary>
	public static DebugParam Debug = new DebugParam();
	[System.Serializable]
	public class DebugParam
	{
		public DebugParam Clone()
		{
			var t = (DebugParam)MemberwiseClone();
			if (this.File != null)
				t.File = this.File.Clone();
			return t;
		}

		[SerializeField]
		private DebugFile.Config file = new DebugFile.Config();
		public DebugFile.Config File { get { return file; } set { file = value; } }
	}
#endif
#endregion
}
