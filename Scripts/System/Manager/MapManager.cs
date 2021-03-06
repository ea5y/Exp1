/// <summary>
/// マップマネージャー
/// 
/// 2012/12/14
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

public class MapManager : Manager
{
	#region フィールド＆プロパティ
	public const int LobbyMapTypeID = 1;

	public static MapManager Instance;

	public ReferenceMapData Map { get; private set; }
	
	public AreaType AreaType { get; private set; }
	public int FieldId { get; private set; }
	public int MapType { get; private set; }

	/// <summary>
	/// マップモデルが存在するかどうか.
	/// </summary>
	public bool MapExists { get { return (this.Map != null); } }
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
		if (this.Map)
		{
			this.Destroy(go);
			return;
		}

		base.Setup(go);

		this.Map = go.GetComponent<ReferenceMapData>();
	}
	#endregion

	#region 削除
	public override void Destroy(GameObject go)
	{
		this.Destroy();
	}
	public void Destroy()
	{
		if (this.Map)
			base.Destroy(this.Map.gameObject);
		this.Map = null;
	}
	#endregion

	#region 作成
	public static void Create(AreaType areaType, int fieldId, int mapID)
	{
		Instance.AreaType = areaType;
		Instance.FieldId = fieldId;
		Instance.MapType = mapID;
		
		CreateField(areaType, mapID);
	}

	private static void CreateField(AreaType areaType, int mapID)
	{
		string assetBundleName = MapName.GetAssetbundleName(areaType, mapID);

		AssetReference assetReference = AssetReference.GetAssetReference(assetBundleName);
		Instance.StartCoroutine(assetReference.GetAssetAsync<GameObject>(MapName.AssetPath, (GameObject resource) =>
			{
                Debug.Log("Loaded map:" + assetBundleName + ",result:" + Instance);
				if(Instance != null)
				{                  
					Instance.Instantiate(resource, null);
				}
			    assetReference = null;
			}
		));
	}
	/// <summary>
	/// Titleシーン用のマップモデル読み込み.
	/// 読込アセットの指定は外部に出す(マスターデータ？アセットバンドル？).
	/// </summary>
	public static void CreateTitle()
	{
		string assetBundleName = MapName.GetTitleAssetbundleName(1);

		AssetReference assetReference = AssetReference.GetAssetReference(assetBundleName);
		Instance.StartCoroutine(assetReference.GetAssetAsync<GameObject>(MapName.AssetPath, (GameObject resource) =>
			{
				if(Instance != null)
				{
					Instance.Instantiate(resource, null);
				}
			}
		));
	}
	#endregion

	#region 判定
	/// <summary>
	/// 現在いるマップと一致しているか？.
	/// </summary>
	public bool IsSameMap(AreaType areaType, int fieldId)
	{
		if(Instance.AreaType == areaType &&
			Instance.FieldId == fieldId)
		{
			return true;
		}
		return false;
	}
	#endregion
}
