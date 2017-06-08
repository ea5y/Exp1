/// <summary>
/// 3Dオブジェクトに対するUIアイテム
/// 勝敗数
/// 
/// 2016/06/08
/// </summary>
using UnityEngine;
using Scm.Common.GameParameter;

public class OUIItemWinLose : OUIItemBase {

	#region ==== フィールド＆プロパティ ====

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	public AttachObject Attach { get { return _attach; } }

	[System.Serializable]
	public class AttachObject {
		public UILabel winNumLabel;
		public UILabel loseNumLabel;
	}

	#endregion ==== フィールド＆プロパティ ====

	#region ==== 作成 ====

	/// <summary>
	/// プレハブ取得
	/// </summary>
	public static OUIItemWinLose GetPrefab( GUIObjectUI.AttachObject.Prefab prefab, ObjectBase o ) {

		return ( ScmParam.Common.AreaType == AreaType.Lobby )?	prefab.winlose.winlose : null;
	}

	#endregion ==== 作成 ====

	#region ==== 更新 ====

	/// <summary>
	/// 勝敗数更新
	/// </summary>
	/// <param name="win"></param>
	/// <param name="lose"></param>
	public void UpdateWinLose( int win, int lose ) {

		this.Attach.winNumLabel.text = string.Format( "{0}", win );
		this.Attach.loseNumLabel.text = string.Format( "{0}", 0 );
		//this.Attach.loseNumLabel.text = string.Format( "{0}", lose );
	}

	#endregion ==== 更新 ====
}
