using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LinePrefab {
    public string name;
    public GameObject prefab;
    public PrefabAlignment alignment;

    [Tooltip("If aligned at the center this is the prefab index for the side prefabs.")]
    public int sidePrefabIndex;

    [Tooltip("original width of the prefab to be used when stretching")]
    public float width = 1; // Width of the prefab
    public Vector3 offsetRotation = new Vector3(0, 0, 0);
}

public enum PrefabAlignment{
    Stretch,
    Repeat,
    Center
}

[System.Serializable]
public class PointPrefab {
    public string name;
    public GameObject prefab;
}


[CreateAssetMenu(fileName = "NewAutoLevelPack", menuName = "AutoLevel/PrefabPack", order = 1)]
public class AutoLevelPrefabPack : ScriptableObject
{
    public float FloorHeight;

    [Header("Prefabs Configurations")]
    public List<LinePrefab> LinePrefabs;
    public List<PointPrefab> PointPrefabs;

}
