/// <summary>
/// ランキングアイテム
/// 
/// 2013/09/26
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIRankingItem : MonoBehaviour
{
	#region フィールド＆プロパティ
	[System.Serializable]
	public class AttachObject
	{
		public UILabel rankLabel;
		public UILabel nameLabel;
		public UILabel scoreLabel;
		public UISprite rankUpCursor;
		public UISprite rankDownCursor;
	}
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach{get{ return this._attach; } }	
	
	int Index { get;set; }
	int Rank { get ; set; }
	string UserName { get ; set; }
	int Score { get ;set; }
	int PrevRank { get;set; }

	public bool IsLoaded{ get;private set; }
	#endregion

	#region 初期化
	public static GUIRankingItem Create(GameObject prefab, Transform parent, int itemIndex)
	{
		// インスタンス化
		var go = SafeObject.Instantiate(prefab) as GameObject;
		if (go == null)
			return null;

		// 名前
		go.name = string.Format("{0}{1}", prefab.name, itemIndex);
		// 親子付け
		go.transform.parent = parent;
		go.transform.localPosition = prefab.transform.localPosition;
		go.transform.localRotation = prefab.transform.localRotation;
		go.transform.localScale = prefab.transform.localScale;
		// 可視化
		go.SetActive(true);

		// コンポーネント取得
		var item = go.GetComponentInChildren<GUIRankingItem>();
		if (item == null)
			return null;
		// 値初期化
		item.ClearValue();

		return item;
	}
	public void ClearValue()
	{
		Setup(false,0,0,"",0,0,false);
	}
	#endregion

	#region セットアップ
	public void Setup( bool isLoad , int index, int rank, string userName, int score, int prevRank, bool isPlayer)
	{
		this.Index = index;
		this.Rank = rank;
		this.UserName = userName;
		this.Score = score;
		this.PrevRank = prevRank;
		this.IsLoaded = isLoad;

		// ロード済
		if(isLoad)
		{
			// 現在ランクが０の場合はランク表示させない
			if (this.Attach.rankLabel != null)
				this.Attach.rankLabel.text  = ( this.Rank == 0 ) ? "-" : string.Format("{0}", this.Rank);

			if (this.Attach.nameLabel != null)
				this.Attach.nameLabel.text =  this.UserName;

			if (this.Attach.scoreLabel != null)
				this.Attach.scoreLabel.text = (this.Rank == 0 ) ? "-" : string.Format("{0}", this.Score);

			this.SetupRankCursor(this.PrevRank,this.Rank);

		}
		// ロード待ち
		else
		{
			ClearParam();
		}		
	}

	void SetupRankCursor( int prevRank , int nowRank )
	{
		if(this.Attach.rankUpCursor != null && this.Attach.rankDownCursor != null)
		{
			// ランク情報が０の場合はデータ無しとみなして表示させない
			if( prevRank == 0 || nowRank == 0 )
			{
				this.Attach.rankUpCursor.gameObject.SetActive(false);
				this.Attach.rankDownCursor.gameObject.SetActive(false);
			}
			else
			{
				// 現在ランクと過去ランクを比較して↑↓アイコンを出す
				this.Attach.rankUpCursor.gameObject.SetActive(( nowRank < prevRank ));
				this.Attach.rankDownCursor.gameObject.SetActive(( nowRank > prevRank ));
			}
		}
	}


	/// <summary>
	/// ロード中のバー設定
	/// </summary>
	void ClearParam()
	{
		if (this.Attach.rankLabel != null)
			this.Attach.rankLabel.text  = "";

		if (this.Attach.nameLabel != null)
			this.Attach.nameLabel.text = "";

		if (this.Attach.scoreLabel != null)
			this.Attach.scoreLabel.text = "";

		if(this.Attach.rankUpCursor != null )
			this.Attach.rankUpCursor.gameObject.SetActive(false);				

		if( this.Attach.rankDownCursor != null )
			this.Attach.rankDownCursor.gameObject.SetActive(false);
	}

	/// <summary>
	/// ランキングに一件もデータが無いときの NO DATA バー設定
	/// </summary>
	public void SetupNoData()
	{
		Setup(true,0,0,MasterData.GetText(TextType.TX145_Ranking_NoData),0,0,false);
	}
	public void SetupLoadFail()
	{
		ClearParam();
		this.IsLoaded = true;
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
		public bool execute;
		[Header("デバッグパラメーター")]
		public int rank;
		public string userName;
		public int score;
		public int prevRank;
		public bool isLoaded;
		public bool isPlayer;
	}


	void DebugUpdate()
	{
		var t = this.DebugParam;
		if (t.execute)
		{
			t.execute = false;
			{
				this.Setup(t.isLoaded,0,t.rank,t.userName,t.score,t.prevRank,t.isPlayer);
			}
		}
	}
	/// <summary>
	/// OnValidate はInspector上で値の変更があった時に呼び出される
	/// GameObject がアクティブ状態じゃなくても呼び出されるのでアクティブ化するときには有効
	/// </summary>
	void OnValidate()
	{
		if( Application.isPlaying )
			this.DebugUpdate();
	}
#endif
	#endregion


}
