using System;
using System.Collections.Generic;
using UnityEngine;

namespace Csv_To_So
{
    [CreateAssetMenu]
    public class CsvToSo : ScriptableObject
    {
        public string saveAssetPath = "Assets/Csv To So/Data/";
        public List<ReadableCsvFile> csvFiles;

        public void Convert()
        {
            foreach (var file in csvFiles)
            {
                if (file.Equals(null) || !file.csvFile || string.IsNullOrEmpty(file.soClassName))
                {
                    Debug.LogError(" ");
                    return;
                }
                
                var fileName = $"{file.csvFile.name}-{IdManager.GetUniqueID()}";
                
                CustomCsvReader.ReadCsvData(file.csvFile);
                var array = CustomCsvReader.array;

                var path = $"{saveAssetPath}{fileName}.asset";
                SoGenerator.GenerateAsset(array, file.soClassName, path);
            }
        }
    }
    

    [Serializable]
    public struct ReadableCsvFile
    {
        private sealed class CsvFileSoClassNameSoNameEqualityComparer : IEqualityComparer<ReadableCsvFile>
        {
            public bool Equals(ReadableCsvFile x, ReadableCsvFile y)
            {
                return Equals(x.csvFile, y.csvFile) && x.soClassName == y.soClassName;
            }

            public int GetHashCode(ReadableCsvFile obj)
            {
                return HashCode.Combine(obj.csvFile, obj.soClassName);
            }
        }

        public static IEqualityComparer<ReadableCsvFile> csvFileSoClassNameSoNameComparer { get; } = new CsvFileSoClassNameSoNameEqualityComparer();

        public TextAsset csvFile;
        public string soClassName;
    }
}