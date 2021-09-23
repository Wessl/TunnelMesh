using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public PathGenerator pathGenRef;
    public DecalGenerator decalGenRef;
    public void GenerateWorld()
    {
        // Generate path & mesh
        pathGenRef.GeneratePath();
        // Generate decals that sit on the mesh
        decalGenRef.PathGenMesh = pathGenRef.Mesh;
        decalGenRef.GenerateDecals();
        
    }
}
