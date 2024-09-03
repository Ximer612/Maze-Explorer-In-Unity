using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct MazeCell
{
    public int X;
    public int Y;
    public bool HasBeenVisited;
    public MazeCellType Type;
    public Transform MyWall;

    public bool Equals(MazeCell other)
    {
        if (X == other.X && Y == other.Y) return true;

        return false;
    }

    public static bool operator ==(MazeCell a, MazeCell b)
    {
        if (a.X == b.X && a.Y == b.Y) return true;
        else return false;
    }

    public static bool operator !=(MazeCell a, MazeCell b)
    {
        if (a.X != b.X || a.Y != b.Y) return true;
        else return false;
    }
}

public enum MazeCellType { Null, FirstCell, Floor, Void, Trap, Door, Key, LAST }
public static class MazeUtilities
{
    public static MazeCell FirstCell { get; private set; }
    static MazeUtilities()
    {

    }

    public static void NewFirstCell()
    {
        MazeGenerator.MazeCells[FirstCell.X,FirstCell.Y].Type = MazeCellType.Floor;
        FirstCell = GetRandomCell();
        MazeGenerator.MazeCells[FirstCell.X,FirstCell.Y].Type = MazeCellType.FirstCell;
        FirstCell = MazeGenerator.MazeCells[FirstCell.X,FirstCell.Y];
    }

    public static Vector2 PositionToMaze(Vector2 position)
    {
        return new Vector2(position.x * MazeGenerator.MazeScale * MazeGenerator.CellSpawnOffset.x, position.y * MazeGenerator.MazeScale * MazeGenerator.CellSpawnOffset.y);
    }

    public static Vector2 PositionToMaze(int x, int y)
    {
        return new Vector2(x * MazeGenerator.MazeScale * MazeGenerator.CellSpawnOffset.x, y * MazeGenerator.MazeScale * MazeGenerator.CellSpawnOffset.y);
    }

    public static Vector2 PositionToMaze(MazeCell cell)
    {
        return new Vector2(cell.X * MazeGenerator.MazeScale * MazeGenerator.CellSpawnOffset.x, cell.Y * MazeGenerator.MazeScale * MazeGenerator.CellSpawnOffset.y);
    }

    public static void PrintCell(MazeCell cell)
    {
        Debug.Log($"Cell[{cell.X},{cell.Y}], Visited = {cell.HasBeenVisited}, Type = {cell.Type}");
    }

    public static MazeCell GetRandomCell()
    {
        return MazeGenerator.MazeCells[Random.Range(1, MazeGenerator.MazeWidth - 1), Random.Range(1, MazeGenerator.MazeHeight - 1)];
    }

    public static MazeCell? GetRandomNeighbour(MazeCell actualCell)
    {
        List<MazeCell> mazeCellsNeighbours = new List<MazeCell>();

        if (actualCell.X + 1 < MazeGenerator.MazeWidth)
            mazeCellsNeighbours.Add(MazeGenerator.MazeCells[actualCell.X + 1, actualCell.Y]);
        if (actualCell.X - 1 > 0)
            mazeCellsNeighbours.Add(MazeGenerator.MazeCells[actualCell.X - 1, actualCell.Y]);
        if (actualCell.Y + 1 < MazeGenerator.MazeHeight)
            mazeCellsNeighbours.Add(MazeGenerator.MazeCells[actualCell.X, actualCell.Y + 1]);
        if (actualCell.Y - 1 > 0)
            mazeCellsNeighbours.Add(MazeGenerator.MazeCells[actualCell.X, actualCell.Y - 1]);

        mazeCellsNeighbours.Shuffle();

        foreach (MazeCell neighbourCell in mazeCellsNeighbours)
        {
            if (!neighbourCell.HasBeenVisited)
                return neighbourCell;
        }

        return null;
    }
}
