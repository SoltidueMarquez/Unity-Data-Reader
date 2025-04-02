using Scenes.Excel_TO_SO.Scripts;
using UnityEditor;
using UnityEngine;

namespace Scenes.Excel_TO_SO.Editor
{
    [CustomEditor (typeof(CsvReaderSoBase), true)]
    public class CsvReadSoBaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var so = (CsvReaderSoBase)target;
            
            if (GUILayout.Button ("Read"))
            {
                so.ReadCsvData();
                EditorUtility.SetDirty(target);
            }
        }
    }
}