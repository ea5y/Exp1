/// <summary>
/// エイミングマーカー
/// 
/// 2013/12/18
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.Master;
using Scm.Common.GameParameter;

public class AimingMarker : MarkerBase
{
	#region フィールド＆プロパティ
	GUISkillButton Button { get; set; }
	Character Caster { get; set; }
	ObjectBase Target { get; set; }
	float Range { get; set; }
	SkillBulletSetMasterData BulletSet { get; set; }
    Vector3? TargetPosition;
	#endregion

	#region 初期化
	public void Setup(GUISkillButton button, Character caster, ObjectBase target, Vector3? targetPosition, float range, SkillBulletSetMasterData bulletSet, SkillAimingMarkerMasterData markerData)
	{
		this.enabled = false;

		this.Button = button;
		this.Caster = caster;
		this.Target = target;
		this.Range = range;
	    this.BulletSet = bulletSet;
        this.TargetPosition = targetPosition;

		// トランスフォーム設定
		Transform t = this.transform;
		t.localPosition = new Vector3(0f, 0f, range);
		t.localRotation = Quaternion.identity;
		t.localScale = new Vector3(markerData.SizeX, markerData.SizeY, markerData.SizeZ);
		// マーカーオブジェクトをインスタンス化する
		base.CreateMarkerObject(caster.TeamType, markerData, this.SetupMarkerObject);

        //Lee add For Skill Lock Rotation
	    CharacterCamera.LockRotationForSkill = true;
	}
	/// <summary>
	/// マーカーオブジェクトがインスタンス化された時に呼び出される
	/// </summary>
	void SetupMarkerObject(GameObject marker)
	{
		if (marker == null)
			{ return; }

		Transform t = marker.transform;
		t.parent = this.transform;
		t.localPosition = Vector3.zero;
		t.localRotation = Quaternion.identity;
		t.localScale = Vector3.one;

		this.enabled = true;
		this.Update();
	}
	#endregion

	#region 更新
	void Update()
	{
		// カメラの向いている方向にマーカーを出す
		if (this.Caster != null)
		{
			Vector3 position;
			Quaternion rotation;
			MarkerBase.CalculateBirth(this.Caster, GUIObjectUI.LockonObject, TargetPosition, this.Range, this.Caster.transform.position, this.BulletSet, out position, out rotation);
			this.transform.localPosition = new Vector3(position.x, 0f, position.z);
			this.transform.localRotation = rotation;
		}

		// ボタンが見つからないなら削除
		if (this.Button == null)
		{
			base.MarkerObjectDestroy();
			return;
		}
		// ボタンを離したら削除
		if (!this.Button.IsDown)
		{
			base.MarkerObjectDestroy();
            //Lee add For Release Lock
		    CharacterCamera.LockRotationForSkill = false;
			return;
		}
		// 怯みなどでキャンセルした時にマーカーを削除する
		if (this.Caster.State != Character.StateProc.SkillMotion)
		{
			base.MarkerObjectDestroy();
			return;
		}
	}
	#endregion
}
