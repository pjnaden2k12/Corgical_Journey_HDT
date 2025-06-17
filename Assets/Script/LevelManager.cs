using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private MapSpawner mapSpawner;
    [SerializeField] private MapData[] levels;
    private int currentLevelIndex = 0;

    private void Start()
    {
        LoadLevel(currentLevelIndex);
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogWarning("Level index out of range");
            return;
        }

        currentLevelIndex = levelIndex;
        mapSpawner.ClearMap();
        mapSpawner.mapData = levels[levelIndex];
        mapSpawner.SpawnMap();
    }

    public void NextLevel()
    {
        int nextLevel = currentLevelIndex + 1;
        if (nextLevel >= levels.Length) nextLevel = 0; 
        LoadLevel(nextLevel);
    }
}
