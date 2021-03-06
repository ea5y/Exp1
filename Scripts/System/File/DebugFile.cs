/// <summary>
/// デバッグファイル
/// 
/// 2014/05/27
/// </summary>
#if XW_DEBUG
using UnityEngine;
using System.Collections;

public class DebugFile : FileBase
{
	#region フィールド＆プロパティ
	public const string Path = "Work";
	public const string Filename = "debug.json";

	public static DebugFile Instance = new DebugFile();

    public DebugFile() {
        UnityEngine.Debug.Log("<color=#00ff00>create debug file</color>");
    }

	/// <summary>
	/// 設定
	/// </summary>
	[SerializeField]
	Config data = new Config();
	[System.Serializable]
	public class Config
	{
		public Config Clone()
		{
			var t = (Config)MemberwiseClone();
			if (this._debug != null)
				t._debug = this._debug.Clone();
			return t;
		}

		/// <summary>
		/// ファイルから読み込んだかどうか
		/// </summary>
		public bool IsFileLoad = false;

		/// <summary>
		/// デバッグモード
		/// </summary>
		public bool IsDebugMode = false;

		/// <summary>
		/// 全般、アソビモID
		/// </summary>
		public string AsobimoID
		{
			get { return (IsDebugMode ? _debug.AsobimoID : ""); }
			set { _debug.AsobimoID = value; }
		}

		/// <summary>
		/// 全般、接続先
		/// </summary>
		public string Host
		{
			get { return (IsDebugMode ? _debug.Host : ObsolateSrc.TestGameServerHost); }
			set { _debug.Host = value; }
		}

        private Scm.Common.GameParameter.EnvironmentType _environment = Scm.Common.GameParameter.EnvironmentType.Any;
        /// <summary>
        /// Environment (from host selection)
        /// </summary>
        public Scm.Common.GameParameter.EnvironmentType Environment {
            get {
                return _environment;
            }
            set {
                _environment = value;
            }
        }

		/// <summary>
		/// 全般、FPS表示
		/// </summary>
		public bool IsDrawFPS { get { return (IsDebugMode ? _debug.IsDrawFPS : false); } }

		/// <summary>
		/// 全般、コリジョン表示
		/// </summary>
		public bool IsDrawCollision { get { return (IsDebugMode ? _debug.IsDrawCollision : false); } }

		/// <summary>
		/// 全般、チュートリアルフラグ
		/// </summary>
		public bool IsTutorial { get { return (IsDebugMode ? _debug.IsTutorial : false); } }

		

		/// <summary>
		/// 全般、リソースのロードをアセットバンドルではなくローカルフォルダから行う.
		/// </summary>
		public bool LoadFromLocalFolder
		{
			get
			{
    			return _debug.LoadFromLocalFolder;
			}
		}

		/// <summary>
		/// 認証時、各項目を確認する
		/// </summary>
		public bool IsAuthCheck { get { return (IsDebugMode ? _debug.IsAuthCheck : false); } }

		/// <summary>
		/// バトル時、スキルを使用した時にキャラが向いている方向に放つ
		/// </summary>
		public bool IsFixedAttackRotation { get { return (IsDebugMode ? _debug.IsFixedAttackRotation : false); } }

		/// <summary>
		/// リザルト時 リザルトをスキップする
		/// </summary>
		public bool IsResultSkip { get { return (IsDebugMode ? _debug.IsResultSkip : false); } }

        /// <summary>
        /// When starts matching, display a dialog to select target battle field
        /// </summary>
        public bool SelectBattleField { get {
                return (IsDebugMode ? _debug.SelectBattleField : false);
            }
            set {
                _debug.SelectBattleField = value;
            }
        }

		/// <summary>
		/// 言語設定 1=>日本語 2=>英語 3=>繁体字 4=>簡体字
		/// </summary>
		public Scm.Common.GameParameter.Language Language {
            get {
                // Huhao
                //return (IsDebugMode ? _debug.Language : Scm.Common.GameParameter.Language.Japanese);
                return Scm.Common.GameParameter.Language.ChineseSimplified;
            }
        }

		/// <summary>
		/// デバッグモード有効時のパラメータ
		/// </summary>
		[SerializeField]
		Debug _debug = new Debug();
		[System.Serializable]
		public class Debug
		{
			public Debug Clone() { return (Debug)MemberwiseClone(); }

			public string AsobimoID = "";
			public string Host = ObsolateSrc.TestGameServerHost;
			public bool IsDrawFPS = true;
			public bool IsDrawCollision = true;
			public bool IsTutorial = false;
			public bool LoadFromLocalFolder = true;
			public bool IsAuthCheck = false;
			public bool IsFixedAttackRotation = false;
			public bool IsResultSkip = false;
            public bool SelectBattleField = false;
            public Scm.Common.GameParameter.Language Language = Scm.Common.GameParameter.Language.Unknown;

			public void Decode(JSONObject json)
			{
				json.GetField(ref this.AsobimoID, "AsobimoID");
				json.GetField(ref this.Host, "Host");
				json.GetField(ref this.IsDrawFPS, "IsDrawFPS");
				json.GetField(ref this.IsDrawCollision, "IsDrawCollision");
				json.GetField(ref this.IsTutorial, "IsTutorial");
				json.GetField(ref this.LoadFromLocalFolder, "LoadFromLocalFolder");
				json.GetField(ref this.IsAuthCheck, "IsAuthCheck");
				json.GetField(ref this.IsFixedAttackRotation, "IsFixedAttackRotation");
				json.GetField(ref this.IsResultSkip, "IsResultSkip");
                json.GetField(ref this.SelectBattleField, "SelectBattleField");
				int la = 0;
				json.GetField(ref la, "Language");
				if (la == 1)
				{
					this.Language = Scm.Common.GameParameter.Language.Japanese;
				}
				else if (la == 3)
				{
					this.Language = Scm.Common.GameParameter.Language.ChineseTraditional;
				}
				else if (la == 4)
				{
					this.Language = Scm.Common.GameParameter.Language.ChineseSimplified;
				}
				else
				{
					this.Language = Scm.Common.GameParameter.Language.English;
				}
                UnityEngine.Debug.Log("<color=#00ff00>Loaded debug file</color>");
            }
		}
		public void DebugDecode(JSONObject json) { _debug.Decode(json); }
	}
	public enum DownloadMode
	{
		Normal,
		ForceDownload,
		NotDownload,
	}
	#endregion

	#region IO
	public Config Clone()
	{
		return this.data.Clone();
	}
	public bool Read()
	{
		return this.Read(Path, Filename);
	}
	public void Write()
	{
		this.Write(Path, Filename);
	}
	protected override void Decode(string encodeString)
	{
		Decode(this.data, new JSONObject(encodeString));
		this.data.IsFileLoad = true;
	}
	protected override void Encode(out string encodeString)
	{
		Encode(this.data, new JSONObject(JSONObject.Type.OBJECT), out encodeString);
	}
	static void Decode(Config data, JSONObject json)
	{
		// フィールド読み込み
		json.GetField(ref data.IsDebugMode, "IsDebugMode");
		{
			data.DebugDecode(json.GetField("DebugMode"));
		}
	}
	static void Encode(Config data, JSONObject json, out string encodeString)
	{
		// 書き込みは今のところ考えていない
		encodeString = "";
	}
	#endregion
}
#endif
