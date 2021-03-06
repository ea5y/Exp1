/// <summary>
/// タッチシステム
/// 
/// 2012/12/25
/// </summary>
using UnityEngine;
using System.Collections;

public class TouchSystem : Singleton<TouchSystem>
{
	#region フィールド＆プロパティ
	/// <summary>
	/// ピンチタイプ
	/// </summary>
	public PinchTypes PinchType { get; set; }
	public enum PinchTypes
	{
		CameraZoom,
		MapWindowZoom,
	};

	/// <summary>
	/// タップイベント.
	/// </summary>
	public event System.Action<Vector3> OnTapEvent;

	/// <summary>
	/// カメラズームデリゲート
	/// </summary>
	public System.Action<float> OnCameraZoom { get; set; }

	/// <summary>
	/// フリックロックオンデリゲート
	/// </summary>
	public System.Action<FingerGestures.SwipeDirection, Vector2> OnFlickLockon { get; set; }

	/// <summary>
	/// タッチフィルターデリゲート
	/// </summary>
	public FingerGestures.GlobalTouchFilterDelegate GlobalTouchFilter { get; set; }

	/// <summary>
	/// カメラ回転時のドラッグ距離
	/// </summary>
	public Vector2 Rotation { get { return this.RotationDrag.Drag; } }

	/// <summary>
	/// カメラ回転時の全快からの移動した距離
	/// </summary>
	public Vector2 RotationDelta { get { return this.RotationDrag.Delta; } }

	// ドラッグ回転
	DragParam RotationDrag { get; set; }
	[System.Serializable]
	public class DragParam
	{
		public int FingerIndex { get; set; }
		public Vector2 Drag { get; set; }	// StartPosからのドラッグ距離
		public Vector2 Delta { get; set; }	// 前回からの移動距離
		public DragParam() { Init(); }
		public void Init()
		{
			this.FingerIndex = -1;
			this.Drag = Vector2.zero;
			this.Delta = Vector2.zero;
		}
	}

	// シリアライズされていないメンバーの初期化
	void MemberInit()
	{
		this.PinchType = PinchTypes.CameraZoom;
		this.OnCameraZoom = delegate { };
		this.OnFlickLockon = delegate { };
		this.GlobalTouchFilter = null;
		this.RotationDrag = new DragParam();
	}
	#endregion

	#region 初期化
	void Start()
	{
		this.MemberInit();
	}
	void OnEnable()
	{
		// タッチフィルター
		FingerGestures.GlobalTouchFilter += this.OnGlobalTouchFilter;
	}
	void OnDisable()
	{
		// タッチフィルター
		FingerGestures.GlobalTouchFilter -= this.OnGlobalTouchFilter;
	}
	#endregion

	#region タッチフィルター
	/// <summary>
	/// タッチフィルター
	/// </summary>
	/// <returns>true=フィルターオフ false=フィルターオン</returns>
	/// <param name="fingerIndex"></param>
	/// <param name="position"></param>
	bool OnGlobalTouchFilter(int fingerIndex, Vector2 position)
	{
		if (this.GlobalTouchFilter == null)
			return true;
		// フィルターオフ(false)が一つでもあるとフィルターを掛ける
		// foreach にしているのは複数関数登録してある時に
		// 最後に呼び出した関数の戻り値を返してしまう為
		foreach (FingerGestures.GlobalTouchFilterDelegate func in this.GlobalTouchFilter.GetInvocationList())
		{
			if (func(fingerIndex, position))
				continue;
			return false;
		}
		return true;
	}
	#endregion

	#region タップ(ロックオン)
	/// <summary>
	/// タップ
	/// </summary>
	void OnTap(TapGesture gesture)
	{
		if (this.OnTapEvent != null)
			this.OnTapEvent(gesture.Position);
	}
	#endregion

	#region ダブルタップ(カメラリセット)
	/// <summary>
	/// タブルタップ
	/// </summary>
	void OnDoubleTap(TapGesture gesture)
	{
		//this.CameraReset();
	}
	/// <summary>
	/// カメラリセット
	/// </summary>
	void CameraReset()
	{
		CharacterCamera camera = GameController.CharacterCamera;
		if (camera == null)
			return;
		camera.CameraReset();
	}
	#endregion

	#region ドラッグ(カメラ回転)
	/// <summary>
	/// ドラッグ
	/// </summary>
	void OnDrag(DragGesture gesture)
	{
	    this.CameraRotation(gesture);
	}
	/// <summary>
	/// カメラ回転
	/// </summary>
	void CameraRotation(DragGesture gesture)
	{
		var finger = gesture.Fingers[0];

		if (gesture.Phase == ContinuousGesturePhase.Started)
		{
			// 既にドラッグされている
			if (this.RotationDrag.FingerIndex != -1)
				return;

			this.RotationDrag.FingerIndex = finger.Index;
		}
		else if (finger.Index == this.RotationDrag.FingerIndex)
		{
			if (gesture.Phase == ContinuousGesturePhase.Updated)
			{
				// ドラッグした距離を計算する
				this.RotationDrag.Drag = finger.Position - finger.StartPosition;
				// 前回からの移動距離を保存
				this.RotationDrag.Delta = finger.DeltaPosition;
			}
			else
			{
				// 初期化
				this.RotationDrag.Init();
			}
		}
	}
	#endregion

	#region ピンチ(カメラズーム)
	void OnPinch(PinchGesture gesture)
	{
		if (this.PinchType == PinchTypes.CameraZoom)
			this.CameraZoom(gesture);
		else
			this.MapWindowZoom(gesture);
	}
	void CameraZoom(PinchGesture gesture)
	{
		if (this.OnCameraZoom != null)
			this.OnCameraZoom(gesture.Delta);
	}
	void MapWindowZoom(PinchGesture gesture)
	{
		if (gesture.Phase == ContinuousGesturePhase.Started)
		{
		}
		else if (gesture.Phase == ContinuousGesturePhase.Updated)
		{
		}
		else
		{
		}
	}
	#endregion

	#region ツイスト
	public void OnTwist(TwistGesture gesture)
	{
		if (gesture.Phase == ContinuousGesturePhase.Started)
		{
		}
		else if (gesture.Phase == ContinuousGesturePhase.Updated)
		{
		}
		else
		{
		}
	}
	#endregion

	#region 長押し
	// FingerGestures v3.1 Sample
	//void OnLongPress( LongPressGesture gesture )
	//{
	//    if( gesture.Selection == longPressObject )
	//    {
	//        SpawnParticles( longPressObject );
	//        UI.StatusText = "Performed a long-press with finger " + gesture.Fingers[0];
	//    }
	//}
	#endregion

	#region スワイプ(次のロックオン)
	void OnSwipe(SwipeGesture gesture)
	{
		this.FlickLockon(gesture.Direction, gesture.Move);

	}
	void FlickLockon(FingerGestures.SwipeDirection direction, Vector2 move)
	{
		if (this.OnFlickLockon != null)
			this.OnFlickLockon(direction, move);
	}
	#endregion
}
