/// <summary>
/// 時間消滅エフェクト
/// 
/// 2013/01/30
/// </summary>
using UnityEngine;
using System.Collections;

public class TimeDestroyEffect : MonoBehaviour
{
	#region フィールド＆プロパティ
	[SerializeField]
	private float destroyTimer = 5f;
	public float DestroyTimer { get { return destroyTimer; } private set { destroyTimer = value; } }
	public float DestroyCounter { get; private set; }

	public Manager Manager { get; private set; }
	public string BundlePath { get; private set; }
	public string FilePath { get; private set; }
	public bool IsPrefabValue { get; private set; }
	#endregion

	#region 初期化
	public static bool Setup(GameObject go, Manager manager, string bundlePath, string filePath, bool isPrefabValue, float destroyTimer)
	{
		// コンポーネント取得
		TimeDestroyEffect effect = go.GetSafeComponent<TimeDestroyEffect>();
		if (effect == null)
		{
			if (manager)
				manager.Destroy(go);
			else
				Object.Destroy(go);
			return false;
		}
		effect.Manager = manager;
		effect.BundlePath = bundlePath;
		effect.FilePath = filePath;
		effect.IsPrefabValue = isPrefabValue;
		effect.DestroyTimer = destroyTimer;
		effect.DestroyCounter = effect.DestroyTimer;
		return true;
	}
	void Start()
	{
		if (this.IsPrefabValue)
		{
			ResourceLoad.Instantiate(this.BundlePath, this.FilePath, null,
				(GameObject go)=>{ go.transform.parent = this.transform; }
			);
		}
		else
		{
			ResourceLoad.Instantiate(this.BundlePath, this.FilePath, this.transform.position, this.transform.rotation, null,
				(GameObject go)=>{ go.transform.parent = this.transform; }
			);
		}
	}
	#endregion

	#region 破棄
	void Update()
	{
		this.DestroyCounter -= Time.deltaTime;
		if (0f < this.DestroyCounter)
			return;

		this.Destroy();
	}
	void OnDestroy()
	{
		this.Destroy();
	}
	void Destroy()
	{
		if (this.Manager)
			this.Manager.Destroy(this.gameObject);
		else
			Object.Destroy(this.gameObject);
	}
	#endregion
}
