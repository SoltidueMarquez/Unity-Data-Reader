using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Tests : ScriptableObject
{
    /// <summary>
    /// 根据ID获取list所在的index标记位
    /// </summary>
    public Dictionary<int, int> idStorageDic = new Dictionary<int, int>();
    public List<Test> list = new List<Test>();

    public void AddList(int id, Test data)
    {
        data.count = new();  // 无二级分组忽略此行代码
        list.Add(data);
        idStorageDic.Add(id, list.Count - 1);
    }

    // 无二级分组忽略此方法
    public void SecondAddList(int id, T2 secData)
    {
        list[idStorageDic[id]].count.Add(secData);
    }
}

[System.Serializable]
public class Test
{
	//公开 字段类型 字段名（与Excel写的一致）
    public int id;
    public string name;
    public int[] desc;
    public bool icon;
    public TestType type;
    public Vector2 vec;
    public List<T2> count;
}

// 存在二级分组时添加该类（类名与Excel二级分组名一致）
[System.Serializable]
public class T2
{
    //公开 字段类型 字段名（与Excel写的一致）
    public int value1;
    public string value2;
}

public enum TestType
{
    Type1,
    Type2
}