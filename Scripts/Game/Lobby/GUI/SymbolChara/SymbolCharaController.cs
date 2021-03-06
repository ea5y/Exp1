/// <summary>
/// シンボルキャラクター制御
/// 
/// 2016/03/28
/// </summary>

using System;
using System.Collections.Generic;

namespace XUI.SymbolChara {

	#region キャラクター変更イベント
	/// <summary>
	/// キャラクター変更イベント引数
	/// </summary>
	public class CharaChangeEventArgs : EventArgs {
		public ulong SelectCharaUUID { get; set; }
	}
	#endregion

	/// <summary>
	/// シンボルキャラクター制御インターフェイス
	/// </summary>
	public interface IController {

		#region 初期化
		/// <summary>
		/// 初期化
		/// </summary>
		void Setup();
		#endregion

		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();
		#endregion

		#region アクティブ設定
		/// <summary>
		/// アクティブ設定
		/// </summary>
		void SetActive( bool isActive, bool isTweenSkip, bool reset );
		#endregion

		#region キャラ変更イベント
		/// <summary>
		/// キャラ変更イベント
		/// </summary>
		event EventHandler<CharaChangeEventArgs> OnCharaChange;
		#endregion

		#region キャラ設定
		/// <summary>
		/// キャラリスト枠を設定
		/// </summary>
		void SetupCapacity( int capacity, int count );

		/// <summary>
		/// キャラリストの中身設定
		/// </summary>
		void SetupItem( List<CharaInfo> list );
		#endregion
	}

	/// <summary>
	/// シンボルキャラクター制御
	/// </summary>
	public class Controller : IController {

		#region 文字列
		string ScreenTitle { get { return MasterData.GetText( TextType.TX402_SymbolChara_Title ); } }
		string BaseHelpMessage { get { return MasterData.GetText( TextType.TX403_SymbolChara_HelpMessage ); } }
		string DialogMessage { get { return MasterData.GetText( TextType.TX404_SymbolChara_CharaChangeMessage ); } }
		#endregion

		#region フィールド＆プロパティ
		// モデル
		readonly IModel _model;
		IModel Model { get { return _model; } }

		// ビュー
		readonly IView _view;
		IView View { get { return _view; } }

		/// <summary>
		/// 更新できる状態かどうか
		/// </summary>
		bool CanUpdate {
			get {
				if( this.Model == null )	return false;
				if( this.View  == null )	return false;
				return true;
			}
		}

		// キャラリスト
		readonly GUICharaPageList _charaList;
		GUICharaPageList CharaList { get { return _charaList; } }

		// キャラ
		readonly GUICharaItem _symbolChara;
		GUICharaItem SymbolChara { get { return _symbolChara; } }

		readonly GUICharaItem _selectChara;
		GUICharaItem SelectChara { get { return _selectChara; } }

		GUICharaItem _oldSelectChara;
		CharaInfo _oldSelectCharaInfo;
		GUICharaItem OldSelectChara {
			get {
				return _oldSelectChara;
			}
			set {
				_oldSelectChara = value;
				_oldSelectCharaInfo = ( _oldSelectChara != null )?	_oldSelectChara.GetCharaInfo() : null;
			}
		}

		// 現在キャラUUID
		ulong SymbolCharaUUID {
			get {
				var info = this.SymbolCharaInfo;
				if( info != null ) {
					return info.UUID;
				}
				return 0;
			}
		}

		// 選択キャラUUID
		ulong SelectCharaUUID {
			get {
				var info = this.SelectCharaInfo;
				if( info != null ) {
					return info.UUID;
				}
				return 0;
			}
		}

		// 前回選択キャラUUID
		ulong OldSelectCharaUUID {
			get {
				var info = this.OldSelectCharaInfo;
				if( info != null ) {
					return info.UUID;
				}
				return 0;
			}
		}

		// 現在キャラ情報
		CharaInfo SymbolCharaInfo { get { return ( this.SymbolChara != null ? this.SymbolChara.GetCharaInfo() : null ); } }

		// 選択キャラ情報
		CharaInfo SelectCharaInfo { get { return ( this.SelectChara != null ? this.SelectChara.GetCharaInfo() : null ); } }

		// 前回選択キャラ情報
		CharaInfo OldSelectCharaInfo { get { return ( this._oldSelectCharaInfo != null ? this._oldSelectCharaInfo : null ); } }

