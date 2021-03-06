/// <summary>
/// リザルトのチーム情報
/// 
/// 2014/11/12
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIResultTeamInfoOld : MonoBehaviour
{
	#region エフェクト状態タイプ

	public enum EffectStateType
	{
		TeamItem,
		Judge,

		None = -1
	}

	#endregion

	#region フィールド&プロパティ

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public GUIResultInfoItem friendItemPrefab;
		public UITable friendTable;
		public GUIResultInfoItem enemyItemPrefab;
		public UITable enemyTable;
		public GameObject playerWinObject;
		public GameObject playerLoseObject;
		public GameObject playerDrawObject;
		public GameObject backButtonParent;
		public GameObject exitButtonParent;
		public UIPlayTween teamPlayTween;
		public UIPlayTween JudgePlayTween;
		public Transform charaBoardParent;		// キャラボードを生成する親
		public TweenPosition charaBoardTween;	// キャラボードを制御するTween
	}

	/// <summary>
	/// プレイヤーの勝敗結果
	/// </summary>
	private JudgeTypeClient resultJudgeType;

	/// <summary>
	/// エフェクトのスキップが可能かどうか
	/// </summary>
	private bool isSkip;

	/// <summary>
	/// エフェクトの状態
	/// </summary>
	private EffectStateType effectState = EffectStateType.None;
	
	/// <summary>
	/// エフェクト更新用
	/// </summary>
	private Fiber effectFiber;

	/// <summary>
	/// プレイヤーのアバタータイプ
	/// </summary>
	private AvatarType playerAvatarType = AvatarType.None;

    /// <summary>
    /// Skin id
    /// </summary>
    private int skinId = 0;

    #endregion

    #region 初期化
    void Awake()
	{
		// エフェクトのスキップフラグをOFFに設定
		this.isSkip = false;
	}
	#endregion

	#region セットアップ
	/// <summary>
	/// チームリザルトの情報をセットアップする
	/// </summary>
	public void Setup(List<MemberInfo> infoList, int playerID, JudgeTypeClient resultJudgeType)
	{
		// プレイヤーの勝敗結果をセット
		this.resultJudgeType = resultJudgeType;

		if(this.Attach.friendTable == null || this.Attach.enemyTable == null)
			return;
		if(this.Attach.friendItemPrefab == null || this.Attach.enemyItemPrefab == null)
			return;

		// テーブル内のアイテムを全削除
		TeamTableClear();

		// アイテムの生成
		// TODO;各チームごとにアイテムを5つまで生成するようにする(いずれアイテムをスクロールさせる予定)
		int friendCount = 0;
		int enemyCount = 0;
		foreach(MemberInfo info in infoList)
		{
			if(info.teamType == TeamTypeClient.Friend)
			{
				// 味方チーム
				GUIResultInfoItem.Create(this.Attach.friendItemPrefab, this.Attach.friendTable.transform, info, playerID, friendCount);
				friendCount++;
			}
			else if(info.teamType == TeamTypeClient.Enemy)
			{
				// 敵チーム
				GUIResultInfoItem.Create(this.Attach.enemyItemPrefab, this.Attach.enemyTable.transform, info, playerID, enemyCount);
				enemyCount++;
			}

			if(info.inFieldID == playerID)
			{
				this.playerAvatarType = info.avatarType;
                this.skinId = info.skinId;
			}
		}
		// テーブル整形する
		TeameTableReposition();

		// リザルト情報が空ならボタンを表示存在するならボタンを非表示にする
		bool isActive = infoList.Count > 0 ? false : true;
		if (this.Attach.exitButtonParent != null)
		{
			this.Attach.exitButtonParent.SetActive(isActive);
		}
		//if (this.Attach.backButtonParent != null)
		//{
		//	this.Attach.backButtonParent.SetActive(isActive);
		//}
		// 2016/06/07売り切り版仕様
		// 戻るボタンは強制的に非表示
		this.Attach.backButtonParent.SetActive(false);
	}
	#endregion

	#region テーブル制御
	/// <summary>
	/// 味方と敵チームのテーブル内のアイテムを全削除する
	/// </summary>
	private void TeamTableClear()
	{
		// 味方テーブル内のアイテムを全削除
		Transform transform = this.Attach.friendTable.transform;
		for (int i=0, max=transform.childCount; i<max; i++)
		{
			var child = transform.GetChild(i);
			Object.Destroy(child.gameObject);
		}

		// 敵テーブル内のアイテムを全削除
		transform = this.Attach.enemyTable.transform;
		for (int i=0, max=transform.childCount; i<max; i++)
		{
			var child = transform.GetChild(i);
			Object.Destroy(child.gameObject);
		}
	}

	/// <summary>
	/// 味方と敵チームのテーブルを整形する
	/// </summary>
	private void TeameTableReposition()
	{
		this.Attach.friendTable.Reposition();
		this.Attach.enemyTable.Reposition();
	}

	/// <summary>
	/// 勝敗オブジェクト(スプライトやエフェクト)の設定を行う
	/// </summary>
	public GameObject GetJudgeObject(JudgeTypeClient judgeType)
	{
		if(this.Attach.playerWinObject == null || this.Attach.playerLoseObject == null || this.Attach.playerDrawObject == null)
			return null;
		
		GameObject judgeObject = null;
		switch(judgeType)
		{
			case JudgeTypeClient.PlayerCompleteWin:
			case JudgeTypeClient.PlayerWin:
			{
				judgeObject = this.Attach.playerWinObject;
				break;
			}
			case JudgeTypeClient.PlayerCompleteLose:
			case JudgeTypeClient.PlayerLose:
			{
				judgeObject = this.Attach.playerLoseObject;
				break;
			}
			case JudgeTypeClient.Draw:
			{
				judgeObject = this.Attach.playerDrawObject;
				break;
			}
			// 勝敗が存在しない(主にデバッグでリザルトに強制的に移った時になど)
			default:
			{
				judgeObject = null;
				break;
			}
		}
		
		return judgeObject;
	}
	#endregion
	
	#region 更新
	void Update()
	{
		if(this.effectFiber != null && !this.effectFiber.IsFinished)
		{
			// エフェクト更新
			this.effectFiber.Update();
		}
	}
	#endregion
	
	#region エフェクト開始処理
	/// <summary>
	/// エフェクトの再生を開始させる
	/// </summary>
	public void playEffect()
	{
		// 2016/06/07 売り切り版
		// キャラボード読み込み
		if (this.Attach.charaBoardParent != null && this.Attach.charaBoardTween != null)
		{
			// キャラボードを生成してセットアップ
			CharaBoard charaBoard = GUIResultOld.CharaBoard;
			if (charaBoard != null)
			{
				charaBoard.GetBoard(this.playerAvatarType, this.skinId, false,
										 (GameObject resource) => { SetupCharaBoard(resource, this.Attach.charaBoardParent, this.Attach.charaBoardTween); });
			}
		}

		if(this.Attach.exitButtonParent != null)
		{
			if(this.Attach.exitButtonParent.activeSelf)
			{
				// ボタンが表示している状態ならエフェクトが1度再生されている状態なのでエフェクトの再生を行わない
				EndEffect();
				return;
			}
		}

		// エフェクト状態変更
		ChangeEffectState(EffectStateType.TeamItem);
		
		// エフェクトのスキップを可能状態に
		this.isSkip = true;
	}

	/// <summary>
	/// キャラボードのセットアップ
	/// </summary>
	private void SetupCharaBoard(GameObject resource, Transform parent, TweenPosition tweenPosition)
	{
		// リソース読み込み完了
		if (resource == null) return;
		// インスタンス化
		var go = SafeObject.Instantiate(resource) as GameObject;
		if (go == null) return;

		// 名前設定
		go.name = resource.name;
		// 親子付け
		var t = go.transform;
		t.parent = parent;
		t.localPosition = Vector3.zero;
		t.localRotation = Quaternion.identity;
		t.localScale = Vector3.one;

		// 読み込みが完了してから再生を開始する
		tweenPosition.Play(true);
	}
	#endregion

	#region 各エフェクト再生
	/// <summary>
	/// 各チーム情報のエフェクト再生
	/// </summary>
	private IEnumerator PlayTeamEffect()
	{
		if(this.Attach.teamPlayTween == null) yield break;
		this.Attach.teamPlayTween.Play(true);
	}

	/// <summary>
	/// 勝敗エフェクト再生
	/// </summary>
	private IEnumerator PlayJudgeEffect()
	{
		if(this.Attach.JudgePlayTween == null) yield break;
		this.Attach.JudgePlayTween.Play(true);
	}
	#endregion

	#region 各エフェクト終了
	/// <summary>
	/// チーム情報エフェクト終了処理(NGUIリフレクション)
	/// </summary>
	public void TeamEffectFinished()
	{
		// プレイヤーの勝敗に適したオブジェクトを取得
		GameObject judgeObj = GetJudgeObject(this.resultJudgeType);
		if(judgeObj != null)
		{
			// 表示
			judgeObj.SetActive(true);
			// 勝敗オブジェクトの演出に切り替える
			ChangeEffectState(EffectStateType.Judge);
		}
		else
		{
			// 勝敗オブジェクトのセットに失敗した時はエフェクトを終了させる
			EndEffect();
		}
	}

	/// <summary>
	/// 勝敗エフェクト終了処理(NGUIリフレクション)
	/// </summary>
	public void JudgeEffectFinished()
	{
		// 全エフェクト再生終了
		EndEffect();
	}

	/// <summary>
	/// 全てのエフェクトが終了した時の処理
	/// </summary>
	private void EndEffect()
	{
		/// スキップが可能なエフェクトの再生が終わったのでスキップ処理を行わないようにする
		this.isSkip = false;

		// ボタン機能ONに
		if(this.Attach.exitButtonParent != null)
		{
			this.Attach.exitButtonParent.SetActive(true);
		}
		// 2016/06/07売り切り版仕様
		//if(this.Attach.backButtonParent != null)
		//{
		//	this.Attach.backButtonParent.SetActive(true);
		//}
	}
	#endregion

	#region エフェクト状態変更
	/// <summary>
	/// エフェクト状態の変更処理
	/// </summary>
	private void ChangeEffectState(EffectStateType state)
	{
		// 前回と同じエフェクトは再生させない(エフェクトのリプレイは考慮していないため)
		if(this.effectState == state) return;

		switch(state)
		{
			case EffectStateType.TeamItem:
			{
				this.effectFiber = new Fiber(PlayTeamEffect());
				break;
			}
			case EffectStateType.Judge:
			{
				this.effectFiber = new Fiber(PlayJudgeEffect());
				break;
			}
		}
		this.effectState = state;
	}
	#endregion

	#region エフェクトスキップ
	/// <summary>
	/// エフェクトをスキップさせる
	/// </summary>
	public void SkipEffect()
	{
		// スキップが可能な状態か
		if(!this.isSkip) return;

		// 各UIPlayTweenの再生を終了させる
		// エフェクトを再再生させる必要があるなら初期の値を再度セットする必要性あり
		UIPlayTween playTween = this.Attach.teamPlayTween;
		if(playTween != null)
		{
			playTween.SetTweener(SkipTweener);
		}

		// プレイヤーの勝敗に適したオブジェクトを取得する
		GameObject judgeObj = GetJudgeObject(this.resultJudgeType);
		if(judgeObj != null)
		{
			bool isActive = judgeObj.activeSelf;
			// TweenスキップのためにアクティブをONにする
			judgeObj.SetActive(true);
			// スキップ処理
			playTween = this.Attach.JudgePlayTween;
			if(playTween != null)
			{
				playTween.SetTweener(SkipTweener);
			}
			// アクティブを元の状態に戻す
			judgeObj.SetActive(isActive);
		}
		this.isSkip = false;
	}
	
	/// <summary>
	/// UIPlayTweenで再生させているTween系の
	/// Duration値とdelay値を0にしTweenの再生を終了させる
	/// </summary>
	private void SkipTweener(UITweener tweener)
	{
		tweener.duration = 0f;
		tweener.delay = 0f;
	}
	#endregion
}
