/// <summary>
/// メッセージーカテゴリークラス
/// 設定されたカテゴリーごとにメッセージを制御していく
/// 特殊な処理を持つカテゴリーを作成する場合はこのカテゴリークラスを継承して作成する
/// 
/// 2014/08/19
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;

public class GUIEffectMessageCategory : MonoBehaviour
{
	#region　アタッチオブジェクト

	[System.Serializable]
	public class AttachObject
	{
		/// <summary>
		/// 生成元のエフェクトメッセージアイテム
		/// </summary>
		public List<GUIEffectMsgItemBase> prefabList = new List<GUIEffectMsgItemBase>();

		/// <summary>
		/// 各レイヤーごとにアイテムを追加する親オブジェクト
		/// </summary>
		public List<GameObject> parentList = new List<GameObject>();
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
	/// メッセージコントローラ
	/// </summary>
	protected GUIEffectMessageController controller;

	/// <summary>
	/// キーをメッセージタイプ 値を生成元のエフェクトメッセージアイテムとしたリスト
	/// </summary>
	protected Dictionary<GUIEffectMessage.MsgType, GUIEffectMsgItemBase> msgResourceDictionary = new Dictionary<GUIEffectMessage.MsgType, GUIEffectMsgItemBase>();
	
	#endregion

	#region 初期化

	/// <summary>
	/// 初期化処理
	/// </summary>
	public void Init(GUIEffectMessageController.Type controllerType)
	{
		// カテゴリーセット
		this.controller = GUIEffectMessageController.CreateController(controllerType);

		// キーをメッセージタイプ 値を生成元のエフェクトメッセージアイテムとした一覧を作成する
		this.msgResourceDictionary.Clear();
		foreach(GUIEffectMsgItemBase item in this.Attach.prefabList)
		{
			if(item == null)
				continue;
			this.msgResourceDictionary.Add(item.MsgType, item);
		}
	}

	#endregion

	#region メッセージのセット

	/// <summary>
	/// メッセージをセットする
	/// </summary>
	public void SetMessage(GUIEffectMessage.MsgType msgType)
	{
		// プレハブ情報取得
		GUIEffectMsgItemBase itemResource;
		if(!this.msgResourceDictionary.TryGetValue(msgType, out itemResource))
			return;
        Debug.LogError("===> GameStart Finded MsgType " + msgType);
		// メッセージアイテム生成
		GUIEffectMsgItemBase newItem = GUIEffectMsgItemBase.Create(itemResource, this.Attach.parentList, true);
		if(newItem == null)
			return;

		// メッセージコントロールクラスに登録
		this.controller.SetEffectMessage(newItem);
        Debug.LogError("===> GameStart  Finished " + msgType);
	}
	
	#endregion

	#region 更新

	/// <summary>
	/// 更新
	/// </summary>
	protected virtual void Update ()
	{
		if(this.controller == null)
			return;

		this.controller.Update();
	}

	#endregion
}