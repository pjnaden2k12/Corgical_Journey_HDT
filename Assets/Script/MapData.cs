using UnityEngine;

[System.Serializable]
public class BoxData
{
    public int id;
    public BoxType boxType;
    public Vector3 position;
    public Vector3 rotation;         // Thêm hướng xoay (Euler angles)
    public bool isPlayerSpawn;       // Đánh dấu là vị trí spawn player
}


public enum BoxType
{
    A,
    B
}

[CreateAssetMenu(fileName = "MapData", menuName = "ScriptableObjects/MapData")]
public class MapData : ScriptableObject
{
    public BoxData[] boxList;
}
