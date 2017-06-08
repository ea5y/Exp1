/// <summary>
/// ウォールベース
/// 
/// 2013/03/11
/// </summary>
using UnityEngine;
using Scm.Common.Master;

public class WallBase : Gadget
{
	#region 経験値処理
	protected override Vector3 ExpEffectOffsetMin{ get{ return new Vector3(0.0f, 1.5f, 0.0f); } }
	protected override Vector3 ExpEffectOffsetMax{ get{ return new Vector3(0.0f, 1.5f, 0.0f); } }
	#endregion
}
