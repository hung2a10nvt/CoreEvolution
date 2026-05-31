using UnityEngine;
using System.Collections.Generic;

public class MapController : MonoBehaviour
{
    [Header("Settings")]
    public GameObject[] chunkPrefabs; 
    public Transform player;        
    public float chunkSize = 20f;    
    public int renderDistance = 1;   

    private Vector2Int currentChunkCoord;
    private Dictionary<Vector2Int, GameObject> spawnedChunks = new Dictionary<Vector2Int, GameObject>();

    void Start()
    {
        if (player == null)
        {
            GameObject PlayerObj = GameObject.FindGameObjectWithTag("Player");
            if (PlayerObj != null)
            {
                player = PlayerObj.transform;
            }
            else
            {
                Debug.LogError("Cant find player!");
                return;
            }
        }

        int xCoord = Mathf.FloorToInt(player.position.x / chunkSize);
        int yCoord = Mathf.FloorToInt(player.position.y / chunkSize);
        currentChunkCoord = new Vector2Int(xCoord, yCoord);
        UpdateChunks();
    }

    void Update()
    {
        int xCoord = Mathf.FloorToInt(player.position.x / chunkSize);
        int yCoord = Mathf.FloorToInt(player.position.y / chunkSize);
        Vector2Int newChunkCoord = new Vector2Int(xCoord, yCoord);

        if (newChunkCoord != currentChunkCoord)
        {
            currentChunkCoord = newChunkCoord;
            UpdateChunks();
        }
    }

    void UpdateChunks()
    {
        List<Vector2Int> coordsToRemove = new List<Vector2Int>(spawnedChunks.Keys);

        for (int x = -renderDistance; x <= renderDistance; x++)
        {
            for (int y = -renderDistance; y <= renderDistance; y++)
            {
                Vector2Int targetCoord = currentChunkCoord + new Vector2Int(x, y);

                if (!spawnedChunks.ContainsKey(targetCoord))
                {
                    SpawnChunk(targetCoord);
                }

                coordsToRemove.Remove(targetCoord);
            }
        }

        foreach (Vector2Int coord in coordsToRemove)
        {
            Destroy(spawnedChunks[coord]);
            spawnedChunks.Remove(coord);
        }
    }

    void SpawnChunk(Vector2Int coord)
    {
        Vector3 spawnPos = new Vector3(coord.x * chunkSize, coord.y * chunkSize, 0);
        GameObject chunkPrefab = chunkPrefabs[Random.Range(0, chunkPrefabs.Length)];

        GameObject newChunk = Instantiate(chunkPrefab, spawnPos, Quaternion.identity);
        newChunk.transform.SetParent(this.transform);
        spawnedChunks.Add(coord, newChunk);
    }
}