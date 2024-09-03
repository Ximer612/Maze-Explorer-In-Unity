using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCellsSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _coinPrefab, _keyPrefab, _speedPowerUpPrefab, _exitDoorRoomPrefab, _spikesPrefab,_ladderPrefab,_zoomPowerUpPrefab;
    [SerializeField] private List<GameObject> _spawnedObjects, _spikes;
    [SerializeField] private Transform _itemsParent;

    private bool _spikesAreUp;
    private float _counter, _timer;

    void Start()
    {
        _timer = 5f;
        _counter = _timer;

        _spawnedObjects = new List<GameObject>();
        _spikes = new List<GameObject>();

        MazeGenerator.OnMazeGenerated += DestroyAll;
        MazeGenerator.OnMazeGenerated += CreateCells;

        _exitDoorRoomPrefab = Instantiate(_exitDoorRoomPrefab, Vector3.zero, Quaternion.identity);
        _exitDoorRoomPrefab.transform.localScale = Vector3.one * MazeGenerator.MazeScale;

    }

    private void Update()
    {
        _counter -= Time.deltaTime;

        if (_counter < 0)
        {
            _spikesAreUp = !_spikesAreUp;

            foreach (var spike in _spikes)
            {
                spike.GetComponent<Animator>().SetBool("Up",_spikesAreUp);
                spike.tag = _spikesAreUp? "Enemy" : "Untagged";
            }

            _counter = _timer;
        }
    }


    void CreateCells()
    {
        int mazeCells = (MazeGenerator.MazeHeight * MazeGenerator.MazeWidth);
        int mazeBorderCells = (MazeGenerator.MazeHeight + MazeGenerator.MazeWidth) * 2 - 4;

        int availableCells = mazeCells - mazeBorderCells; // 4 = corners

        SpawnExitDoor();
        SpawnKey();

        availableCells -= 2;

        SpawnCoins((int)(availableCells * 0.5f));
        SpawnTraps((int)(availableCells * 0.4f));
        SpawnSpeedPowerUps((int)(availableCells * 0.2f));
        SpawnZoomPowerUpPrefab(4);
        SpawnStartLadder();
    }

    void SpawnStartLadder()
    {
        GameObject go = Instantiate(_ladderPrefab, _itemsParent);
        _spawnedObjects.Add(go);
        go.transform.position = MazeUtilities.PositionToMaze(MazeUtilities.FirstCell);
    }

    void SpawnKey()
    {
            MazeCell randomCell;

            do
            {
                randomCell = MazeUtilities.GetRandomCell();

            } while (randomCell.Type != MazeCellType.Floor);

            GameObject key = Instantiate(_keyPrefab, _itemsParent);
            _spawnedObjects.Add(key);
            key.transform.position = MazeUtilities.PositionToMaze(randomCell);
            randomCell.Type = MazeCellType.Key;
    }

    void SpawnCoins(int coinsToSpawn)
    {
        for (int i = 0; i < coinsToSpawn; i++)
        {
            MazeCell randomCell;

            do
            {
                randomCell = MazeUtilities.GetRandomCell();

            } while (randomCell.Type == MazeCellType.Door || randomCell.Type == MazeCellType.FirstCell);

            GameObject go = Instantiate(_coinPrefab, _itemsParent);
            _spawnedObjects.Add(go);
            go.transform.position = MazeUtilities.PositionToMaze(randomCell);
            Vector3 randomOffset = new Vector3(Random.Range(-1.25f, 1.25f), Random.Range(-1.25f, 1.25f), 0);
            go.transform.localPosition += randomOffset * MazeGenerator.MazeScale;
        }
    }

    void SpawnSpeedPowerUps(int powerUpsToSpawn)
    {
        for (int i = 0; i < powerUpsToSpawn; i++)
        {
            MazeCell randomCell;

            do
            {
                randomCell = MazeUtilities.GetRandomCell();

            } while (randomCell.Type == MazeCellType.Floor);

            GameObject go = Instantiate(_speedPowerUpPrefab, _itemsParent);
            _spawnedObjects.Add(go);
            go.transform.position = MazeUtilities.PositionToMaze(randomCell);
            Vector3 randomOffset = new Vector3(Random.Range(-1.25f, 1.25f), Random.Range(-1.25f, 1.25f), 0);
            go.transform.localPosition += randomOffset * MazeGenerator.MazeScale;
        }
    }

    void SpawnZoomPowerUpPrefab(int powerUpsToSpawn)
    {
        for (int i = 0; i < powerUpsToSpawn; i++)
        {
            MazeCell randomCell;

            do
            {
                randomCell = MazeUtilities.GetRandomCell();

            } while (randomCell.Type == MazeCellType.Floor);

            GameObject go = Instantiate(_zoomPowerUpPrefab, _itemsParent);
            _spawnedObjects.Add(go);
            go.transform.position = MazeUtilities.PositionToMaze(randomCell);
            Vector3 randomOffset = new Vector3(Random.Range(-1.25f, 1.25f), Random.Range(-1.25f, 1.25f), 0);
            go.transform.localPosition += randomOffset * MazeGenerator.MazeScale;
        }
    }

    void SpawnTraps(int trapsToSpawn)
    {
        for (int i = 0; i < trapsToSpawn; i++)
        {
            MazeCell randomCell;

            do
            {
                randomCell = MazeUtilities.GetRandomCell();

            } while (randomCell.Type != MazeCellType.Floor);

            GameObject go = Instantiate(_spikesPrefab, _itemsParent);
            go.transform.localScale = Vector3.one * MazeGenerator.MazeScale;
            _spawnedObjects.Add(go);
            _spikes.Add(go);
            go.transform.position = MazeUtilities.PositionToMaze(randomCell);
            MazeGenerator.MazeCells[randomCell.X, randomCell.Y].Type = MazeCellType.Trap;
        }
    }

    void SpawnExitDoor()
    {
        MazeCell randomCell;
        int wallIndex=-1;
        do
        {
            randomCell = MazeUtilities.GetRandomCell();
            int wallsNumber = GetCellWallsNumber(randomCell);

            if (wallsNumber > 2)
            {
                wallIndex = GetCellWallEmpty(randomCell);

                if (randomCell == MazeUtilities.FirstCell)
                {

                    wallIndex = -1;
                }
            }

        } while (wallIndex < 0);

        MazeGenerator.MazeCells[randomCell.X, randomCell.Y].Type = MazeCellType.Door;

        _exitDoorRoomPrefab.transform.position = MazeUtilities.PositionToMaze(randomCell);
        HideExitDoor();
        _exitDoorRoomPrefab.transform.GetChild(wallIndex).gameObject.SetActive(true);
        MazeGenerator.MazeCells[randomCell.X, randomCell.Y].MyWall.GetChild(4).GetComponent<SpriteRenderer>().color = Color.green;

        PlayerManager.OnKeyTaken += () => HideExitDoor();

    }

    void HideExitDoor()
    {
        _exitDoorRoomPrefab.transform.GetChild(3).gameObject.SetActive(false);
        _exitDoorRoomPrefab.transform.GetChild(2).gameObject.SetActive(false);
        _exitDoorRoomPrefab.transform.GetChild(1).gameObject.SetActive(false);
        _exitDoorRoomPrefab.transform.GetChild(0).gameObject.SetActive(false);
    }

    void DestroyAll()
    {
        foreach (GameObject coin in _spawnedObjects)
        {
            Destroy(coin);
        }

        foreach (GameObject spike in _spikes)
        {
            Destroy(spike);
        }

        _spawnedObjects = new List<GameObject>();
        _spikes = new List<GameObject>();   
    }

    int GetCellWallsNumber(MazeCell cell)
    {
        int wallsNumber = 0;

        wallsNumber += cell.MyWall.GetChild(0).gameObject.activeSelf ? 1 : 0;
        wallsNumber += cell.MyWall.GetChild(1).gameObject.activeSelf ? 1 : 0;
        wallsNumber += cell.MyWall.GetChild(2).gameObject.activeSelf ? 1 : 0;
        wallsNumber += cell.MyWall.GetChild(3).gameObject.activeSelf ? 1 : 0;


        return wallsNumber;
    }

    int GetCellWallEmpty(MazeCell cell)
    {

        if (!cell.MyWall.GetChild(0).gameObject.activeSelf) return 0;
        if (!cell.MyWall.GetChild(1).gameObject.activeSelf) return 1;
        if (!cell.MyWall.GetChild(2).gameObject.activeSelf) return 2;
        if (!cell.MyWall.GetChild(3).gameObject.activeSelf) return 3;

        return -1;
    }
}
