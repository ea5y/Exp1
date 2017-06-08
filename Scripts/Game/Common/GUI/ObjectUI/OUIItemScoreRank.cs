using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

public class OUIItemScoreRank : OUIItemBase
{
	#region 定義

	enum ScoreRank{ First = 1 , Second , Third }

	#endregion

	#region フィールド＆プロパティ
	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[System.Serializable]
	public class AttachObject
	{
		[Tooltip("１位アイコン")]
		public GameObject FirstRankIcon;

		[Tooltip("２位アイコン")]
		public GameObject SecondRankIcon;

		[Tooltip("３位アイコン")]
		public GameObject ThirdRankIcon;
	}

	[SerializeField] AttachObject _attach;
	public AttachObject Attach { get { return _attach; } }

	#endregion

	#region 作成
	/// <summary>
	/// プレハブ取得
	/// </summary>
	public static OUIItemScoreRank GetPrefab(GUIObjectUI.AttachObject.Prefab prefab, ObjectBase o)
	{
		// 戦闘中以外は作成しない
		if (ScmParam.Common.AreaType != AreaType.Field)
			return null;
		return prefab.scoreRank.scoreRank;
	}
	#endregion

	#region 更新
	public void UpdateUI(int scoreRank)
	{
		switch( (ScoreRank)scoreRank )
		{
		case ScoreRank.First:
			this.Attach.FirstRankIcon.SetActive(true);
			this.Attach.SecondRankIcon.SetActive(false);
			this.Attach.ThirdRankIcon.SetActive(false);
			break;

		case ScoreRank.Second:
			this.Attach.FirstRankIcon.SetActive(false);
			this.Attach.SecondRankIcon.SetActive(true);
			this.Attach.ThirdRankIcon.SetActive(false);
			break;

		case ScoreRank.Third:
			this.Attach.FirstRankIcon.SetActive(false);
			this.Attach.SecondRankIcon.SetActive(false);
			this.Attach.ThirdRankIcon.SetActive(true);
			break;

		default:
			this.Attach.FirstRankIcon.SetActive(false);
			this.Attach.SecondRankIcon.SetActive(false);
			this.Attach.ThirdRankIcon.SetActive(false);
			break;
		}
	}
	#endregion
}
