/// <summary>
/// ランキングウィンドウ
/// 
/// 2013/09/26
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.GameParameter;
using Scm.Common.Packet;

public class GUIRankingWindow : MonoBehaviour
{
	#region 定数
	
	/// <summary>
	/// 読み込んだアイテムを所持できる最大数.
	/// </summary>
	private const int ItemOverCount = 16;
	
	#endregion
	
	#region フィールド＆プロパティ
	[SerializeField]
	private GameObject itemPrefab;
	public  GameObject ItemPrefab { get { return itemPrefab; } }
	[SerializeField]
	private Transform itemRoot;
	public  Transform ItemRoot { get { return itemRoot; } }
	[SerializeField]
	private UIScrollBar scrollBar;
	public  UIScrollBar ScrollBar { get { return scrollBar; } }
	[SerializeField]
	private GUIRankingItem playerItem;
	public  GUIRankingItem PlayerItem { get { return playerItem; } }
	[SerializeField]
	private SortedList<int, GUIRankingItem> itemList = new SortedList<int, GUIRankingItem>();
	public  SortedList<int, GUIRankingItem> ItemList { get { return itemList; } }
	[SerializeField]
	private UILabel rankingModeLabel;
	public UILabel RankingModeLabel { get { return rankingModeLabel; } }
	[SerializeField]
	private UIScrollView scrollView;
	public UIScrollView ScrollView { get { return scrollView; } }
	[SerializeField]
	private UIPanel panel;
	public UIPanel Panel { get { return panel; } }
	
	const float ScrollValueEnd = 0.75f;
	public bool IsScrollEnd
	{
		get
		{
			if (this.ScrollBar == null)
				{ return false; }
			return ScrollValueEnd <= this.ScrollBar.value;
		}
	}
	
	UITable Table { get; set; }
	UIPlayTween PlayTween { get; set; }

	const int LoadNum = 8;
	const int MinCount = 1;
	int MaxCount { get; set; }
	int PlayerIndex { get; set; }
	bool IsActive { get; set; }
	public bool IsLoad { get; set; }
	int LoadMinIndex { get; set; }
	int LoadMaxIndex { get; set; }
	RankingType RankingType { get; set; }
	RankingPeriodType PeriodType { get; set; }
	#endregion

	#region 初期化
	void Awake()
	{
		this.IsLoad = false;

		// コンポーネント取得
		this.Table = this.gameObject.GetSafeComponentInChildren<UITable>();
		this.PlayTween = this.gameObject.GetSafeComponentInChildren<UIPlayTween>();
		this.ScrollView.onDragFinished = OnDragFinish;
		this.DeleteAll();
		this.IsActive = false;
		this.gameObject.SetActive(this.IsActive);
	}
	#endregion
	
	#region 更新
	void LoadItem()
	{
		if (this.ScrollBar == null)
			{ return; }
		if (this.IsLoad)
			{ return; }
		if (this.LoadMaxIndex > this.MaxCount && this.LoadMinIndex < MinCount)
			{ return; }

		if (this.LoadMinIndex > MinCount && this.ScrollBar.value <= 0f)
		{
			this.IsLoad = true;
			// 次の読み込む開始順位をセット
			//int startLoadRank = Mathf.Max(MinCount, (this.LoadMinIndex - LoadNum)+1);
			//LobbyPacket.SendRankingList(this.RankingType, this.PeriodType, startLoadRank, LoadNum);
		}
		if ((this.LoadMaxIndex+1) < MaxCount && this.ScrollBar.value >= 1f)
		{
			this.IsLoad = true;
			// 次の読み込む開始順位をセット
			//int startLoadRank = this.LoadMaxIndex + 2;
			//LobbyPacket.SendRankingList(this.RankingType, this.PeriodType, startLoadRank, LoadNum);
		}
	}
	#endregion
	
	#region アクティブ設定
	public void SetActive(bool isActive, RankingType rankingType, string rankingModeName = "")
	{
		if (this.PlayTween == null)
			{ return; }
		if (this.Table == null)
			{ return; }

		this.IsActive = isActive;
		if (isActive)
		{
			// アイテム削除
			this.DeleteAll();
			
			// ランキングタイプをセット
			this.RankingType = rankingType;
			
			// 期間タイプをセット(ランキングウィンドウを開いた時(デフォルト時)は総合スコアを表示する
			//this.PeriodType = RankingPeriodType.Total;
			
			// アイテムを取得し直す
			//LobbyPacket.SendRanking(this.RankingType, this.PeriodType);
		}
		else
		{
			// 変数初期化
			this.LoadMinIndex = 0;
			this.LoadMaxIndex = 0;
			this.PlayerIndex = 0;
			this.MaxCount = 0;
		}
		
		// ランキングモード名セット
		if(this.rankingModeLabel)
		{
			this.rankingModeLabel.text = rankingModeName;
		}
		
		// アニメーション開始
		this.PlayTween.Play(isActive);
	}
	public void SetItemNum(int index, int num, int provisionalRank, string userName, int score)
	{
		this.LoadMinIndex = index;
		this.LoadMaxIndex = index;
		this.PlayerIndex = index;
		this.MaxCount = num;
		
		/* 自分の暫定ランキングの表示がなくなったのでコメント化
		 * いずれリアルタイムのランキングが表示される可能性があるのでコードは残しておく.
		if (this.PlayerItem)
		{
			this.PlayerItem.Setup(0, provisionalRank, userName, score, true);
		}
		 */
		
		// 自分プレイヤーが一番上に表示されるようにする
		//int loadIndex = (index+1);
		//if(LoadNum >= this.MaxCount-index)
		//{
		//	// 最大表示件数より表示数が少ない場合は上の件数から最大数まで表示する
		//	loadIndex = this.MaxCount - LoadNum + 1;
		//}
		
		// ロビーリストを更新する
		this.IsLoad = true;
		//LobbyPacket.SendRankingList(this.RankingType, this.PeriodType, loadIndex, LoadNum);
	}
	#endregion

