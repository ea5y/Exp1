using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

public class HttpUtil : Singleton<HttpUtil>
{

    [SerializeField]
    private float timeout = 30.0f;

    /// <summary>
    /// 最大リトライ数
    /// </summary>
    [SerializeField]
    private int maxRetry = 3;

    /// <summary>
    /// リトライ時のウェイト
    /// </summary>
    private readonly float retryWait = 3.0f;

    /// <summary>
    /// キャンセルした
    /// </summary>
    private bool isCancel = false;

    /// <summary>
    /// 指定URLにPostする
    /// もちろんのごとくコルーチンで。
    /// </summary>
    /// <param name="url"></param>
    /// <param name="postData"></param>
    /// <param name="callback">結果取得</param>
    /// <returns></returns>
    public IEnumerator HttpPost(string url, WWWForm postData, Action<bool, string> callback, Action retryCallback)
    {
        string retText = "";
        int retry = 0;

        while (retry < maxRetry)
        {
            using (WWW www = new WWW(url, postData))
            {
                string err = null;

                yield return StartCoroutine(downloadCheckTimeOut(www, timeout, (x => err = x)));

                if (isCancel)
                {
                    break;
                }

                // Errorがなければセットして終了
                if (String.IsNullOrEmpty(www.error) && String.IsNullOrEmpty(err))
                {
                    retText = www.text;
                    break;
                }
                else
                {

                    if (++retry >= maxRetry)
                    {
//                        DebugLog.Log("Download Failed " + www.error);
                        retText = "";
                        break;
                    }

                    retryCallback();

                    // リトライ
                    yield return new WaitForSeconds(retryWait);
                }
            }
        }

        callback(!String.IsNullOrEmpty(retText), retText);
    }

    /// <summary>
    /// Https the post data string.
    /// </summary>
    /// <returns>The post.</returns>
    /// <param name="url">URL.</param>
    /// <param name="postData">Post data.</param>
    /// <param name="callback">Callback.</param>
    /// <param name="retryCallback">Retry callback.</param>
    public IEnumerator HttpPost(string url, string data, Action<bool, string> callback, Action retryCallback)
    {
        string retText = "";
        int retry = 0;
        byte[] postData = UTF8Encoding.UTF8.GetBytes(data);

        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

        HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
        req.Method = "POST";
        req.ContentLength = postData.Length;
        req.Timeout = 30000;
        req.ReadWriteTimeout = 30000;
        try {
            using (Stream reqStream = req.GetRequestStream()) {
                reqStream.Write(postData, 0, postData.Length);
            }
        } catch (Exception e) {
            // Exception happened in post
            Debug.LogError(e.Message);
            callback(false, string.Empty);
            yield break;
        }
        using (WebResponse wr = req.GetResponse()) {
            yield return null;
            Stream responseStream = wr.GetResponseStream();
            StreamReader sr = new StreamReader(responseStream);
            retText = sr.ReadToEnd();
        }

        callback(!String.IsNullOrEmpty(retText), retText);
    }

    public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;
        // If there are errors in the certificate chain, look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                    }
                }
            }
        }
        return isOk;
    }

    static public IEnumerator StaticHttpPost(string url, WWWForm postData, Action<bool, string> callback, Action retryCallback)
    {
        string retText = "";
        int retry = 0;

        int maxRetry = 3;
        float retryWait = 5.0f;

        while (retry < maxRetry)
        {
            using (WWW www = new WWW(url, postData))
            {
                string err = null;

                while (!www.isDone)
                {
                    yield return 0;
                }

                // Errorがなければセットして終了
                if (String.IsNullOrEmpty(www.error) && String.IsNullOrEmpty(err))
                {
                    retText = www.text;
                    break;
                }
                else
                {

                    if (++retry >= maxRetry)
                    {
//                        DebugLog.Log("Download Failed " + www.error);
                        retText = "";
                        break;
                    }

                    retryCallback();

                    // リトライ
                    yield return new WaitForSeconds(retryWait);
                }
            }
        }

        callback(!String.IsNullOrEmpty(retText), retText);
    }

    /// <summary>
    /// タイムアウトありのチェック
    /// </summary>
    /// <param name="www"></param>
    /// <param name="timeout"></param>
    /// <param name="timeoutCallback"></param>
    /// <returns></returns>
    private IEnumerator downloadCheckTimeOut(WWW www, float timeout, Action<string> timeoutCallback)
    {
        float requestTime = Time.realtimeSinceStartup;
        float lastProgress = www.progress;

        // 終了していなければチェック
        while (!www.isDone)
        {
            if (isCancel)
            {
                www.Dispose();
                break;
            }

            if (lastProgress < www.progress)
            {
                // 進捗が進んでいる
                lastProgress = www.progress;
                requestTime = Time.realtimeSinceStartup;
            }
            else
            {
                // 進んでいないのでTimeoutチェック
                if (Time.realtimeSinceStartup - requestTime > timeout)
                {
                    // Timeoutなので終了
//                    DebugLog.Log("Download Timeout");
                    timeoutCallback("Timeout " + www.url);
                    break;
                }
            }

            yield return null;
        }

    }


}
