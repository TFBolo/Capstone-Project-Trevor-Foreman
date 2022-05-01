using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DungeonLoader : MonoBehaviour
{
    public NavMeshSurface surfacePet;
    public NavMeshSurface surfaceBoss;

    public class Cell
    {
        public bool visited = false;
        public bool[] status = new bool[4];
    }

    [System.Serializable]
    public class Rule
    {
        public GameObject room;
        public Vector2Int minPos;
        public Vector2Int maxPos;

        public bool required;

        public int ProbOfSpawn(int x, int y)
        {
            if (x >= minPos.x && x <= maxPos.x && y >= minPos.y && y <= maxPos.y)
            {
                return required ? 2 : 1;
            }

            return 0;
        }
    }
    public Vector2 size;
    public int startPos = 0;
    public Rule[] rooms;
    public Vector2 offset;

    List<Cell> board;

    void Start()
    {
        DungeonGenerator();

        surfacePet.BuildNavMesh();
        surfaceBoss.BuildNavMesh();
    }

    public void GenerateDungeon()
    {
        for (int i=0; i < size.x; i++)
        {
            for (int j=0; j < size.y; j++)
            {
                Cell currentCell = board[Mathf.FloorToInt(i + j * size.x)];
                if (currentCell.visited)
                {
                    int rRoom = -1;
                    List<int> availRooms = new List<int>();

                    for (int k=0; k < rooms.Length; k++)
                    {
                        int prob = rooms[k].ProbOfSpawn(i, j);

                        if (prob == 2)
                        {
                            rRoom = k;
                            break;
                        }
                        else if (prob == 1)
                        {
                            availRooms.Add(k);
                        }
                    }

                    if (rRoom == -1)
                    {
                        if (availRooms.Count > 0)
                        {
                            rRoom = availRooms[Random.Range(0, availRooms.Count)];
                        }
                        else
                        {
                            rRoom = 0;
                        }
                    }


                    var newRoom = Instantiate(rooms[rRoom].room, transform.position + new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<DungeonRooms>();
                    newRoom.UpdateRooms(currentCell.status);
                }
                
            }
        }
    }

    void DungeonGenerator()
    {
        board = new List<Cell>();
        
        for (int i=0; i < size.x; i++)
        {
            for (int j=0; j < size.y; j++)
            {
                board.Add(new Cell());
            }
        }

        int currentCell = startPos;

        Stack<int> path = new Stack<int>();

        int k = 0;

        while (k < 1000)
        {
            k++;

            board[currentCell].visited = true;

            if (currentCell == board.Count - 1)
            {
                break;
            }

            List<int> neighbors = CheckNeighbors(currentCell);

            if (neighbors.Count == 0)
            {
                if (path.Count == 0)
                {
                    break;
                }
                else
                {
                    currentCell = path.Pop();
                }
            }
            else
            {
                path.Push(currentCell);

                int newCell = neighbors[Random.Range(0, neighbors.Count)];

                if (newCell > currentCell)
                {
                    if (newCell - 1 == currentCell)
                    {
                        board[currentCell].status[2] = true;
                        currentCell = newCell;
                        board[currentCell].status[3] = true;
                    }
                    else
                    {
                        board[currentCell].status[1] = true;
                        currentCell = newCell;
                        board[currentCell].status[0] = true;
                    }
                }
                else
                {
                    if (newCell + 1 == currentCell)
                    {
                        board[currentCell].status[3] = true;
                        currentCell = newCell;
                        board[currentCell].status[2] = true;
                    }
                    else
                    {
                        board[currentCell].status[0] = true;
                        currentCell = newCell;
                        board[currentCell].status[1] = true;
                    }
                }
            }
        }
        GenerateDungeon();
    }

    List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();

        if (cell - size.x >= 0 && !board[Mathf.FloorToInt(cell - size.x)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell - size.x));
        }

        if (cell + size.x < board.Count && !board[Mathf.FloorToInt(cell + size.x)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell + size.x));
        }

        if ((cell + 1) % size.x != 0 && !board[Mathf.FloorToInt(cell + 1)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell + 1));
        }

        if (cell % size.x != 0 && !board[Mathf.FloorToInt(cell - 1)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell - 1));
        }

        return neighbors;
    }
}
