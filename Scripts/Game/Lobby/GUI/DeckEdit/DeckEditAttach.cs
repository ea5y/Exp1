using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeckEditAttach : GridItemBase
{
    public AttachType Type;
    public Root mRoot;
    public Left mLeft;
    public Right mRight;
    public ScrollItem mScrollItem;
    public Item mItem;
    public SlotItem mSlotItem;
    public GameObject mTips;

    public static Dictionary<AttachType, DeckEditAttach> Attach = new Dictionary<AttachType, DeckEditAttach>();
    void Start()
    {
        Attach[Type] = this;
    }

    public void OnClickSlotItem()
    {
        SoundController.PlaySe(SoundController.SeID.Select);
        DeckEdit.Instance.SelectSlotItem(this, true);
    }

    public void OnClickRightItem()
    {
        SoundController.PlaySe(SoundController.SeID.Select);
        DeckEdit.Instance.SelectRightItem(this, true);
    }

    [Serializable]
    public class Root
    {
        public DeckEditAttach Left;
        public DeckEditAttach Right;
    }

    [Serializable]
    public class Left
    {
        public List<DeckEditAttach> SlotItems;
    }

    [Serializable]
    public class Right
    {
        public List<DeckEditAttach> ScrollItems;
        public UIGrid Grid;
    }
    [Serializable]
    public class ScrollItem
    {
        public List<DeckEditAttach> Items;

    }
    [Serializable]
    public class Item
    {
        public UISprite Icon;
        public UISprite Level1;
        public UISprite Level2;
        public UISprite ToogleSprite;
        public UISprite HighLightSprite;
        public CharaInfo CharaInfo;
        public UISprite BGSprite;
        public UIButton BGButton;

        public UILabel lv;

        private readonly Color expireCharacterColor = new Color(0.109375f, 1, 0);

        public bool WillExpire {
            get {
                return willExpire;
            }
            set {
                willExpire = value;
                if (BGSprite != null) {
                    BGSprite.color = willExpire ? expireCharacterColor : Color.white;
                } else {
                    Debug.LogError("Not set BGSprite");
                }
                if (BGButton != null) {
                    BGButton.hover = BGButton.defaultColor = willExpire ? expireCharacterColor : Color.white;
                } else {
                    Debug.LogError("Not set BGButton");
                }
            }
        }
        private bool willExpire = false;
    }

    [Serializable]
    public class SlotItem
    {
        public UISprite Icon;
        public UISprite Level1;
        public UISprite Level2;
        public UISprite ToogleSprite;
        public CharaInfo CharaInfo;

        public UILabel lv;
    }

    public enum AttachType
    {
        Root = 0,
        Left,
        Right,
        ScrollItem,
        Item,
        SlotItem,
    }

    protected override void OnReposition(int i)
    {
        base.OnReposition(i);
        transform.localScale = Vector3.one;
    }

    public void OnShowTips() {
        mTips.SetActive(true);
    }

    public void OnCloseTips() {
        mTips.SetActive(false);
    }
}
