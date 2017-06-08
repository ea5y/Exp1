using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

public class OUIItemResidentProgress : OUIItemBase
{
    #region フィールド＆プロパティ
    /// <summary>
    /// アタッチオブジェクト
    /// </summary>
    [SerializeField]
    AttachObject _attach;
    public AttachObject Attach { get { return _attach; } }
    [System.Serializable]
    public class AttachObject
    {
        public UISlider slider;
        public UISprite sprite;
        public UILabel blue;
        public UILabel red;
    }

    // シリアライズされていないメンバーの初期化
    void MemberInit()
    {
        //        this.ShakeCount = 0f;
        //        this.ShakeFiber = null;
    }
    #endregion

    #region 初期化
    void Awake()
    {
        this.MemberInit();
    }
    #endregion

    #region 作成
    /// <summary>
    /// プレハブ取得
    /// </summary>
    public static OUIItemResidentProgress GetPrefab(GUIObjectUI.AttachObject.Prefab prefab, ObjectBase o)
    {

        return prefab.resident.resident;
    }
    #endregion

    #region 更新
    public void UpdateUI(Scm.Common.Packet.ResidentAreaSideGaugeEvent pack, bool pBlue)
    {
        this.Attach.red.text = "";
        this.Attach.blue.text = "";
        this.Attach.slider.value = 0;
        if (null == pack)
        {
            return;
        }
        if (GUITacticalGauge.RoundIndex != 3)
        {
            float t = 0f;
            float Max = pack.RedTotal;
            float Cur = pack.RedRemain;

            this.Attach.sprite.spriteName = "ui_Btl_TeamTE";
            if (pBlue)
            {
                Max = pack.BlueTotal;
                Cur = pack.BlueRemain;
                this.Attach.sprite.spriteName = "ui_Btl_TeamTP";
            }

            if (0 < Max)
                t = Cur / Max;

            this.Attach.slider.value = t;
        }
        else
        {
            float t = 0f;
            float Max = pack.RedTotal;
            float Cur = pack.RedStandBy;
            this.Attach.sprite.spriteName = "ui_Btl_TeamTE";
            if (pBlue)
            {
                Max = pack.BlueTotal;
                Cur = pack.BlueStandBy;
                this.Attach.sprite.spriteName = "ui_Btl_TeamTP";
            }
            this.Attach.red.text = pack.RedRemain + "%";
            this.Attach.blue.text = pack.BlueRemain + "%";
            if (0 < Max)
                t = Cur / Max;

            this.Attach.slider.value = t;
        }
    }
    #endregion

    #region
    void Update()
    {

    }
    #endregion

}
