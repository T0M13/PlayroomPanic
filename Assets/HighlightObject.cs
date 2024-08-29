using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightObject : MonoBehaviour
{
    private Material originalMaterial;
    private Material highlightMaterial;
    private Renderer objectRenderer;

    [Range(0, 1)]
    [SerializeField] private float whitenessFactor = 0.3f;
    [SerializeField][ShowOnly] private bool highlighted = false;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        originalMaterial = objectRenderer.material;

        highlightMaterial = new Material(originalMaterial);
        UpdateHighlightColor();
    }

    private void UpdateHighlightColor()
    {
        highlightMaterial.color = originalMaterial.color + Color.white * whitenessFactor;
    }

    public void Highlight()
    {
        objectRenderer.material = highlightMaterial;
        highlighted = true;
    }

    public void Unhighlight()
    {
        objectRenderer.material = originalMaterial;
        highlighted = false;
    }

}

