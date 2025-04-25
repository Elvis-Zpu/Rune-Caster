using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RuneSequenceUI : MonoBehaviour
{
    [SerializeField] private GameObject uiElementPrefab;
    [SerializeField] private Transform uiContainer;
    [SerializeField] private Sprite[] runeSprites;
    [SerializeField] private float spacing = 60f;
    [SerializeField] private float fadeDelay = 3f;
    
    private List<GameObject> activeUIElements = new List<GameObject>();
    private float fadeTimer = 0f;
    private bool fading = false;
    
    private void Start()
    {
        if (uiContainer == null)
        {
            uiContainer = transform;
        }
    }
    
    private void Update()
    {
        if (fading)
        {
            fadeTimer -= Time.deltaTime;
            if (fadeTimer <= 0f)
            {
                ClearSequence();
                fading = false;
            }
        }
    }
    
    public void UpdateSequence(List<int> sequence)
    {
        ClearSequence();
        
        for (int i = 0; i < sequence.Count; i++)
        {
            GameObject uiElement = Instantiate(uiElementPrefab, uiContainer);
            
            // Position element
            RectTransform rectTransform = uiElement.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = new Vector2(i * spacing, 0);
            }
            
            // Set sprite
            Image image = uiElement.GetComponent<Image>();
            if (image != null)
            {
                int runeIndex = sequence[i] - 1; // Convert from 1-based to 0-based
                if (runeIndex >= 0 && runeIndex < runeSprites.Length)
                {
                    image.sprite = runeSprites[runeIndex];
                }
            }
            
            activeUIElements.Add(uiElement);
        }
        
        // Reset fade timer
        fadeTimer = fadeDelay;
        fading = true;
    }
    
    public void ClearSequence()
    {
        foreach (GameObject element in activeUIElements)
        {
            Destroy(element);
        }
        activeUIElements.Clear();
        fading = false;
    }
}