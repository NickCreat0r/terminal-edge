using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CeilingGripZone : MonoBehaviour
{
    [SerializeField] private float width = 3f;
    [SerializeField] private float height = 0.5f;

    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private Vector2 nativeSpriteSize = Vector2.one; // sprite's unscaled size, cached once

    public float Width => width;
    public float Height => height;

    private void Awake()
    {
        CacheNativeSpriteSize();
        UpdateDimensions();
    }

    private void OnValidate()
    {
        CacheNativeSpriteSize();
        UpdateDimensions();
    }

    public void SetDimensions(float newWidth, float newHeight)
    {
        width = Mathf.Max(0.2f, newWidth);
        height = Mathf.Max(0.1f, newHeight);
        UpdateDimensions();
    }

    private void CacheNativeSpriteSize()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            nativeSpriteSize = spriteRenderer.sprite.bounds.size;
        }
    }

    public void UpdateDimensions()
    {
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        // Scale the whole object so the sprite visually matches width/height.
        // The collider stays fixed at the sprite's native size and inherits
        // the same scale automatically, so it can never drift from what's drawn.
        transform.localScale = new Vector3(
            width / nativeSpriteSize.x,
            height / nativeSpriteSize.y,
            1f
        );

        boxCollider.isTrigger = true;
        boxCollider.size = nativeSpriteSize;
        boxCollider.offset = Vector2.zero;

        if (spriteRenderer != null)
        {
            spriteRenderer.drawMode = SpriteDrawMode.Simple; // kills the built-in tiling handles
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0.5f, 0.3f);
        Gizmos.DrawCube(transform.position, new Vector3(width, height, 0.1f));
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0.1f));
    }
}