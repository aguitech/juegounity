using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    public GameObject[] buildingPrefabs;
    public float spawnInterval = 4f;
    public float spawnDistance = 100f;
    public float despawnDistance = 30f;
    private float nextSpawnZ = 0f;

    void Start()
    {
        // Pre-popular escena con edificios
        for (int i = 0; i < 30; i++)
        {
            SpawnBuilding(transform.position.z + i * 6f);
        }
    }

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.player == null) return;

        float playerZ = GameManager.Instance.player.transform.position.z;

        // Spawn nuevos edificios adelante
        while (nextSpawnZ < playerZ + spawnDistance)
        {
            nextSpawnZ += Random.Range(spawnInterval * 0.5f, spawnInterval * 1.5f);
            SpawnBuilding(nextSpawnZ);
        }

        // Cleanup
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.position.z < playerZ - despawnDistance)
            {
                Destroy(child.gameObject);
            }
        }
    }

    void SpawnBuilding(float z)
    {
        if (buildingPrefabs.Length == 0) return;

        int side = Random.value > 0.5f ? 1 : -1;
        float x = side * Random.Range(8f, 18f);
        float y = Random.Range(0f, 3f);
        float scale = Random.Range(0.8f, 2.5f);

        GameObject prefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];
        GameObject building = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity);
        building.transform.localScale = new Vector3(scale, scale * Random.Range(2f, 5f), scale);
        building.transform.SetParent(transform);

        // Color neón aleatorio
        Renderer rend = building.GetComponent<Renderer>();
        if (rend != null && rend.material != null)
        {
            Color[] neonColors = {
                new Color(0f, 0.5f, 1f),
                new Color(0.5f, 0f, 1f),
                new Color(1f, 0f, 0.5f),
                new Color(0f, 1f, 0.5f)
            };
            Color c = neonColors[Random.Range(0, neonColors.Length)];
            c = Color.Lerp(c, Color.black, 0.6f);
            rend.material.color = c;
            rend.material.SetColor("_EmissionColor", c * 0.5f);
        }
    }
}