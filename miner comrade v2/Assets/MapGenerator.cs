using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

    public int width;
    public int height;
    [Range(0, 10)]
    public int zThickness;
    public string seed;
    public bool useRandomSeed;
    public bool GenerateMesh;
    public bool ShowGizmoDelete;
    public bool ShowGizmos;
    private int failCount = 0;

    [Range(0,100)]
    public int randomFillPercent;

    [Range(0, 100)]
    public int randomMineralPercent;

    [Range(0, 10)]
    public int smoothness;

    public int minSizeTiles;

    public GameObject tempMineral;
    List<Mineral> minerals;

    MeshGenerator meshGen;
    DisplayController dispCont;

    int[,] map;
    //this is a temp object for gizmos
    int[,] delMap;
    // Minerals need a specific place, copying the map will work, but better to make a class for them
    public int[,] mineralMap;


    public void GenerateMap() {
        map = new int[width, height];
        delMap = new int[width, height];

        CameraFocus();

        if (meshGen != null)
        {
            ClearMinerals();
            meshGen.ClearMesh();
        }
        else
        {
            meshGen = GetComponent<MeshGenerator>();
            dispCont = GameObject.FindObjectOfType<DisplayController>();
        }

        RandomFillMap();

        for (int i = 0; i < smoothness; i++)
        {
            SmoothMap(i);
        }
        // this calls finish map after the cooroutine (done to test gizmos for smoothmap and process map);
        // TODO : change this when you need to make the code normal agian
        StartCoroutine( ProcessMap());
    }
    public void FinishMap() { 

        for (int i = 1; i < zThickness + 1; i++)
        {
            SmoothMapZ(i);
        }
        AverageZ();

        if (GenerateMesh)
            CreateMinerals();

        if (GenerateMesh)
            meshGen.OriginateMesh(map, 1f);

    }

    void ClearMinerals()
    {
        if (minerals != null)
        {
            if (minerals.Count > 0)
            {
                foreach (Mineral Go in minerals)
                {
                    Destroy(Go.ore);
                }
            }
            minerals.Clear();
        }
        
        mineralMap = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                mineralMap[x, y] = 0;
            }
        }
    }

    void CreateMinerals()
    {
        // create a code here that looks for and deletes exisiting prefabs

        if (minerals != null)
        {
            ClearMinerals();
        }

        minerals = new List<Mineral>();
        if (mineralMap == null)
        {
            mineralMap = new int[width,height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    mineralMap[x, y] = 0;
                }
            }
        }

        System.Random pseudoRandomNum = new System.Random(seed.GetHashCode());
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] >= 2)
                {
                    if (pseudoRandomNum.Next(0, 100) < randomMineralPercent)
                    {
                        GameObject ore = Instantiate(tempMineral, new Vector3(-width + x + .5f, map[x, y], -height + y + .5f), Quaternion.identity) as GameObject;
                        ore.transform.localScale = new Vector3(Random.Range(0.5f, 2), Random.Range(0.5f, 2), Random.Range(0.5f, 2));
                        minerals.Add(new Mineral(x,y,map[x,y], ore));
                        mineralMap[x, y] = 1;
                    }
                }
            }
        }
    }

    public void MineralBreak(int x, int y)
    {
        Mineral listIndex = null;
        foreach(Mineral m in minerals)
        {
            //Debug.Log("called for break at X : " + x + " Y : " + y);
            //Debug.Log("mineral X : " + m.x + " mineral Y : " + m.y);
            if (m.x == x && m.y == y)
            {
                dispCont.MineralPointsUp(1);
                listIndex = m;
                Destroy(m.ore);
            }
        }
        if (listIndex != null)
        {
            minerals.Remove(listIndex);
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
        int randZ = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);
                if (map[x, y] >= 1)
                {
                    if (neighbourWallTiles == 8)
                    {
                        randZ = Mathf.RoundToInt(2 + Random.Range(-1, 2));
                        //Debug.Log(randZ.ToString() + " : randZ  " + neighbourWallTiles.ToString() + " : wall tiles");
                        map[x, y] = Mathf.RoundToInt(Mathf.Clamp(randZ, map[x, y], timeNum + 1));
                    } else if (neighbourWallTiles >= 5)
                    {
                        randZ = Mathf.RoundToInt((neighbourWallTiles / 4f) + Random.Range(-1, 2));
                        //Debug.Log(randZ.ToString() + " : randZ  " + neighbourWallTiles.ToString() + " : wall tiles");
                        map[x, y] = Mathf.RoundToInt(Mathf.Clamp(randZ, 1, timeNum + 1));
                    }
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
                if (map[x, y] >= 1)
                {
                    int neighbourWallZs = GetSurroundingWallZ(x, y);
                    int neighbourWallTiles = GetSurroundingWallCount(x, y);
                    if (neighbourWallTiles == 8)
                    {
                        map[x,y] = Mathf.RoundToInt(neighbourWallZs / 6);
                    }
                    else if (neighbourWallTiles != 0)
                    {
                        map[x, y] = Mathf.RoundToInt(neighbourWallZs / (neighbourWallTiles));
                    } else
                    {
                        map[x, y] = 0;
                    }
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

    IEnumerator ProcessMap()
    {
        List<List<Coord>> roidRegions = GetRegions(1);
        List<List<Coord>> failedRegions = new List<List<Coord>>();

        int largestSizeRegion = 0;
        List<Coord> largestRegion = new List<Coord>();

        foreach (List<Coord> roidRegion in roidRegions)
        {
            if(roidRegion.Count <= minSizeTiles)
            {
                yield return StartCoroutine(SlowDeletion(roidRegion));
                failedRegions.Add(roidRegion);
            }else if (roidRegion.Count < largestSizeRegion)
            {
                yield return StartCoroutine(SlowDeletion(roidRegion));
                failedRegions.Add(roidRegion);
            } else
            {
                yield return StartCoroutine(SlowDeletion(largestRegion));
                failedRegions.Add(largestRegion);
                largestSizeRegion = roidRegion.Count;
                largestRegion = roidRegion;
            }
        }
        foreach (List<Coord> region in failedRegions)
        {
            roidRegions.Remove(region);
            // can't do this becuase you can't mod lists when looping them failedRegions.Remove(region);
        }
        
        if (roidRegions.Count > 1) {
            // If for some reason there are multiple reasons, this throws an error and escapes
            Debug.Log("Region Count Error!");
            Debug.Log("Current Regions : " + roidRegions.Count.ToString());
            Debug.DebugBreak();
        }
        if(roidRegions.Count == 1)
        {
            // Finish map is here to test the gizmos drawing of deleting the map
            // this can be moved later when gizmos and the coroutine are no longer needed 
            failCount = 0;
            FinishMap();
        } else if (failCount < 3)
        {
            // didn't make a valid asteroid (count of regions < 1)
            failCount++;
            Debug.Log("Failed to generate valid asteroid time : " + failCount.ToString());
            GenerateMap();
        } else
        {
            //This will be thrown if the asteroid fails to have any valid regions 3 times in a row.
            // pretty much keeps from getting a stackOverflow from the wrong settings getting used.
            // keep this here, though it would be nice to have a new class for generation that will
            // calculate the ranges of invalid asteroids from the width height fill percent and smoothness.
            Debug.Log("Failed to generate valid asteroid with current settings - Change Settings");
        }
    }

    // this is for the gizmo to draw the cubes so that they are visibly erased
    IEnumerator SlowDeletion (List<Coord> roidRegion)
    {
        foreach (Coord tile in roidRegion)
        {
            map[tile.tileX, tile.tileY] = 0;
            delMap[tile.tileX, tile.tileY] = 1;

            //Debug.Log("deleteing tiles : " + tile.tileX.ToString() + "  " + tile.tileY.ToString());
            //yield return null;
            // switch line above with line below to see how gizmo calculates the smaller pieces to delete 
            if (ShowGizmoDelete)
                yield return new WaitForSeconds(0.02f);
        }
        //Debug.Log("roid too small - deleted tiles : " + roidRegion.Count.ToString());
        yield return null;
    }

    List<List<Coord>> GetRegions(int tileType)
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapFlags[x, y] == 0 && map[x,y] == tileType) {
                    List<Coord> newRegion = GetRegionCoordinates(x, y);
                    regions.Add(newRegion);

                    foreach(Coord tile in newRegion)
                    {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }
        return regions;
    }

    List<Coord> GetRegionCoordinates(int startX, int startY)
    {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[width, height];
        int tileType = 0;
        if (map[startX, startY] != 0)
            tileType = 1;

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX, startY] = 1;

        while(queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);
            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (IsInMapRange(x, y) == true && (y==tile.tileY || x == tile.tileX))
                    {
                        try
                        {
                            if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                            {
                                mapFlags[x, y] = 1;
                                queue.Enqueue(new Coord(x, y));
                            }
                        }
                        catch (System.Exception)
                        {
                            Debug.Log("Map is in range? : " + IsInMapRange(x, y).ToString());
                            Debug.Log("StartX : " + startX.ToString() + "StartY : " + startY.ToString());
                            Debug.Log("X : " + x.ToString() + "Y : " + y.ToString());
                            throw;
                        }
                        
                    }
                    //  This fills the space with the occurances of the blocks meeting the 'blank' criteria
                }
            }
        }
        return tiles;
    }

    bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    struct Coord{
        public int tileX;
        public int tileY;

        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
    }

    void OnDrawGizmos()
    {
        if (map != null  &&  ShowGizmos)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
                    Vector3 pos = new Vector3(-width + x + .5f, map[x, y] - 2, - height + y + .5f);
                    if (map[x,y] > 1)
                        Gizmos.color = Color.grey;
                    Gizmos.DrawCube(pos, Vector3.one);
                    // shows deleted roids
                    if (delMap[x,y] == 1)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawCube(pos, Vector3.one);
                    }
                }
            }
        }
    }
    void CameraFocus()
    {
        Camera cam = GameObject.FindObjectOfType<Camera>();
        int pickLarger = 0;
        if (width >= height)
        {
            pickLarger = width;
        } else
        {
            pickLarger = height;
        }
        cam.orthographicSize = pickLarger/2;
        cam.transform.position = new Vector3(-pickLarger / 2, 10f, -pickLarger / 2);

    }
    public class Mineral
    {
        public int x;
        public int y;
        public int oreHeight;
        public GameObject ore;
        public Mineral(int xIn, int yIn, int height, GameObject GO )
        {
            x = xIn;
            y = yIn;
            oreHeight = height;
            ore = GO;
        }

    }

}
