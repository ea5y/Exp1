/// <summary>
/// 共有アセットバンドル用,UnityEngine.Objectを保持するだけ.
/// 
/// 2014/06/12
/// </summary>
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(AssetBinder), true)]
public class AssetBindersEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		GUILayout.Space(8f);
		
		AssetBinder assetBinders = (AssetBinder)target;
		if(GUILayout.Button("Sort Asset Type"))
		{
			if(assetBinders != null)
			{
				List<UnityEngine.Object> objList = new List<UnityEngine.Object>(assetBinders.Assets);
				objList.Sort((UnityEngine.Object x,UnityEngine.Object y) => {
					return string.Compare(x.GetType().ToString(), y.GetType().ToString());
				});
				assetBinders.Assets = objList.ToArray();
				EditorUtility.SetDirty(target);
			}
		}
		
		GUILayout.Space(8f);
	}
}
