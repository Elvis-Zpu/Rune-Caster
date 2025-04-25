using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class GlowEffect : MonoBehaviour
{
    [SerializeField] private Color glowColor = Color.cyan;
    [SerializeField] private float glowIntensity = 1.5f;
    [SerializeField] private float pulsateSpeed = 2f;
    [SerializeField] private bool usePulsating = true;
    
    private Renderer objectRenderer;
    private Material originalMaterial;
    private Material glowMaterial;
    private bool isGlowing = false;
    
    private void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        originalMaterial = objectRenderer.material;
        
        // Create a new material based on the original
        glowMaterial = new Material(Shader.Find("Standard"));
        glowMaterial.CopyPropertiesFromMaterial(originalMaterial);
        
        // Ensure the object is not glowing at start
        EndGlow();
    }
    
    private void Update()
    {
        if (isGlowing && usePulsating)
        {
            float emission = Mathf.PingPong(Time.time * pulsateSpeed, 1.0f) * glowIntensity;
            Color finalColor = glowColor * Mathf.LinearToGammaSpace(emission);
            glowMaterial.SetColor("_EmissionColor", finalColor);
        }
    }
    
    public void StartGlow()
    {
        isGlowing = true;
        
        // Enable emission
        glowMaterial.EnableKeyword("_EMISSION");
        glowMaterial.SetColor("_EmissionColor", glowColor * glowIntensity);
        
        // Apply the glow material
        objectRenderer.material = glowMaterial;
    }
    
    public void EndGlow()
    {
        isGlowing = false;
        
        // Apply the original material
        objectRenderer.material = originalMaterial;
    }
    
    // Method to set custom glow color at runtime
    public void SetGlowColor(Color newColor)
    {
        glowColor = newColor;
        if (isGlowing)
        {
            glowMaterial.SetColor("_EmissionColor", glowColor * glowIntensity);
        }
    }
}