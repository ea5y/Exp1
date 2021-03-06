/// <summary>
/// GMコマンド
/// 
/// 2014/05/28
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.Master;
using Scm.Common.GameParameter;

public static class GMCommand
{
	/// <summary>
	/// GMコマンドかどうか
	/// </summary>
	/// <param name="text"></param>
	/// <returns></returns>
	public static bool IsGMCommand(string text)
	{
		if (string.IsNullOrEmpty(text)) return false;
		return text.StartsWith("@");
	}

#if XW_DEBUG || ENABLE_GM_COMMAND
	#region 宣言
	public enum Type
	{
		// 単発系
		h,
		bf,
		bf_a,
		lb,
		lb_a,
		re,
		re_a,
		gc,
		gc_a,
		lg,
		lg_a,
		mt,
		mt_a,
		sk,
		tk,
		af,
		ts,
		dkc,
		hitroot,
		hitroot_a,
		// 持続系
		skillmode,
		skillchangemode,
        w,
        sbf,
		Max,
	}

	/// <summary>
	/// @h時に表示する文字列
	/// </summary>
	static readonly List<string> argList = new List<string>(
		new string[(int)Type.Max]
		{
			// 単発系
			"@h HelpCommand",
			"@bf BattleFieldCommand [1]=BattleFieldID [2]=Timer",
			"@bf_a 〃",
			"@lb LobbyCommand [1]=LobbyNo [2]=Timer",
			"@lb_a 〃",
			"@re ResultCommand [1]=Timer",
			"@re_a 〃",
			"@gc GMChatCommand [1]=Message",
			"@gc_a 〃",
			"@lg LogoutCommand [1]=Timer",
			"@lg_a 〃",
			"@mt MatchingCommand [1]=Timer",
			"@mt_a 〃",
			"@sk SkillCommand [1]=SkillID [2]=SkillID...",
			"@tk TeamKickCommand [1]=memberNum",
			"@af AudienceFieldCommand [1]=BattleFieldID [2]=Timer",
			"@ts TimeScaleCommand [1]=TimeScale",
			"@dkc DebugKeyCommand Script Additive",
			"@hitroot HitRootCheckCommand [1]=ONOFF(0=OFF, 1=ON)",
			"@hitroot_a 〃",
			// 持続系
			"@skillmode [1]=Timer",
			"@skillchangemode [1]=Timer",
            "@sbf [1]=ONOFF(0=OFF, 1=ON)",
            "@w Where 〃",
        }
	);

	/// <summary>
	/// テキストから分割する文字リスト
	/// </summary>
	static readonly char[] TextSplitChars = new char[] { ',', ' ' };

	/// <summary>
	/// 登録されているコマンドディクショナリ
	/// </summary>
	public class Command
	{
		public Type type;
		public string command;
		public string argument;
	}
	static Dictionary<string, Command> _commandDict;
	static Dictionary<string, Command> CommandDict
	{
		get
		{
			if (_commandDict == null)
			{
				_commandDict = new Dictionary<string, Command>();
				for (int i = 0, max = (int)Type.Max; i < max; i++)
				{
					string[] split = argList[i].Split(new char[] { ' ' }, 2);
					var t = new Command()
					{
						type = (Type)i,
						command = string.Format("@{0}", (Type)i),
						argument = split.Length >= 2 ? split[1] : "",
					};
					_commandDict.Add(t.command, t);
				}
			}
			return _commandDict;
		}
	}

	/// <summary>
	/// ファイバーセット
	/// </summary>
	static FiberSet _fiberSet;
	static FiberSet FiberSet
	{
		get
		{
			if (_fiberSet == null)
			{
				_fiberSet = new FiberSet();
				FiberController.AddFiber(CommandCoroutine());
			}
			return _fiberSet;
		}
	}
	static IEnumerator CommandCoroutine()
	{
		while (FiberSet.Update())
			yield return null;
		_fiberSet = null;
	}

	/// <summary>
	/// 持続系コルーチンの中断フラグ
	/// </summary>
	static bool _isQuit;
	static bool IsQuit { get { return _isQuit; } set { _isQuit = value; } }

	/// <summary>
	/// 入力中の文字列
	/// </summary>
	static string _inputString = "";
	public static string InputString { get { return _inputString; } }
	#endregion

