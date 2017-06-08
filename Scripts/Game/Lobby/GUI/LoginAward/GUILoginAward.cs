using UnityEngine;
using System.Collections;
using Scm.Common.Master;
using Scm.Common.Packet;
using Scm.Common.GameParameter;

namespace XUI
{
    public class GUILoginAward : Singleton<GUILoginAward>
    {
        #region View
        public LoginAwarView View;
        [System.Serializable]
        public class LoginAwarView
        {
            public UIAtlas LobbyI;
            public UIAtlas LobbyII;
            public UIAtlas Common;
            public GameObject root;
            public UIButton btnClose;

            public ItemLoginAward[] days;
        }
        #endregion

        private int DAYS_NUM = 7;
        private ItemLoginAward currentDay;

        private int currentRewardCount;
        void Awake()
        {
            base.Awake();
            this.gameObject.SetActive(false);
        }

        public void Open()
        {
            Net.Network.Instance.StartCoroutine(this._Open());
        }

        private IEnumerator _Open()
        {
            yield return Net.Network.GetLoginAwardInfo(
                (res) =>
                {
                    var info = res.GetLoginBonusParams();

                    this.SetDays(info);
                });

            PanelManager.Instance.Open(this.View.root, true);
        }

        private void SetDays(LoginBonusParameter[] info)
        {
            for (int i = 0; i < DAYS_NUM; i++)
            {
                var itemInfo = info[i];
                LoginBonusMasterData data;
                MasterData.TryGetLoginBonusMasterData(itemInfo.LoginBonusMasterId, out data);

                this.SetDay(itemInfo, data, this.View.days[i]);

            }
        }

        private void SetDay(LoginBonusParameter itemInfo, LoginBonusMasterData data, ItemLoginAward day)
        {
            this.SetIcon(data, day);
            this.SetDesc(data, day);

            this.SaveCurrentDay(data, itemInfo, day);

            //Status
            if (itemInfo.Progress == 1)
                this.SetStatus(StatusType.Completed, day);
            if (itemInfo.Progress == 0)
                this.SetStatus(StatusType.Inactive, day);
            if (itemInfo.Progress == -1)
                this.SetStatus(StatusType.Active, day);

            this.SetEvent(data, day);
        }

        private void SetIcon(LoginBonusMasterData data, ItemLoginAward day)
        {
            string iconName = "";
            switch (data.RewardType)
            {
                case Scm.Common.GameParameter.RewardType.Gold:
                    day.icon.atlas = this.View.LobbyII;
                    iconName = "gold1";
                    day.icon.spriteName = iconName;
                    day.icon.MakePixelPerfect();
                    break;
                case Scm.Common.GameParameter.RewardType.Coin:
                    day.icon.atlas = this.View.LobbyII;
                    iconName = "coin1";
                    day.icon.spriteName = iconName;
                    day.icon.MakePixelPerfect();
                    day.icon.width = 96;
                    day.icon.height = 80;
                    break;
                case Scm.Common.GameParameter.RewardType.Character:
                    day.icon.atlas = this.View.LobbyI;
                    iconName = "wenhao";
                    day.icon.spriteName = iconName;
                    day.icon.MakePixelPerfect();
                    break;
                case RewardType.ExpCharacter:
                    day.icon.atlas = this.View.Common;
                    iconName = "wenhao";
                    day.icon.spriteName = iconName;
                    day.icon.width = day.icon.height = 60;
                    break;
            }
        }

        private void SetDesc(LoginBonusMasterData data, ItemLoginAward day)
        {
            day.num.text = data.RewardCount + "";
        }

        private void SaveCurrentDay(LoginBonusMasterData data, LoginBonusParameter itemInfo, ItemLoginAward day)
        {
            if (itemInfo.Progress == -1)
            {
                if (this.currentDay == null)
                    this.currentDay = day;
                this.currentRewardCount = data.RewardCount;
            }
        }

        private void SetEvent(LoginBonusMasterData data, ItemLoginAward day)
        {
            var type = data.RewardType;
            var effect = day.effectRecieve;
            day.btnRecieve.onClick.Clear();
            EventDelegate.Add(day.btnRecieve.onClick, () => { this.OnBtnRecieveClick(type, effect); SoundController.PlaySe(SoundController.SeID.Button); });
        }

        private enum StatusType
        {
            Active,
            Inactive,
            Completed
        }

        private void SetStatus(StatusType type, ItemLoginAward item)
        {
            switch (type)
            {
                case StatusType.Active:
                    //hook
                    item.hook.gameObject.SetActive(false);
                    //effectActive
                    item.effectActive.gameObject.SetActive(true);
                    //effectRecieve
                    item.effectRecieve.gameObject.SetActive(false);
                    //status
                    item.status.gameObject.SetActive(false);
                    //btn
                    item.btnRecieve.gameObject.SetActive(true);
                    break;
                case StatusType.Inactive:
                    //hook
                    item.hook.gameObject.SetActive(false);
                    //effectActive
                    item.effectActive.gameObject.SetActive(false);
                    //effectRecieve
                    item.effectRecieve.gameObject.SetActive(false);
                    //status
                    item.status.gameObject.SetActive(true);
                    item.status.text = "未领取";
                    //btn
                    item.btnRecieve.gameObject.SetActive(false);
                    break;
                case StatusType.Completed:
                    //hook
                    item.hook.gameObject.SetActive(true);
                    //effectActive
                    item.effectActive.gameObject.SetActive(false);
                    //effectRecieve
                    item.effectRecieve.gameObject.SetActive(false);
                    //status
                    item.status.gameObject.SetActive(true);
                    item.status.text = "已领取";
                    //btn
                    item.btnRecieve.gameObject.SetActive(false);
                    break;
            }
        }

        public void Close()
        {
            SoundController.PlaySe(SoundController.SeID.WindowClose);
            PanelManager.Instance.Close(this.View.root);
        }

        private void OnBtnRecieveClick(Scm.Common.GameParameter.RewardType type, ParticleSystem effectRecieve)
        {
            var typeLocal = type;
            var effectLocal = effectRecieve;
            StartCoroutine(
            Net.Network.GetLoginAward((res) =>
            {
                if(res.Result == true)
                {
                    StartCoroutine(this.ReciveMotion(typeLocal, res.ItemId, res.RewardCount, effectLocal));
                }
            }));
        }

        private IEnumerator ReciveMotion(RewardType type, int itemId, int count, ParticleSystem effectRecieve)
        {
            this.SetStatus(StatusType.Completed, this.currentDay);
            effectRecieve.gameObject.SetActive(true);
            yield return new WaitForSeconds(effectRecieve.duration);
            
            this.ShowProduct(type, itemId, count);
        }

        private void ShowProduct(RewardType type, int itemId, int count)
        {
            var data = this.PackData(type, itemId, count);
            GUIProductsWindow.Instance.OpenOK(data, null);
        }

        private ProductShowData PackData(RewardType type,int itemId, int count)
        {
            ProductShowData data = new ProductShowData();
            switch(type)
            {
                case RewardType.Character:
                    data.type = ShopItemType.Character;
                    data.characterId = itemId;
                    data.desc = "角色x1";
                    break;
                case RewardType.Coin:
                    data.type = ShopItemType.Ticket;
                    data.desc = "点券x" + this.currentRewardCount;
                    break;
                case RewardType.Gold:
                    data.type = ShopItemType.Gold;
                    data.desc = "金币x" + this.currentRewardCount;
                    break;
            }

            return data;
        }
    }
}
