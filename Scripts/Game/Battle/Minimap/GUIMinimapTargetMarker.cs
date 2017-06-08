/// <summary>
/// ターゲットマーカー
/// 
/// 2014/12/05
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.GameParameter;

public class GUIMinimapTargetMarker : MonoBehaviour
{
	#region 定義

	// 画面外に出た時に出る誘導アイコンの距離によるスケール最小値
	const float DistanceMinScale = 0.4f;
	// 画面外に出た時に出る誘導アイコンの距離によるスケール倍率
	const float DistanceScaleMag = 0.4f;

	#endregion 

	#region	フィールド・プロパティ
	const float FlipBorder = 3f;
	/// <summary>
	/// 初期化時のアクティブ状態
	/// </summary>
	[SerializeField]
	bool _isStartActive = false;
	bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UIPlayTween	rootTween;
		public UISprite		attackIconSprite;
		public UISprite		defenceIconSprite;
		public UITable		iconRoot;
		public GameObject	iconPrefab;
		public Transform	groupSymbolTrans;
		public Transform	groupDispItem;
	}

	[SerializeField,Tooltip("X軸回転した時のコマンドシンボルアイコンY座標オフセット値")] 
	float _groupSymbolOffsetY = -10f;
	float GroupSymbolOffsetY{get{ return _groupSymbolOffsetY;}}

	[SerializeField,Tooltip("X軸回転した時の数字アイコンのY座標オフセット値")] 
	float _iconRootOffsetY = -11f;
	float IconRootOffsetY{get{ return _iconRootOffsetY;}}

	// アクティブ設定
	bool IsActive { get; set; }
	// アイコンリスト
	Dictionary<int,GUIMinimapTargetMarkerIcon> TargetIconList;
	// 親アイコン
	GUIMinimapIconItem		ParentIcon{ get;set; }
	// このマーカーの所持オブジェクト
	ObjectBase					OwnObjctBase{ get;set; }
	public int					ParentInFieldID{get;private set;}
	// デフォルトスケール
	Vector3 DefaultLocalScale{ get;set; }
	Vector3 DefaultGlobalScale{ get;set; }
	// チームタイプ
	TeamType TeamType
	{
		get
		{
			if( this.OwnObjctBase != null ) 
				return this.OwnObjctBase.TeamType;

			return TeamType.Unknown;
		}
	}
	// 現在の角度
	Vector3 _nowRotate;

	// 向き
	bool IsRight{ get{ return this._nowRotate.y < 90; } }
	bool IsUp{ get{ return this._nowRotate.x < 90; } }

	Vector3 IconRootDefaultPos{get;set;}
	Vector3 GroupSymbolDefaultPos{get;set;}

	GUIMapWindow.MapMode LastMapWindowMode{get;set;}

	/// <summary>
	/// マップウィンドウの状態によって実行する処理を変える
	/// </summary>
	System.Action UpdateExec{get;set;}
	System.Action ExitExec{get;set;}
	#endregion 

	#region 初期化・作成
	void Awake()
	{
		this.TargetIconList = new Dictionary<int,GUIMinimapTargetMarkerIcon>();
		for(int i = this.Attach.iconRoot.transform.childCount-1 ; 0 <= i ; i--)
		{
			var item = this.Attach.iconRoot.transform.GetChild(i);
			item.transform.parent = null;
			Destroy(item.gameObject);
		}

		this._nowRotate = Vector3.zero;

		this.IconRootDefaultPos = this.Attach.iconRoot.transform.localPosition;
		this.GroupSymbolDefaultPos = this.Attach.groupSymbolTrans.transform.localPosition;

		gameObject.SetActive(true);
		this._SetActive(this.IsStartActive);

		this.LastMapWindowMode = GUIMapWindow.MapMode.Off;

		this.UpdateExec = ()=>{};
		this.ExitExec = ()=>{};
	}

	public static GUIMinimapTargetMarker Create( GameObject prefab , Transform parent)
	{
		// インスタンス化
		var go = SafeObject.Instantiate(prefab) as GameObject;
		if (go == null)
			return null;
		// 名前
		go.name = prefab.name;

		// 親子付け
		go.transform.parent = parent;
		go.transform.localPosition = prefab.transform.localPosition;
		go.transform.localRotation = prefab.transform.localRotation;
		go.transform.localScale = prefab.transform.localScale;
		
		// コンポーネント取得
		var item = go.GetComponent(typeof(GUIMinimapTargetMarker)) as GUIMinimapTargetMarker;
		if (item == null)
			return null;
		// 値初期化
		item.Awake();
		//if( GUIMapWindow.IsActive )
		//{
		//	item.DefaultGlobalScale = new Vector3( 
		//		go.transform.lossyScale.x/(GUIMapWindow.MiniMapParentScale.x*GUIMapWindow.NowMapScale),
		//		go.transform.lossyScale.y/(GUIMapWindow.MiniMapParentScale.y*GUIMapWindow.NowMapScale),
		//		go.transform.lossyScale.z/GUIMapWindow.MiniMapParentScale.z
		//		);
		//}
		//else
		{
			item.DefaultGlobalScale = go.transform.lossyScale;
		}

		item.DefaultLocalScale = prefab.transform.localScale;
		item.gameObject.SetActive(false);
		item.gameObject.SetActive(true);

		return item;
	}
	public void Setup( GUIMinimapIconItem parentIcon , ObjectBase objectBase )
	{
		this.ParentIcon = parentIcon;
		this.OwnObjctBase = objectBase;

		if(objectBase != null)
			this.ParentInFieldID = objectBase.InFieldId;
		
		if( parentIcon != null )
		{
			// 情報セット
			if( this.TeamType == PlayerManager.Instance.PlayerTeamType )	
			{
				if( this.Attach.defenceIconSprite != null )
					this.Attach.defenceIconSprite.enabled = true;
				if(this.Attach.attackIconSprite != null)
					this.Attach.attackIconSprite.enabled = false;
			}
			else
			{
				if( this.Attach.defenceIconSprite != null )
					this.Attach.defenceIconSprite.enabled = false;
				if(this.Attach.attackIconSprite != null)
					this.Attach.attackIconSprite.enabled = true;
			}
		}

	}
	#endregion

	#region アクティブ設定
	void _SetActive(bool isActive)
	{
		this.IsActive = isActive;
		gameObject.SetActive(true);
		// アニメーション開始
		this.Attach.rootTween.Play(this.IsActive);

		if( this.IsActive )
			SetDispItemActive(true);
	}
	void SetDispItemActive( bool isActive )
	{
		foreach( Transform obj in this.Attach.groupDispItem )
		{
			obj.gameObject.SetActive(this.IsActive);
		}
	}
	#endregion

	#region Update

	void Update()
	{
		if(ParentIcon == null || !this.IsActive)
			return ;

 		// 設定した更新処理を実行
		this.UpdateExec();

		// モード変化をチェック
		this.CheckMapWindowMode();
	}

	/// <summary>
	/// マップウィンドウのモード変化をチェック
	/// </summary>
	void CheckMapWindowMode()
	{
		var nowMode = GUIMapWindow.Mode;
		if( this.LastMapWindowMode != nowMode)
		{
			// 終了処理を実行
			this.ExitExec();

			// 各モード毎に初期化
			switch( nowMode )
			{
			case GUIMapWindow.MapMode.Briefing:
			case GUIMapWindow.MapMode.Battle:
			case GUIMapWindow.MapMode.Transport:
			case GUIMapWindow.MapMode.Respawn:
				{
					this.UpdateExec = InMapWindowUpdate;
					this.ExitExec = ()=>
						{
							Transform myTrans = this.transform;

							// Transform関連を元に戻す
							if( myTrans.position != ParentIcon .transform.position )
								myTrans.position = ParentIcon.transform.position;
							if( myTrans.localScale != this.DefaultLocalScale )
								myTrans.localScale = this.DefaultLocalScale;
							if( myTrans.rotation != Quaternion.identity )
								myTrans.rotation = Quaternion.identity;

							if( this.Attach.iconRoot.transform.localPosition != this.IconRootDefaultPos )  
								this.Attach.iconRoot.transform.localPosition = this.IconRootDefaultPos;
							if(  this.Attach.groupSymbolTrans.localPosition != this.GroupSymbolDefaultPos )
								this.Attach.groupSymbolTrans.localPosition = this.GroupSymbolDefaultPos;	

							// シンボルアイコンの回転を０に
							if( this.Attach.attackIconSprite != null )
								this.Attach.attackIconSprite.transform.rotation = Quaternion.identity;

							if( this.Attach.defenceIconSprite != null )
								this.Attach.defenceIconSprite.transform.rotation = Quaternion.identity;

							this._nowRotate = Vector3.zero;

							// アイコンの回転を初期値に
							foreach( var icon in this.TargetIconList.Values )
							{
								icon.ResetRotation();
							}
						};

					// １フレーム遅れて反転処理が入って一瞬ちらつくのでここで位置・回転計算しておく
					this.UpdateExec();
				}
				break;
			case GUIMapWindow.MapMode.Off:
				{
					this.UpdateExec = ()=>{};
					this.ExitExec = ()=>{};
				}
				break;
			}
		}

		this.LastMapWindowMode = nowMode;
	}

	#endregion 

	#region マップウィンドウ内での処理
	void InMapWindowUpdate()
	{
		Transform myTrans = this.transform;

		// 座標とスケール更新
		{
			if( myTrans.position != ParentIcon .transform.position )
				myTrans.position = ParentIcon.transform.position;

			// 画面内かどうか判定
			// スケールを一定に
			Vector3 lossyScale = myTrans.lossyScale;
			Vector3 localScale = myTrans.localScale;
			myTrans.localScale = new Vector3(
					localScale.x / lossyScale.x * this.DefaultGlobalScale.x/**GUIMapWindow.MiniMapParentScale.x*/,
					localScale.y / lossyScale.y * this.DefaultGlobalScale.y/**GUIMapWindow.MiniMapParentScale.y*/,
					localScale.z
			);
		}

		// クリッピングの四隅を取得
		Vector3[] panelCorners = GUIMapWindow.MapPanelWorldCorners;

		// 画面外にはみ出した時に枠内に戻す処理
		{
			// 画面外に出ていれば処理を行う
			if(!GUIMapWindow.IsMapIconVisible(this.ParentIcon.transform.position))
			{
				// 枠内に補正
				// 上下左右チェック
				Vector3 orgPos = this.ParentIcon.transform.position;
				Vector3 newPos = myTrans.position;

				if( panelCorners[2].y < orgPos.y)
				{
					newPos.y = panelCorners[2].y;
				}
				else if( panelCorners[0].y > orgPos.y)
				{
					newPos.y = panelCorners[0].y;
				}
				if( panelCorners[2].x < orgPos.x)
				{
					newPos.x = panelCorners[2].x;
				}
				else if( panelCorners[0].x > orgPos.x)
				{
					newPos.x = panelCorners[0].x;
				}

				myTrans.position = newPos;

				// 距離によって縮小
				float distance = Vector3.Distance(orgPos,newPos);
				float scaleMag = Mathf.Clamp(1-distance*GUIMinimapTargetMarker.DistanceScaleMag,GUIMinimapTargetMarker.DistanceMinScale,1);

				myTrans.localScale = new Vector3(myTrans.localScale.x*scaleMag,myTrans.localScale.y*scaleMag,myTrans.localScale.z);
			}
		}


		// はみ出しそうになった時の回転処理
		{
			Vector3 nowPos = this.ParentIcon.transform.position;

			// 向き変更

			// 横
			float judgeSizeX = (panelCorners[2].x-panelCorners[0].x)/GUIMinimapTargetMarker.FlipBorder;

			Vector3 rotate = this._nowRotate;

			// パネルの３分の１より左
			if( nowPos.x < panelCorners[0].x+judgeSizeX && !this.IsRight )
			{
				rotate.y = 0f;
			}
			// パネルの３分の１より右
			else if( panelCorners[2].x-judgeSizeX < nowPos.x && this.IsRight )
			{
				rotate.y = 180;
			}

			// 縦
			float judgeSizeY = (panelCorners[1].y-panelCorners[0].y)/GUIMinimapTargetMarker.FlipBorder;

			// パネルの３分の１より下
			if( nowPos.y < panelCorners[0].y+judgeSizeY && !this.IsUp )
			{
				rotate.x = 0;
			}
			// パネルの３分の１より上
			else if( panelCorners[1].y-judgeSizeY < nowPos.y && this.IsUp )
			{
				rotate.x = 180;
			}

			// 今と変化があったら回転をかける
			if( rotate.x != this._nowRotate.x || rotate.y != this._nowRotate.y )
			{
				myTrans.rotation = Quaternion.identity*Quaternion.Euler(rotate.x,rotate.y,rotate.z);
				this._nowRotate = rotate;

				// アイコンの位置を補正
				if(! this.IsUp )
				{
					var iconRootNewPos = this.IconRootDefaultPos;
					var groupSymbolNewPos = this.GroupSymbolDefaultPos;
					iconRootNewPos.y += this.IconRootOffsetY;
					groupSymbolNewPos.y += this.GroupSymbolOffsetY;

					this.Attach.iconRoot.transform.localPosition = iconRootNewPos;
					this.Attach.groupSymbolTrans.localPosition = groupSymbolNewPos;
				}
				else
				{
					this.Attach.iconRoot.transform.localPosition = this.IconRootDefaultPos;
					this.Attach.groupSymbolTrans.localPosition = this.GroupSymbolDefaultPos;	
				}

				// シンボルアイコンの回転を０に
				if( this.Attach.attackIconSprite != null )
					this.Attach.attackIconSprite.transform.rotation = Quaternion.identity;

				if( this.Attach.defenceIconSprite != null )
					this.Attach.defenceIconSprite.transform.rotation = Quaternion.identity;

				// アイコンの回転を初期値に
				foreach( var icon in this.TargetIconList.Values )
				{
					icon.ResetRotation();
				}
			}
		}
	}
	#endregion 

	#region アイコン追加・削除
	public void AddIcon( Character character )
	{
		if( character == null )
			return;

		GUIMinimapTargetMarkerIcon checkObj = null;

		// 無いときは新規生成
		if( !TargetIconList.TryGetValue(character.TacticalId,out checkObj) )
		{
	 		var icon = GUIMinimapTargetMarkerIcon.Create(this.Attach.iconPrefab,this.Attach.iconRoot.transform,character.TacticalId);
			icon.Setup(character,this);
			this.TargetIconList.Add(character.TacticalId,icon);
			this.Attach.iconRoot.Reposition();
			this._SetActive(true);
		}
		else
		{
			checkObj.Setup(character,this);
			this.Attach.iconRoot.Reposition();
			this._SetActive(true);
		}
	}

	public void RemoveIcon( Character character )
	{
		if( character == null )
			return;
		this.RemoveIcon(character.TacticalId);
	}

	public void RemoveIcon( int tacticalID )
	{
		GUIMinimapTargetMarkerIcon icon = null;

		if( this.TargetIconList.TryGetValue(tacticalID, out icon ))
		{
			icon.transform.parent = null;
			this.TargetIconList.Remove(tacticalID);
			Destroy(icon.gameObject);
			this.Attach.iconRoot.Reposition();
		}
		// ０個になったらオフする
		if( this.TargetIconList.Count <= 0 )
		{
			this._SetActive(false);
		}
	}

	#endregion 

	#region NGUIリフレクション
	public void OnReposition()
	{
		this.Attach.iconRoot.Reposition();
	}
	public void OnDisableDispItem()
	{
		if(!this.IsActive)
			this.SetDispItemActive(false);
	}
	#endregion

	#region OnDestroy
	void OnDestroy()
	{
		// 破棄時にマップウィンドウのアイコン一覧から外す
		//GUIMapWindow.RemoveGUITargetMakerList(this.ParentInFieldID);
	}
	#endregion

	#region デバッグ
#if UNITY_EDITOR && XW_DEBUG
	[SerializeField]
	DebugParameter _debugParam = new DebugParameter();
	DebugParameter DebugParam { get { return _debugParam; } }
	[System.Serializable]
	public class DebugParameter
	{
		public bool execute;
		public bool isActive;
	}
	void DebugUpdate()
	{
		var t = this.DebugParam;
		if(t.execute)
		{
			t.execute = false;
			this._SetActive(t.isActive);
		}
	}
	/// <summary>
	/// OnValidate はInspector上で値の変更があった時に呼び出される
	/// GameObject がアクティブ状態じゃなくても呼び出されるのでアクティブ化するときには有効
	/// </summary>
	void OnValidate()
	{
		this.DebugUpdate();
	}
#endif
	#endregion

}
