using Game;
using System.Collections.Generic;

namespace API_WE_Mod
{
    public class Maze
    {
        private Room[,] roomMatrix;
        private List<List<Maze.Room>> roads;
        private Random random;

        public Maze(int width, int height)
        {
            this.random = new Random();
            this.InstRooms(width, height);
            this.OrganizeRooms();
            this.SetFixedDoor();
            this.Interlink();
        }

        private void InstRooms(int width, int height)
        {
            this.roomMatrix = new Maze.Room[width, height];
            this.roads = new List<List<Maze.Room>>();
            for (int index1 = 0; index1 < width; ++index1)
            {
                for (int index2 = 0; index2 < height; ++index2)
                {
                    List<Maze.Room> roomList = new List<Maze.Room>();
                    this.roomMatrix[index1, index2] = new Maze.Room();
                    roomList.Add(this.roomMatrix[index1, index2]);
                    this.roads.Add(roomList);
                }
            }
        }

        private void OrganizeRooms()
        {
            for (int index1 = 0; index1 < this.roomMatrix.GetLength(0); ++index1)
            {
                for (int index2 = 0; index2 < this.roomMatrix.GetLength(1) - 1; ++index2)
                    this.roomMatrix[index1, index2].BottonDoor = this.roomMatrix[index1, index2 + 1].TopDoor;
            }
            for (int index1 = 0; index1 < this.roomMatrix.GetLength(1); ++index1)
            {
                for (int index2 = 0; index2 < this.roomMatrix.GetLength(0) - 1; ++index2)
                    this.roomMatrix[index2, index1].RightDoor = this.roomMatrix[index2 + 1, index1].LeftDoor;
            }
        }

        private void SetFixedDoor()
        {
            for (int index = 0; index < this.roomMatrix.GetLength(0); ++index)
                this.roomMatrix[index, 0].TopDoor.IsFixed = true;
            for (int index = 0; index < this.roomMatrix.GetLength(0); ++index)
                this.roomMatrix[index, this.roomMatrix.GetLength(1) - 1].BottonDoor.IsFixed = true;
            for (int index = 0; index < this.roomMatrix.GetLength(1); ++index)
                this.roomMatrix[0, index].LeftDoor.IsFixed = true;
            for (int index = 0; index < this.roomMatrix.GetLength(1); ++index)
                this.roomMatrix[this.roomMatrix.GetLength(0) - 1, index].RightDoor.IsFixed = true;
        }

        private void Interlink()
        {
            while (!this.AllRoomLinked())
            {
                List<Maze.Door> outlineDoors = this.GetOutlineDoors(this.roads[this.random.UniformInt(0, 1048575) % this.roads.Count]);
                Maze.Door door = outlineDoors[this.random.UniformInt(0, 1048575) % outlineDoors.Count];
                List<List<Maze.Room>> oldRoads = this.GetOldRoads(door);
                List<Maze.Room> newRoad = this.GetNewRoad(oldRoads);
                this.RemoveOldRoads(oldRoads);
                this.roads.Add(newRoad);
                door.OpenTheDoor();
            }
        }

        private void RemoveOldRoads(List<List<Maze.Room>> oldRoads)
        {
            foreach (List<Maze.Room> oldRoad in oldRoads)
                this.roads.Remove(oldRoad);
        }

        private List<Maze.Room> GetNewRoad(List<List<Maze.Room>> oldRoad)
        {
            List<Maze.Room> roomList1 = new List<Maze.Room>();
            foreach (List<Maze.Room> roomList2 in oldRoad)
            {
                foreach (Maze.Room room in roomList2)
                    roomList1.Add(room);
            }
            return roomList1;
        }

        private List<List<Maze.Room>> GetOldRoads(Maze.Door door)
        {
            List<List<Maze.Room>> roomListList = new List<List<Maze.Room>>();
            foreach (List<Maze.Room> road in this.roads)
            {
                foreach (Maze.Room room in road)
                {
                    if (this.TwoDoorAreEqual(room.TopDoor, door))
                    {
                        roomListList.Add(road);
                        break;
                    }
                    if (this.TwoDoorAreEqual(room.BottonDoor, door))
                    {
                        roomListList.Add(road);
                        break;
                    }
                    if (this.TwoDoorAreEqual(room.LeftDoor, door))
                    {
                        roomListList.Add(road);
                        break;
                    }
                    if (this.TwoDoorAreEqual(room.RightDoor, door))
                    {
                        roomListList.Add(road);
                        break;
                    }
                }
            }
            return roomListList;
        }

        private bool TwoDoorAreEqual(Maze.Door doorSource, Maze.Door doorTarget)
        {
            return doorSource.GetLockState() && !doorSource.IsFixed && object.Equals((object)doorSource, (object)doorTarget);
        }

        private bool AllRoomLinked()
        {
            return this.roads.Count == 1;
        }

