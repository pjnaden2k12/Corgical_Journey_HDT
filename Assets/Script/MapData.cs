using UnityEditor;
using UnityEngine;

[System.Serializable]
public class BoxData
{
    
    public BoxType boxType;
    public Vector3 position;
    public float rotationY;         
    public bool isPlayerSpawn;      
}


public enum BoxType
{
    normal,
    tree,
    stick,
    win
}

[CreateAssetMenu(fileName = "MapData", menuName = "ScriptableObjects/MapData")]
public class MapData : ScriptableObject
{
    public BoxData[] boxList;
}
#if UNITY_EDITOR
[CustomEditor(typeof(MapData))]
public class MapDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty boxList = serializedObject.FindProperty("boxList");

        for (int i = 0; i < boxList.arraySize; i++)
        {
            SerializedProperty box = boxList.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"Box {i + 1}", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(box.FindPropertyRelative("boxType"));
            EditorGUILayout.PropertyField(box.FindPropertyRelative("position"));
            SerializedProperty isPlayerSpawnProp = box.FindPropertyRelative("isPlayerSpawn");
            EditorGUILayout.PropertyField(isPlayerSpawnProp);

            if (isPlayerSpawnProp.boolValue)
            {
                EditorGUILayout.PropertyField(box.FindPropertyRelative("rotationY"), new GUIContent("Rotation Y"));
            }

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Add Box"))
        {
            boxList.InsertArrayElementAtIndex(boxList.arraySize);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif