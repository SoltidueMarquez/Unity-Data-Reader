using UnityEditor;
using UnityEngine;

namespace Csv_To_So.Editor
{
    [CustomEditor (typeof(CsvToSo), true)]
    public class CsvConverterEditor: UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var so = (CsvToSo)target;
            
            if (GUILayout.Button ("Read Csv"))
            {
                so.Convert();
            }
        }
    }
}