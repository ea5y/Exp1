/// <summary>
/// アカウントファイル
/// 
/// 2013/09/04
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.NGWord;

public class ConfigFile : FileBase
{
	#region フィールド＆プロパティ
	public const string Path = "Work";
	public const string Filename = "config.json";

	public static ConfigFile Instance = new ConfigFile();
	public static Config.SystemParam System { get { return Instance.Data.System; } }
	public static Config.OptionParam Option { get { return Instance.Data.Option; } }

	[SerializeField]
	Config _data = new Config();
	public Config Data { get { return _data; } set { _data = value; } }
	#endregion

	#region 宣言
	[System.Serializable]
	public class Config
	{
		public Config Clone()
		{
			var t = (Config)MemberwiseClone();
			if (this.System != null)
				t.System = this.System.Clone();
			if (this.Option != null)
				t.Option = this.Option.Clone();
			return t;
		}

		public override bool Equals(object obj)
		{
			// objがnullか、型が違うときは、等価でない
			if (obj == null || this.GetType() != obj.GetType())
				return false;
			var t = (Config)obj;

			if (!this.System.Equals(t.System)) return false;
			if (!this.Option.Equals(t.Option)) return false;

			return true;
		}

		/// <summary>
		/// 通常Equalsメソッドをオーバーライドしたときは、GetHashCodeメソッドもオーバーライドします。
		/// GetHashCodeメソッドは、Hashtableなどのディクショナリコレクションで同じ値のキーを効率的に探すために使われます。
		/// 補足：「Equals() と演算子 == のオーバーロードに関するガイドライン (C# プログラミング ガイド)」では、
		/// Equalsメソッドをオーバーライドしたときは、GetHashCodeメソッドもオーバーライドすることを勧めるとされていますが、
		/// 「Equals および等値演算子 (==) 実装のガイドライン」では、必ずGetHashCodeも実装するとされています。
		/// どちらが正しいのかは分かりませんが、少なくともコレクションのキーとして使用するときは、
		/// GetHashCodeもオーバーライドしないと不具合が生じる可能性があるようです。
		/// </summary>
		public override int GetHashCode()
		{
			return this.System.GetHashCode() ^ this.Option.GetHashCode();
		}

		#region システム
		/// <summary>
		/// システム
		/// </summary>
		SystemParam _system = new SystemParam();
		public SystemParam System { get { return _system; } set { _system = value; } }
		[System.Serializable]
		public class SystemParam
		{
			const string Field = "System";

			public SystemParam Clone() { return (SystemParam)MemberwiseClone(); }

			public override bool Equals(object obj)
			{
				// objがnullか、型が違うときは、等価でない
				if (obj == null || this.GetType() != obj.GetType())
					return false;
				var t = (SystemParam)obj;

				if (!this.IsOpenLog.Equals(t.IsOpenLog)) return false;
				if (!this.LogPosX.Equals(t.LogPosX)) return false;
				if (!this.LogPosY.Equals(t.LogPosY)) return false;
				if (!this.LogWidth.Equals(t.LogWidth)) return false;
				if (!this.LogHeight.Equals(t.LogHeight)) return false;
				if (!this.AgeVerificationSkip.Equals(t.AgeVerificationSkip)) return false; 

				return true;
			}

			/// <summary>
			/// 通常Equalsメソッドをオーバーライドしたときは、GetHashCodeメソッドもオーバーライドします。
			/// GetHashCodeメソッドは、Hashtableなどのディクショナリコレクションで同じ値のキーを効率的に探すために使われます。
			/// 補足：「Equals() と演算子 == のオーバーロードに関するガイドライン (C# プログラミング ガイド)」では、
			/// Equalsメソッドをオーバーライドしたときは、GetHashCodeメソッドもオーバーライドすることを勧めるとされていますが、
			/// 「Equals および等値演算子 (==) 実装のガイドライン」では、必ずGetHashCodeも実装するとされています。
			/// どちらが正しいのかは分かりませんが、少なくともコレクションのキーとして使用するときは、
			/// GetHashCodeもオーバーライドしないと不具合が生じる可能性があるようです。
			/// </summary>
			public override int GetHashCode()
			{
				return
					this.IsOpenLog.GetHashCode()
					^ this.LogPosX.GetHashCode()
					^ this.LogPosY.GetHashCode()
					^ this.LogWidth.GetHashCode()
					^ this.LogHeight.GetHashCode()
					^ this.AgeVerificationSkip.GetHashCode()
					;
			}

