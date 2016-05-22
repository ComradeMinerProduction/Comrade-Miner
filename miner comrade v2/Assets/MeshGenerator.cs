using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshGenerator : MonoBehaviour
{

    public SquareGrid squareGrid;
    public SquareGrid negSquareGrid;
    public List<int> triangles;
    public List<Vector3> vertices;
    public List<int[,]> pseudoMap;

    private bool mouseDown;
    private float roundX;
    private float roundZ;
    private int localX;
    private int localZ;
    private Vector3 rawMouse;
    private Queue<int> alterVerts = new Queue<int>();
    private bool altered = false;

    public int[,] currentMap;

    private Mesh mesh;
    private MeshCollider MC;

    public void FixedUpdate()
    {
        if (Input.GetMouseButton(0) && currentMap != null)
        {
            mouseDown = true;
            rawMouse = Input.mousePosition;
            rawMouse = Camera.main.ScreenToWorldPoint(rawMouse);
            roundX = Mathf.Round(rawMouse.x);
            roundZ = Mathf.Round(rawMouse.z);

            localX = Mathf.RoundToInt(roundX + currentMap.GetLength(0));
            localZ = Mathf.RoundToInt(roundZ + currentMap.GetLength(1));

            if (localX >= 0 && localX < currentMap.GetLength(0) && localZ >= 0 && localZ < currentMap.GetLength(1))
            {
                if (currentMap[localX, localZ] != 0)
                {
                    currentMap[localX, localZ] = 0;
                }
            }
            foreach(Vector3 vert in vertices)
            {
                if (vert.x == roundX || vert.x == roundX + .5f)
                {
                    if (vert.z == roundZ || vert.z == roundZ + .5f)
                    {
                        if (vert.y != 0)
                            alterVerts.Enqueue(vertices.IndexOf(vert));
                    }
                }
            }

            //GetTriFromRay();

        } else
        {
            mouseDown = false;
        }
        while (alterVerts.Count > 0  && !altered)
        {
            int alterVert = alterVerts.Dequeue();
            if (vertices[alterVert] != null)
                vertices[alterVert] = new Vector3(vertices[alterVert].x, 0, vertices[alterVert].z);
            // add this to a que so that it doesn't get called five times in a row.
            //GetTriangleFromVertex(alterVert);
            //triangles.RemoveAll(delegate (int i) {return i == alterVert;});
            altered = true;
        }
        if (altered)
        {
            mesh.vertices = vertices.ToArray();
            //mesh.triangles = triangles.ToArray();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            
            // Below is the current working version of the deletable roid
            // There could be a better way of doing this, as this 'feels' like
            // a really expensive way of changing the object
            GenerateMesh(currentMap, 1);

            // alternativly the mesh could be checked for triangles where the verticies all go to zero
            // this would ensure flat edges, for now it still leaves one up high without a wall function

        }
        altered = false;
        /// there is another way to do this - using raycast to get the triangles, four of them around the point
        
    }

    void GetTriangleFromVertex(int target)
    {
        int iTwo = -1;
        int iThree = -1;
        Queue<int[]> TriQue = new Queue<int[]>();
        for (int i = 0; i < triangles.Count; i += 3)
        {
            iTwo = i + 1;
            iThree = i + 2;
            if (i == target || iTwo == target || iThree == target)
            {
                int[] j = { i, iTwo, iThree };
                TriQue.Enqueue(j);
            }
        }
        while(TriQue.Count > 0)
        {
            int[] i = TriQue.Dequeue();
            Debug.Log("Looking for triangle : " + i[0] + " " + i[1] + " " + i[2]);
            triangles.Remove(i[0]);
            triangles.Remove(i[1]);
            triangles.Remove(i[2]);
        }
    }

    void GetTriFromRay()
    {
        RaycastHit hit;
        Debug.DrawRay(new Vector3(roundX, 10f, roundZ), Vector3.down * 10, Color.green, 2.0f);
        if (Physics.Raycast(new Vector3(roundX, 10f, roundZ), Vector3.down * 10, out hit))
        {
            Debug.Log("hit a mesh");
            int triIndex = hit.triangleIndex;
            Debug.Log(mesh.triangles[triIndex]);
        }
        //Debug.Log("Mouse Position : " + roundX.ToString() + "  " + roundZ.ToString());
    }
    // Dictionary<int,string> SqaureLib = new Dictionary<int,string>();

    public void OriginateMesh(int[,] map, float squareSize)
    {
        triangles = new List<int>();
        vertices = new List<Vector3>();
        currentMap = CopyMap(map);

        int[,] negMap = CreateNegativeMap(map);

        negSquareGrid = new SquareGrid(negMap, squareSize);

        squareGrid = new SquareGrid(map, squareSize);

        for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
        {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
            {
                TriangulateSquare(squareGrid.squares[x, y]);
            }
        }
        for (int x = negSquareGrid.squares.GetLength(0) - 1; x >= 0; x--)
        {
            for (int y = negSquareGrid.squares.GetLength(1) - 1; y >= 0; y--)
            {
                TriangulateRevSquare(negSquareGrid.squares[x, y]);
            }
        }

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    public void GenerateMesh(int[,] map, float squareSize)
    {

        triangles = new List<int>();
        vertices = new List<Vector3>();

        int[,] negMap = CreateNegativeMap(map);

        negSquareGrid = new SquareGrid(negMap, squareSize);

        squareGrid = new SquareGrid(map, squareSize);

        for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
        {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
            {
                TriangulateSquare(squareGrid.squares[x, y]);
            }
        }
        for (int x = negSquareGrid.squares.GetLength(0) - 1; x >= 0; x--)
        {
            for (int y = negSquareGrid.squares.GetLength(1) - 1; y >= 0; y--)
            {
                TriangulateRevSquare(negSquareGrid.squares[x, y]);
            }
        }

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();


        // This goes with getTrisFromRay()
        // found to be excessivly expensive - though may be neat to learn from
        //MC = transform.gameObject.AddComponent<MeshCollider>();
        //MC.sharedMesh = mesh;

    }

    public void ClearMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    int[,] CopyMap(int[,] map)
    {
        int tempX = map.GetLength(0);
        int tempY = map.GetLength(1);
        int[,] mapCopy = new int[tempX, tempY];
        for (int x = 0; x < tempX; x++)
        {
            for (int y = 0; y < tempY; y++)
            {
                mapCopy[x, y] = map[x, y];
            }
        }
        return mapCopy;
    }

    public int[,] CreateNegativeMap(int[,] map)
    {
        int tempX = map.GetLength(0);
        int tempY = map.GetLength(1);
        int[,] negMap = new int[tempX, tempY];
        for (int x = 0; x < tempX; x++)
        {
            for (int y = 0; y < tempY; y++)
            {
                if (map[x, y] != 0)
                {
                    negMap[x, y] = -map[x, y];
                }
            }
        }
        return negMap;
    }


    // this may need to be run in the generator
    int GetCongruentSquare(int[,] map)
    {
        int linkedSquares = 0;
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; x < map.GetLength(1); y++)
            {
                // find nodes going left and nodes going right
                // put total X + total Y into Largest value if
                // x + y is greater than previous or the starting value
                // reroll the map if there is no value greater than min size
            }
        }

        return linkedSquares;
    }

    void TriangulateSquare(Square square)
    {
        switch (square.configuration)
        {
            case 0:
                break;

            // 1 points:
            case 1:
                MeshFromPoints(square.middleBottom, square.bottomLeft, square.middleLeft);
                break;
            case 2:
                MeshFromPoints(square.middleRight, square.bottomRight, square.middleBottom);
                break;
            case 4:
                MeshFromPoints(square.middleTop, square.topRight, square.middleRight);
                break;
            case 8:
                MeshFromPoints(square.topLeft, square.middleTop, square.middleLeft);
                break;

            // 2 points:
            case 3:
                MeshFromPoints(square.middleRight, square.bottomRight, square.bottomLeft, square.middleLeft);
                break;
            case 6:
                MeshFromPoints(square.middleTop, square.topRight, square.bottomRight, square.middleBottom);
                break;
            case 9:
                MeshFromPoints(square.topLeft, square.middleTop, square.middleBottom, square.bottomLeft);
                break;
            case 12:
                MeshFromPoints(square.topLeft, square.topRight, square.middleRight, square.middleLeft);
                break;
            case 5:
                MeshFromPoints(square.middleTop, square.topRight, square.middleRight, square.middleBottom, square.bottomLeft, square.middleLeft);
                break;
            case 10:
                MeshFromPoints(square.topLeft, square.middleTop, square.middleRight, square.bottomRight, square.middleBottom, square.middleLeft);
                break;

            // 3 point:
            case 7:
                MeshFromPoints(square.middleTop, square.topRight, square.bottomRight, square.bottomLeft, square.middleLeft);
                break;
            case 11:
                MeshFromPoints(square.topLeft, square.middleTop, square.middleRight, square.bottomRight, square.bottomLeft);
                break;
            case 13:
                MeshFromPoints(square.topLeft, square.topRight, square.middleRight, square.middleBottom, square.bottomLeft);
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.middleBottom, square.middleLeft);
                break;

            // 4 point:
            case 15:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
                break;
        }
        if (square.configuration != 0)
        {
            /// create a hit box here based on the type of material

        }
    }

    void TriangulateRevSquare(Square square)
    {
        switch (square.configuration)
        {
            case 0:
                break;

            // 1 points:
            case 1:
                MeshFromPoints(square.middleBottom, square.middleLeft, square.bottomLeft);
                break;
            case 2:
                MeshFromPoints(square.middleRight, square.middleBottom, square.bottomRight);
                break;
            case 4:
                MeshFromPoints(square.middleTop, square.middleRight, square.topRight);
                break;
            case 8:
                MeshFromPoints(square.topLeft, square.middleLeft, square.middleTop);
                break;

            // 2 points:
            case 3:
                MeshFromPoints(square.middleRight, square.middleLeft, square.bottomLeft, square.bottomRight);
                break;
            case 6:
                MeshFromPoints(square.middleTop, square.middleBottom, square.bottomRight, square.topRight);
                break;
            case 9:
                MeshFromPoints(square.topLeft, square.bottomLeft, square.middleBottom, square.middleTop);
                break;
            case 12:
                MeshFromPoints(square.topLeft, square.middleLeft, square.middleRight, square.topRight);
                break;
            case 5:
                MeshFromPoints(square.middleTop, square.middleLeft, square.bottomLeft, square.middleBottom, square.middleRight, square.topRight);
                break;
            case 10:
                MeshFromPoints(square.topLeft, square.middleLeft, square.middleBottom, square.bottomRight, square.middleRight, square.middleTop);
                break;

            // 3 point:
            case 7:
                MeshFromPoints(square.middleTop, square.middleLeft, square.bottomLeft, square.bottomRight, square.topRight);
                break;
            case 11:
                MeshFromPoints(square.topLeft, square.bottomLeft, square.bottomRight, square.middleRight, square.middleTop);
                break;
            case 13:
                MeshFromPoints(square.topLeft, square.bottomLeft, square.middleBottom, square.middleRight, square.topRight);
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.middleLeft, square.middleBottom, square.bottomRight, square.topRight);
                break;

            // 4 point:
            case 15:
                MeshFromPoints(square.topRight, square.topLeft, square.bottomLeft, square.bottomRight);
                break;
        }
        if (square.configuration != 0)
        {
            /// create a hit box here based on the type of material

        }
    }

    void MeshFromPoints(params Node[] points)
    {
        AssignVertices(points);

        if (points.Length >= 3)
            CreateTriangle(points[0], points[1], points[2]);
        if (points.Length >= 4)
            CreateTriangle(points[0], points[2], points[3]);
        if (points.Length >= 5)
            CreateTriangle(points[0], points[3], points[4]);
        if (points.Length >= 6)
            CreateTriangle(points[0], points[4], points[5]);
    }

    void AssignVertices(Node[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].vertexIndex == -1)
            {
                points[i].vertexIndex = vertices.Count;
                vertices.Add(points[i].position);
                //vertices.Add(new Vector3(points[i].position.x, points[i].position.y, -points[i].position.z));
            }
        }
    }

    void AssignNegativeVertices(Node[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].vertexIndex == -1)
            {
                points[i].vertexIndex = vertices.Count;
                vertices.Add(new Vector3(points[i].position.x, points[i].position.y, -points[i].position.z));
            }
        }
    }

    void CreateTriangle(Node a, Node b, Node c)
    {
        triangles.Add(a.vertexIndex);
        triangles.Add(b.vertexIndex);
        triangles.Add(c.vertexIndex);
    }

    void OnDrawGizmos()
    {
        if (mouseDown)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(new Vector3(roundX + .5f, 1, roundZ + .5f), Vector3.one);
        } else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(roundX + .5f, 1, roundZ + .5f), Vector3.one);
        }
    //    if (squareGrid != null)
    //    {
    //        for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
    //        {
    //            for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
    //            {

    //                Gizmos.color = (squareGrid.squares[x, y].topLeft.active) ? Color.black : Color.white;
    //                Gizmos.DrawCube(squareGrid.squares[x, y].topLeft.position, Vector3.one * .4f);

    //                Gizmos.color = (squareGrid.squares[x, y].topRight.active) ? Color.black : Color.white;
    //                Gizmos.DrawCube(squareGrid.squares[x, y].topRight.position, Vector3.one * .4f);

    //                Gizmos.color = (squareGrid.squares[x, y].bottomRight.active) ? Color.black : Color.white;
    //                Gizmos.DrawCube(squareGrid.squares[x, y].bottomRight.position, Vector3.one * .4f);

    //                Gizmos.color = (squareGrid.squares[x, y].bottomLeft.active) ? Color.black : Color.white;
    //                Gizmos.DrawCube(squareGrid.squares[x, y].bottomLeft.position, Vector3.one * .4f);


    //                Gizmos.color = Color.grey;
    //                Gizmos.DrawCube(squareGrid.squares[x, y].middleTop.position, Vector3.one * .15f);
    //                Gizmos.DrawCube(squareGrid.squares[x, y].middleRight.position, Vector3.one * .15f);
    //                Gizmos.DrawCube(squareGrid.squares[x, y].middleBottom.position, Vector3.one * .15f);
    //                Gizmos.DrawCube(squareGrid.squares[x, y].middleLeft.position, Vector3.one * .15f);

    //            }
    //        }
    //    }
    }

    public class SquareGrid
    {
        public Square[,] squares;

        public SquareGrid(int[,] map, float squareSize)
        {
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            ControlNode[,] controlNode = new ControlNode[nodeCountX, nodeCountY];

            for (int x = 0; x < nodeCountX; x++)
            {
                for (int y = 0; y < nodeCountY; y++)
                {
                    Vector3 pos = new Vector3(-mapWidth + x * squareSize + squareSize / 2f, map[x, y],
                        -mapHeight + y * squareSize + squareSize / 2f);
                    controlNode[x, y] = new ControlNode(map[x, y] != 0, pos, squareSize);
                }
            }
            squares = new Square[nodeCountX - 1, nodeCountY - 1];
            for (int x = 0; x < nodeCountX - 1; x++)
            {
                for (int y = 0; y < nodeCountY - 1; y++)
                {
                    squares[x, y] = new Square(controlNode[x, y + 1], controlNode[x + 1, y + 1], controlNode[x + 1, y], controlNode[x, y]);
                }
            }
        }
    }

    public class Square
    {
        public ControlNode topLeft, topRight, bottomLeft, bottomRight;
        public Node middleTop, middleRight, middleBottom, middleLeft;
        public int configuration;

        public Square(ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft)
        {
            topLeft = _topLeft;
            topRight = _topRight;
            bottomLeft = _bottomLeft;
            bottomRight = _bottomRight;

            middleTop = topLeft.rightNode;
            middleRight = bottomRight.aboveNode;
            middleBottom = bottomLeft.rightNode;
            middleLeft = bottomLeft.aboveNode;

            if (topLeft.active)
                configuration += 8;
            if (topRight.active)
                configuration += 4;
            if (bottomLeft.active)
                configuration += 1;
            if (bottomRight.active)
                configuration += 2;

        }
    }

    public class Node
    {
        public Vector3 position;
        public int vertexIndex = -1;

        public Node(Vector3 _pos)
        {
            position = _pos;
        }
    }
    public class ControlNode : Node
    {
        public bool active;
        public Node aboveNode, rightNode;

        public ControlNode(bool _active, Vector3 _pos, float squareSize) : base(_pos)
        {
            active = _active;
            aboveNode = new Node(position + Vector3.forward * squareSize / 2f);
            rightNode = new Node(position + Vector3.right * squareSize / 2f);
        }
    }
}
