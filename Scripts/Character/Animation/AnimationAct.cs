/// <summary>
/// アニメーションイベントの設定・実行
/// 
/// 2013/10/10
/// </summary>

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;
using Object = UnityEngine.Object;

/// <summary>
/// AnimationActSetのアニメーションイベントを処理する.
/// </summary>
[RequireComponent(typeof(Animation))]
public class AnimationAct : MonoBehaviour
{
	#region フィールド＆プロパティ
	/// <summary>
	/// このアニメーションイベントが適用されるObjectBase.
	/// </summary>
	private ObjectBase ownerObject;

	/// <summary>
	/// ScriptableObject参照保持用リスト.
	/// これが無いとResources.UnloadUnusedAssetsで勝手に削除されることがある.
	/// </summary>
	[SerializeField]
	private List<AnimationActParameter> paramList = new List<AnimationActParameter>();
	#endregion
	
	#region 作成.
	/// <summary>
	/// avatarTypeに設定されているアニメーションイベントをanimationにセットする.
	/// </summary>
	public static void SetAnimationEvent(AvatarType avatarType, Animation animation)
	{
	    try
	    {
            Dictionary<int, AnimationActSetMasterData> animActSetDic = new Dictionary<int, AnimationActSetMasterData>();
            avatarType = ObsolateSrc.GetBaseAvatarType(avatarType);
            if (!MasterData.TryGetAnimationActSet(avatarType, out animActSetDic))
            {
                return;
            }

            if (0 < animActSetDic.Count)
            {
                var animationAct = animation.gameObject.GetComponent<AnimationAct>();
                if (animationAct == null)
                {
                    animationAct = animation.gameObject.AddComponent<AnimationAct>();
                }
                if (null != animationAct)
                {
                    animationAct.Setup(animActSetDic);
                }
                else
                {
                    Debug.LogError("===> Can not Get AnimationAct");
                }
            }
	    }
	    catch (Exception)
	    {
            Debug.LogError("===> NullReferenceException");
	        //throw;
	    }
		
	}

	/// <summary>
	/// AnimationActSetMasterDataによってAnimationActの設定を行う.
	/// </summary>
	private void Setup(Dictionary<int, AnimationActSetMasterData> animActSetDic)
	{
		this.SetObjectBase();
		foreach(var animActSet in animActSetDic.Values)
		{
			if(animActSet.PlayerOnlyFlag &&
				!(this.ownerObject is Player))
			{
				// PlayerOnlyフラグが立っている場合,プレイヤー以外には適用しない.
				continue;
			}
			AnimationClip clip = GetComponent<Animation>().GetClip(animActSet.AnimationName);
			if(clip != null && GetComponent<Animation>().gameObject != null)
			{
				if(!this.SetAnimationEvent(clip, animActSet.Time, animActSet.Script, animActSet.Argument))
				{
#if UNITY_EDITOR
					Debug.LogError("AnimationActSetID = "+animActSet.ID+" is over time of AnimationClip.\r\nActTime = "+animActSet.Time+", Clip.length = "+clip.length);
#endif
				}
			}
		}
	}
	
	/// <summary>
	/// 親階層を探索してObjectBaseを取得する.
	/// </summary>
	private void SetObjectBase()
	{
		Transform parent = this.transform.parent;
		while(parent != null)
		{
			ownerObject = parent.GetComponent<ObjectBase>();
			
			if(ownerObject != null)
			{
				return;
			}
			parent = parent.parent;
		}
#if UNITY_EDITOR
		Debug.LogError("Not found ObjectBase : " + this.gameObject.name);
#endif
	}

