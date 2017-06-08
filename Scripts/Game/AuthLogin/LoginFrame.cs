using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LoginFrame : MonoBehaviour
{
    public static LoginFrame Instance;
    public GameObject mLoading;
    public GameObject mLoadingMask;
    public GameObject mLogin;
    public GameObject mChange;
    public GameObject mRegister;
    public GameObject mPasChange;
    public UIInput mLoginName;
    public UIInput mLoginPas;
    public UIInput mRegisterName;
    public UIInput mRegisterPas;
    public UIInput mRegisterPhone;
    public UIInput mPaschangeName;
    public UIInput mPaschangePas;
    public UIInput mPaschangePhone;
    public UILabel mChangeName;
    public UISprite mAuto;

    public bool AutoLogin
    {
        get { return mAuto.spriteName == "gou"; }
        set
        {
            if (value)
            {
                mAuto.spriteName = "gou";
            }
            else
            {
                mAuto.spriteName = "nogou";
            }
        }
    }
    void Awake()
    {
        gameObject.SetActive(false);
        Instance = this;
    }

    private Coroutine WaitCoroutine;
    public GUIAuth.AuthState AuthState;
    public void SetDetail(GUIAuth.AuthState pAuthState)
    {
        ShowLoading(false);
        AuthState = pAuthState;
        gameObject.SetActive(true);
        mLogin.SetActive(false);
        mChange.SetActive(false);
        mRegister.SetActive(false);
        mPasChange.SetActive(false);
        string name = PlayerPrefs.GetString("Name", "");
        string pass = PlayerPrefs.GetString("Pass", "");
        AutoLogin = PlayerPrefs.GetInt("Auto", 1) == 1 ? true : false;
        if (AutoLogin)
        {
            if ("" == name)
            {
                mLogin.SetActive(true);
                mLoginName.text = "";
                mLoginPas.text = "";
            }
            else
            {
                mChange.SetActive(true);
                mChangeName.text = "账号：" + name;
                WaitCoroutine = StartCoroutine(WaitLogin());
            }
        }
        else
        {
            mLogin.SetActive(true);
            mLoginName.text = name;
            mLoginPas.text = pass;
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void ShowLoading(bool pShow, bool pWithMask = true)
    {
        mLoadingMask.SetActive(pWithMask);
        mLoading.SetActive(pShow);
    }

    private IEnumerator WaitLogin()
    {
        ShowLoading(true, false);
        yield return new WaitForSeconds(2f);
        string name = PlayerPrefs.GetString("Name", "");
        string pass = PlayerPrefs.GetString("Pass", "");
        UnityXYManager.Instance.LoginToken(long.Parse(name), pass);
    }

    public void OnClickAuto()
    {
        AutoLogin = !AutoLogin;
        PlayerPrefs.SetInt("Auto", AutoLogin ? 1 : 0);
    }

    public void OnClickLogin()
    {
        mLoginName.text = mLoginName.text.Trim();
        if (!CheckPhone(mLoginName.text))
        {
            GUITipMessage.Instance.Show("手机号输入格式错误");
            mLoginName.text = "";
            return;
        }
        PlayerPrefs.SetString("Name", mLoginName.text);
        PlayerPrefs.SetString("Pass", mLoginPas.text);
        PlayerPrefs.Save();
        ShowLoading(true);
        UnityXYManager.Instance.LoginToken(long.Parse(mLoginName.text), mLoginPas.text);
    }

    public void OnClickNewUser()
    {
        mLogin.SetActive(false);
        mRegister.SetActive(true);
    }

    public void OnClickForget()
    {
        mLogin.SetActive(false);
        mPasChange.SetActive(true);
    }

    public void OnClickChangeUser()
    {
        ShowLoading(false);
        StopCoroutine(WaitCoroutine);
        mLogin.SetActive(true);
        mChange.SetActive(false);
        string name = PlayerPrefs.GetString("Name", "");
        string pass = PlayerPrefs.GetString("Pass", "");
        mLoginName.text = name;
        mLoginPas.text = pass;
    }

    public void OnClickBack()
    {
        mLogin.SetActive(true);
        mRegister.SetActive(false);
        mPasChange.SetActive(false);
    }

    public void OnClickRegister()
    {
        mRegisterName.text = mRegisterName.text.Trim();
        if (!CheckPhone(mRegisterName.text))
        {
            GUITipMessage.Instance.Show("手机号输入格式错误");
            mRegisterName.text = "";
            return;
        }
        ShowLoading(true);
        UnityXYManager.Instance.RegisterAccount(long.Parse(mRegisterName.text), mRegisterPas.text, mRegisterPhone.text);
        PlayerPrefs.SetString("Name", mRegisterName.text);
        PlayerPrefs.SetString("Pass", mRegisterPas.text);
        PlayerPrefs.Save();
    }

    public void OnClickPasChange()
    {
        mPaschangeName.text = mPaschangeName.text.Trim();
        if (!CheckPhone(mPaschangeName.text))
        {
            GUITipMessage.Instance.Show("手机号输入格式错误");
            mPaschangeName.text = "";
            return;
        }
        ShowLoading(true);
        UnityXYManager.Instance.ModifyPWD(long.Parse(mPaschangeName.text), mPaschangePas.text, mPaschangePhone.text);
        PlayerPrefs.SetString("Name", mPaschangeName.text);
        PlayerPrefs.SetString("Pass", mPaschangePas.text);
        PlayerPrefs.Save();
        mLoginName.text = "";
        mLoginPas.text = "";
    }

    public void OnClickGetVerify_Register()
    {
        try
        {
            UnityXYManager.Instance.SendVerifyCode(long.Parse(mRegisterName.text));
        }
        catch (Exception)
        {
            GUITipMessage.Instance.Show("手机号输入错误！！！");
            //            throw;
        }
    }

    public void OnClickGetVerify_PasChange()
    {
        try
        {
            UnityXYManager.Instance.SendVerifyCode(long.Parse(mPaschangeName.text));
        }
        catch (Exception)
        {
            GUITipMessage.Instance.Show("手机号输入错误！！！");
            //            throw;
        }
    }

    public bool CheckEmail(string email)
    {
        email = email.Trim();
        if (email.Contains("@"))
        {
            return true;
        }
        return false;
    }

    public bool CheckPhone(string phone)
    {
        phone = phone.Trim();
        if (phone.Length != 11)
        {
            return false;
        }
        try
        {
           long.Parse(phone);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
        return true;
    }
}
