using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Asobimo.WebAPI;


public class UnityXYManager : Singleton<UnityXYManager>
{
    private string mAsobimoId = null;
    private string mToken = null;
    private string mUid = null;

    private string KEYUserId = "userid";
    private string KEYToken = "token";
    private string KEYError = "errcode";
    private string KEYData = "data";
    private string KEYPhone = "mobilephone";
    private string KEYUserPWD = "userpwd";
    private string KEYDevId = "deviceid";
    private string KEYVerifyCode = "verifycode";

    #region ConnectServer

    #region Register
    /// <summary>
    /// Registers the account.
    /// </summary>
    public void RegisterAccount(long phone, string pwd, string verifycode)
    {
        string url = "register";
        Dictionary<string, object> form = new Dictionary<string, object>();
        form.Add(KEYPhone, phone);
        form.Add(KEYUserPWD, pwd);
        form.Add(KEYDevId, "123456");
        form.Add(KEYVerifyCode, verifycode);
        StartCoroutine(CoroutinePost(url, form, OnRegisterResponse));
    }
    /// <summary>
    /// Raises the register response event.
    /// </summary>
    /// <param name="result">If set to <c>true</c> result.</param>
    /// <param name="data">Data.</param>
    void OnRegisterResponse(bool result, string data)
    {
        if (result && data != null)
        {
            var json = LitJson.JsonMapper.ToObject<Dictionary<string, object>>(data);
            if (json != null)
            {
                object error = null;
                if (json.TryGetValue(KEYError, out error))
                {
                    DealErrorMsg(error);
                    //PlayerPrefs.SetString("Name", "");
                    //PlayerPrefs.SetString("Pass", "");
                    LoginFrame.Instance.AuthState.Finished = false;
                    LoginFrame.Instance.AuthState.Success = false;
                }
                else
                {

                    object aesData = null;
                    if (json.TryGetValue(KEYData, out aesData))
                    {
                        string decrptData = AESHelper.Decrypt(aesData.ToString());
                        Dictionary<string, object> userData = LitJson.JsonMapper.ToObject<Dictionary<string, object>>(decrptData);
                        if (userData != null && userData.ContainsKey(KEYUserId))
                        {
                            mToken = userData[KEYToken].ToString();
                            StartCoroutine(CoroutinePost_GetToken(mToken, OnResponseGetToken));
                        }
                    }
                }
            }
        }
        else
        {
            GUITipMessage.Instance.Show("Error");
            //PlayerPrefs.SetString("Name", "");
            //PlayerPrefs.SetString("Pass", "");
            LoginFrame.Instance.AuthState.Finished = false;
            LoginFrame.Instance.AuthState.Success = false;
        }
    }
#endregion

#region VerifyCode

