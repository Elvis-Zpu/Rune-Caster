using UnityEngine;

public class RuneCollider : MonoBehaviour
{
    public int colliderID;
    
    [Header("Visual Feedback")]
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material glowMaterial;
    [SerializeField] private float glowDuration = 0.5f;
    
    private RuneManager runeManager;
    private Renderer runeRenderer;
    private float glowTimer = 0f;
    private bool isGlowing = false;
    
    private void Start()
    {
        runeManager = FindObjectOfType<RuneManager>();
        runeRenderer = GetComponent<Renderer>();
        
        if (runeRenderer == null)
        {
            Debug.LogWarning("No renderer found on RuneCollider. Visual feedback won't work.");
        }
        else if (defaultMaterial == null)
        {
            defaultMaterial = runeRenderer.material;
        }
    }
    
    private void Update()
    {
        // Handle glow effect timer
        if (isGlowing)
        {
            glowTimer -= Time.deltaTime;
            if (glowTimer <= 0f)
            {
                isGlowing = false;
                if (runeRenderer != null)
                {
                    runeRenderer.material = defaultMaterial;
                }
            }
        }
    }
    
    private void OnMouseDown()
    {
        ActivateRune();
    }
    
    public void ActivateRune()
    {
        // Trigger visual feedback
        if (runeRenderer != null && glowMaterial != null)
        {
            runeRenderer.material = glowMaterial;
            isGlowing = true;
            glowTimer = glowDuration;
        }
        
        // Register the rune hit with the manager
        if (runeManager != null)
        {
            runeManager.RegisterRuneHit(colliderID);
        }
        else
        {
            Debug.LogError("RuneManager not found in scene!");
        }
    }
}