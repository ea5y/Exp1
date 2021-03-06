/// <summary>
/// 3Dオブジェクトに対するUIアイテム
/// HP
/// 
/// 2014/06/24
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

public class OUIItemHP : OUIItemBase
{
	#region フィールド＆プロパティ
	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField] AttachObject _attach;
	public AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UISlider slider;
		public UISprite sprite;
	}

	// 揺れ時間.
	const float ShakeTime = 1.0f;
	// 揺れ幅.
	const float Shake = 2.0f;
	// 揺れスピード.
	const float ShakeSpeed = 2.0f;
	// 現在揺れている時間.
	float ShakeCount { get; set; }
	// 揺れファイバー
	IEnumerator ShakeFiber { get; set; }
	// シリアライズされていないメンバーの初期化
	void MemberInit()
	{
		this.ShakeCount = 0f;
		this.ShakeFiber = null;
	}
	#endregion

	#region 初期化
	void Awake()
	{
		this.MemberInit();
	}
	#endregion

	#region 作成
	/// <summary>
	/// プレハブ取得
	/// </summary>
	public static OUIItemHP GetPrefab(Transform root, ObjectBase o, string myteamItemName, string enemyItemName, string etcItemName)
	{
		// 戦闘中以外は作成しない
		if (ScmParam.Common.AreaType != AreaType.Field)
			return null;
		if (root == null)
			return null;

		Transform t = null;
		switch (o.TeamType.GetClientTeam())
		{
		case TeamTypeClient.Friend:
			t = root.Search(myteamItemName);
			break;
		case TeamTypeClient.Enemy:
			t = root.Search(enemyItemName);
			break;
		default:
			t = root.Search(etcItemName);
			break;
		}
		if (t == null)
			return null;

		OUIItemHP com = t.gameObject.GetComponent<OUIItemHP>();
		if (com == null)
			return null;

		return com;
	}
	#endregion

	#region 更新
	public void UpdateUI(int hp, int maxHp)
	{
		// ゲージ更新
		float t = 0f;
		if (0 < maxHp)
			t = (float)hp / (float)maxHp;

		this.Attach.slider.value = t;
		if (this.Attach.sprite != null)
			this.Attach.sprite.fillAmount = t;
	}
	#endregion

	#region 揺れ演出.
	void Update()
	{
		if (this.ShakeFiber != null)
			this.ShakeFiber.MoveNext();
	}
	public void ShakeGauge()
	{
		this.ShakeCount = ShakeTime;
		if (this.ShakeFiber != null)
		{
			return;
		}
		// OnValidate 内から呼び出すとコルーチンが停止してしまうためファイバーに変更
		this.ShakeFiber = ShakeCoroutine();
	}
	IEnumerator ShakeCoroutine()
	{
		Vector3 startPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, this.transform.localPosition.z);
		float shakeSin;
		float shakeCos;
		while (this.ShakeCount > 0)
		{
			this.ShakeCount -= Time.deltaTime;
			shakeSin = Mathf.Sin(Time.frameCount * ShakeSpeed) * Shake;
			shakeCos = Mathf.Cos(Time.frameCount * ShakeSpeed) * Shake;
			this.transform.localPosition = new Vector3(startPosition.x + shakeCos, startPosition.y + shakeSin, startPosition.z);
			yield return 0;
		}
		this.transform.localPosition = startPosition;
		this.ShakeFiber = null;
	}
	#endregion
}
