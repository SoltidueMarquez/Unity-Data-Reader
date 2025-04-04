using System.Collections.Generic;
using UnityEngine;

public class Tests : ScriptableObject
{
    /// <summary>
    /// 根据ID获取list所在的index标记位
    /// </summary>
    public Dictionary<int, int> idStorageDic = new Dictionary<int, int>();
    public List<Test> testList = new List<Test>();

    public void AddList(int id, Test data)
    {
        data.count = new List<T>();  // 无二级分组忽略此行代码
        testList.Add(data);
        idStorageDic.Add(id, testList.Count - 1);
    }

    // 无二级分组忽略此方法
    public void SecondAddList(int id, T secData)
    {
        testList[idStorageDic[id]].count.Add(secData);
    }
}