		// 選択キャラが空かどうか
		bool IsEmptySelectChara { get { return ( this.SelectCharaInfo == null ? true : false ); } }

		// 加工していない大元のキャラリスト
		List<CharaInfo> RawList { get; set; }

		// シリアライズされていないメンバーの初期化
		void MemberInit() {
			this.RawList = new List<CharaInfo>();
		}
		#endregion

		#region 初期化
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Controller( IModel model, IView view, GUICharaPageList charaList, GUICharaItem symbolChara, GUICharaItem selectChara ) {

			if( model == null || view == null )	return;

			this._charaList = charaList;
			this._symbolChara = symbolChara;
			this._selectChara = selectChara;

			this.MemberInit();

			// ビュー設定
			this._view = view;
			this.View.OnHome		+= this.HandleHome;
			this.View.OnClose		+= this.HandleClose;
			this.View.OnCharaChange	+= this.HandleCharaChange;

			// モデル設定
			this._model = model;

			// イベント登録
			if( this.CharaList != null ) {
				this.CharaList.OnItemChangeEvent		+= this.HandleCharaListItemChangeEvent;
				this.CharaList.OnItemClickEvent			+= this.HandleCharaListItemClickEvent;
				this.CharaList.OnItemLongPressEvent		+= this.HandleCharaListItemLongPressEvent;
				this.CharaList.OnUpdateItemsEvent		+= this.HandleCharaListUpdateItemsEvent;
			}

			// 現在選択中のキャラクターアイコン
			if( SelectChara != null ) {
				SelectChara.OnItemClickEvent += this.HandleSelectCharClick;

				SelectChara.SetSelect( true );
			}
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() {

			this.OnCharaChange = null;

			if( this.View != null ) {
				this.View.Dispose();
			}
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void Setup() {

			this.MemberInit();

			this.ClearCharaList();
			this.ClearSymbolChara();
			this.ClearSelectChara();

			// 同期
			this.SyncCharaList();
			this.SyncSymbolCharaName();
			this.SyncSelectCharaName( null );

			// ボタン初期化
			UpdateCharaChangeButtonEnable();
		}
		#endregion

		#region アクティブ設定
		/// <summary>
		/// アクティブ設定
		/// </summary>
		public void SetActive( bool isActive, bool isTweenSkip, bool reset ) {

			if( this.CanUpdate ) {
				this.View.SetActive( isActive, isTweenSkip );

				if( reset ) {
					if( this.CharaList != null ) {
						this.SyncCharaList();
					}
				}

				// その他UIの表示設定
				GUILobbyResident.SetActive( !isActive );
				GUIScreenTitle.Play( isActive, this.ScreenTitle );
				GUIHelpMessage.Play( isActive, this.BaseHelpMessage );
			}
		}
		#endregion

		#region 各状態を更新する

		/// <summary>
		/// キャラ変更ボタンの状態を更新する
		/// </summary>
		void UpdateCharaChangeButtonEnable() {

			if( !this.CanUpdate )	return;

			// キャラが空かどうか
			bool isEnable = !this.IsEmptySelectChara;

			this.View.SetCharaChangeButtonEnable( isEnable );
		}

		#endregion

		#region 表示直結系

		#region 現在のキャラクター
		void HandleSymbolCharChange( object sender, EventArgs e ) { this.SyncSymbolCharaName(); }
		void SyncSymbolCharaName() {

			if( this.CanUpdate ) {
				this.SetSymbolChara( this.SymbolChara.GetCharaInfo());
			}
		}
		void ClearSymbolChara() {

			if( this.CanUpdate ) {
				this.SetCharaItem( this.SymbolChara, null );
			}
		}
		#endregion

		#region 選択中のキャラクター
		void HandleSelectCharChange( GUICharaItem item ) { this.SyncSelectCharaName( item ); }
		void SyncSelectCharaName( GUICharaItem item ) {

			if( this.CanUpdate ) {
				this.SetSelectChara(( item != null )? item.GetCharaInfo() : null );
			}
		}
		void ClearSelectChara() {

			if( this.CanUpdate ) {
				// 選択キャラの解除
				RemoveSelectChara();
			}
		}
		#endregion

		#region 選択中のキャラクターを外す
		void HandleSelectCharClick( GUICharaItem item ) { this.RemoveSelectChara(); }
		void RemoveSelectChara() {

			if( this.CanUpdate ) {
				if( OldSelectChara != null ) {
					// 選択解除
					OldSelectChara.SetDisableState( CharaItem.Controller.DisableType.None );
					OldSelectChara.SetSelect( false );
					OldSelectChara = null;
				}
				this.SetSelectChara( null );

				// ボタン状態設定
				UpdateCharaChangeButtonEnable();
			}
		}
		#endregion

		#endregion

		#region ホーム、閉じるボタンイベント
		void HandleHome( object sender, EventArgs e ) {

			if( CharaList != null ) {
				// Newフラグ一括解除
				CharaList.DeleteAllNewFlag();
			}

			GUIController.Clear();
		}

		void HandleClose( object sender, EventArgs e ) {

			if( CharaList != null ) {
				// Newフラグ一括解除
				CharaList.DeleteAllNewFlag();
			}

			GUIController.Back();
		}
		#endregion

		#region キャラ変更イベント
		public event EventHandler<CharaChangeEventArgs> OnCharaChange = ( sender, e ) => { };

		void HandleCharaChange( object sender, EventArgs e ) {

			// 確認
			GUIMessageWindow.SetModeYesNo( DialogMessage, _charaChange, null );
		}

		/// <summary>
		/// キャラ変更通知
		/// </summary>
		private void _charaChange() {

			// 選択中のキャラクターのUUIDを設定
			CharaChangeEventArgs eventArgs = new CharaChangeEventArgs();
			eventArgs.SelectCharaUUID = this.SelectChara.GetCharaInfo().UUID;

			// 通知
			this.OnCharaChange( this, eventArgs );

			// 閉じる
			HandleClose( this, EventArgs.Empty );
		}

		#endregion

		#region キャラリスト設定

		#region キャラアイテム操作

		void HandleCharaListItemChangeEvent( GUICharaItem obj ) { this.UpdateItemType( obj ); }
		void HandleCharaListItemClickEvent( GUICharaItem obj ) { this.SetSelectCharItem( obj ); }
		void HandleCharaListItemLongPressEvent( GUICharaItem obj ) { }
		void HandleCharaListUpdateItemsEvent() {
			if( this.CharaList != null ) {
				var list = this.CharaList.GetNowPageItemList();
				list.ForEach( this.UpdateItemType );
			}
		}

		/// <summary>
		/// キャラリストを同期
		/// </summary>
		void SyncCharaList() {

			if( this.CharaList == null ) return;

			// 各キャラ情報が選択できるか設定する
			var list = this.CharaList.GetCharaInfo();
			this.UpdateCanSelect( list );

			// キャラリスト更新
			this.CharaList.SetupItems( list );
		}

		/// <summary>
		/// キャラリスト枠を設定
		/// </summary>
		public void SetupCapacity( int capacity, int count ) {

			if( this.CharaList != null ) {
				this.CharaList.SetupCapacity( capacity, count );
			}
		}

		/// <summary>
		/// キャラリストの中身設定
		/// </summary>
		public void SetupItem( List<CharaInfo> list ) {

			this.RawList = list;
			this.UpdateCharaList();
		}

		/// <summary>
		/// キャラリストの中身更新
		/// </summary>
		void UpdateCharaList() {

			// リストから現在のキャラ情報を更新する
			this.UpdateSymbolChara( this.RawList );

			// 選択できるかどうかの情報を更新する
			this.UpdateCanSelect( this.RawList );

			// キャラリストの中身を更新する
			if( this.CharaList != null ) {
				this.CharaList.Setup();
				this.CharaList.SetupItems( this.RawList );
			}
		}

		/// <summary>
		/// キャラリストから現在キャラの中身を更新する
		/// </summary>
		void UpdateSymbolChara( List<CharaInfo> list ) {

			if( list == null )	return;
			if( this.SymbolChara == null )	return;

			// 現在のキャラ情報を更新する
			var symbolInfo = list.Find( t => { return t.IsSymbol; } );
			this.SetSymbolChara( symbolInfo );
		}

		/// <summary>
		/// 選択したキャラクターを表示
		/// </summary>
		/// <param name="item"></param>
		void SetSelectCharItem( GUICharaItem item ) {

			if( item == null ) {
				return;
			}

			var info = item.GetCharaInfo();

			if( info == null ) {
				// 選択状態
				item.SetSelect( false );
				item.SetDisableState( CharaItem.Controller.DisableType.None );
				return;
			}
			if( info.IsSymbol ) {
				return;
			}

			// 前回と同キャラかどうかチェック
			bool check = ( item == OldSelectChara );

			// 選択解除
			RemoveSelectChara();

			// 同キャラでなければ変更
			if( !check ) {

				// 選択アイテム変更
				OldSelectChara = item;
				this.SetSelectChara( info );

				// 選択状態
				item.SetDisableState( CharaItem.Controller.DisableType.Select );
			}

			// ボタン状態設定
			UpdateCharaChangeButtonEnable();
		}

		/// <summary>
		/// キャラリストクリア
		/// </summary>
		void ClearCharaList() {

			if( this.CharaList != null ) {
				this.CharaList.SetupItems( null );
			}
		}
		#endregion

		#region キャラ情報
		/// <summary>
		/// 選択できるかどうかの情報を更新する
		/// </summary>
		void UpdateCanSelect( List<CharaInfo> list ) {

			if( list == null )	return;

			// 各キャラ情報の選択出来るかどうかの情報を更新する
			list.ForEach( this.UpdateCanSelect );
		}

		/// <summary>
		/// 選択できるかどうかの情報を更新する
		/// </summary>
		void UpdateCanSelect( CharaInfo info ) {

			if (info == null) return;

			// タイプを取得する
			var disableType = CharaItem.Controller.DisableType.None;
			this.GetBaseCharaType( info, out disableType );

			// 選択できるか設定する
			info.CanSelect = ( disableType == CharaItem.Controller.DisableType.None );
		}

		/// <summary>
		/// キャラタイプを取得する
		/// </summary>
		void GetBaseCharaType( CharaInfo info, out CharaItem.Controller.DisableType disableType ) {

			disableType = XUI.CharaItem.Controller.DisableType.None;
			if( info == null )	return;

			if( info.UUID == this.SymbolCharaUUID ) {
				// シンボルチェック
				disableType = CharaItem.Controller.DisableType.Symbol;

			} else if( info.UUID == this.OldSelectCharaUUID ) {
				// 選択中チェック
				disableType = CharaItem.Controller.DisableType.Select;
			}
		}

		#endregion

		#region キャラアイテム
		/// <summary>
		/// アイテムのタイプ更新
		/// </summary>
		void UpdateItemType( GUICharaItem item ) {

			if( item == null )	return;

			// タイプを取得する
			var disableType = CharaItem.Controller.DisableType.None;
			this.GetBaseCharaType( item.GetCharaInfo(), out disableType );

			// 選択中キャラかどうかの確認
			if( this.SelectChara != null ) {
				// 選択状態の設定
				CharaInfo info = item.GetCharaInfo();
				if( info != null ) {
					// 現在のシンボルキャラクターかどうかチェック
					if( info.UUID == this.SymbolCharaUUID ) {
						// シンボル設定
						disableType = CharaItem.Controller.DisableType.Symbol;
					}
				}
			}
			// タイプを設定する
			item.SetDisableState( disableType );
		}

		/// <summary>
		/// キャラアイテムを設定する
		/// </summary>
		void SetCharaItem( GUICharaItem item, CharaInfo info ) {

			if( item == null )	return;

			var state = CharaItem.Controller.ItemStateType.Icon;

			if( info == null || info.IsEmpty ) {
				state = CharaItem.Controller.ItemStateType.FillEmpty;
			}
			item.SetState( state, info );
		}
		#endregion

		#endregion

		#region キャラ設定

		/// <summary>
		/// 現在キャラ設定
		/// </summary>
		void SetSymbolChara( CharaInfo info ) {

			if( this.SymbolChara == null ) return;

			this.SymbolChara.SetCharaInfo( info );
			this.SetCharaItem( this.SymbolChara, info );

			this.View.SetSymbolCharaName( Model.SymbolNameFormat, ( info != null ) ? info.Name : "" );
		}

		/// <summary>
		/// 選択中キャラ設定
		/// </summary>
		/// <param name="info"></param>
		void SetSelectChara( CharaInfo info ) {

			if( this.SelectChara == null ) return;

			this.SetCharaItem( this.SelectChara, info );

			this.View.SetSelectCharaName( Model.SelectNameFormat, ( info != null ) ? info.Name : "" );
		}

		#endregion
	}
}