			/// <summary>
			/// チャットログ開閉フラグ
			/// </summary>
			[SerializeField]
			bool _isOpenLog = false;
			public bool IsOpenLog { get { return _isOpenLog; } set { _isOpenLog = value; } }

			/// <summary>
			/// チャットログ位置
			/// </summary>
			[SerializeField]
			float _logPosX = -430f;
			public float LogPosX { get { return _logPosX; } set { _logPosX = value; } }
			[SerializeField]
			float _logPosY = 300f;
			public float LogPosY { get { return _logPosY; } set { _logPosY = value; } }

			/// <summary>
			/// チャットログサイズ
			/// </summary>
			[SerializeField]
			int _logWidth = 380;
			public int LogWidth { get { return _logWidth; } set { _logWidth = value; } }
			[SerializeField]
			int _logHeight = 380;
			public int LogHeight { get { return _logHeight; } set { _logHeight = value; } }

			// 年齢認証スキップフラグ
			bool _ageVerificationSkip = false;
			[SerializeField]
			public bool AgeVerificationSkip { get { return _ageVerificationSkip; } set { _ageVerificationSkip = value; } }

			/// <summary>
			/// デコード
			/// </summary>
			public void Decode(JSONObject parentJson)
			{
				if (parentJson == null)
					return;
				var json = parentJson.GetField(Field);
				if (json == null)
					return;
				// 読み込み
				//json.GetField(ref this._isOpenLog, "IsOpenLog", (name) => { ExceptionField(name); });
				//json.GetField(ref this._logPosX, "LogPosX", (name) => { ExceptionField(name); });
				//json.GetField(ref this._logPosY, "LogPosY", (name) => { ExceptionField(name); });
				//json.GetField(ref this._logWidth, "LogWidth", (name) => { ExceptionField(name); });
				//json.GetField(ref this._logHeight, "LogHeight", (name) => { ExceptionField(name); });
				json.GetField(ref this._ageVerificationSkip, "AgeVerificationSkip", (name) => { ExceptionField( name ); });
			}

			/// <summary>
			/// エンコード
			/// </summary>
			public void Encode(JSONObject parentJson)
			{
				if (parentJson == null)
					return;
				var json = new JSONObject(JSONObject.Type.OBJECT);
				// 書き込み
				//json.AddField("IsOpenLog", this._isOpenLog);
				//json.AddField("LogPosX", this._logPosX);
				//json.AddField("LogPosY", this._logPosY);
				//json.AddField("LogWidth", this._logWidth);
				//json.AddField("LogHeight", this._logHeight);
				json.AddField("AgeVerificationSkip", this._ageVerificationSkip);
				parentJson.AddField(Field, json);
			}
		}
		#endregion

		#region オプション
		/// <summary>
		/// オプション
		/// </summary>
		OptionParam _option = new OptionParam();
		public OptionParam Option { get { return _option; } set { _option = value; } }
		[System.Serializable]
		public class OptionParam
		{
			const string Field = "Option";

			public OptionParam Clone()
			{
				var t = (OptionParam)MemberwiseClone();
				if (this.ChatMacroList != null)
				{
					t.ChatMacroList = new List<ChatMacroInfo>();
					foreach (var info in this.ChatMacroList)
						t.ChatMacroList.Add(info.Clone());
				}
				return t;
			}

			public override bool Equals(object obj)
			{
				// objがnullか、型が違うときは、等価でない
				if (obj == null || this.GetType() != obj.GetType())
					return false;
				var t = (OptionParam)obj;

				if (!this.ChatPopupNumDB.Equals(t.ChatPopupNumDB)) return false;
				if (!this.ChatPopupTimerDB.Equals(t.ChatPopupTimerDB)) return false;
				if (!this.BgmDB.Equals(t.BgmDB)) return false;
				if (!this.SeDB.Equals(t.SeDB)) return false;
				if (!this.VoiceDB.Equals(t.VoiceDB)) return false;
				if (!this.IsMacroClose.Equals(t.IsMacroClose)) return false;
				if (!this.LockonRangeDB.Equals(t.LockonRangeDB)) return false;
				if (!this.MacroButtonColumnDB.Equals(t.MacroButtonColumnDB)) return false;

				if (!Object.ReferenceEquals(this.ChatMacroList, t.ChatMacroList))
				{
					if (this.ChatMacroList.Count != t.ChatMacroList.Count) return false;
					for (int i = 0, max = this.ChatMacroList.Count; i < max; i++)
					{
						if (!this.ChatMacroList[i].Equals(t.ChatMacroList[i]))
							return false;
					}
				}

				return true;
			}

