using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class TalkingDataMgr : MonoBehaviour
{
	#if UNITY_IOS
	[DllImport ("__Internal")]
	private static extern void talkingDataInit (string appid);

	[DllImport ("__Internal")]
	private static extern void talkingDataRegister (string userid);

	[DllImport ("__Internal")]
	private static extern void talkingDataLogin (string userid);

	[DllImport ("__Internal")]
	private static extern void talkingDataCreateRole (string userid);

	[DllImport ("__Internal")]
	private static extern void talkingDataPaySuccess (string userid, string orderid, int cost, string currencytype, string paytype);


	public static TalkingDataMgr Instance;

	void Awake(){
		Instance = this;
	}
		
	void Start(){
		#if UNITY_IOS && !UNITY_EDITOR
		talkingDataInit ("5E1987D606D64A47BADABBDBBBB32027");
		Debug.Log ("===> init Finished");
		#endif
	}
		
	void Register(string pUserId){
		talkingDataRegister (pUserId);
		Debug.Log ("===> register Finished");
	}

	public void Login(string pUserId){
		string hasRegister = PlayerPrefs.GetString ("TalkingDataRegister", "");
		if ("true" != hasRegister) {
			Register (pUserId);
			PlayerPrefs.SetString ("TalkingDataRegister", "true");
		} 
		talkingDataLogin (pUserId);
		Debug.Log ("===> login Finished");
	}

	//currncytype:人民币 CNY，港元HKD，台币TWD，美元USD，欧元EUR，英镑GBP，日元JPY。比如：CNY
	public void Pay(string userid, string orderid, int cost){
		talkingDataPaySuccess (userid, orderid, cost, "CNY", "AppleIap");
		Debug.Log ("===> pay Finished");
	}
	#endif
}
