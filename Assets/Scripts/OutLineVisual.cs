using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OutLineVisual : MonoBehaviour
{
    
    public Material oldMaterial;
    public Material newMaterial;
    public SpriteRenderer oldSpriteRenderer;
    public void OutLineShow(SpriteRenderer spriteRenderer)
    {
        oldSpriteRenderer = spriteRenderer;
        oldMaterial = spriteRenderer.material;
        spriteRenderer.material = newMaterial;
    }

    public void ResetMaterial(SpriteRenderer spriteRenderer)
    {
        spriteRenderer.material = oldMaterial;
    }
    
}
