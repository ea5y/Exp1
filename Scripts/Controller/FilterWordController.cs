using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class FilterWordController : Singleton<FilterWordController>
{
    const char replacedChar = '*';
    List<string> filterWordList = new List<string>();

	// Use this for initialization
	void Start () {
        
	}

    public void Init()
    {
        filterWordList.Clear();
        filterWordList.AddRange(MasterData.TryGetChinaNgWordMasterData());
    }

    public string GetFilteredWord(string target)
    {
        if (string.IsNullOrEmpty(target))
        {
            return target;
        }

        StringBuilder tmp = new StringBuilder(target);

        foreach (var item in filterWordList)
        {
            tmp.Replace(item, new string(replacedChar, item.Length));
        }
        return tmp.ToString();
    }

    public string GetFilteredWord2(string target)
    {
        if (string.IsNullOrEmpty(target))
        {
            return target;
        }

        string result = target;

        foreach (var item in filterWordList)
        {
            result = result.Replace(item, new string(replacedChar, item.Length));
        }
        return result;
    }

    public bool IsNeedFilter(string target)
    {
        if (string.IsNullOrEmpty(target))
        {
            return false;
        }

        bool result = false;
        foreach (var item in filterWordList)
        {
            if (target.Contains(item))
            {
                result = true;
                break;
            }
        }

        return result;
    }
}
