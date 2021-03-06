/// <summary>
/// バフアイコンのポップアップ処理
/// 
/// 2014/07/14
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.GameParameter;
using Scm.Common.Master;

public class GUIBuffPopup : MonoBehaviour
{
	#region 定数

	public enum XSide
	{
		Right = 1,
		Left = -1
	}

	public enum YSide
	{
		Up = 1,
		Down = -1
	}

	#endregion

	#region アタッチオブジェクト

	[System.Serializable]
	public class AttachObject
	{
		public GameObject prefab;
		public Transform root;
	}

	#endregion

	#region フィールド&プロパティ

	/// <summary>
	/// 追加できるアイテムの最大数
	/// </summary>
	[SerializeField]
	private int itemMax = 10;

	/// <summary>
	/// 何個ずつ折り返すかの回数
	/// </summary>
	[SerializeField]
	private int wrapping = 5;

	/// <summary>
	/// 左右どちら側に追加していくか
	/// </summary>
	[SerializeField]
	private XSide xSide = XSide.Right;

	/// <summary>
	/// 上下どちら側に追加していくか
	/// </summary>
	[SerializeField]
	private YSide ySide = YSide.Down;

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField] AttachObject attach;
	public AttachObject Attach { get { return attach; } }

	/// <summary>
	/// 生成したアイテムを格納する用
	/// </summary>
	private List<GUIBuffPopupItem> buffItemList = new List<GUIBuffPopupItem>();

	/// <summary>
	/// バフパケット情報格納用
	/// </summary>
	private BuffPacketInfotList packetInfoList = new BuffPacketInfotList();

	/// <summary>
	/// アイコンの状態コルーチンの管理用
	/// </summary>
	private FiberSet fiberSet = new FiberSet();

	/// <summary>
	/// バフ更新用
	/// </summary>
	private Fiber fiber;

	#endregion
	
	#region セットアップ

	/// <summary>
	/// サーバからきたバフパケットをリストに格納する
	/// </summary>
	public void SetUp(LinkedList<BuffInfo> buffInfoList)
	{
		this.packetInfoList.Add(buffInfoList);

		if(!this.fiberSet.Contains(this.fiber))
		{
			this.fiber = this.fiberSet.AddFiber(UpdateCoroutine());
		}
	}

	#endregion

	#region バフアイコン削除

	/// <summary>
	/// バフアイコンを全て削除する
	/// </summary>
	public void Clear()
	{
		LinkedList<BuffInfo> emptyList = new LinkedList<BuffInfo>();
		SetUp(emptyList);
	}

	#endregion

	#region バフアイコン更新処理

	void Update()
	{
		this.fiberSet.Update();
	}
	
	private IEnumerator UpdateCoroutine()
	{
		while(this.packetInfoList.isCount())
		{
			// サーバからきたバフパケットを1つずつ処理していく
			LinkedList<BuffInfo> buffParam = this.packetInfoList.FirstValue();
			// 削除するバフを格納する用
			List<GUIBuffPopupItem> deleteList = new List<GUIBuffPopupItem>();

			// サーバからきたバフ情報を基にアイテムを追加&削除
			List<GUIBuffPopupItem> newBuffList = AddBuff(buffParam, ref deleteList);

			// 削除アニメ
			yield return new DeleteAnime(deleteList);

			// 最新のアイテムリストに更新
			// 削除処理終了前に更新するとSetActiveが呼ばれた時に削除対象のアイテムが適用されない
			// (削除エフェクト中のアイテムのactiveが適用されない)
			this.buffItemList = newBuffList;

			// ソート処理
			SortBuffIcon();

			// 移動処理
			yield return new MoveAnime(this.buffItemList, this.wrapping, (int)xSide, (int)ySide);

			// 生成アニメ
			CreateEffect();

			// バフパケット削除
			this.packetInfoList.RemoveFirst();
		}

		yield break;
	}
	
	#endregion

	/// <summary>
	// サーバからきたバフ情報を基にアイテムを追加し削除するバフは削除リストに追加する
	/// </summary>
	private List<GUIBuffPopupItem> AddBuff(LinkedList<BuffInfo> newBuffParamQueue, ref List<GUIBuffPopupItem> deleteList)
	{
		// 一旦全てのアイテムに削除フラグをOnにし削除チェック用にDictionaryを作成する
		Dictionary<BuffType, GUIBuffPopupItem> buffItemDict = new Dictionary<BuffType, GUIBuffPopupItem>();
		foreach(GUIBuffPopupItem item in this.buffItemList)
		{
			deleteList.Add(item);
			buffItemDict.Add(item.Parameter.buffType, item);
		}

		Dictionary<BuffType, GUIBuffPopupItem> newBuffItemDict = new Dictionary<BuffType, GUIBuffPopupItem>();
		List<GUIBuffPopupItem> newBuffItemList = new List<GUIBuffPopupItem>();

		foreach(BuffInfo info in newBuffParamQueue)
		{
			if(newBuffItemList.Count >= this.itemMax)
			{
				// 追加できるアイテムの最大数を超えている場合は追加しない
				break;
			}

			// まだ新リストに存在しないParameterのみ処理
			if(!newBuffItemDict.ContainsKey(info.buffType))
			{
				GUIBuffPopupItem popupItem;
				if(buffItemDict.TryGetValue(info.buffType, out popupItem))
				{
					// 旧リストに存在するなら引き継ぎ
					if(info.isNew)
					{
						// 現在表示中のバフに新規フラグの付いたバフ情報なら上書きを行う
						popupItem.Override(info);
					}
					deleteList.Remove(popupItem);
					newBuffItemDict.Add(info.buffType, popupItem);
					newBuffItemList.Add(popupItem);
				}
				else
				{
					// 旧リストに存在しないなら新規作成
					// マスターデータ取得
					StateEffectMasterData masterData;
					if(!MasterData.TryGetStateEffect((int)info.buffType, out masterData))
					{
						// 取得失敗
						continue;
					}
					GUIBuffPopupItem item = GUIBuffPopupItem.Create(this.attach.root, this.attach.prefab, info, masterData);
					if(item == null)
						continue;

					// リストに登録
					newBuffItemDict.Add(info.buffType, item);
					newBuffItemList.Add(item);
				}
			}
		}

		// 最新のアイテムリスト
		return newBuffItemList;
	}

	/// <summary>
	/// バフアイコンのソートを行う
	/// </summary>
	private void SortBuffIcon()
	{
		// ソート
		this.buffItemList.MargeSort((a, b) => a.MasterData.Priority - b.MasterData.Priority);
	}

	/// <summary>
	/// 生成エフェクト処理
	/// </summary>
	private void CreateEffect()
	{
		int count = 0;
		int x;
		int y = -1;
		foreach(GUIBuffPopupItem item in this.buffItemList)
		{
			// X座標Y座標を求める
			x = count % this.wrapping;
			if(x == 0)
			{
				y++;
			}
			
			if(item.isNew)
			{
				// 新規生成のみ生成アニメーションを再生する
				item.CreateAnime(x*(int)xSide, y*(int)ySide);
			}
			count++;
		}
	}

	#region Active

	public void SetActive(bool isActive)
	{
		foreach(GUIBuffPopupItem item in this.buffItemList)
		{
			item.SetActive(isActive);
		}
	}

	#endregion

	#region ファイバークラス群

	/// <summary>
	///  ファイバー用 削除アニメーション用 アニメーションが終了するまで待機
	/// </summary>
	public class DeleteAnime : IFiberWait
	{
		/// <summary>
		/// 削除するアイテムリスト
		/// </summary>
		private List<GUIBuffPopupItem> deleteList = new List<GUIBuffPopupItem>();
		
		public DeleteAnime(List<GUIBuffPopupItem> deleteList)
		{
			foreach(GUIBuffPopupItem item in deleteList)
			{
				// 削除アニメ開始
				item.DeleteAnime();
			}
			this.deleteList = deleteList;
		}
		
		#region IFiberWait
		
		public bool IsWait
		{
			get
			{
				bool isWait = false;
				foreach(GUIBuffPopupItem item in this.deleteList)
				{
					if(item.IsEvent)
					{
						// イベント中は削除しない
						isWait = true;
						break;
					}
					
					// GameObjectのアイテムを削除
					item.Delete();
				}
				
				if(!isWait)
				{
					// 全削除
					this.deleteList.Clear();
				}
				
				return isWait;
			}
		}
		
		#endregion
	}
	
	/// <summary>
	/// ファイバー用 移動アニメーション用 移動アニメーションが終了するまで待機.
	/// </summary>
	public class MoveAnime : IFiberWait
	{
		/// <summary>
		/// 現在生成されているアイテムリスト
		/// </summary>
		private List<GUIBuffPopupItem> popupItemList =new List<GUIBuffPopupItem>();
		
		public MoveAnime(List<GUIBuffPopupItem> popupItemList, int wrapping, int xSide, int ySide)
		{
			int count = 0;
			int y = -1;
			int x;
			foreach(GUIBuffPopupItem item in popupItemList)
			{
				// X座標Y座標を求める
				x = count % wrapping;
				if(x == 0)
				{
					y++;
				}
				
				// 新規生成のアイコンは移動処理を行わない
				if(!item.isNew)
				{
					item.SortAnimePlay(x*xSide, y*ySide);
				}
				count++;
			}
			
			this.popupItemList = popupItemList;
		}
		
		
		#region IFiberWait
		
		public bool IsWait
		{
			get
			{
				bool isWait = false;
				foreach(GUIBuffPopupItem item in this.popupItemList)
				{
					if(item.IsEvent)
					{
						// 移動中
						isWait = true;
						break;
					}
				}
				return isWait;
			}
		}
		
		#endregion
	}

	#endregion

	#region バフパケット情報リストクラス

	/// <summary>
	/// バフパケット情報をリストで管理するクラス
	/// </summary>
	public class BuffPacketInfotList
	{
		/// <summary>
		/// サーバからきたバフパケットを格納するリスト
		/// 中のLinkedListは追加された順にバフを格納するリスト
		/// </summary>
		private LinkedList<LinkedList<BuffInfo>> buffLinkList = new LinkedList<LinkedList<BuffInfo>>();
		
		#region 追加処理
		
		public void Add(LinkedList<BuffInfo> buffInfoList)
		{
			LinkedList<BuffInfo> buffList = new LinkedList<BuffInfo>();
			foreach(BuffInfo info in buffInfoList)
			{
				// 表示するバフかどうかチェック
				if(!AddListCheck(info)) continue;

				buffList.AddLast(info);
			}
			this.buffLinkList.AddLast(buffList);
		}

		/// <summary>
		/// 表示リストに追加するかどうか
		/// </summary>
		private bool AddListCheck(BuffInfo info)
		{
			// マスターデータが存在しない場合は表示しない
			StateEffectMasterData masterData;
			if(!MasterData.TryGetStateEffect((int)info.buffType, out masterData))
			{
				// マスターデータが見つからない
				return false;
			}

			// アイコンファイル名が空文字の場合は表示しない
			if(string.IsNullOrEmpty(masterData.IconFile))
			{
				return false;
			}

			return true;
		}

		#endregion
		
		#region　取得
		
		/// <summary>
		/// 最初に追加したデータを追加.
		/// </summary>
		public LinkedList<BuffInfo> FirstValue()
		{
			LinkedList<BuffInfo> popItemQueue = this.buffLinkList.First.Value;
			
			return popItemQueue;
		}
		
		/// <summary>
		/// 存在するかどうか
		/// </summary>
		public bool isCount()
		{
			return this.buffLinkList.Count > 0;
		}
		
		#endregion
		
		#region 削除
		
		/// <summary>
		/// 最初に追加したデータを削除
		/// </summary>
		public void RemoveFirst()
		{
			this.buffLinkList.RemoveFirst();
		}
		
		#endregion
	}

	#endregion
}