	#region GMコマンド
	public static bool TryParse(string text, out Command command, out List<string> argList)
	{
		// コマンドと引数を切り分ける
		string[] split = text.Split(TextSplitChars, 2);
		string key = split[0];
		if (split.Length >= 2)
			argList = new List<string>(split[1].Split(TextSplitChars));
		else
			argList = new List<string>();
		if (!CommandDict.TryGetValue(key, out command))
			return false;
		return true;
	}
	public static bool CommandSelf(string text)
	{
		// テキストからコマンドと引数部分を切り離す
		Command command;
		List<string> argList;
		if (TryParse(text, out command, out argList))
		{
			// argList.Count チェックを全てここでやると使うとき便利かも
			string arg1 = argList.Count > 0 ? argList[0] : "";

			switch (command.type)
			{
			case Type.h:
				GUIChat.AddSystemMessage(false, "クライアント主導GMコマンド一覧");
				foreach (var c in CommandDict)
					GUIChat.AddSystemMessage(false, string.Format("{0} {1}", c.Key, c.Value.argument));
				break;
			case Type.bf:
				return CommandAll(Type.bf_a, argList);
			case Type.lb:
				return CommandAll(Type.lb_a, argList);
			case Type.re:
				return CommandAll(Type.re_a, argList);
			case Type.gc:
				return CommandAll(Type.gc_a, argList);
			case Type.lg:
				return CommandAll(Type.lg_a, argList);
			case Type.mt:
				return CommandAll(Type.mt_a, argList);
			case Type.sk:
				return CommandSkill(argList);
			case Type.tk:
				return CommandTeamKick(argList);
			case Type.af:
				return CommandAll(Type.af, argList);
			case Type.ts:
			{
				float scale;
				if (string.IsNullOrEmpty(arg1))
				{
					scale = 1f;
				}
				else
				{
					float.TryParse(arg1, out scale);
				}
				Time.timeScale = scale;
				GUIChat.AddSystemMessage(false, string.Format("TimeScale = {0:0.00}", scale));
				break;
			}
			case Type.dkc:
			{
				var com = Object.FindObjectOfType(typeof(DebugKeyCommand)) as DebugKeyCommand;
				if (com == null)
				{
					var debugMain = Object.FindObjectOfType(typeof(DebugMain)) as DebugMain;
					if (debugMain != null)
					{
						debugMain.gameObject.AddComponent<DebugKeyCommand>();
						GUIChat.AddSystemMessage(false, string.Format("DebugKeyCommand Additive!"));
					}
					else
					{
						GUIChat.AddSystemMessage(false, string.Format("DebugKeyCommand "));
					}
				}
				else
				{
					GUIChat.AddSystemMessage(false, string.Format("DebugKeyCommand Exist!"));
				}
				break;
			}
			case Type.hitroot:
				return CommandAll(Type.hitroot_a, argList);
			case Type.skillmode:
				if (BattleMain.Instance != null)
				{
					float timer;
					if (!float.TryParse(arg1, out timer))
						timer = 60f;
					BattleMain.Instance.StartCoroutine(SkillModeCoroutine(timer));
				}
				return true;
			case Type.skillchangemode:
				if (BattleMain.Instance != null)
				{
					float timer;
					if (!float.TryParse(arg1, out timer))
						timer = 60f;
					BattleMain.Instance.StartCoroutine(SkillChangeModeCoroutine(timer));
				}
				return true;
            case Type.w: {
                    Player player = GameController.GetPlayer();
                    if (player != null) {
                        GUIChat.AddSystemMessage(false, player.Position.ToString());
                    }
                }
                return true;
            case Type.sbf: {
                    int value;
                    if (!int.TryParse(arg1, out value)) {
                        value = 0;
                    }
                    ScmParam.SelectBattleField = value != 0;
                }
                return true;
			default:
				break;
			}
		}
		else
		{
			if (IsGMCommand(text))
			{
				CommonPacket.SendGmCommand(text.Substring(1));
				return true;
			}
		}
		return false;
	}
	public static bool CommandAll(string text)
	{
		Command command;
		List<string> argList;
		if (!TryParse(text, out command, out argList))
			return false;
		return CommandAll(command.type, argList);
	}
	public static bool CommandAll(Type type, List<string> argList)
	{
		// argList.Count チェックを全てここでやると使うとき便利かも
		string arg1 = argList.Count > 0 ? argList[0] : "";
		string arg2 = argList.Count > 1 ? argList[1] : "";
        string arg3 = argList.Count > 2 ? argList[2] : "";

		switch (type)
		{
		case Type.bf_a:
			if (LobbyMain.Instance != null)
			{
				int no;
				int.TryParse(arg1, out no);
				float timer;
				float.TryParse(arg2, out timer);
                int scoreType;
                int.TryParse(arg3, out scoreType);
				LobbyMain.Instance.StartCoroutine(BattleFieldChangeCoroutine(timer, no, (ScoreType)scoreType, false));
			}
			break;
		case Type.lb_a:
			if (LobbyMain.Instance != null)
			{
				int no;
				int.TryParse(arg1, out no);
				float timer;
				float.TryParse(arg2, out timer);
				LobbyMain.Instance.StartCoroutine(LobbyChangeCoroutine(timer, no));
			}
			break;
		case Type.re_a:
			if (BattleMain.Instance != null)
			{
				float timer;
				float.TryParse(arg1, out timer);
				BattleMain.Instance.StartCoroutine(ResultCoroutine(timer));
			}
			break;
		case Type.gc_a:
			GUIChat.AddMessage(
				new ChatInfo()
				{
					chatType = ChatType.AdminShout,
					name = "管理者",
					playerID = 0,
					text = arg1,
				}
			);
			break;
		case Type.lg_a:
			{
				float timer;
				float.TryParse(arg1, out timer);
				FiberSet.AddFiber(LogoutCoroutine(timer));
				break;
			}
		case Type.mt_a:
			{
				int bf;
				int.TryParse(arg1, out bf);
				Matching(bf);
				break;
			}
		case Type.af:
			if (LobbyMain.Instance != null)
			{
				int no;
				int.TryParse(arg1, out no);
				float timer;
				float.TryParse(arg2, out timer);
                int scoreType;
                int.TryParse(arg3, out scoreType);
				LobbyMain.Instance.StartCoroutine(BattleFieldChangeCoroutine(timer, no, (ScoreType)scoreType, true));
			}
			break;
		case Type.hitroot_a:
			{
				int no;
				int.TryParse(arg1, out no);
				ObjectCollider.IsCheckHitRoot = (no != 0);
				GUIChat.AddSystemMessage(false, string.Format("HitRootCheck is {0}", ObjectCollider.IsCheckHitRoot ? "ON" : "OFF"));
			}
			break;
		default:
			return true;
		}

		return true;
	}
	#endregion

