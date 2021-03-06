/// <summary>
/// 受け身ボタン
/// 
/// 2013/12/24
/// </summary>
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUISkillButton))]
public class GUITechButton : GUIRepeatButton
{
	#region フィールド＆プロパティ
	private GUISkillButton SkillButton { get; set; }
	public bool IsRepeat { get; set; }
	#endregion

	#region 初期化
	protected override void Start()
	{
		base.Start();

		this.SkillButton = this.gameObject.GetComponent<GUISkillButton>();
		this.IsRepeat = false;
	}
	#endregion

	#region 更新
	protected override void Update()
	{
		this.RepeatCounter(this.IsRepeat);
	}
	#endregion

	#region GUIRepeatButton Override
	protected override void Send()
	{
		Player player = GameController.GetPlayer();
		if (null == player)
			return;

		// 受け身.
		if(player.State == Character.StateProc.Down)
		{
			this.IsRepeat = !player.AirTech();
			this.SkillButton.IsDown = false;
		}
	}
	#endregion
}
