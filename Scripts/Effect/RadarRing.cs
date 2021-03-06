using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RadarRing : MonoBehaviour
{
	#region フィールド＆プロパティ
	static HashSet<EnemyMaker> enemyMarkerList = new HashSet<EnemyMaker>();

//	[SerializeField]
//	private GameObject shadowPrefab;
	[SerializeField]
	private float shadowScale = 1.6f;
	
	static private RadarRing radarRing;
	public GameObject RingObject { get; private set; }
	public GameObject ShadowObject { get; private set; }
	
	private float size = 1f;
	private float Size { get { return size; } set { size = value; } }
	
	Character character;
	#endregion

	#region 初期化
	public void SetupMarker()
	{
		this.character = GetComponent<Character>();
		
		// 影を作成.
		this.ShadowObject = SafeObject.Instantiate(PlayerManager.Instance.ShadowPrefab) as GameObject;
		if (ShadowObject)
		{
			Transform t = ShadowObject.transform;
			t.parent = EffectManager.Instance.transform;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = new Vector3(this.shadowScale, 1f, this.shadowScale);
		}
		
		// リングを作成.
		GameObject raderRingPrefab = PlayerManager.Instance.RaderRingPrefab.GetPrefab(this.character.TeamType.GetClientTeam());
		if(raderRingPrefab == null)
		{
			return;
		}
		this.RingObject = SafeObject.Instantiate(raderRingPrefab) as GameObject;
		if (this.RingObject)
		{
			Transform t = this.RingObject.transform;
			t.parent = EffectManager.Instance.transform;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = new Vector3(this.Size, 1f, this.Size);
		}

		radarRing = this;
		foreach(EnemyMaker enemyMarker in enemyMarkerList)
		{
			if(enemyMarker)
			{
				radarRing.AddArrow(enemyMarker.Enemy.gameObject, enemyMarker.Enemy.TeamType);
			}
			else
			{
				enemyMarkerList.Remove(enemyMarker);
			}
		}
	}

	#endregion
	
	#region 更新
	void LateUpdate()
	{
		if(RingObject)
		{
			Transform rootTransform = character.AvaterModel.RootTransform;
			Quaternion rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0));
			if(rootTransform)
			{
				Vector3 position = new Vector3(
					rootTransform.position.x,
					this.transform.position.y,
					rootTransform.position.z
				);

				this.RingObject.transform.position = position;
				this.RingObject.transform.rotation = rotation;
				this.RingObject.transform.localScale = rootTransform.localScale * Size;

				if(ShadowObject)
				{
					position.y = character.CharacterMove.GroundPosition.y;
					this.ShadowObject.transform.position = position;
					this.ShadowObject.transform.rotation = rotation;
					this.ShadowObject.transform.localScale = rootTransform.localScale * shadowScale;
				}
			}
		}
	}
	#endregion

	#region 破棄
	void OnDestroy()
	{
		GameObject.Destroy(this.RingObject);
		GameObject.Destroy(this.ShadowObject);
	}
	#endregion

	#region 追加
	/// <summary>
	/// インジケータリングの表示対象を登録する.
	/// </summary>
	static public void AddTarget(EnemyMaker enemyMaker)
	{
		enemyMarkerList.Add(enemyMaker);
		if(radarRing)
		{
			radarRing.AddArrow(enemyMaker.Enemy.gameObject, enemyMaker.Enemy.TeamType);
		}
	}
	static public void RemoveTarget(EnemyMaker enemyMaker)
	{
		enemyMarkerList.Remove(enemyMaker);
	}
	private void AddArrow(GameObject targetObj, Scm.Common.GameParameter.TeamType teamType)
	{
		if(targetObj)
		{
			Player player = this.character as Player;
			if(player != null && player.TeamType != teamType)
			{
				GameObject raderArrowPrefab = PlayerManager.Instance.RaderArrowPrefab.GetPrefab(teamType.GetClientTeam());
				if(raderArrowPrefab != null)
				{
					GameObject arrow = SafeObject.Instantiate(raderArrowPrefab) as GameObject;
					if (arrow)
					{
						Transform t = arrow.transform;
						t.parent = this.RingObject.transform;
						t.localPosition = Vector3.zero;
						t.localRotation = Quaternion.identity;
						t.localScale = new Vector3(this.Size, 1f, this.Size);
						
						RadarArrow radarArrow = arrow.AddComponent<RadarArrow>();
						radarArrow.SetTarget(targetObj);
					}
				}
			}
		}
	}
	#endregion
}
