using UnityEngine;
using System.Collections;
using System;

public class GUIEvolutionDirectionRank : MonoBehaviour
{
	#region === Field ===
	

	[SerializeField]
	private GameObject oldRankGroup;

	[SerializeField]
	private UILabel oldRankTextLabel;

	[SerializeField]
	private UITweener oldRankTweener;


	[SerializeField]
	private GameObject newRankGroup;

	[SerializeField]
	private UILabel newRankTextLabel;

	[SerializeField]
	private UILabel newRankGlowLabel;

	[SerializeField]
	private float rankUpTime = 0.5f;

	private bool isRankUp = false;

	private Action callback;

	private float waitTime = 0;

	private bool isWait = false;

	#endregion === Field ===




	public void EffectStart(int oldRank, int newRank, Action callback)
	{
		isWait = false;
		waitTime = 0;

		this.callback = callback;
		isRankUp = (oldRank != newRank);


		oldRankTextLabel.text = oldRank.ToString();
		EventDelegate.Add(oldRankTweener.onFinished, OnRankFinished, true);

		newRankTextLabel.text = newRank.ToString();
		newRankGlowLabel.text = newRank.ToString();
	}

	private void OnRankFinished()
	{
		if(isRankUp) {
			oldRankGroup.SetActive(false);
			newRankGroup.SetActive(true);

			isWait = true;
			waitTime = rankUpTime;
		}else{
			if(callback != null) {
				callback();
				callback = null;
			}
		}
	}

	public void Close()
	{
		waitTime = 0;
		isWait = false;

		callback = null;
	}

	private void Update()
	{
		if(!isWait) return;

		waitTime -= Time.deltaTime;

		if(waitTime < 0) {
			waitTime = 0;
			isWait = false;

			if(callback != null) {
				callback();
				callback = null;
			}
		}

	}

}
