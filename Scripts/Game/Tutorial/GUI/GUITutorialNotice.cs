/// <summary>
/// チュートリアル用注視マーク
/// 
/// 2014/06/23
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUITutorialNotice : Singleton<GUITutorialNotice>
{
    #region Enumes
    #endregion
    
    #region Classes & Structs

    [System.Serializable]
    public class AttachObjects
    {
		public UIPanel Panel;
		public UIPlayTween RootTween;
		public UILabel Label;
    }

    #endregion
    
    #region Fields & Properties

    [SerializeField]
    private AttachObjects attach;

    #endregion

	#region Static Methods

	public static void SetActive(bool isActive)
	{
		if (Instance != null)
			Instance.setActive(isActive);
	}

	public static void SetAnchor(GameObject target, int offsetX, int offsetY, int sizeX, int sizeY)
	{
		if (Instance != null)
			Instance.setAnchor(target, offsetX, offsetY, sizeX, sizeY);
	}

	#endregion

    #region MonoBehaviour

    protected override void Awake()
    {
		base.Awake();
	}
    
    void Start()
    {
		setActive(false);
	}
    
    void Update()
    {
 
    }

    void OnDestroy()
    {

    }
    
    #endregion

    #region Private Methods

	private void setActive(bool isActive)
	{
		gameObject.SetActive(isActive);

		if (isActive)
			attach.RootTween.Play(true);
	}

	private void setAnchor(GameObject target, int offsetX, int offsetY, int sizeX, int sizeY)
	{
		attach.Label.SetAnchor(target,
		                       -(sizeX >> 1) + offsetX,
		                       -(sizeY >> 1) + offsetY,
		                       (sizeX >> 1) + offsetX,
		                       (sizeY >> 1) + offsetY);
	}
	
	#endregion
    
    #region Public Methods
    #endregion
}
