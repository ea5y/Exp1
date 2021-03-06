/// <summary>
/// ヒット/コンボメッセージのカテゴリー処理
/// 
/// 2014/01/27
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIHitMsgCategory : GUIEffectMessageCategory
{
	#region プロパティ&フィールド

	/// <summary>
	/// 現在ヒット(コンボ)中のヒットメッセージアイテム
	/// </summary>
	private GUIHitMessageItem item;

	#endregion

	#region ヒットメッセージのセット

	public void SetHit()
	{
		if(this.item == null || !this.item.IsCombo)
		{
			// プレハブ情報取得
			GUIEffectMsgItemBase itemResource;
			if(!this.msgResourceDictionary.TryGetValue(GUIEffectMessage.MsgType.Hit, out itemResource))
				return;
			
			// メッセージアイテム生成
			GUIEffectMsgItemBase newItem = GUIEffectMsgItemBase.Create(itemResource, base.Attach.parentList, true);
			if(newItem == null)
				return;
			
			// スキルメッセージアイテムに変換しセットアップ処理を行う
			GUIHitMessageItem hitMsgItem = newItem as GUIHitMessageItem;
			if(hitMsgItem == null)
				return;
			hitMsgItem.Setup();
			
			// メッセージコントロールクラスに登録
			base.controller.SetEffectMessage(hitMsgItem);

			this.item = hitMsgItem;
		}
		else
		{
			// コンボが継続中なら再セットする
			this.item.Setup();
		}
	}

	#endregion
}
