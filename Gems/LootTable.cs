using UnityEngine;

[CreateAssetMenu(fileName = "NewLootTable", menuName = "ScriptableObject/Loot/LootTable")]
public class LootTable : ScriptableObject 
{
    [System.Serializable]
    public struct LootWeight
    {
        public GemData gemData;
        public float dropChance; // Chance to drop differents gem (of each type of enemies)
    }

    public LootWeight[] weights; 
}