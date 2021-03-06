/// <summary>
/// ヒエラルキーのソート
/// 
/// 2013/05/02
/// </summary>
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class AlphaNumericSort : BaseHierarchySort
{
	public override int Compare(GameObject lhs, GameObject rhs)
	{
		if (lhs == rhs) return 0;
		if (lhs == null) return -1;
		if (rhs == null) return 1;
		return EditorUtility.NaturalCompare(lhs.name, rhs.name);
	}
}

public class AlphaNumericInvSort : BaseHierarchySort
{
	public override int Compare(GameObject lhs, GameObject rhs)
	{
		if (lhs == rhs) return 0;
		if (lhs == null) return 1;
		if (rhs == null) return -1;
		return -EditorUtility.NaturalCompare(lhs.name, rhs.name);
	}
}

public class LayerSort : BaseHierarchySort
{
	public override int Compare(GameObject lhs, GameObject rhs)
	{
		if (lhs == rhs) return 0;
		if (lhs == null) return 1;
		if (rhs == null) return -1;
		if (lhs.layer == rhs.layer) return 0;
		if (lhs.layer > rhs.layer) return 1;
		return -1;
	}
}

public class LayerInvSort : BaseHierarchySort
{
	public override int Compare(GameObject lhs, GameObject rhs)
	{
		if (lhs == rhs) return 0;
		if (lhs == null) return -1;
		if (rhs == null) return 1;
		if (lhs.layer == rhs.layer) return 0;
		if (lhs.layer > rhs.layer) return -1;
		return 1;
	}
}
