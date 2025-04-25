using UnityEngine;

public class Change : MonoBehaviour
{
    public GameObject rabbit;
    public GameObject frog;
    public GameObject Collider;

    void Start()
    {
        // Ensure only the rabbit is active at the start
        rabbit.SetActive(true);
        frog.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Swap rabbit for frog
            if (Collider.activeSelf) {
                if (rabbit.activeSelf)
                {
                    rabbit.SetActive(false);
                    frog.SetActive(true);
                }
                else
                {
                    frog.SetActive(false);
                    rabbit.SetActive(true);
                }
            }  
        }
    }
}
