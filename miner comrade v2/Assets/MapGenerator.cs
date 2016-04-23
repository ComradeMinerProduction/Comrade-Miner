using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

    public int width;
    public int height;
    public int z;
    public string seed;
    public bool useRandomSeed;

    [Range(0,100)]
    public int randomFillPercent;

    [Range(0, 100)]
    public int randomMineralPercent;

    [Range(0, 10)]
    public int smoothness;

    public GameObject tempMineral;

    int[,] map;

    public void GenerateMap(){
        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < smoothness; i++)
        {
            SmoothMap(i);
        }
        for (int i = 1; i < smoothness+1; i++)
        {
            SmoothMapZ(i);
        }
        AverageZ();

        CreateMinerals();

        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(map, 1f);

    }
    void CreateMinerals()
    {
        System.Random pseudoRandomNum = new System.Random(seed.GetHashCode());
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] >= 2)
                {
                    if (pseudoRandomNum.Next(0, 100) < randomMineralPercent)
                    {
                        Instantiate(tempMineral, new Vector3(-width + x + .5f, map[x, y], -height + y + .5f), Quaternion.identity);
                    }
                }
            }
        }
    }

    public void RandomFillMap()
    {
        if (useRandomSeed)
        {
            seed = Time.time.ToString();
        }
        System.Random pseudoRandomNum = new System.Random(seed.GetHashCode());
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (pseudoRandomNum.Next(0, 100) < randomFillPercent)
                {
                    map[x, y] = 1;
                } else
                {
                    map[x, y] = 0;
                }
            }
        }
    }

    void SmoothMap(int timeNum)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    map[x, y] = 0;
            }
        }
    }

    void SmoothMapZ(int timeNum)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);
                if (map[x, y] == 1)
                {
                    if (neighbourWallTiles >= 6)
                        map[x, y] = Mathf.RoundToInt(Mathf.Clamp((neighbourWallTiles / 4f) + Random.Range(-1, 1), 1, timeNum + 1));
                }
            }
        }
    }

    void AverageZ()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 1)
                {
                    int neighbourWallTiles = GetSurroundingWallZ(x, y);
                    map[x, y] = Mathf.RoundToInt(neighbourWallTiles / GetSurroundingWallCount(x, y));
                }
            }
        }
    }

    int GetSurroundingWallZ(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
            }
        }

        return wallCount;
    }
    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        if (map[neighbourX,neighbourY] != 0)
                            wallCount += 1;
                    }
                }
            }
        }
        return wallCount;
        
    }
    void OnDrawGizmos()
    {
        if (map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Gizmos.color = (map[x, y] >= 1) ? Color.black : Color.white;
                    Vector3 pos = new Vector3(-width / 2 + x + .5f, -height / 2 + y + .5f, map[x, y]);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }

}
