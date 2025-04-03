using System.Collections.Generic;
using Excel_TO_SO.Scripts.Csv;
using Excel_TO_SO.Scripts.Excel;
using UnityEngine;

namespace Excel_TO_SO.Scripts
{
    public abstract class ReaderSoBase: ScriptableObject
    {
        public abstract void ReadExcelData();
        public abstract void ReadCsvData();
        public abstract void ReadJsonData();
    }
    
    /// <summary>
    /// 所有可以阅读文件的SO泛型父类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReaderSoBase<T> : ReaderSoBase where T : IParseable, new()
    {
        public TextAsset file;
        [Tooltip("数据列表")] public List<T> dataList = new List<T>();
        
        [Tooltip("工作表索引（从0开始）")] public int sheetIndex = 0;

        /// <summary>
        /// 根据Csv更新数据，为空则直接清空数据
        /// </summary>
        public override void ReadExcelData()
        {
            dataList.Clear();
            if (!file) return;
            
            dataList = ExcelReadUtil.ReadExcelData<T>(file, sheetIndex);
        }

        /// <summary>
        /// 根据Excel更新数据，为空则直接清空数据
        /// </summary>
        public override void ReadCsvData()
        {
            dataList.Clear();
            if (!file) return;
            
            dataList = CsvReadUtil.ReadCsvData<T>(file);
        }

        /// <summary>
        /// 根据Json更新数据，为空则直接清空数据
        /// </summary>
        public override void ReadJsonData()
        {
            throw new System.NotImplementedException();
        }
    }
}