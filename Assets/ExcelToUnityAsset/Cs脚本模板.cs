using System.Collections.Generic;
using UnityEngine;

/*public class 一级类名+后缀s : ScriptableObject
{
    /// <summary>
    /// 根据ID获取list所在的index标记位
    /// </summary>
    public Dictionary<int, int> idStorageDic = new Dictionary<int, int>();
    public List<自定义一级类> 一级List = new List<自定义一级类>();

    public void AddList(int id, 自定义一级类 data)
    {
        data.XXX = new();  // 无二级分组忽略此行代码
        一级List.Add(data);
        idStorageDic.Add(id, 一级List.Count - 1);
    }

    // 无二级分组忽略此方法
    public void SecondAddList(int id, 二级分组类 secData)
    {
        一级List[idStorageDic[id]].XXX.Add(secData);
    }
}

[System.Serializable]
public class 自定义一级类（与csv同名）
{
	//公开 字段类型 字段名（与Excel写的一致）
    public int id;
    public List<二级分组类> XXX;
}

// 存在二级分组时添加该类（类名与Excel二级分组名一致）
[System.Serializable]
public class 二级分组类
{
    //公开 字段类型 字段名（与Excel写的一致）
    public int value;
}*/
