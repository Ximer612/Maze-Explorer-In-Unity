using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MazeGenerator))]
public class MazeGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MazeGenerator myTarget = (MazeGenerator)target;

        if (GUILayout.Button("Generate New Maze"))
        {
            myTarget.SpawnNewMaze();
            EditorApplication.QueuePlayerLoopUpdate();
        }

        base.OnInspectorGUI();
    }
}
