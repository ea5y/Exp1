/// <summary>
/// プレイヤーマネージャー
/// 
/// 2012/12/26
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;
using Scm.Common.Master;

public class PlayerManager : Manager
{
	#region フィールド＆プロパティ
	public static PlayerManager Instance;

	public Player playerPrefab;

	private PlayerInfo info;
	private PlayerInfo Info
	{
		get { return info; }
		set
		{
			if(this.info != null)
			{
				Entrant.RemoveEntrant(this.info);
			}
			this.info = value;
		}
	}
	public Player Player { get; private set; }
	
	// プレイヤー用のアセットバンドルキャッシュ(直前と同じキャラクターを使うことが多いため).
	#pragma warning disable 0414
	private AssetReference assetReferenceCache;
	/// <summary>
	/// プレイヤーの所属するチーム.
	/// 観戦モードやキャラクターチェンジ中など,プレイヤーが存在しない場合も参照可能.
	/// </summary>
	public TeamType PlayerTeamType {
		get{
			if(BattleMain.Instance != null)
			{
				return BattleMain.Instance.PlayerTeamType;
			}
			return TeamType.Unknown;
		}
		/*
		set{
			playerTeamType = value;

			// HACK: 試遊会用応急処置.チームカラーがバグっていることがあったのでプレイヤーのチーム変更時に呼び出すように.
			GUIObjectInfo[] infoArray = FindObjectsOfType(typeof(GUIObjectInfo)) as GUIObjectInfo[];
			foreach(var info in infoArray)
			{
				info.UpdateName();
			}
		}
		*/
	}

	/// <summary>
	/// 経験値
	/// TODO: 経験値は複数プレイヤーキャラクター共通で扱う
	/// いずれ1ユーザに複数のプレイヤーキャラを持つ仕組みに変える予定
	/// </summary>
	private int exp;
	public int Exp
	{
		get
		{
			return exp;
		}
		private set
		{
			this.exp = value;
			// UIセット
			GUIBattlePlayerInfo.SetExp(value, this.NextExp);
		}
	}
	public int NextExp { get; private set; }
	
	[SerializeField]
	private GameObject fontPrefab;
	public  GameObject FontPrefab { get{ return fontPrefab;} }
	[SerializeField]
	private GameObject lockonPrefab;
	public  GameObject LockonPrefab { get{ return lockonPrefab;} }
	[SerializeField]
	private GameObject shadowPrefab;
	public  GameObject ShadowPrefab { get{ return shadowPrefab;} }
	[SerializeField]
	private TeamPrefab raderRingPrefab;
	public  TeamPrefab RaderRingPrefab { get{ return raderRingPrefab;} }
	[SerializeField]
	private TeamPrefab raderArrowPrefab;
	public  TeamPrefab RaderArrowPrefab { get{ return raderArrowPrefab;} }
	#endregion

	#region 初期化
	void Awake()
	{
		if (Instance == null)
			Instance = this;
	}
	protected override void Setup(GameObject go)
	{
		// 既に登録済み
		if (this.Player)
		{
			this.Destroy();
		}

		base.Setup(go);

		this.Player = go.GetSafeComponent<Player>();
	}
	#endregion

	#region 削除
	public void Destroy()
	{
		if (this.Player)
		{
			if(this.Player.EntrantInfo == this.Info)
			{
				this.Info = null;
			}
			Entrant.RemoveEntrant(this.Player.EntrantInfo);
			base.Destroy(this.Player.gameObject);
			this.Player = null;
		}
	}
	#endregion

	#region 作成
	public void Create(PlayerInfo info)
	{
		this.Info = info;
        
		// 存在していない.
		//if (!info.IsInArea)
		//	return;

		this.Instantiate(this.playerPrefab.gameObject, info.StartPosition, Quaternion.Euler(0f, info.StartRotation, 0f), (GameObject go) =>
		{
			float size = ObsolateSrc.ModelSize.GetModelSize(info.Id);
			go.transform.localScale = new Vector3(size,size,size);
			Player.Setup(go, this, info);
			// プレイヤー生成時に経験値UIを更新する(バトルシーンに移る前に経験値が初期化されるため)
			GUIBattlePlayerInfo.SetExp(this.Exp, this.NextExp);
		});

		/*
		CharaMasterData data;
		if(MasterData.TryGetChara(info.Id, out data))
		{
			AssetReference assetReference = AssetReference.GetAssetReference(data.AssetPath);
			assetReferenceCache = assetReference;
			CharacterLoadingAsset loadingAsset = new CharacterLoadingAsset();

			// 本体の読み込み.
			StartCoroutine(assetReference.GetAssetAsync<GameObject>(CharacterName.PlayerPath, (GameObject resource) =>
				{
					loadingAsset.Main = resource;
					GetAssetCallBack(loadingAsset, info, assetReference);
				}
			));
			// アニメーションの読み込み.
			StartCoroutine(assetReference.GetAssetAsync<AnimationReference>(CharacterName.AnimationPath, (AnimationReference resource) =>
				{
					loadingAsset.Animation = resource;
					GetAssetCallBack(loadingAsset, info, assetReference);
				}
			));
		}
		*/
	}
	/*
	// アセットロード後の処理.
	private void GetAssetCallBack(CharacterLoadingAsset loadingAsset, PlayerInfo info, AssetReference assetReference)
	{
		// 必要リソースがすべて揃っていたらInstantiate開始.
		if(loadingAsset.CanInstantiate)
		{
			StartCoroutine(InstantiateCoroutine(loadingAsset, info,assetReference));
		}
	}
	private IEnumerator InstantiateCoroutine(CharacterLoadingAsset loadingAsset, PlayerInfo info, AssetReference assetReference)
	{
		while(!MapManager.Instance.MapExists)
		{
			// マップの読み込みが終わっていないので待つ.
			yield return null;
		}

		// 読み込み中にいなくなっていたら生成しない.
		if (Entrant.Exists(info))
		{
			this.Instantiate(loadingAsset.Main, info.StartPosition, Quaternion.Euler(0f, info.StartRotation, 0f), (GameObject go) =>
			{
				Player.Setup(go, this, info, assetReference, loadingAsset.Animation);
			});
		}
	}
	*/
	#endregion

	#region 経験値
	/*
	 TODO: 経験値は複数プレイヤーキャラクター共通で扱う
	 いずれ1ユーザに複数のプレイヤーキャラを持つ仕組みに変える予定
	 */ 
	public void SetupExp(int exp, int nextExp)
	{
		this.NextExp = nextExp;
		this.Exp = exp;
	}
	public void AddExp(int exp)
	{
		this.Exp += exp;
	}
	public void SetLevelUpParameter(AvatarType avatarType, int nowLevel, int upLevel)
	{
		// レベルアップした分の経験値を求める
		int nextExp = this.Exp; 
		CharaLevelMasterData charaLv = null;
		for(int addLevel = nowLevel; addLevel < upLevel; ++addLevel)
		{
			if (MasterData.TryGetCharaLv((int)avatarType, addLevel, out charaLv))
			{
				nextExp -= charaLv.NextExp;
			}
		}

		// レベルアップ後のレベルマスターデータ取得
		if (MasterData.TryGetCharaLv((int)avatarType, upLevel, out charaLv))
		{
			this.NextExp = (charaLv != null) ? charaLv.NextExp : 0;
			this.Exp = nextExp;
		}
	}
	#endregion
}
