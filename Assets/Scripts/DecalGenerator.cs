using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class DecalGenerator : MonoBehaviour
{
    public GameObject pipe;
    [SerializeField] private int pipeSpread;
    [SerializeField] private int pipeSpreadRandomRange;
    [SerializeField] private float pipeSizeRandomRange;
    public GameObject lightPipe;
    [SerializeField] private int lightPipeSpread;
    [SerializeField] private int lightPipeRandomRange;
    public GameObject organicBulb;
    [SerializeField] private int bulbSpread;
    [SerializeField] private int bulbRandomRange;

    private int[] triangles;
    private Vector3[] vertices;
    private Vector3[] normals;

    private bool[] filledVertices;


    private Mesh _pathGenMesh;

    public void GenerateDecals()
    {
        triangles = _pathGenMesh.triangles;
        vertices = _pathGenMesh.vertices;
        normals = _pathGenMesh.normals;
        
        filledVertices = new bool[vertices.Length];
        
        RemoveOldDecals();
        PlacePipes();
        PlaceLightPipes();
        PlaceBulbs();
    }

    private void PlacePipes()
    {
        for (int x = 0; x < vertices.Length; x+=pipeSpread)
        {
            x = CheckFilledVertices(x);
            GameObject genPipe = Instantiate(pipe, vertices[x], Quaternion.LookRotation(normals[x]));
            float rndSizeInc = Random.Range(0, pipeSizeRandomRange);
            genPipe.transform.localScale += new Vector3(rndSizeInc,rndSizeInc,rndSizeInc);
            genPipe.transform.Translate(-genPipe.transform.forward * (genPipe.transform.localScale.y * 6), Space.Self);
            genPipe.transform.parent = this.transform;
            filledVertices[x] = true;
            x += Random.Range(0, pipeSpreadRandomRange);
        }
    }

    private void PlaceLightPipes()
    {
        for (int x = 0; x < vertices.Length; x+=lightPipeSpread)
        {
            x = CheckFilledVertices(x);
            GameObject genLight = Instantiate(lightPipe, vertices[x], Quaternion.LookRotation(normals[x]));
            genLight.transform.Translate(genLight.transform.forward * (genLight.transform.localScale.x * 0.7f), Space.Self);
            genLight.transform.Rotate(new Vector3(0,180,0), Space.Self);
            genLight.transform.parent = this.transform;
            filledVertices[x] = true;
            x += Random.Range(0, lightPipeRandomRange);
        }
    }

    private void PlaceBulbs()
    {
        for (int x = 0; x < vertices.Length; x+=lightPipeSpread)
        {
            x = CheckFilledVertices(x);
            GameObject genBulb = Instantiate(organicBulb, vertices[x], Quaternion.LookRotation(normals[x]));
            genBulb.transform.localScale += new Vector3(Random.Range(1, 5), Random.Range(1, 5), Random.Range(1, 5));
            genBulb.transform.parent = this.transform;
            filledVertices[x] = true;
            x += Random.Range(0, lightPipeRandomRange);
        }
    }

    private int CheckFilledVertices(int x)
    {
        while (filledVertices[x])
        {
            x++;
        }

        return x;
    }

    private void RemoveOldDecals()
    {
        GameObject[] oldDecals = GameObject.FindGameObjectsWithTag("Decal");
        foreach (var decal in oldDecals)
        {
            Debug.Log("removed decals");
            DestroyImmediate(decal);
        }
    }

    public Mesh PathGenMesh
    {
        get => _pathGenMesh;
        set => _pathGenMesh = value;
    }
}