    /// <summary>
    /// Sends the verify code.
    /// </summary>
    private float lastverfytime = 0;
    public void SendVerifyCode(long phone)
    {
        if (Time.time < lastverfytime)
        {
            int left = (int)(-Time.time + lastverfytime);
            GUITipMessage.Instance.Show(left + " 秒之后再次获取！！！");
            return;
        }
        string url = "sendVerifyCode";
        Dictionary<string, object> form = new Dictionary<string, object>();
        form.Add(KEYPhone, phone);
        StartCoroutine(CoroutinePost(url, form, OnVerifyCodeResponse));
        lastverfytime = Time.time + 60;
    }
    /// <summary>
    /// Raises the verify code response event.
    /// </summary>
    /// <param name="result">If set to <c>true</c> result.</param>
    /// <param name="data">Data.</param>
    void OnVerifyCodeResponse(bool result, string data)
    {
        if (result && data != null)
        {
            var json = LitJson.JsonMapper.ToObject<Dictionary<string, object>>(data);
            if (json != null)
            {
                object error = null;
                if (json.TryGetValue("errcode", out error))
                {
                    DealErrorMsg(error);
                    //PlayerPrefs.SetString("Name", "");
                    //PlayerPrefs.SetString("Pass", "");
                    LoginFrame.Instance.AuthState.Finished = false;
                    LoginFrame.Instance.AuthState.Success = false;
                }
                else
                {
                    DealErrorMsg("获取成功");
                }
            }
        }
        else
        {
            GUITipMessage.Instance.Show("Error OnVerifyCodeResponse");
            //PlayerPrefs.SetString("Name", "");
            //PlayerPrefs.SetString("Pass", "");
            LoginFrame.Instance.AuthState.Finished = false;
            LoginFrame.Instance.AuthState.Success = false;
        }
    }
#endregion

#region Login
    /// <summary>
    /// Gets the token.
    /// </summary>
    public void LoginToken(long phone, string pwd)
    {
        string url = "login";
        Dictionary<string, object> form = new Dictionary<string, object>();
        form.Add(KEYPhone, phone);
        form.Add(KEYUserPWD, pwd);
        form.Add(KEYDevId, "12356");
        StartCoroutine(CoroutinePost(url, form, OnLoginTokenResponse));
    }
    /// <summary>
    /// Raises the token response event.
    /// </summary>
    /// <param name="result">If set to <c>true</c> result.</param>
    /// <param name="data">Data.</param>
    void OnLoginTokenResponse(bool result, string data)
    {
        Debug.LogError("===> OnLoginTokenResponse " + result + "  " + data);
        if (result && data != null)
        {
            var json = LitJson.JsonMapper.ToObject<Dictionary<string, object>>(data);
            if (json != null)
            {
                object error = null;
                if (json.TryGetValue(KEYError, out error))
                {
                    DealErrorMsg(error);
                    PlayerPrefs.SetString("Name", "");
                    PlayerPrefs.SetString("Pass", "");
                    LoginFrame.Instance.SetDetail(LoginFrame.Instance.AuthState);
                }
                else
                {
                    object aesData = null;
                    if (json.TryGetValue(KEYData, out aesData))
                    {
                        string decrptData = AESHelper.Decrypt(aesData.ToString());
                        Dictionary<string, object> userData = LitJson.JsonMapper.ToObject<Dictionary<string, object>>(decrptData);
                        if (userData != null && userData.ContainsKey(KEYToken))
                        {
                            mToken = userData[KEYToken].ToString();
                        }

                        if (userData != null && userData.ContainsKey(KEYUserId))
                        {
                            mUid = userData[KEYUserId].ToString();
                            StartCoroutine(CoroutinePost_GetToken(mUid, OnResponseGetToken));
                        }
                    }
                }
            }
        }
        else
        {
            GUITipMessage.Instance.Show("登录失败");
            PlayerPrefs.SetString("Name", "");
            PlayerPrefs.SetString("Pass", "");
            LoginFrame.Instance.SetDetail(LoginFrame.Instance.AuthState);
        }
    }
#endregion

#region ValidataToken
    /// <summary>
    /// Validates the token.
    /// </summary>

