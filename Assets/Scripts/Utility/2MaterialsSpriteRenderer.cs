using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TwoMaterialsSpriteRenderer : MonoBehaviour
{
    [Header("Materials")]
    public Material baseMaterial;
    public Material overlayMaterial;

    private SpriteRenderer baseRenderer;
    private SpriteRenderer overlayRenderer;

    private void Awake()
    {
        // Main SpriteRenderer
        baseRenderer = GetComponent<SpriteRenderer>();
        if (baseMaterial != null)
            baseRenderer.material = baseMaterial;

        // Create overlay GameObject
        GameObject overlayObj = new GameObject("Overlay");
        overlayObj.transform.parent = transform;
        overlayObj.transform.localPosition = Vector3.zero;

        overlayRenderer = overlayObj.AddComponent<SpriteRenderer>();
        overlayRenderer.sprite = baseRenderer.sprite;
        overlayRenderer.material = overlayMaterial;
        overlayRenderer.sortingLayerID = baseRenderer.sortingLayerID;
        overlayRenderer.sortingOrder = baseRenderer.sortingOrder + 1;
    }

    private void Update()
    {
        if (overlayRenderer != null && baseRenderer != null)
        {
            // Keep sprite and flip in sync
            overlayRenderer.sprite = baseRenderer.sprite;
            overlayRenderer.flipX = baseRenderer.flipX;
            overlayRenderer.flipY = baseRenderer.flipY;
            overlayRenderer.enabled = baseRenderer.enabled;
        }
    }
}