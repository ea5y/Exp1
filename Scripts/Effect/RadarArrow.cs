using UnityEngine;

public class RadarArrow : MonoBehaviour
{
	#region フィールド＆プロパティ
	const float RenderDistance   = 50f;	// 表示限界距離
	const float RenderScaleValue = 25f;	// 距離に応じた表示スケール係数
	
	private GameObject targetObj;
	private Transform child;
	#endregion

	#region 初期化
	void Awake()
	{
		// arrowモデルを取得
		child = this.gameObject.transform.GetChild(0);
	}
	public void SetTarget(GameObject targetObj)
	{
		this.targetObj = targetObj;
		Update();
	}
	#endregion
	
	#region 更新
	void Update()
	{
		if(targetObj && child)
		{
			Character chara = this.targetObj.GetComponent<Character>();
			if(chara != null && chara.StatusType == Scm.Common.GameParameter.StatusType.Dead)
			{
				// Characterの場合,Dead状態は非表示.
				child.GetComponent<Renderer>().enabled = false;
			}
			else
			{
				Vector3 vec = this.targetObj.transform.position - this.gameObject.transform.position;
				vec.y = 0;
				float distance = vec.magnitude;

				if(distance < RenderDistance)
				{
					// 対象の位置に合わせて回転、距離に応じてスケール.
					this.gameObject.transform.rotation = Quaternion.LookRotation(vec);
					
					float rength = 1f + (distance / RenderScaleValue);
					child.localScale = new Vector3(1f / rength, 1f, rength);
					child.GetComponent<Renderer>().enabled = true;
				}
				else
				{
					// 距離が一定以上離れていたら非表示.
					child.GetComponent<Renderer>().enabled = false;
				}
			}
		}
		else
		{
			// 対象をロストした場合、消滅する.
			Object.Destroy(this.gameObject);
		}
	}
	#endregion
}
