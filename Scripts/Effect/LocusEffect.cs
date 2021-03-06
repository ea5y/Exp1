/// <summary>
/// 軌跡エフェクト
/// 
/// 2013/02/15
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.Master;

public class LocusEffect : MonoBehaviour
{
	#region フィールド＆プロパティ
	[SerializeField]
	private float destroyCounter = 20f;
	public  float DestroyCounter { get { return destroyCounter; } private set { destroyCounter = value; } }

	public Manager Manager { get; private set; }
	public Character Caster { get; private set; }
	public SkillEffectMasterData MasterData { get; private set; }
	public bool IsLoop { get; private set; }
	public bool IsChargeSkill { get; private set; }

	public System.Action UpdateFunc { get; private set; }
	#endregion

	#region セットアップ
	public static bool Setup(GameObject go, Manager manager, ObjectBase caster, SkillEffectMasterData effectData)
	{
		// コンポーネント取得
		LocusEffect effect = go.GetSafeComponent<LocusEffect>();
		if (effect == null)
		{
			if (manager)
				manager.Destroy(go);
			else
				GameObject.Destroy(go);
			return false;
		}
		effect.Manager = manager;
		effect.Caster = caster as Character;
		effect.MasterData = effectData;
		effect.IsLoop = effectData.IsLoop;
		effect.IsChargeSkill = false;
		if (effect.Caster != null)
			effect.IsChargeSkill = effect.Caster.SkillMotionParam.IsChargeSkill;

		// 生成時SE.
		var bornSe = SoundManager.CreateSeObject(go.transform.position, go.transform.rotation, effectData.BornSeFile);
		if(effectData.IsAttach && bornSe != null)
		{
			// 生成時SEを追尾オブジェクトにする.エフェクトと位置を共有.
			TransformChaser.Create(bornSe, go.transform);
		}
		// 生成中SE.エフェクトと位置＆寿命を共有.
		SoundController.AddSeSource(go, effectData.PassSeFile);

		return true;
	}
	void Start()
	{
		if (this.IsLoop)
		{
			this.UpdateFunc = this.UpdateLoop;
		}
		else
		{
			this.UpdateFunc = this.UpdateOnce;
		}
	}
	#endregion

	#region 破棄
	public void Destroy()
	{
		// ロストSE.
		SoundManager.CreateSeObject(this.transform.position, this.transform.rotation, this.MasterData.LostSeFile);

		if (this.Manager)
			this.Manager.Destroy(this.gameObject);
		else
			GameObject.Destroy(this.gameObject);
	}
	void OnDestroy()
	{
		this.Dispose();
	}
	private void Dispose()
	{
		if (this.Manager)
			this.Manager.Destroy(this.gameObject);
	}
	#endregion

	#region 更新
	void Update()
	{
		this.UpdateFunc();
	}
	void UpdateOnce()
	{
		// 子供がいなくなったら消滅
		if (0 >= this.transform.childCount)
			{ this.Destroy(); }
		// 一定時間で消滅
		this.DestroyCounter -= Time.deltaTime;
		if (0f >= this.DestroyCounter)
			{ this.Destroy(); }
	}
	void UpdateLoop()
	{
		// 子供がいなくなったら消滅
		if (0 >= this.transform.childCount)
			{ this.Destroy(); }
		// 一定時間で消滅
		this.DestroyCounter -= Time.deltaTime;
		if (this.Caster == null)
		{
			if (0f >= this.DestroyCounter)
			{
				this.Destroy();
			}
			return;
		}

		// スキルモーションでなければ消滅
		if (this.Caster.State != Character.StateProc.SkillMotion)
		{
			this.Destroy();
		}
		// チャージスキル中のエフェクトなら
		// チャージスキルじゃなくなった瞬間に消滅
		if (this.IsChargeSkill)
		{
			if (!this.Caster.SkillMotionParam.IsChargeSkill)
			{
				this.Destroy();
			}
		}
	}
	#endregion
}
