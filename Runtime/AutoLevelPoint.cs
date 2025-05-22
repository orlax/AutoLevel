using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/**

Primer intento de tener un prefab auto configurable, me di cuenta
de que tratar de configurar puntos individuales no es lo mejor
ya que hay muchos problemas de rotaciones. 

talvez haya futuro aqui tratando de configurar custom prefabs en ciertas esquinas?

vamonos con puertas y ventanas a nivel de muros por el momento.

*/
[SelectionBase]
public class AutoLevelPoint : MonoBehaviour
{
    public AutoLevelPrefabPack prefabs;

    public int vertexIndex;

    public int TypeIndex_ = 1;
    public int TypeIndex{
        get{return TypeIndex_;}
        set{
            TypeIndex_ = value;
            GenerateGeometry();
            WriteToMesh();
        }
    }

    AutoLevel autoLevel;

    public void Setup(Vector3 position, int index, AutoLevel autoLevel_){
        autoLevel = autoLevel_;
        prefabs = autoLevel.prefabs;
        vertexIndex = index;
        transform.position = position;
        TypeIndex_ = (int)autoLevel.colors[index].g;
        GenerateGeometry();        
    }

    public void GenerateGeometry(){
        //delete all children
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }

        if(TypeIndex_ == 0){
            return;
        }

        GameObject prefab = prefabs.PointPrefabs[TypeIndex_].prefab;

        #if UNITY_EDITOR
        GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        go.transform.position = transform.position;
        go.transform.parent = transform;
        go.transform.rotation = Quaternion.identity;
        go.transform.parent = transform;
        #endif
    }

    public void WriteToMesh(){
        autoLevel?.SavePointType(vertexIndex,TypeIndex);
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.4f);
    }
}
