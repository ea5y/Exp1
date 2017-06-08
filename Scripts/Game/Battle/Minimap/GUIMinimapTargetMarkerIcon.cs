/// <summary>
/// 誰がターゲットしているかのアイコン
/// 
/// 2014/12/05
/// </summary>
using UnityEngine;
using System.Collections;

public class GUIMinimapTargetMarkerIcon : MonoBehaviour 
{
	#region フィールド・プロパティ
	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField] AttachObject _attach;
	public AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UILabel	noLabel;
		public UISprite iconSprite;
	}

	GUIMinimapTargetMarker ParentTargetMarker{get;set;}
	int TacticalID{ get;set; }
	#endregion

	#region 作成 初期化
	public static GUIMinimapTargetMarkerIcon Create( GameObject prefab , Transform parent , int tacticalID )
	{
		// インスタンス化
		var go = SafeObject.Instantiate(prefab) as GameObject;
		if (go == null)
			return null;
		// 名前
		go.name = string.Format("{0}_{1:000}", prefab.name,tacticalID);
		// 親子付け
		go.transform.parent = parent;
		go.transform.localPosition = prefab.transform.localPosition;
		go.transform.localRotation = prefab.transform.localRotation;
		go.transform.localScale = prefab.transform.localScale;

		// コンポーネント取得
		var item = go.GetComponent(typeof(GUIMinimapTargetMarkerIcon)) as GUIMinimapTargetMarkerIcon;
		if (item == null)
			return null;

		// 値初期化
		item.ClearValue();

		return item;
	}
	public void Setup( Character character , GUIMinimapTargetMarker parentTargetMarker )
	{
		this.ParentTargetMarker = parentTargetMarker;
		this.TacticalID = character.TacticalId;

		// 情報セット
		if( this.Attach.noLabel != null )	
		{
			this.Attach.noLabel.text = this.TacticalID.ToString();
		}

		this.gameObject.SetActive(true);
	}

	public void ClearValue()
	{
		if( this.Attach.noLabel != null)
			this.Attach.noLabel.text = "";
	}
	#endregion

	//#region 更新
	//void Update()
	//{
	//	this.Attach.noLabel.transform.rotation = Quaternion.identity;
	//}
	//#endregion 

	#region 回転Reset
	public void ResetRotation()
	{
		this.Attach.noLabel.transform.rotation = Quaternion.identity;
	}
	#endregion

}
