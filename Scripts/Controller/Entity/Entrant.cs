/// <summary>
/// EntrantInfo(EntrantInfoとObjectBase)を管理する静的クラス.
/// 
/// 2014/11/07
/// </summary>

/* 使い方
 * 
 * 1.通信パケットからEntrantInfoを作ったら【AddEntrant(EntrantInfo)】で登録する.
 *   これで通信→生成の間もTryGetEntrant()で取得が可能になる.
 * 
 * 2.GameObjectが生成出来たら【Replace(ObjectBase)】で登録する.
 *   これでEntrantInfo.GameObjectでGameObjectが帰るようになる.
 * 
 * 3.GameObjectが破棄されたら【RemoveEntrant(EntrantInfo)】で消去する.
 *   破壊演出がある場合,RemoveEntrant(isDestroy = false)を使うことでGameObjectを破棄せずにEntrantInfo.GameObjectから外せる.
 * 
 * EX)InfieldIDの被りは自動的に古い方を消去するようになっている.
 *    ExitFieldなどで強制的に消去する場合は【RemoveEntrant(int)】を使う
 */

using UnityEngine;
using System;
using System.Collections.Generic;
using Scm.Common.Packet;
using Scm.Common.GameParameter;

public static class Entrant
{
	#region フィールド.
	static private Dictionary<int, EntrantInfo> entrantDict = new Dictionary<int, EntrantInfo>();
    static private int _firstPlayerEntrant = -1;
    static private float _lastPlayerEntrantCalcTime = 0;
    static private Dictionary<int, RecruitmentParameter> recruitmentDict = new Dictionary<int, RecruitmentParameter>();
	#endregion

	#region staticメソッド.
	/// <summary>
	/// InFieldIdをキーに登録されたEntrantInfoを返す.
	/// </summary>
	static public bool TryGetEntrant(int inFieldId, out EntrantInfo entrant)
	{
		return entrantDict.TryGetValue(inFieldId, out entrant);
	}
	static public bool TryGetEntrant<T>(int inFieldId, out T entrant) where T : EntrantInfo
	{
		EntrantInfo ent;
		entrantDict.TryGetValue(inFieldId, out ent);
		entrant = ent as T;
		return entrant != null;
	}
	/// <summary>
	/// EntrantInfoをリストに追加する.
	/// </summary>
	static public void AddEntrant(EntrantInfo info)
	{
		if(info.CreateStatus != EntrantInfo.CreateState.Uncreated)
		{
			SaveInvalidIDInfoLog(info);
			return;
		}
		if(entrantDict.ContainsKey(info.InFieldId))
		{
			// InFieldIdが重複している.
			// 一旦出力しないようにする.次回はSendEntrantとセットで出力する.
			// ! 2015/04/14 今回は例えExitFieldが掛かっても情報だけは保持するようにするので重複して当然になった.
			//SaveConflictEntrantLog(entrantDict[info.InFieldId], info);
			RemoveEntrant(info.InFieldId, true);
		}
		entrantDict[info.InFieldId] = info;
	}
	/// <summary>
	/// InFieldIdをキーにEntrantInfoをリストから削除する.
	/// ExitFieldやID被りなど,IDしかわからない場合に使う.
	/// </summary>
	static public void RemoveEntrant(int inFieldId, bool isDestroy = true)
	{
		EntrantInfo entrant;
		if(entrantDict.TryGetValue(inFieldId, out entrant))
		{
			//entrantDict.Remove(entrant.InFieldId);
			entrant.Remove(isDestroy);
		}
        recruitmentDict.Remove(inFieldId);
	}
	/// <summary>
	/// EntrantInfoをリストから削除する.
	/// EntrantInfoそのものがある場合はできるだけこちらを使う.
	/// </summary>
	static public void RemoveEntrant(EntrantInfo entrant, bool isDestroy = true)
	{
		//if(Exists(entrant))
		if(entrant != null)
		{
			//entrantDict.Remove(entrant.InFieldId);
			entrant.Remove(isDestroy);
            recruitmentDict.Remove(entrant.InFieldId);
        }
	}
	/// <summary>
	/// EntrantInfoリストをクリアする.
	/// フィールド移動などの際に使う.
	/// </summary>
	static public void ClearEntrant()
	{
		foreach(var info in (new List<EntrantInfo>(entrantDict.Values)))
		{
			// GameObjectの破壊は各々のManagerに任せる.
			RemoveEntrant(info, false);
		}
		entrantDict.Clear();
        recruitmentDict.Clear();
    }
	/// <summary>
	/// EntrantInfoからObjectBaseを生成した際に実行する.
	/// </summary>
	static public bool Replace(ObjectBase objectBase)
	{
		EntrantInfo outEntrant;
		if(entrantDict.TryGetValue(objectBase.EntrantInfo.InFieldId, out outEntrant))
		{
			if(objectBase.EntrantInfo == outEntrant)
			{
				objectBase.EntrantInfo.Created(objectBase);
				//entrantDict[objectBase.EntrantInfo.InFieldId] = objectBase;
			}
		}
		return false;
	}
	/// <summary>
	/// InFieldIdが登録済みのモノかどうか.
	/// EntrantInfoそのものがある場合はExists()を使う.
	/// </summary>
	static public bool Contains(int inFieldId)
	{
		return entrantDict.ContainsKey(inFieldId);
	}
	/// <summary>
	/// EntrantInfoが現在登録されているかどうか.
	/// </summary>
	static public bool Exists(EntrantInfo entrant)
	{
		EntrantInfo outEntrant;
		if(entrantDict.TryGetValue(entrant.InFieldId, out outEntrant))
		{
			return (entrant == outEntrant) && entrant.CreateStatus != EntrantInfo.CreateState.Removed;
		}
		return false;
	}

