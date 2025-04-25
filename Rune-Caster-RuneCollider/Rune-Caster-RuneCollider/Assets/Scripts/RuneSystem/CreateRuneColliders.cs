using UnityEngine;

public class CreateRuneColliders : MonoBehaviour
{
    [SerializeField] private int numberOfColliders = 5;      
    [SerializeField] private float radius = 0.3f;         
    [SerializeField] private GameObject colliderPrefab; 
    [SerializeField] private Transform playerTransform; 
    [SerializeField] private float distanceFromPlayer = 1f; 

    private GameObject[] colliders; 

    private bool collidersVisible = false;

    private void Start()
    {
        if (colliderPrefab == null)
        {
            Debug.LogError("Collider prefab is not assigned.");
            return;
        }

        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is not assigned.");
            return;
        }

        colliders = new GameObject[numberOfColliders];

        GenerateColliders();

        SetCollidersActive(false);
    }

    private void GenerateColliders()
    {
        for (int i = 0; i < numberOfColliders; i++)
        {
            float angle = i * Mathf.PI * 2f / numberOfColliders;

            float y = Mathf.Cos(angle) * radius; // Vertical
            float z = Mathf.Sin(angle) * radius; // Depth
            Vector3 colliderPosition = new Vector3(0f, y, z); // Position in YZ plane

            GameObject collider = Instantiate(colliderPrefab, colliderPosition, Quaternion.identity, transform);

            collider.name = $"RuneCollider_{i + 1}";

            RuneCollider runeCollider = collider.GetComponent<RuneCollider>();
            if (runeCollider == null)
            {
                runeCollider = collider.AddComponent<RuneCollider>();
            }
            runeCollider.colliderID = i + 1;

            colliders[i] = collider;
        }
    }

    private void SetCollidersActive(bool isActive)
    {
        foreach (GameObject collider in colliders)
        {
            if (collider != null)
            {
                collider.SetActive(isActive);
            }
        }
    }

    public void TriggerAction()
    {
        if (collidersVisible)
        {
            HideColliders();
        }
        else
        {
            ShowColliders();
        }
        collidersVisible = !collidersVisible; // Toggle visibility
    }

    public void ShowColliders()
    {
        transform.position = playerTransform.position + playerTransform.forward * distanceFromPlayer;
        transform.rotation = playerTransform.rotation;

        // Position the colliders relative to the player's orientation
        for (int i = 0; i < numberOfColliders; i++)
        {
            float angle = i * Mathf.PI * 2f / numberOfColliders;

            float x = Mathf.Cos(angle) * radius; // Horizontal
            float y = Mathf.Sin(angle) * radius; // Vertical
            Vector3 localPosition = new Vector3(x, y, 0f); // In player's local X-Y plane

            colliders[i].transform.localPosition = localPosition;
        }

        SetCollidersActive(true);
    }

    public void HideColliders()
    {
        SetCollidersActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TriggerAction();
        }
    }
}
