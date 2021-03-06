/// <summary>
/// UI3DFXカメラ
/// 
/// 2016/04/28
/// </summary>
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UI3DFXCamera : Singleton<UI3DFXCamera>
{
	private Camera mainCamera = null;

	private CharacterCamera charaCamera = null;

	Camera MainCamera
	{
		get
		{
			if(mainCamera == null) {
				mainCamera = Camera.main;
			}
			return mainCamera;
		}
	}

	CharacterCamera CharacterCamera
	{
		get
		{
			if(charaCamera == null) {
				charaCamera = GameController.CharacterCamera;
			}
			return charaCamera;
		}
	}



	GameObject MainCameraObject
	{
		get
		{
			if (Camera.main != null)
			{
				return Camera.main.gameObject;
			}
			if (GameController.CharacterCamera != null)
			{
				return GameController.CharacterCamera.gameObject;
			}
			return null;
		}
	}

	private void Start()
	{
		this.gameObject.SetActive(false);
	}


	/// <summary>
	/// 3DFXカメラとメインカメラと切り替える
	/// </summary>
	public static void ToggleMainCamera()
	{
		if (Instance != null) Instance._ToggleMainCamera();
	}
	void _ToggleMainCamera()
	{
		this._SwitchActive(!this.gameObject.activeSelf);
	}

	/// <summary>
	/// 3DFXカメラとメインカメラのアクティブを切り替える
	/// </summary>
	public static void SwitchActive(bool isFXActive)
	{
		if (Instance != null) Instance._SwitchActive(isFXActive);
	}
	void _SwitchActive(bool isFXActive)
	{
		this.gameObject.SetActive(isFXActive);
		
		if(MainCamera != null) {
			MainCamera.enabled = (!isFXActive);
		}
		if(CharacterCamera != null) {
			CharacterCamera.gameObject.SetActive(!isFXActive);
		}

		//var go = this.MainCameraObject;
		//if (go != null)
		//{
		//	go.SetActive(!isFXActive);
		//}
	}

	/// <summary>
	/// アクティブ化
	/// </summary>
	public static void SetActive(bool isActive)
	{
		if (Instance != null) Instance._SetActive(isActive);
	}
	void _SetActive(bool isActive)
	{
		this.gameObject.SetActive(isActive);
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(UI3DFXCamera))]
	public class UI3DFXCameraEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Toggle"))
			{
				UI3DFXCamera.ToggleMainCamera();
			}
			base.OnInspectorGUI();
		}
	}
#endif
}
