using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace XUI.MailItemPageList
{
	/// <summary>
	/// ページ変更イベント引数
	/// </summary>
	public class PageChangeEventArgs : EventArgs
	{
		/// <summary>
		/// 変更後のページ
		/// </summary>
		public int Page { get; set; }

		/// <summary>
		/// 変更後のページの最初のインデックス
		/// </summary>
		public int ItemIndex { get; set; }

		/// <summary>
		/// 件数
		/// </summary>
		public int ItemCount { get; set; }

		public PageChangeEventArgs(int page, int itemIndex, int itemCount)
		{
			Page = page;
			ItemIndex = itemIndex;
			ItemCount = itemCount;
		}
	}


	public interface IController
	{
		#region === Event ===

		/// <summary>
		/// ページ変更イベント
		/// </summary>
		event EventHandler<PageChangeEventArgs> OnPageChange;

		#endregion === Event ===


		/// <summary>
		/// アクティブ設定
		/// </summary>
		void SetActive(bool isActive, bool isTweenSkip);
		
		/// <summary>
		/// 初期化
		/// </summary>
		void Setup();
		
		/// <summary>
		/// オブジェクトの破棄
		/// </summary>
		void Dispose();


		void Clear();

		/// <summary>
		/// 指定ページにセット
		/// </summary>
		void SetPage(int page);

		/// <summary>
		/// 今のページを再度開く
		/// </summary>
		void ReopenPage();

		/// <summary>
		/// トータルのアイテム数をセット
		/// </summary>
		void SetTotalItemCount(int count);

		/// <summary>
		/// 表示するアイテムをセット
		/// </summary>
		void SetViewItem(List<MailInfo> mails);

		/// <summary>
		/// 現在のリストを更新
		/// </summary>
		void UpdateCurrentList();

	}

	public class Controller : IController
	{
		#region === Field ===

		// モデル
		private readonly IModel _model;

		// ビュー
		private readonly IView _view;

		#endregion === Field ===

		#region === Property ===

		private IModel Model { get { return _model; } }

		private IView View { get { return _view; } }

		/// <summary>
		/// 更新できる状態かどうか
		/// </summary>
		private bool CanUpdate
		{
			get
			{
				if(this.Model == null) return false;
				if(this.View == null) return false;
				return true;
			}
		}


		#endregion === Property ===

		#region === Event ===

		/// <summary>
		/// ページ変更イベント
		/// </summary>
		public event EventHandler<PageChangeEventArgs> OnPageChange = (sender, e) => { };

		#endregion === Event ===


		public Controller(IModel model, IView view)
		{
			if(model == null || view == null) return;

			// ビュー設定
			this._view = view;
			View.OnBackPage += HandleBackPage;
			View.OnBackEndPage += HandleBackEndPage;
			View.OnNextPage += HandleNextPage;
			View.OnNextEndPage += HandleNextEndPage;

			// モデル設定
			this._model = model;
			Model.OnPageChange += HandlePageChange;
			Model.OnCurrentMailInfoListChange += HandleCurrentMailInfoListChange;
		}


		/// <summary>
		/// 初期化
		/// </summary>
		public void Setup()
		{
			Model.Setup();
		}

		/// <summary>
		/// オブジェクトの破棄
		/// </summary>
		public void Dispose()
		{
			// モデル破棄
			if(_model != null) {
				this._model.Dispose();
			}
		}

		/// <summary>
		/// アクティブ設定
		/// </summary>
		public void SetActive(bool isActive, bool isTweenSkip)
		{
			if(this.CanUpdate) {
				this.View.SetActive(isActive, isTweenSkip);
			}
		}

		public void Clear()
		{
			if(!CanUpdate) return;

			Model.MailItemScrollView.Clear();
		}


		/// <summary>
		/// ページ切り替えの有効を切り替える
		/// </summary>
		/// <param name="enable"></param>
		private void SetPageButtonEnable(bool enable)
		{
			if(!CanUpdate) return;

			Model.MailItemScrollView.SetPageButtonEnable(enable);
			Model.PageButtonEnabele = enable;
			View.SetActive(enable, false);
		}


		#region === Page Event ===

		/// <summary>
		/// 戻る
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleBackPage(object sender, EventArgs e)
		{
			if(!CanUpdate) return;
			if(!Model.PageButtonEnabele) return;

			Model.BackPage();
		}

		/// <summary>
		/// 一番最初に戻る
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleBackEndPage(object sender, EventArgs e)
		{
			if(!CanUpdate) return;
			if(!Model.PageButtonEnabele) return;

			Model.BackEndPage();
		}

		/// <summary>
		/// 進む
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleNextPage(object sender, EventArgs e)
		{
			if(!CanUpdate) return;
			if(!Model.PageButtonEnabele) return;

			Model.NextPage();
		}

		/// <summary>
		/// 一番最後に進む
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleNextEndPage(object sender, EventArgs e)
		{
			if(!CanUpdate) return;
			if(!Model.PageButtonEnabele) return;

			Model.NextEndPage();
		}

		/// <summary>
		/// ページ変更したとき
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandlePageChange(object sender, EventArgs e)
		{
			if(!CanUpdate) return;

			// 一度非表示にする・・・
			SetPageButtonEnable(false);
			
			// 通信挟む
			var args = new PageChangeEventArgs(
				Model.MailItemScrollView.PageIndex,
				Model.MailItemScrollView.NowPageStartIndex,
				Model.MailItemScrollView.NowPageItemMax
			);
			OnPageChange(this, args);
		}

		#endregion === Page Event ===
		
		/// <summary>
		/// 指定ページにセット
		/// </summary>
		public void SetPage(int page)
		{
			if(!CanUpdate) return;

			Model.SetPage(page);
		}
		
		/// <summary>
		/// 今のページを再度開く
		/// </summary>
		public void ReopenPage()
		{
			if(!CanUpdate) return;

			Model.SetPage(Model.CurrentPage);
		}

		/// <summary>
		/// トータルのアイテム数をセット
		/// </summary>
		/// <param name="count"></param>
		public void SetTotalItemCount(int count)
		{
			if(!CanUpdate) return;

			Model.SetTotalItemCount(count);
		}

		/// <summary>
		/// 表示アイテムをセット
		/// </summary>
		/// <param name="mails"></param>
		public void SetViewItem(List<MailInfo> mails)
		{
			if(!CanUpdate) return;

			Model.SetCurrentMailInfoList(mails);
		}


		/// <summary>
		/// 表示リストが更新時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleCurrentMailInfoListChange(object sender, EventArgs e)
		{
			UpdateItem();
		}

		/// <summary>
		/// リストのアイテムを更新する
		/// </summary>
		private void UpdateItem()
		{
			if(!CanUpdate) return;
			
			// 全アイテムリスト
			var items = Model.GetMailItemList();

			int mailCount = Model.CurrentMailInfoList.Count;
			for(int i = 0; i < items.Count; i++) {
				if(i < mailCount) {
					items[i].SetMailInfo(Model.CurrentMailInfoList[i]);
				} else {
					items[i].SetMailInfo(null);
				}
			}

			SetPageButtonEnable(true);

			Model.Reposition();
		}


		/// <summary>
		/// 現在のリストを更新
		/// </summary>
		public void UpdateCurrentList()
		{
			if(!CanUpdate) return;

			// 全アイテムリスト
			var items = Model.GetMailItemList();
			
			for(int i = 0; i < items.Count; i++) {
				items[i].UpdateMailInfo();
			}
		}

	}
}

