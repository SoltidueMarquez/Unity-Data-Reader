using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Csv_To_So
{
    public static class CustomCsvReader
    {
        public static string[][] array = null;

        #region 数据读取层
        public static void ReadCsvData(TextAsset csvFile) // 读取Csv文件数据，转化为二维字符串数组array
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
        #endregion

        #region 数据映射层
        public static int StrToInt(string str)
        {
            var result = int.Parse(str is "" or null ? "0" : str);
            return result;
        }

        private static long StrToLong(string str)
        {
            var result = long.Parse(str is "" or null ? "0" : str);
            return result;
        }

        private static float StrToSingleFloat(string str)
        {
            var result = float.Parse(str is "" or null ? "0" : str);
            return result;
        }

        private static bool StrToBool(string str)
        {
            var result = bool.Parse(str is "" or null ? "false" : str);
            return result;
        }
        #endregion


        #region 数据解析层

        // 定义统一的转换器委托签名
        private delegate object FieldValueConverter(string rawValue, Assembly assembly, string typeMetadata);

        // 在类中创建转换器字典
        private static readonly Dictionary<string, FieldValueConverter> s_Converters =
            new Dictionary<string, FieldValueConverter>(StringComparer.OrdinalIgnoreCase)
            {
                // 基础类型
                ["System.Int32"] = HandleInt,
                ["System.Int32[]"] = HandleIntArray,
                ["System.Int64"] = HandleLong,
                ["System.Int64[]"] = HandleLongArray,
                ["System.Single"] = HandleFloat,
                ["System.Single[]"] = HandleFloatArray,
                ["System.String"] = HandleString,
                ["System.String[]"] = HandleStringArray,
                ["System.Boolean"] = HandleBoolean,
                ["System.Boolean[]"] = HandleBooleanArray,

                // Unity特殊类型
                ["Vector2Int"] = HandleVector2Int,
                ["Vector2"] = HandleVector2,
                ["Vector3Int"] = HandleVector3Int,
                ["Vector3"] = HandleVector3,

                // 枚举类型
                ["enum"] = HandleEnum
            };

        #region 基础类型处理器
        // 处理Int32类型，空值处理为0，否则转换为int
        private static object HandleInt(string value, Assembly _, string __)
            => StrToInt(value);

        // 处理Int32数组类型
        private static object HandleIntArray(string value, Assembly _, string __)
            => value.Split('|').Select(StrToInt).ToArray();

        // 处理Int64类型（长整型）空值处理为0，否则转换为long
        private static object HandleLong(string value, Assembly _, string __)
            => StrToLong(value);

        // 处理Int64数组类型
        private static object HandleLongArray(string value, Assembly _, string __)
            => value.Split('|').Select(StrToLong).ToArray();

        // 处理Single类型（单精度浮点数）
        private static object HandleFloat(string value, Assembly _, string __)
            => StrToSingleFloat(value);

        // 处理Single数组类型
        private static object HandleFloatArray(string value, Assembly _, string __)
            => value.Split('|').Select(StrToSingleFloat).ToArray();

        // 处理String类型（直接赋值）
        private static object HandleString(string value, Assembly _, string __)
            => value;

        // 处理String数组类型
        private static object HandleStringArray(string value, Assembly _, string __)
            => value.Split('|').ToArray();

        // 处理Boolean类型（直接解析布尔值）
        private static object HandleBoolean(string value, Assembly _, string __)
            => StrToBool(value);

        // 处理Boolean类型数组
        private static object HandleBooleanArray(string value, Assembly _, string __)
            => value.Split('|').Select(StrToBool).ToArray();
        #endregion

        #region Unity类型处理器
        // Unity Vector2Int类型处理，为空默认返回(0，0，0)
        private static object HandleVector2Int(string value, Assembly _, string __)
        {
            if (string.IsNullOrEmpty(value)) return Vector2Int.zero;
            var parts = value.Split('|');
            return new Vector2Int(StrToInt(parts[0]), StrToInt(parts[1]));
        }

        // Unity Vector2类型处理
        private static object HandleVector2(string value, Assembly _, string __)
        {
            if (string.IsNullOrEmpty(value)) return Vector2.zero;
            var parts = value.Split('|');
            return new Vector2(StrToSingleFloat(parts[0]), StrToSingleFloat(parts[1]));
        }

        // Unity Vector3Int类型处理
        private static object HandleVector3Int(string value, Assembly _, string __)
        {
            if (string.IsNullOrEmpty(value)) return Vector3Int.zero;
            var parts = value.Split('|');
            return new Vector3Int(StrToInt(parts[0]), StrToInt(parts[1]), StrToInt(parts[2]));
        }

        // Unity Vector3Int类型处理
        private static object HandleVector3(string value, Assembly _, string __)
        {
            if (string.IsNullOrEmpty(value)) return Vector3.zero;
            var parts = value.Split('|');
            return new Vector3(StrToSingleFloat(parts[0]), StrToSingleFloat(parts[1]), StrToSingleFloat(parts[2]));
        }
        #endregion

        #region 枚举
        private static object HandleEnum(string value, Assembly assembly, string typeMetadata)
        {
            // typeMetadata[0] 对应 array[2][n]（枚举类型名称）
            // typeMetadata[1] 对应 array[3][n]（字段类型标记）
            var enumType = assembly.GetType(typeMetadata);
            return Enum.Parse(enumType, value.Replace("|", ","));
        }
        #endregion

        public static void IdentifyDataParameters(Assembly assembly, int m, int n, object classObj, FieldInfo[] fiInfo,
            int fiI, IReadOnlyList<string> strings)
        {
            // 非空判断
            if (fiInfo == null) { throw new ArgumentNullException($"访问不到对应字段{nameof(fiInfo)}"); }
            if (array == null) { throw new ArgumentNullException($"文件{nameof(array)}内容为空"); }

            // 参数校验...
            var rawValue = array[m][n];
            var fieldTypeKey = strings[0];// 根据字段类型进行数据转换和赋值
            var typeMetadata = array[2][n]; // 组装元数据

            // 核心转换逻辑
            if (s_Converters.TryGetValue(fieldTypeKey, out var converter))
            {
                var convertedValue = converter(rawValue, assembly, typeMetadata);
                fiInfo[fiI].SetValue(classObj, convertedValue);
            }
            else // 处理未注册类型的备选方案
            {
                if (s_Converters.TryGetValue(array[3][n], out var otherConverter))
                {
                    // 非基本类型，如枚举
                    var convertedValue = otherConverter(rawValue, assembly, typeMetadata);
                    fiInfo[fiI].SetValue(classObj, convertedValue);
                }
                else
                {
                    HandleFallbackTypes(rawValue, assembly, fiInfo[fiI], classObj, typeMetadata);
                }
            }
        }
        
        // 备用的复杂类型处理
        private static void HandleFallbackTypes(string rawValue, Assembly assembly, FieldInfo field, object target,
            string typeMetadata)
        {
            // 这里可以处理其他特殊逻辑或抛出明确异常
            Debug.LogError($"可能无法被处理的类别，如果是List<T>1请忽略{field.FieldType}");
        }
        #endregion
    }
}