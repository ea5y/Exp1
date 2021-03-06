/// <summary>
/// スキル名メッセージアイテム
/// 
/// 2015/01/16
/// </summary>
using UnityEngine;
using System.Collections;

public class GUISkillMessageItem : GUIEffectMessageItem
{
	#region アタッチオブジェクト

	[System.Serializable]
	public class AttachSkillObject
	{
		public UILabel nameLabel;
	}

	#endregion

	#region フィールド&プロパティ

	/// <summary>
	/// スキルアタッチオブジェクト
	/// </summary>
	[SerializeField]
	private AttachSkillObject skillAttach;
	public AttachSkillObject SkillAttach { get { return this.skillAttach; } }

	#endregion

	#region セットアップ

	/// <summary>
	/// セットアップ処理
	/// </summary>
	public void Setup(string skillName)
	{
		if(this.SkillAttach.nameLabel == null) return;
		this.skillAttach.nameLabel.text = skillName;
	}

	#endregion

}
