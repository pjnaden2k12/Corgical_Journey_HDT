using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    public MapData mapData;

    public GameObject boxAPrefab;
    public GameObject boxBPrefab;
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
                case BoxType.A:
                    prefabToSpawn = boxAPrefab;
                    break;
                case BoxType.B:
                    prefabToSpawn = boxBPrefab;
                    break;
            }

            Vector3 worldPosition = this.transform.position + box.position;
            Quaternion worldRotation = Quaternion.Euler(box.rotation);

            if (prefabToSpawn != null)
            {
                Instantiate(prefabToSpawn, worldPosition, Quaternion.identity, this.transform);
            }

            if (box.isPlayerSpawn && playerPrefab != null)
            {
                float blockHeight = 0f; // nếu bạn có collider/block cao khác nhau thì có thể lấy chiều cao từ đó
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
        // Xóa tất cả các con của MapSpawner (các box + player)
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        playerInstance = null;
    }
}
