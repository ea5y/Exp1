/// <summary>
/// リザルトの報酬情報
/// 
/// 2014/11/12
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Packet;

public class GUIResultRewardsInfoOld : MonoBehaviour
{
	#region 定数
	/// <summary>
	/// 手に入れる報酬アイテムの最大数
	/// </summary>
	private const int GetItemMaxCount = 4;
	#endregion

	#region フィールド&プロパティ
	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public XUIButton exitButton;
		public UITable itemTable;
		public UILabel itemCountLabel;
	}

	/// <summary>
	/// 報酬アイテムリスト
	/// </summary>
	private List<GUIResultRewardItem> rewardItemList = new List<GUIResultRewardItem>();

	/// <summary>
	/// 残りの報酬取得数
	/// </summary>
	private int remainingItemCount;
	#endregion
	
	#region セットアップ
	/// <summary>
	/// 報酬情報のセットアップ
	/// </summary>
	public void Setup(List<RewardBattleResultParameter> rewardList)
	{
		if(this.Attach.itemTable == null) return;

		// Exitボタンを非表示にする
		if(this.Attach.exitButton != null)
		{
			this.Attach.exitButton.gameObject.SetActive(false);
		}

		// テーブルからアイテムの一覧を取得する(サーバからアイテムの情報が取得するようになるまで)
		List<Transform> itemList = this.Attach.itemTable.GetChildList();
		foreach(Transform item in itemList)
		{
			// GUIResultRewardItem取得
			GUIResultRewardItem rewardItem = item.gameObject.GetSafeComponent<GUIResultRewardItem>();
			if(rewardItem == null) continue;

			// セットアップしリストに登録する
			rewardItem.Setup(OpenRewardItem);
			this.rewardItemList.Add(rewardItem);
		}

		// テーブル整形
		this.Attach.itemTable.Reposition();

		// 手に入れるアイテムの数をセット
		this.remainingItemCount = GetItemMaxCount;
		if(this.Attach.itemCountLabel != null)
		{
			this.Attach.itemCountLabel.text = this.remainingItemCount.ToString();
		}
	}
	#endregion

	#region 報酬アイテムが開いた時の処理
	/// <summary>
	/// 報酬アイテムが開いた時に呼び出されるメソッド
	/// </summary>
	/// <param name="itemID">
	/// 開いた報酬アイテムID
	/// </param>
	private void OpenRewardItem(int itemID)
	{
		// 取得出来る残りのアイテム数を減らしラベルにセットする
		this.remainingItemCount--;
		if(this.Attach.itemCountLabel != null)
		{
			this.Attach.itemCountLabel.text = this.remainingItemCount.ToString();
		}

		// 取得出来るアイテム数が無いかどうか判定
		if(this.remainingItemCount <= 0)
		{
			// 全てのアイテムのボタン機能をOFFにする
			foreach(GUIResultRewardItem item in this.rewardItemList)
			{
				item.SetButtonEnable(false);
			}

			// Exitボタンを表示する
			if(this.Attach.exitButton != null)
			{
				this.Attach.exitButton.gameObject.SetActive(true);
			}
		}
	}
	#endregion
}
