using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class GUICharacterGroupItem : MonoBehaviour {
    public List<GUICharacterItem> listGUICharacterItem = new List<GUICharacterItem>();
    int _index = 0;

    public int index
    {
        get { return _index; }
        set { _index = value; }
    }
    public void Setup(List<CharaInfo> charaInfoList, int index)
    {
        gameObject.SetActive(true);
        this.index = index;
        for (int i = 0; i < listGUICharacterItem.Count; i++)
        {
            if (i < charaInfoList.Count)
            {
                listGUICharacterItem[i].Setup(charaInfoList[i], index == 0 && i == 0);
            }
            else
            {
                listGUICharacterItem[i].gameObject.SetActive(false);
            }
        }
    }
}
