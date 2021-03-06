/// <summary>
/// 3Dオブジェクトに対するUIを設定する(バトル用)
/// 
/// 2014/06/25
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

public class GUIObjectUIBattleData : GUIObjectUIBaseData
{
	#region フィールド＆プロパティ
	/// <summary>
	/// モデルのアタッチする名前
	/// </summary>
	[SerializeField]
	string _modelAttachName = "name_plate";
	public string ModelAttachName { get { return _modelAttachName; } }

	/// <summary>
	/// 表示範囲
	/// 0なら表示しない
	/// </summary>
	[SerializeField]
	float _drawRange = 50f;
	public float DrawRange { get { return _drawRange; } }

	/// <summary>
	/// 各アイテムの位置設定用
	/// </summary>
	[SerializeField]
	OUIItemRoot.AttachSettings _attach = new OUIItemRoot.AttachSettings() { isEnable = true, };
	public OUIItemRoot.AttachSettings Attach { get { return _attach; } }

	/// <summary>
	/// BG用スプライト
	/// </summary>
	[SerializeField]
	OUIItemRoot.Settings _bg = new OUIItemRoot.Settings() { isEnable = true, isInRangeDraw = true, attachName = "BG", };
	public OUIItemRoot.Settings Bg { get { return _bg; } }

	/// <summary>
	/// 名前設定
	/// </summary>
	[SerializeField]
	OUIItemRoot.Settings _name = new OUIItemRoot.Settings() { isEnable = true, isInRangeDraw = true, attachName = "Name", };
	public OUIItemRoot.Settings Name { get { return _name; } }

	/// <summary>
	/// HP設定
	/// </summary>
	[SerializeField]
	OUIItemRoot.HPSettings _hp = new OUIItemRoot.HPSettings() { isEnable = true, isInRangeDraw = true, attachName = "HP", myteamItemName = "", enemyItemName = "", etcItemName = "", };
	public OUIItemRoot.HPSettings Hp { get { return _hp; } }

	/// <summary>
	/// ダメージ
	/// </summary>
	[SerializeField]
	OUIItemRoot.DamageSettings _damage = new OUIItemRoot.DamageSettings() { isEnable = true, attachName = "Damage", };
	public OUIItemRoot.DamageSettings Damage { get { return _damage; } }

	/// <summary>
	/// バフデバフ
	/// </summary>
	[SerializeField]
	OUIItemRoot.Settings _buff = new OUIItemRoot.Settings() { isEnable = true, isInRangeDraw = true, attachName = "Buff", };
	public OUIItemRoot.Settings Buff { get { return _buff; } }

	/// <summary>
	/// 戦闘中のスコア順位アイコン
	/// </summary>
	[SerializeField]
	OUIItemRoot.Settings _scoreRank = new OUIItemRoot.Settings() { isEnable = true, isInRangeDraw = true, attachName = "ScoreRank" };
	public OUIItemRoot.Settings ScoreRank { get { return _scoreRank; } }

	/// <summary>
	/// キル数
	/// </summary>
	[SerializeField]
	OUIItemRoot.Settings _kill = new OUIItemRoot.Settings() { isEnable = true, isInRangeDraw = false, attachName = "Kill", };
	public OUIItemRoot.Settings Kill { get { return _kill; } }

	/// <summary>
	/// 勝敗数設定
	/// </summary>
	[SerializeField]
	OUIItemRoot.Settings _winlose = new OUIItemRoot.Settings() { isEnable = true, isInRangeDraw = true, attachName = "WinLose", };
	public OUIItemRoot.Settings WinLose { get { return _winlose; } }

    /// <summary>
    /// Process
    /// </summary>
    [SerializeField]
    OUIItemRoot.Settings _process = new OUIItemRoot.Settings() { isEnable = false, isInRangeDraw = false, attachName = "Process", };
    public OUIItemRoot.Settings Process { get { return _process; } }
	#endregion

	#region 初期化
	public override OUIItemRoot Create(ObjectBase o)
	{
		if (o == null)
			return null;
		var r = GUIObjectUI.CreateRoot(o, this.ModelAttachName, this.DrawRange);
		if (r == null)
			return null;

		r.SetupAttach(this.Attach);
		r.SetupBG(this.Bg);
		r.SetupName(this.Name);
		r.SetupWinLose(this.WinLose);
		r.SetupHP(this.Hp);
		r.SetupDamage(this.Damage);
		r.SetupBuff(this.Buff);
		r.SetupScoreRank(this.ScoreRank);
		r.SetupKill(this.Kill);
        r.SetupResidentProcess(this.Process);
		return r;
	}
	#endregion
}
