using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject floorTile;
    private GameObject[,] boardPositionsFloor;



    public int boardRows, boardColumns;
    public int minRoomSize, maxRoomSize;

    public class SubDungeon {
        public SubDungeon left, right;
        public Rect rect;
        public Rect room = new Rect(-1, -1, 0, 0);


        // Debug 이용
        public int debugId;
        private static int debugCounter = 0;

        public SubDungeon(Rect mrect)
        {
            rect = mrect;
            debugId = debugCounter;
            debugCounter++;
        }

        public bool IAmLeaf()
        {
            //child node 없을 시 true 반환
            return left == null && right == null;
        }

        public bool Split(int minRoomSize, int maxRoomSize)
        {
            if (!IAmLeaf())
            {
                return false;
            }

            //비율에 따라 수직 또는 수평 분할을 선택한다.
            // 너무 넓게 수직으로 분할되거나 수평으로 너무 길게 분할 혹은 정사각형에 가까울 경우 임의로 수정할 것.
            bool splitH;
            if (rect.width / rect.height >= 1.25)
            {
                splitH = false;
            }
            else if (rect.height / rect.width >= 1.25)
            {
                splitH = true;
            }
            else
            {
                splitH = Random.Range(0.0f, 1.0f) > 0.5;
            }
            if (Mathf.Min(rect.height, rect.width) / 2 < minRoomSize)
            {
                Debug.Log("Sub-dungeon " + debugId + " will be a leaf");
                return false;
            }

            if (splitH)
            {
                // split so that the resulting sub-dungeons widths are not too small
                // (since we are splitting horizontally)
                int split = Random.Range(minRoomSize, (int)(rect.width - minRoomSize));

                left = new SubDungeon(new Rect(rect.x, rect.y, rect.width, split));
                right = new SubDungeon(
                  new Rect(rect.x, rect.y + split, rect.width, rect.height - split));
            }
            else
            {
                int split = Random.Range(minRoomSize, (int)(rect.height - minRoomSize));

                left = new SubDungeon(new Rect(rect.x, rect.y, split, rect.height));
                right = new SubDungeon(
                  new Rect(rect.x + split, rect.y, rect.width - split, rect.height));
            }

            return true;


        }

        public void CreateRoom()
        {
            if(left != null)
            {
                left.CreateRoom();
            }
            if(right != null)
            {
                left.CreateRoom();
            }
            if (IAmLeaf())
            {
                int roomWidth = (int)Random.Range(rect.width / 2, rect.width - 2);
                int roomHeight = (int)Random.Range(rect.height / 2, rect.height - 2);
                int roomX = (int)Random.Range(1, rect.width - roomWidth - 1);
                int roomY = (int)Random.Range(1, rect.height - roomHeight - 1);

                // room position will be absolute in the board, not relative to the sub-dungeon
                room = new Rect(rect.x + roomX, rect.y + roomY, roomWidth, roomHeight);
                Debug.Log("Created room " + room + " in sub-dungeon " + debugId + " " + rect);
            }
        }
    }

    public void DrawRooms(SubDungeon subDungeon)
    {
        if (subDungeon == null)
        {
            return;
        }
        if (subDungeon.IAmLeaf())
        {
            for (int i = (int)subDungeon.room.x; i < subDungeon.room.xMax; i++)
            {
                for (int j = (int)subDungeon.room.y; j < subDungeon.room.yMax; j++)
                {
                    GameObject instance = Instantiate(floorTile, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(transform);
                    boardPositionsFloor[i, j] = instance;
                }
            }
        }
        else
        {
            DrawRooms(subDungeon.left);
            DrawRooms(subDungeon.right);
        }
    }


    public void CreateBSP(SubDungeon subDungeon)
    {
        Debug.Log("Splitting sub-dungeon " + subDungeon.debugId + ": " + subDungeon.rect);
        if (subDungeon.IAmLeaf())
        {
            // if the sub-dungeon is too large
            if (subDungeon.rect.width > maxRoomSize
              || subDungeon.rect.height > maxRoomSize
              || Random.Range(0.0f, 1.0f) > 0.25)
            {

                if (subDungeon.Split(minRoomSize, maxRoomSize))
                {
                    Debug.Log("Splitted sub-dungeon " + subDungeon.debugId + " in "
                      + subDungeon.left.debugId + ": " + subDungeon.left.rect + ", "
                      + subDungeon.right.debugId + ": " + subDungeon.right.rect);

                    CreateBSP(subDungeon.left);
                    CreateBSP(subDungeon.right);
                }
            }
        }
    }

    void Start()
    {
        SubDungeon rootSubDungeon = new SubDungeon(new Rect(0, 0, boardRows, boardColumns));
        CreateBSP(rootSubDungeon);
        rootSubDungeon.CreateRoom();

        boardPositionsFloor = new GameObject[boardRows, boardColumns];
        DrawRooms(rootSubDungeon);
    }


}
