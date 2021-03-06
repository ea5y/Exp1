/// <summary>
/// プレイヤーのGUI情報
/// 
/// 2014/07/22
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.Packet;
using Scm.Common.GameParameter;
using Scm.Common.XwMaster;
using System.Collections.Generic;

public class GUIBattlePlayerInfo : Singleton<GUIBattlePlayerInfo>
{
	#region アタッチオブジェクト

	[System.Serializable]
	public class AttachObject
	{
		public UILabel nameLabel;
		public UISlider hpBar;
		public UISlider damageBar;
		public UISlider expBar;
		public UILabel lvLabel;
		public GUIBuffPopup buffIconPopup;
	}

	#endregion

	#region フィールド&プロパティ

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	private AttachObject attach;
	public AttachObject Attach { get { return attach; } }
	
	#endregion

	#region 初期化

	protected override void Awake()
	{
		base.Awake();

		// HPとダメージHPバーの表示をOFFにしておく
		// 再参戦時に最大値から再参戦前のHPバーの更新部分が一瞬見てしまうので再参戦前のHPバー値にセットされるまで表示をOFFに
		if(this.Attach.hpBar != null)
		{
			this.Attach.hpBar.gameObject.SetActive(false);
		}
		if(this.Attach.damageBar != null)
		{
			this.Attach.damageBar.gameObject.SetActive(false);
		}
	}

	#endregion

	#region 名前

	/// <summary>
	/// プレイヤー名セット
	/// </summary>
	public static void SetName(string name)
	{
		if(Instance == null)
			return;

		if(Instance.Attach.nameLabel == null)
			return;
		Instance.Attach.nameLabel.text = name;
	}
	
	#endregion

	#region ヒットポイント

	/// <summary>
	/// HPバーの初期セットアップ処理
	/// </summary>
	public static void SetupHP(int hp, int maxHP)
	{
		if(Instance == null)
			return;

		Instance._SetupHP(hp, maxHP);
	}
	private void _SetupHP(int hp, int maxHP)
	{
		if(this.Attach.hpBar == null)
			return;

		// 変化量
		float t = (float)hp / (float)maxHP;

		// HPバーセット
		this.Attach.hpBar.gameObject.SetActive(true);
		this.Attach.hpBar.value = t;

		if(this.Attach.damageBar == null)
			return;

		// HPダメージバーセット
		this.Attach.damageBar.gameObject.SetActive(true);
		this.Attach.damageBar.value = t;
	}

	/// <summary>
	/// HPバー更新
	/// </summary>
	public static void UpdateHP(int hp, int maxHP)
	{
		if(Instance == null)
			return;

		Instance._UpdateHP(hp, maxHP);
	}
	private void _UpdateHP(int hp, int maxHP)
	{
		// SetupHPのHPバー初期セット処理を呼ばない限りは更新されないようにしておく
		if(this.Attach.hpBar == null || !this.Attach.hpBar.gameObject.activeSelf)
			return;

		// HPセット
		float t = (float)hp / (float)maxHP;
		this.Attach.hpBar.value = t;

		// ダメージ変化演出
		DamageBarEffect();
	}

	#region ダメージHPゲージ演出

	/// <summary>
	/// ダメージ変化演出
	/// </summary>
	private void DamageBarEffect()
	{
		if(this.Attach.damageBar == null)
			return;

		// HPゲージ変化演出.
		if(this.Attach.damageBar.value < this.Attach.hpBar.value)
		{
			this.Attach.damageBar.value = this.Attach.hpBar.value;
		}
		else if(this.Attach.damageBar.value > this.Attach.hpBar.value)
		{
			// 変化演出開始
			StartCoroutine(DamageBarEffectCoroutine(this.Attach.hpBar.value));
		}
	}
	IEnumerator DamageBarEffectCoroutine(float targetValue)
	{
		const float EffectSpeed = 1.0f;		// ゲージの減少スピード.
		const float EffectWaitTime = 1.0f;  // 減少開始までの待ち時間.
		
		// 減少前に一定時間待つ.
		yield return new WaitForSeconds(EffectWaitTime);
		
		while(targetValue < this.Attach.damageBar.value)
		{
			this.Attach.damageBar.value -= Time.deltaTime * EffectSpeed;
			
			yield return new WaitForEndOfFrame();
			
			if(targetValue < this.Attach.hpBar.value)
			{
				targetValue = this.Attach.hpBar.value;
			}
		}
		
		this.Attach.damageBar.value = targetValue;
	}

	#endregion

	#region HPバー揺れ演出

	/// <summary>
	/// 揺れ演出
	/// </summary>
	public static void ShakeHpBar()
	{
		if(Instance == null)
			return;

		Instance._ShakeHpBar();
	}

	private bool isShake = false;
	private const float ShakeTime = 1.0f;
	private const float Shake = 3.0f;
	private const float ShakeSpeed = 2.5f;
	private float shakeTime = 0;
	public void _ShakeHpBar()
	{
		// 揺れ時間をセット.
		this.shakeTime = ShakeTime;
		
		if(isShake)
		{
			return;
		}
		
		this.isShake = true;
		StartCoroutine(ShakeHpBarCoroutine());
	}
	IEnumerator ShakeHpBarCoroutine()
	{
		Vector3 position = this.Attach.hpBar.transform.localPosition;
		while(this.shakeTime > 0)
		{
			this.shakeTime -= Time.deltaTime;
			float shakeSin = Mathf.Sin(Time.frameCount * ShakeSpeed) * Shake;
			float shakeCos = Mathf.Cos(Time.frameCount * ShakeSpeed) * Shake;
			this.Attach.hpBar.transform.localPosition = new Vector3(position.x + shakeCos, position.y + shakeSin, position.z);
			yield return 0;
		}
		this.Attach.hpBar.transform.localPosition = position;
		this.isShake = false;
	}

	#endregion
	#endregion

	#region 経験地

	/// <summary>
	/// 経験値セット
	/// </summary>
	public static void SetExp(int exp, int nextExp)
	{
		if(Instance == null)
			return;

		Instance._SetExp(exp, nextExp);
	}
	private void _SetExp(int exp, int nextExp)
	{
		if(this.Attach.expBar == null)
			return;

		if(nextExp > 0)
		{
			float t = (float)exp / nextExp;
			this.Attach.expBar.value = t;
		}
	}

	#endregion

	#region レベル

	/// <summary>
	/// レベルセット
	/// </summary>
	public static void SetLevel(int level)
	{
		if(Instance == null)
			return;

		if(Instance.Attach.lvLabel == null)
			return;
		Instance.Attach.lvLabel.text = string.Format(MasterData.GetText(TextType.TX601_Lv_Display), level);
	}

	#endregion	

	#region バフアイコン

	/// <summary>
	/// バフアイコンセット処理
	/// </summary>
	public static void SetBuffIcon(LinkedList<BuffInfo> buffInfoList)
	{
		if(Instance == null)
			return;

		if(Instance.Attach.buffIconPopup == null)
			return;
		Instance.Attach.buffIconPopup.SetUp(buffInfoList);
	}

	/// <summary>
	/// バフアイコン削除処理
	/// </summary>
	public static void BuffIconClear()
	{
		if(Instance == null)
			return;

		if(Instance.Attach.buffIconPopup == null)
			return;
		Instance.Attach.buffIconPopup.Clear();
	}

	#endregion
}
