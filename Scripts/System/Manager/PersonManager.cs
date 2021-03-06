/// <summary>
/// 他プレイヤーマネージャー
/// 
/// 2012/12/26
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;

public class PersonManager : Manager
{
	#region フィールド＆プロパティ
	public static PersonManager Instance;

	public Person personPrefab;

	private Dictionary<int, Person> Dict { get; set; }
	public int Count { get{ return Dict.Count; } }

	[SerializeField]
	private GameObject fontPrefab;
	public  GameObject FontPrefab { get{ return fontPrefab;} }
	[SerializeField]
	private GameObject gaugePrefab;
	public  GameObject GaugePrefab { get{ return gaugePrefab;} }
	[SerializeField]
	private GameObject shadowPrefab;
	public  GameObject ShadowPrefab { get{ return shadowPrefab;} }
	[SerializeField]
	private TeamPrefab enemyMakerPrefab;
	public  TeamPrefab EnemyMakerPrefab { get{ return enemyMakerPrefab;} }

	[SerializeField]
	private int drawLimit = 19;
	public  int DrawLimit { get { return drawLimit; } }

	const float DrawLimitTime = 1f;
	private float drawLimitCount = DrawLimitTime;
	#endregion

	#region 初期化
	void Awake()
	{
		if (Instance == null)
			Instance = this;

		this.Dict = new Dictionary<int, Person>();
	}
	#endregion

	#region 更新.
	void Update()
	{
		UpdateDrawLimit();
	}
	/// <summary>
	/// 描画制限.
	/// </summary>
	private void UpdateDrawLimit()
	{
		// 表示人数制限.
		drawLimitCount -= Time.deltaTime;
		if(drawLimitCount < 0)
		{
			drawLimitCount = DrawLimitTime;

			if(this.Dict.Count <= this.DrawLimit)
			{
				// 人数 <= 表示人数制限.
				foreach(var person in this.Dict)
				{
					if(person.Value != null)
					{
						person.Value.IsDrawLimit = false;
					}
				}
			}
			else
			{
				// 表示人数制限 < 人数.
				Player player = GameController.GetPlayer();
				if(player != null)
				{
					RunDrawLimit(player.transform.position);
				}
				else if(GameController.CharacterCamera != null)
				{
					RunDrawLimit(GameController.CharacterCamera.transform.position);
				}
			}
		}
	}
	private void RunDrawLimit(Vector3 basePoint)
	{
		var limitList = new List<Person>();
		foreach(var person in this.Dict)
		{
			if(person.Value != null)
			{
				limitList.Add(person.Value);
			}
		}
		limitList.Sort((Person x, Person y) =>
		{
			var xDist = Vector3.SqrMagnitude(x.transform.position - basePoint);
			var yDist = Vector3.SqrMagnitude(y.transform.position - basePoint);
			return (int)((xDist - yDist) * 128);
		});

		for(int i = 0; i < this.DrawLimit; ++i)
		{
			limitList[i].IsDrawLimit = false;
		}
		for(int i = this.DrawLimit; i < limitList.Count; ++i)
		{
			limitList[i].IsDrawLimit = true;
		}
	}
	#endregion

	#region セットアップ
	protected override void Setup(GameObject go)
	{
		base.Setup(go);

		Person com = go.GetSafeComponent<Person>();
		if (com)
		{
			if(this.Dict.ContainsKey(com.InFieldId))
			{
				if(this.Dict[com.InFieldId] != null)
				{
					// 同じInFieldIdのPersonが存在する.
					NetworkController.SaveConflictIdLog(this.Dict[com.InFieldId], com);
					this.Dict[com.InFieldId].Remove();
				}
			}
			this.Dict[com.InFieldId] = com;
		}
		else
		{
			Debug.LogWarning(string.Format("Person({0}) コンポーネントが見つからない", go.name));
		}
	}
	#endregion

	#region 削除
	public override void Destroy(GameObject go)
	{
		Person person = go.GetSafeComponent<Person>();
		if (person != null)
		{
			this.Remove(person);
		}
		base.Destroy(go);
	}
	private void Remove(Person person)
	{
		Person outPerson;
		if (this.Dict.TryGetValue(person.InFieldId, out outPerson))
		{
			if(person == outPerson)
			{
				this.Dict.Remove(person.InFieldId);
			}
		}
	}
	#endregion

	#region 作成
	public void Create(PersonInfo info)
	{
		// 存在していない.
		if (!info.IsInArea)
			return;

		// メッセージ
		/* 入場メッセージを一旦コメントアウト
		if (info.EnterFieldTime > 0)
		{
			string userName = GameConstant.StringWithTeamColor(info.UserName, info.TeamType);
			string str = MasterData.GetText(TextType.TX007_Entry, new string[] { userName });
			GUIChat.AddMessage(str);
		}
		*/

		this.Instantiate(this.personPrefab.gameObject, info.StartPosition, Quaternion.Euler(0f, info.StartRotation, 0f), (GameObject go) =>
		{
			float size = ObsolateSrc.ModelSize.GetModelSize(info.Id);
			go.transform.localScale = new Vector3(size,size,size);
			Person.Setup(go, this, info);
		});
		/*
		CharaMasterData data;
		if(MasterData.TryGetChara(info.Id, out data))
		{
			AssetReference assetReference = AssetReference.GetAssetReference(data.AssetPath);
			CharacterLoadingAsset loadingAsset = new CharacterLoadingAsset();

			// 本体の読み込み.
			StartCoroutine(assetReference.GetAssetAsync<GameObject>(CharacterName.PersonPath, (GameObject resource) =>
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
	private void GetAssetCallBack(CharacterLoadingAsset loadingAsset, PersonInfo info, AssetReference assetReference)
	{
		// 必要リソースがすべて揃っていたらInstantiate開始.
		if(loadingAsset.CanInstantiate)
		{
			StartCoroutine(InstantiateCoroutine(loadingAsset, info,assetReference));
		}
	}
	private IEnumerator InstantiateCoroutine(CharacterLoadingAsset loadingAsset, PersonInfo info, AssetReference assetReference)
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
				Person.Setup(go, this, info, assetReference, loadingAsset.Animation);
			});
		}
	}
	*/
	#endregion
}
