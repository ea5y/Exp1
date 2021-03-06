//
// 
// 2014/05/23
//
using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

/// <summary>
/// 拡張メソッドのうち,Scm独自クラスを使用するもの.
/// </summary>
public static class StaticClassScm
{
	#region UIPlayTween 拡張メソッド
	/// <summary>
	/// UIPlayTween 内で取得している UITweener と同じものに設定をする
	/// </summary>
	public static void SetTweener(this UIPlayTween pt, System.Action<UITweener> action)
	{
		if (pt == null)
			return;

		// PlayTween.Play 内で取得している UITweener と同じものに設定をする
		var go = (pt.tweenTarget == null) ? pt.gameObject : pt.tweenTarget;
		var tweens = pt.includeChildren ? go.GetComponentsInChildren<UITweener>() : go.GetComponents<UITweener>();
		foreach (var tw in tweens)
		{
			if (tw.tweenGroup != pt.tweenGroup)
				continue;
			action(tw);
		}
	}
	#endregion

	#region TeamType JudgeType 拡張メソッド
	/// <summary>
	/// サーバ上でのチームをクライアント上でのチームに変換する(味方は常に青).
	/// </summary>
	public static TeamTypeClient GetClientTeam(this TeamType teamType)
	{
		try
		{
			// 中立同士は味方ではない.
			if(teamType == TeamType.Unknown)
			{
				return TeamTypeClient.Unknown;
			}

			// PlayerManager のインスタンスがない
			if (PlayerManager.Instance == null)
				return TeamTypeClient.Unknown;

			TeamType playerTeamType = PlayerManager.Instance.PlayerTeamType;
			if(teamType == playerTeamType)
			{
				return TeamTypeClient.Friend;
			}
			else
			{
				return TeamTypeClient.Enemy;
			}
		}
		catch(System.Exception e)
		{
			BugReportController.SaveLogFileWithOutStackTrace(e.ToString());
#if UNITY_EDITOR
			Debug.LogException(e);
#endif
			return TeamTypeClient.Unknown;
		}
	}

	/// <summary>
	/// サーバ上でのJudgeをクライアント上でのJudgeに変換する(プレイヤーが勝ったか負けたか).
	/// </summary>
	public static JudgeTypeClient GetClientJudge(this JudgeType judgeType)
	{
		JudgeTypeClient ret = JudgeTypeClient.Unknown;

		try
		{
			TeamType playerTeam = PlayerManager.Instance.PlayerTeamType;
			switch(judgeType)
			{
				case JudgeType.WinnerBlue:
					if(playerTeam == TeamType.Blue)
					{
						ret = JudgeTypeClient.PlayerWin;
					}
					else if(playerTeam == TeamType.Red)
					{
						ret = JudgeTypeClient.PlayerLose;
					}
					break;
				case JudgeType.WinnerRed:
					if(playerTeam == TeamType.Blue)
					{
						ret = JudgeTypeClient.PlayerLose;
					}
					else if(playerTeam == TeamType.Red)
					{
						ret = JudgeTypeClient.PlayerWin;
					}
					break;
				case JudgeType.Draw:
					ret = JudgeTypeClient.Draw;
					break;
				case JudgeType.CompleteWinnerBlue:
					if(playerTeam == TeamType.Blue)
					{
						ret = JudgeTypeClient.PlayerCompleteWin;
					}
					else if(playerTeam == TeamType.Red)
					{
						ret = JudgeTypeClient.PlayerCompleteLose;
					}
					break;
				case JudgeType.CompleteWinnerRed:
					if(playerTeam == TeamType.Blue)
					{
						ret = JudgeTypeClient.PlayerCompleteLose;
					}
					else if(playerTeam == TeamType.Red)
					{
						ret = JudgeTypeClient.PlayerCompleteWin;
					}
					break;
			}
		}
		catch(System.Exception e)
		{
			BugReportController.SaveLogFileWithOutStackTrace(e.ToString());
#if UNITY_EDITOR
			Debug.LogException(e);
#endif
		}

		return ret;
	}
	#endregion

	#region EntrantType 拡張メソッド
	/// <summary>
	/// EntrantTypeごとのターゲットロックオン優先順位.
	/// </summary>
	public static int GetTargetPriority(this EntrantType entrantType)
	{
		// 特定オブジェクト
		switch(entrantType)
		{
			// ロックオン不可能.
			case EntrantType.Unknown:
			case EntrantType.Item:
			case EntrantType.Wall:
			case EntrantType.Start:
			case EntrantType.Respawn:
			case EntrantType.Jump:
			case EntrantType.FieldPortal:
			case EntrantType.RankingPortal:
			case EntrantType.Barrier:
			case EntrantType.BulletObj:
			case EntrantType.Transporter:
				return -1;
			// 優先順位低い
			case EntrantType.Tank:
			case EntrantType.MiniNpc:
			case EntrantType.Mob:
				return 5;
			// デフォルト.
			case EntrantType.Pc:
			case EntrantType.MainTower:
			case EntrantType.SubTower:
			case EntrantType.Npc:
            case EntrantType.Hostage:
			default:
				return 10;
		}
	}
	#endregion
}
