using UnityEngine;
using System.Collections;

public class GUIEvolutionDirectionText : MonoBehaviour
{
	#region === Field ===

	[SerializeField]
	private GameObject evoTextGroup;


	[SerializeField]
	private GUIEvolutionDirectionRank rankDirection;
	

	[SerializeField]
	private GameObject synchroTextGroup;

	[SerializeField]
	private UILabel synchroTextLabel;
	


	private GameObject evoTextObj;

	private GUIEvolutionDirectionRank rankTextObj;

	private GameObject synchroTextObj;

	#endregion === Field ===


	private void DestroyObject()
	{
		if(evoTextObj != null) {
			Destroy(evoTextObj);
			evoTextObj = null;
		}
		if(rankTextObj != null) {
			rankTextObj.Close();
			Destroy(rankTextObj.gameObject);
			rankTextObj = null;
		}
		if(synchroTextObj != null) {
			Destroy(synchroTextObj);
			synchroTextObj = null;
		}
	}


	public void EffectStart(int oldRank, int newRank, int syncUp)
	{
		DestroyObject();
		
		synchroTextLabel.text = syncUp.ToString();
		
		rankTextObj = SafeObject.Instantiate(rankDirection);
		rankTextObj.EffectStart(oldRank, newRank, OnRankTweenFinished);
		rankTextObj.transform.SetParent(transform, false);
		rankTextObj.gameObject.SetActive(true);

		if(oldRank != newRank) {
			evoTextObj = SafeObject.Instantiate(evoTextGroup);
			evoTextObj.transform.SetParent(transform, false);
			evoTextObj.SetActive(true);
		}

		synchroTextObj = SafeObject.Instantiate(synchroTextGroup);
		synchroTextObj.transform.SetParent(transform, false);
	}

	private void OnRankTweenFinished()
	{
		if(synchroTextObj != null) {
			synchroTextObj.SetActive(true);
		}
	}
	
	public void Close()
	{


		DestroyObject();

	}

}