    /// <summary>
    /// Test if an entrant is the first player entrant (non-bot)
    /// </summary>
    /// <param name="inFieldId"></param>
    /// <returns></returns>
    static public bool IsFirstPlayerEntrant(int inFieldId) {
        const float FIRST_PLAYER_CALC_INTERVAL = 1.0f;
        // FIXME: each player has 4 entrants, should we test for the first available entrant's infieldid?
        if (_firstPlayerEntrant == -1 || Time.fixedTime - _lastPlayerEntrantCalcTime >= FIRST_PLAYER_CALC_INTERVAL) {
            _lastPlayerEntrantCalcTime = Time.fixedTime;
            _firstPlayerEntrant = short.MaxValue;
            foreach (EntrantInfo entrant in entrantDict.Values) {
                if (entrant.EntrantType == EntrantType.Pc && (!entrant.NeedCalcInertia) && entrant.CreateStatus != EntrantInfo.CreateState.Removed/* && entrant.StatusType != StatusType.NotInArea*/) {
                    if (entrant.InFieldId < _firstPlayerEntrant) {
                        _firstPlayerEntrant = entrant.InFieldId;
                    }
                }
            }
        }
        return _firstPlayerEntrant == inFieldId;
    }

    #region Find.
    /// <summary>
    /// matchに一致するEntrantInfoを返す.
    /// </summary>
    static public EntrantInfo Find(Predicate<EntrantInfo> match)
	{
		foreach(var entrant in entrantDict)
		{
			if(match(entrant.Value)) { return entrant.Value; }
		}
		return null;
	}
	/// <summary>
	/// matchに一致するEntrantInfoを全て返す.
	/// </summary>
	static public List<EntrantInfo> FindAll(Predicate<EntrantInfo> match)
	{
		var list = new List<EntrantInfo>();
		foreach(var entrant in entrantDict)
		{
			if(match(entrant.Value)) { list.Add(entrant.Value); }
		}
		return list;
	}
	/// <summary>
	/// matchに一致するT : ObjectBaseを返す.
	/// </summary>
	static public T Find<T>(Predicate<T> match) where T : ObjectBase
	{
		foreach(var entrant in entrantDict)
		{
			T t = entrant.Value as T;
			if(t != null)
			{
				if(match(t)) { return t; }
			}
		}
		return null;
	}
	/// <summary>
	/// matchに一致するT : ObjectBaseを全て返す.
	/// </summary>
	static public List<T> FindAll<T>(Predicate<T> match) where T : ObjectBase
	{
		var list = new List<T>();
		foreach(var entrant in entrantDict)
		{
			T t = entrant.Value.GameObject as T;
			if(t != null)
			{
				if(match(t)) { list.Add(t); }
			}
		}
		return list;
	}

    static public RecruitmentParameter GetRecruitment(int inFieldId) {
        RecruitmentParameter result;
        recruitmentDict.TryGetValue(inFieldId, out result);
        return result;
    }

    static public void UpdateRecruitment(RecruitmentParameter recruitment) {
        recruitmentDict[recruitment.InFieldId] = recruitment;
        UpdateOUIRecruitment(recruitment.InFieldId);
    }

    static public void RemoveRecruitment(RecruitmentParameter recruitment) {
        recruitmentDict.Remove(recruitment.InFieldId);
        UpdateOUIRecruitment(recruitment.InFieldId);
    }

    private static void UpdateOUIRecruitment(short inFieldId) {
        EntrantInfo entrant;
        Debug.Log("<color=#00ff00> update oui rec:" + inFieldId + "</color>");
        if (TryGetEntrant(inFieldId, out entrant) && entrant.GameObject != null) {
            Debug.Log("<color=#00ff00> found entrant:" + entrant.GameObject + "</color>");
            Character chara = entrant.GameObject as Character;
            if (chara == null) {
                Debug.Log("<color=#00ff00> chara is null</color>");
            }
            if (chara != null && chara.ObjectUIRoot == null) {
                Debug.Log("<color=#00ff00> chara.ObjectUIRoot is null</color>");
            }
            if (chara != null && chara.ObjectUIRoot != null) {
                Debug.Log("<color=#00ff00> update chara</color>");
                chara.ObjectUIRoot.UpdateRecruitment();
            }
        }
    }
    #endregion
    #endregion

    #region エラーログ
    static void SaveConflictEntrantLog(EntrantInfo oldEnt, EntrantInfo newEnt)
	{
		try
		{
			// エラーログ&古い情報を破棄.
			string eLog = "ConflictEntrant " + newEnt.InFieldId +
				" : old=" + oldEnt.ToString()+ " " + oldEnt.InFieldId + " : " + oldEnt.EntrantType + ", " + oldEnt.UserName +
					", new=" + newEnt.ToString() + " " + newEnt.InFieldId + " : " + newEnt.EntrantType + ", " + newEnt.UserName;
			//Debug.LogError(eLog);
			BugReportController.SaveLogFile(eLog);
		}
		catch(System.Exception e)
		{
			BugReportController.SaveLogFileWithOutStackTrace(e.ToString());
		}
	}
	static void SaveInvalidIDInfoLog(EntrantInfo info)
	{
		try
		{
			// エラーログ&古い情報を破棄.
			string eLog = "InvalidIDInfo " + info.CreateStatus + " " + info.InFieldId + " : " + info.EntrantType + ", " + info.UserName;
			Debug.LogError(eLog);
			BugReportController.SaveLogFile(eLog);
		}
		catch(System.Exception e)
		{
			BugReportController.SaveLogFileWithOutStackTrace(e.ToString());
		}
	}
	#endregion
}