using System.Collections.Generic;
using UnityEngine;

namespace Excel_TO_SO.Scripts.Excel
{
    public static class ExcelReadUtil
    {
        public static List<T> ReadExcelData<T>(TextAsset excelFile, int sheetIndex = 0) where T : IParseable, new()
        {
            var data = new List<T>();

            // TODO:待实现

            return data;
        }
        
    }
}