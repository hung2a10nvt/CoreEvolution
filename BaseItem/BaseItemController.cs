using UnityEngine;

public abstract class BaseItemController : MonoBehaviour
{
    public int currentLevel = 1;

    public abstract string GetID();
    public abstract int GetMaxLevel();
    public abstract void LevelUp();
}