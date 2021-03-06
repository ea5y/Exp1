/// <summary>
/// 3Dオブジェクトに対するUIアイテム
/// バフデバフ
/// 
/// 2014/06/23
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;
using Scm.Common.Packet;
using System.Collections.Generic;

public class OUIItemBuff : OUIItemBase
{
	#region フィールド＆プロパティ
	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField] AttachObject _attach;
	public AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public GameObject prefab;
		public Transform root;
		public GUIBuffPopup buffPopup;
	}
	#endregion

	#region 作成
	/// <summary>
	/// プレハブ取得
	/// </summary>
	public static OUIItemBuff GetPrefab(GUIObjectUI.AttachObject.Prefab prefab, ObjectBase o)
	{
		// 戦闘中以外は作成しない
		if (ScmParam.Common.AreaType != AreaType.Field)
			return null;
		return prefab.buff.buff;
	}
	#endregion

	#region 更新
	public void UpdateUI()
	{
	}
	#endregion

	#region セットアップ
	public void SetupIcon(LinkedList<BuffInfo> buffInfoList)
	{
		if(this.Attach.buffPopup == null)
			return;
		this.Attach.buffPopup.SetUp(buffInfoList);
	}
	#endregion

	#region OUIItemBase override
	public override void SetActive(bool isLockon, bool isInRange)
	{
		TeamTypeClient teamType = this.ItemRoot.TeamType.GetClientTeam();
		if(teamType == TeamTypeClient.Friend)
		{
			// 味方チームはロックオンしていな状態でも表示する
			base.SetActive(isLockon, isInRange);
		}
		else
		{
			SetActive(isLockon);
		}
	}
	protected override void SetActive(bool isActive)
	{
		if(this.Attach.buffPopup == null)
			return;
		this.Attach.buffPopup.SetActive(isActive);
	}
	#endregion
}
