/// <summary>
/// スキル名エフェクトメッセージのカテゴリー処理
/// 
/// 2015/01/16
/// </summary>
using UnityEngine;
using System.Collections;

public class GUISkillMsgCategory : GUIEffectMessageCategory
{
	#region スキルメッセージのセット

	public void SetSkill(string skillName)
	{
		// 空文字の場合は登録しない
		if(string.IsNullOrEmpty(skillName)) return;

		// プレハブ情報取得
		GUIEffectMsgItemBase itemResource;
		if(!this.msgResourceDictionary.TryGetValue(GUIEffectMessage.MsgType.Skill, out itemResource))
			return;
		
		// メッセージアイテム生成
		GUIEffectMsgItemBase newItem = GUIEffectMessageItem.Create(itemResource, base.Attach.parentList, true);
		if(newItem == null)
			return;
		
		// スキルメッセージアイテムに変換しセットアップ処理を行う
		GUISkillMessageItem skillMsgItem = newItem as GUISkillMessageItem;
		if(skillMsgItem == null)
			return;
		skillMsgItem.Setup(skillName);
		
		// メッセージコントロールクラスに登録
		base.controller.SetEffectMessage(skillMsgItem);
	}

	#endregion
}
