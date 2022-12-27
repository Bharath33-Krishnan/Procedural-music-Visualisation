using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain_Face
{
   
    Mesh mesh;
    Vector3 localup;
    int resolution;
    Vector3 AxisA;//X axis wrt to local up
    Vector3 AxisB;//Y axis wrt to local up
    ShapeGenerator shapeGenerator;

    static int filter;



    public Terrain_Face(ShapeGenerator shapeGenerator,Mesh mesh, Vector3 localup, int resolution)
    {
        this.mesh = mesh;
        this.localup = localup;
        this.resolution = resolution;
        this.shapeGenerator = shapeGenerator;

        AxisA = new Vector3(localup.y, localup.z, localup.x);//for a cube obtains perpendicular
        AxisB = Vector3.Cross(localup,AxisA);
    }


    public void GenerateMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
         
        int pointIndex;
        int triIndex = 0;
        int FilId=AUdio.FilterId;
        Vector2[] uv= mesh.uv;
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                pointIndex = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 vert = localup + AxisA * (percent.x-0.5f) * 2 + AxisB * (percent.y-0.5f) * 2;//percent -0.5 => from centre offset by -0.5f

                BoundaryManagement(x,y,FilId);
                vertices[pointIndex] = shapeGenerator.CalculatePointonPlanet(vert.normalized);
               

                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex] = pointIndex;
                    triangles[triIndex + 1] = pointIndex + resolution + 1;
                    triangles[triIndex + 2] = pointIndex + resolution;

                    triangles[triIndex + 3] = pointIndex;
                    triangles[triIndex + 4] = pointIndex + 1;
                    triangles[triIndex + 5] = pointIndex + resolution + 1;
                    triIndex += 6;
                }
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.uv=uv;


    }
    void BoundaryManagement(int x,int y,int FilId){
        if(x==0||y==0||x==resolution-1||y==resolution-1)
        {
            AUdio.FilterId=10;
        }
        else
        {
            AUdio.FilterId=FilId;
        }
    }

    public void UpdateUVs(ColorGenerator colorGenerator)
    {
        Vector2[] uvs=new Vector2[resolution*resolution];
         for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int pointIndex = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 vert = localup + AxisA * (percent.x-0.5f) * 2 + AxisB * (percent.y-0.5f) * 2;//percent -0.5 => from centre offset by -0.5f
 
                Vector3 PointOnUnitSphere= vert.normalized;

                uvs[pointIndex]=new Vector2(colorGenerator.BiomeHeightPercent(PointOnUnitSphere),0);
            }
        }
        mesh.uv=uvs;

    }
}


public class Planet : MonoBehaviour
{
    [SerializeField,HideInInspector]
    public MeshFilter[] meshfills;
    Terrain_Face[] TerrainFaces;

    [HideInInspector]
    public bool colorFoldout;
    [HideInInspector]
    public bool shapeFoldout;


    [SerializeField][Range(2,256)]int Resolution=20;
    [SerializeField]Material mat;
    Mesh mesh;
    AUdio Audio; 

    public ColorSettings colorSettings;
    public ShapeSettings shapeSettings;

    ShapeGenerator shapeGenerator=new ShapeGenerator();

    ColorGenerator colorGenerator=new ColorGenerator();

    public bool AutoUpdate;

     public enum RenderMask{All,Top,Bottom,left,right,front,back};
     public RenderMask renderMask=RenderMask.All;

    

    private void Start() {
        Audio=GetComponent<AUdio>();
        shapeSettings.audio=Audio;
    }
  
    void Initialize()
    {
        shapeGenerator.UpdateSettings(shapeSettings);
        colorGenerator.UpdateSettings(colorSettings);
        if (meshfills == null || meshfills.Length == 0)
        {
            meshfills = new MeshFilter[6];
        }
        TerrainFaces = new Terrain_Face[6];

        Vector3[] dirs =
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right,
            Vector3.forward,
            Vector3.back
        };

        for (int i = 0; i < 6; i++)
        {
            if (meshfills[i] == null)
            {
                GameObject meshObj = new GameObject("Mesh");
                meshObj.transform.parent = transform;
                meshObj.AddComponent<MeshRenderer>();
                meshfills[i] = meshObj.AddComponent<MeshFilter>();
                meshfills[i].sharedMesh = new Mesh();
            }
            meshfills[i].GetComponent<MeshRenderer>().sharedMaterial = colorSettings.ShaderMaterial;

            TerrainFaces[i] = new Terrain_Face(shapeGenerator,meshfills[i].sharedMesh, dirs[i], Resolution);
            bool renderFace= renderMask==RenderMask.All||((int)renderMask-1)==i;
            meshfills[i].gameObject.SetActive(renderFace);

        }
   

    }
    void GenerateMesh()
    {
        for (int i = 0; i < TerrainFaces.Length; i++)
        {
            if(meshfills[i].gameObject.activeSelf)
            {
                AUdio.FilterId=i;
                TerrainFaces[i].GenerateMesh();
            }
        }
        colorGenerator.SetMinMax(shapeGenerator.ElevationMinMax);
        
            
    }
    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColors();
    }
    public void OnShapeSettingsUpdated()
    {
        if (!AutoUpdate) return;
        Initialize();
        GenerateMesh();
    }
    public void OnColorSettingsUpdated()
    {
        if (!AutoUpdate) return;
        Initialize();
        GenerateColors();
    }
    void GenerateColors()
    {
        if (colorSettings == null) return;
        colorGenerator.UpdateTexColors();
        for (int i = 0; i < TerrainFaces.Length; i++)
        {
            if(meshfills[i].gameObject.activeSelf)
            {

                TerrainFaces[i].UpdateUVs(colorGenerator);
            }
        }
    }
}
