using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR; // Required for XR tracking

public class GestureEventProcessor : MonoBehaviour
{
    public InputActionProperty triggerAction; // Reference to the Input Action for trigger
    public InputActionProperty trackingAction; // Reference to the Input Action for tracking
    public Text triggerText; // Assign a UI Text in the Inspector
    public GameObject redBallPrefab; // Assign the RedBall prefab in the Inspector
    public GameObject firePrefab;
    private Rigidbody fireRb;
    public GameObject thrownFirePrefab;
    private bool isHoldingFire = false;
    private bool fireThrown = false;
    private bool fireSettled = false;
    private bool fireJustThrown = false;
    


    private List<GameObject> spawnedBalls = new List<GameObject>(); // Store spawned balls
    private GameObject activeFireEffect;

void Start()
{

}


void Update()
{
    bool isTriggerPressed = false;

    // Check if Spacebar is pressed
    if (Input.GetKey(KeyCode.Space))
    {
        isTriggerPressed = true;
        Debug.Log("⌨ Spacebar Pressed! Simulating Trigger.");
    }

    // Log when calling SpawnRedBall()
    if (isTriggerPressed)
    {
        Debug.Log("✅ Spacebar detected, calling SpawnRedBall()");
        SpawnRedBall();
    }

if (activeFireEffect != null && fireRb != null)
{
    UnityEngine.XR.InputDevice rightController = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightHand);
    Vector3 controllerPosition = Vector3.zero;
    Vector3 controllerVelocity = Vector3.forward;

    rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out controllerPosition);
    rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceVelocity, out controllerVelocity);

    if (Input.GetKeyDown(KeyCode.D) && !fireThrown && !fireJustThrown)
    {
        Debug.Log("🔘 D key pressed! Throwing fire.");

        isHoldingFire = false;
        fireThrown = true;
        fireSettled = false;
        fireJustThrown = true;

        fireRb.useGravity = true;
        fireRb.isKinematic = false;

        // Make it fly in an arc
        Vector3 forward = Camera.main.transform.forward;
        Vector3 upward = Vector3.up * 3f;
        fireRb.linearVelocity = (forward + upward) * 5f;

        fireRb.AddTorque(Random.onUnitSphere * 10f);

        Debug.Log("💨 Fire thrown with velocity: " + fireRb.linearVelocity);
    }
    else if (!fireThrown)
    {
        isHoldingFire = true;

        fireRb.useGravity = false;
        fireRb.isKinematic = true;
        fireRb.linearVelocity = Vector3.zero;

        // ✅ move this line inside the same scope as controllerPosition
        activeFireEffect.transform.position = controllerPosition;
    }
}
}






    void SpawnRedBall()
    {
        Debug.Log("🛠 SpawnRedBall() is being called!"); // Check if function runs

        if (redBallPrefab == null)
        {
            Debug.LogError("❌ Red Ball Prefab is not assigned!");
            return;
        }

        // Get controller's position
        UnityEngine.XR.InputDevice rightController = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightHand);
        Vector3 controllerPosition = Vector3.zero;

        if (rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out controllerPosition))
        {
            Debug.Log("📍 Spawning Red Ball at: " + controllerPosition);
        }
        else
        {
            Debug.LogError("❌ Failed to get controller position!");
            return;
        }

        // Instantiate the red ball at the controller position
        GameObject newBall = Instantiate(redBallPrefab, controllerPosition, Quaternion.identity);

        

        // Store it in the list
        spawnedBalls.Add(newBall);

        // Destroy the ball after 2 seconds to avoid clutter
        Destroy(newBall, 2.5f);
    }

public void OnGestureCompleted(GestureCompletionData gestureCompletionData)
{
    if (gestureCompletionData.gestureID < 0)
    {
        string errorMessage = GestureRecognition.getErrorMessage(gestureCompletionData.gestureID);
        Debug.LogError("❌ Gesture Error: " + errorMessage);
        if (triggerText != null) triggerText.text = "❌ Gesture Error: " + errorMessage;
        return;
    }

    Debug.Log("🖐 Gesture Detected: " + gestureCompletionData.gestureID);
    Debug.Log("🔍 Similarity Score: " + gestureCompletionData.similarity);

    // Update UI text immediately
    if (triggerText != null)
    {
        triggerText.text = $"🖐 Gesture ID: {gestureCompletionData.gestureID}\n🔍 Similarity: {gestureCompletionData.similarity:F3}";
    }

    if (gestureCompletionData.similarity >= 0.3f) // Adjust threshold as needed
    {
        Debug.Log("✅ Valid Gesture Recognized! ID: " + gestureCompletionData.gestureID);
        
        if (gestureCompletionData.gestureID == 0) // If Gesture No. 0 is detected
        {
            Debug.Log("🔥 Spawning Fire Effect!");
            SpawnFireEffect();
            
            // Also update UI text for this specific event
            if (triggerText != null)
            {
                triggerText.text += "\n🔥 Fire Effect Spawned!";
            }
        }
    }
    else
    {
        Debug.LogWarning("⚠ Gesture detected, but similarity too low: " + gestureCompletionData.similarity);
        if (triggerText != null)
        {
            triggerText.text += "\n⚠ Similarity too low!";
        }
    }
}


void SpawnFireEffect()
{
    if (firePrefab == null)
    {
        Debug.LogError("❌ Fire Effect Prefab is not assigned!");
        return;
    }

    // If a fire effect is already active, do not create a new one
    if (activeFireEffect == null)
    {
        // Get controller's position
        UnityEngine.XR.InputDevice rightController = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightHand);
        Vector3 controllerPosition = Vector3.zero;

        if (rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out controllerPosition))
        {
            Debug.Log("📍 Spawning Fire Effect at: " + controllerPosition);
        }
        else
        {
            Debug.LogError("❌ Failed to get controller position!");
            return;
        }

        // Instantiate the fire effect at the controller position
        activeFireEffect = Instantiate(firePrefab, controllerPosition, Quaternion.identity);
        Debug.Log("🔥 Fire Effect Spawned and Following Controller!");

        fireRb = activeFireEffect.GetComponent<Rigidbody>();
    if (fireRb == null)
    {
        fireRb = activeFireEffect.AddComponent<Rigidbody>();
    }
    fireRb.useGravity = false;
    fireRb.isKinematic = true;


        // Destroy the fire effect after 10 seconds
        StartCoroutine(DestroyFireEffectAfterTime(50f));
    }
    fireSettled = false;
}


IEnumerator DestroyFireEffectAfterTime(float delay)
{
    yield return new WaitForSeconds(delay);
    
    if (activeFireEffect != null)
    {
        Debug.Log("🔥 Fire Effect Destroyed After " + delay + " Seconds!");
        Destroy(activeFireEffect);
        activeFireEffect = null; // Reset reference
    }
}

}


