using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Excel_TO_SO.Scripts
{
    // 非泛型基类（用于编辑器）
    public abstract class CsvReaderSoBase : ScriptableObject
    {
        public abstract void ReadCsvData();
    }
    
    /// <summary>
    /// 所有可以阅读Csv文件的SO泛型父类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CsvReaderSoBase<T> : CsvReaderSoBase  where T : IParseable, new()
    {
        public TextAsset csvFile;
        [Tooltip("数据列表")] public List<T> dataList = new List<T>();

        /// <summary>
        /// 根据Csv更新数据，为空则直接清空数据
        /// </summary>
        public override void ReadCsvData()
        {
            dataList.Clear();
            if (!csvFile) return;
            
            dataList = CsvReadUtil.ReadCsvData<T>(csvFile);
        }
    }
}
