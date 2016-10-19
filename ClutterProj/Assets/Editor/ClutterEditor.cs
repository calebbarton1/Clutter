﻿// Copyright (C) 2016 Caleb Barton (caleb.barton@hotmail.com)
//github.com/calebbarton1
//Released under MIT License
//https://opensource.org/licenses/MIT

using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class ClutterMenu : MonoBehaviour
{
    static private GameObject node;

    [MenuItem("ClutterBug/Create3DNode")]
    static public void Create3DNode()
    {
        //gets prefab path
        node = AssetDatabase.LoadAssetAtPath("Assets/ClutterBug/ClutterNode.prefab", typeof(GameObject)) as GameObject;
        Object clone = Instantiate(node, Vector3.zero, Quaternion.identity);
        //removes the (clone) in name
        clone.name = node.name;
    }
    [MenuItem("ClutterBug/Create2DNode")]

    static public void Create2DNode()
    {
        //gets prefab path
        node = AssetDatabase.LoadAssetAtPath("Assets/ClutterBug/ClutterNode2D.prefab", typeof(GameObject)) as GameObject;
        Object clone = Instantiate(node, Vector2.zero, Quaternion.identity);
        //removes the (clone) in name
        clone.name = node.name;
    }
}


[CustomEditor(typeof(Node))]
public class NodeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        Node nodeScript = (Node)target;

        EditorGUILayout.Separator();

        //buttons
        if (GUILayout.Button("Spawn Clutter"))
            nodeScript.SpawnObjectsInArea();

        EditorGUILayout.Separator();

        if (GUILayout.Button("Delete Clutter"))
            nodeScript.DeleteClutter();

        EditorGUILayout.Separator();


        nodeScript.debug = EditorGUILayout.Toggle("Enable Debugging", nodeScript.debug);//debug bool
        nodeScript.shape = (Node.colliderMenu)EditorGUILayout.EnumPopup("Shape of Clutter Node", nodeScript.shape);//shape enum
        nodeScript.clutterMask = LayerMaskField("Clutter Mask", nodeScript.clutterMask);//layermask list
        nodeScript.numberToSpawn = EditorGUILayout.IntField("Number of Clutter", nodeScript.numberToSpawn);

        EditorGUILayout.Separator();

        //draw list
        var property = serializedObject.FindProperty("prefabList");
        serializedObject.Update();
        EditorGUILayout.PropertyField(property, true);
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Separator();

        nodeScript.allowOverlap = EditorGUILayout.Toggle("Enable Clutter Overlap", nodeScript.allowOverlap);
        nodeScript.additive = EditorGUILayout.Toggle("Enable Additive Clutter", nodeScript.additive);
        nodeScript.faceNormal = EditorGUILayout.Toggle("Rotate to Surface Normal", nodeScript.faceNormal);
        nodeScript.angleLimit = EditorGUILayout.Slider("Surface Angle Limit", nodeScript.angleLimit, 0, 89);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        if (nodeScript.rotationOverride == Vector3.zero)
        {
            EditorGUILayout.LabelField("Random Rotation");

            //Showing the vector2 values as min/max
            //X
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("X Rotation");
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.rotX.x = EditorGUILayout.FloatField(nodeScript.rotX.x);
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.rotX.y = EditorGUILayout.FloatField(nodeScript.rotX.y);
            EditorGUILayout.EndHorizontal();

            //Y
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Y Rotation");
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.rotY.x = EditorGUILayout.FloatField(nodeScript.rotY.x);
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.rotY.y = EditorGUILayout.FloatField(nodeScript.rotY.y);
            EditorGUILayout.EndHorizontal();

            //Z
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Z Rotation");
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.rotZ.x = EditorGUILayout.FloatField(nodeScript.rotZ.x);
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.rotZ.y = EditorGUILayout.FloatField(nodeScript.rotZ.y);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
        }

        nodeScript.rotationOverride = EditorGUILayout.Vector3Field("Rotation Override", nodeScript.rotationOverride);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        if (nodeScript.scaleOverride == Vector3.zero)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Random Scale");
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.randomScale.x = EditorGUILayout.FloatField(nodeScript.randomScale.x);
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.randomScale.y = EditorGUILayout.FloatField(nodeScript.randomScale.y);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
        }
        nodeScript.scaleOverride = EditorGUILayout.Vector3Field("Scale Override", nodeScript.scaleOverride);
    }


    //Getting the Layers to draw into the inspector
    static LayerMask LayerMaskField(string label, LayerMask layerMask)
    {
        List<int> layerNumbers = new List<int>();

        var layers = InternalEditorUtility.layers;

        layerNumbers.Clear();

        for (int i = 0; i < layers.Length; i++)
            layerNumbers.Add(LayerMask.NameToLayer(layers[i]));

        int maskWithoutEmpty = 0;
        for (int i = 0; i < layerNumbers.Count; i++)
        {
            if (((1 << layerNumbers[i]) & layerMask.value) > 0)
                maskWithoutEmpty |= (1 << i);
        }

        maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers);

        int mask = 0;
        for (int i = 0; i < layerNumbers.Count; i++)
        {
            if ((maskWithoutEmpty & (1 << i)) > 0)
                mask |= (1 << layerNumbers[i]);
        }
        layerMask.value = mask;

        return layerMask;
    }
}

