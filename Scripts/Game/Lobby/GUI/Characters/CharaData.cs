using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace XDATA
{
    public enum CharaTypt
    {
        Unknow,
        Normal,
        Upgrade,
        Evolve
    }

    public class CharaData : CustomControl.ScrollViewCellItemData 
    {
        public Chara chara;
        public bool isInDeck;

        public bool selectFlagNormal;
        public bool selectFlagUpgrade;
        public bool selectFlagEvolve;

        public CharaTypt NormalType { get { return CharaTypt.Normal; } }
        public CharaTypt UpgradeType { get { return CharaTypt.Upgrade; } }
        public CharaTypt EvolveType { get { return CharaTypt.Evolve; } }

        public CharaData(Chara chara)
        {
            this.chara = chara;
            //this.isSelect = false;
            this.selectFlagNormal = false;
            this.selectFlagUpgrade = false;
            this.selectFlagEvolve = false;
            this.isInDeck = false;            
        }

        public static List<List<CharaData>> Pack(List<CharaData> datas, int groupNum)
        {
            List<List<CharaData>> charaDataList = new List<List<CharaData>>();
            List<CharaData> charaDataGroupList = new List<CharaData>();

            List<CharaData> charaDataBufferList = new List<CharaData>(datas);

            for (int i = 0; i < Mathf.CeilToInt((float)datas.Count / groupNum); i++)
            {
                //Get group List
                charaDataGroupList = new List<CharaData>();
                for (int a = 0; a < groupNum; a++)
                {
                    if (charaDataBufferList.Count != 0)
                    {
                        charaDataGroupList.Add(charaDataBufferList[0]);
                        charaDataBufferList.RemoveAt(0);
                    }
                }

                //Get data List
                charaDataList.Add(charaDataGroupList);
            }

            return charaDataList;
        }
    }

    /*
    public class NormalCharaData : CharaData
    {
        public override CharaTypt CharaType
        {
            get
            {
                return CharaTypt.Normal;
            }
        }

        public NormalCharaData(Chara chara) : base(chara)
        {
        }
    }

    public class UpgradeCharaData : CharaData
    {
        public override CharaTypt CharaType
        {
            get
            {
                return CharaTypt.Upgrade;
            }
        }

        public UpgradeCharaData(Chara chara) : base(chara)
        { 
        }
    }

    public class EvolveCharaData : CharaData
    {
        public override CharaTypt CharaType
        {
            get
            {
                return CharaTypt.Evolve;
            }
        }

        public EvolveCharaData(Chara chara) : base(chara)
        {
        }
    }*/
}
