using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace XUI
{
    [System.SerializableAttribute]
    public class ShopView : MonoBehaviour
    {
        public GameObject root;

        public TabBtn tabBtn;
        public ShopViewXD xd;
        public ShopViewPF pf;
        public ShopViewJS js;
        public ShopViewCZ cz;
        public ShopViewDQ dq;
        public ShopViewLB lb;

        public ShopViewDetail detail;
    }

    [System.SerializableAttribute]
    public class TabBtn
    {
        public UIButton btnXD;
        public UIButton btnPF;
        public UIButton btnJS;
        public UIButton btnCZ;
        public UIButton btnDQ;
        public UIButton btnLB;
    }
    
    [System.SerializableAttribute]
    public class ShopViewXD : CustomControl.IPage
    {
        public GameObject root;
        public Action action;

        public GameObject portriat;
        public List<ItemShopXD> items;
        public UIButton btnStart;
        public UIButton btnShowOne;
        public UIButton btnReset;
        public UILabel lblPrice;
        public UILabel lblResetPrice;
        public UILabel lblTime;
        public UILabel lblName;

        public UIButton btnTip;
        public GameObject groupTip;
        public UIButton btnTipClose;

        public GameObject Root
        {
            get
            {
                return root;
            }
        }

        public void Init()
        {
            if(action != null)
                action();
        }
    }

    [System.SerializableAttribute]
    public class ShopViewPF : CustomControl.IPage
    {
        public GameObject root;
        public Action action;

        public GameObject Root
        {
            get
            {
                return root;
            }
        }

        public void Init()
        {
            if(action != null)
                action();
        }

        public UIButton btn3Day;
        public UIButton btn7Day;
        public UIButton btn30Day;
        public UIButton btnLong;

        public PagePF3Day page3Day;
        public PagePF7Day page7Day;
        public PagePF30Day page30Day;
        public PagePFLong pageLong;
    }     

    [System.SerializableAttribute]
    public class PagePF3Day : CustomControl.IPage
    {
        public GameObject root;

        public GameObject itemPF;
        public UIGrid grid;

        public GameObject Root
        {
            get
            {
                return root;
            }
        }

        public void Init()
        {
        }
    }

    [System.SerializableAttribute]
    public class PagePF7Day : CustomControl.IPage
    {
        public GameObject root;

        public GameObject itemPF;
        public UIGrid grid;

        public GameObject Root
        {
            get
            {
                return root;
            }
        }

        public void Init()
        {
        }
    }

    [System.SerializableAttribute]
    public class PagePF30Day : CustomControl.IPage
    {
        public GameObject root;

        public GameObject itemPF;
        public UIGrid grid;

        public GameObject Root
        {
            get
            {
                return root;
            }
        }

        public void Init()
        {
        }
    }

    [System.SerializableAttribute]
    public class PagePFLong : CustomControl.IPage
    {
        public GameObject root;

        public GameObject itemPF;
        public UIGrid grid;

        public GameObject Root
        {
            get
            {
                return root;
            }
        }

        public void Init()
        {
        }
    }

    [System.SerializableAttribute]
    public class ShopViewJS : CustomControl.IPage
    {
        public GameObject root;
        public Action action;

        public GameObject Root
        {
            get
            {
                return root;
            }
        }

        public void Init()
        {
            if(action != null)
                action();
        }
        
        public UIButton btn3Day;
        public UIButton btn7Day;
        public UIButton btn30Day;
        public UIButton btnLong;

        public PageJS3Day page3Day;
        public PageJS7Day page7Day;
        public PageJS30Day page30Day;
        public PageJSLong pageLong;
    }     

    [System.SerializableAttribute]
    public class PageJS3Day : CustomControl.IPage    
    {
        public GameObject root;

        public GameObject itemJS;
        public UIGrid grid;

        public GameObject Root
        {
            get
            {
                return root;
            }
        }

        public void Init()
        {
        }
    }

    [System.SerializableAttribute]
    public class PageJS7Day : CustomControl.IPage    
    {
        public GameObject root;

        public GameObject itemJS;
        public UIGrid grid;

        public GameObject Root
        {
            get
            {
                return root;
            }
        }

        public void Init()
        {
        }
    }

    [System.SerializableAttribute]
    public class PageJS30Day : CustomControl.IPage    
    {
        public GameObject root;

        public GameObject itemJS;
        public UIGrid grid;

        public GameObject Root
        {
            get
            {
                return root;
            }
        }

        public void Init()
        {
        }
    }

    [System.SerializableAttribute]
    public class PageJSLong : CustomControl.IPage    
    {
        public GameObject root;

        public GameObject itemJS;
        public UIGrid grid;

        public GameObject Root
        {
            get
            {
                return root;
            }
        }

        public void Init()
        {
        }
    }


    [System.SerializableAttribute]
    public class ShopViewCZ : CustomControl.IPage
    {
        public GameObject root;
        public Action action;
        public List<ItemShopCZ> items;

        public GameObject Root
        {
            get
            {
                return root;
            }
        }

        public void Init()
        {
            if(action != null)
                action();
        }
     
    }     

    [System.SerializableAttribute]
    public class ShopViewDQ : CustomControl.IPage
    {
        public GameObject root;
        public Action action;
        public List<ItemShopDQ> items;
        public UILabel lblEnergy;

        public GameObject Root
        {
            get
            {
                return root;
            }
        }

        public void Init()
        {
            if(action != null)
                action();
        }
 
    }                 

    [System.SerializableAttribute]
    public class ShopViewLB : CustomControl.IPage    
    {
        public GameObject root;
        public UIButton btnGo;
        public GameObject container;
        public GameObject imgParent;

        public GameObject Root
        {
            get
            {
                return root;
            }
        }

        public void Init()
        {
            //None
        }
    }

    [System.SerializableAttribute]
    public class ShopViewDetail
    {
        public GameObject root;
        public GameObject portriat;
        public UILabel lblName;
        public UILabel lblTime;
        public ShopViewDetailJS detailJS;
        public ShopViewDetailPF detailPF;
    }

    [System.SerializableAttribute]
    public class ShopViewDetailJS : CustomControl.IPage
    {
        public GameObject root;
        public GameObject Root
        {
            get{return this.root;}
        }

        public void Init()
        {
        }
        public GameObject radar;
        public Material mtRadar;

        public UISprite skill_1;
        public UISprite skill_2;
        public UISprite skill_3;
        public UISprite skill_4;

        public GameObject skillDesc;
        public UILabel lblSkillName;
        public UILabel lblSkillDesc;

        public UIButton btnBuy;
        public UIButton btnCancel;
        public UILabel lblPrice;
    }

    [System.SerializableAttribute]
    public class ShopViewDetailPF : CustomControl.IPage
    {
        public GameObject root;
        public GameObject Root
        {
            get{return this.root;}
        }

        public void Init()
        {
        }

        public GameObject portriat3D;

        public UIButton btnBuy;
        public UIButton btnCancel;
        public UILabel lblPrice;
    }

}

