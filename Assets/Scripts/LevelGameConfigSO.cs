using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelGameConfig", menuName = "LevelConfiguration/LevelGameConfig", order = 1)]
public class LevelGameConfigSO : ScriptableObject
{
    public int mapID = 1;
    public int RoadsNumber = 1;
    public float tsunamiSpeed = 10f;
}