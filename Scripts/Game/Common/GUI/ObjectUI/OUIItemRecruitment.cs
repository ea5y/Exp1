/// <summary>
/// 3Dオブジェクトに対するUIアイテム
/// 勝敗数
/// 
/// 2016/06/08
/// </summary>
using UnityEngine;
using Scm.Common.GameParameter;

public class OUIItemRecruitment : OUIItemBase {

	#region ==== フィールド＆プロパティ ====

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	public AttachObject Attach { get { return _attach; } }

	[System.Serializable]
	public class AttachObject {
        public UISprite bg;
		public UILabel text;
	}

	#endregion ==== フィールド＆プロパティ ====

	#region ==== 作成 ====

	/// <summary>
	/// プレハブ取得
	/// </summary>
	public static OUIItemRecruitment GetPrefab( GUIObjectUI.AttachObject.Prefab prefab, ObjectBase o ) {

		return ( ScmParam.Common.AreaType == AreaType.Lobby )?	prefab.recruitment.recruitment : null;
	}

	#endregion ==== 作成 ====

	#region ==== 更新 ====

	/// <summary>
	/// 勝敗数更新
	/// </summary>
	/// <param name="win"></param>
	/// <param name="lose"></param>
	public void UpdateRecruitment( int inFieldId, bool visible, string text ) {

        this.Attach.bg.gameObject.SetActive(visible);
        this.Attach.text.text = text;
        this._inFieldId = inFieldId;
	}

	#endregion ==== 更新 ====

    public void OnClick() {

        XUI.GUIRecruitment.Instance.OnRecruitmentItemClick(_inFieldId);
    }

    private int _inFieldId = 0;
}
