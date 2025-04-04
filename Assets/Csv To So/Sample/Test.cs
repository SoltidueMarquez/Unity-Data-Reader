using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Test
{
    //公开 字段类型 字段名（与Excel写的一致）
    public int id;
    public string name;
    public int[] desc;
    public bool icon;
    public Type1 Type;
    public Vector2 pos;

    public List<T> count;
}

// 存在二级分组时添加该类（类名与Excel二级分组名一致）
[System.Serializable]
public class T
{
    //公开 字段类型 字段名（与Excel写的一致）
    public int value1;
    public string value2;
}

public enum Type1
{
    t1,
    t2
}