using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Excel_TO_SO.Scripts
{
    /// <summary>
    /// 静态Csv阅读工具类
    /// </summary>
    public static class CsvReadUtil
    {
        public static List<T> ReadCsvData<T>(TextAsset csvFile) where T : IParseable, new()
        {
            var data = new List<T>();
            // 按换行符分割，自动移除为空的行，增加容错率
            var lines = csvFile.text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            for (var i = 1; i < lines.Length; i++) // 跳过标题行开始遍历
            {
                // 按逗号分割字段，移除为空的字段
                var fields = lines[i].Split(',', StringSplitOptions.RemoveEmptyEntries);
                // 调用每个类自己的解析方法
                var item = new T();
                item.ParseDataAndInit(fields);
                // 添加进列表
                data.Add(item);
            }

            return data;
        }
    }

}