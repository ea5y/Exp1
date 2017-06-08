using UnityEngine;
using System.Collections;

/// <summary>
/// For Android call
/// </summary>
public class AndroidMessageController : MonoBehaviour {
    
    //TalkingData
#if ANDROID_XY 
    public void OnRegisterSuccess(string msg)
    {
        Debug.Log("RegisterSuccess! UserId: " + msg);
        AndroidTalkingDataSDKController.Instance.OnRegister(msg);
    }

    public void OnLoginSuccess(string msg)
    {
        Debug.Log("LoingSuccess! UserId: " + msg);
        AndroidTalkingDataSDKController.Instance.OnLogin(msg);
    }

    public void OnPaySuccess(string msg)
    {
        Debug.Log("PaySuccess! Msg: " + msg);

        //Tip: strArry[0] = orderId, strArry[1] = price(单位元), strArry[2] = payType
        var strArry = msg.Split(','); 
        Net.Network.Instance.StartCoroutine( Net.Network.AfterBuy((res) => {}));

        int amount = (int)(double.Parse(strArry[1]) * 100.00);
        Debug.Log("Amount: " + amount);
        AndroidTalkingDataSDKController.Instance.OnPay(strArry[0], amount, "CNY", strArry[2]);
    }
#endif
}
