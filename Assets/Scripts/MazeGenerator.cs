using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public static event System.Action OnMazeGenerated;
    public static float MazeScale { get; private set; }
    public static MazeCell[,] MazeCells { get; private set; }
    public static int MazeWidth { get; private set; }
    public static int MazeHeight { get; private set; }
    public static Vector2 CellSpawnOffset { get; private set; }

    [SerializeField] private Transform _interiorWallPrefab,_mazeCornerUp,_mazeCornerDown;
    [SerializeField] private float _mazeScale;
    [SerializeField] private Transform _mazeParent;
    [SerializeField] private bool _showDebug;
    [SerializeField] private Vector2 _cellSpawnOffset;
    private Stack<MazeCell> _visitedCells;
    private Stack<MazeCell> _toVisitCells;
    private void Awake()
    {
        OnMazeGenerated = new System.Action(() => { });
        MazeScale = _mazeScale;
        CellSpawnOffset = _cellSpawnOffset;
        _mazeParent.localScale = new Vector3(MazeScale, MazeScale, MazeScale);

    }

    private void Start()
    {
        PlayerManager.OnMazeCompleted += SpawnNewMaze;
        PlayerManager.OnDead += SpawnNewMaze;
    }

    public void SetMazeWidthHeight(int sizeRows, int sizeCols)
    {
        MazeHeight = sizeRows;
        MazeWidth = sizeCols;
    }

    public void SpawnNewMaze()
    {
        _toVisitCells = new Stack<MazeCell>();
        _visitedCells = new Stack<MazeCell>();

        for (int i = 0; i < _mazeParent.childCount; i++)
        {
            Destroy(_mazeParent.GetChild(i).gameObject);
        }

        InitMazeCells();
        GenerateMaze();
        GenerateCellBorders();
    }
    private void GenerateMaze2()
    {
        MazeCell startCell = MazeCells[1, 1];
        _toVisitCells.Push(startCell);

        do
        {
            MazeCell actualCell = _toVisitCells.Pop();
            MazeCells[actualCell.X, actualCell.Y].HasBeenVisited = true;

            List<MazeCell> mazeCellsNeighbours = new List<MazeCell>();

            mazeCellsNeighbours.Add(MazeCells[actualCell.X + 1, actualCell.Y]);
            mazeCellsNeighbours.Add(MazeCells[actualCell.X - 1, actualCell.Y]);
            mazeCellsNeighbours.Add(MazeCells[actualCell.X, actualCell.Y + 1]);
            mazeCellsNeighbours.Add(MazeCells[actualCell.X, actualCell.Y - 1]);

            mazeCellsNeighbours.Shuffle();

            foreach (MazeCell neighbourCell in mazeCellsNeighbours)
            {
                if (!neighbourCell.HasBeenVisited)
                    if (!_toVisitCells.Contains(neighbourCell))
                        _toVisitCells.Push(MazeCells[neighbourCell.X, neighbourCell.Y]);
            }


        } while (_toVisitCells.Count > 0);
    }
    private void GenerateMaze1()
    {
        MazeCell startCell = MazeCells[1, 1];
        _toVisitCells.Push(startCell);

        int index = 0;

        do
        {
            MazeCell actualCell = _toVisitCells.Pop();

            AddRandomNeighboursToVisit(actualCell);

            MazeCells[actualCell.X, actualCell.Y].HasBeenVisited = true;

            TMP_Text text = MazeCells[actualCell.X, actualCell.Y].MyWall.GetComponentInChildren<TextMeshPro>();

            index++;
            text.SetText(index.ToString());

            if (_visitedCells.TryPeek(out MazeCell lastVisitedCell))
            {
                DestroyWalls(actualCell, lastVisitedCell);
            }
            else
            {
                //only for the first because it dosen't have a cell before
                //Debug.Log("-- NO CELLA --");
            }

            _visitedCells.Push(actualCell);

            #region comment
            /*
                //if (toVisitCells.TryPeek(out MazeCell nextCell))
                //{
                //    Vector2 nextCellDirection = new Vector2(nextCell.X - actualCell.X, nextCell.Y - actualCell.Y);

                //    switch (nextCellDirection)
                //    {
                //        case Vector2 v when v.Equals(Vector2.up):
                //            Destroy(MazeCells[actualCell.X, actualCell.Y].MyWall.GetChild(0).gameObject);
                //            Destroy(MazeCells[nextCell.X, nextCell.Y].MyWall.GetChild(2).gameObject);
                //            Debug.Log("Up");
                //            break;
                //        case Vector2 v when v.Equals(Vector2.right):
                //            Destroy(MazeCells[actualCell.X, actualCell.Y].MyWall.GetChild(1).gameObject);
                //            Destroy(MazeCells[nextCell.X, nextCell.Y].MyWall.GetChild(3).gameObject);
                //            Debug.Log("Right");
                //            break;
                //        case Vector2 v when v.Equals(Vector2.down):
                //            Destroy(MazeCells[actualCell.X, actualCell.Y].MyWall.GetChild(2).gameObject);
                //            Destroy(MazeCells[nextCell.X, nextCell.Y].MyWall.GetChild(0).gameObject);
                //            Debug.Log("Down");
                //            break;
                //        case Vector2 v when v.Equals(Vector2.left):
                //            Destroy(MazeCells[actualCell.X, actualCell.Y].MyWall.GetChild(3).gameObject);
                //            Destroy(MazeCells[nextCell.X, nextCell.Y].MyWall.GetChild(1).gameObject);
                //            Debug.Log("Left");
                //            break;
                //    }
                //}
                //else
                //{
                //    MazeCell lastMazeCell = visitedCells.Peek();
                //    MazeCells[actualCell.X, actualCell.Y].MyWall = Instantiate(ArrowPrefab, new Vector3(actualCell.X, actualCell.Y, 0), Quaternion.identity);
                //    MazeCells[actualCell.X, actualCell.Y].MyWall.SetParent(MazeParent);

                //    //Vector2 lastCellDirection = new Vector2(lastMazeCell.X - actualCell.X, lastMazeCell.Y - actualCell.Y);

                //    //switch (lastCellDirection)
                //    //{
                //    //    case Vector2 v when v.Equals(Vector2.up):
                //    //        Destroy(MazeCells[actualCell.X, actualCell.Y].MyWall.GetChild(0));
                //    //        Destroy(MazeCells[nextCell.X, nextCell.Y].MyWall.GetChild(2));
                //    //        Debug.Log("Up");
                //    //        break;
                //    //    case Vector2 v when v.Equals(Vector2.right):
                //    //        Destroy(MazeCells[actualCell.X, actualCell.Y].MyWall.GetChild(1));
                //    //        Destroy(MazeCells[nextCell.X, nextCell.Y].MyWall.GetChild(3));
                //    //        Debug.Log("Right");
                //    //        break;
                //    //    case Vector2 v when v.Equals(Vector2.down):
                //    //        Destroy(MazeCells[actualCell.X, actualCell.Y].MyWall.GetChild(2));
                //    //        Destroy(MazeCells[nextCell.X, nextCell.Y].MyWall.GetChild(0));
                //    //        Debug.Log("Down");
                //    //        break;
                //    //    case Vector2 v when v.Equals(Vector2.left):
                //    //        Destroy(MazeCells[actualCell.X, actualCell.Y].MyWall.GetChild(3));
                //    //        Destroy(MazeCells[nextCell.X, nextCell.Y].MyWall.GetChild(1));
                //    //        Debug.Log("Left");
                //    //        break;
                //    //}
                //}

                //PrintCell(MazeCells[actualCell.X, actualCell.Y]);
            */
            #endregion


        } while (_toVisitCells.Count > 0);

    }
    private void GenerateMaze3()
    {
        MazeCell startCell = MazeCells[1, 1];
        _toVisitCells.Push(startCell);

        int index = 0;
        bool hasReachedEnd = false;
        do
        {
            MazeCell actualCell = _toVisitCells.Pop();
            MazeCells[actualCell.X, actualCell.Y].HasBeenVisited = true;
            _visitedCells.Push(actualCell);

            MazeCell? randomNeighbour = MazeUtilities.GetRandomNeighbour(actualCell);

            if (randomNeighbour != null)
            {
                _toVisitCells.Push(randomNeighbour.Value);
                DestroyWalls(randomNeighbour.Value, actualCell);

            }
            else
            {
                hasReachedEnd = true;
            }

            TMP_Text text = MazeCells[actualCell.X, actualCell.Y].MyWall.GetComponentInChildren<TextMeshPro>();

            index++;
            text.SetText(index.ToString());

        } while (!hasReachedEnd);

    }
    private void GenerateMaze()
    {
        MazeUtilities.NewFirstCell();
        _toVisitCells.Push(MazeUtilities.FirstCell);

        int index = 0;

        do
        {
            MazeCell actualCell = _toVisitCells.Pop();
            MazeCells[actualCell.X, actualCell.Y].HasBeenVisited = true;

            MazeCell? randomNeighbour = MazeUtilities.GetRandomNeighbour(actualCell);

            if (randomNeighbour != null)
            {
                _toVisitCells.Push(randomNeighbour.Value);
                DestroyWalls(randomNeighbour.Value, actualCell);
                _visitedCells.Push(actualCell);
                index++;

            }
            else
            {
                if(_visitedCells.TryPop(out MazeCell poppedCell))
                {
                    _toVisitCells.Push(poppedCell);
                }
                    
                else
                {
                    OnMazeGenerated.Invoke();
                    return;
                }
            }

            if (_showDebug)
            {
                TMP_Text text = MazeCells[actualCell.X, actualCell.Y].MyWall.GetComponentInChildren<TextMeshPro>();

                if(text.text == "X"){
                    text.SetText(index.ToString());
                }
            }


        } while (_toVisitCells.Count > 0);

    }
    private void GenerateMaze5()
    {
        MazeCell actualCell = MazeCells[1, 1];
        int break_count = 1;

        while(break_count != MazeCells.Length)
        {
            MazeCells[actualCell.X, actualCell.Y].HasBeenVisited = true;

            //nullable
            MazeCell next_cell = (MazeCell)MazeUtilities.GetRandomNeighbour(actualCell);

            if(next_cell.X != actualCell.X || next_cell.Y != actualCell.Y)
            {
                next_cell.HasBeenVisited = true;
                break_count += 1;
                _visitedCells.Push(actualCell);
                DestroyWalls(next_cell, actualCell);
                actualCell = next_cell;
            }
            else
            {
                actualCell = _visitedCells.Pop();
            }

        }

    }
    private void GenerateCellBorders()
    {
        for (int y = 1; y < MazeHeight - 1; y++)
        {
            for (int x = 1; x < MazeWidth - 1; x++)
            {
                MazeCell cell = MazeCells[x, y];
                Transform UpWall = cell.MyWall.GetChild(0);
                Transform RightWall = cell.MyWall.GetChild(1);
                Transform DownWall = cell.MyWall.GetChild(2);
                Transform LeftWall = cell.MyWall.GetChild(3);

                if (!UpWall.gameObject.activeSelf)
                {
                    if (!RightWall.gameObject.activeSelf)
                    {
                        //2 2
                        Transform corner = Instantiate(_mazeCornerDown, cell.MyWall);
                        corner.localPosition = new Vector3(2, 2, 0);
                    }
                    if (!LeftWall.gameObject.activeSelf)
                    {
                        //-2 2
                        Transform corner = Instantiate(_mazeCornerDown, cell.MyWall);
                        corner.localPosition = new Vector3(-2, 2, 0);
                    }
                }
                if (!DownWall.gameObject.activeSelf)
                {
                    if (!RightWall.gameObject.activeSelf)
                    {
                        //2 -2
                        Transform corner = Instantiate(_mazeCornerUp, cell.MyWall);
                        corner.localPosition = new Vector3(2, -2, 0);
                    }
                    if (!LeftWall.gameObject.activeSelf)
                    {
                        //-2 -2
                        Transform corner = Instantiate(_mazeCornerUp, cell.MyWall);
                        corner.localPosition = new Vector3(-2, -2, 0);
                        corner.localRotation = Quaternion.Euler(0, 0, -90);
                    }
                }

            }
        }
    }
    private void DestroyWalls(MazeCell nextCell, MazeCell previousCell)
    {
        Vector2 lastCellDirection = new Vector2(previousCell.X - nextCell.X, previousCell.Y - nextCell.Y);

        switch (lastCellDirection)
        {
            case Vector2 v when v.Equals(Vector2.up):
                MazeCells[nextCell.X, nextCell.Y].MyWall.GetChild(0).gameObject.SetActive(false);
                MazeCells[previousCell.X, previousCell.Y].MyWall.GetChild(2).gameObject.SetActive(false);
                //Debug.Log("Up");
                break;
            case Vector2 v when v.Equals(Vector2.right):
                MazeCells[nextCell.X, nextCell.Y].MyWall.GetChild(1).gameObject.SetActive(false);
                MazeCells[previousCell.X, previousCell.Y].MyWall.GetChild(3).gameObject.SetActive(false);
                //Debug.Log("Right");
                break;
            case Vector2 v when v.Equals(Vector2.down):
                MazeCells[nextCell.X, nextCell.Y].MyWall.GetChild(2).gameObject.SetActive(false);
                MazeCells[previousCell.X, previousCell.Y].MyWall.GetChild(0).gameObject.SetActive(false);
                //Debug.Log("Down");
                break;
            case Vector2 v when v.Equals(Vector2.left):
                MazeCells[nextCell.X, nextCell.Y].MyWall.GetChild(3).gameObject.SetActive(false);
                MazeCells[previousCell.X, previousCell.Y].MyWall.GetChild(1).gameObject.SetActive(false);
                //Debug.Log("Left");
                break;
            default:
                //Debug.Log("------ZERO-----");
                //PrintCell(actualCell);
                //PrintCell(lastVisitedCell);
                //Destroy(MazeCells[lastVisitedCell.X, lastVisitedCell.Y].MyWall.GetChild(0).gameObject);
                //Destroy(MazeCells[lastVisitedCell.X, lastVisitedCell.Y].MyWall.GetChild(1).gameObject);
                //Destroy(MazeCells[lastVisitedCell.X, lastVisitedCell.Y].MyWall.GetChild(2).gameObject);
                //Destroy(MazeCells[lastVisitedCell.X, lastVisitedCell.Y].MyWall.GetChild(3).gameObject);
                //Debug.Log("------ZERO-----");

                TMP_Text text = MazeCells[nextCell.X, nextCell.Y].MyWall.GetComponentInChildren<TextMeshPro>();
                text.SetText("0");

                break;
        }
    }
    private void AddRandomNeighboursToVisit(MazeCell actualCell)
    {
        List<MazeCell> mazeCellsNeighbours = new List<MazeCell>();

        mazeCellsNeighbours.Add(MazeCells[actualCell.X + 1, actualCell.Y]);
        mazeCellsNeighbours.Add(MazeCells[actualCell.X - 1, actualCell.Y]);
        mazeCellsNeighbours.Add(MazeCells[actualCell.X, actualCell.Y + 1]);
        mazeCellsNeighbours.Add(MazeCells[actualCell.X, actualCell.Y - 1]);

        mazeCellsNeighbours.Shuffle();

        foreach (MazeCell neighbourCell in mazeCellsNeighbours)
        {
            if (!neighbourCell.HasBeenVisited)
                if (!_toVisitCells.Contains(neighbourCell))
                    _toVisitCells.Push(MazeCells[neighbourCell.X, neighbourCell.Y]);
        }
    }
    private void InitMazeCells()
    {
        MazeCells = new MazeCell[MazeWidth, MazeHeight];
        _toVisitCells = new Stack<MazeCell>();
        _visitedCells = new Stack<MazeCell>();

        GenerateMazeBorders();
        GenerateMazeInside();

    }
    private void GenerateMazeBorders()
    {
        //creates borders
        for (int x = 0; x < MazeWidth; x++)
        {
            SetMazeCell(x, 0, true, MazeCellType.Void);
            SetMazeCell(x, MazeHeight - 1, true, MazeCellType.Void);
        }

        for (int y = 1; y < MazeHeight - 1; y++)
        {
            SetMazeCell(0, y, true, MazeCellType.Void);
            SetMazeCell(MazeWidth - 1, y, true, MazeCellType.Void);
        }
    }
    private void GenerateMazeInside()
    {
        //internal maze generator
        for (int y = 1; y < MazeHeight - 1; y++)
        {
            for (int x = 1; x < MazeWidth - 1; x++)
            {
                SetMazeCell(x, y, false, MazeCellType.Floor);
            }
        }
    }
    private void SetMazeCell(int x, int y, bool hasBeenVisited=false, MazeCellType type = MazeCellType.Null)
    {
        MazeCells[x, y].X = x;
        MazeCells[x, y].Y = y;
        MazeCells[x, y].HasBeenVisited = hasBeenVisited;
        MazeCells[x, y].Type = type;

        if(type == MazeCellType.Void)
        {

        }
        else
        {
            MazeCells[x, y].MyWall = Instantiate(_interiorWallPrefab, _mazeParent);
            MazeCells[x, y].MyWall.localPosition = new Vector3(x * _cellSpawnOffset.x, y * _cellSpawnOffset.y, 0);
        }
    }
    private void OnDrawGizmos()
    {
        if (!_showDebug || MazeCells == null) return;

        for (int y = MazeHeight-1; y >= 0; y--)
        {
            for (int x = 0; x <= MazeWidth-1; x++)
            {
                MazeCell actualCell = MazeCells[x,y];

                switch (actualCell.Type)
                {
                    case MazeCellType.Null:
                        Gizmos.color = Color.cyan;
                        break;
                    case MazeCellType.Floor:
                        Gizmos.color = Color.black;
                        break;
                    case MazeCellType.Void:
                        Gizmos.color = Color.white;
                        break;
                    case MazeCellType.Trap:
                        Gizmos.color = Color.red;
                        break;
                }
                    Gizmos.DrawCube(new Vector3(actualCell.X + 0.5f, actualCell.Y + 0.5f, 0), Vector3.one * 0.9f);
            }
        }
    }
}