[CustomEditor(typeof(NodeChild))]
public class NodeChildInspector : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        NodeChild nodeScript = (NodeChild)target;

        nodeScript.debug = EditorGUILayout.Toggle("Enable Debugging", nodeScript.debug);//debug bool
        nodeScript.clutterMask = LayerMaskField("Clutter Mask", nodeScript.clutterMask);//layermask list
        nodeScript.numberToSpawn = EditorGUILayout.IntField("Number of Clutter", nodeScript.numberToSpawn);
        nodeScript.distance = EditorGUILayout.FloatField("Distance from Parent", nodeScript.distance);

        EditorGUILayout.Separator();

        //draw list
        var property = serializedObject.FindProperty("prefabList");
        serializedObject.Update();
        EditorGUILayout.PropertyField(property, true);
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Separator();

        nodeScript.allowOverlap = EditorGUILayout.Toggle("Enable Clutter Overlap", nodeScript.allowOverlap);
        nodeScript.faceNormal = EditorGUILayout.Toggle("Rotate to Surface Normal", nodeScript.faceNormal);
        nodeScript.angleLimit = EditorGUILayout.Slider("Surface Angle Limit", nodeScript.angleLimit, 0, 89);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        if (nodeScript.rotationOverride == Vector3.zero)
        {
            EditorGUILayout.LabelField("Random Rotation");

            //Showing the vector2 values as min/max
            //X
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("X Rotation");
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.rotX.x = EditorGUILayout.FloatField(nodeScript.rotX.x);
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.rotX.y = EditorGUILayout.FloatField(nodeScript.rotX.y);
            EditorGUILayout.EndHorizontal();

            //Y
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Y Rotation");
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.rotY.x = EditorGUILayout.FloatField(nodeScript.rotY.x);
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.rotY.y = EditorGUILayout.FloatField(nodeScript.rotY.y);
            EditorGUILayout.EndHorizontal();

            //Z
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Z Rotation");
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.rotZ.x = EditorGUILayout.FloatField(nodeScript.rotZ.x);
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.rotZ.y = EditorGUILayout.FloatField(nodeScript.rotZ.y);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
        }

        nodeScript.rotationOverride = EditorGUILayout.Vector3Field("Rotation Override", nodeScript.rotationOverride);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        if (nodeScript.scaleOverride == Vector3.zero)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Random Scale");
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.randomScale.x = EditorGUILayout.FloatField(nodeScript.randomScale.x);
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.randomScale.y = EditorGUILayout.FloatField(nodeScript.randomScale.y);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
        }
        nodeScript.scaleOverride = EditorGUILayout.Vector3Field("Scale Override", nodeScript.scaleOverride);
    }


    //Getting the Layers to draw into the inspector
    static LayerMask LayerMaskField(string label, LayerMask layerMask)
    {
        List<int> layerNumbers = new List<int>();

        var layers = InternalEditorUtility.layers;

        layerNumbers.Clear();

        for (int i = 0; i < layers.Length; i++)
            layerNumbers.Add(LayerMask.NameToLayer(layers[i]));

        int maskWithoutEmpty = 0;
        for (int i = 0; i < layerNumbers.Count; i++)
        {
            if (((1 << layerNumbers[i]) & layerMask.value) > 0)
                maskWithoutEmpty |= (1 << i);
        }

        maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers);

        int mask = 0;
        for (int i = 0; i < layerNumbers.Count; i++)
        {
            if ((maskWithoutEmpty & (1 << i)) > 0)
                mask |= (1 << layerNumbers[i]);
        }
        layerMask.value = mask;

        return layerMask;
    }
}

