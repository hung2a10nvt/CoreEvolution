using UnityEngine;
using System.Collections.Generic;

public class PropRandomizer : MonoBehaviour
{
    [Header("Pools")]
    [SerializeField] private GameObject[] decorationProps; // Decor...
    [SerializeField] private GameObject[] hazardProps;     // Electronic traps, acid...

    [Header("Spawn Points")]
    [SerializeField] private List<Transform> spawnPoints;

    [Header("Settings")]
    [Range(0, 1)][SerializeField] private float spawnChance = 0.6f; 

    void Start()
    {
        SpawnProps();
    }

    void SpawnProps()
    {
        foreach (Transform point in spawnPoints)
        {
            if (Random.value < spawnChance)
            {
                // Or decors or traps
                GameObject[] selectedPool = (Random.value > 0.3f) ? decorationProps : hazardProps;

                if (selectedPool.Length > 0)
                {
                    GameObject randomProp = selectedPool[Random.Range(0, selectedPool.Length)];

                    GameObject spawnedProp = Instantiate(randomProp, point.position, Quaternion.identity);
                    spawnedProp.transform.SetParent(this.transform);
                }
            }
        }
    }
}