    public void ValidateToken(string token)
    {
        string url = "validate";
        Dictionary<string, object> form = new Dictionary<string, object>();
        form.Add(KEYToken, token);
        form.Add(KEYDevId, "1befa08963d5e36b");
        StartCoroutine(CoroutinePost(url, form, OnValidateTokenResponse));
    }
    /// <summary>
    /// Raises the validate token response event.
    /// </summary>
    /// <param name="result">If set to <c>true</c> result.</param>
    /// <param name="data">Data.</param>
    void OnValidateTokenResponse(bool result, string data)
    {
        if (result && data != null)
        {
            var json = LitJson.JsonMapper.ToObject<Dictionary<string, object>>(data);
            if (json != null)
            {
                object error = null;
                if (json.TryGetValue(KEYError, out error))
                {
                    DealErrorMsg(error);
                    LoginFrame.Instance.AuthState.Finished = false;
                    LoginFrame.Instance.AuthState.Success = false;
                }
                else
                {
                    object aesData = null;
                    if (json.TryGetValue(KEYData, out aesData))
                    {
                        string decrptData = AESHelper.Decrypt(aesData.ToString());
                        Dictionary<string, object> userData =
                            LitJson.JsonMapper.ToObject<Dictionary<string, object>>(decrptData);

                        if (userData != null && userData.ContainsKey(KEYToken))
                        {
                            mToken = userData[KEYToken].ToString();
                        }
                    }
                }
            }
        }
        else
        {
            GUITipMessage.Instance.Show("Error OnValidateTokenResponse");
        }
    }
#endregion

#region ModifyPWD
    /// <summary>
    /// Modifies the PW.
    /// </summary>
    public void ModifyPWD(long number, string pwd, string verifycode)
    {
        string url = "modifypwd";
        Dictionary<string, object> form = new Dictionary<string, object>();
        form.Add(KEYPhone, number);
        form.Add(KEYUserPWD, pwd);
        form.Add(KEYVerifyCode, verifycode);
        StartCoroutine(CoroutinePost(url, form, OnModifyPWDResponse));
    }
    /// <summary>
    /// Raises the modify PWD response event.
    /// </summary>
    /// <param name="result">If set to <c>true</c> result.</param>
    /// <param name="data">Data.</param>
    void OnModifyPWDResponse(bool result, string data)
    {
        if (result && data != null)
        {
            var json = LitJson.JsonMapper.ToObject<Dictionary<string, object>>(data);
            if (json != null)
            {
                object error = null;
                if (json.TryGetValue("errcode", out error))
                {
                    DealErrorMsg(error);
                    //PlayerPrefs.SetString("Name", "");
                    //PlayerPrefs.SetString("Pass", "");
                    LoginFrame.Instance.AuthState.Finished = false;
                    LoginFrame.Instance.AuthState.Success = false;
                }
                else
                {
                    GUITipMessage.Instance.Show("密码修改成功，请登录！！！");
                    LoginFrame.Instance.mLogin.SetActive(true);
                    LoginFrame.Instance.mPasChange.SetActive(false);
                }
            }
        }
        else
        {
            GUITipMessage.Instance.Show("Error OnModifyPWDResponse");
            //PlayerPrefs.SetString("Name", "");
            //PlayerPrefs.SetString("Pass", "");
            LoginFrame.Instance.AuthState.Finished = false;
            LoginFrame.Instance.AuthState.Success = false;
        }
    }
#endregion

    /// <summary>
    /// Coroutines the post.
    /// </summary>
    /// <returns>The post.</returns>
    /// <param name="url">URL.</param>
    /// <param name="form">Form.</param>
    /// <param name="func">Func.</param>
    IEnumerator CoroutinePost(string url, Dictionary<string, object> data, System.Action<bool, string> func)
    {
        string strData = LitJson.JsonMapper.ToJson(data);
        string encryptData = AESHelper.Encrypt(strData);
        Dictionary<string, object> postDic = new Dictionary<string, object>();
        postDic.Add("gameid", 174);
        postDic.Add("data", encryptData);
        Debug.Log("encryptData:" + encryptData);
        string postData = LitJson.JsonMapper.ToJson(postDic);
        yield return StartCoroutine(
        HttpUtil.Instance.HttpPost("https://account.xiaoyougame.com:4000/game/" + url, postData,
                                   (result, response) => func(result, response), () => { /*retry callback*/ }));

        //        yield return StartCoroutine(TestPost(encryptData));
    }

    public IEnumerator CoroutinePost_GetToken(string phonetoken, Action<bool, string> func, GUIAuth.AuthState pAuthState = null)
    {
        //易接平台突然加入，暂时添加一个pAuthState作为信号槽，丑陋之处勿喷
        if (null != pAuthState)
        {
            LoginFrame.Instance.AuthState = pAuthState;
        }
#if !EJPL
        phonetoken = "xiaoyoupf_" + phonetoken;
#endif
        Debug.Log("===> 换取asobimo之前的id: " + phonetoken);
        WWWForm data = new WWWForm();
        string addhash = "^^6FTeVXPQgJMmS7fX9klhSXtZQzKUVXVPofdgKTWn";
        string hash = EncryptToSHA1(phonetoken + addhash);
        data.AddField("gid", phonetoken);
        data.AddField("hash", hash);
        Debug.Log(hash);
        yield return StartCoroutine(
                HttpUtil.Instance.HttpPost("http://auth.xiaoyougame.com/getAuth", data, (result, res) =>
                {
                    func(result, res);
                }, () => { }));

    }

