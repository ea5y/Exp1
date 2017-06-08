using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AtlasHold : MonoBehaviour
{
    public static AtlasHold Instance;
    public List<UIAtlas> HeroIcon;
    public List<UIAtlas> HeroSkin;
    // Use this for initialization
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public UIAtlas GetHeroIcon(string pSpriteName)
    {
        for (int i = 0; i < HeroIcon.Count; i++)
        {
            if (HeroIcon[i].GetSprite(pSpriteName) == null)
            {
                continue;
            }
            return HeroIcon[i];
        }
        Debug.LogError("Can not Find " + pSpriteName);
        return new UIAtlas();
    }

    public UIAtlas GetSkinIcon(string pSpriteName)
    {
        for (int i = 0; i < HeroSkin.Count; i++)
        {
            if (HeroSkin[i].GetSprite(pSpriteName) == null)
            {
                continue;
            }
            return HeroSkin[i];
        }
        Debug.LogError("Can not Find " + pSpriteName);
        return new UIAtlas();
    }
}
