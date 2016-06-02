using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// MapGen - creates a single new asteroid map array - uses MeshGen
///     
/// Theoretical Structure of Asteroid generation : 
/// 1.  Game overall generation (how many levels and where they are in ref to the sun)
/// 2.  Level generation (things that would be in a level; asteroids, enemies, stations, items etc)
/// 3.  Specific Generators : 
///     Asteroid Generator :
///         Classes to Call :
///             GameObject Generator : type Asteroid (get mineral specifics from Level and hold them in an array and add physics to the object) 
///             MapGen (create a 2D binary map for the asteroid outline and decided minerals from GOgen)
///             MeshGen (create a 3D mesh from the map and add destructability)
///             (MinGen - maybe create a specific class for minerals if things get too complex)
///             MiniMeshGen (create detials in the mesh; divits, holes, other asteroidical features)
///             MatGen  (create and apply materials to the asteroids based on level)
///             GameObject Saver (binary formatter that stores information about each object to feed to level)
///             (Once these are all called this should be fed into a scriptable object class and derive from Monobehavoir)
///         May need to add a new script just for handling rotation and giving the section hit back to MeshGen
///     
///             
///         
/// </summary>


public class MapGenerator : MonoBehaviour {

    public int width;               // start width of the map to use
    public int height;              // start height
    [Range(0, 10)]
    public int zThickness;          // the maximum thickness of the z axis (this gets doubled)
    public string seed;             // seed number for creating a set hash
    public bool useRandomSeed;      // bool for using Time.time to create the map
    public bool GenerateMesh;       // bool for debug, used to create the map, but not the mesh
    public bool ShowGizmoDelete;    // bool for debug, uses cooroutine to show maps that fail
    public bool ShowGizmos;         // bool for debug, use to show gizmos for the map (not the mesh)
    private int failCount = 0;      // keeps track of the number of times the current settings are used before a good asteroid is made

    [Range(0,100)]
    public int randomFillPercent;   // how much noise to generate on the map, best results are occuring at 50% as of right now

    [Range(0, 100)]
    public int randomMineralPercent;// percent of the asteroid that should be minerals

    [Range(0, 10)]
    public int smoothness;          // number of iterations to use the smoothing algorithm for

    public int minSizeTiles;        // minimum number of tiles that an asteroid must contain to pass

    public GameObject tempMineral;  // prefab for input ore  TODO : make this an array[] that is fed by the level generator
    List<Mineral> minerals;         // holder for the minerals generated on the asteroid (class contains 3 int positions and GO type)

    MeshGenerator meshGen;          // ref to the mesh side of the generator

    DisplayController dispCont;     // temp ref that holds a score for minerals destroyed
                                    // NOTE : use this structure later for generating GOs and passing detials

    int[,] map;                     // holds the z height of asteroid on an x,y grid  0 = no asteroid
    int[,] delMap;                  // temp object for gizmos, holds the deleted regions for display purposes
    
    // NOTE : Minerals need a specific place, copying the map will work, but better to make a class for them - DONE!
    public int[,] mineralMap;       // binary x,y array that holds minerals prior to creation

    // main funtion of the MapGen - Calling this will clear the board and create a single new asteroid and mesh
    public void GenerateMap() {
        map = new int[width, height];
        delMap = new int[width, height];

        CameraFocus();              // move the camera to hold the whole grid in view - temporary

        if (meshGen != null)        // clear out old mesh if regenerating
        {
            ClearMinerals();
            meshGen.ClearMesh();
        }
        else                        // make new references if mesh is not here
        {
            meshGen = GetComponent<MeshGenerator>();
            dispCont = GameObject.FindObjectOfType<DisplayController>();
        }

        RandomFillMap();                        // create a noise map based on seed

        for (int i = 0; i < smoothness; i++)    // uses a neighboring algorithm to smooth the noise map into a ball shape
        {
            SmoothMap(i);                       // i = iterations of smoothing
        }

        // NOTE : this calls finish map after the cooroutine (done to test gizmos for smoothmap and process map);
        // TODO : change this when you need to make the code normal agian
        StartCoroutine( ProcessMap());          // temporary IEnumerator code that holds gizmo functionality for educational purposes
    }
    // Called after map passes requirements and adds 3D arcitecture
    public void FinishMap() { 

        for (int i = 1; i < zThickness + 1; i++)    // Increases z levels slowly and evenly
        {
            SmoothMapZ(i);                          // i = iteration of smoothing and therefore height
        }
        AverageZ();                                 // makes sure the z isn't whacky looking

        if (GenerateMesh)                           // if not in debug mode it create minerals for the map
            CreateMinerals();

        if (GenerateMesh)                           // MeshGen - See the MeshGenerator.cs script
            meshGen.OriginateMesh(map, 1f);

    }