			/// <summary>
			/// 通常Equalsメソッドをオーバーライドしたときは、GetHashCodeメソッドもオーバーライドします。
			/// GetHashCodeメソッドは、Hashtableなどのディクショナリコレクションで同じ値のキーを効率的に探すために使われます。
			/// 補足：「Equals() と演算子 == のオーバーロードに関するガイドライン (C# プログラミング ガイド)」では、
			/// Equalsメソッドをオーバーライドしたときは、GetHashCodeメソッドもオーバーライドすることを勧めるとされていますが、
			/// 「Equals および等値演算子 (==) 実装のガイドライン」では、必ずGetHashCodeも実装するとされています。
			/// どちらが正しいのかは分かりませんが、少なくともコレクションのキーとして使用するときは、
			/// GetHashCodeもオーバーライドしないと不具合が生じる可能性があるようです。
			/// </summary>
			public override int GetHashCode()
			{
				return
					this.ChatPopupNumDB.GetHashCode()
					^ this.ChatPopupTimerDB.GetHashCode()
					^ this.BgmDB.GetHashCode()
					^ this.SeDB.GetHashCode()
					^ this.VoiceDB.GetHashCode()
					^ this.IsMacroClose.GetHashCode()
					^ this.LockonRangeDB.GetHashCode()
					^ this.MacroButtonColumnDB.GetHashCode()
					^ this.ChatMacroList.GetHashCode()
					;
			}
			/// <summary>
			/// ポップアップの数
			/// </summary>
			int _chatPopupNumDB = ObsolateSrc.Option.ChatPopupNumDefault;
			public int ChatPopupNumDB { get { return _chatPopupNumDB; } set { _chatPopupNumDB = ObsolateSrc.Option.ChatPopupNumDict.ContainsKey(value) ? value : ObsolateSrc.Option.ChatPopupNumDefault; } }
			public int ChatPopupNum { get { return _chatPopupNumDB; } }
			public string ChatPopupNumStr { get { string s; return ObsolateSrc.Option.ChatPopupNumDict.TryGetValue(_chatPopupNumDB, out s) ? s : ""; } }

			/// <summary>
			/// ポップアップ表示時間
			/// </summary>
			int _chatPopupTimerDB = ObsolateSrc.Option.ChatPopupTimerDefault;
			public int ChatPopupTimerDB { get { return _chatPopupTimerDB; } set { _chatPopupTimerDB = Mathf.Clamp(value, ObsolateSrc.Option.ChatPopupTimerMin, ObsolateSrc.Option.ChatPopupTimerMax); } }
			public float ChatPopupTimer { get { return (float)(ChatPopupTimerDB * 0.1f); } set { ChatPopupTimerDB = (int)(value * 10); } }
			public float ChatPopupTimerMin { get { return (float)(ObsolateSrc.Option.ChatPopupTimerMin * 0.1f); } }
			public float ChatPopupTimerMax { get { return (float)(ObsolateSrc.Option.ChatPopupTimerMax * 0.1f); } }

			/// <summary>
			/// BGM
			/// </summary>
			int _bgmDB = ObsolateSrc.Option.BgmDefault;
			public int BgmDB { get { return _bgmDB; } set { _bgmDB = Mathf.Clamp(value, ObsolateSrc.Option.BgmMin, ObsolateSrc.Option.BgmMax); } }
			public float Bgm { get { return (float)(BgmDB * 0.01f); } set { BgmDB = (int)(value * 100); } }
			public float BgmMin { get { return (float)(ObsolateSrc.Option.BgmMin * 0.01f); } }
			public float BgmMax { get { return (float)(ObsolateSrc.Option.BgmMax * 0.01f); } }