    public void OnResponseGetToken(bool result, string response)
    {
        if (result)
        {
#if EJPL
#if UNITY_EDITOR
            APaymentHelperDemo.TokenCache = new SFJSONObject("\"{\"sdk\":\"\",\"app\":\"\",\"uin\":\"\",\"sess\":\"\",\"token\":\"\"}\"");
            APaymentHelperDemo.TokenCache.put("sdk", "E7FDED8015C8FD56");
            APaymentHelperDemo.TokenCache.put("app", "A3BA28AEE6FCC882");
            APaymentHelperDemo.TokenCache.put("uin", "1335071295");
            APaymentHelperDemo.TokenCache.put("sess", "1335071295658be241649540cd15b30287ed352928eda79789");
#endif
            APaymentHelperDemo.TokenCache.put("token", response);
            
            PluginController.AuthInfo.token = response;
            PluginController.AuthInfo.EJInfo = APaymentHelperDemo.TokenCache.toString();
#else
            PluginController.AuthInfo.token = response;
#endif
            StartCoroutine(CoroutinePost_GetAsobimoId(response, OnResponseGetAsobimoId));
        }
        else
        {
            GUITipMessage.Instance.Show("Error OnResponseGetToken");
            LoginFrame.Instance.AuthState.Finished = false;
            LoginFrame.Instance.AuthState.Success = false;
        }
    }

    IEnumerator CoroutinePost_GetAsobimoId(string token, Action<bool, string> func)
    {
        WWWForm data = new WWWForm();
        data.AddField("at", token);
        yield return StartCoroutine(
                HttpUtil.Instance.HttpPost("http://auth.xiaoyougame.com/getAsoid", data, (result, res) =>
                {
                    func(result, res);
                }, () => { }));

    }

    public void OnResponseGetAsobimoId(bool result, string response)
    {
        if (result)
        {
            Debug.LogError("===> " + response);
            PluginController.AuthInfo.authID = response;

            AsobimoWebAPI.Instance.CheckAuthLogin<Asobimo.WebAPI.AsobimoWebAPI.CheckLogin>((v) =>
            {
                Debug.Log(v);
                LoginFrame.Instance.AuthState.Success = true;
                LoginFrame.Instance.AuthState.Finished = true;
            });
        }
        else
        {
            GUITipMessage.Instance.Show("Error OnResponseGetAsobimoId");
            LoginFrame.Instance.AuthState.Finished = false;
            LoginFrame.Instance.AuthState.Success = false;
        }
    }

    /// <summary>
    /// Deals the error message.
    /// </summary>
    /// <param name="codeData">Code data.</param>
    void DealErrorMsg(object codeData)
    {
        Debug.LogError("===> ErrorCode " + codeData);
        if ("0" == codeData.ToString())
        {
            GUITipMessage.Instance.Show("登录失败");
        }
        else if ("101" == codeData.ToString())
        {
            GUITipMessage.Instance.Show("账户已存在，无法重复注册！！！");
        }
        else if ("112" == codeData.ToString())
        {
            GUITipMessage.Instance.Show("验证码输入错误！！！");
        }
        else if ("105" == codeData.ToString())
        {
            GUITipMessage.Instance.Show("密码输入错误！！！");
        }
        else
        {
            GUITipMessage.Instance.Show(codeData.ToString());
        }
    }

    public string EncryptToSHA1(string str)
    {
        System.Security.Cryptography.SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
        byte[] str1 = Encoding.UTF8.GetBytes(str);
        byte[] str2 = sha1.ComputeHash(str1);
        sha1.Clear();
        (sha1 as IDisposable).Dispose();
        return Encode(str2);
    }

    public static string Encode(byte[] data)
    {
        var buffer = new StringBuilder(data.Length * 2);

        for (var i = 0; i < data.Length; i++)
        {
            buffer.Append((data[i] & 0xff).ToString("x").PadLeft(2, '0'));
        }

        return buffer.ToString();
    }
#endregion
}