	#region 追加

	
	private void AddItem(int index, int rank, string userName, int score, int prevRank)
	{
		if (this.ItemPrefab == null)
			{ return; }
		if (!this.IsActive)
			{ return; }
		// 既に登録されている
		if (this.ItemList.ContainsKey(index))
			{ return; }

		// インスタンス化
		GameObject go = SafeObject.Instantiate(this.ItemPrefab) as GameObject;
		if (go == null)
			{ return; }
		go.SetActive(true);
		// コンポーネント取得
		GUIRankingItem item = go.GetSafeComponentInChildren<GUIRankingItem>();
		if (item == null)
			{ return; }

		// 名前
		go.name = string.Format("{0}{1:0000}", this.ItemPrefab.name, rank);

		// 親子付け
		go.transform.parent = this.ItemRoot;
		go.transform.localPosition = this.ItemPrefab.transform.localPosition;
		go.transform.localRotation = this.ItemPrefab.transform.localRotation;
		go.transform.localScale = this.ItemPrefab.transform.localScale;

		// アイテムセットアップ
		//item.Setup(index, rank, userName, score, prevRank, this.PlayerIndex == index);
		// リストに追加
		this.ItemList.Add(index, item);
		
		// ロビー変数更新
		if (this.LoadMinIndex > index)
		{
			this.LoadMinIndex = index;
		}
		this.LoadMaxIndex = (this.LoadMaxIndex < index) ? index : this.LoadMaxIndex;
		
		// アイテム再配置
		this.Table.Reposition();
		
		// スクロールを更新するかどうか
		// アイテム再配置前に現在のスクロール位置を取得する
		bool isScrollEnd = this.IsScrollEnd;
		if (!this.ScrollView.shouldMoveVertically || isScrollEnd)
		{
			// スクロールを最後まで下げる
			this.ScrollEnd();
		}
		this.Panel.Refresh();
	}
	
	void DeleteAll()
	{
		if (this.ItemRoot == null)
			{ return; }
		if (this.ScrollBar == null)
			{ return; }
		if (this.Table == null)
			{ return; }

		// オブジェクト削除
		Transform transform = this.ItemRoot.transform;
		for (int i=0; i<transform.childCount; i++)
		{
			var item = transform.GetChild(i);
			Object.Destroy(item.gameObject);
		}
		// リストクリア
		this.ItemList.Clear();
		
		// アイテム再配置
		this.Table.Reposition();
	}
	
	#endregion
	
	#region スクロール関係
	
	/// <summary>
	/// ドラッグされた後に呼ばれるメソッド.
	/// </summary>
	void OnDragFinish()
	{
		this.LoadItem();
	}
	
	/// <summary>
	/// スクロールバー一番下の位置にセットする
	/// </summary>
	private void ScrollEnd()
	{
		this.ScrollView.UpdateScrollbars(true);
		this.ScrollView.SetDragAmount(0f, 1f, false);
	}
	
	#endregion
	
	#region 期間選択ボタンが押された時に呼ばれるメソッド
	
	/// <summary>
	/// 累計ボタン
	/// </summary>
	private void OnTotalButton()
	{
		//SelectPeriod(RankingPeriodType.Total);
	}
	
	/// <summary>
	/// 確定した日時のボタン
	/// </summary>
	private void OnDailyFixButton()
	{
		//SelectPeriod(RankingPeriodType.DailyFix);
	}
	
	/// <summary>
	/// 週毎(中間)
	/// </summary>
	private void OnWeeklyButton()
	{
		//SelectPeriod(RankingPeriodType.Weekly);
	}
	
	/// <summary>
	/// 確定した週毎
	/// </summary>
	private void OnWeeklyFixButton()
	{
		//SelectPeriod(RankingPeriodType.WeeklyFix);
	}
	
	#endregion
	
	#region 期間選択時の処理
	
	/// <summary>
	/// 期間を選択した時の処理.
	/// </summary>
	/// <param name='periodType'>
	/// Period type.
	/// </param>
	private void SelectPeriod(RankingPeriodType periodType)
	{
		// 現在表示しているものを全て削除する
		DeleteAll();
		ModeSelectClear();
		
		// スクロールバー初期化.
		ScrollEnd();
		this.panel.Refresh();
		
		// サーバにデータ取得の送信を行う
		this.PeriodType = periodType;
		//LobbyPacket.SendRanking(this.RankingType, this.PeriodType);
	}
	
	/// <summary>
	/// 期間を変更した時にデータをクリアする処理
	/// </summary>
	private void ModeSelectClear()
	{
		this.LoadMinIndex = 0;
		this.LoadMaxIndex = 0;
		this.PlayerIndex = 0;
		this.MaxCount = 0;
		this.IsLoad = true;
	}
	
	#endregion
}
