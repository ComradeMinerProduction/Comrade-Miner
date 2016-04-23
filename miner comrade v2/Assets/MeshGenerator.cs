using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshGenerator : MonoBehaviour {

    public SquareGrid squareGrid;
    public List<int> triangles;
    public List<Vector3> vertices;

    public void GenerateMesh(int[,] map, float squareSize)
    {

        triangles = new List<int>();
        vertices = new List<Vector3>();

        squareGrid = new SquareGrid(map, squareSize);

        for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
        {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
            {
                TriangulateSquare(squareGrid.squares[x, y]);
            }
        }

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

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
        if(square.configuration != 0)
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
            }
        }
    }

    void CreateTriangle(Node a, Node b, Node c)
    {
        triangles.Add(a.vertexIndex);
        triangles.Add(b.vertexIndex);
        triangles.Add(c.vertexIndex);
    }

    //void OnDrawGizmos()
    //{
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
    //}

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
                    Vector3 pos = new Vector3(-mapWidth + x * squareSize + squareSize / 2f, map[x,y],
                        -mapHeight + y * squareSize + squareSize / 2f);
                    controlNode[x, y] = new ControlNode(map[x, y] >= 1, pos, squareSize);
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

        public Node (Vector3 _pos)
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
