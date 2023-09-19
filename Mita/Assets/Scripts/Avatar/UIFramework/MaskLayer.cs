using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskLayer : Graphic, ICanvasRaycastFilter
{
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        return true;
    }

    protected override void UpdateGeometry()
    {
        
    }

    protected override void UpdateMaterial()
    {
        
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        
    }
}
