using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Csv_To_So
{
    public static class SoGenerator
    {
        static List<string> childListMark = new List<string>();
        static List<int> childListFlag = new List<int>();

        /// <summary>
        /// 查找子列表在数组中的索引位置
        /// </summary>
        /// <param name="className"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private static int FindArrayIndex(string[][] array, string className, string fieldName)
        {
            // 遍历所有标记的子列表
            for (var i = 0; i < childListMark.Count; i++)
            {
                if (childListMark[i] == className) // 匹配类名和字段名
                {
                    var index = childListFlag[i];
                    if (array[2][index] == fieldName)
                    {
                        return index;
                    }
                }
            }

            return -1; // 未找到返回-1
        }

        /// <summary>
        /// 核心方法：生成ScriptableObject资产
        /// </summary>
        /// <param name="array"></param>
        /// <param name="dataName"></param>
        /// <param name="saveAssetPath"></param>
        public static void GenerateAsset(string[][] array, string dataName, string saveAssetPath)
        {
            // 加载程序集、获取文件输入
            var assembly = System.Reflection.Assembly.Load("Assembly-CSharp");
            var id = 0;

            // 校验输入有效性
            if (string.IsNullOrEmpty(dataName)) return;

            // 开始转换
            string className = dataName; // 去除后缀

            // 反射创建ScriptableObject实例
            var dataType = assembly.GetType(className);
            var dataObj = ScriptableObject.CreateInstance(dataType);


            // array[0]第一行，中文说明（略过）
            // array[1]第二行，是否有二级分组
            // array[2]第三行，字段名
            // array[3]第四行，类型
            // array[4]第五行，数据（可读部分）

            for (var m = 1; m < array.Length - 1; m++) // 遍历数据行（从第1行开始）
            {
                if (array[m] != null && m == 1) // 检测第二行里作为嵌套list的字段信息
                {
                    for (var n = 0; n < array[m].Length; n++)
                    {
                        if (array[m][n] != "") // 记录子列表的列索引和类名
                        {
                            childListFlag.Add(n);
                            childListMark.Add(array[m][n]);
                        }

                        // Debug.Log(array[m][n]);
                    }
                }

                if (m <= 3) continue; // 跳过前4行元数据

                var classType = assembly.GetType(className);
                var classObj = Activator.CreateInstance(classType);
                var fiInfo = classType.GetFields();
                
                // 根据首列是否有值来判断存储与数据读取
                var firstColumn = Regex.IsMatch(array[m][0], @"^\d+$"); // 正则表达式，检查当前行的第一列是否全为数字
                if (firstColumn)
                {
                    id = CustomCsvReader.StrToInt(array[m][0]);
                    // 先处理外层的数据，填充外层字段
                    for (var i = 0; i < fiInfo.Length; i++)
                    {
                        var strs = fiInfo[i].ToString().Split(' ');
                        CustomCsvReader.IdentifyDataParameters(assembly, m, i, classObj, fiInfo, i, strs);
                    }

                    // 获取dataType里AddList的方法
                    MethodInfo methodInfo = dataType.GetMethod("AddList"); 
                    object[] parameters = new object[] { id, classObj };
                    methodInfo.Invoke(dataObj, parameters);
                    
                    // 再处理内层的数据
                    DealInnerData(classType, classObj, fiInfo, assembly, array, m, id, dataType, dataObj);
                }
                else // 不存在外层数据，只考虑内层的数据
                {
                    DealInnerData(classType, classObj, fiInfo, assembly, array, m, id, dataType, dataObj);
                }
            }

            // 创建SO并刷新
            AssetDatabase.CreateAsset((UnityEngine.Object)dataObj, saveAssetPath + dataName + ".asset");
            AssetDatabase.Refresh();
            Debug.Log($"生成成功：{dataName}.asset");
        }

        
        
        /// <summary>
        /// 处理内层的数据
        /// </summary>
        private static void DealInnerData(Type classType, object classObj, FieldInfo[] fiInfo, Assembly assembly,
            string[][] array, int m, int id, Type dataType, ScriptableObject dataObj)
        {
            // 再做内层的数据
            foreach (var fieldInfo in fiInfo)
            {
                var strings = fieldInfo.ToString().Split(' ');
                if (!strings[0].Contains("List")) // 不存在List找下一个
                {
                    continue;
                }
                else
                {
                    // 存在后需要找到List的标记位
                    var startStr = strings[0].LastIndexOf("[");
                    var endStr = strings[0].LastIndexOf("]");
                    var childClassName = strings[0].Substring(startStr + 1, endStr - startStr - 1);
                    var childClassType = assembly.GetType(childClassName);
                    var childClassObj = Activator.CreateInstance(childClassType);
                    var childFiInfo = childClassType.GetFields();
                    for (var j = 0; j < childFiInfo.Length; j++)
                    {
                        var childStrings = childFiInfo[j].ToString().Split(' ');
                        var childName = childStrings[1];
                        var index = FindArrayIndex(array, childClassName, childName);
                        if (index == -1)
                        {
                            Debug.LogWarning("数据读取异常，是关于 " + childName);
                            continue;
                        }

                        CustomCsvReader.IdentifyDataParameters(assembly, m, index, childClassObj,
                            childFiInfo, j, childStrings);
                    }

                    var childMethodInfo = dataType.GetMethod("SecondAddList");
                    var childParameters = new object[] { id, childClassObj };
                    childMethodInfo.Invoke(dataObj, childParameters);
                }
            }
        }
    }
}