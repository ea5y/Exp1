using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

public class ItemRankPlayer : MonoBehaviour 
{
    public UISprite icon;
    public UISprite frame;
    public UILabel username;	

    private enum TagType
    {
        None,
        Tag1,
        Tag2
    }

    public void SetFrame(ShopItemTag tag)
    {
        if(tag == ShopItemTag.Light)
        {
            frame.spriteName = "kuang_b01";
            frame.width = 150;
            frame.height = 172;
            frame.transform.localPosition = new Vector3(6, 0, 0);
        }
        else
        {
            frame.spriteName = "lankuang";
            frame.width = 142;
            frame.height = 170;
            frame.transform.localPosition = new Vector3(0, 0, 0);
        }
    }
}
