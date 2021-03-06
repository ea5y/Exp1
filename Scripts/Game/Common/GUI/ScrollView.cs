/// <summary>
/// スクロールビュー
/// 
/// 2014/06/09
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ScrollViewAttach
{
	public GameObject prefab;
	public UIScrollView scrollView;
	public UITable table;
//	public UICenterOnChild center;
}
public abstract class ScrollView<T, C> where T : Component where C : ICollection, new()
{
	#region フィールド&プロパティ
	public ScrollViewAttach Attach { get; private set; }
	public C ItemList { get; private set; }

	public UIProgressBar VScroll { get { return (Attach.scrollView == null ? null : Attach.scrollView.verticalScrollBar); } }
	public float VScrollValue
	{
		get
		{
			var t = VScroll;
			return (t == null ? 0f : t.value);
		}
		set
		{
			var t = VScroll;
			if (t != null) t.value = value;
		}
	}
	public UIProgressBar HScroll { get { return (Attach.scrollView == null ? null : Attach.scrollView.horizontalScrollBar); } }
	public float HScrollValue
	{
		get
		{
			var t = HScroll;
			return (t == null ? 0f : t.value);
		}
		set
		{
			var t = HScroll;
			if (t != null) t.value = value;
		}
	}
	// ドラッグ開始イベント
	public event UIScrollView.OnDragNotification DragStartedEvent
	{
		add{ if( this.Attach.scrollView != null ) this.Attach.scrollView.onDragStarted += value;}
		remove{ if( this.Attach.scrollView != null ) this.Attach.scrollView.onDragStarted -= value;}
	}
	// 移動終了イベント
	public event UIScrollView.OnDragNotification StoppedMoveEvent
	{
		add{ if(this.Attach.scrollView != null) this.Attach.scrollView.onStoppedMoving += value;}
		remove{ if(this.Attach.scrollView != null) this.Attach.scrollView.onStoppedMoving -= value;}
	}

	#endregion

	#region 抽象メソッド
	protected abstract T Create(GameObject prefab, Transform parent, int itemIndex);
	protected abstract void Add(T item);
	#endregion

	#region 初期化
	/// <summary>
	/// 作成
	/// </summary>
	public void Create(ScrollViewAttach attach)
	{
		this.Attach = attach;

		// テーブル以下の余計なオブジェクトを削除する
		this.Clear();
	}
	public void Clear()
	{
		// テーブル以下の余計なオブジェクトを削除する
		this.DestroyItem();

		this.ItemList = new C();
	}
	#endregion

	#region 設定
	/// <summary>
	/// セットアップ
	/// </summary>
	public T AddItem(int itemIndex)
	{
		T item = this.Create(this.Attach.prefab, this.Attach.table.transform, itemIndex);
		this.Add(item);
		return item;
	}
	/// <summary>
	/// テーブル整形
	/// </summary>
	public void Reposition()
	{
		this.Attach.table.Reposition();
	}
	/// <summary>
	/// 全てのアイテムを削除
	/// </summary>
	public void DestroyItem()
	{
		if (this.Attach.table == null)
			return;

		// テーブル以下の余計なオブジェクトを削除する
		{
			Transform transform = this.Attach.table.transform;
			for (int i=0, max=transform.childCount; i<max; i++)
			{
				var child = transform.GetChild(i);
				Object.Destroy(child.gameObject);
			}
		}
	}
	/// <summary>
	/// スクロール更新
	/// </summary>
	public void SetScroll(float x, float y)
	{
		UIScrollView sv = this.Attach.scrollView;
		this.HScrollValue = x;
		this.VScrollValue = y;
		sv.UpdatePosition();
	}
	#endregion


	#region 取得
	/// <summary>
	/// アイテムが表示されているかどうか
	/// </summary>
	public bool IsVisible(T item)
	{
		bool isVisible = false;
		UIWidget[] widgets = item.transform.GetComponentsInChildren<UIWidget>();
		foreach (var w in widgets)
		{
			if (!this.Attach.scrollView.panel.IsVisible(w))
				continue;
			isVisible = true;
			break;
		}
		return isVisible;
	}
	#endregion 
}
