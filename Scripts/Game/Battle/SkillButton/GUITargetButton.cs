/// <summary>
/// ターゲットボタン
/// 
/// 2013/02/21
/// </summary>
using UnityEngine;
using System.Collections;

public class GUITargetButton : MonoBehaviour
{
	#region フィールド＆プロパティ
	public UIButton Button { get; private set; }
    public UISprite Sprite;
    public static GUITargetButton Instance;
	#endregion

	#region MonoBehaviourリフレクション
	void Start()
	{
	    Instance = this;
		this.Button = this.gameObject.GetSafeComponentInChildren<UIButton>();
	}
	#endregion

	#region NGUI
	void OnClick()
	{
        OUILockon.Instance.ToggleLockonTarget();
    }
	public void OnToggleLockonTarget(OUILockon.Type type)
	{
        ChangeSprite(type);
    }

    public void ChangeSprite(OUILockon.Type type) {
        switch (type) {
        case OUILockon.Type.None:
            Sprite.spriteName = "NoneLock";
            break;
        case OUILockon.Type.Single:
            Sprite.spriteName = "AccurateLock";
            break;
        case OUILockon.Type.Double:
            Sprite.spriteName = "SmartLock";
            break;
        default:
            throw new System.Exception("Unimplemented lock type icon");
        }
    }
    #endregion
}
