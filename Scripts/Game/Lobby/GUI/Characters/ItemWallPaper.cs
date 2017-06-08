using UnityEngine;
using System.Collections;
using Scm.Common.Packet;

namespace XUI
{
    public class ItemWallPaper : CustomControl.ScrollViewItem
    {
        private CharacterWallpaperParameter data;
        private bool isLock = false;

        public Material dim;
        
        public UITexture bigWallpaper;
        public UITexture wallpaper;
        public UISprite inactive;
        public UISprite active;
        public UISprite imgLock;

        public UIButton btnEnter;

        private TweenScale scaleTween;
        private TweenPosition posTween;

        private string GetAssetPath(string name)
        {
            return string.Format("{0}.bytes", name);
        }

        public override void FillItem(IList datas, int index)
        {
            base.FillItem(datas, index);

            this.data = datas[index] as CharacterWallpaperParameter;

            // 生成.
            AssetReference assetReference = AssetReference.GetAssetReference("wallpaper");
            StartCoroutine(assetReference.GetAssetAsync<TextAsset>(this.GetAssetPath(data.IconFile), (res) =>
            {
                Texture2D t = new Texture2D(100, 100);
                t.LoadImage(res.bytes);
                this.wallpaper.mainTexture = t;
            }
            ));

            if (this.data.Count > 0)
            {
                this.Lock(false);
            }
            else
            {
                this.Lock(true);
            }

            this.btnEnter.onClick.Clear();
            EventDelegate.Add(this.btnEnter.onClick, this.OnClick);

            this.scaleTween = this.bigWallpaper.GetComponent<TweenScale>();
            this.posTween = this.bigWallpaper.GetComponent<TweenPosition>();
        }
        
        private void Lock(bool isLock)
        {
            this.isLock = isLock;
            this.wallpaper.color = isLock == true ? Color.gray : Color.white;
            this.wallpaper.alpha = isLock == true ? 0.5f : 1f;
            this.wallpaper.material = isLock == true ? this.dim : null;
            this.inactive.gameObject.SetActive(isLock);
            this.active.gameObject.SetActive(!isLock);
            this.imgLock.gameObject.SetActive(isLock);
            this.btnEnter.enabled = !isLock;
        }

        private void OnClick()
        {
            if(!this.isLock)
            {
                this.bigWallpaper.mainTexture = this.wallpaper.mainTexture;
                //this.bigWallpaper.gameObject.SetActive(true);
                this.posTween.from = transform.position;
                Net.Network.Instance.StartCoroutine(this.Forward());
            }
        }

        private IEnumerator Forward()
        {
            this.scaleTween = this.bigWallpaper.GetComponent<TweenScale>();
            this.posTween = this.bigWallpaper.GetComponent<TweenPosition>();
            this.scaleTween.PlayForward();
            this.posTween.PlayForward();
            yield return new WaitForSeconds(this.scaleTween.duration);
            //TODO Filter Touch
            //this.scaleTween.ResetToBeginning();
            //this.posTween.ResetToBeginning();
        }

        public void OnTweenFinished()
        {
           
        }

        public void OnBigWallpaperClick()
        {
            Net.Network.Instance.StartCoroutine(this.Reverse());
        }
        
        private IEnumerator Reverse()
        {
            this.scaleTween = this.bigWallpaper.GetComponent<TweenScale>();
            this.posTween = this.bigWallpaper.GetComponent<TweenPosition>();
            this.scaleTween.PlayReverse();
            this.posTween.PlayReverse();
            yield return new WaitForSeconds(this.scaleTween.duration);
            //this.scaleTween.ResetToBeginning();
            //this.posTween.ResetToBeginning();
        }

    }

}
