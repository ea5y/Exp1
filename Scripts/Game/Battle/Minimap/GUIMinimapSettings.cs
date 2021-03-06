/// <summary>
/// ミニマップ本体の設定
/// 
/// 2013/03/07
/// </summary>
using UnityEngine;
using System.Collections;

public class GUIMinimapSettings : MonoBehaviour
{
	#region フィールド＆プロパティ
	/// <summary>
	/// ミニマップアトラス
	/// </summary>
	[SerializeField]
	UIAtlas _minimapAtlas;
	UIAtlas MinimapAtlas { get { return _minimapAtlas; } }

	/// <summary>
	/// 設定
	/// </summary>
	[SerializeField]
	Configuration _config;
	public Configuration Config { get { return _config; } }
	[System.Serializable]
	public class Configuration
	{
		public float scaleX = 1f;
		public float scaleY = 1f;
	}
	#endregion

	#region 初期化
	IEnumerator Start()
	{
		while (GUIMinimap.Instance == null)
		{
			yield return null;
		}

		// ミニマップ作成
		if (this.MinimapAtlas != null)
		{
			GUIMinimap.CreateMinimap(this.MinimapAtlas, new Vector2(this.Config.scaleX, this.Config.scaleY));
		}
		else
		{
			Debug.LogError("Minimap Atlas is Null!");
		}
	}
	#endregion
}
