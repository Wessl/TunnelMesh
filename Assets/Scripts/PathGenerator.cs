using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;


[RequireComponent(typeof(MeshFilter))]
public class PathGenerator : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float incAmount;   // How many units long will each tunnel segment be
    [Tooltip("How many times do you want to subdivide the outer mesh (more = higher fidelity)")]
    [SerializeField] private int meshFidelity;
    [SerializeField] private int tunnelLength;
    [SerializeField] private float tunnelWidth;
    
    private float tunnelWidthCurrent;
    private Vector3[][] circlesToPoints;    // Every "circle" needs to be mapped to an array of points that make up that circle
    private List<Vector3> points;           // The points to use for drawing lines with the line renderer
    private List<Vector3> vertices;         // A convenient list that contains every point that makes up every circle in order
    private List<int> triangles;            // The definition of every triangle that will make up the mesh
    private Mesh mesh;

    // Must be called first of all before generating a path
    private void Setup()
    {
        tunnelWidthCurrent = tunnelWidth;
        
        // Make a new list that has a default first line segment that is just from the origin straight forward
        points = new List<Vector3>();
        points.Add(new Vector3(0,0,0));
        points.Add(new Vector3(incAmount,0,0));
        vertices = new List<Vector3>();
        triangles = new List<int>();
        
        SetLine();
        
        // set up mesh
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // There is one "circle" in 2d space per tunnelLength, each of which has a set amount of sides (4,8,16...)
        circlesToPoints = new Vector3[tunnelLength-1][];
        for (int i = 0; i < tunnelLength-1; i++)
        {
            circlesToPoints[i] = new Vector3[GetTunnelFrontViewEdgeAmount()];
        }
    }


    // Entry method
    public void GeneratePath()
    {
        // Need to get a random seed or else the same map will be created every time the player restarts for example
        Random.InitState((int)DateTime.Now.Ticks);      

        Setup();
        // Create the line segments that will define the center of the tunnel,
        // by getting a new point incAmount forwards and a random point on the YZ plane some distance from the current center
        for (int x = 1; x < tunnelLength; x++)
        {
            Vector3 newPoint = (points[x] + new Vector3(incAmount, Random.Range(-incAmount*1.4f, incAmount*1.4f), Random.Range(-incAmount*1.4f, incAmount*1.4f)));
            points.Add(newPoint);
        }
        SetLine();
        // Create every "circle" along each line segment
        CreateOutsideMeshPoints();
        // Now we have a line, with points on each midsegment extending out to form a circle. Now connect them. 
        CreateMesh();
    }

    private void CreateOutsideMeshPoints()
    {
        for (int x = 1; x < lineRenderer.positionCount - 1; x++)
        {
            Vector3 origin = lineRenderer.GetPosition(x - 1);
            Vector3 lineBetween = (lineRenderer.GetPosition(x) - origin);
            Vector3 midPoint = lineRenderer.GetPosition(x) - lineBetween.normalized * lineBetween.magnitude / 2;
            Vector3 res = Vector3.Cross(lineBetween, FindPerpendicular(lineBetween));
            List<Vector3> pointsOnCircle = SubdivideMesh(res, lineBetween, midPoint);
            circlesToPoints[x-1] = pointsOnCircle.ToArray();
        }
    }

    private void CreateMesh()
    {
        // First just get all the points because why not idk why im doing this twice anyways
        foreach (var circle in circlesToPoints)
        {
            foreach (var point in circle)
            {
                vertices.Add(point);
            }   
        }

        int rows = tunnelLength-1;
        int cols = GetTunnelFrontViewEdgeAmount();
        for (int x = 0; x < rows-1; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                triangles.Add(y + cols * x);
                triangles.Add(cols * (x+1) + y);
                triangles.Add(cols * (x+1) + (y + 1)%cols);
                triangles.Add(y + cols * x);
                triangles.Add(cols * (x+1) + (y + 1)%cols);
                triangles.Add((y + 1)%cols + cols * x);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        Vector2[] uvs = new Vector2[vertices.Count];

        
        mesh.RecalculateNormals();
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
        mesh.uv = uvs;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
    
    private List<Vector3> SubdivideMesh(Vector3 res, Vector3 lineBetween, Vector3 midPoint)
    {
        List<Vector3> outerPoints = new List<Vector3>();
        tunnelWidthCurrent = GetTunnelWidth();
        // Create four points perpendicular to the lineBetween by utilizing the cross product
        for (int i = 0; i < 4; i++)
        {
            res = Vector3.Cross(lineBetween, res).normalized * tunnelWidthCurrent;
            
            Vector3 outerPoint = midPoint + res;
            outerPoints.Add(outerPoint);
            //Debug.DrawLine(midPoint, outerPoint, Color.black, 5, false);
        }

        if (meshFidelity > 0)
        {
            // If the meshFidelity needs to be higher than just four points, go into recursive function that can do this
            // based on these four points as a starting point
            outerPoints = FurtherMeshDivisions(outerPoints, midPoint, 1);
        }
        return outerPoints;
    }

    private List<Vector3> FurtherMeshDivisions(List<Vector3> outerPoints, Vector3 middle, int iterations)
    {
        List<Vector3> newOuterPoints = new List<Vector3>();
        int loopTimes = (int) Math.Pow(2,iterations+1);
        for (int i = 0; i < loopTimes; i++)
        {
            Vector3 newVec = ((outerPoints[i] + outerPoints[(i+1)%loopTimes])-middle);
            newOuterPoints.Add(outerPoints[i]);
            // Create a normalized length along the vector between the middle and the new point
            var BP = middle + (newVec - middle).normalized * tunnelWidthCurrent;
            newOuterPoints.Add(BP);
            //Debug.DrawLine(middle, BP, Color.yellow, 4);
        }
        iterations++;
        // Recursively call yourself with the old points to increase circlular fidelity
        if (iterations <= meshFidelity)
        {
            newOuterPoints = FurtherMeshDivisions(newOuterPoints, middle, iterations);
        }
        
        return newOuterPoints;
    }

    public int GetTunnelFrontViewEdgeAmount()
    {
        return (int)Math.Pow(2, meshFidelity + 2);
    }

    private float GetTunnelWidth()
    {
        float t = tunnelWidthCurrent += Random.Range(-0.5f, 0.5f);
        if (t < tunnelWidth / 1.25f);
        {
            t = tunnelWidth / 1.25f;
        }
        if (t > tunnelWidth * 1.5f);
        {
            t = tunnelWidth * 1.5f;
        }

        return t;
    }

    // Tells the line renderer how many points there are and what those points are
    private void SetLine()
    {
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    public Mesh Mesh => mesh;

    public int MeshFidelity => meshFidelity;
    
    
    /**
     * https://math.stackexchange.com/questions/133177/finding-a-unit-vector-perpendicular-to-another-vector
     * @param xVec must be a nonzero vector
     */
    private Vector3 FindPerpendicular(Vector3 xVec)
    {
        if (xVec.magnitude.Equals(0))
        {
            throw new Exception("Must not supply nonzero vector to FindPerpendicular!");
        }
        Vector3 v = new Vector3(0, 0, 0);
        int m, n;
        m = n = 0;
        for (int i = 0; i < 3; i++)
        {
            if (xVec[i] != 0)
            {
                m = i;
                n = (i + 1) % 3;
            }
        }

        v[n] = xVec[m];
        v[m] = -xVec[n];
        return v.normalized;
    }

}
