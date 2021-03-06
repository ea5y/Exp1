/// <summary>
/// ミニマップアイコン設定
/// ObjectBase にくっつける
/// 
/// 2014/10/30
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

public class GUIMinimapIconSettings : MonoBehaviour
{
	#region フィールド＆プロパティ
	/// <summary>
	/// アイコンタイプ
	/// </summary>
	[SerializeField]
	GUIMinimap.IconType _iconType;
	GUIMinimap.IconType IconType { get { return _iconType; } }

	public GUIMinimapIconItem Icon { get; private set; }
	#endregion

	#region 初期化
	IEnumerator Start()
	{
		// ミニマップが存在するかどうか
		while (GUIMinimap.Instance == null)
		{
			yield return null;
		}

		var o = this.GetComponent(typeof(ObjectBase)) as ObjectBase;
		if (o == null)
			yield break;

		this.Icon = GUIMinimap.CreateIcon(this.IconType, o);
	}
	#endregion
}