	#region @mt
	static void Matching(int battleFieldNo)
	{
		if (LobbyMain.Instance == null)
			return;

		BattleFieldMasterData bf;
		if (MasterData.TryGetBattleField((int)battleFieldNo, out bf))
		{
			GUIChat.AddSystemMessage(false, string.Format("[GMコマンド]【{0}】にマッチングを開始します", bf.Name));
			LobbyPacket.SendMatchingEntry((BattleFieldType)battleFieldNo, Scm.Common.GameParameter.ScoreType.QuickMatching);
		}
		else
		{
			GUIChat.AddSystemMessage(false, string.Format("[GMコマンド]マッチングをキャンセルします"));
			LobbyPacket.SendMatchingCancel();
		}
	}
	#endregion

	#region @bf
	static IEnumerator BattleFieldChangeCoroutine(float timer, int battleFieldNo, ScoreType scoreType, bool isAudiences)
	{
		if (battleFieldNo == 0 || battleFieldNo == int.MaxValue)
		{
			battleFieldNo = int.MaxValue;
			GUIChat.AddSystemMessage(false, string.Format("【デバッグフィールド】に移動します"));
		}
		else
		{
			BattleFieldMasterData bf;
			if (!MasterData.TryGetBattleField(battleFieldNo, out bf))
				yield break;
			GUIChat.AddSystemMessage(false, string.Format("【{0}】に移動します", bf.Name));
		}

		int count = (int)timer;
		while (timer > 0f)
		{
			timer -= Time.deltaTime;
			int c = (int)timer;
			if (count != c)
			{
				GUIChat.AddSystemMessage(false, count.ToString());
				count = c;
			}
			yield return null;
		}

		// 全てのコルーチンを停止
		FiberSet.Clear();
		// 変更
		ScmParam.Battle.BattleFieldType = (BattleFieldType)battleFieldNo;
        ScmParam.Battle.ScoreType = scoreType;
		if (isAudiences)
		{
			BattlePacket.SendEnterFieldAudiences((BattleFieldType)battleFieldNo, scoreType);
		}
		else
		{
			LobbyMain.NextScene_Battle();
		}
	}
	#endregion

