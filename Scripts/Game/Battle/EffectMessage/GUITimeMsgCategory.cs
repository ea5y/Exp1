/// <summary>
/// 時間エフェクトメッセージのカテゴリー処理
/// 
/// 2014/12/08
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.Master;
using Scm.Common.GameParameter;

public class GUITimeMsgCategory : GUIEffectMessageCategory
{
	#region 時間メッセージのセット

	/// <summary>
	/// 時間メッセージをセット
	/// </summary>
	public void SetTimeMessage(GUIEffectMessage.MsgType msgType, BattleFieldTimeEventMasterData timeMasterData)
	{
		// バトルが終了している場合はメッセージを表示しない
		if (NetworkController.ServerValue != null && NetworkController.ServerValue.FieldStateType == FieldStateType.Extra)
			return;

		// プレハブ情報取得
		GUIEffectMsgItemBase itemResource;
		if(!this.msgResourceDictionary.TryGetValue(msgType, out itemResource))
			return;
		
		// メッセージアイテム生成
		GUIEffectMsgItemBase newItem = GUIEffectMessageItem.Create(itemResource, this.Attach.parentList, true);
		if(newItem == null)
			return;

		// 時間メッセージアイテムに変換しセットアップ処理を行う
		GUITimeMessageItem timeMsgItem = newItem as GUITimeMessageItem;
		if(timeMsgItem == null)
			return;
		timeMsgItem.Setup(timeMasterData);
		
		// メッセージコントロールクラスに登録
		this.controller.SetEffectMessage(timeMsgItem);
	}

	#endregion
}
