using UnityEngine;

public class DNA : MonoBehaviour
{
    public float r, g, b, s;
    private Vector3 originalScale;
    SpriteRenderer spriteRenderer;
    Collider2D hitCollider;
    public float SurvivalTime { get; set; }
    public bool IsDead { get; private set; } = false;

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hitCollider = GetComponent<Collider2D>();
        originalScale = transform.localScale;
    }

    public void Initialize(float red, float green, float blue, float size)
    {
        r = Mathf.Clamp01(red);
        g = Mathf.Clamp01(green);
        b = Mathf.Clamp01(blue);
        s = Mathf.Clamp(size, 0.2f, 2.0f);
        spriteRenderer.color = new Color(r, g, b, 1f);
        transform.localScale = originalScale * s;
    }

    public void Kill(float elapsedTime)
    {
        if (IsDead) return;
        IsDead = true;
        SurvivalTime = elapsedTime;
        HideFromHunter();
    }

    public void FinishGeneration(float duration)
    {
        if (!IsDead) SurvivalTime = duration;
    }

    private void HideFromHunter()
    {
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (hitCollider != null) hitCollider.enabled = false;
    }
}

