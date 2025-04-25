using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RuneManager : MonoBehaviour
{
    public List<int[]> runePatterns;
    private List<int> currentSequence = new List<int>();
    private SummonBehavior summonBehavior;

    private void Start()
    {
        runePatterns = new List<int[]>
        {
            new int[] {1, 3, 5}, // Animal Transformation
            new int[] {2, 4, 6}, // Float Spell
            new int[] {3, 6, 9}, // Swirl Spell
            new int[] {4, 7, 9}  // Calling Spell - Mummy
        };

        // Find or add SummonBehavior
        summonBehavior = FindObjectOfType<SummonBehavior>();
        if (summonBehavior == null)
        {
            GameObject summonSystem = new GameObject("SummonSystem");
            summonBehavior = summonSystem.AddComponent<SummonBehavior>();
        }
    }

    public void RegisterRuneHit(int colliderID)
    {
        currentSequence.Add(colliderID);
        CheckRuneSequence();
    }

    private void CheckRuneSequence()
    {
        foreach (var pattern in runePatterns)
        {
            if (SequenceMatchesPattern(pattern))
            {
                CastSpell(pattern);
                currentSequence.Clear();
                return;
            }
        }

        if (currentSequence.Count > 5)
            currentSequence.Clear();
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

    private void CastSpell(int[] pattern)
    {
        Debug.Log("Spell cast: " + string.Join(",", pattern));
        
        if (pattern.SequenceEqual(new int[] {1, 3, 5}))
        {
            // Animal Transformation spell
            Debug.Log("Animal Transformation spell cast!");
        }
        else if (pattern.SequenceEqual(new int[] {2, 4, 6}))
        {
            // Float spell
            Debug.Log("Float spell cast!");
        }
        else if (pattern.SequenceEqual(new int[] {3, 6, 9}))
        {
            // Swirl spell
            Debug.Log("Swirl spell cast!");
        }
        else if (pattern.SequenceEqual(new int[] {4, 7, 9}))
        {
            // Calling spell - Summon Mummy
            Debug.Log("Calling spell cast - Summoning Mummy!");
            if (summonBehavior != null)
            {
                summonBehavior.CallSummon();
            }
            else
            {
                Debug.LogError("SummonBehavior not found in the scene!");
            }
        }
    }
}