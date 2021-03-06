/// <summary>
/// キル情報メッセージアイテム
/// 
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.Master;

public class GUIBreakInfoMessageItem : GUIEffectMessageItem
{
	#region アタッチオブジェクト
	
	[System.Serializable]
	public class AttachBreakObject
	{
		[SerializeField]
		private GameObject breakObject;
		public GameObject BreakObject { get { return breakObject; } }
		
		[SerializeField]
		private UILabel killCountLabel;
		public UILabel KillCountLabel { get { return killCountLabel; } }
		
		[SerializeField]
		private UILabel praiseLabel;
		public UILabel PraiseLabel { get { return praiseLabel; } }
		
		[SerializeField]
		private UILabel breakInfoLabel;
		public UILabel BreakInfoLabel { get { return breakInfoLabel; } }
		
		[SerializeField]
		private UISprite iconSprite;
		public UISprite IconSprite { get { return iconSprite; } }
		
		[SerializeField]
		private UISprite deathSprite;
		public UISprite DeathSprite { get { return deathSprite; } }
		
		[SerializeField]
		private UIPlayTween killPlayTween;
		public UIPlayTween KillPlayTween { get { return killPlayTween; } }
		
		[SerializeField]
		private UIPlayTween deadPlayTween;
		public UIPlayTween DeadPlayTween { get { return deadPlayTween; } }
	}
	
	#endregion
	
	#region 定数
	
	/// <summary>
	/// 最大キル数表示
	/// </summary>
	private const int KillCountMax = 99;
	
	#endregion
	
	#region フィールド&プロパティ
	
	/// <summary>
	/// 倒した時に表示するメッセージ表示時間
	/// </summary>
	[SerializeField]
	private float killShowTime;
	
	/// <summary>
	/// 倒された時に表示するメッセージの表示時間
	/// </summary>
	[SerializeField]
	private float deadShowTime;
	
	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	private AttachBreakObject attachBreakObj;
	public AttachBreakObject AttachBreakObj { get { return attachBreakObj; } }
	
	#endregion
	
	#region セットアップ
	
	/// <summary>
	/// プレイヤーが敵を倒した時のセットアップ処理
	/// </summary>
	public void KilledSetup(int killCount, AvatarType enemyType, int skinId, string enemyName, CharaIcon charaIcon)
	{
		// 表示時間セット
		base.time = killShowTime;
		
		// キャラアイコンのアトラスとスプライト名を取得しセットする
		SetCharIcon(enemyType, skinId, charaIcon);
		
		// キルメッセージセット
		SetKillMessage(TextType.TX005_Kill, enemyName);
		
		// 称賛メッセージセット
		if(this.AttachBreakObj.PraiseLabel != null)
		{
			string praiseMsg = MasterData.GetText(TextType.TX028_Good);
			this.AttachBreakObj.PraiseLabel.text = praiseMsg;
		}
		
		// キル数セット
		if(killCount > 0)
		{
			if(this.AttachBreakObj.BreakObject != null)
			{
				this.AttachBreakObj.BreakObject.SetActive(true);
			}
			if(this.AttachBreakObj.KillCountLabel != null)
			{
				this.AttachBreakObj.KillCountLabel.text = Mathf.Min(killCount, KillCountMax).ToString();
			}
		}
		else
		{
			// キル数が0以下の時は表示しない
			if(this.AttachBreakObj.BreakObject != null)
			{
				this.AttachBreakObj.BreakObject.SetActive(false);
			}
		}
		
		// キャラアイコンの上に死亡マークを表示
		if(this.AttachBreakObj.DeathSprite != null)
		{
			this.AttachBreakObj.DeathSprite.gameObject.SetActive(true);
		}
		
		// キルエフェクト(Tween)再生
		if(this.AttachBreakObj.KillPlayTween != null)
		{
			this.AttachBreakObj.KillPlayTween.Play(true);
		}
	}
	
	/// <summary>
	/// プレイヤーが敵に倒された時にのセットアップ処理
	/// </summary>
	public void DeadSetup(AvatarType enemyType, int skinId, string enemyName, CharaIcon charaIcon)
	{
		// 表示時間セット
		base.time = deadShowTime;
		
		// キャラアイコンのアトラスとスプライト名を取得しセットする
		SetCharIcon(enemyType, skinId, charaIcon);
		
		// キルメッセージセット
		SetKillMessage(TextType.TX013_Dead, enemyName);
		
		// 称賛メッセージの表示を非表示に
		if(this.AttachBreakObj.PraiseLabel != null)
		{
			this.AttachBreakObj.PraiseLabel.gameObject.SetActive(false);
		}
		
		// Breakの表示を非表示に
		if(this.AttachBreakObj.BreakObject != null)
		{
			this.AttachBreakObj.BreakObject.SetActive(false);
		}
		
		// 死亡マークの表示を非表示に
		if(this.AttachBreakObj.DeathSprite != null)
		{
			this.AttachBreakObj.DeathSprite.gameObject.SetActive(false);
		}
		
		// 死亡エフェクト(Tween)再生
		if(this.AttachBreakObj.DeadPlayTween != null)
		{
			this.AttachBreakObj.DeadPlayTween.Play(true);
		}
	}
	
	#endregion
	
	#region キャラアイコンのセット
	
	/// <summary>
	/// キャラアイコンのアトラスとスプライト名を取得しセットする
	/// </summary>
	private void SetCharIcon(AvatarType avatarType, int skinId, CharaIcon charaIcon)
	{
		// スプライトのnullチェック
		if(this.AttachBreakObj.IconSprite == null)
			return;
		
		// アトラス・スプライト取得&セット
		if(charaIcon == null)
			return;
		charaIcon.GetIcon(avatarType, skinId, true, SetCharIconSprite);
		
		// アトラス内にアイコンが含まれているかチェック
		if(this.AttachBreakObj.IconSprite.GetAtlasSprite() == null)
		{
			// アトラスとスプライト名が両方共設定されていてスプライトがない場合はエラー扱い
			if (this.AttachBreakObj.IconSprite.atlas != null && !string.IsNullOrEmpty(this.AttachBreakObj.IconSprite.spriteName))
			{
				string msgDebug = string.Format("Sprite Not Found. AvatarType = {0} SpriteName = {1}", avatarType, this.AttachBreakObj.IconSprite.spriteName);
				BugReportController.SaveLogFile(msgDebug);
			}
		}
	}
	
	/// <summary>
	/// スプライトにアトラスとスプライトを設定する
	/// </summary>
	private void SetCharIconSprite(UIAtlas atlas, string spriteName)
	{
		// アトラスとスプライトの設定
		this.AttachBreakObj.IconSprite.atlas = atlas;
		this.AttachBreakObj.IconSprite.spriteName = spriteName;
	}
	
	#endregion
	
	#region キルメッセージセット
	
	/// <summary>
	/// キルメッセージラベルにメッセージをセットする
	/// </summary>
	private void SetKillMessage(TextType textType, string enemyName)
	{
		// ラベルのnullチェック
		if(this.AttachBreakObj.BreakInfoLabel == null)
			return;
		
		// テキストマスターデータからメッセージを取得しラベルにセットする
		string message = MasterData.GetText(textType, new string[] { enemyName });
		this.AttachBreakObj.BreakInfoLabel.text = message;
	}
	
	#endregion
}
