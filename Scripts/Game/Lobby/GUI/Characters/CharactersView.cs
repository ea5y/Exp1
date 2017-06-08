using UnityEngine;
using System.Collections;

namespace XUI
{
    [System.Serializable]
    public class CharactersView : MonoBehaviour
    {
        public GameObject root;
        public GameObject portrait;
        public GameObject portraitPreview;
        public UILabel charaName;
        public UILabel remainTime;
        public UIButton btnEnter;
        public UIButton btnCell;
        public UIButton btnLock;

        public UISprite[] stars;

        public UISprite itemStar;
        public GameObject starsGroup;

        public UIGrid grid;
        public GameObject itemPrefab;

        //for tween
        public GameObject group;
        public GameObject right;
        public UISprite frame;
        public UILabel lblLimit;
        public UISprite spHelp;
    }

}
