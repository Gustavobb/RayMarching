using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteAlways, ImageEffectAllowedInSceneView]
public class MarcherManager : MonoBehaviour
{
    #region Variables
    [SerializeField] List<MarcherShape> allShapes;
    [SerializeField] ComputeShader raymarching;
    
    RenderTexture target;
    Camera cam;
    Light lightSource;
    List<ComputeBuffer> buffersToDispose = new List<ComputeBuffer>();

    int threadGroupsX, threadGroupsY;
    #endregion

    #region MonoBehaviour Functions
    void Start()
    {
        Init();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination) 
    {   
        cam = Camera.current;
        threadGroupsX = Mathf.CeilToInt(cam.pixelWidth / 8.0f);
        threadGroupsY = Mathf.CeilToInt(cam.pixelHeight / 8.0f);

        SetParameters();
        InitRenderTexture();
        UpdateScene();
        raymarching.SetTexture(0, "Source", source);
        raymarching.SetTexture(0, "Destination", target);

        raymarching.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        Graphics.Blit(target, destination);

        foreach (var buffer in buffersToDispose)
            buffer.Dispose();
    }

    void OnApplicationQuit()
    {   
        allShapes.Clear();        
    }
    #endregion

    #region Compute Shader functions
    void Init() 
    {
        lightSource = FindObjectOfType<Light>();
    }

    void SetParameters() 
    {
        bool lightIsDirectional = lightSource.type == LightType.Directional;
        raymarching.SetMatrix("_CameraToWorld", cam.cameraToWorldMatrix);
        raymarching.SetMatrix("_CameraInverseProjection", cam.projectionMatrix.inverse);
        raymarching.SetVector("_Light", (lightIsDirectional) ? lightSource.transform.forward : lightSource.transform.position);
        raymarching.SetFloat("_LightIntensity", lightSource.intensity);
        raymarching.SetVector("_LightColor", lightSource.color);
        raymarching.SetFloat("_AmbientLightIntensity", RenderSettings.ambientIntensity);
        raymarching.SetVector("_AmbientLightColor", RenderSettings.ambientLight);
        raymarching.SetBool("positionLight", !lightIsDirectional);
        raymarching.SetFloat("_Time", Time.time);
    }

    void InitRenderTexture() 
    {
        if (target == null || target.width != cam.pixelWidth || target.height != cam.pixelHeight) 
        {
            if (target != null)
                target.Release ();
            
            target = new RenderTexture (cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            target.enableRandomWrite = true;
            target.Create ();
        }
    }

    void UpdateScene()
    {
        ShapeData[] shapeData = new ShapeData[allShapes.Count];
        for (int i = 0; i < allShapes.Count; i++) 
        {
            MarcherShape shape = allShapes[i];
            Vector3 col = new Vector3(shape.colour.r, shape.colour.g, shape.colour.b);

            shapeData[i] = new ShapeData () 
            {
                position = shape.Position,
                scale = shape.Scale, colour = col,
                rotationMatrix = shape.MatrixRotation,
                shapeType = (int) shape.shapeType,
                operation = (int) shape.operation,
                repeatInfinite = shape.repeatInfinite ? 1 : 0,
                infiniteOffset = new Vector3(shape.infiniteXOffset, shape.infiniteYOffset, shape.infiniteZOffset),
                roundStrength = shape.roundStrength,
                blendStrength = shape.blendStrength,
                bendStrength = shape.bendStrength,
                twistStrength = shape.twistStrength,
                displacementStrength = shape.displacementStrength,
                hasOutline = shape.hasOutline ? 1 : 0,
                outlineColour = new Vector3(shape.outlineColour.r, shape.outlineColour.g, shape.outlineColour.b),
                outlineWidth = shape.outlineWidth,
                numChildren = shape.numChildren
            };
        }

        if (shapeData.Length <= 0) return;

        ComputeBuffer shapeBuffer = new ComputeBuffer(shapeData.Length, ShapeData.GetSize());
        shapeBuffer.SetData(shapeData);
        raymarching.SetBuffer(0, "shapes", shapeBuffer);
        raymarching.SetInt("numShapes", shapeData.Length);

        buffersToDispose.Add(shapeBuffer);
    }

    public void AddShape(MarcherShape shape)
    {
        if (allShapes.Contains(shape)) return;

        print("Added shape " + shape.name);
        allShapes.Add(shape);
        allShapes = allShapes.OrderByDescending(o => o.operation).OrderBy(b => b.blendStrength).ToList();

        string orderDebug = "";
        for (int i = 0; i < allShapes.Count; i++)
            orderDebug += allShapes[i].name + " ";

        print("Order: " + orderDebug);
    }

    public void RemoveShape(MarcherShape shape)
    {
        print("Removed shape " + shape.name);
        allShapes.Remove(shape);
    }
    #endregion

    #region Shape Data
    struct ShapeData 
    {
        public Vector3 position;
        public Vector3 scale;
        public Vector3 colour;
        public Vector3 infiniteOffset;
        public Matrix4x4 rotationMatrix;
        public int shapeType;
        public int repeatInfinite;
        public int operation;
        public float blendStrength;
        public float bendStrength;
        public float roundStrength;
        public float twistStrength;
        public float displacementStrength;
        public int hasOutline;
        public Vector3 outlineColour;
        public float outlineWidth;
        public int numChildren;

        public static int GetSize() 
        {
            return sizeof(float) * 37 + sizeof(int) * 5;
        }
    }
    #endregion
}