			/// <summary>
			/// SE
			/// </summary>
			int _seDB = ObsolateSrc.Option.SeDefault;
			public int SeDB { get { return _seDB; } set { _seDB = Mathf.Clamp(value, ObsolateSrc.Option.SeMin, ObsolateSrc.Option.SeMax); } }
			public float Se { get { return (float)(SeDB * 0.01f); } set { SeDB = (int)(value * 100); } }
			public float SeMin { get { return (float)(ObsolateSrc.Option.SeMin * 0.01f); } }
			public float SeMax { get { return (float)(ObsolateSrc.Option.SeMax * 0.01f); } }

			/// <summary>
			/// BGM
			/// </summary>
			int _voiceDB = ObsolateSrc.Option.VoiceDefault;
			public int VoiceDB { get { return _voiceDB; } set { _voiceDB = Mathf.Clamp(value, ObsolateSrc.Option.VoiceMin, ObsolateSrc.Option.VoiceMax); } }
			public float Voice { get { return (float)(VoiceDB * 0.01f); } set { VoiceDB = (int)(value * 100); } }
			public float VoiceMin { get { return (float)(ObsolateSrc.Option.VoiceMin * 0.01f); } }
			public float VoiceMax { get { return (float)(ObsolateSrc.Option.VoiceMax * 0.01f); } }

			/// <summary>
			/// マクロ発言時にマクロを閉じるかどうか
			/// </summary>
			bool _isMacroClose = ObsolateSrc.Option.MacroCloseDefault;
			public bool IsMacroClose { get { return _isMacroClose; } set { _isMacroClose = value; } }

			/// <summary>
			/// ロックオン範囲
			/// </summary>
			int _lockonRangeDB = ObsolateSrc.Option.LockonRangeDefault;
			public int LockonRangeDB { get { return _lockonRangeDB; } set { _lockonRangeDB = ObsolateSrc.Option.LockonRangeDict.ContainsKey(value) ? value : ObsolateSrc.Option.LockonRangeDefault; } }
			public float LockonRange { get { return (float)_lockonRangeDB; } }
			public string LockonRangeStr { get { string s; return ObsolateSrc.Option.LockonRangeDict.TryGetValue(_lockonRangeDB, out s) ? s : ""; } }

			/// <summary>
			/// マクロボタンの1行の表示数
			/// </summary>
			int _macroButtonColumnDB = ObsolateSrc.Option.MacroButtonColumnDefault;
			public int MacroButtonColumnDB { get { return _macroButtonColumnDB; } set { _macroButtonColumnDB = ObsolateSrc.Option.MacroButtonColumnDict.ContainsKey(value) ? value : ObsolateSrc.Option.MacroButtonColumnDefault; } }
			public int MacroButtonColumn { get { return _macroButtonColumnDB; } }
			public string MacroButtonColumnStr { get { string s; return ObsolateSrc.Option.MacroButtonColumnDict.TryGetValue(_macroButtonColumnDB, out s) ? s : ""; } }

			/// <summary>
			/// チャットマクロリスト
			/// </summary>
			List<ChatMacroInfo> _chatMacroList = null;
			public List<ChatMacroInfo> ChatMacroList
			{
				get
				{
					if (_chatMacroList == null)
					{
						_chatMacroList = new List<ChatMacroInfo>();
						_chatMacroList.Add(new ChatMacroInfo("", MasterData.GetTextNonBugReport(TextType.TX505_Option_MacroDefault00), 0));
						_chatMacroList.Add(new ChatMacroInfo("", MasterData.GetTextNonBugReport(TextType.TX506_Option_MacroDefault01), 1));
						_chatMacroList.Add(new ChatMacroInfo("", MasterData.GetTextNonBugReport(TextType.TX507_Option_MacroDefault02), 2));
						_chatMacroList.Add(new ChatMacroInfo("", MasterData.GetTextNonBugReport(TextType.TX508_Option_MacroDefault03), 3));
						_chatMacroList.Add(new ChatMacroInfo("", MasterData.GetTextNonBugReport(TextType.TX509_Option_MacroDefault04), 4));
						_chatMacroList.Add(new ChatMacroInfo("", MasterData.GetTextNonBugReport(TextType.TX510_Option_MacroDefault05), 5));
						_chatMacroList.Add(new ChatMacroInfo("", MasterData.GetTextNonBugReport(TextType.TX511_Option_MacroDefault06), 6));
						_chatMacroList.Add(new ChatMacroInfo("", MasterData.GetTextNonBugReport(TextType.TX512_Option_MacroDefault07), 7));
					}
					return _chatMacroList;
				}
				private set
				{
					_chatMacroList = value;
				}
			}

