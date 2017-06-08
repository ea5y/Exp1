using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace XUI.MailItemPageList
{
	/// <summary>
	/// キャラアイテムリストのページ付スクロールビュークラス
	/// </summary>
	[Serializable]
	public class GUIMailItemScrollView : PageScrollView<GUIMailItem>
	{
		protected override GUIMailItem Create(GameObject prefab, UnityEngine.Transform parent, int itemIndex)
		{
			return GUIMailItem.Create(prefab, parent, itemIndex);
		}
		protected override void ClearValue(GUIMailItem item)
		{
			item.SetMailInfo(null);
		}

		public void SetPageButtonEnable(bool enable)
		{
			float alpha = enable ? 1.0f : 0.5f;
			if(Attach.BackButtonGroup != null) {
				Attach.BackButtonGroup.alpha = alpha;
			}
			if(Attach.NextButtonGroup != null) {
				Attach.NextButtonGroup.alpha = alpha;
			}
		}
	}

	public interface IModel
	{
		/// <summary>
		/// オブジェクトの破棄
		/// </summary>
		void Dispose();

		/// <summary>
		/// 初期化
		/// </summary>
		void Setup();

		#region === Property ===
		
		/// <summary>
		/// メールスクロールビュー
		/// </summary>
		GUIMailItemScrollView MailItemScrollView { get; }

		/// <summary>
		/// 表示するリスト
		/// </summary>
		ReadOnlyCollection<MailInfo> CurrentMailInfoList { get; }

		/// <summary>
		/// ページボタンの有効
		/// </summary>
		bool PageButtonEnabele { get; set; }

		/// <summary>
		/// 今のページ
		/// </summary>
		int CurrentPage { get; }
		
		#endregion === Property ===

		#region === Event ===

		/// <summary>
		/// ページ変更イベント
		/// </summary>
		event EventHandler OnPageChange;
		
		/// <summary>
		/// 表示するリスト変更イベント
		/// </summary>
		event EventHandler OnCurrentMailInfoListChange;
		
		#endregion === Event ===

		/// <summary>
		/// メールリストを更新
		/// </summary>
		/// <param name="mails"></param>
		void SetCurrentMailInfoList(IList<MailInfo> mails);

		/// <summary>
		/// ページを変更する
		/// </summary>
		void SetPage(int page);

		/// <summary>
		/// ページ戻る
		/// </summary>
		void BackPage();

		/// <summary>
		/// 一番後ろに戻る
		/// </summary>
		void BackEndPage();
		
		/// <summary>
		/// ページを進める
		/// </summary>
		void NextPage();
		
		/// <summary>
		/// 一番最後のページに進める
		/// </summary>
		void NextEndPage();

		/// <summary>
		/// トータルのアイテム数をセットする
		/// </summary>
		/// <param name="count"></param>
		void SetTotalItemCount(int count);
		
		/// <summary>
		/// リストの位置を修正する
		/// </summary>
		void Reposition();
		
		/// <summary>
		/// アイテムリストを取得
		/// </summary>
		/// <returns></returns>
		List<GUIMailItem> GetMailItemList();

	}

	public class Model : IModel
	{
		#region === Field ===

		private GUIMailItemScrollView mailItemScrollView = null;

		private readonly List<MailInfo> currentViewMailInfo = new List<MailInfo>();

		private bool pageButtonEnabele = true;

		#endregion === Field ===

		#region === Property ===

		/// <summary>
		/// メールスクロールビュー
		/// </summary>
		public GUIMailItemScrollView MailItemScrollView { get { return mailItemScrollView; } }

		/// <summary>
		/// 表示するリスト
		/// </summary>
		public ReadOnlyCollection<MailInfo> CurrentMailInfoList
		{
			get { return currentViewMailInfo.AsReadOnly(); }
		}

		/// <summary>
		/// ページボタンの有効
		/// </summary>
		public bool PageButtonEnabele
		{
			get { return pageButtonEnabele; }
			set { pageButtonEnabele = value; }
		}

		/// <summary>
		/// 今のページ
		/// </summary>
		public int CurrentPage { get; private set; }

		#endregion === Property ===

		#region === Event ===

		/// <summary>
		/// ページ変更イベント
		/// </summary>
		public event EventHandler OnPageChange = (sender, e) => { };

		/// <summary>
		/// 表示するリスト変更イベント
		/// </summary>
		public event EventHandler OnCurrentMailInfoListChange = (sender, e) => { };

		#endregion === Event ===

		public Model(GUIMailItemScrollView itemScrollView, PageScrollViewAttach viewAttach)
		{
			mailItemScrollView = itemScrollView;
			MailItemScrollView.Create(viewAttach, null);
		}


		/// <summary>
		/// オブジェクトの破棄
		/// </summary>
		public void Dispose()
		{
			currentViewMailInfo.Clear();

			OnPageChange = null;
			OnCurrentMailInfoListChange = null;
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void Setup()
		{
			CurrentPage = -1;
			currentViewMailInfo.Clear();
			MailItemScrollView.Clear();
		}

		/// <summary>
		/// メールリストを更新
		/// </summary>
		/// <param name="mails"></param>
		public void SetCurrentMailInfoList(IList<MailInfo> mails)
		{
			currentViewMailInfo.Clear();
			currentViewMailInfo.AddRange(mails);

			OnCurrentMailInfoListChange(this, EventArgs.Empty);
		}


		/// <summary>
		/// ページを変更
		/// </summary>
		public void SetPage(int page)
		{
			MailItemScrollView.ScrollReset();
			if(MailItemScrollView.SetPage(page, 0)) {
				CurrentPage = MailItemScrollView.PageIndex;
			}
			OnPageChange(this, EventArgs.Empty);
		}

		/// <summary>
		/// ページ戻る
		/// </summary>
		public void BackPage()
		{
			MailItemScrollView.ScrollReset();
			if(MailItemScrollView.SetNextPage(-1)) {
				CurrentPage = MailItemScrollView.PageIndex;
				OnPageChange(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// 一番後ろに戻る
		/// </summary>
		public void BackEndPage()
		{
			MailItemScrollView.ScrollReset();
			if(MailItemScrollView.SetPage(0, 0)) {
				CurrentPage = MailItemScrollView.PageIndex;
				OnPageChange(this, EventArgs.Empty);
			}
		}


		/// <summary>
		/// ページを進める
		/// </summary>
		public void NextPage()
		{
			MailItemScrollView.ScrollReset();
			if(MailItemScrollView.SetNextPage(1)) {
				CurrentPage = MailItemScrollView.PageIndex;
				OnPageChange(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// 一番最後のページに進める
		/// </summary>
		public void NextEndPage()
		{
			MailItemScrollView.ScrollReset();
			if(MailItemScrollView.SetPage(MailItemScrollView.PageMax - 1, 0)) {
				CurrentPage = MailItemScrollView.PageIndex;
				OnPageChange(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// トータルのアイテム数をセットする
		/// </summary>
		/// <param name="count"></param>
		public void SetTotalItemCount(int count)
		{
			MailItemScrollView.ScrollReset();
			MailItemScrollView.Setup(count, 0);
		}

		/// <summary>
		/// リストの位置を修正する
		/// </summary>
		public void Reposition()
		{
			MailItemScrollView.ScrollReset();
			MailItemScrollView.Reposition();
		}

		/// <summary>
		/// アイテムリストを取得
		/// </summary>
		/// <returns></returns>
		public List<GUIMailItem> GetMailItemList()
		{
			List<GUIMailItem> itemList = new List<GUIMailItem>();

			for(int i = 0; i < MailItemScrollView.ItemMax; i++) {
				itemList.Add(MailItemScrollView.GetItem(i));
			}

			return itemList;
		}

	}
}
