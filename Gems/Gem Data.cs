using UnityEngine;

[CreateAssetMenu(fileName = "NewGemData", menuName = "ScriptableObject/Loot/GemData")]
public class GemData : ScriptableObject
{
    [field: SerializeField] public int ExpValue { get; private set; }
    [field: SerializeField] public Sprite GemSprite { get; private set; }
}