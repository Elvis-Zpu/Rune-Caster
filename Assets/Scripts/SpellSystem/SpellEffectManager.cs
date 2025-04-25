using UnityEngine;
using System.Collections.Generic;

public class SpellEffectManager : MonoBehaviour
{
    [System.Serializable]
    public class SpellData
    {
        public string spellName;
        public int[] pattern;
        [TextArea] public string description;
    }
    
    [Header("Spells")]
    [SerializeField] private List<SpellData> knownSpells = new List<SpellData>();
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
    
    public delegate void SpellCastEvent(string spellName, int[] pattern);
    public static event SpellCastEvent OnSpellCast;
    
    private Dictionary<string, SpellData> patternToSpell = new Dictionary<string, SpellData>();
    
    private void Start()
    {
        InitializeSpellDictionary();
    }
    
    private void InitializeSpellDictionary()
    {
        patternToSpell.Clear();
        
        foreach (var spell in knownSpells)
        {
            string key = GetPatternKey(spell.pattern);
            if (!patternToSpell.ContainsKey(key))
            {
                patternToSpell.Add(key, spell);
            }
            else
            {
                Debug.LogWarning($"Duplicate spell pattern found for {spell.spellName}!");
            }
        }
        
        if (debugMode)
        {
            Debug.Log($"SpellEffectManager initialized with {patternToSpell.Count} spells");
        }
    }
    
    public void CastSpell(int[] pattern, string spellName = "")
    {
        string patternKey = GetPatternKey(pattern);
        
        if (patternToSpell.TryGetValue(patternKey, out SpellData spell))
        {
            if (debugMode)
            {
                Debug.Log($"Casting spell: {spell.spellName} with pattern: {patternKey}");
            }
            
            // Trigger the event for other systems to respond
            OnSpellCast?.Invoke(spell.spellName, spell.pattern);
        }
        else if (!string.IsNullOrEmpty(spellName))
        {
            // We got a spell name but don't have it in our dictionary
            if (debugMode)
            {
                Debug.Log($"Casting unregistered spell: {spellName}");
            }
            
            OnSpellCast?.Invoke(spellName, pattern);
        }
        else
        {
            Debug.LogWarning($"No spell found for pattern: {patternKey}");
        }
    }
    
    private string GetPatternKey(int[] pattern)
    {
        return string.Join(",", pattern);
    }
}