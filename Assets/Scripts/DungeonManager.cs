using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct DungeonRoom
{
    public int width;
    public int height;
    public int x;
    public int y;
    public bool connected;

    public bool Equals(DungeonRoom other)
    {
        return this.width == other.width && this.height == other.height && this.x == other.x && this.y == other.y;
    }

    public override string ToString()
    {
        return x.ToString() + ", " + y.ToString();
    }

}
public class DungeonManager : MonoBehaviour
{
    [SerializeField] int dungeonWidth = 50;
    [SerializeField] int dungeonHeight = 50;
    [SerializeField] int minDungeonRooms = 8;
    [SerializeField] int maxDungeonRooms = 11;
    [SerializeField] int minSize = 1;
    [SerializeField] int maxSize = 8;
    [SerializeField] GameObject dungeonTilePrefab;
    public int floor;
    DungeonTile[,] dungeonMap;
    public static List<DungeonManager> dungeonFloors = new List<DungeonManager>();
    List<DungeonRoom> rooms;

    [HideInInspector] public GameObject upstairs;
    [HideInInspector] public GameObject downstairs;

    [SerializeField] GameObject stairPrefab;
    [SerializeField] GameObject enemyPrefab;

    enum ConnectionFilter { None, Connected, Unconnected };

    // Start is called before the first frame update
    void Start()
    {
        dungeonMap = new DungeonTile[dungeonWidth, dungeonHeight];
        rooms = new List<DungeonRoom>();
        floor = dungeonFloors.Count;
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        dungeonFloors.Add(this);

        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                DungeonTile newTile = Instantiate(dungeonTilePrefab, transform).GetComponent<DungeonTile>();
                newTile.transform.position = transform.position + new Vector3(x * newTile.size, 0, y * newTile.size);
                newTile.Init(x, y);
                dungeonMap[x, y] = newTile;
            }
        }

        int dungeonRooms = Random.Range(minDungeonRooms, maxDungeonRooms);
        for (int i = 0; i < dungeonRooms; i++)
        {
            while (rooms.Count == i)
            {
                DungeonRoom room = new DungeonRoom();
                room.width = Random.Range(minSize, maxSize);
                room.height = Random.Range(minSize, maxSize);
                room.x = Random.Range(0, dungeonWidth - room.width);
                room.y = Random.Range(0, dungeonHeight - room.height);

                if (!RoomOverlapsExisting(room))
                {
                    for (int x = room.x; x < room.x + room.width; x++)
                    {
                        for (int y = room.y; y < room.y + room.height; y++)
                        {
                            dungeonMap[x, y].isOn = true;
                        }
                    }
                    rooms.Add(room);
                }
            }
        }

        DungeonRoom center = rooms[Random.Range(0, rooms.Count)];
        center.connected = true;
        rooms[RoomIndex(center)] = center;
        while (FindClosestRoom(center, ConnectionFilter.Unconnected).x != 1000)
        {
            DungeonRoom currentRoom = FindClosestRoom(center, ConnectionFilter.Unconnected);
            DungeonRoom connectRoom = FindClosestRoom(currentRoom, ConnectionFilter.Connected);
            ConnectRooms(connectRoom, currentRoom);
        }
        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                dungeonMap[x, y].gameObject.SetActive(dungeonMap[x, y].isOn);
                if (dungeonMap[x, y].isOn)
                {
                    if (CheckDown(x,y)) dungeonMap[x, y].wallDown.SetActive(false);
                    if (CheckUp(x,y)) dungeonMap[x, y].wallUp.SetActive(false);
                    if (CheckLeft(x,y)) dungeonMap[x, y].wallLeft.SetActive(false);
                    if (CheckRight(x,y)) dungeonMap[x, y].wallRight.SetActive(false);
                }
            }
        }

        transform.position = new Vector3(0f, -floor * 4.5f, 0f);
        SetUpstairsPoint();

        AddEnemies();
        //SetDownstairsPoint();
    }

    bool RoomOverlapsExisting(DungeonRoom room)
    {
        foreach (DungeonRoom existing in rooms)
        {
            if (RoomsOverlap(room, existing)) return true;
        }
        return false;
    }
    bool RoomsOverlap(DungeonRoom room1, DungeonRoom room2)
    {
        if (room1.x < room2.x + room2.width &&
            room1.x + room1.width > room2.x &&
            room1.y < room2.y + room2.height &&
            room1.y + room1.height > room2.y) return true;
        return false;
    }

    int RoomIndex(DungeonRoom room)
    {
        for(int r = 0; r < rooms.Count; r++)
        {
            if (rooms[r].Equals(room)) return r;
        }
        return -1;
    }
    DungeonRoom FindClosestRoom(DungeonRoom from, ConnectionFilter checkConnected=ConnectionFilter.None)
    {
        DungeonRoom furthestRoom = new DungeonRoom();
        furthestRoom.x = 1000; //arbitrarily large number
        furthestRoom.y = 1000;
        furthestRoom.width = 1;
        furthestRoom.height = 1;
        DungeonRoom closestRoom = furthestRoom;
        int distance = RoomDistance(from, furthestRoom);
        foreach (DungeonRoom to in rooms)
        {
            if (!from.Equals(to))
            {
                if (RoomDistance(from, to) < distance)
                {
                    if (checkConnected == ConnectionFilter.None ||
                        (checkConnected == ConnectionFilter.Connected && to.connected) ||
                        (checkConnected == ConnectionFilter.Unconnected && !to.connected))
                    {
                        closestRoom = to;
                        distance = RoomDistance(from, to);
                    }
                }
            }
        }
        return closestRoom;
    }

    /// <summary>
    /// Returns the number of hallway tiles required to connect from and to
    /// </summary>
    int RoomDistance(DungeonRoom from, DungeonRoom to)
    {
        int xDistance = 0;
        int yDistance = 0;
        int distance = 0;
        if (from.x + from.width - 1 < to.x) xDistance += to.x - (from.x + from.width - 1);
        if (from.x > to.x + to.width - 1) xDistance += from.x - (to.x + to.width - 1);
        if (from.y + from.height - 1 < to.y) yDistance += to.y - (from.y + from.height - 1);
        if (from.y > to.y + to.height - 1) yDistance += from.y - (to.y + to.height - 1);
        if (xDistance != 0) distance += xDistance - 1;
        if (yDistance != 0) distance += yDistance - 1;
        if ((xDistance == 1 && yDistance > 0) || (yDistance == 1 && xDistance > 0)) distance += 1;
        return distance;
    }

    void ConnectRooms(DungeonRoom from, DungeonRoom to)
    {
        if (RoomDistance(from, to) == 0)
        {
            to.connected = true;
            rooms[RoomIndex(to)] = to;
            return;
        }
        int xDirection = 0;
        int yDirection = 0;
        List<int> possibleStartPoints = new List<int>();
        if (from.x + from.width - 1 < to.x) xDirection = 1;
        else if (from.x > to.x + to.width - 1) xDirection = -1;
        else
        {
            int startX = from.x > to.x ? from.x : to.x;
            int endX = from.x + from.width - 1 < to.x + to.width - 1 ? from.x + from.width - 1 : to.x + to.width - 1;
            for (int x = startX; x <= endX; x++) possibleStartPoints.Add(x);
        }
        if (from.y + from.height - 1 < to.y) yDirection = 1;
        else if (from.y > to.y + to.height - 1) yDirection = -1;
        else
        {
            int startY = from.y > to.y ? from.y : to.y;
            int endY = from.y + from.height - 1 < to.y + to.height - 1 ? from.y + from.height - 1 : to.y + to.height - 1;
            for (int y = startY; y <= endY; y++) possibleStartPoints.Add(y);
        }

        if (xDirection == 0)
        {
            if (yDirection < 0) CreateHallwayDown(possibleStartPoints[Random.Range(0, possibleStartPoints.Count)], from.y, to.y + to.height - 1);
            else CreateHallwayUp(possibleStartPoints[Random.Range(0, possibleStartPoints.Count)], from.y + from.height - 1, to.y);
        }
        else if (yDirection == 0)
        {
            if (xDirection < 0) CreateHallwayLeft(possibleStartPoints[Random.Range(0, possibleStartPoints.Count)], from.x, to.x + to.width - 1);
            else CreateHallwayRight(possibleStartPoints[Random.Range(0, possibleStartPoints.Count)], from.x + from.width - 1, to.x);
        }
        else
        {
            int startX = xDirection < 0 ? from.x : from.x + from.width - 1;
            int endX = xDirection < 0 ? to.x + to.width - 1 : to.x;
            int startY = yDirection < 0 ? from.y : from.y + from.height - 1;
            int endY = yDirection < 0 ? to.y + to.height - 1 : to.y;
            if (xDirection < 0) CreateHallwayLeft(startY, startX, endX);
            else CreateHallwayRight(startY, startX, endX);
            if (yDirection < 0) CreateHallwayDown(endX, startY, endY);
            else CreateHallwayUp(endX, startY, endY);
        }

        to.connected = true;
        rooms[RoomIndex(to)] = to;
        return;
    }

    void CreateHallwayDown(int x, int startY, int endY)
    {
        for (int y = startY; y >= endY; y--)
        {
            dungeonMap[x, y].isOn = true;
        }
    }
    void CreateHallwayUp(int x, int startY, int endY)
    {
        for (int y = startY; y <= endY; y++)
        {
            dungeonMap[x, y].isOn = true;
        }
    }
    void CreateHallwayLeft(int y, int startX, int endX)
    {
        for (int x = startX; x >= endX; x--)
        {
            dungeonMap[x, y].isOn = true;
        }
    }
    void CreateHallwayRight(int y, int startX, int endX)
    {
        for (int x = startX; x <= endX; x++)
        {
            dungeonMap[x, y].isOn = true;
        }
    }

    //Functions to check whether tiles in each direction are dungeon tiles
    bool CheckUp(int x, int y)
    {
        return y != dungeonHeight-1 && dungeonMap[x, y + 1].isOn;
    }

    bool CheckDown(int x, int y)
    {
        return y != 0 && dungeonMap[x, y - 1].isOn;
    }
    bool CheckLeft(int x, int y)
    {
        return x != 0 && dungeonMap[x - 1, y].isOn;
    }
    bool CheckRight(int x, int y)
    {
        return x != dungeonWidth && dungeonMap[x + 1, y].isOn;
    }

    public void SetUpstairsPoint()
    {
        int x;
        int y;
        while (true)
        {
            x = Random.Range(0, dungeonWidth);
            y = Random.Range(0, dungeonHeight);
            List<float> directions = TryPlaceStairs(x, y);
            if (directions.Contains(upstairs.transform.rotation.eulerAngles.y))
            {
                dungeonMap[x, y].ceiling.SetActive(false);
                transform.position = new Vector3(upstairs.transform.position.x - (x * 5), transform.position.y, upstairs.transform.position. y - (y * 5));
                break;
            }
        }
    }

    public void SetDownstairsPoint()
    {
        int x;
        int y;
        while (true)
        {
            x = Random.Range(0, dungeonWidth);
            y = Random.Range(0, dungeonHeight);
            List<float> directions = TryPlaceStairs(x, y);
            if (directions.Count > 0 && dungeonMap[x, y].ceiling.activeSelf)
            {
                dungeonMap[x, y].floor.SetActive(false);
                downstairs = Instantiate(
                    stairPrefab,
                    transform.position + new Vector3(x * 5, -4.5f, y * 5),
                    Quaternion.Euler(0f, directions[Random.Range(0, directions.Count)] + 180, 0f));
                downstairs.GetComponent<Stairs>().upperFloor = this;
                break;
            }
        }
    }
    List<float> TryPlaceStairs(int x, int y)
    {
        List<float> possibleDirections = new List<float>();
        if (dungeonMap[x, y].isOn)
        {
            if ((!CheckUp(x, y) && !CheckDown(x, y)) || (!CheckLeft(x, y) && !CheckRight(x, y))) return possibleDirections;
            if (CheckUp(x, y)) 
            {
                if (CheckLeft(x, y + 1) || CheckRight(x, y + 1)) possibleDirections.Add(0);
                else return new List<float>();
            }
            if (CheckDown(x, y))
            {
                if (CheckLeft(x, y - 1) || CheckRight(x, y - 1)) possibleDirections.Add(180);
                else return new List<float>();
            }
            if (CheckLeft(x, y))
            {
                if (CheckUp(x - 1, y) || CheckDown(x - 1, y)) possibleDirections.Add(270);
                else return new List<float>();
            }
            if (CheckRight(x, y))
            {
                if (CheckUp(x + 1, y) || CheckDown(x + 1, y)) possibleDirections.Add(90);
                else return new List<float>();
            }
        }
        return possibleDirections;
    }

    void AddEnemies()
    {
        int enemies = 0;
        for (int i = 0; i < QuestManager.instance.enemies; i++)
        {
            while (enemies == i)
            {
                int x = Random.Range(0, dungeonWidth);
                int y = Random.Range(0, dungeonHeight);
                if (dungeonMap[x, y].isOn && dungeonMap[x, y].ceiling.activeSelf)
                {
                    Vector3 position = transform.position + new Vector3(x * 5, 1, y * 5);
                    if (Vector3.Distance(upstairs.transform.position, position) > 30)
                    {
                        Instantiate(enemyPrefab, position, Quaternion.identity);
                        enemies++;
                    }
                }
            }
        }
    }
}
