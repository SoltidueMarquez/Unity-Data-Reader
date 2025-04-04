using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace Csv_To_So
{
    public static class CustomCsvReader
    {
        public static string[][] array = null;
            
        /// <summary>
        /// 读取Csv文件数据，转化为二维字符串数组array
        /// </summary>
        /// <param name="csvFile"></param>
        public static void ReadCsvData(TextAsset csvFile)
        {
            // 读取每一行的内容，到切分换行为止，按换行符分割，自动移除为空的行，增加容错率
            var lineArray = csvFile.text.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
            // 创建二维数组
            array = new string[lineArray.Length][];

            // 把csv中的数据存储在二维数组中
            for (int i = 0; i < lineArray.Length; i++)
            {
                array[i] = lineArray[i].Split(',');
            }
        }

        #region 将字符串转化为对应类型
        public static int StrToInt(string str)
        {
            int result = int.Parse(str is "" or null ? "0" : str);
            return result;
        }

        public static long StrToLong(string str)
        {
            long result = long.Parse(str is "" or null ? "0" : str);
            return result;
        }

        public static float StrToSingleFloat(string str)
        {
            float result = float.Parse(str is "" or null ? "0" : str);
            return result;
        }

        /// <summary>
        /// 辨识数据的参数，转化为对应的字段信息
        /// </summary>
        /// <param name="assembly">程序集对象，用于反射获取类型（如枚举类型）</param>
        /// <param name="m">外层数组的索引（对应array的第一维索引）</param>
        /// <param name="n">内层数组的索引（对应array[m]的第二维索引）</param>
        /// <param name="classObj">目标对象实例，用于设置字段值</param>
        /// <param name="fiInfo">目标对象的所有字段信息集合</param>
        /// <param name="fiI">当前正在处理的字段索引（对应fiInfo数组）</param>
        /// <param name="strs">包含字段类型信息的字符串数组（strs[0]表示字段类型）</param>
        public static void IdentifyDataParameters(
            Assembly assembly, 
            int m, 
            int n, 
            object classObj,
            [NotNull] FieldInfo[] fiInfo,
            int fiI, 
            IReadOnlyList<string> strs)
        {
            if (fiInfo == null) throw new ArgumentNullException(nameof(fiInfo));
            if (array == null) throw new ArgumentNullException(nameof(array));
            // 根据字段类型进行数据转换和赋值
            switch (strs[0])
            {
                // 处理Int32类型
                case "System.Int32":
                    // 空值处理为0，否则转换为int
                    fiInfo[fiI].SetValue(classObj, int.Parse(array[m][n] is "" or null ? "0" : array[m][n]));
                    break;
                
                // 处理Int32数组类型
                case "System.Int32[]":
                    string[] aStr = array[m][n].Split("|");  // 用管道符分割数组元素
                    int[] aInt = Array.ConvertAll(aStr, StrToInt);  // 自定义字符串转int方法
                    fiInfo[fiI].SetValue(classObj, aInt);
                    break;
                
                // 处理Int64类型（长整型）
                case "System.Int64":
                    fiInfo[fiI].SetValue(classObj, long.Parse(array[m][n] is "" or null ? "0" : array[m][n]));
                    break;
                
                // 处理Int64数组类型
                case "System.Int64[]":
                    string[] aStr1 = array[m][n].Split("|");
                    long[] aLong = Array.ConvertAll(aStr1, StrToLong);
                    fiInfo[fiI].SetValue(classObj, aLong);
                    break;
                
                // 处理Single类型（单精度浮点数）
                case "System.Single":
                    fiInfo[fiI].SetValue(classObj, float.Parse(array[m][n] is "" or null ? "0" : array[m][n]));
                    break;
                
                // 处理Single数组类型
                case "System.Single[]":
                    string[] aStr2 = array[m][n].Split("|");
                    float[] aSingle = Array.ConvertAll(aStr2, StrToSingleFloat);
                    fiInfo[fiI].SetValue(classObj, aSingle);
                    break;
                
                // 处理String类型（直接赋值）
                case "System.String":
                    fiInfo[fiI].SetValue(classObj, array[m][n]);
                    break;
                
                // 处理String数组类型
                case "System.String[]":
                    string[] aStr3 = array[m][n].Split("|");
                    fiInfo[fiI].SetValue(classObj, aStr3);
                    break;
                
                // 处理Boolean类型（直接解析布尔值）
                case "System.Boolean":
                    fiInfo[fiI].SetValue(classObj, bool.Parse(array[m][n]));
                    break;

                // 处理自定义类型和复杂结构
                default:
                    // 枚举类型处理
                    if (array[3][n] == "enum")
                    {
                        Type t = assembly.GetType(array[2][n]);  // 通过反射获取枚举类型
                        string str = array[m][n].Replace("|", ",");  // 将枚举值转换为逗号分隔格式（处理多选枚举）
                        fiInfo[fiI].SetValue(classObj, Enum.Parse(t, str));  // 解析枚举值
                    }
                    // Unity Vector2Int类型处理
                    else if (array[3][n] == "Vector2Int")
                    {
                        if (array[m][n] != "")
                        {
                            string[] str = array[m][n].Split("|");
                            fiInfo[fiI].SetValue(classObj, 
                                new Vector2Int(int.Parse(str[0]), int.Parse(str[1])));
                        }
                        else
                            fiInfo[fiI].SetValue(classObj, new Vector2Int(0, 0));  // 空值默认(0,0)
                    }
                    // Unity Vector2类型处理
                    else if (array[3][n] == "Vector2")
                    {
                        if (array[m][n] != "")
                        {
                            string[] str = array[m][n].Split("|");
                            fiInfo[fiI].SetValue(classObj, 
                                new Vector2(float.Parse(str[0]), float.Parse(str[1])));
                        }
                        else
                            fiInfo[fiI].SetValue(classObj, new Vector2(0f, 0f));
                    }
                    // Unity Vector3Int类型处理
                    else if (array[3][n] == "Vector3Int")
                    {
                        if (array[m][n] != "")
                        {
                            string[] str = array[m][n].Split("|");
                            fiInfo[fiI].SetValue(classObj,
                                new Vector3Int(int.Parse(str[0]), int.Parse(str[1]), int.Parse(str[2])));
                        }
                        else
                            fiInfo[fiI].SetValue(classObj, new Vector3Int(0, 0, 0));
                    }
                    // Unity Vector3类型处理
                    else if (array[3][n] == "Vector3")
                    {
                        if (array[m][n] != "")
                        {
                            string[] str = array[m][n].Split("|");
                            fiInfo[fiI].SetValue(classObj,
                                new Vector3(float.Parse(str[0]), float.Parse(str[1]), float.Parse(str[2])));
                        }
                        else
                            fiInfo[fiI].SetValue(classObj, new Vector3(0f, 0f, 0f));
                    }
                    break;
            }
        }
        #endregion
    }
}