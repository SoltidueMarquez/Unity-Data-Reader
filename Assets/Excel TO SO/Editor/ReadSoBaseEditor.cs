using Excel_TO_SO.Scripts;
using UnityEditor;
using UnityEngine;

namespace Excel_TO_SO.Editor
{
    [CustomEditor (typeof(ReaderSoBase), true)]
    public class ReadSoBaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var so = (ReaderSoBase)target;
            
            if (GUILayout.Button ("Read Csv"))
            {
                so.ReadCsvData();
                EditorUtility.SetDirty(target);
            }
            
            if (GUILayout.Button ("Read Excel"))
            {
                so.ReadExcelData();
                EditorUtility.SetDirty(target);
            }
            
            if (GUILayout.Button ("Read Json"))
            {
                so.ReadJsonData();
                EditorUtility.SetDirty(target);
            }
        }
    }
}