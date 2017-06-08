using UnityEngine;
using System.Collections;
using System;

namespace XUI
{
    [System.Serializable]
    public class CharacterDetialView : MonoBehaviour
    {
        public GameObject root;
        
        public CharacterInfoView infoView;
        public CharacterGrowView growView;
        public CharacterSkinView skinView;
        public CharacterWallpaper wallpaperView;

        public UIButton btnInfo;
        public UIButton btnGrow;
        public UIButton btnSkin;
        public UIButton btnWallpaper;

        //for tween
        public GameObject right;
        public GameObject tabMenu;
        
    }
    
    [System.Serializable]
    public class CharacterInfoView : CustomControl.IPage
    {
        public GameObject root;

        public UISprite skill_1;
        public UISprite skill_2;
        public UISprite skill_3;
        public UISprite skill_4;
        public GameObject skillDesc;
        public UILabel lblSkillName;
        public UILabel lblSkillDesc;

        public UILabel back;
        
        public GameObject abilityView;
        public GameObject backView;
        public UIButton btnAbility;
        public UIButton btnBack;

        public UILabel hp;
        public UILabel def;
        public UILabel atk;
        public UILabel spd;

        public GameObject Root
        {
            get
            {
                return this.root;
            }
        }

        public void Init()
        {
            //Init
        }
    }

    [System.Serializable]
    public class CharacterGrowView : CustomControl.IPage
    {
        public GameObject root;
        public UIButton btnUpgrade;
        public UIButton btnEvolve;

        public GameObject itemCharacters;

        [SerializeField]
        UpgradePage upgrade;
        public UpgradePage UpgradeView { get { return this.upgrade; } }

        [SerializeField]
        EvolvePage evolveView;
        public EvolvePage EvolveView { get { return this.evolveView; } }

        public GameObject Root
        {
            get
            {
                return this.root;
            }
        }

        public void Init()
        {
            //Init
        }
    }

    [System.Serializable]
    public class UpgradePage : CustomControl.IPage
    {
        public GameObject root;

        public GameObject Root { get { return this.root; } }
        public GameObject itemUpgrade;

        public void Init()
        {            
            GUICharacterDetial.Instance.ResetUpgradeScrollView();
        }

        public UILabel hp;
        public UILabel def;
        public UILabel atk;
        public UILabel spd;

        public UILabel lvCurrent;
        public UILabel lvTarget;

        public UISprite expCurrent;
        public UISprite expTarget;
        public UILabel exp;

        public UIGrid grid;

        public UILabel curGold;
        public UILabel price;

        public UIButton btnSure;
        public GameObject afx;
    }

    [System.Serializable]
    public class EvolvePage : CustomControl.IPage
    {
        public GameObject root;

        public GameObject Root { get { return this.root; } }
        public GameObject itemEvolve;

        public void Init()
        {
            GUICharacterDetial.Instance.ResetEvolveScrollView();
        }

        public UILabel hp;
        public UILabel def;
        public UILabel atk;
        public UILabel spd;

        public UILabel lblTip;

        public UIGrid grid;

        public UILabel curGold;
        public UILabel price;

        public UIButton btnSure;
        public GameObject afx;
    }

    [System.Serializable]
    public class CharacterSkinView : CustomControl.IPage
    {
        public GameObject root;
        public GameObject Root
        {
            get
            {
                return this.root;
            }
        }

        public void Init()
        {
            foreach(Transform c in this.realView.transform)
            {
                GameObject.Destroy(c.gameObject);
            }
        }

        public UIScrollView scrollView3D;
        public GameObject realView;
        public GameObject itemSkinCard;

        public UIButton btnSure;
        public UIButton btnShop;
        public UILabel lblStatus;

        public Material skinMaskMaterial;

        public GameObject portriat3D;
    }

    [System.Serializable]
    public class CharacterWallpaper : CustomControl.IPage
    {
        public GameObject root;
        public GameObject Root
        {
            get
            {
                return this.root;
            }
        }

        public UIGrid grid;
        public GameObject itemWallpaper;
        
        public void Init()
        {
            //None
        }
    }
}
