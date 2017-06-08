/// <summary>
/// アニメーションイベント用データクラス
/// 
/// 2014/02/12
/// </summary>
using UnityEngine;

[System.Serializable]
public class AnimationActParameter : ScriptableObject
{
	/// <summary>
	/// イベントを呼び出したクリップ情報.
	/// </summary>
	public AnimationClip Clip { get { return clip;} }
	[SerializeField]
	private AnimationClip clip;

	/// <summary>
	/// パラメータ文字列.
	/// </summary>
	public string[] Str{ get { return str;} }
	[SerializeField]
	private string[] str;

	public AnimationActParameter SetParam(AnimationClip clip, string str)
	{
		this.clip = clip;
		this.str = str.Split(',');

		return this;
	}
}
