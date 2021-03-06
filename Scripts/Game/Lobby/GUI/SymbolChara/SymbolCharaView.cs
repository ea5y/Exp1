/// <summary>
/// シンボルキャラクター表示
/// 
/// 2016/03/28
/// </summary>

using UnityEngine;
using System;

namespace XUI.SymbolChara {

	/// <summary>
	/// シンボルキャラクター表示インターフェイス
	/// </summary>
	public interface IView {

		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();

		#endregion

		#region アクティブ
		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		void SetActive( bool isActive, bool isTweenSkip );

		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		GUIViewBase.ActiveState GetActiveState();
		#endregion

		#region ホーム、閉じるボタンイベント
		event EventHandler<EventArgs> OnHome;
		event EventHandler<EventArgs> OnClose;
		#endregion

		#region キャラ変更
		event EventHandler<EventArgs> OnCharaChange;

		/// <summary>
		/// キャラ変更ボタンの有効化
		/// </summary>
		void SetCharaChangeButtonEnable( bool isEnable );
		#endregion

		#region 現在のキャラクター
		/// <summary>
		/// 現在のキャラクター名
		/// </summary>
		void SetSymbolCharaName( string format, string name );
		#endregion

		#region 選択中のキャラクター
		/// <summary>
		/// 選択中のキャラクター名
		/// </summary>
		/// <param name="name"></param>
		void SetSelectCharaName( string format, string name );
		#endregion
	}

	/// <summary>
	/// シンボルキャラクター表示
	/// </summary>
	public class SymbolCharaView : GUIScreenViewBase, IView {

		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() {

			this.OnHome = null;
			this.OnClose = null;
			this.OnCharaChange = null;
		}
		#endregion

		#region アクティブ
		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		public void SetActive( bool isActive, bool isTweenSkip ) {

			this.SetRootActive( isActive, isTweenSkip );
		}

		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		public GUIViewBase.ActiveState GetActiveState() {

			return this.GetRootActiveState();
		}
		#endregion

		#region ホーム、閉じるボタンイベント
		public event EventHandler<EventArgs> OnHome = ( sender, e ) => { };
		public event EventHandler<EventArgs> OnClose = ( sender, e ) => { };

		/// <summary>
		/// ホームボタンイベント
		/// </summary>
		public override void OnHomeEvent() {

			// 通知
			this.OnHome( this, EventArgs.Empty );
		}

		/// <summary>
		/// 閉じるボタンイベント
		/// </summary>
		public override void OnCloseEvent() {

			// 通知
			this.OnClose( this, EventArgs.Empty );
		}
		#endregion

		#region キャラ変更ボタン
		public event EventHandler<EventArgs> OnCharaChange = ( sender, e ) => { };

		/// <summary>
		/// キャラ変更ボタンイベント
		/// </summary>
		public void OnCharaChangeEvent() {

			// 通知
			this.OnCharaChange( this, EventArgs.Empty );
		}

		[SerializeField]
		UIButton _charaChangeButton = null;
		UIButton CharaChangeButton { get { return _charaChangeButton; } }

		/// <summary>
		/// キャラ変更ボタンの有効化
		/// </summary>
		public void SetCharaChangeButtonEnable( bool isEnable ) {

			if( this.CharaChangeButton != null ) {
				this.CharaChangeButton.isEnabled = isEnable;
			}
		}
		#endregion

		#region 現在のキャラクター
		[SerializeField]
		UILabel _symbolCharaNameLabel = null;
		UILabel SymbolCharaNameLabel { get { return _symbolCharaNameLabel; } }

		/// <summary>
		/// 現在のキャラクター名設定
		/// </summary>
		public void SetSymbolCharaName( string format, string name ) {

			if( this.SymbolCharaNameLabel != null ) {
				this.SymbolCharaNameLabel.text = string.Format( format, name );
			}
		}
		#endregion

		#region 選択中のキャラクター
		[SerializeField]
		UILabel _selectCharaNameLabel = null;
		UILabel SelectCharaNameLabel { get { return _selectCharaNameLabel; } }

		/// <summary>
		/// 選択中のキャラクター名設定
		/// </summary>
		public void SetSelectCharaName( string format, string name ) {

			if( this.SelectCharaNameLabel != null ) {
				this.SelectCharaNameLabel.text = string.Format( format, name );
			}
		}
		#endregion
	}
}
