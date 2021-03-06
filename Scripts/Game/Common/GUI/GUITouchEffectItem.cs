/// <summary>
/// タッチエフェクトアイテム
/// 
/// 2014/12/01
/// </summary>
using UnityEngine;
using System.Collections;

public class GUITouchEffectItem : MonoBehaviour
{
	#region フィールド&プロパティ
	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UIPlayTween effectTween;
	}
	#endregion

	#region 生成
	/// <summary>
	/// アイテム生成
	/// </summary>
	public static GUITouchEffectItem Create(GameObject prefab, Transform parent, Vector3 position)
	{
		// インスタンス化
		GameObject gameObj = SafeObject.Instantiate(prefab) as GameObject;
		if(gameObj == null) return null;

		// GUITouchEffectItem取得
		GUITouchEffectItem item = gameObj.GetSafeComponent<GUITouchEffectItem>();
		if(item == null) return null;

		// 親子付け
		item.transform.parent = parent;
		item.transform.position = position;
		item.transform.localPosition = new Vector3(item.transform.localPosition.x, item.transform.localPosition.y, 0);
		item.transform.localRotation = Quaternion.identity;
		item.transform.localScale = Vector3.one;

		// 表示
		item.gameObject.SetActive(true);

		// エフェクト再生
		item.Play();

		return item;
	}
	#endregion

	#region エフェクトの再生
	private void Play()
	{
		if(this.Attach.effectTween == null) return;
		this.Attach.effectTween.Play(true);
	}
	#endregion

	#region 削除
	private void Delete()
	{
		GameObject.Destroy(this.gameObject);
	}
	#endregion

	#region NGUIリフレクション
	public void OnDelete()
	{
		Delete();
	}
	#endregion
}
