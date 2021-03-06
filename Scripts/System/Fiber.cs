/// <summary>
/// 
/// 
/// 2014/05/27
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#region FiberSet
/// <summary>
/// 複数のコルーチンを制御するクラス.
/// </summary>
public class FiberSet
{
	private List<Fiber> fiberlist;

	/// <summary>
	/// 現在保持しているコルーチンの数を返す.
	/// </summary>
	public int Count { get { return fiberlist.Count; } }

	public FiberSet()
	{
		fiberlist = new List<Fiber>();
	}

	/// <summary>
	/// コルーチンを実行する.全てのコルーチンが終了している場合falseを返す.
	/// </summary>
	public bool Update()
	{
		foreach(Fiber fiber in fiberlist.ToArray())
		{
			fiber.Update();
			if(!fiber.IsWait)
			{
				fiberlist.Remove(fiber);
			}
		}

		return (fiberlist.Count != 0);
	}
	/// <summary>
	/// 実行リストに新たなコルーチンを加える.
	/// </summary>
	public Fiber AddFiber(IEnumerator fiber)
	{
		Fiber ret = new Fiber(fiber);
		fiberlist.Add(ret);
		return ret;
	}
	/// <summary>
	/// 実行リストに新たなコルーチンを加える.
	/// </summary>
	public Fiber AddFiber(Fiber fiber)
	{
		fiberlist.Add(fiber);
		return fiber;
	}
	/// <summary>
	/// 実行リストからコルーチンを消す.
	/// </summary>
	public bool Remove(Fiber fiber)
	{
		return fiberlist.Remove(fiber);
	}
	/// <summary>
	/// 実行リストを全消去する.
	/// </summary>
	public void Clear()
	{
		fiberlist.Clear();
	}
	/// <summary>
	/// コルーチンが登録されているかどうか.
	/// </summary>
	public bool Contains(Fiber fiber)
	{
		return fiberlist.Contains(fiber);
	}
}
#endregion

#region Fiber
/// <summary>
/// コルーチン制御クラス.
/// </summary>
public class Fiber : IFiberWait
{
	private readonly IEnumerator fiber;

	/// <summary>
	/// コルーチンが正常に終了したかどうか.
	/// </summary>
	public bool IsFinished{ get { return !(this.IsWait); } }
	/// <summary>
	/// コルーチンがエラー終了したかどうか.
	/// </summary>
	public bool IsError{ get; private set; }
	/// <summary>
	/// コルーチンが実行中かどうか.IFiberWait継承.
	/// </summary>
	public bool IsWait{ get; private set; }

	public Fiber(IEnumerator fiber)
	{
		this.fiber = fiber;
		this.IsWait = true;
		this.IsError = false;
	}

	public bool Update()
	{
		IFiberWait wait = this.fiber.Current as IFiberWait;
		if(wait != null && wait.IsWait)
		{
			// Waitが掛かっているので今回は何もしない.
			this.IsWait  = true;
		}
		else
		{
			try
			{
				this.IsWait  = this.fiber.MoveNext();
			}
			catch(System.Exception e)
			{
				BugReportController.SaveLogFile(e.ToString());
				IsError = true;
				this.IsWait  = false;
			}
		}
		
		return this.IsWait ;
	}
}
#endregion

#region IFiberWait
/// <summary>
/// コルーチン制御待ちクラス.
/// </summary>
public interface IFiberWait
{
	bool IsWait { get; }
}

/// <summary>
/// 指定秒数コルーチンの実行待ちをする.
/// </summary>
public class WaitSeconds : IFiberWait
{
	float endTime;
	public WaitSeconds(float waitTime)
	{
		this.endTime = waitTime + Time.time;
	}
	public bool IsWait
	{
		get
		{
			return (Time.time < this.endTime);
		}
	}
}
#endregion