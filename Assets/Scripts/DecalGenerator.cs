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
    public GameObject frog;
    [SerializeField] private int frogSpread;
    [SerializeField] private int frogRandomRange;

    public PathGenerator pathGenerator;

    private int startIterationAt;

    private int[] triangles;
    private Vector3[] vertices;
    private Vector3[] normals;

    private bool[] filledVertices;


    private Mesh _pathGenMesh;

    public void GenerateDecals()
    {
        pipeSpread *= pathGenerator.MeshFidelity;
        lightPipeSpread *= pathGenerator.MeshFidelity;
        bulbSpread *= pathGenerator.MeshFidelity;
        
        startIterationAt = pathGenerator.GetTunnelFrontViewEdgeAmount();
        
        triangles = _pathGenMesh.triangles;
        vertices = _pathGenMesh.vertices;
        normals = _pathGenMesh.normals;
        
        filledVertices = new bool[vertices.Length];
        
        RemoveOldDecals();
        PlacePipes();
        PlaceLightPipes();
        PlaceBulbs();
        //PlaceFrogs();
    }

    private void PlacePipes()
    {
        for (int x = startIterationAt; x < vertices.Length; x+=pipeSpread)
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
        for (int x = startIterationAt; x < vertices.Length; x+=lightPipeSpread)
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
        for (int x = startIterationAt; x < vertices.Length; x+=bulbSpread)
        {
            x = CheckFilledVertices(x);
            GameObject genBulb = Instantiate(organicBulb, vertices[x], Quaternion.LookRotation(normals[x]));
            genBulb.transform.localScale += new Vector3(Random.Range(1, 5), Random.Range(1, 5), Random.Range(1, 5));
            genBulb.transform.parent = this.transform;
            filledVertices[x] = true;
            x += Random.Range(0, bulbRandomRange);
        }
    }
    private void PlaceFrogs()
    {
        for (int x = startIterationAt; x < vertices.Length; x+=lightPipeSpread)
        {
            x = CheckFilledVertices(x);
            GameObject genFrog = Instantiate(frog, vertices[x], Quaternion.LookRotation(normals[x]));
            var f_rb = genFrog.GetComponent<Rigidbody>();
            f_rb.constraints = RigidbodyConstraints.None;
            genFrog.transform.Translate(genFrog.transform.forward * (genFrog.transform.localScale.x * 2f), Space.Self);
            genFrog.transform.rotation = Quaternion.identity;
            f_rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ ;
            genFrog.transform.parent = this.transform;
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
            DestroyImmediate(decal);
        }
    }

    public Mesh PathGenMesh
    {
        get => _pathGenMesh;
        set => _pathGenMesh = value;
    }
}
