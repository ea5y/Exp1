/// <summary>
/// 3Dオブジェクトに対するUI（ロックオン）のセッティングするコンポーネント
/// 
/// 2014/07/07
/// </summary>
using UnityEngine;
using System.Collections;

public class OUILockonSetting : MonoBehaviour
{
	#region フィールド＆プロパティ
	/// <summary>
	/// モデルのアタッチする名前
	/// </summary>
	[SerializeField] string _modelAttachName = "Root";
	public string ModelAttachName { get { return _modelAttachName; } }

	/// <summary>
	/// ロックオン範囲
	/// </summary>
	[SerializeField] float _lockonRange = 5000f;
	public float LockonRange { get { return _lockonRange; } }

	/// <summary>
	/// スクリーン上のオフセット
	/// </summary>
	[SerializeField] Vector3 _offset;
	public Vector3 Offset { get { return _offset; } }

	public OUILockon Lockon { get; private set; }
	#endregion

	#region 初期化
	void Start()
	{
		var o = this.GetComponent(typeof(ObjectBase)) as ObjectBase;
		if (o == null)
			return;
		// TODO:仮にオプションに移行
		//this.Lockon = GUIObjectUI.CreateLockon(this.ModelAttachName, this.LockonRange, this.Offset);
		this.Lockon = GUIObjectUI.CreateLockon(this.ModelAttachName, ConfigFile.Option.LockonRange, this.Offset);
	}
	#endregion
}
