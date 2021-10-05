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
    public LineRenderer lineRenderer;
    public bool autoUpdate = true;
    [Tooltip("The points to use for the linerenderers' points. ")]
    private List<Vector3> points;
    public float incAmount = 10;
    [Tooltip("How many times do you want to subdivide the outer mesh (more = higher fidelity)")]
    [SerializeField] private int meshFidelity;
    [SerializeField] private int tunnelLength;
    [SerializeField] private float tunnelWidth;
    private float tunnelWidthCurrent;

    private Vector3[][] circlesToPoints;

    private List<Vector3> vertices;
    private List<int> triangles;

    private Mesh mesh;

    private void Setup()
    {
        tunnelWidthCurrent = tunnelWidth;
        points = new List<Vector3>();
        points.Add(new Vector3(0,0,0));
        points.Add(new Vector3(incAmount,0,0));
        SetLine();
        // set up mesh
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        vertices = new List<Vector3>();
        triangles = new List<int>();
        // There is one "circle" in 2d space per tunnelLength, each of which has a set amount of sides (4,8,16...)
        circlesToPoints = new Vector3[tunnelLength-1][];
        for (int i = 0; i < tunnelLength-1; i++)
        {
            circlesToPoints[i] = new Vector3[GetTunnelFrontViewEdgeAmount()];
        }
    }

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

    // Entry method
    public void GeneratePath()
    {
        Random.InitState((int)DateTime.Now.Ticks);      // Need to get a random seed or else the same map will be created every time

        Setup();
        Debug.Log("Random state: " + Random.state.ToString());
        for (int x = 1; x < tunnelLength; x++)
        {
            Vector3 newPoint = (points[x] + new Vector3(incAmount, Random.Range(-incAmount*1.4f, incAmount*1.4f), Random.Range(-incAmount*1.5f, incAmount*1.5f)));
            points.Add(newPoint);
        }
        SetLine();
        CreateOutsideMeshPoints();
        
    }

    private void CreateOutsideMeshPoints()
    {
        // start with just trying to create a line perpendicular to each line segment
        for (int x = 1; x < lineRenderer.positionCount - 1; x++)
        {
            Vector3 origin = lineRenderer.GetPosition(x - 1);
            Vector3 lineBetween = (lineRenderer.GetPosition(x) - origin);
            Vector3 midPoint = lineRenderer.GetPosition(x) - lineBetween.normalized * lineBetween.magnitude / 2;
            Vector3 res = Vector3.Cross(lineBetween, FindPerpendicular(lineBetween));
            List<Vector3> pointsOnCircle = SubdivideMesh(origin, res, lineBetween, midPoint);
            circlesToPoints[x-1] = pointsOnCircle.ToArray();
        }
        // Now we have a line, with points on each midsegment extending out to form a circle. Now connect them. 
        CreateMesh();
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

    private List<Vector3> SubdivideMesh(Vector3 origin, Vector3 res, Vector3 lineBetween, Vector3 midPoint)
    {
        List<Vector3> outerPoints = new List<Vector3>();
        tunnelWidthCurrent = GetTunnelWidth();
        for (int i = 0; i < 4; i++)
        {
            res = Vector3.Cross(lineBetween, res).normalized * tunnelWidthCurrent;
            
            Vector3 outerPoint = midPoint + res;
            outerPoints.Add(outerPoint);
            //Debug.DrawLine(midPoint, outerPoint, Color.black, 5, false);
        }

        if (meshFidelity > 0)
        {
            // instead of this, cache the old vectors and loop through them and create intermediary
            outerPoints = FurtherMeshDivisions(lineBetween, outerPoints, midPoint, 1);
        }
        return outerPoints;
    }

    private List<Vector3> FurtherMeshDivisions(Vector3 lineBetween, List<Vector3> outerPoints, Vector3 middle, int iterations)
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
            newOuterPoints = FurtherMeshDivisions(lineBetween, newOuterPoints, middle, iterations);
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
        if (t < tunnelWidth / 1.5f);
        {
            t = tunnelWidth / 1.5f;
        }
        if (t > tunnelWidth * 1.5f);
        {
            t = tunnelWidth * 1.5f;
        }

        return t;
    }

    private void SetLine()
    {
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    public Mesh Mesh => mesh;

}
