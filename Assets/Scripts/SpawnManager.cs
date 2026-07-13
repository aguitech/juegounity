using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawning")]
    public GameObject[] obstaclePrefabs;
    public GameObject orbPrefab;
    public float spawnDistance = 80f;
    public float minObstacleGap = 8f;
    public float maxObstacleGap = 18f;
    public float despawnDistance = 20f;

    [Header("State")]
    public bool isSpawning = false;
    public float lastSpawnZ = 30f;

    [Header("Lanes")]
    public float[] lanePositions = { -3f, 0f, 3f };

    void Update()
    {
        if (!isSpawning) return;

        GameManager gm = GameManager.Instance;
        if (gm == null) return;

        // Spawn cuando el último está suficientemente lejos
        if (lastSpawnZ < gm.player.transform.position.z + spawnDistance)
        {
            SpawnRow();
        }

        // Cleanup
        CleanupOldObjects(gm.player.transform.position.z);
    }

    public void StartSpawning()
    {
        isSpawning = true;
        lastSpawnZ = GameManager.Instance.player.transform.position.z + 30f;

        // Spawn inicial de varias filas para que ya haya algo en pantalla
        for (int i = 0; i < 10; i++)
        {
            SpawnRow();
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    void SpawnRow()
    {
        GameManager gm = GameManager.Instance;
        float gap = Random.Range(minObstacleGap, maxObstacleGap);
        // Más gap a mayor velocidad para que sea jugable
        gap = Mathf.Max(gap, gm.speed * 1.2f);
        lastSpawnZ += gap;

        // Decide: ¿fila de orbes, fila de obstáculo, o ambas?
        float roll = Random.value;

        if (roll < 0.4f && obstaclePrefabs.Length > 0)
        {
            // Obstáculo en un lane random
            int lane = Random.Range(0, 3);
            SpawnObstacle(lane, lastSpawnZ);
        }
        else if (roll < 0.7f)
        {
            // Línea de orbes en un lane
            int lane = Random.Range(0, 3);
            SpawnOrbLine(lane, lastSpawnZ, 3 + Random.Range(0, 3));
        }
        else
        {
            // Obstáculo doble (dos lanes libres)
            int obstacleLane = Random.Range(0, 3);
            for (int i = 0; i < 3; i++)
            {
                if (i != obstacleLane)
                {
                    SpawnObstacle(i, lastSpawnZ, true);
                }
            }
        }
    }

    void SpawnObstacle(int lane, float z, bool isSecondary = false)
    {
        if (obstaclePrefabs.Length == 0) return;

        GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        Vector3 pos = new Vector3(lanePositions[lane], 1f, z);

        GameObject obstacle = Instantiate(prefab, pos, Quaternion.identity);
        obstacle.tag = "Obstacle";
        obstacle.transform.SetParent(transform);

        // Color aleatorio neón
        Renderer rend = obstacle.GetComponent<Renderer>();
        if (rend != null && rend.material != null)
        {
            Color[] neonColors = {
                new Color(1f, 0.2f, 0.6f), // magenta
                new Color(0f, 1f, 1f),     // cyan
                new Color(1f, 0.3f, 0f),   // naranja neón
                new Color(0.6f, 0.2f, 1f)  // púrpura
            };
            Color c = neonColors[Random.Range(0, neonColors.Length)];
            c = Color.Lerp(c, Color.white, 0.3f);
            rend.material.color = c;
            rend.material.SetColor("_EmissionColor", c * 2f);
        }
    }

    void SpawnOrbLine(int lane, float z, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(lanePositions[lane], 1.5f, z + i * 2.5f);
            GameObject orb = Instantiate(orbPrefab, pos, Quaternion.identity);
            orb.tag = "Orb";
            orb.transform.SetParent(transform);

            // Animación de flotación
            OrbFloat floatScript = orb.GetComponent<OrbFloat>();
            if (floatScript == null) floatScript = orb.AddComponent<OrbFloat>();
            floatScript.phase = i * 0.5f;
        }
    }

    void CleanupOldObjects(float playerZ)
    {
        // Destruir obstáculos/orbes que quedaron atrás
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.position.z < playerZ - despawnDistance)
            {
                Destroy(child.gameObject);
            }
        }
    }
}