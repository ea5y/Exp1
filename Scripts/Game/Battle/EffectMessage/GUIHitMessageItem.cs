/// <summary>
/// ヒット/コンボメッセージ
/// 
/// 2015/01/23
/// </summary>
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;

public class GUIHitMessageItem : GUIEffectMsgItemBase
{
	#region 定数

	/// <summary>
	/// コンボが継続時していられる時間
	/// </summary>
	private const float KeepTime = 1.5f;

	/// <summary>
	/// コンボ最大数
	/// </summary>
	private const int ComboMax = 99;

	#endregion

	#region アタッチオブジェクト
	
	[System.Serializable]
	public class HitAttachObject
	{
		public UILabel countLabel;
		public UILabel hitLabel;
		public float praiseEffectTime;
		public List<Effect> effectList;
		public UIPlayTween startPlayTween;
		public UIPlayTween hitPlayTween;
		public UIPlayTween deletePlayTween;
	}
	
	#endregion

	#region エフェクト

	// 段階ごとに設定するエフェクト
	[System.Serializable]
	public class Effect
	{
		public EffectLabel countLabel;
		public EffectLabel hitLabel;
		public GameObject praiseParent;
		public UILabel praiseLabel;
	}

	// エフェクト時に設定するラベルの項目
	[System.Serializable]
	public class EffectLabel
	{
		public Gradient gradient;
		public Color effectColor;
		public GameObject effectParent;
	}

	// ラベルのグラデーションカラー設定用
	[System.Serializable]
	public class Gradient
	{
		public Color top;
		public Color bottom;
	}

	#endregion

	#region フィールド&プロパティ

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	private HitAttachObject hitAttach;
	protected HitAttachObject HitAttach { get { return hitAttach; } }

	/// <summary>
	/// 計測用
	/// </summary>
	protected float time;
	
	/// <summary>
	/// ヒット総数
	/// </summary>
	private int hitCount;

	/// <summary>
	/// コンボ中かどうか
	/// </summary>
	public bool IsCombo { get; private set; }

	/// <summary>
	/// 各時間更新用
	/// </summary>
	private Action timeUpdate = ()=>{};

	#endregion

	#region セットアップ

	public void Setup()
	{
		// ヒット数カウントアップさせ表示する
		if(this.HitAttach.countLabel == null || this.HitAttach.hitLabel == null) return;
		this.hitCount = Mathf.Min(this.hitCount+1, ComboMax);
		this.HitAttach.countLabel.text = string.Format("{0}", hitCount);

		// 段階ごとにエフェクトを設定する
		ComboPraiseMasterData masterData;
		if(MasterData.TryGetComboPraise(this.hitCount, out masterData))
		{
			int effectLevel = masterData.EffectLevel-1;
			SetLabelColor(this.HitAttach.countLabel, this.HitAttach.effectList[effectLevel].countLabel);
			SetLabelColor(this.HitAttach.hitLabel, this.HitAttach.effectList[effectLevel].hitLabel);
		}

		// コンボ時間をリセット
		this.time = KeepTime;

		if(this.hitCount <= 1)
		{
			// 開始エフェクト再生
			PlayStartEffect();
		}

		// ヒットエフェクト再生
		PlayHitEffect();

		this.IsCombo = true;
		this.timeUpdate = ComboTimeUpdate;
	}

	/// <summary>
	/// ラベルの色を設定する
	/// </summary>
	private void SetLabelColor(UILabel label, EffectLabel effectLabel)
	{
		// すでに表示状態の場合は設定をする必要がない
		if(effectLabel.effectParent.activeSelf) return;

		label.gradientTop = effectLabel.gradient.top;
		label.gradientBottom = effectLabel.gradient.bottom;
		label.effectColor = effectLabel.effectColor;
		effectLabel.effectParent.SetActive(true);
	}

	#endregion

	#region 更新
	
	protected override void Update ()
	{
		// 時間更新
		this.timeUpdate();
	}
	
	/// <summary>
	/// コンボ時間更新
	/// </summary>
	private void ComboTimeUpdate()
	{
		this.time -= Time.deltaTime;
		if(this.time < 0)
		{
			// コンボ更新終了処理
			ComboUpdateFinsh();
		}
	}

	/// <summary>
	/// コンボ更新終了処理
	/// </summary>
	private void ComboUpdateFinsh()
	{
		// コンボ終了にセット
		this.IsCombo = false;

		ComboPraiseMasterData masterData;
		if(MasterData.TryGetComboPraise(this.hitCount, out masterData))
		{
			// 賞賛セット
			SetupPraise(masterData);
			
			// 賞賛エフェクト更新処理をセット
			this.time = this.HitAttach.praiseEffectTime;
			this.timeUpdate = PraiseEffectUpdate;
		}
		else
		{
			// マスターデータが見つからない場合は賞賛文字を表示する必要性がないので賞賛エフェクト終了処理を行う
			PraiseEffectFinsh();
		}
	}
	
	/// <summary>
	/// 賞賛エフェクト更新
	/// </summary>
	private void PraiseEffectUpdate()
	{
		this.time -= Time.deltaTime;
		if(this.time < 0)
		{
			// 賞賛エフェクト終了処理
			PraiseEffectFinsh();
		}
	}

	/// <summary>
	/// 賞賛エフェクト終了処理
	/// </summary>
	private void PraiseEffectFinsh()
	{
		// 賞賛エフェクト終了したので削除エフェクト再生
		PlayDeleteEffect();
		
		// 時間更新終了
		this.timeUpdate = ()=>{};
	}
	
	#endregion

	#region 賞賛のセット

	/// <summary>
	/// コンボ数によって賞賛文字をセットする
	/// </summary>
	private void SetupPraise(ComboPraiseMasterData masterdata)
	{
		int effectLevel = masterdata.EffectLevel-1;

		GameObject praiseParent = this.HitAttach.effectList[effectLevel].praiseParent;
		if(praiseParent != null)
		{
			// 表示
			praiseParent.SetActive(true);
		}
		UILabel praiseLabel = this.HitAttach.effectList[effectLevel].praiseLabel;
		if(praiseLabel != null)
		{
            // 賞賛文字セット
#if PLATE_NUMBER_REVIEW
            praiseLabel.text = Scm.Common.Utility.Language == Scm.Common.GameParameter.Language.ChineseSimplified ? masterdata.PraiseMessageChn : masterdata.PraiseMessage;
#else
            praiseLabel.text = masterdata.PraiseMessage;
#endif
        }
    }

#endregion

#region エフェクト再生

	/// <summary>
	/// 生成時のエフェクト再生
	/// </summary>
	private void PlayStartEffect()
	{
		if(this.hitAttach.startPlayTween == null) return;
		this.HitAttach.startPlayTween.Play(true);
	}

	/// <summary>
	/// ヒットごとに再生させるエフェクト
	/// </summary>
	private void PlayHitEffect()
	{
		if(this.HitAttach.hitPlayTween == null) return;
		this.HitAttach.hitPlayTween.Play(true);
	}

	/// <summary>
	/// 削除エフェクト再生
	/// </summary>
	private void PlayDeleteEffect()
	{
		if(this.HitAttach.deletePlayTween == null) return;
		this.HitAttach.deletePlayTween.Play(true);
	}

#endregion

#region エフェクト終了(NGUIリフレクション)

	/// <summary>
	/// 削除エフェクト終了後に呼ばれる
	/// </summary>
	public void DeleteEffectFinished()
	{
		// 削除フラグをONに設定
		base.IsDelete = true;
	}

#endregion
}
