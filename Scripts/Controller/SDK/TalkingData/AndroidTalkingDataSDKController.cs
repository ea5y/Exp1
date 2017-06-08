using UnityEngine;
using System.Collections;

public class AndroidTalkingDataSDKController : Singleton<AndroidTalkingDataSDKController>
{
    private AndroidJavaObject currentActiviry;
    private AndroidJavaObject sdkObj;
    private string appId = "E5152FA07E9A4747906A8D8632F41C5E";

    private string isDebug = "false";

    private void Awake()
    {
        base.Awake();


#if !UNITY_EDITOR && ANDROID_XY || !UNITY_EDITOR && EJPL
        using(var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            this.currentActiviry = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }

        using(var obj = new AndroidJavaClass("com.asobimo.xworld_official.AndroidSDK.TalkingData"))
        {
            //Get SDKActivity
            this.sdkObj = obj.CallStatic<AndroidJavaObject>("GetSDKActivity");
        }

        //Get unityContext
        this.sdkObj.Call("GetUnityContext", currentActiviry);
#endif

    }

    /// <summary>
    /// 注册，登陆之前，必须先初始化TalkingData
    /// </summary>
    /// <param name="channelId">渠道Id</param>
    public void Init(string channelId)
    {
        Debug.Log(">>>>>>TalkingDataSDK Init");
        this.sdkObj.Call("Init", this.appId, channelId, this.isDebug);
    }

    public void OnRegister(string userId)
    {
        Debug.Log(">>>>>>TakkingDataSDK OnRegister");
        this.sdkObj.Call("OnRegister", userId);
    }

    public void OnLogin(string userId)
    {
        Debug.Log(">>>>>>TakkingDataSDKInit OnLogin");
        this.sdkObj.Call("OnLogin", userId);
    }

    public void OnCreateRole(string roleName)
    {
        Debug.Log(">>>>>>TakkingDataSDKInit OnCreateRole");
        this.sdkObj.Call("OnCreateRole", roleName);
    }

    /// <summary>
    /// OnPay
    /// </summary>
    /// <param name="orderId">订单ID，最多64个字符，全局唯一，由开发者提供并维护,用于唯一标识一次交易。以及后期系统之间对账使用！*如果多次充值成功的orderID重复，将只计算首次成功的数据，其他数据会认为重复数据丢弃。</param>
    /// <param name="amount">支付的总金额，(货币单位为分)如：1300为13元</param>
    /// <param name="currency">人民币CNY，港元 HKD，台币：TWD，美元USD；欧元EUR；英镑 GBP，日元 JPY</param>
    /// <param name="payType">Alipay, Wxpay</param>
    public void OnPay(string orderId, int amount, string currency, string payType)
    {
        Debug.Log(">>>>>>TakkingDataSDK OnPay");
        this.sdkObj.Call("OnPay", orderId, amount, currency, payType); 
    }
}
