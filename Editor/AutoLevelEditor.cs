using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using System.Linq;

[CustomEditor(typeof(AutoLevel))]
public class AutoLevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AutoLevel autoLevel = (AutoLevel)target;

        if (GUILayout.Button("🔨 Generate Level"))
        {
            autoLevel.GenerateLevel();
        }

        if (GUILayout.Button("❌ Clear Objects"))
        {
            autoLevel.DeleteAllFeatures();
        }

        if (GUILayout.Button("Reset Mesh Values"))
        {
            autoLevel.ResetMeshValues();
        }

        GUILayout.Label("Ignore or Show Vertices");
        if (GUILayout.Button("Ignore Vertices"))
        {
            autoLevel.WriteAlphaToSelected(0);
        }

        if (GUILayout.Button("ShowVertices"))
        {
            autoLevel.WriteAlphaToSelected(1);
        }

        GUILayout.Label("Anchor Placement");
        if (GUILayout.Button("Add Anchor"))
        {
            autoLevel.AddAnchor();
        }
        if (GUILayout.Button("Remove Anchor"))
        {
            autoLevel.RemoveAnchor();
        }
        if (GUILayout.Button("Update Anchor Offsets"))
        {
            autoLevel.updateAnchorOffsets();
        }

        if (GUILayout.Button("GENERATE ALL"))
        {
            AutoLevel[] allAutoLevels = FindObjectsOfType<AutoLevel>();
            foreach (AutoLevel al in allAutoLevels)
            {
                al.GenerateLevel();
            }
        }
    }
}

[CustomEditor(typeof(AutoLevelLine)), CanEditMultipleObjects]
public class AutoLevelLineEditor : Editor
{
    protected static bool expandDebugValues = false;

    
    public override void OnInspectorGUI()
    {
        AutoLevelLine autoLevelLine = (AutoLevelLine)target;

        EditorGUILayout.LabelField("Line Type");
        //generate list of options from autoLevelLine.prefabs.LinePrefabs name
        int TypeIndex = EditorGUILayout.Popup(autoLevelLine.TypeIndex, autoLevelLine.prefabs.LinePrefabs.Select(x => x.name).ToArray());
        if(TypeIndex != autoLevelLine.TypeIndex)
        {
            foreach (Object line in targets)
            {
                (line as AutoLevelLine).TypeIndex = TypeIndex;
            }
        }

        if(GUILayout.Button("Flip"))
        {
            autoLevelLine.Flip();
        }

        var prefabs = serializedObject.FindProperty("prefabs");
        EditorGUILayout.PropertyField(prefabs);

        var startPos = serializedObject.FindProperty("startPos");
        var endPos = serializedObject.FindProperty("endPos");
        var endistancedPos = serializedObject.FindProperty("distance");
        var direction = serializedObject.FindProperty("direction");
        var center = serializedObject.FindProperty("center");

        expandDebugValues = EditorGUILayout.Foldout(expandDebugValues, "Debug Values");
 
        if (expandDebugValues)
        {
            EditorGUILayout.PropertyField(startPos);
            EditorGUILayout.PropertyField(endPos);
            EditorGUILayout.PropertyField(endistancedPos);
            EditorGUILayout.PropertyField(direction);
            EditorGUILayout.PropertyField(center);
        }
    }

    [CustomEditor(typeof(AutoLevelPoint)), CanEditMultipleObjects]
    public class AutoLevelPointEditor: Editor{
        public override void OnInspectorGUI()
        {
            AutoLevelPoint autoLevelPoint = (AutoLevelPoint)target;

            EditorGUILayout.LabelField("Point Type");
            //generate list of options from autoLevelPoint.prefabs.PointPrefabs name
            int TypeIndex = EditorGUILayout.Popup(autoLevelPoint.TypeIndex, autoLevelPoint.prefabs.PointPrefabs.Select(x => x.name).ToArray());
            if(TypeIndex != autoLevelPoint.TypeIndex)
            {
                foreach (Object point in targets)
                {
                    (point as AutoLevelPoint).TypeIndex = TypeIndex;
                }
            }

            var prefabs = serializedObject.FindProperty("prefabs");
            EditorGUILayout.PropertyField(prefabs);

            var vertexIndex = serializedObject.FindProperty("vertexIndex");
            EditorGUILayout.PropertyField(vertexIndex);
        }
    }
}
