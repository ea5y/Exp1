using UnityEngine;
using System.Collections;

public class ItemShopLB : CustomControl.ScrollView3DItem
{
    public UITexture img;

    public override void FillItem(IList datas, int index)
    {
        this.Index = index;

        this.widgets.Add(img);
        // 生成.
        AssetReference assetReference = AssetReference.GetAssetReference("Activity");
        StartCoroutine(assetReference.GetAssetAsync<TextAsset>(this.GetAssetPath((string)datas[index]), (res) =>
        {
            Texture2D t = new Texture2D(100, 100);
            t.LoadImage(res.bytes);
            this.img.mainTexture = t;
        }
        ));

        XUI.GUIShop.Instance.lbPage.counter++;
    }

    public override void Finish()
    {
        base.Finish();
    }

    private string GetAssetPath(string name)
    {
        return string.Format("{0}.bytes", name);
    }
}
