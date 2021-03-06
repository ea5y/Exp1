/// <summary>
/// 3Dオブジェクトに対するUIをセッティングするコンポーネント
/// 
/// 2014/06/25
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

public class GUIObjectUISettings : MonoBehaviour {
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
	/// バフデバフ
	/// </summary>
	[SerializeField]
	OUIItemRoot.Settings _buff = new OUIItemRoot.Settings() { isEnable = true, isInRangeDraw = true, attachName = "Buff", };
	public OUIItemRoot.Settings Buff { get { return _buff; } }

	/// <summary>
	/// ダメージ
	/// </summary>
	[SerializeField]
	OUIItemRoot.DamageSettings _damage = new OUIItemRoot.DamageSettings() { isEnable = true, attachName = "Damage", };
	public OUIItemRoot.DamageSettings Damage { get { return _damage; } }

	/// <summary>
	/// HP設定
	/// </summary>
	[SerializeField]
	OUIItemRoot.Settings _hp = new OUIItemRoot.Settings() { isEnable = true, isInRangeDraw = true, attachName = "HP", };
	public OUIItemRoot.Settings Hp { get { return _hp; } }

	/// <summary>
	/// キル数
	/// </summary>
	[SerializeField]
	OUIItemRoot.Settings _kill = new OUIItemRoot.Settings() { isEnable = true, isInRangeDraw = false, attachName = "Kill", };
	public OUIItemRoot.Settings Kill { get { return _kill; } }

	/// <summary>
	/// 名前設定
	/// </summary>
	[SerializeField]
	OUIItemRoot.Settings _name = new OUIItemRoot.Settings() { isEnable = true, isInRangeDraw = true, attachName = "Name", };
	public OUIItemRoot.Settings Name { get { return _name; } }

	/// <summary>
	/// 状態アイコン
	/// </summary>
	[SerializeField]
	OUIItemRoot.Settings _status = new OUIItemRoot.Settings() { isEnable = true, isInRangeDraw = true, attachName = "Status", };
	public OUIItemRoot.Settings Status { get { return _status; } }

	/// <summary>
	/// ランキング上位者の特殊アイコン
	/// </summary>
	[SerializeField]
	OUIItemRoot.Settings _ranking = new OUIItemRoot.Settings() { isEnable = true, isInRangeDraw = true, attachName = "Ranking" };
	public OUIItemRoot.Settings Ranking { get { return _ranking; } }

	/// <summary>
	/// 戦闘中のスコア順位アイコン
	/// </summary>
	[SerializeField]
	OUIItemRoot.Settings _scoreRank = new OUIItemRoot.Settings() { isEnable = true, isInRangeDraw = true, attachName = "ScoreRank" };
	public OUIItemRoot.Settings ScoreRank { get { return _scoreRank; } }

	/// <summary>
	/// 勝敗数アイコン
	/// </summary>
	[SerializeField]
	OUIItemRoot.Settings _winLose = new OUIItemRoot.Settings() { isEnable = true, isInRangeDraw = true, attachName = "WinLose" };
	public OUIItemRoot.Settings WinLose { get { return _winLose; } }


	public OUIItemRoot ItemRoot { get; private set; }
	#endregion

	#region 初期化
	//void Start()
	//{
	//    var o = this.GetComponent<ObjectBase>();
	//    if (o == null)
	//        return;
	//    var r = GUIObjectUI.CreateRoot(o, this.ModelAttachName, this.DrawRange);
	//    if (r == null)
	//        return;
	//    this.ItemRoot = r;

	//    r.SetupAttach(this.Attach);
	//    r.SetupBG(this.Bg);
	//    r.SetupBuff(this.Buff);
	//    r.SetupDamage(this.Damage);
	//    r.SetupHP(this.Hp);
	//    r.SetupKill(this.Kill);
	//    r.SetupName(this.Name);
	//    r.SetupStatus(this.Status);
	//    r.SetupRanking(this.Ranking);
	//    r.SetupScoreRank(this.ScoreRank);
	//}
	#endregion
}
