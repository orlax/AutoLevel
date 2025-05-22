using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

[Serializable]
public class AutoLevelAnchor
{
    public string name;
    public GameObject gameObject;
    public Vector3 offsetPosition; //offset from the vertex
    public Vector3 offsetRotation; //offset from the rotation of the level
    public int ID; //id anchor to be saved on the geometry
    public int vertex;//index of the vertex this object is attached to
}
