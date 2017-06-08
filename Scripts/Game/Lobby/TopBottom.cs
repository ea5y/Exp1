using System;
using UnityEngine;
using System.Collections;

public class TopBottom : MonoBehaviour
{
    public static TopBottom Instance;
    public UIPlayTween mPlayTween;
    public UILabel mUILabel;

    public UIButton btnBack;
    public UIButton btnGoldAdd;
    public UIButton btnCoinAdd;
    public UILabel lblGold;
    public UILabel lblCoin;

    public Action OnIn;
    public Action<Action> OnBack;

    private void OnActive()
    {
        if(this.gameObject.activeSelf == false)
            mPlayTween.Play(true);
    }

    private void OnDeactive()
    {
        mPlayTween.Play(false);
        OnIn = null;
        OnBack = null;
        //暂时认为Back最终都是退回到主界面
        GUILobbyResident.Instance.OnIn();
    }

    private void Awake()
    {
        Instance = this;
        this.RegisterEventOnce();
        this.SetGold(this, EventArgs.Empty);
        this.SetCoin(this, EventArgs.Empty);
        gameObject.SetActive(false);
    }

    public void In(string pTitle = "")
    {
        mUILabel.text = pTitle;
        if (null != OnIn)
        {
            OnIn();
        }
        OnActive();
    }

    public void Back()
    {
        SoundController.PlaySe(SoundController.SeID.WindowClose);
        if (null != OnBack)
        {
            OnBack(OnDeactive);
        }
    }

    private void RegisterEventOnce()
    {
        EventDelegate.Add(this.btnBack.onClick, ()=>{
                this.Back();
                });
        EventDelegate.Add(this.btnGoldAdd.onClick, ()=>{
                SoundController.PlaySe(SoundController.SeID.Select);
                XUI.GUIShop.Instance.OpenToDQ();
                });
        EventDelegate.Add(this.btnCoinAdd.onClick, ()=>{
                SoundController.PlaySe(SoundController.SeID.Select);
                XUI.GUIShop.Instance.OpenToCZ();
                });

        XDATA.PlayerData.Instance.OnGoldChange += this.SetGold;
        XDATA.PlayerData.Instance.OnCoinChange += this.SetCoin;
    }

    private void SetGold(object sender, EventArgs args)
    {
        this.lblGold.text = XDATA.PlayerData.Instance.Gold + "";
    }

    private void SetCoin(object sender, EventArgs args)
    {
        this.lblCoin.text = XDATA.PlayerData.Instance.Coin + "";
    }
}
