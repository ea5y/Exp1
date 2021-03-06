/// <summary>
/// エフェクトのパーティクルやモデルアニメーションを途中中断させるためのインターフェイス.
/// 
/// 2014/01/25
/// </summary>
using UnityEngine;

/// <summary>
/// エフェクトのパーティクルやモデルアニメーションを途中中断させるためのインターフェイス.
/// </summary>
public interface IInterrupt
{
	void Interrupt();
}