	/// <summary>
	/// アニメーションクリップにアニメーションイベントを設定する.
	/// </summary>
	private bool SetAnimationEvent(AnimationClip clip, float time, string funcName, string funcArg)
	{
		bool ret = true;
		if(clip.length < time)
		{
			time = clip.length;
			ret = false;
		}
		AnimationEvent animEvent = new AnimationEvent();
		animEvent.time = time;
		animEvent.functionName = funcName;
		AnimationActParameter actParameter = ScriptableObject.CreateInstance<AnimationActParameter>().SetParam(clip, funcArg);
		animEvent.objectReferenceParameter = actParameter;
		paramList.Add(actParameter);

		clip.AddEvent(animEvent);
		
		return ret;
	}
	#endregion

	#region アニメーションイベントスクリプト.
	/// <summary>
	/// SEを鳴らす.位置はAnimationスクリプトの位置.
	/// </summary>
	private void playSe(Object args)
	{
		var param = args as AnimationActParameter;
		try
		{
			SoundManager.CreateSeObject(this.transform.position, this.transform.rotation, param.Str[0]);
		}
		catch(System.Exception e)
		{
			// エラー調査用.
			BugReportController.SaveLogFileWithOutStackTrace(CreateErrorLog(param) + e.ToString());
		}
	}
	/// <summary>
	/// ボイスを鳴らす.対象はownerObject.
	/// </summary>
	private void playVoice(Object args)
	{
		var param = args as AnimationActParameter;
		try
		{
			Character ownChara = this.ownerObject as Character;
			if(ownChara != null)
			{
				ownChara.CharacterVoice.Play(param.Str[0]);
			}
		}
		catch(System.Exception e)
		{
			BugReportController.SaveLogFileWithOutStackTrace(CreateErrorLog(param) + e.ToString());
		}
	}
	/// <summary>
	/// 演出カメラを使う.
	/// </summary>
	private void directCamera(Object args)
	{
		var param = args as AnimationActParameter;
		if (this.ownerObject is Player)
		{
			CharacterCamera camera = GameController.CharacterCamera;
			if(camera != null)
			{
				if(1 < param.Str.Length)
				{
					// 視点,注視点を探索.
					Transform cPos = this.transform.SafeFindChild(param.Str[0]);
					Transform cTar = this.transform.SafeFindChild(param.Str[1]);
					
					// 現在のモーションが再生されなくなったら終了.
					camera.CreateDirectCamera(cPos, cTar, () =>
					{
						if(this != null)
						{
							return this.GetComponent<Animation>().IsPlaying(param.Clip.name);
						}
						else
						{
							// this==nullは妙な判定だが,thisがUnityシーンヒエラルキー上に無い場合を指す.
							return false;
						}
					});
				}
			}
		}
	}
	/// <summary>
	/// 通常カメラに戻す.
	/// </summary>
	private void resetCamera(Object args)
	{
		//var param = args as AnimationActParameter;
		if (this.ownerObject is Player)
		{
			CharacterCamera camera = GameController.CharacterCamera;
			if(camera != null)
			{
				camera.RemoveDirectCamera();
			}
		}
	}
	#endregion

	#region エラーログ作成.
	private string CreateErrorLog(Object args)
	{
		var param = args as AnimationActParameter;
		string paramStr0;
		string paramClipname;
		if(param != null)
		{
			if(param.Str != null)
			{
				if(0 < param.Str.Length)
				{
					paramStr0 = param.Str[0];
				}
				else
				{
					paramStr0 = "Length0";
				}
			}
			else
			{
				paramStr0 = "null";
			}

			if(param.Clip != null)
			{
				if(param.Clip.name != null)
				{
					paramClipname = param.Clip.name;
				}
				else
				{
					paramClipname = "name null";
				}
			}
			else
			{
				paramClipname = "Clip null";
			}
		}
		else
		{
			paramClipname = "Param null";
			paramStr0 = string.Empty;
		}
#if UNITY_EDITOR && XW_DEBUG
		return paramClipname + " : " + paramStr0 + "\r\n LoadFromLocalFolder : " + ScmParam.Debug.File.LoadFromLocalFolder;
#else
		return paramClipname + " : " + paramStr0 + "\r\n";
#endif
	}
	#endregion
}
