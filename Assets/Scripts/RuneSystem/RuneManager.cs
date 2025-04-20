using UnityEngine;
using System.Collections.Generic;

public class RuneManager : MonoBehaviour
{
    public List<int[]> runePatterns;
    private List<int> currentSequence = new List<int>();

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
        // To add
    }
}