			/// <summary>
			/// デコード
			/// </summary>
			public void Decode(JSONObject parentJson)
			{
				if (parentJson == null)
					return;
				var json = parentJson.GetField(Field);
				if (json == null)
					return;
				int n = 0;
				//bool b = false;
				// 読み込み
				//json.GetField(ref n, "ChatPopupNum", (name) => { ExceptionField(name); }); this.ChatPopupNumDB = n;
				//json.GetField(ref n, "ChatPopupTimer", (name) => { ExceptionField(name); }); this.ChatPopupTimerDB = n;
				json.GetField(ref n, "Bgm", (name) => { ExceptionField(name); }); this.BgmDB = n;
				json.GetField(ref n, "Se", (name) => { ExceptionField(name); }); this.SeDB = n;
				json.GetField(ref n, "Voice", (name) => { ExceptionField(name); }); this.VoiceDB = n;
				//json.GetField(ref b, "IsMacroClose", (name) => { ExceptionField(name); }); this.IsMacroClose = b;
				//json.GetField(ref n, "LockonRange", (name) => { ExceptionField(name); }); this.LockonRangeDB = n;
				//json.GetField(ref n, "MacroButtonColumn", (name) => { ExceptionField(name); }); this.MacroButtonColumnDB = n;
				// チャットマクロ
				{
					JSONObject jsonList = json.GetField("ChatMacro");
					if (jsonList == null)
						return;
					for (int i = 0, max = jsonList.Count; i < max; i++)
					{
						if (i >= this.ChatMacroList.Count)
							break;
						string buttonName = "", macro = "";
						var jsonMacro = jsonList.list[i];
						//jsonMacro.GetField(ref buttonName, "ButtonName", (name) => { ExceptionField(name); });
						jsonMacro.GetField(ref macro, "Macro", (name) => { ExceptionField(name); });
						this.ChatMacroList[i] = new ChatMacroInfo(NGWord.DeleteNGWord(buttonName), NGWord.DeleteNGWord(macro), i);
					}
				}
			}

			/// <summary>
			/// エンコード
			/// </summary>
			public void Encode(JSONObject parentJson)
			{
				if (parentJson == null)
					return;
				var json = new JSONObject(JSONObject.Type.OBJECT);
				// 書き込み
				//json.AddField("ChatPopupNum", this.ChatPopupNumDB);
				//json.AddField("ChatPopupTimer", this.ChatPopupTimerDB);
				json.AddField("Bgm", this.BgmDB);
				json.AddField("Se", this.SeDB);
				json.AddField("Voice", this.VoiceDB);
				//json.AddField("IsMacroClose", this.IsMacroClose);
				//json.AddField("LockonRange", this.LockonRangeDB);
				//json.AddField("MacroButtonColumn", this.MacroButtonColumnDB);
				// チャットマクロ
				{
					var jsonList = new JSONObject(JSONObject.Type.OBJECT);
					foreach (var info in this.ChatMacroList)
					{
						var jsonMacro = new JSONObject(JSONObject.Type.OBJECT);
						//jsonMacro.AddField("ButtonName", info.ButtonName);
						jsonMacro.AddField("Macro", info.Macro);
						jsonList.Add(jsonMacro);
					}
					json.AddField("ChatMacro", jsonList);
				}
				parentJson.AddField(Field, json);
			}
		}
		#endregion
	}
	#endregion

	#region IO
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
		Decode(this.Data, new JSONObject(encodeString));
	}
	protected override void Encode(out string encodeString)
	{
		Encode(this.Data, new JSONObject(JSONObject.Type.OBJECT), out encodeString);
	}
	public static void ExceptionField(string name)
	{
		throw new System.Exception(string.Format("Not Found Field \"{0}\"\r\n", name));
	}
	static void Decode(Config data, JSONObject json)
	{
		// フィールド読み込み
		data.System.Decode(json);
		data.Option.Decode(json);
	}
	static void Encode(Config data, JSONObject json, out string encodeString)
	{
		// フィールド追加
		data.System.Encode(json);
		data.Option.Encode(json);

		encodeString = json.print();
	}
	#endregion
}
