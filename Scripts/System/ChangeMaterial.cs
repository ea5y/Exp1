/// <summary>
/// マテリアル変更
/// 
/// 2013/05/02
/// </summary>
using UnityEngine;
using System.Collections;

public class ChangeMaterial : MonoBehaviour
{
	#region フィールド＆プロパティ
	[SerializeField]
	private string objectName;
	public string ObjectName { get { return objectName; } private set { objectName = value; } }
	[SerializeField]
	private int materialIndex;
	public int MaterialIndex { get { return materialIndex; } private set { materialIndex = value; } }
	[SerializeField]
	private Material[] materials;
	public Material[] Materials { get { return materials; } private set { materials = value; } }

	public Renderer Renderer { get; private set; }
	#endregion

	#region 設定
	void Start()
	{
		Transform transform = this.transform.SafeSearch(this.ObjectName);
		if (transform && transform.GetComponent<Renderer>())
		{
			this.Renderer = transform.GetComponent<Renderer>();
		}
		this.SetMaterial(0);
	}
	public void SetMaterial(int index)
	{
		if (this.Renderer == null)
			{ return; }
		if (this.Materials.Length <= index)
			{ return; }
		Material mtrl = this.Materials[index];
		this.Renderer.material.CopyPropertiesFromMaterial(mtrl);
//		this.Renderer.materials[this.MaterialIndex] = mtrl;
//		this.Renderer.sharedMaterials[this.MaterialIndex] = mtrl;
	}
	#endregion
}
