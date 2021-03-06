/// <summary>
/// NPC砲台用スクリプト
/// 
/// 2013/10/04
/// </summary>
using UnityEngine;

public class Turret : MonoBehaviour
{
	/// <summary>
	/// Turretを使用することを表すインターフェイス.
	/// </summary>
	public interface ITurretCarrier
	{
		// 砲台オブジェクト
		Turret Turret { get; set; }
	}
	
	private Quaternion toRot;
	private float time;

	#region 初期化
	void Start()
	{
		toRot = this.transform.rotation;
		time = 0f;

		Transform parent = this.transform.parent;
		while(parent != null)
		{
			ObjectBase objectBase = parent.GetComponent<ObjectBase>();
			ITurretCarrier turretCarrier = objectBase as ITurretCarrier;
			if(turretCarrier != null)
			{
				turretCarrier.Turret = this;
				break;
			}
			parent = parent.parent;
		}
	}
	#endregion

	#region 更新
	void Update()
	{
		if(0f < time)
		{
			float factor = Time.deltaTime / time;
			this.transform.rotation = Quaternion.Slerp(this.transform.rotation, toRot, factor);
			time -= Time.deltaTime;
		}
		else
		{
			this.transform.rotation = toRot;
		}
	}
	#endregion

	#region Method
	/// <summary>
	/// 回転目標値をセット.
	/// </summary>
	public void SetRotation(Quaternion rotation, float time)
	{
		this.toRot = rotation;
		this.time = time;
	}
	#endregion
}
