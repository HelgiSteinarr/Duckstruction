using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;

public class Propeller : MonoBehaviour
{

    public WaterDecal bowWave; // reference to the bowWave script

    private InputAction moveAction;
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ensure rigidbody exists and is not affected by gravity
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
        }
        else
        {
            Debug.LogError("Propeller: No Rigidbody found. Please attach a Rigidbody component to the propeller.");
        }

        moveAction = InputSystem.actions.FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {
        // +x = right, -x = left | +y = front, -y = back
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        
        transform.localRotation = Quaternion.Euler(0f, -moveValue.x * 45f, 0f);
        rb.AddForce(transform.forward * moveValue.y * 10f, ForceMode.Acceleration);

        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        if (bowWave != null)
        {
            bowWave.amplitude = Mathf.Max(0f, forwardSpeed * 0.1f / 2); // clamp to min 0
            bowWave.RequestUpdate();
        }

        Debug.Log(transform.forward * moveValue.y * 10f);
        Debug.Log(ForceMode.Acceleration);
        Debug.Log($"Move Value: {moveValue}");
    }
}
