using UnityEngine;
using System.Collections;
using System;

public class GUISynchroDirectionText : MonoBehaviour
{

	private static string GetTypeName(UpTypeName type)
	{
		switch(type) {
			case UpTypeName.HP: return MasterData.GetText(TextType.TX253_CharaInfo_HitPoint);
			case UpTypeName.Attack: return MasterData.GetText(TextType.TX254_CharaInfo_Attack);
			case UpTypeName.Defense: return MasterData.GetText(TextType.TX255_CharaInfo_Defense);
			case UpTypeName.Extra: return MasterData.GetText(TextType.TX256_CharaInfo_Extra);
		}
		return "";
	}



	private enum UpTypeName
	{
		HP,
		Attack,
		Defense,
		Extra
	}


	private struct UpType
	{
		public UpTypeName type;

		public bool isMax;

		public UpType(UpTypeName type, bool max)
		{
			this.type = type;
			isMax = max;
		}
	}



	[Serializable]
	private class SynchroUpMessage
	{
		public GameObject rootGroup= null;

		public GameObject upGroup= null;

		public GameObject maxGroup = null;

		public UILabel categoryText = null;
		
		public UILabel categoryMaxText = null;
		
		//private bool isMax = false;

		public void SetInvisible()
		{
			if(upGroup != null) {
				upGroup.SetActive(false);
			}
			if(maxGroup != null) {
				maxGroup.SetActive(false);
			}
		}

		public void SetCategory(bool isMax, string name)
		{
			//this.isMax = isMax;

			if(isMax) {
				if(upGroup != null) {
					upGroup.SetActive(false);
				}
				if(maxGroup != null) {
					maxGroup.SetActive(true);
				}

				if(categoryMaxText != null) categoryMaxText.text = name;
			} else {
				if(upGroup != null) {
					upGroup.SetActive(true);
				}
				if(maxGroup != null) {
					maxGroup.SetActive(false);
				}
				
				if(categoryText != null) categoryText.text = name;
			}
		}

		//public void Start(EventDelegate.Callback callback)
		//{
		//	UITweener tweener = null;
		//	if(isMax) {
		//		if(upGroup != null) {
		//			upGroup.SetActive(false);
		//		}
		//		if(maxGroup != null) {
		//			maxGroup.SetActive(true);
		//			tweener = maxGroup.GetComponent<UITweener>();
		//		}
		//	} else {

		//		if(upGroup != null) {
		//			upGroup.SetActive(true);
		//			tweener = upGroup.GetComponent<UITweener>();
		//		}
		//		if(maxGroup != null) {
		//			maxGroup.SetActive(false);
		//		}

		//	}

		//	if(tweener != null && callback != null) {
		//		EventDelegate.Add(tweener.onFinished, callback, true);
		//	}
		//}

	}

	#region === Field ===

	[SerializeField]
	private GameObject synchroParamObject;

	[SerializeField]
	private SynchroUpMessage[] synchroUpTextList = null;
	
	private GameObject synchroObj;
	

	#endregion === Field ===


	private void DestroyObject()
	{
		if(synchroObj != null) {
			Destroy(synchroObj);
			synchroObj = null;
		}
	}


	public void EffectStart(int hpUp, bool hpMax, int atkUp, bool atkMax, int defUp, bool defMax, int exUp, bool exMax)
	{
		DestroyObject();
		
		int upTotal = hpUp + atkUp + defUp + exUp;
		UpType []upTypeList = new UpType[upTotal];
		
		int i = 0;
		// ステータスアップ部分の表示切り替え
		for(i = 0; i < synchroUpTextList.Length; i++) {
			if(synchroUpTextList[i].rootGroup != null) {
				synchroUpTextList[i].rootGroup.SetActive(1 <= upTotal);
			}
		}
		
		int total = 0;

		for(i = 0; i < hpUp; i++, total++) {
			upTypeList[total] = new UpType(UpTypeName.HP, (hpMax && (i + 1 == hpUp)));
		}
		for(i = 0; i < atkUp; i++, total++) {
			upTypeList[total] = new UpType(UpTypeName.Attack, (atkMax && (i + 1 == atkUp)));
		}
		for(i = 0; i < defUp; i++, total++) {
			upTypeList[total] = new UpType(UpTypeName.Defense, (defMax && (i + 1 == defUp)));
		}
		for(i = 0; i < exUp; i++, total++) {
			upTypeList[total] = new UpType(UpTypeName.Extra, (exMax && (i + 1 == exUp)));
		}

		// ステータスアップ部分の表示切り替え
		for(i = 0; i < synchroUpTextList.Length; i++) {
			if(i < total) {
				synchroUpTextList[i].SetCategory(upTypeList[i].isMax, GetTypeName(upTypeList[i].type));
			} else {
				synchroUpTextList[i].SetInvisible();
			}
		}


		synchroObj = SafeObject.Instantiate(synchroParamObject);
		synchroObj.transform.SetParent(transform, false);
		synchroObj.SetActive(true);
	}
	
	
	public void Close()
	{


		DestroyObject();

	}

}
