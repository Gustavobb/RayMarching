using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MarcherShape : MonoBehaviour
{
    #region Variables
    public enum ShapeType { Sphere, Cube, Torus, Pyramid, Cylinder, Cone, Capsule };
    public enum Operation { Cut, Mask, Default };

    public ShapeType shapeType;
    public Operation operation;
    public Color colour = Color.white;
    public bool repeatInfinite = false;
    public bool hasOutline = false;
    public float outlineWidth = 0.1f;
    public Color outlineColour = Color.white;

    [Range(0, 2)] public float roundStrength = 1.0f;
    [Range(-100, 100)] public float infiniteXOffset, infiniteYOffset, infiniteZOffset;
    [Range(0, 30)] public float blendStrength;
    [Range(0, 30)] public float bendStrength;
    [Range(0, 30)] public float twistStrength;
    [Range(0, 5)] public float displacementStrength;
    [HideInInspector] public int numChildren;
    MarcherManager manager;
    #endregion

    #region Object Variables
    public Vector3 Position 
    {
        get { return transform.position; }
    }

    public Vector3 Scale
    {
        get
        {
            Vector3 parentScale = Vector3.one;
            if (transform.parent != null) 
                parentScale = transform.parent.localScale;

            return Vector3.Scale(transform.localScale, parentScale);
        }
    }

    public Matrix4x4 MatrixRotation 
    {
        get { return Matrix4x4.Rotate(transform.rotation); }
    }
    #endregion

    #region MonoBehaviour Functions
    void OnEnable()
    {
        manager = FindObjectOfType<MarcherManager>();
        manager.AddShape(this);
    }

    void OnDisable()
    {
        manager.RemoveShape(this);
    }
    #endregion
}
