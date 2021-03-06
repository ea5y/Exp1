/// <summary>
/// サウンドコントローラー
/// 
/// 2013/08/15
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundController : Singleton<SoundController> {

	#region キューID定義
	public enum BgmID
	{
		None    =  0,
		Title   =  1,
		Lobby   =  2,
		Stage01_Free   =  3,
		Stage01_Normal =  4,
		Stage01_Limit  =  5,
		Victory =  9,
		Lose    = 10,
		//Result  = 11,		// Victory,LoseのBGMをresult画面でも流し続ける.
		Ranking = 12,
		CharaSelect    = 13
	}
	public enum JingleID
	{
		None      =  0,
		//Victory   = 14,	// VictoryとLoseのJingleはBgmのイントロに統合.
		//Lose      = 15,
		GameStart = 18,
		//GameEnd = 19
	}
	public enum SeID
	{
		// 名前はwavファイル名から定型句を除いたもの.
		Enter       = 0,
		Cancel      = 1,
		Select      = 2,
		WindowOpen  = 3,
		WindowClose = 4,
		Button      = 7,
		LockOn      = 8,
		LockOff     = 9,
	}
	#endregion
	
	#region フィールド＆プロパティ
	// acfファイル名.
	private const string AcfName = "X_world.acf";
	// キューシート名.
	private const string cueSheetName_BGM = "X_world_Bgm";
	private const string cueSheetName_SE = "X_world_Se";
    private const string cueSheetName_UI = "X_world_Voice_p001";
	// 決め打ちキュー名.
	public const string CueName_SysHit = "c000_se_syscmn_hit";

	// ボリューム補正.
	private const float Bgm_VolumeLate = 0.4f;
	private const float Se_VolumeLate = 1f;
	private const float Jingle_VolumeLate = 1f;
	private const float Voice_VolumeLate = 1f;

	// コンフィグ用.
	static float bgm_Volume = 1;
	static public float Bgm_Volume
	{
		get { return bgm_Volume; }
		set
		{
			bgm_Volume = value;
			SetBgmVolume(bgm_Volume);
		}
	}
	// コンフィグ用.
	static float se_Volume = 1;
	static public float Se_Volume
	{
		get { return se_Volume; }
		set
		{
			se_Volume = value;
			SetSeVolume(se_Volume);
		}
	}
	// コンフィグ用.
	static float voice_Volume = 1;
	static public float Voice_Volume
	{
		get { return voice_Volume; }
		set
		{
			voice_Volume = value;
			SetVoiceVolume(voice_Volume);
		}
	}

	CriAtomSource atomSourceSe;
	CriAtomSource atomSourceBgm;
	CriAtomSource atomSourceJingle;
    CriAtomSource atomSourceUI;
	#endregion

	#region Unityリフレクション
	protected override void Awake ()
	{
		base.Awake();
		
		// Bgm音用のCriAtomSource作成
		atomSourceBgm = gameObject.AddComponent<CriAtomSource> ();
		atomSourceBgm.cueSheet = cueSheetName_BGM;

		// SE音用のCriAtomSource作成
		atomSourceSe = gameObject.AddComponent<CriAtomSource> ();
		atomSourceSe.cueSheet = cueSheetName_SE;

		// Jingle音用のCriAtomSource作成
		atomSourceJingle = gameObject.AddComponent<CriAtomSource> ();
		atomSourceJingle.cueSheet = cueSheetName_BGM;

        // UI测试
        atomSourceUI = gameObject.AddComponent<CriAtomSource>();
        atomSourceUI.cueSheet = cueSheetName_UI;
	}

	void Start () {
		// DSPバス設定: CRIWARE Object 側で指定済み
		//CriAtom.AttachDspBusSetting("DspBusSetting_0");
		
		// インゲームプレビュー時のレベルモニタ機能追加
		//CriAtom.SetBusAnalyzer(true);

		this.enabled = false;
	}
	#endregion
	
	#region Method
	static public void ReStart()
	{
		// ACBのアンロード.
		CriAtom.RemoveCueSheet(cueSheetName_BGM);
		CriAtom.RemoveCueSheet(cueSheetName_SE);
        CriAtom.RemoveCueSheet(cueSheetName_UI);
		// ACFのアンレジスト.
		CriAtomEx.UnregisterAcf();
		
		// 新しいACFの登録.
		string acfPath = AssetReference.GetAssetBundlePath(AcfName);
		acfPath = System.IO.Path.GetFullPath(acfPath);
		CriAtomEx.RegisterAcf(null, acfPath);
		CriAtomEx.AttachDspBusSetting("DspBusSetting_0");
		// 新しいACBのロード.
		AddAssetCueSheet(cueSheetName_BGM);
		AddAssetCueSheet(cueSheetName_SE);
        AddAssetCueSheet(cueSheetName_UI);
		SetBgmVolume(bgm_Volume);
		SetSeVolume(se_Volume);
		SetVoiceVolume(voice_Volume);
	}
	/// <summary>
	/// BgmとJingleの音量をセットする.
	/// </summary>
	static private void SetBgmVolume(float volume)
	{
		CriAtom.SetCategoryVolume("Bgm", Bgm_VolumeLate * volume);
		CriAtom.SetCategoryVolume("Jingle", Jingle_VolumeLate * volume);
	}
	/// <summary>
	/// Seの音量をセットする.
	/// </summary>
	static private void SetSeVolume(float volume)
	{
		CriAtom.SetCategoryVolume("Se", Se_VolumeLate * volume);
	}
	/// <summary>
	/// Voiceの音量をセットする.
	/// </summary>
	static private void SetVoiceVolume(float volume)
	{
		CriAtom.SetCategoryVolume("Voice", Voice_VolumeLate * volume);
	}

	/// <summary>
	/// DLしてきたキューシートをロードする.
	/// </summary>
	static public CriAtomCueSheet AddAssetCueSheet(string cueSheetName)
	{
		string path = AssetReference.GetAssetBundlePath(cueSheetName+".acb");
		path = System.IO.Path.GetFullPath(path);
		return CriAtom.AddCueSheet(cueSheetName, path, path+".awb");
	}
	#region BGM
	
	private CriAtomExPlayback playbackBGM;
	private BgmID playBgmId = BgmID.None;
	
	/// <summary>
	/// BGMを鳴らす.
	/// </summary>
	static public void PlayBGM(BgmID bgmID, bool forceBegin = false)
	{
		if(Instance != null)
		{
			Instance._PlayBGM(bgmID, forceBegin);
		}
	}
	private void _PlayBGM(BgmID bgmID, bool forceBegin)
	{
		if(this.atomSourceBgm != null)
		{
			if( forceBegin ||
				playBgmId != bgmID ||
				atomSourceBgm.status == CriAtomSource.Status.Stop ||
				atomSourceBgm.status == CriAtomSource.Status.PlayEnd)
			{
				this.atomSourceBgm.Stop();
				this.playbackBGM = this.atomSourceBgm.Play((int)bgmID);
				this.playBgmId = bgmID;
			}
		}
	}
	
	/// <summary>
	/// ブロックを指定してBGMを鳴らす.
	/// </summary>
	static public void PlayBgmBlock(BgmID bgmID, int block)
	{
		if(Instance != null)
		{
			Instance._PlayBgmBlock(bgmID, block);
		}
	}
	private void _PlayBgmBlock(BgmID bgmID, int block)
	{
		if(this.atomSourceBgm != null)
		{
			// 現在のBGMと異なる場合、bgmIDのBGMを鳴らす.
			this._PlayBGM(bgmID, false);
			
			// ブロックをセットする.
			CriAtomExAcb acb = CriAtom.GetAcb(this.atomSourceBgm.cueSheet);
			if(acb != null){
				CriAtomEx.CueInfo cueInfo;
				if(acb.GetCueInfo((int)this.playBgmId, out cueInfo))
				{
					if(cueInfo.numBlocks > 0){
						this.playbackBGM.SetNextBlockIndex(block % cueInfo.numBlocks);
					}
				}
			}
		}
	}
	
	/// <summary>
	/// BGMを止める.
	/// </summary>
	static public void StopBGM()
	{
		if(Instance != null)
		{
			Instance._StopBGM();
		}
	}
	private void _StopBGM()
	{
		if(this.atomSourceBgm != null)
		{
			this.atomSourceBgm.Stop();
		}
	}
	
	/// <summary>
	/// BGMを一時停止する.
	/// </summary>
	static public void PauseBGM()
	{
		if(Instance != null)
		{
			Instance._PauseBGM();
		}
	}
	private void _PauseBGM()
	{
		if(this.atomSourceBgm != null)
		{
			this.atomSourceBgm.Pause(true);
		}
	}
	
	/// <summary>
	/// BGMの一時停止を解除する.
	/// </summary>
	static public void ResumeBGM()
	{
		if(Instance != null)
		{
			Instance._ResumeBGM();
		}
	}
	private void _ResumeBGM()
	{
		if(this.atomSourceBgm != null)
		{
			this.atomSourceBgm.Pause(false);
		}
	}
	
	#endregion

	#region Jingle
	
	/// <summary>
	/// Jingleを鳴らす.
	/// </summary>
	static public void PlayJingle(JingleID jingleID)
	{
		if(Instance != null)
		{
			Instance._PlayJingle(jingleID);
		}
	}
	private void _PlayJingle(JingleID jingleID)
	{
		if(this.atomSourceJingle != null)
		{
			this.atomSourceJingle.Play((int)jingleID);
		}
	}
	/// <summary>
	/// Jingleを止める.
	/// </summary>
	static public void StopJingle()
	{
		if(Instance != null)
		{
			Instance._StopJingle();
		}
	}
	private void _StopJingle()
	{
		if(this.atomSourceJingle != null)
		{
			this.atomSourceJingle.Stop();
		}
	}

	#endregion
	
	#region SE

	/// <summary>
	/// SEを鳴らす（2Dサウンド用）.
	/// </summary>
	static public void PlaySe(string cueName)
	{
		if(Instance != null)
		{
			Instance._PlaySe(cueName);
		}
	}
	private void _PlaySe(string cueName)
	{
		if(this.atomSourceSe != null)
		{
			this.atomSourceSe.Play(cueName);
		}
	}
	/// <summary>
	/// SEを鳴らす（2Dサウンド用）.
	/// </summary>
	static public void PlaySe(SeID seID)
	{
		if(Instance != null)
		{
			Instance._PlaySe(seID);
		}
	}
	private void _PlaySe(SeID seID)
	{
		if(this.atomSourceSe != null)
		{
			this.atomSourceSe.Play((int)seID);
		}
	}

    static public void PlayUISe(string cueSheetName, string cueName)
    {
        if (Instance != null)
        {
            Instance._PlayUISe(cueSheetName,cueName);
        }
    }
    private void _PlayUISe(string cueSheetName, string cueName)
    {
        if (this.atomSourceUI != null)
        {
            atomSourceUI.Stop();
            CriAtomCueSheet cueSheet = CriAtom.GetCueSheet(cueSheetName);
            if (cueSheet == null)
            {
                // cueSheetのロード.
                cueSheet = SoundController.AddAssetCueSheet(cueSheetName);
            }
            atomSourceUI.cueSheet = cueSheet.name;
            atomSourceUI.Stop();
            this.atomSourceUI.Play(cueName);
        }
    }

	/// <summary>
	/// SEを鳴らす（3Dサウンド用 / 音源オブジェクト指定）.空文字やnull文字では処理を行わない(nullを返す).
	/// </summary>
	static public CriAtomSource AddSeSource(GameObject go, string cueName)
	{
		// SE音用のCriAtomSource作成
		CriAtomSource cas = null;
		if(!string.IsNullOrEmpty(cueName) && go != null)
		{
			cas = go.AddComponent<CriAtomSource> ();
			cas.cueSheet = cueSheetName_SE;
			cas.Play(cueName);
		}
		return cas;
	}

	/// <summary>
	/// SEを鳴らす（3Dサウンド用 / 音源位置指定）.空文字やnull文字では処理を行わない(nullを返す).
	/// </summary>
	static public GameObject CreateSeObject(Vector3 position, Quaternion rotation, string cueName)
	{
		return SoundManager.CreateSeObject(position, rotation, cueName);
	}
	#endregion

	#endregion
}
