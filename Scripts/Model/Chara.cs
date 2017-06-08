using UnityEngine;
using System.Collections;
using System;
using Scm.Common.Packet;

namespace XDATA
{
    public class Chara
    {
        public CharaInfo Info { get; private set; }

        public Chara(CharaInfo info)
        {
            this.Info = info;           
        }

        public void UpdateInfo(CharaInfo info)
        {
            this.Info = info;
        }

        public IEnumerator GetSkin(Action<GetCharacterAvatarAllRes> callback)
        {
            yield return Net.Network.GetSkinInfo((long)this.Info.UUID, callback);
        }

        public IEnumerator GetWallpaper(Action<GetCharacterWallpaperAllRes> callback)
        {
            yield return Net.Network.GetWallpaperInfo((long)this.Info.UUID, callback);
        }

        public IEnumerator Lock(Action<SetLockPlayerCharacterRes> callback)
        {
            yield return Net.Network.LockChara(this.Info.UUID, !this.Info.IsLock, callback);
        }
    }
}

