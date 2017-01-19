using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomManager : ScriptableObject {

    private Rooms rooms;
    private static RoomManager instance = null;
    private string dataPath;
    

    public static RoomManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = CreateInstance<RoomManager>();
                instance.initRooms();
            }
            return instance;
        }
    }

    void initRooms()
    {
        rooms = new Rooms();
        int level = GameManager.instance.level;

        rooms.addRoom(0, level, -8, 0, 27, 13);
        rooms.addRoom(1, level, 28, 0, 47, 11);
        rooms.addRoom(2, level, 48, 0, 73, 11);
        rooms.addRoom(3, level, -7, 14, 20, 26);
        rooms.addRoom(4, level, 21, 14, 51, 26);
        rooms.addRoom(5, level, 52, 14, 73, 26);
        rooms.addRoom(6, level, -7, 29, 10, 40);
        rooms.addRoom(7, level, 11, 29, 51, 40);
        rooms.addRoom(8, level, 52, 29, 73, 40);
        rooms.addRoom(9, level, -7, 43, 73, 60);
    }

    public int findRoomId(float x, float y)
    {
        return rooms.findRoomId(x, y);
    }

    public bool inRoom(int roomId, float x, float y)
    {
        return rooms.inRoom(roomId, x, y);
    }
}

public class Room
{
    public int level;
    public float xMin, xMax, yMin, yMax;

    public Room(int lv, float x1, float x2, float y1, float y2)
    {
        level = lv;
        xMin = x1;
        xMax = x2;
        yMin = y1;
        yMax = y2;
    }
}

public class Rooms
{
    private Dictionary<int, Room> roomDictionary;

    public Rooms()
    {
        roomDictionary = new Dictionary<int, Room>();
    }

    public void addRoom(int roomId, int lv, float x1, float y1, float x2, float y2)
    {
        roomDictionary.Add(roomId, new Room(lv, x1, x2, y1, y2));
    }

    public int findRoomId(float x, float y)
    {
        for(int i = 0; i < roomDictionary.Count;++i)
        {
            if(inRoom(i, x, y))
            {
                return i;
            }
        }
        return 0; // can't find any matching room
    }

    public bool inRoom(int roomId, float x, float y)
    {
        Room targetRoom = roomDictionary[roomId];
        if (targetRoom.yMax>=y && y >= targetRoom.yMin && targetRoom.xMax >= x && x >= targetRoom.xMin)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}