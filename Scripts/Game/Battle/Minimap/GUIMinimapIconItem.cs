/// <summary>
/// ミニマップアイコンアイテム
/// 
/// 2014/10/30
/// </summary>
using UnityEngine;
using System;
using System.Collections;

using Scm.Common.GameParameter;

public class GUIMinimapIconItem : MonoBehaviour
{
	#region フィールド＆プロパティ
	/// <summary>
	/// 味方アイコンの色
	/// </summary>
	[SerializeField]
	ColorType _myteamColorType;
	ColorType MyteamColorType { get { return _myteamColorType; } }

	/// <summary>
	/// 敵アイコンの色
	/// </summary>
	[SerializeField]
	ColorType _enemyColorType;
	ColorType EnemyColorType { get { return _enemyColorType; } }

	/// <summary>
	/// 中立アイコンの色
	/// </summary>
	[SerializeField]
	ColorType _unknownColorType;
	ColorType UnknownColorType { get { return _unknownColorType; } }

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UIPlayTween createPlayTween;	// 作成時アニメーション
		public UIPlayTween deathPlayTween;	// 死亡時アニメーション
		public UIPlayTween damagePlayTween;	// ダメージアニメーション
		public UIPlayTween targetPlayTween;	// ターゲットマーカー時のアニメーション
		public UIPlayTween respawnBlinkTween; // リスポーン地点選択時のアニメーション
		public Transform iconRoot;		// アイコンルート
		public Transform rotationRoot;	// 回転情報更新
		public Transform positionRoot;	// 移動情報更新
		public UISprite hpSprite;		// HPゲージ更新
		public UILabel numberLabel;		// 認識番号
		public UISprite cameraSprite;	// カメラ更新
		public UISprite arrowSprite;	// MapWindow外に出た時の矢印表示スプライト
		public UISprite respawnBlinkSprite; // リスポーン地点の点滅スプライト
		public UIDragScrollView dragScrollView;	// MapWindowでのミニマップ拡縮時にドラッグ処理を無効化するのに使う
		public BoxCollider collider;
		public bool isAdjustMapScale;		// ミニマップの縮尺にアイコンスプライトのスケールを合わせるか
	}

	/// <summary>
	/// アイコンをぶら下げるためのルートオブジェクト
	/// </summary>
	public Transform IconRoot { get { return this.Attach.iconRoot; } }

	// アイコンと対応する ObjectBase
	ObjectBase ObjectBase { get; set; }
	// アイコンスプライト
	UISprite IconSprite { get; set; }
	// 死亡アニメーションフラグ
	bool IsDeathAnimation { get; set; }

	// MapWindow内のマップパネル外に出た時にパネル内に留まるかどうかのフラグ
	public bool IsGuide { get { return this.Attach.arrowSprite != null; } }
	// 現在MapWindow内のマップパネル内に留まる処理をしているかどうか
	bool IsGuideExecute { get { return ( this.Attach.arrowSprite != null && this.Attach.arrowSprite.gameObject.activeSelf ); } }
	// 画面外に出た時に出る誘導アイコンの距離によるスケール最小値
	const float DistanceMinScale = 0.4f;
	// 画面外に出た時に出る誘導アイコンの距離によるスケール倍率
	const float DistanceScaleMag = 0.4f;
	// Setup時のローカルスケール
	Vector3 DefaultLocalScale{get;set;}

	// 前のフレームのマップウィンドウの状態
	GUIMapWindow.MapMode LastMapWindowMode{get;set;}

	// MapWindowの各モードに対応した実行アクション
	System.Action MapWindowModeUpdate{get;set;}

	// MapWindowのモード変化時の終了処理
	System.Action MapWindowModeExit{get;set;}

	// 前フレームのパラメータのキャッシュ

	// アイコンと対応するオブジェクトの位置
	Vector3 LastObjPosition{get;set;}
	// アイコンと対応するオブジェクトの角度
	Quaternion LastObjAngle{get;set;}
	// カメラ角度
	Quaternion LastCameraAngle{get;set;}
	// 最大HP
	int LastMaxHP{get;set;}
	// 現在HP
	int LastNowHP{get;set;}

	// アイコンと対応する ObjectBase の Transform キャッシュ 
	Transform ObjBaseTrans{get;set;}
	#endregion

	#region 初期化

	void Awake()
	{
		this.MapWindowModeUpdate = ()=>{};
		this.MapWindowModeExit = ()=>{};
	}

	void Start()
	{
		this.CreateAnimation();
		GUIMapWindow.ChangeModeEvent += this.ChangeMapWindowMode;
	}
	public void Setup(ObjectBase o, UISprite iconSprite)
	{
		// オブジェクトベース取得
		if (o == null)
			return;
		this.ObjectBase = o;
		this.IconSprite = iconSprite;
		this.name = o.name;

		// デプス設定 MapWindowのコリジョンと競合したのでコメントアウト
		//{
		//	// プレイヤーじゃなければ InFieldID を元にアイコンのデプスを調整する
		//	var player = GameController.GetPlayer();
		//	if (o != player)
		//	{
		//		var widgets = this.gameObject.GetComponentsInChildren<UIWidget>();
		//		foreach (var w in widgets)
		//			w.depth -= o.InFieldId * 10;
		//	}
		//}

		// カラー設定
		Color color = Color.white;
		{
			ColorType colorType;
			switch (o.TeamType.GetClientTeam())
			{
			case TeamTypeClient.Friend:
				colorType = this.MyteamColorType;
				break;
			case TeamTypeClient.Enemy:
				colorType = this.EnemyColorType;
				break;
			default:
				colorType = this.UnknownColorType;
				break;
			}
			color = MasterData.GetColor(colorType);
		}

		// アイコン設定
		var icon = this.IconSprite;
		if (icon != null)
		{
			icon.color = color;
		}

		// 識別番号設定
		var number = this.Attach.numberLabel;
		if (number != null)
		{
			string text = string.Empty;
			switch (o.EntrantType)
			{
			case EntrantType.Pc:
				text = o.TacticalId.ToString();
				break;
			case EntrantType.SubTower:
				text = Convert.ToChar(o.TacticalId + 0x40).ToString();
				break;
			default:
				text = o.TacticalId.ToString();
				break;
			}
			number.text = text;
		}

		// HP設定
		var hp = this.Attach.hpSprite;
		if (hp != null)
		{
			hp.color = color;
		}

		// スケール取得
		DefaultLocalScale = this.transform.localScale;

		// マップが回転して向きが変な場合があるため回転を0にする
		transform.rotation = Quaternion.identity;

		// ObjectBaseのTransformをキャッシュしとく
		this.ObjBaseTrans = this.ObjectBase.transform;


		// リスポーンアイコンは状況に応じて非表示にする
		if( this.ObjectBase.EntrantType == EntrantType.Respawn && this.ObjectBase.TeamType == PlayerManager.Instance.PlayerTeamType )
		{
			var mode = GUIMapWindow.Mode;
			if( mode != GUIMapWindow.MapMode.Respawn && mode != GUIMapWindow.MapMode.Transport )
				this.Attach.iconRoot.gameObject.SetActive(false);
		}
		
		this.ChangeMapWindowMode(GUIMapWindow.Mode);
	}
	#endregion

	#region 更新
	void Update()
	{
		// 対象がいないか死亡状態ならアイコン破棄
		if (this.ObjectBase == null || (this.ObjectBase != null && this.ObjectBase.StatusType == StatusType.Dead) )
		{
			// 死亡アニメーション中じゃなければ削除する
			if (!this.IsDeathAnimation)
			{
				// 元になる ObjectBase が居ないためアイコン削除
				Destroy(this.gameObject);
			}
			return;
		}

		// 位置更新
		if ( this.LastObjPosition != this.ObjBaseTrans.localPosition)
		{
			var posTrans = this.Attach.positionRoot;
			if( posTrans != null )
			{
				Vector3 position = Vector3.zero;
				position.x = this.ObjBaseTrans.localPosition.x * GUIMinimap.Scale.x;
				position.y = this.ObjBaseTrans.localPosition.z * GUIMinimap.Scale.y;
				position.z = posTrans.localPosition.z;
				posTrans.localPosition = position;
			}

			this.LastObjPosition = this.ObjBaseTrans.localPosition;
		}

		// 回転更新	
		if ( this.LastObjAngle != this.ObjBaseTrans.rotation)
		{
			this.RotateUpdate();
		}

		// カメラ更新
		var camera = this.Attach.cameraSprite;
		if (camera != null && this.LastCameraAngle != Camera.main.transform.rotation)
		{
			camera.transform.rotation = Quaternion.Euler(0f, 0f, -Camera.main.transform.eulerAngles.y - GUIMinimap.MapRotation);
			this.LastCameraAngle = Camera.main.transform.rotation;
		}

		// HP更新
		var hp = this.Attach.hpSprite;
		if (hp != null && (this.LastMaxHP != this.ObjectBase.MaxHitPoint || this.LastNowHP != this.ObjectBase.HitPoint))
		{
			// ゲージ更新
			float fillAmount = 0f;
			if (this.ObjectBase.MaxHitPoint != 0)
				fillAmount = (float)this.ObjectBase.HitPoint / (float)this.ObjectBase.MaxHitPoint;
			hp.fillAmount = fillAmount;

			this.LastMaxHP = this.ObjectBase.MaxHitPoint;
			this.LastNowHP = this.ObjectBase.HitPoint;
		}

		// アイコンスプライトのスケールを調整する
		if (this.Attach.isAdjustMapScale && this.IconSprite.transform.localScale.x != GUIMinimap.Scale.x)
			this.IconSprite.transform.localScale = new Vector3(GUIMinimap.Scale.x, GUIMinimap.Scale.x, this.IconSprite.transform.localScale.z);

		// マップウィンドウモードに対応した更新処理を実行
		this.MapWindowModeUpdate();
	}

	/// <summary>
	/// 回転の更新
	/// </summary>
	public void RotateUpdate()
	{
		var rotTrans = this.Attach.rotationRoot;
		if(rotTrans != null)	
			rotTrans.localRotation = Quaternion.Euler(0f, 0f, -this.ObjBaseTrans.eulerAngles.y - GUIMinimap.MapRotation);
		this.LastObjAngle = this.ObjBaseTrans.rotation;
	}
	#endregion

	#region GUIMapWindowの各モードでの初期化・更新処理
	void ChangeMapWindowMode( GUIMapWindow.MapMode mode )
	{
		GUIMapWindow.MapMode nowMode = mode;

		// モードが切り替わったら初期化・終了処理を行う
		if( this.LastMapWindowMode != nowMode )
		{
			// 初期化時に設定された終了処理を実行
			this.MapWindowModeExit();

			// 初期化
			switch(nowMode)
			{
			case GUIMapWindow.MapMode.Briefing:
				{
					this.MapWindowModeUpdate = ()=>{};
					this.MapWindowModeExit = ()=>{};
				}
				break;
			case GUIMapWindow.MapMode.Battle:
				{
					this.MapWindowModeUpdate = BattleModeUpdate;
					this.MapWindowModeExit = BattleModeExit;
				}
				break;
			case GUIMapWindow.MapMode.Respawn:
				{
					this.MapWindowModeUpdate = ()=>{};
					this.InitRespawnSelect();
				}
				break;
			case GUIMapWindow.MapMode.Transport:
				{
					this.MapWindowModeUpdate = ()=>{};
					this.InitRespawnSelect();
				}
				break;
			case GUIMapWindow.MapMode.Off:
				{
					this.MapWindowModeUpdate = ()=>{};
					this.MapWindowModeExit = ()=>{};
				}
				break;
			}
		}

		this.LastMapWindowMode = nowMode;
	}	

	/// <summary>
	/// 通常戦闘中更新
	/// </summary>
	void BattleModeUpdate()
	{
		// 画面外に行った時の矢印案内処理
		if( this.IsGuide )
		{
			var myTrans = this.transform;

			// 本来の位置を計算
			//var objTrans = this.ObjectBase.transform;
			var posTrans = this.Attach.positionRoot;
			Vector3 position = Vector3.zero;
			if( posTrans != null )
			{
				position.x = this.ObjBaseTrans.localPosition.x * GUIMinimap.Scale.x;
				position.y = this.ObjBaseTrans.localPosition.z * GUIMinimap.Scale.y;
				position.z = posTrans.localPosition.z;
				posTrans.localPosition = position;
			}

			if(!GUIMapWindow.IsMapIconVisible(myTrans.position))
			{
				// 矢印オン
				this.Attach.arrowSprite.gameObject.SetActive(true);

				// クリッピングの四隅を計算
				Vector3[] panelCorners = GUIMapWindow.MapPanelWorldCorners;

				// 枠内に補正
				Vector3 orgPos = myTrans.position;
				Vector3 newPos = myTrans.position;

				if( panelCorners[2].y < myTrans.position.y)
				{
					newPos.y = panelCorners[2].y;
				}
				else if( panelCorners[0].y > myTrans.position.y)
				{
					newPos.y = panelCorners[0].y;
				}
				if( panelCorners[2].x < myTrans.position.x)
				{
					newPos.x = panelCorners[2].x;
				}
				else if( panelCorners[0].x > myTrans.position.x )
				{
					newPos.x = panelCorners[0].x;
				}

				// 距離によって縮小させる
				float distance = Vector3.Distance(orgPos,newPos);
				float scaleMag = Mathf.Clamp(1-distance*GUIMinimapIconItem.DistanceScaleMag,GUIMinimapIconItem.DistanceMinScale,1);
				myTrans.localScale = new Vector3(this.DefaultLocalScale.x*scaleMag,this.DefaultLocalScale.y*scaleMag,this.DefaultLocalScale.z);

				// 位置を再計算
				// サイズ計算の為一旦回転を０にする
				this.Attach.arrowSprite.transform.rotation = Quaternion.identity;
				Vector3[] arrowSpriteCorners = this.Attach.arrowSprite.worldCorners;
				float spriteHalfHeight = Mathf.Abs( (arrowSpriteCorners[1].y-arrowSpriteCorners[0].y));

				// 半分出ちゃってるので見えるようにオフセット
				if( panelCorners[2].y < myTrans.position.y+spriteHalfHeight)
				{
					newPos.y = panelCorners[2].y-spriteHalfHeight;
				}
				else if( panelCorners[0].y > myTrans.position.y-spriteHalfHeight)
				{
					newPos.y = panelCorners[0].y+spriteHalfHeight;
				}
				if( panelCorners[2].x < myTrans.position.x+spriteHalfHeight)
				{
					newPos.x = panelCorners[2].x-spriteHalfHeight;
				}
				else if( panelCorners[0].x > myTrans.position.x-spriteHalfHeight )
				{
					newPos.x = panelCorners[0].x+spriteHalfHeight;
				}

				myTrans.position = newPos;

				// 矢印の向きを求める
				float x1 = myTrans.position.x;
				float y1 = myTrans.position.y;
				float x2 = orgPos.x;
				float y2 = orgPos.y;

				// 角度求める　-90はスプライトの初期の向きによる　右向きなら０
				float deg = Mathf.Atan2(y2-y1,x2-x1)*Mathf.Rad2Deg-90;

				// 矢印回転
				this.Attach.arrowSprite.transform.rotation = Quaternion.Euler(0,0,deg);
			}
			else
			{
				// 矢印オフ
				this.Attach.arrowSprite.gameObject.SetActive(false);

				myTrans.localScale = this.DefaultLocalScale;
			}
		}
	}
	/// <summary>
	/// バトルーモード終了処理
	/// </summary>
	void BattleModeExit()
	{
		// スケールを元に戻す
		this.transform.localScale = this.DefaultLocalScale;

		if(this.Attach.arrowSprite != null)
			this.Attach.arrowSprite.gameObject.SetActive(false);

		// ポジションの再計算

		if( this.IsGuide )
		{
			var objTrans = this.ObjectBase.transform;
			var posTrans = this.Attach.positionRoot;
			if( posTrans != null )
			{
				Vector3 position = Vector3.zero;
				position.x = objTrans.localPosition.x * GUIMinimap.Scale.x;
				position.y = objTrans.localPosition.z * GUIMinimap.Scale.y;
				position.z = posTrans.localPosition.z;
				posTrans.localPosition = position;
			}
		}
	}

	/// <summary>
	/// リスポーン&トランスポートモードの初期化
	/// </summary>
	void InitRespawnSelect()
	{	
		// 自チームのリスポーン地点
		if( this.ObjectBase != null && PlayerManager.Instance != null &&
			this.ObjectBase.EntrantType == EntrantType.Respawn &&
			this.ObjectBase.TeamType == PlayerManager.Instance.PlayerTeamType )
		{
			this.Attach.iconRoot.gameObject.SetActive(true);

			// リスポーン地点アイコンはDepthをメッチャ手前に
			var widgets = this.gameObject.GetComponentsInChildren<UIWidget>();
			foreach (var w in widgets)
				w.depth += 1000;

			this.MapWindowModeExit = ()=>
			{
				widgets = this.gameObject.GetComponentsInChildren<UIWidget>();
				foreach (var w in widgets)
					w.depth -= 1000;

				RespawnBlinkAnimationStop();
				this.Attach.iconRoot.gameObject.SetActive(false);
			};

		}
		// それ以外
		else
		{
			// リスポーン地点の点滅スプライトオフ
			if( this.Attach.respawnBlinkSprite != null )
				this.Attach.respawnBlinkSprite.enabled = false;

			if( this.Attach.collider != null)
				this.Attach.collider.enabled = false;

			this.MapWindowModeExit = ()=>
			{
				if( this.Attach.collider != null)
					this.Attach.collider.enabled = true;
				RespawnBlinkAnimationStop();
			};
		}

		// マーカーも点滅アニメーションしないといけないので全てに対して行う
		RespawnBlinkAnimationStart();
	}
	#endregion 

	#region アニメーション
	/// <summary>
	/// 生成時アニメーション
	/// </summary>
	public void CreateAnimation()
	{
		var pt = this.Attach.createPlayTween;
		if (pt != null)
			pt.Play(true);
	}
	/// <summary>
	/// 死亡時アニメーション
	/// </summary>
	public void DeathAnimation()
	{
		var pt = this.Attach.deathPlayTween;
		if (pt != null)
		{
			this.IsDeathAnimation = true;
			pt.Play(true);
		}
	}
	/// <summary>
	/// 死亡時アニメーションが終了したら呼ばれる(UIPlayTween.onFinish にインスペクター上で設定する)
	/// </summary>
	public void OnDeathFinish()
	{
		this.IsDeathAnimation = false;
	}
	/// <summary>
	/// ダメージアニメーション
	/// </summary>
	public void DamageAnimation()
	{
		var pt = this.Attach.damagePlayTween;
		if (pt != null)
			pt.Play(true);
	}
	/// <summary>
	/// ターゲット時アニメーション開始
	/// </summary>
	public void TargetAnimationStart()
	{
		var pt = this.Attach.targetPlayTween;
		if (pt != null)
			pt.Play(true);
	}
	/// <summary>
	/// ターゲット時アニメーション停止
	/// </summary>
	public void TargetAnimationStop()
	{
		var pt = this.Attach.targetPlayTween;
		if (pt != null)
		{
			pt.SetTweener((tw) =>
			{
				tw.Sample(0f, true);
				tw.enabled = false;
			});
		}
	}
	/// <summary>
	/// リスポーンアイコンアニメーション開始
	/// </summary>
	public void RespawnBlinkAnimationStart()
	{
		var pt = this.Attach.respawnBlinkTween;
		if (pt != null)
		{
			pt.SetTweener((tw)=>
				{
					tw.tweenFactor = 0f;
				});
			pt.Play(true);
		}
	}
	/// <summary>
	/// リスポーンアイコンアニメーション停止
	/// </summary>
	public void RespawnBlinkAnimationStop()
	{
		var pt = this.Attach.respawnBlinkTween;
		if (pt != null)
		{
			pt.SetTweener((tw) =>
			{
				tw.Sample(0f,true);
				tw.enabled = false;
			});
		}
	}
	#endregion


	#region OnDestroy
	void OnDestroy()
	{
		GUIMapWindow.ChangeModeEvent -= this.ChangeMapWindowMode;
	}
	#endregion 

	#region NGUIリフレクション
	public void OnIconTap()
	{
		switch(GUIMapWindow.Mode)
		{
		case GUIMapWindow.MapMode.Transport:
		case GUIMapWindow.MapMode.Respawn:
			{
				if( ( this.ObjectBase.EntrantType == EntrantType.Respawn ) && 
					PlayerManager.Instance != null && this.ObjectBase.TeamType == PlayerManager.Instance.PlayerTeamType )
				{
					GUIMapWindow.SelectRespawnPoint(this.ObjectBase);
				}

			}
			break;
		}
	}
	#endregion
}