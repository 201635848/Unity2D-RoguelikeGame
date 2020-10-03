using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int width;
    public int height;

    public GameObject floorTile;
    public GameObject wallTile;


    public string seed;
    public bool useRandomSeed;

    [Range(0, 100)]
    public int randomFillPercent;

    int[,] map;
    private GameObject[,] boardPositionsFloor;

    private void Start()
    {
        boardPositionsFloor = new GameObject[width, height];
        GenerateMap();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GenerateMap();
        }
    }

    private void GenerateMap()
    {
        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < 5; i++)
        {
            SmoothMap();
        }
        DrawMap();

    }

    void RandomFillMap()
    {
        if (useRandomSeed)
        {
            seed = Time.time.ToString();
        }

        //GetHashCode()를 하면 string 타입의 seed 에서 숫자를 뽑아내는데 렌덤한 숫자라 prng 로서 가치가 있는듯, 자세하게는 더 알아봐야할듯.
        //prng는 pseudoRandomNumberGenerator 의 약자인데, 컴퓨터는 난수를 생성하지 못해서 일반적으로 난수를 위해서 seed가 필요하다.
        //이때 이 난수를 위한 seed를 랜덤하게 생성해주는게 필요한데, 일반적으로 prng 라고 부르는듯하다.
        System.Random prng = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //아래 조건문은 그림을 그려보던가 해서 이해를 해야겠다... 아마도 정가운데 3*3공간에 1값을 넣어주는 듯 하다.
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    //랜덤퍼센트보다 낮으면 1을 반환하는데, 이는 1벽을 의미하고 0은 빈 공간을 의미한다.
                    map[x, y] = (prng.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiels = GetSurroundingWallCount(x, y);
                //이웃한 타일이 4개 초과면 자신도 벽이된다..
                if (neighbourWallTiels > 4)
                {
                    map[x, y] = 1;
                }
                //이웃한 타일이 4개 미만이면 뚫린 공간이된다..
                //즉 이웃한 타일이 많은쪽은 점점 많은쪽으로 벽이되고, 없는쪽은 점점 없는쪽으로 빈공간이 되는것.
                else if (neighbourWallTiels < 4)
                {
                    map[x, y] = 0;
                }
            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        //아래 2중 for문은 gridX의 주면 3*3에 있는 이웃의 값을 가져온다.
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                //아래 이프문은 예방문인듯. 가로세로값 이내에서 이웃값을 계산하기위한 조건문
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    //이웃의 값만 가져오기 위해 자기자신의 값을 제외한다
                    //자기 자신을 빼고 이웃한 8개의 타일에서 벽의 개수를 카운트합니다
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    //이웃값을 계산할때 width, height값을 포함하지 않았으므로 테두리는 모두 벽으로 인정되며 카운트된다;
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

    private void DrawMap()
    {
        if (map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (map[x, y] == 1)
                    {
                        GameObject instance = Instantiate(wallTile, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(transform);
                        boardPositionsFloor[x, y] = instance;
                    }
                    else if (map[x, y] == 0)
                    {
                        GameObject instance = Instantiate(floorTile, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(transform);
                        boardPositionsFloor[x, y] = instance;
                    }
                }
            }
        }
    }
}
