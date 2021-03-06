/// <summary>
/// バフアイコンのポップアップアイテム処理
/// 
/// 2014/07/14
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.Master;

public class GUIBuffPopupItem : MonoBehaviour
{
	#region アタッチ類

	// アタッチオブジェクト
	[System.Serializable]
	public class AttachObject
	{
		public UIWidget widget;
		public GUIBuffIconItem buffIcon;
		public UIPlayTween createPlayTween;
		public UIPlayTween deletePlayTween;
		public TweenPosition sortTween;
	}
	
	#endregion

	#region プロパティ&フィールド

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	private AttachObject attach;
	public AttachObject Attach { get { return attach; } }

	/// <summary>
	/// 効果時間
	/// </summary>
	private float time;

	/// <summary>
	/// 新規生成かどうか.
	/// </summary>
	public bool isNew { get; private set; }

	/// <summary>
	/// 削除するかどうか
	/// </summary>
	public bool IsDelete { get; set; }

	/// <summary>
	/// イベント発生中かどうか
	/// </summary>
	public bool IsEvent { get; private set; }

	/// <summary>
	/// 表示中かどうか
	/// </summary>
	public bool IsShow { get; private set; }

	/// <summary>
	/// マスターデータ
	/// </summary>
	public StateEffectMasterData MasterData { get; private set; }

	/// <summary>
	/// バフ情報
	/// </summary>
	public BuffInfo Parameter { get; private set; }

	#endregion

	#region 生成

	/// <summary>
	/// アイテムの生成
	/// </summary>
	static public GUIBuffPopupItem Create(Transform parent, GameObject prefab, BuffInfo buffInfo, StateEffectMasterData masterData)
	{
		// プレハブ生成
		GameObject go = SafeObject.Instantiate(prefab) as GameObject;
		if (go == null)
			return null;

		// コンポーネント取得
		GUIBuffPopupItem item = go.GetComponent(typeof(GUIBuffPopupItem)) as GUIBuffPopupItem;
		if(item == null)
			return null;

		// 親子付け
		item.transform.parent = parent;
		item.transform.localPosition = Vector3.zero;
		item.transform.localScale = Vector3.one;
		item.transform.localRotation = Quaternion.identity;

		// セットアップ
		item.SetUp(buffInfo, masterData);

		// 生成アニメが再生されるまでOFF
		item.gameObject.SetActive(false);

		return item;
	}

	#endregion

	#region 開始

	void Awake()
	{
		if(this.Attach.sortTween != null)
		{
			this.Attach.sortTween.enabled = false;
		}
	}

	#endregion

	#region データセット

	/// <summary>
	/// データセット処理
	/// </summary>
	public void SetUp(BuffInfo buffInfo, StateEffectMasterData masterData)
	{
		this.time = buffInfo.remainingTime / 10f;
		this.IsEvent = false;
		this.IsDelete = false;
		this.MasterData = masterData;
		this.Parameter = buffInfo;
		this.isNew = true;

		// アイコンセット
		this.attach.buffIcon.SetUp(masterData);
	}

	/// <summary>
	/// 上書き処理
	/// </summary>
	public void Override(BuffInfo buffInfo)
	{
		this.time = buffInfo.remainingTime / 10f;
		this.Parameter = buffInfo;
	}

	#endregion

	#region 更新

	void Update()
	{
		// 効果時間を減らす
		this.time -= Time.deltaTime;
		this.Attach.buffIcon.TimeUpdate(this.time, (this.Parameter.remainingTime/10f));
	}

	#endregion

	#region 削除

	/// <summary>
	/// 削除アニメ再生
	/// </summary>
	public void DeleteAnime()
	{
		if(this.Attach.deletePlayTween == null)
		{
			EffectFinish();
		}
		else
		{
			this.IsEvent = true;
			this.Attach.deletePlayTween.Play(true);
		}
	}

	/// <summary>
	/// 削除処理
	/// </summary>
	public void Delete()
	{
		GameObject.Destroy(this.gameObject);
	}

	#endregion

	#region 生成

	/// <summary>
	/// 生成アニメ再生
	/// 表示位置のセットもここで行う
	/// </summary>
	public void CreateAnime(int x, int y)
	{
		this.isNew = false;

		// 位置セット
		SetPosition(x, y);

		// 表示ON
		this.gameObject.SetActive(true);

		if(this.Attach.createPlayTween == null)
			return;

		// 生成エフェクト再生
		this.Attach.createPlayTween.Play(true);
	}

	/// <summary>
	/// 表示位置のセット処理
	private void SetPosition(int x, int y)
	{
		Vector3 position = Vector3.zero;
		position.x = x * this.attach.widget.width;
		position.y = y * this.attach.widget.height;
		this.transform.localPosition = position;
	}
	
	#endregion

	#region ソート処理.

	/// <summary>
	/// デォルトのDuration値
	/// </summary>
	private const float defaultSortDuration = 0.2f;

	/// <summary>
	/// ソート時のアニメ再生
	/// </summary>
	public void SortAnimePlay(int x, int y)
	{
		Vector3 from = this.transform.localPosition;
		Vector3 to = this.transform.localPosition;
		to.x = x * this.attach.widget.width;
		to.y = y * this.attach.widget.width;

		// 移動値セット
		TweenPosition sortTween;
		if(this.Attach.sortTween == null)
		{
			// ソート時に使用するTweenがアタッチされていなければTweenを生成する
			sortTween = TweenPosition.Begin(this.gameObject, defaultSortDuration, to);
			sortTween.from = from;
		}
		else
		{
			this.Attach.sortTween.ResetToBeginning();
			this.Attach.sortTween.from = from;
			this.Attach.sortTween.to = to;
			this.Attach.sortTween.Play(true);
		}

		this.IsEvent = true;
	}

	#endregion

	#region エフェクト終了

	/// <summary>
	/// エフェクト再生終了時
	/// </summary>
	public void EffectFinish()
	{
		this.IsEvent = false;
	}

	#endregion

	#region Active

	public void SetActive(bool isActive)
	{
		this.IsShow = isActive;
		this.Attach.buffIcon.SetActive(isActive);
	}

	#endregion
}