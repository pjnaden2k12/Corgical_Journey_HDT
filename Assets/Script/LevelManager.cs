using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;


public class LevelManager : MonoBehaviour
{
    [SerializeField] private MapSpawner mapSpawner;
    [SerializeField] private MapData[] levels;
    [SerializeField] private GameObject water;
    [SerializeField] private Image fadeImage;  // kéo thả Image đen full màn hình vào inspector
    [SerializeField] private float fadeDuration = 1f;

    private int currentLevelIndex = 0;

    private void Start()
    {
        
        if (water != null)
            water.SetActive(false);
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogWarning("Level index out of range");

            if (water != null)
                water.SetActive(false);

            return;
        }

        currentLevelIndex = levelIndex;

        SaveLevel(currentLevelIndex);

        if (water != null)
            water.SetActive(true);

        mapSpawner.ClearMap();
        mapSpawner.mapData = levels[levelIndex];
        mapSpawner.SpawnMap();
    }

    public void NextLevel()
    {
        int nextLevel = currentLevelIndex + 1;
        if (nextLevel >= levels.Length)
        {
            Debug.Log("Đã hoàn thành tất cả màn chơi!");

            if (water != null)
                water.SetActive(false);

            return;
        }
        
        LoadLevel(nextLevel);
    }

    public void ResetProgress()
    {
        currentLevelIndex = 0;
        SaveLevel(currentLevelIndex);
    }

    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }
    public void FadeInAndLoadLevel(int levelIndex)
    {
        StartCoroutine(FadeInCoroutine(levelIndex));
    }

    private IEnumerator FadeInCoroutine(int levelIndex)
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.color = new Color(0, 0, 0, 0);
        yield return fadeImage.DOFade(1f, fadeDuration).WaitForCompletion();

        yield return new WaitForSeconds(0.3f);
     

        LoadLevel(levelIndex);

        fadeImage.color = new Color(0, 0, 0, 1);
        yield return fadeImage.DOFade(0f, fadeDuration).WaitForCompletion();

        fadeImage.gameObject.SetActive(false);
    }

    public void LevelComplete()
    {
        int savedLevel = PlayerPrefs.GetInt("CurrentLevel", 0);
        if (currentLevelIndex >= savedLevel)
        {
            PlayerPrefs.SetInt("CurrentLevel", currentLevelIndex + 1);
            PlayerPrefs.Save();
        }

        int nextLevel = currentLevelIndex + 1;

      
        FadeInAndLoadLevel(nextLevel);
    }
    public void SaveLevel(int levelIndex)
    {
        PlayerPrefs.SetInt("CurrentLevel", levelIndex);
        PlayerPrefs.Save();
    }
}