        private List<Maze.Door> GetOutlineDoors(List<Maze.Room> road)
        {
            List<Maze.Door> outlineDoors = new List<Maze.Door>();
            foreach (Maze.Room room in road)
            {
                this.AddOutlineDoor(room.TopDoor, outlineDoors);
                this.AddOutlineDoor(room.BottonDoor, outlineDoors);
                this.AddOutlineDoor(room.LeftDoor, outlineDoors);
                this.AddOutlineDoor(room.RightDoor, outlineDoors);
            }
            return outlineDoors;
        }

        private void AddOutlineDoor(Maze.Door door, List<Maze.Door> outlineDoors)
        {
            if (!door.GetLockState() || door.IsFixed)
                return;
            if (!outlineDoors.Contains(door))
                outlineDoors.Add(door);
            else
                outlineDoors.Remove(door);
        }

        private bool[,] RoomToData()
        {
            bool[,] dataMatrix = new bool[this.roomMatrix.GetLength(0) * 2 + 1, this.roomMatrix.GetLength(1) * 2 + 1];
            this.PreFill(dataMatrix);
            for (int xPos = 0; xPos < this.roomMatrix.GetLength(0); ++xPos)
            {
                for (int yPos = 0; yPos < this.roomMatrix.GetLength(1); ++yPos)
                {
                    this.SetData(dataMatrix, xPos, yPos, -1, 0, this.roomMatrix[xPos, yPos].LeftDoor.GetLockState());
                    this.SetData(dataMatrix, xPos, yPos, 1, 0, this.roomMatrix[xPos, yPos].RightDoor.GetLockState());
                    this.SetData(dataMatrix, xPos, yPos, 0, -1, this.roomMatrix[xPos, yPos].TopDoor.GetLockState());
                    this.SetData(dataMatrix, xPos, yPos, 0, 1, this.roomMatrix[xPos, yPos].BottonDoor.GetLockState());
                }
            }
            return dataMatrix;
        }

        private void SetData(bool[,] dataMatrix, int xPos, int yPos, int xOffset, int yOffset, bool isClose)
        {
            dataMatrix[xPos * 2 + 1 + xOffset, yPos * 2 + 1 + yOffset] = isClose;
        }

        private void PreFill(bool[,] dataMatrix)
        {
            int index1 = 0;
            while (index1 < dataMatrix.GetLength(0))
            {
                int index2 = 0;
                while (index2 < dataMatrix.GetLength(1))
                {
                    dataMatrix[index1, index2] = true;
                    index2 += 2;
                }
                index1 += 2;
            }
        }

        public bool[,] GetBoolArray()
        {
            return this.RoomToData();
        }

        private class Room
        {
            private Maze.Door topDoor;
            private Maze.Door bottonDoor;
            private Maze.Door leftDoor;
            private Maze.Door rightDoor;
            private Maze.Door topLeftDoor;
            private Maze.Door topRightDoor;
            private Maze.Door bottonLeftDoor;
            private Maze.Door bottonRightDoor;

            public Maze.Door TopDoor
            {
                get
                {
                    return this.topDoor;
                }
                set
                {
                    this.topDoor = value;
                }
            }

            public Maze.Door BottonDoor
            {
                get
                {
                    return this.bottonDoor;
                }
                set
                {
                    this.bottonDoor = value;
                }
            }

            public Maze.Door LeftDoor
            {
                get
                {
                    return this.leftDoor;
                }
                set
                {
                    this.leftDoor = value;
                }
            }

            public Maze.Door RightDoor
            {
                get
                {
                    return this.rightDoor;
                }
                set
                {
                    this.rightDoor = value;
                }
            }

            public Maze.Door TopLeftDoor
            {
                get
                {
                    return this.topLeftDoor;
                }
                set
                {
                    this.topLeftDoor = value;
                }
            }

            public Maze.Door TopRightDoor
            {
                get
                {
                    return this.topRightDoor;
                }
                set
                {
                    this.topRightDoor = value;
                }
            }

            public Maze.Door BottonLeftDoor
            {
                get
                {
                    return this.bottonLeftDoor;
                }
                set
                {
                    this.bottonLeftDoor = value;
                }
            }

            public Maze.Door BottonRightDoor
            {
                get
                {
                    return this.bottonRightDoor;
                }
                set
                {
                    this.bottonRightDoor = value;
                }
            }

            public Room()
            {
                this.topDoor = new Maze.Door();
                this.bottonDoor = new Maze.Door();
                this.leftDoor = new Maze.Door();
                this.rightDoor = new Maze.Door();
                this.topLeftDoor = new Maze.Door();
                this.topRightDoor = new Maze.Door();
                this.bottonLeftDoor = new Maze.Door();
                this.bottonRightDoor = new Maze.Door();
            }
        }

        private class Door
        {
            private bool isLocked;
            private bool isFixed;

            public bool IsFixed
            {
                get
                {
                    return this.isFixed;
                }
                set
                {
                    this.isFixed = value;
                }
            }

            public Door()
            {
                this.isLocked = true;
            }

            public void OpenTheDoor()
            {
                this.isLocked = false;
            }

            public void CloseTheDoor()
            {
                this.isLocked = true;
            }

            public bool GetLockState()
            {
                return this.isLocked;
            }
        }
    }
}