	#region @lb
	static IEnumerator LobbyChangeCoroutine(float timer, int lobbyNo)
	{
		GUIChat.AddSystemMessage(false, string.Format("【{0}】に移動します", (LobbyType)lobbyNo));

		int count = (int)timer;
		while (timer > 0f)
		{
			timer -= Time.deltaTime;
			int c = (int)timer;
			if (count != c)
			{
				GUIChat.AddSystemMessage(false, count.ToString());
				count = c;
			}
			yield return 0;
		}

		// 全てのコルーチンを停止
		FiberSet.Clear();
		// 変更
		LobbyMain.LobbyChange(lobbyNo);
	}
	#endregion

	#region @re
	static IEnumerator ResultCoroutine(float timer)
	{
		GUIChat.AddSystemMessage(false, "リザルトに移動します");
		Debug.Log("リザルトに移動します");

		int count = (int)timer;
		while (timer > 0f)
		{
			timer -= Time.deltaTime;
			int c = (int)timer;
			if (count != c)
			{
				GUIChat.AddSystemMessage(false, count.ToString());
				count = c;
			}
			yield return null;
		}

		// 全てのコルーチンを停止
		FiberSet.Clear();
		// リザルトへ
		BattleMain.Instance.OnResult();
	}
	#endregion

	#region @lg
	static IEnumerator LogoutCoroutine(float timer)
	{
		GUIChat.AddSystemMessage(false, "ログアウトします");
		Debug.Log("ログアウトします");

		int count = (int)timer;
		while (timer > 0f)
		{
			timer -= Time.deltaTime;
			int c = (int)timer;
			if (count != c)
			{
				GUIChat.AddSystemMessage(false, count.ToString());
				count = c;
			}
			yield return null;
		}

		// 全てのコルーチンを停止
		FiberSet.Clear();
		// ログアウト
		TitleMain.LoadScene();
	}
	#endregion

	#region @sk
	static bool CommandSkill(List<string> argList)
	{
		var p = GameController.GetPlayer();
		if (p == null)
			return false;

		foreach (string s in argList)
		{
			int id;
			if (int.TryParse(s, out id))
			{
				SkillAttack(p, id, false, null);
			}
		}
		return true;
	}
	static bool SkillAttack(Player p, int id, bool isSameNumber, Vector3? targetPosition)
	{
		SkillMasterData skill;
		if (!MasterData.TryGetSkill(id, out skill))
		{
			GUIChat.AddSystemMessage(false, string.Format("ID【{0}】のスキルはない", id));
			return false;
		}
		else
		{
			if (!isSameNumber)
				GUIChat.AddSystemMessage(false, skill.Name);
			var fiberList = p.SetBulletFiber(skill, GUIObjectUI.LockonObject, targetPosition);
			foreach (var t in fiberList)
				FiberSet.AddFiber(t);
			return true;
		}
	}
	#endregion

	#region @tk
	/// <summary>
	/// 試遊会用.SendTeamRemoveMember代用.
	/// </summary>
	static bool CommandTeamKick(List<string> argList)
	{
		if (NetworkController.ServerValue == null ||
		   !NetworkController.ServerValue.IsJoinedTeam)
		{
			GUIChat.AddSystemMessage(false, "チームに入っていません");
			return true;
		}

		long targetPlayerId;
		if (argList.Count < 1)
		{
			targetPlayerId = NetworkController.ServerValue.PlayerId;
		}
		else
		{
			int memberNum;
			if (!int.TryParse(argList[0], out memberNum))
			{
				GUIChat.AddSystemMessage(false, string.Format("{0}を数値に変換できません", argList[0]));
				return true;
			}
			else if (memberNum < NetworkController.ServerValue.Members.Count)
			{
				targetPlayerId = NetworkController.ServerValue.Members[memberNum].PlayerId;
			}
			else
			{
				GUIChat.AddSystemMessage(false, string.Format("{0}番目のメンバーはいません", argList[0]));
				return true;
			}
		}
#if OLD_TEAM_LOGIC
        CommonPacket.SendTeamRemoveMember(targetPlayerId);
#endif
		return true;
	}
#endregion