[CustomEditor(typeof(Node2D))]
public class Node2DInspector : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        Node2D nodeScript = (Node2D)target;

        EditorGUILayout.Separator();

        //buttons
        if (GUILayout.Button("Spawn Clutter"))
            nodeScript.SpawnObjectsInArea();

        EditorGUILayout.Separator();

        if (GUILayout.Button("Delete Clutter"))
            nodeScript.DeleteClutter();

        EditorGUILayout.Separator();


        nodeScript.debug = EditorGUILayout.Toggle("Enable Debugging", nodeScript.debug);//debug bool
        nodeScript.shape = (Node2D.colliderMenu)EditorGUILayout.EnumPopup("Shape of Clutter Node", nodeScript.shape);//shape enum
        nodeScript.clutterMask = LayerMaskField("Clutter Mask", nodeScript.clutterMask);//layermask list
        nodeScript.numberToSpawn = EditorGUILayout.IntField("Number of Clutter", nodeScript.numberToSpawn);

        EditorGUILayout.Separator();

        //draw list
        var property = serializedObject.FindProperty("prefabList");
        serializedObject.Update();
        EditorGUILayout.PropertyField(property, true);
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Separator();

        nodeScript.allowOverlap = EditorGUILayout.Toggle("Enable Clutter Overlap", nodeScript.allowOverlap);
        nodeScript.additive = EditorGUILayout.Toggle("Enable Additive Clutter", nodeScript.additive);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        if (nodeScript.orderLayerOverride == 0)
        {
            EditorGUILayout.LabelField("Random Order in Layer");

            //Showing the vector2 values as min/max
            //X
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.randomOrderLayer.x = EditorGUILayout.FloatField(nodeScript.randomOrderLayer.x);
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.randomOrderLayer.y = EditorGUILayout.FloatField(nodeScript.randomOrderLayer.y);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
        }

        nodeScript.orderLayerOverride = (int)EditorGUILayout.FloatField("Order Layer", nodeScript.orderLayerOverride);


        EditorGUILayout.Separator();
        EditorGUILayout.Separator();


        if (nodeScript.positionOverride == Vector2.zero)
        {
            EditorGUILayout.LabelField("Random Position");

            //Showing the vector2 values as min/max
            //X
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("X Position");
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.posX.x = EditorGUILayout.FloatField(nodeScript.posX.x);
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.posX.y = EditorGUILayout.FloatField(nodeScript.posX.y);
            EditorGUILayout.EndHorizontal();

            //Y
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Y Position");
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.posY.x = EditorGUILayout.FloatField(nodeScript.posY.x);
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.posY.y = EditorGUILayout.FloatField(nodeScript.posY.y);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Override Position");
        EditorGUILayout.LabelField("X", GUILayout.Width(30));
        nodeScript.positionOverride.x = EditorGUILayout.FloatField(nodeScript.positionOverride.x);
        EditorGUILayout.LabelField("Y", GUILayout.Width(30));
        nodeScript.positionOverride.y = EditorGUILayout.FloatField(nodeScript.positionOverride.y);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();


        if (nodeScript.rotationOverride == 0)
        {
            //Showing the vector2 values as min/max
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Random Rotation");
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.rotZ.x = EditorGUILayout.FloatField(nodeScript.rotZ.x);
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.rotZ.y = EditorGUILayout.FloatField(nodeScript.rotZ.y);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
        }

        nodeScript.rotationOverride = EditorGUILayout.FloatField("Rotation Override", nodeScript.rotationOverride);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        if (nodeScript.scaleOverride == Vector2.zero)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Random Scale");
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.randomScale.x = EditorGUILayout.FloatField(nodeScript.randomScale.x);
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.randomScale.y = EditorGUILayout.FloatField(nodeScript.randomScale.y);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
        }
        nodeScript.scaleOverride = EditorGUILayout.Vector3Field("Scale Override", nodeScript.scaleOverride);
    }


    //Getting the Layers to draw into the inspector
    static LayerMask LayerMaskField(string label, LayerMask layerMask)
    {
        List<int> layerNumbers = new List<int>();

        var layers = InternalEditorUtility.layers;

        layerNumbers.Clear();

        for (int i = 0; i < layers.Length; i++)
            layerNumbers.Add(LayerMask.NameToLayer(layers[i]));

        int maskWithoutEmpty = 0;
        for (int i = 0; i < layerNumbers.Count; i++)
        {
            if (((1 << layerNumbers[i]) & layerMask.value) > 0)
                maskWithoutEmpty |= (1 << i);
        }

        maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers);

        int mask = 0;
        for (int i = 0; i < layerNumbers.Count; i++)
        {
            if ((maskWithoutEmpty & (1 << i)) > 0)
                mask |= (1 << layerNumbers[i]);
        }
        layerMask.value = mask;

        return layerMask;
    }
}