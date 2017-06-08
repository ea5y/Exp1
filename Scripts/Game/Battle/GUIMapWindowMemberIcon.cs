/// <summary>
/// マップウィンドウのメンバーアイコン
/// 
/// 2014/12/05
/// </summary>
using UnityEngine;
using System.Collections;

public class GUIMapWindowMemberIcon : MonoBehaviour
{
	#region フィールド・プロパティ
	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField] AttachObject _attach;
	public AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UILabel	noLabel;
		public UILabel	nameLabel;
		public UISprite charaIconSprite;
		public UILabel	BLvLabel;
		public UILabel	BLvNoLabel;
		public UISprite styleIconSprite;
        public UILabel	breakLabel;
		public UILabel	breakCountLabel;
		public UIPlayTween playerEmphasisTween;

		[Header("ランクアイコン")]
		public UISprite firstRankIcon;
		public UISprite secondRankIcon;
		public UISprite thirdRankIcon;
	}

	/// <summary>
	/// バトルメンバー情報
	/// </summary>
	MemberInfo MemberInfo{ get; set; }

	public int Index{ get;private set; }
	#endregion

	#region 作成 初期化
	public static GUIMapWindowMemberIcon Create( GameObject prefab , Transform parent , int idx )
	{
		// インスタンス化
		var go = SafeObject.Instantiate(prefab) as GameObject;
		if (go == null)
			return null;
		// 名前
		go.name = string.Format("{0}_{1:000}", prefab.name,idx);
		// 親子付け
		go.transform.parent = parent;
		go.transform.localPosition = prefab.transform.localPosition;
		go.transform.localRotation = prefab.transform.localRotation;
		go.transform.localScale = prefab.transform.localScale;

		// コンポーネント取得
		var item = go.GetComponent<GUIMapWindowMemberIcon>();
		if (item == null)
			return null;

		// 値初期化
		item.ClearValue();
		item.Index = idx;

		return item;
	}
	public void Setup( MemberInfo info ,CharaIcon charaIcon )
	{
		this.MemberInfo = info;

		if( info != null )
		{
			// 情報セット

			// 名前
			if (this.Attach.nameLabel != null)
			{
				this.Attach.nameLabel.text = info.name;

				// 自己アピール
				
				PlayerInfo playerInfo = NetworkController.ServerValue != null ? NetworkController.ServerValue.PlayerInfo : null;

				if( playerInfo != null )
				{
					var pt = this.Attach.playerEmphasisTween;
					if( playerInfo == info.avatarInfo )
					{
						if (pt != null)
							pt.Play(true);
					}
					else
					{
						// 止める
						if (pt != null)
						{
							pt.SetTweener((tw) =>
							{
								tw.Sample(0f, true);
								tw.enabled = false;
							});
						}
					}
				}
			}


			// タクティカルID
			if (this.Attach.noLabel != null)
				this.Attach.noLabel.text = info.tacticalId.ToString();

			if( info.avatarInfo != null )
			{
				// バトル中のレベル
				if (this.Attach.BLvNoLabel != null)
					this.Attach.BLvNoLabel.text = info.avatarInfo.Level.ToString();

				// ブレイク数ラベル
				if (this.Attach.breakCountLabel != null)
					this.Attach.breakCountLabel.text =  string.Format( MasterData.GetText(TextType.TX072_MW_ItemBreakCount), info.avatarInfo.KillCount.ToString());

				// ランクアイコン
				SetRankIcon(info.avatarInfo.ScoreRank);
			}

			// デバッグ用パラメータセットアップ
			DebugSetup(info);

			this.SetIconSprite(charaIcon);
		}

		this.gameObject.SetActive(true);
	}

	[System.Diagnostics.Conditional("XW_DEBUG")]
	void DebugSetup(MemberInfo info)
	{

#if UNITY_EDITOR && XW_DEBUG	

		if (GUIMapWindow.Instance != null && info.avatarInfo == null)
		{
			GUIMapWindow.DebugIconParam debugIconParam = null;

			// 情報取得
			if( info.teamType ==  TeamTypeClient.Enemy )
			{
				if( this.Index < GUIMapWindow.Instance.DebugParam.debugIcon.enemyIconParams.Count )
					debugIconParam = GUIMapWindow.Instance.DebugParam.debugIcon.enemyIconParams[this.Index];
			}
			else if( info.teamType == TeamTypeClient.Friend )
			{
				if( this.Index < GUIMapWindow.Instance.DebugParam.debugIcon.friendIconParams.Count )
					debugIconParam = GUIMapWindow.Instance.DebugParam.debugIcon.friendIconParams[this.Index];
			}

			if( debugIconParam == null )
				return;

			// バトル中のレベル
			if (this.Attach.BLvNoLabel != null)
				this.Attach.BLvNoLabel.text = debugIconParam.lv.ToString();

			// ブレイク数ラベル
			if (this.Attach.breakCountLabel != null)
				this.Attach.breakCountLabel.text = string.Format( MasterData.GetText(TextType.TX072_MW_ItemBreakCount), debugIconParam.breakCount.ToString());

			// ランクアイコン
			SetRankIcon(debugIconParam.rank);
		}
#endif

	}

	public void ClearValue()
	{
		if( this.Attach.nameLabel != null )
			this.Attach.nameLabel.text = "";

		if( this.Attach.noLabel != null )
			this.Attach.noLabel.text = "";

		if( this.Attach.BLvNoLabel != null )
			this.Attach.BLvNoLabel.text = "";

		if( this.Attach.styleIconSprite != null )
			this.Attach.styleIconSprite.gameObject.SetActive(false);

		if (this.Attach.breakCountLabel != null)
			this.Attach.breakCountLabel.text = "";

		SetRankIcon(0);
	}
	#endregion

	#region キャラアイコン設定
	void SetIconSprite(CharaIcon charaIcon)
	{
		if(this.MemberInfo.avatarInfo != null && this.MemberInfo.avatarInfo.GameObject != null)
		{
			if (charaIcon != null )
			{
				charaIcon.GetIcon(this.MemberInfo.avatarType, this.MemberInfo.skinId, false, this.SetIconSprite);
			}
		}
		else
		{
			this.UnsetIconSprite();
		}
	}
	void SetIconSprite(UIAtlas atlas , string spriteName)
	{
		var sp = this.Attach.charaIconSprite;
		if( sp != null )
		{
			// アトラス設定
			sp.atlas = atlas;
			// スプライト設定
			sp.spriteName = spriteName;

			// アトラス内にアイコンが含まれているかチェック
			if (sp.GetAtlasSprite() == null)
			{
				// アトラスとスプライト名が両方共設定されていてスプライトがない場合はエラー扱いｌ
				if (atlas != null && !string.IsNullOrEmpty(spriteName))
				{
					Debug.LogWarning(string.Format(
						"SetIconSprite:\r\n" +
						"Sprite Not Found!! AvatarType = {0} SpriteName = {1}", this.MemberInfo.avatarType, spriteName));
				}
			}

			if(!sp.enabled)
			{
				sp.enabled = true;
			}
		}
	}
	void UnsetIconSprite()
	{
		var sp = this.Attach.charaIconSprite;
		if( sp != null && sp.enabled)
		{
			sp.enabled = false;
		}
	}
	#endregion


	#region スタイルアイコン設定
	void SetStyle(AvatarType avatarType)
	{
		// スタイル情報を取得する
		StyleInfo info = null;
		bool isEnable = false;

		if (ObsolateSrc.TryGetStyleInfo(avatarType, out info))
		{
			isEnable = true;
		}

		// スタイルアイコン設定
		var sp = this.Attach.styleIconSprite;
		if (sp == null)
		{
			Debug.LogWarning("StyleSprite Non Attach!");
		}
		else
		{
			// スタイルアイコン表示設定
			sp.gameObject.SetActive(isEnable);
			if (isEnable)
			{
				// スプライト変更
				// 同じアトラス内にあることを前提とする
				if (info != null)
				{
					sp.spriteName = info.iconName;

					// アトラス内にアイコンが含まれているかチェック
					if (sp.GetAtlasSprite() == null)
					{
						// アトラスとスプライト名が両方共設定されていてスプライトがない場合はエラー扱いｌ
						if (sp.atlas != null && !string.IsNullOrEmpty(info.iconName))
						{
							Debug.LogWarning(string.Format(
								"SetStyle:\r\n" +
								"Sprite Not Found!! StyleType = {0} SpriteName = {1}", info.type, info.iconName));
						}
					}
				}
			}
		}
	}
	#endregion 


	#region ランクアイコン設定
	void SetRankIcon( int rank )
	{
		switch(rank)
		{
		case 1:
			{
				this.Attach.firstRankIcon.gameObject.SetActive(true);
				this.Attach.secondRankIcon.gameObject.SetActive(false);
				this.Attach.thirdRankIcon.gameObject.SetActive(false);
			}
			break;

		case 2:
			{
				this.Attach.firstRankIcon.gameObject.SetActive(false);
				this.Attach.secondRankIcon.gameObject.SetActive(true);
				this.Attach.thirdRankIcon.gameObject.SetActive(false);
			}
			break;

		case 3:
			{
				this.Attach.firstRankIcon.gameObject.SetActive(false);
				this.Attach.secondRankIcon.gameObject.SetActive(false);
				this.Attach.thirdRankIcon.gameObject.SetActive(true);
			}
			break;
		default:
			{
				this.Attach.firstRankIcon.gameObject.SetActive(false);
				this.Attach.secondRankIcon.gameObject.SetActive(false);
				this.Attach.thirdRankIcon.gameObject.SetActive(false);
			}
			break;
		}
	}
	#endregion
}
