/// <summary>
/// キャストマーカー
/// 
/// 2013/01/21
/// </summary>
using UnityEngine;
using Scm.Common.Master;

public class CastMarker : MarkerBase
{
	#region フィールド＆プロパティ
	float Timer { get; set; }
	#endregion

	#region 初期化
	/// <summary>
	/// 初期化
	/// </summary>
	public void Setup(ObjectBase caster, Vector3 position, Quaternion rotation, SkillCastMarkerMasterData markerData)
	{
		this.enabled = false;

		this.Timer = markerData.Timer;

		// トランスフォーム設定
		Transform t = this.transform;
		t.localPosition = position;
		t.localRotation = rotation;
		t.localScale = new Vector3(markerData.SizeX, markerData.SizeY, markerData.SizeZ);
		// マーカーオブジェクトをインスタンス化する
		base.CreateMarkerObject(caster.TeamType, markerData, this.SetupMarkerObject);
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
		// タイマーで削除
		this.Timer -= Time.deltaTime;
		if (0f >= this.Timer)
		{
			base.MarkerObjectDestroy();
		}
	}
	#endregion
}
