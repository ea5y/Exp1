/// <summary>
/// 時間メッセージアイテム
/// 2014/08/06
/// 
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.Master;

public class GUITimeMessageItem : GUIEffectMessageItem
{
	#region アタッチオブジェクト

	[System.Serializable]
	public class AttachTimeObject
	{
		[SerializeField]
		private UILabel messageLabel;
		public UILabel MessageLabel { get { return messageLabel; } }

		[SerializeField]
		private UILabel numberLabel;
		public UILabel NumberLabel { get { return numberLabel; } }
	}

	#endregion

	#region フィールド&プロパティ

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	private AttachTimeObject attachTimeobj;
	public AttachTimeObject AttachTimeObj { get { return attachTimeobj; } }

	/// <summary>
	/// タイムイベントマスターデータ
	/// </summary>
	public BattleFieldTimeEventMasterData TimeEventMasterData { get; private set; }
	
	#endregion
	

	#region セットアップ
	
	public virtual void Setup (BattleFieldTimeEventMasterData timeEventMasterData)
	{
		this.TimeEventMasterData = timeEventMasterData;

		// 秒→分変換
		int min = this.TimeEventMasterData.Time / 60;

		// Nullチェック
		if(this.AttachTimeObj.MessageLabel == null || this.AttachTimeObj.NumberLabel == null)
			return;

		// テキストマスターデータからメッセージ取得
		string message = "";
		if(this.MsgType == GUIEffectMessage.MsgType.TimeLimit)
		{
			// 残り時間
			message = MasterData.GetText(TextType.TX024_TimeLimit);
		}
		else if(this.MsgType == GUIEffectMessage.MsgType.TimeLater)
		{
			// 経過時間
			message = MasterData.GetText(TextType.TX026_Minutes) + MasterData.GetText(TextType.TX025_TimeLater);
		}

		// ラベルセット
		this.AttachTimeObj.MessageLabel.text = message;
		this.AttachTimeObj.NumberLabel.text = min.ToString();
	}
	
	#endregion
}