    void ClearMinerals()
    {
        if (minerals != null)                       // if the list is not empty.....
        {
            if (minerals.Count > 0)                 // and it has items in it....
            {
                foreach (Mineral Go in minerals)    // for every gameObject....
                {
                    Destroy(Go.ore);                // destroy the gameObject
                }
            }
            minerals.Clear();                       // After the GOs are gone then you can clear the list
        }
        mineralMap = new int[width, height];        // make a new empty map that is the same size as the base map 
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
        // NOTE : create a code here that looks for and deletes exisiting prefabs - taken care of in ClearMinerals() - Done

        if (minerals != null)                       // if there are minerals get rid of them
        {
            ClearMinerals();
        }

        minerals = new List<Mineral>();             // create a new list for minerals
        if (mineralMap == null)                     // this gets done in ClearMinerals() - but it needs to be here to for first round
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

        System.Random pseudoRandomNum = new System.Random(seed.GetHashCode());      // retrieve the same seed used for map
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] >= 2)                                                 // check to make minerals in middle
                {
                    if (pseudoRandomNum.Next(0, 100) < randomMineralPercent)        // take the seed from map and apply to asteroid
                    {
                        GameObject ore = Instantiate(tempMineral, new Vector3(-width + x + .5f, map[x, y], -height + y + .5f), Quaternion.identity) as GameObject;
                        // create a new ore object that is in the middle of a square
                        ore.transform.localScale = new Vector3(Random.Range(0.5f, 2), Random.Range(0.5f, 2), Random.Range(0.5f, 2));
                        // randomize the shape, just because it looks more 'organic'
                        minerals.Add(new Mineral(x,y,map[x,y], ore));
                        // put the new object into the list using the Mineral() class - check it for details
                        mineralMap[x, y] = 1;
                        // mineralMap is marked to show a new ore
                    }
                }
            }
        }
    }
    // Part of MeshGen's destruction update - TODO : move this onto the standalone object for asteroid
    // takes in where the mineral was destroyed at and removes it from the list
    // could be optimized greatly by learning more about the .contains function from list
    public void MineralBreak(int x, int y)
    {
        Mineral listIndex = null;           // empty Mineral() to ref for list removal
        foreach(Mineral m in minerals)
        {
            //Debug.Log("called for break at X : " + x + " Y : " + y);
            //Debug.Log("mineral X : " + m.x + " mineral Y : " + m.y);
            if (m.x == x && m.y == y)
            {
                dispCont.MineralPointsUp(1);// tell the counter to go up one
                // NOTE : the functionallity for creating an 'ore' object should go here
                listIndex = m;              // fill empty ref with the right one
                Destroy(m.ore);             // destroy it...
            }
        }
        if (listIndex != null)
        {
            minerals.Remove(listIndex);     //....and take it out of the list
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

    // Function to make the noise map congruent
    // takes in an int for which iteration it is, called from GenerateMap()
    void SmoothMap(int timeNum)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y); // call a function that checks how many array items are around it

                if (neighbourWallTiles > 4)             // when there are a number of active grid items around it
                    map[x, y] = 1;                      // ...makes a new grid item 
                else if (neighbourWallTiles < 4)        // but if there are too few
                    map[x, y] = 0;                      // then it clears the area
                // this is the core of how the shape is built, Use of purlion noise would be best for larger shapes
                // purloin noise would add more organic value to the shape, but, it would be more difficult to seed
            }
        }
    }
    // Creates the Z depth of the asteroid
    // takes in an int for which iteration it is, called from GenerateMap() - ProcessMap() for right now though
    void SmoothMapZ(int timeNum)
    {
        int randZ = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y); // store the number of surrounding tiles
                if (map[x, y] >= 1)                                     // check to make sure this tile is not empty
                {
                    if (neighbourWallTiles == 8)                                                    // if it is a center tile...
                    {
                        randZ = Mathf.RoundToInt(2 + Random.Range(-1, 2));                          // ...give it a random z value
                        //Debug.Log(randZ.ToString() + " : randZ  " + neighbourWallTiles.ToString() + " : wall tiles");
                        map[x, y] = Mathf.RoundToInt(Mathf.Clamp(randZ, map[x, y], timeNum + 1));   // keep the z between current and max
                    } else if (neighbourWallTiles >= 5)                                             // if it is an edge tile...
                    {
                        randZ = Mathf.RoundToInt((neighbourWallTiles / 4f) + Random.Range(-1, 2));  // ...make it cound near edges
                        //Debug.Log(randZ.ToString() + " : randZ  " + neighbourWallTiles.ToString() + " : wall tiles");
                        map[x, y] = Mathf.RoundToInt(Mathf.Clamp(randZ, 1, timeNum + 1));           // keep it under the max and above 1
                    }
                }
            }
        }
    }
    // rounds the existing Zs based off of there neighbor Zs
    void AverageZ()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] >= 1)         // if the map is not empty here....
                {
                    int neighbourWallZs = GetSurroundingWallZ(x, y);        // get the height of the Zs near it
                    int neighbourWallTiles = GetSurroundingWallCount(x, y); // get the number of filled tiles near it
                    if (neighbourWallTiles == 8)                            // if there it 
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
