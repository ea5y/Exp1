/// <summary>
/// ランダムの実体管理
///
/// 2013/03/14
/// </summary>
using System;

public class RandomGenerator
{
	static private Random random = new Random();
	
	/// <summary>
	/// 0以上 int.MaxValue未満 の乱数.
	/// </summary>
	static public int Next()
	{
		return random.Next();
	}
	
	/// <summary>
	/// 0以上 maxValue未満 の乱数.
	/// </summary>
	static public int Next(int maxValue)
	{
		return random.Next(maxValue);
	}
	
	/// <summary>
	/// minValue以上 maxValue未満 の乱数.
	/// </summary>
	static public int Next(int minValue, int maxValue)
	{
		return random.Next(minValue, maxValue);
	}
	
	/// <summary>
	/// 0.0以上 1.0未満 の乱数.
	/// </summary>
	static public double NextDouble()
	{
		return random.NextDouble();
	}
}
