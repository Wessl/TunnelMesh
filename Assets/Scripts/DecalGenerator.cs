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

    private Mesh _pathGenMesh;

    public void GenerateDecals()
    {
        RemoveOldDecals();
        PlacePipes();
        PlaceLightPipes();
    }

    private void PlacePipes()
    {
        int[] triangles = _pathGenMesh.triangles;
        Vector3[] vertices = _pathGenMesh.vertices;
        Vector3[] normals = _pathGenMesh.normals;

        for (int x = 0; x < vertices.Length; x+=pipeSpread)
        {
            GameObject genPipe = Instantiate(pipe, vertices[x], Quaternion.LookRotation(normals[x]));
            float rndSizeInc = Random.Range(0, pipeSizeRandomRange);
            genPipe.transform.localScale += new Vector3(rndSizeInc,rndSizeInc,rndSizeInc);
            genPipe.transform.Translate(-genPipe.transform.forward * (genPipe.transform.localScale.y * 5), Space.Self);
            
            x += Random.Range(0, pipeSpreadRandomRange);
        }
    }

    private void PlaceLightPipes()
    {
        int[] triangles = _pathGenMesh.triangles;
        Vector3[] vertices = _pathGenMesh.vertices;
        Vector3[] normals = _pathGenMesh.normals;

        for (int x = 0; x < vertices.Length; x+=lightPipeSpread)
        {
            GameObject genLight = Instantiate(lightPipe, vertices[x], Quaternion.LookRotation(normals[x]));
            genLight.transform.Translate(genLight.transform.forward * (genLight.transform.localScale.x * 0.8f), Space.Self);
            genLight.transform.Rotate(new Vector3(0,180,0), Space.Self);

            x += Random.Range(0, lightPipeRandomRange);
        }
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