	#region @skillmode
	static IEnumerator SkillModeCoroutine(float timer)
	{
		var p = GameController.GetPlayer();
		if (p == null)
			yield break;

		// 既に持続系コルーチンが回っている場合は中断させる
		// 1フレーム遅らせて他のコルーチンの終了処理待ちをする
		IsQuit = true;
		yield return null;
		yield return null;
		IsQuit = false;

		GUIChat.AddSystemMessage(false, string.Format("スキルモード開始\r\n{0}秒間", timer));
		GUIChat.AddSystemMessage(false, string.Format("Escape=中止 F1=残り時間表示 Enter=スキル発動"));

		IEnumerator fiber = ModeCoroutine(timer, (int number, bool isSameNumber) =>
			{
				return SkillAttack(GameController.GetPlayer(), number, isSameNumber, null);
			});
		while (fiber.MoveNext())
			yield return null;

		GUIChat.AddSystemMessage(false, "スキルモード終了");
	}
	#endregion

	#region @skillchangemode
	static IEnumerator SkillChangeModeCoroutine(float timer)
	{
		var p = GameController.GetPlayer();
		if (p == null)
			yield break;

		// 既に持続系コルーチンが回っている場合は中断させる
		// 1フレーム遅らせて他のコルーチンの終了処理待ちをする
		IsQuit = true;
		yield return null;
		yield return null;
		IsQuit = false;

		GUIChat.AddSystemMessage(false, string.Format("スキル内容変更モード開始\r\n{0}秒間", timer));
		GUIChat.AddSystemMessage(false, string.Format("Escape=中止 F1=残り時間表示 Enter=変更"));

		IEnumerator fiber = ModeCoroutine(timer, (int number, bool isSameNumber) =>
			{
				var avatarType = (AvatarType)number;
				CharaMasterData chara;
				if (!MasterData.TryGetChara(number, out chara))
				{
					GUIChat.AddSystemMessage(false, string.Format("ID【{0}】のキャラは居ない", number));
					return false;
				}
				else
				{
					if (!isSameNumber)
						GUIChat.AddSystemMessage(false, string.Format("【{0}】にスキル内容を変更", chara.Name));

					p.EntrantInfo.Id = number;
					GUISkill.ChangeButtonName(avatarType, p.Level);
					return true;
				}
			});
		while (fiber.MoveNext())
			yield return null;

		GUIChat.AddSystemMessage(false, "スキル内容変更モード終了");
	}
	#endregion

	#region 持続系コルーチン
	static IEnumerator ModeCoroutine(float timer, System.Func<int, bool, bool> enterCallback)
	{
		bool isReset = false;

		while (timer > 0f)
		{
			timer -= Time.deltaTime;

			// 中断
			if (IsQuit)
				yield break;

			// 終了
			if (Input.GetKeyDown(KeyCode.Escape))
				break;

			// 残り時間表示
			if (Input.GetKeyDown(KeyCode.F1))
				GUIChat.AddSystemMessage(false, string.Format("残り時間 {0} 秒", timer));

			// 入力した数字をためていく
			if (!string.IsNullOrEmpty(Input.inputString))
			{
				int input;
				if (int.TryParse(Input.inputString, out input))
				{
					if (isReset)
					{
						isReset = false;
						_inputString = Input.inputString;
					}
					else
					{
						_inputString += Input.inputString;
					}
				}
			}

			// 決定
			if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
			{
				int number;
				if (int.TryParse(_inputString, out number))
				{
					if (!enterCallback(number, isReset))
						_inputString = "";
					isReset = true;
				}
				else
				{
					_inputString = "";
					isReset = true;
				}
			}
			yield return null;
		}
	}
	#endregion
#endif
}
