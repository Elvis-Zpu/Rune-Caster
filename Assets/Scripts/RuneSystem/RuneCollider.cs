using UnityEngine;

public class RuneCollider : MonoBehaviour
{
    public int colliderID;

    private RuneManager runeManager;

    private void Start()
    {
        runeManager = FindObjectOfType<RuneManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerController"))
        {
            runeManager.RegisterRuneHit(colliderID);
        }
    }
}