using UnityEngine;

namespace Csv_To_So
{
    [CreateAssetMenu]
    public class CsvToSo : ScriptableObject
    {
        public string saveAssetPath = "Assets/Csv To So/Data/";
        public TextAsset csvFile;
        
        public void Convert()
        {
            if (!csvFile) return;
            CustomCsvReader.ReadCsvData(csvFile);
            var array = CustomCsvReader.array;
            
            SoGenerator.GenerateAsset(array, csvFile.name, saveAssetPath);
        }
    }
}