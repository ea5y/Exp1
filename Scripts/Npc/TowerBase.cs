/// <summary>
/// タワーベース
/// 
/// 2013/02/18
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.Master;

public abstract class TowerBase : NpcBase
{
	#region フィールド＆プロパティ

	// 経験値処理
	protected override Vector3 ExpEffectOffsetMin { get { return new Vector3(-2.0f, 2.0f, -2.0f); } }
	protected override Vector3 ExpEffectOffsetMax { get { return new Vector3(2.0f, 4.0f, 2.0f); } }

	abstract protected string AnimationDamage { get; }
	abstract protected string AnimationDestroy { get; }

	[SerializeField]
	private string breakObjectName;
	protected string BreakObjectName { get { return breakObjectName; } }

	// ダメージ演出用
	private TowerCore towerCore;
	private DamageFire damageFire;

	// タワーゲージ取得
	GUITowerGauge _towerGauge = null;
	public GUITowerGauge TowerGauge
	{
		get
		{
			if (_towerGauge == null)
				_towerGauge = GUITacticalGauge.GetTowerGauge(this);
			return _towerGauge;
		}
	}

	#region プロパティ変更時に実行するメソッド.
	protected override void OnSetHP()
	{
		base.OnSetHP();
		// UI更新
		//if (TowerGauge != null) TowerGauge.UpdateGauge(base.HitPoint, base.MaxHitPoint);
	}
	#endregion
	#endregion

	#region セットアップ
	IEnumerator Start()
	{
		// GUITacticalGauge を取得するまで待機
		// 無限ループにならないように念の為タイマーを仕込む
		{
			float timer = 5f;
			while (timer > 0f)
			{
				timer -= Time.deltaTime;
				if (this.TowerGauge != null)
				{
					// ゲージ更新
					this.HitPoint = this.HitPoint;
					break;
				}
				yield return null;
			}
		}
	}
	#endregion

	#region ObjectBase Override
	#region ヒットパケット
	public override void Hit(HitInfo hitInfo)
	{
		this.HitTowerBase(hitInfo);
	}
	const float vibrateDistanceSqr = 576f;
	protected void HitTowerBase(HitInfo hitInfo)
	{
		base.HitBase(hitInfo);

		if (hitInfo.hitPoint <= 0)
		{
			Player player = GameController.GetPlayer();
			if (player != null)
			{
				if ((player.transform.position - this.transform.position).sqrMagnitude < vibrateDistanceSqr)
				{
					// カメラを揺らす
					{
						CharacterCamera cc = GameController.CharacterCamera;
						if (cc)
						{
							cc.Shake();
						}
					}
				}
			}
		}
		else
		{
			// ダメージ演出
			if (0 < hitInfo.damage)
			{
				if (this.Animation != null)
				{
					this.Animation.Play(AnimationDamage, 0);
				}
			}
			if (this.towerCore != null)
			{
				this.towerCore.SetDamage(this);
			}
			if (this.damageFire != null)
			{
				this.damageFire.SetDamage(this);
			}
		}
	}
	#endregion

	#region 破壊処理
	protected override void Destroy()
	{
		this.StartCoroutine(DeadCoroutine());	// base.Destroy();はコルーチン終了時に呼ぶ.
	}
	protected IEnumerator DeadCoroutine()
	{
		if (this.Animation)
		{
			this.Animation.Play(AnimationDestroy, 0);
			yield return new WaitForSeconds(this.Animation.GetAnimationLength(AnimationDestroy));
		}
		this.CreateBrakeObject();

		base.Destroy();
	}
	public void CreateBrakeObject()
	{
		EffectManager.Create(this.BreakObjectName, this.transform.position, this.transform.rotation);
	}
	#endregion

	#endregion

	#region ダメージ演出
	public void SetTowerCore(TowerCore towerCore)
	{
		this.towerCore = towerCore;
		if (this.towerCore != null)
		{
			this.towerCore.SetDamage(this);
		}
	}
	public void SetDamageFire(DamageFire damageFire)
	{
		this.damageFire = damageFire;
		if (this.damageFire != null)
		{
			this.damageFire.SetDamage(this);
		}
	}
	#endregion
}
