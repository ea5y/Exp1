using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GUIPowerupDirectionText : MonoBehaviour
{
	/// <summary>
	/// Expバー動作
	/// </summary>
	private class ExpProgress
	{
		#region === Field ===
		
		private Transform parent;

		private GameObject lvUpText;
		
		private GameObject lvMaxText;

		private float sliderTime;

		private float startVal;

		private float endVal;

		private int loopCount;


		private float delayTime = 1.2f;

		private float delay = 0;

		private float startTime;

		private float currentTime;

		private float currentStart;

		private float currentEnd;

		private float currentVal;
		
		private int currentLoop;

		private bool isLvMax;

		private bool isStopProgress = false;

		private bool lastLoop = false;

		// 表示中のテキスト
		private GameObject textObject = null;

		#endregion === Field ===

		#region === Property ===

		/// <summary>
		/// 現在の数値
		/// </summary>
		public float CurrentVal
		{
			get { return currentVal; }
		}

		/// <summary>
		/// 実行中か
		/// </summary>
		public bool IsRunning
		{
			get;
			private set;
		}
		
		/// <summary>
		/// 終了しているか
		/// </summary>
		public bool IsEnd
		{
			get;
			private set;
		}

		#endregion === Property ===


		public ExpProgress(Transform parent, GameObject lvUpText, GameObject lvMaxText)
		{
			this.parent = parent;
			this.lvUpText = lvUpText;
			this.lvMaxText = lvMaxText;

			IsRunning = false;
			IsEnd = false;
		}

		public void Start(float expSliderTime, float popupWait, float start, float end, int count, bool lvMax)
		{
			delayTime = popupWait;

			sliderTime	= expSliderTime;
			startVal	= start;
			endVal		= end;
			loopCount	= count;
			isLvMax		= lvMax;

			IsEnd		= false;
			IsRunning	= true;

			lastLoop = (count == 0);
			isStopProgress = false;

			
			currentVal	= startVal;
			currentStart= startVal;
			currentEnd	= (loopCount > 0 ? 1 : end);
			currentLoop	= 0;

			// 時間を残りに変換
			currentTime = startTime = this.sliderTime * (currentEnd - startVal);
		}

		public void Stop()
		{
			IsRunning = false;
			IsEnd = false;
		}

		public void Clear()
		{
			if(textObject != null) {
				Destroy(textObject);
				textObject = null;
			}
		}


		public void Update()
		{
			if(!IsRunning || IsEnd) return;

			if(delay > 0) {
				delay -= Time.deltaTime;
				return;
			}

			if(currentTime > 0) {

				currentTime -= Time.deltaTime;

				// カウンタ終了
				if(currentTime <= 0) {

					// ラスト
					if(lastLoop) {
						IsEnd = true;
					}
					
					currentStart = 0;
					currentTime = startTime = sliderTime;

					if(!IsEnd) {
						delay = delayTime;

						currentVal = 1;
						// ループが終了
						if(++currentLoop >= loopCount) {
							lastLoop = true;
							
							// レベルが最大の時は止める
							if(isLvMax) {
								CreateText(true);
								currentEnd = 1;
								currentVal = 1;
								isStopProgress = true;
							} else {
								CreateText(false);
								currentEnd = endVal;

								// 時間の比率変更必要
								currentTime = startTime = sliderTime * currentEnd;

							}
						} else {
							currentEnd = 1;
							CreateText(false);
						}
					} else {
						currentVal = currentEnd;
					}
				} else {
					if(!isStopProgress) {
						currentVal = Mathf.Lerp(currentStart, currentEnd, 1.0f - (currentTime / startTime));
					}
				}

			}
		}

		private void CreateText(bool lvMax)
		{
			if(textObject != null) {
				Destroy(textObject);
				textObject = null;
			}
			
			if(lvMax) {
				textObject = SafeObject.Instantiate(lvMaxText);
			} else {
				textObject = SafeObject.Instantiate(lvUpText);
			}

			textObject.transform.SetParent(parent, false);
			textObject.SetActive(true);

		}
	}

	#region === Field ===
	
	[SerializeField]
	private float expSliderTime = 1.0f;

	[SerializeField]
	private float lvPopupWaitTime = 1.0f;


	[SerializeField]
	private GameObject expSliderObject;

	[SerializeField]
	private UIProgressBar expSlider;

	[SerializeField]
	private GameObject lvUpText;

	[SerializeField]
	private GameObject lvMaxText;

	private Action callback;

	private ExpProgress progress;

	#endregion === Field ===

	
	private void Awake()
	{
		CreateProgress();
	}

	/// <summary>
	/// 作成
	/// </summary>
	private void CreateProgress()
	{
		if(progress != null) return;

		progress = new ExpProgress(transform, lvUpText, lvMaxText);
	}

	/// <summary>
	/// レベルアップ開始
	/// </summary>
	/// <param name="start"></param>
	/// <param name="count"></param>
	/// <param name="end"></param>
	/// <param name="lvMax"></param>
	/// <param name="callback"></param>
	public void LevelUpStart(float start, int count, float end, bool lvMax, Action callback)
	{
		CreateProgress();

		this.callback = callback;

		progress.Start(expSliderTime, lvPopupWaitTime, start, end, count, lvMax);

		expSlider.value = start;
		expSliderObject.SetActive(true);
	}

	/// <summary>
	/// 終了処理
	/// </summary>
	public void Close()
	{
		if(progress != null) {
			progress.Stop();
			progress.Clear();
		}

		callback = null;

		expSliderObject.SetActive(false);
	}

	private void Update()
	{
		if(progress != null && !progress.IsRunning) return;

		progress.Update();
		expSlider.value = progress.CurrentVal;

		if(progress.IsEnd) {
			progress.Stop();
			callback();
			callback = null;
		}
	}

	

	
}
