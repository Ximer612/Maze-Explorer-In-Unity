using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MazeGenerator))]
[RequireComponent(typeof(MazeCellsSpawner))]
public class GameManager : MonoBehaviour
{
    [SerializeField] int mazeWidth, mazeHeight;
    MazeGenerator generator;
    void Start()
    {
        generator = GetComponent<MazeGenerator>();
        generator.SetMazeWidthHeight(mazeWidth,mazeHeight);
        generator.SpawnNewMaze();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Debug Reset"))
        {
            generator.SpawnNewMaze();
        }
    }
}
