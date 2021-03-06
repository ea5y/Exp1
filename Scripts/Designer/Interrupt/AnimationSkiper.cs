/// <summary>
/// モデルアニメーションを途中中断させる.
/// 
/// 2014/01/25
/// </summary>
using UnityEngine;

/// <summary>
/// モデルアニメーションを途中中断させる.
/// </summary>
public class AnimationSkiper : MonoBehaviour, IInterrupt
{
	/// <summary>
	/// AnimationのSkip時間指定。正の値で開始から、負の値で終了からの時間.
	/// </summary>
	public float time = -1f;

	#region IInterrupt.
	public void Interrupt()
	{
		try
		{
			this.Skip();
		}
		catch(System.Exception e)
		{
			BugReportController.SaveLogFile(e.ToString());
		}
	}
	#endregion

	/// <summary>
	/// スキップ対象を検出し,処理メソッドに渡す.
	/// </summary>
	private void Skip()
	{
		// Animation処理.
		{
			var anims = this.GetComponentsInChildren<Animation>();
			foreach(var anim in anims)
			{
				SkipAnimation(anim);
			}
		}

		// Animator処理.
		{
			var anims = this.GetComponentsInChildren<Animator>();
			foreach(var anim in anims)
			{
				SkipAnimator(anim);
			}
		}
	}
	/// <summary>
	/// Animationのスキップ処理.
	/// </summary>
	private void SkipAnimation(Animation anim)
	{
		if(0f < this.time)
		{
			if(anim[anim.clip.name].time < this.time)
			{
				anim[anim.clip.name].time = this.time;
			}
		}
		else
		{
			float skipTime = anim.clip.length + this.time;
			if(anim[anim.clip.name].time < skipTime)
			{
				anim[anim.clip.name].time = skipTime;
			}
		}
	}
	/// <summary>
	/// Animationのスキップ処理.
	/// </summary>
	private void SkipAnimator(Animator anim)
	{
		if(anim != null)
		{
			anim.SetBool("IsSkip", true);
		}
	}
}
