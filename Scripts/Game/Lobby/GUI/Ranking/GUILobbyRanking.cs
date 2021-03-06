/// <summary>
/// ロビーで表示するランキング処理.
/// .
/// 2014/04/07.
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;
using Scm.Common.Packet;

public class GUILobbyRanking : MonoBehaviour
{
	/// <summary>
	/// ランキングメニュー.
	/// </summary>
	[SerializeField]
	private GUIRankingMenuWindow menuWindow;
	
	/// <summary>
	/// ランキング.
	/// </summary>
	[SerializeField]
	private GUIRankingWindow rankingWindow;
	
	/// <summary>
	/// インスタンス.
	/// </summary>
	private static GUILobbyRanking instance;
	
	/// <summary>
	/// ランキングのロード.
	/// </summary>
	static public bool IsLoading
	{
		get { return instance != null ? instance.rankingWindow.IsLoad : false; }
		set { if (instance != null) { instance.rankingWindow.IsLoad = value; } }
	}
	
	#region 開始メソッド.
	
	void Awake()
	{
		instance = this;
	}
	
	#endregion
	
	#region ランキングウィンドウ.
	
	/// <summary>
	/// ランキングのウィンドウを開く.
	/// </summary>
	public static void OpenWindow()
	{
		if(instance)
		{
			instance.OpenRankingMenuWindow();
		}
	}
	
	/// <summary>
	/// ランキングのウィンドウを閉じる.
	/// </summary>
	public static void CloseWindow()
	{
		if(instance)
		{
			instance._CloseWindow();
		}
	}
	
	/// <summary>
	/// ランキングのウィンドウを閉じる処理.
	/// </summary>
	private void _CloseWindow()
	{
		this.menuWindow.Close();
		//this.rankingWindow.SetActive(false);
	}
	
	/// <summary>
	/// メインのランキングウィンドウを開く処理.
	/// </summary>
	/// <param name='rankingType'>
	/// ランキングの種別.
	/// </param>
	/// <param name='rankingModeName'>
	/// ランキングモード名.
	/// </param>
	private void OpenRankingWindow(RankingType rankingType, string rankingModeName)
	{
		this.menuWindow.Close();
		this.rankingWindow.SetActive(true, rankingType, rankingModeName);
	}
	
	/// <summary>
	/// メニューのランキングウィンドウを開く処理.
	/// </summary>
	private void OpenRankingMenuWindow()
	{
		this.menuWindow.Open();
		//this.rankingWindow.SetActive(false);
	}
	
	#endregion
	
	#region モード.
	
	/// <summary>
	/// モード選択..
	/// </summary>
	/// <param name='rankingType'>
	/// ランキングの種別.
	/// </param>
	/// <param name='rankingModeName'>
	/// ランキングモード名.
	/// </param>
	public static void ModeSelect(RankingType rankingType, string rankingModeName)
	{
		if(instance)
		{
			instance._ModeSelect(rankingType, rankingModeName);
		}
	}
	
	/// <summary>
	/// モード選択処理..
	/// </summary>
	/// <param name='rankingType'>
	/// ランキングの種別.
	/// </param>	/// <param name='rankingModeName'>
	/// ランキングモード名.
	/// </param>
	private void _ModeSelect(RankingType rankingType, string rankingModeName)
	{
		OpenRankingWindow(rankingType, rankingModeName);
	}
	
	#endregion-
	
	#region ランキングデータ追加.
	
	/// <summary>
	/// 自キャラのランキングデータをセット.
	/// </summary>
	/// <param name='index'>
	/// 確定している順位のインデックス.
	/// </param>
	/// <param name='num'>
	/// ランキング総数.
	/// </param>
	/// <param name='provisionalRank'>
	/// 暫定順位.
	/// </param>
	/// <param name='userName'>
	/// 名前.
	/// </param>
	/// <param name='score'>
	/// 暫定順位のスコア.
	/// </param>
	public static void SetItemNum(int index, int num, int provisionalRank, string userName, int score)
	{
		if(instance)
		{
			instance.rankingWindow.SetItemNum(index, num, provisionalRank, userName, score);
		}
	}
	

	
	#endregion

}
