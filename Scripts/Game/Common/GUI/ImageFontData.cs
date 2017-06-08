using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "J-World/FontData")]
public class ImageFontData : ScriptableObject
{
    public List<FontData> SmallFont;
    public List<FontData> BigFont;


    private Dictionary<string, string> SmallFontDic = new Dictionary<string, string>();
    private Dictionary<string, string> BigFontDic = new Dictionary<string, string>();

    public void Init()
    {
        if (SmallFontDic.Count > 0)
        {
            return;
        }
        SmallFont.ForEach(v =>
        {
            SmallFontDic[v.Num.ToString()] = v.SpriteName;
        });

        BigFont.ForEach(v =>
        {
            BigFontDic[v.Num.ToString()] = v.SpriteName;
        });
    }

    public string GetSmallFont(string pFont)
    {
        return SmallFontDic[pFont];
    }

    public string GetBigFont(string pFont)
    {
        return BigFontDic[pFont];
    }
}

[Serializable]
public class FontData
{
    public int Num;
    public string SpriteName;
}
