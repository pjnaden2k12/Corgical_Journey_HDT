using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    public MapData mapData;

    public GameObject blockNormalPrefab;
    public GameObject blockTreePrefab;
    public GameObject blockWinPrefab;
    public GameObject playerPrefab;

    private GameObject playerInstance;

    public void SpawnMap()
    {
        if (mapData == null) return;

        foreach (var box in mapData.boxList)
        {
            GameObject prefabToSpawn = null;

            switch (box.boxType)
            {
                case BoxType.normal:
                    prefabToSpawn = blockNormalPrefab;
                    break;
                case BoxType.tree:
                    prefabToSpawn = blockTreePrefab;
                    break;
                case BoxType.win:
                    prefabToSpawn = blockWinPrefab;
                    break;
            }

            Vector3 worldPosition = this.transform.position + box.position;
            Quaternion worldRotation = Quaternion.Euler(0f, box.rotationY, 0f);


            if (prefabToSpawn != null)
            {
                Instantiate(prefabToSpawn, worldPosition, Quaternion.identity, this.transform);
            }

            if (box.isPlayerSpawn && playerPrefab != null)
            {
                float blockHeight = 0f; 
                float playerHeight = playerPrefab.transform.localScale.y;

                Vector3 playerPosition = worldPosition + new Vector3(0, (blockHeight / 2f) + (playerHeight / 2f), 0);

                if (playerInstance != null)
                {
                    Destroy(playerInstance);
                }

                playerInstance = Instantiate(playerPrefab, playerPosition, worldRotation, this.transform);
            }
        }
    }

    public void ClearMap()
    {
        
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        playerInstance = null;
    }
}
