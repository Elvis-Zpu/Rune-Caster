using UnityEngine;
using System.Collections;

public class SummonBehavior : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject coffinPrefab; // Reference Coffin.prefab
    public GameObject mummyPrefab; // Reference Mummy_Monster_Prefab.prefab
    
    [Header("Spawn Settings")]
    public Transform summonLocation;
    public float coffinDropHeight = 10f;
    public float coffinDropSpeed = 5f;
    public float mummySpawnDelay = 1f;
    
    [Header("Effects")]
    public ParticleSystem summonEffect;
    public AudioSource summonSound;
    public ParticleSystem coffinImpactEffect;
    public AudioSource coffinImpactSound;
    public ParticleSystem mummySpawnEffect;
    
    private GameObject currentCoffin;
    private GameObject currentMummy;
    private bool isSummoning = false;

    public void CallSummon()
    {
        if (isSummoning)
        {
            Debug.Log("Summoning is already in progress...");
            return;
        }

        if (coffinPrefab == null || mummyPrefab == null || summonLocation == null)
        {
            Debug.LogError("Required prefabs or summon location not set!");
            return;
        }

        StartCoroutine(SummonSequence());
    }

    private IEnumerator SummonSequence()
    {
        isSummoning = true;

        // Play initial summon effects
        if (summonEffect != null)
            summonEffect.Play();
        if (summonSound != null)
            summonSound.Play();

        // Calculate spawn position above the summon location
        Vector3 spawnPos = summonLocation.position + Vector3.up * coffinDropHeight;
        
        // Spawn and drop the coffin
        currentCoffin = Instantiate(coffinPrefab, spawnPos, Quaternion.identity);
        
        // Drop the coffin
        float distanceToGround = coffinDropHeight;
        while (distanceToGround > 0)
        {
            float dropAmount = coffinDropSpeed * Time.deltaTime;
            currentCoffin.transform.Translate(Vector3.down * dropAmount);
            distanceToGround -= dropAmount;
            yield return null;
        }

        // Coffin impact effects
        if (coffinImpactEffect != null)
            coffinImpactEffect.Play();
        if (coffinImpactSound != null)
            coffinImpactSound.Play();

        yield return new WaitForSeconds(mummySpawnDelay);

        // Spawn mummy
        if (mummySpawnEffect != null)
            mummySpawnEffect.Play();
            
        Vector3 mummySpawnPos = currentCoffin.transform.position + currentCoffin.transform.forward * 2f;
        currentMummy = Instantiate(mummyPrefab, mummySpawnPos, currentCoffin.transform.rotation);

        // Add AI behavior component if it exists
        MummyAI mummyAI = currentMummy.GetComponent<MummyAI>();
        if (mummyAI != null)
        {
            mummyAI.Initialize();
        }

        isSummoning = false;
    }

    public void DismissSummon()
    {
        if (currentMummy != null)
        {
            Debug.Log("Dismissing mummy...");
            Destroy(currentMummy);
            currentMummy = null;
        }

        if (currentCoffin != null)
        {
            Debug.Log("Dismissing coffin...");
            Destroy(currentCoffin);
            currentCoffin = null;
        }

        if (summonEffect != null)
            summonEffect.Stop();
    }

    private void OnDestroy()
    {
        DismissSummon();
    }
} 