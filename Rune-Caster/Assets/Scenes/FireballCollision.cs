using UnityEngine;

public class FireballCollision : MonoBehaviour
{
    public GameObject groundFirePrefab;

void OnCollisionEnter(Collision collision)
{
    Debug.Log("💥 Fireball collided with: " + collision.gameObject.name);

    if (collision.gameObject.CompareTag("Ground"))
    {
        Vector3 hitPosition = collision.contacts[0].point;
        Quaternion hitRotation = Quaternion.LookRotation(Vector3.up);

        GameObject groundFire = Instantiate(groundFirePrefab, hitPosition, hitRotation);

        // ✅ Stop it from falling
        Rigidbody rb = groundFire.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        Destroy(gameObject); // Destroy the fireball
    }
}

}
