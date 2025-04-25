using UnityEngine;
using System.Collections.Generic;

public class RuneManager : MonoBehaviour
{
    [Header("Rune Patterns")]
    public List<RunePattern> runePatterns = new List<RunePattern>();
    
    [Header("Feedback")]
    [SerializeField] private RuneSequenceUI sequenceUI;
    [SerializeField] private AudioClip runeActivationSound;
    [SerializeField] private AudioClip spellSuccessSound;
    [SerializeField] private AudioClip spellFailSound;
    [SerializeField] private GameObject spellFailEffectPrefab;
    [SerializeField] private Transform spellSpawnPoint;
    [SerializeField] private float maxSequenceTime = 5f; // Time to complete a sequence before reset
    
    private List<int> currentSequence = new List<int>();
    private AudioSource audioSource;
    private float sequenceTimer = 0f;
    private bool sequenceInProgress = false;
    private SpellEffectManager spellEffectManager;
    
    [System.Serializable]
    public class RunePattern
    {
        public string patternName;
        public int[] pattern;
        public GameObject spellEffect;
    }
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        spellEffectManager = GetComponent<SpellEffectManager>();
        if (spellEffectManager == null)
        {
            spellEffectManager = gameObject.AddComponent<SpellEffectManager>();
        }
    }
    
    private void Update()
    {
        // Reset sequence if too much time passes
        if (sequenceInProgress)
        {
            sequenceTimer -= Time.deltaTime;
            if (sequenceTimer <= 0f)
            {
                ResetSequence();
            }
        }
    }
    
    public void RegisterRuneHit(int colliderID)
    {
        // Play rune activation sound
        if (audioSource != null && runeActivationSound != null)
        {
            audioSource.PlayOneShot(runeActivationSound);
        }
        
        // Add to sequence
        currentSequence.Add(colliderID);
        
        // Update sequence timer
        sequenceTimer = maxSequenceTime;
        sequenceInProgress = true;
        
        // Update UI
        if (sequenceUI != null)
        {
            sequenceUI.UpdateSequence(currentSequence);
        }
        
        // Check if sequence matches any patterns
        CheckRuneSequence();
    }
    
    private void CheckRuneSequence()
    {
        foreach (var runePattern in runePatterns)
        {
            if (SequenceMatchesPattern(runePattern.pattern))
            {
                CastSpell(runePattern);
                ResetSequence();
                return;
            }
        }
        
        // If sequence is too long, clear it
        if (currentSequence.Count >= 8)
        {
            FailSpellCast();
        }
    }
    
    private bool SequenceMatchesPattern(int[] pattern)
    {
        if (currentSequence.Count != pattern.Length) return false;
        
        for (int i = 0; i < pattern.Length; i++)
        {
            if (currentSequence[i] != pattern[i]) return false;
        }
        return true;
    }
    
    private void CastSpell(RunePattern pattern)
    {
        Debug.Log("Spell cast: " + pattern.patternName);
        
        // Play success sound
        if (audioSource != null && spellSuccessSound != null)
        {
            audioSource.PlayOneShot(spellSuccessSound);
        }
        
        // Spawn spell effect
        if (pattern.spellEffect != null && spellSpawnPoint != null)
        {
            Instantiate(pattern.spellEffect, spellSpawnPoint.position, spellSpawnPoint.rotation);
        }
        
        // Inform spell effect manager
        if (spellEffectManager != null)
        {
            spellEffectManager.CastSpell(pattern.pattern, pattern.patternName);
        }
    }
    
    private void FailSpellCast()
    {
        Debug.Log("Spell casting failed");
        
        // Play fail sound
        if (audioSource != null && spellFailSound != null)
        {
            audioSource.PlayOneShot(spellFailSound);
        }
        
        // Show fail effect
        if (spellFailEffectPrefab != null && spellSpawnPoint != null)
        {
            GameObject failEffect = Instantiate(spellFailEffectPrefab, spellSpawnPoint.position, spellSpawnPoint.rotation);
            Destroy(failEffect, 2f);
        }
        
        ResetSequence();
    }
    
    private void ResetSequence()
    {
        currentSequence.Clear();
        sequenceInProgress = false;
        
        // Clear UI
        if (sequenceUI != null)
        {
            sequenceUI.ClearSequence();
        }
    }

    public void FinalizeSpellAttempt()
    {
        if (currentSequence.Count > 0)
        {
            bool patternFound = false;
            
            foreach (var runePattern in runePatterns)
            {
                if (SequenceMatchesPattern(runePattern.pattern))
                {
                    CastSpell(runePattern);
                    patternFound = true;
                    break;
                }
            }
            
            if (!patternFound)
            {
                FailSpellCast();
            }
            
            ResetSequence();
        }
    }
}