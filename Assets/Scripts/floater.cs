using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Floater : MonoBehaviour
{
    public Rigidbody rb;
    public float depthBefSub = 1f;
    public float displacementAmt = 3f; // volume
    public int floaters = 1;
    public float waterDrag = 0.99f;
    public float waterAngularDrag = 0.5f;
    public float waveForceMultiplier = 1f;
    public WaterSurface water;

    private Vector3 currentDirection;
    private Vector3 A, B, C;
    private Vector3 waterPosition;
    private Vector3 normal;
    private Vector3 deformationDirection;

    private void Start()
    {
        if (water == null)
        {
            Debug.LogError("Floater: No water surface assigned. Please assign a water surface in the inspector.");
        }
        if (rb == null)
        {
            Debug.LogError("Floater: No Rigidbody assigned. Please assign a parent Rigidbody in the inspector. Remember to disable gravity");
        }
    }

    private void FixedUpdate()
    {
        if (water == null)
        {
            Debug.LogError("Floater: No water surface assigned. Please assign a water surface in the inspector.");
            return;
        }

        FetchWaterSurfaceData(this.transform.position, out waterPosition, out normal, out currentDirection);
        deformationDirection = Vector3.ProjectOnPlane(normal, Vector3.up);

        // simulating gravity for each floater. divided by how many floaters there are on the object as we dont want x4 gravity if there are 4 floaters
        rb.AddForceAtPosition(Physics.gravity / floaters, transform.position, ForceMode.Acceleration);


        // Adding force for small waves and ripples pushing objects. 
        rb.AddForce(deformationDirection * waveForceMultiplier);

        // when the floater is below the water surface, it will be displaced by the water
        if (transform.position.y < waterPosition.y)
        {
            float displacementMulti = Mathf.Clamp01((waterPosition.y - transform.position.y) / depthBefSub) * displacementAmt;

            rb.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMulti, 0f), transform.position, ForceMode.Acceleration);
            rb.AddForce(displacementMulti * -rb.linearVelocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            rb.AddTorque(displacementMulti * -rb.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }

    }
    private Vector3 FetchWaterSurfaceData(Vector3 point, out Vector3 positionWS, out Vector3 normalWS, out Vector3 currentDirectionWS)
    {
        WaterSearchParameters searchParameters = new WaterSearchParameters();
        WaterSearchResult searchResult = new WaterSearchResult();
        
        // Build the search parameters
        searchParameters.startPositionWS = searchResult.candidateLocationWS;
        searchParameters.targetPositionWS = point;
        searchParameters.error = 0.01f;
        searchParameters.maxIterations = 8;
        searchParameters.includeDeformation = true;
        searchParameters.outputNormal = true;

        // Init the out variable with default values. 
        positionWS = searchResult.candidateLocationWS;
        normalWS = Vector3.up;
        currentDirectionWS = Vector3.right; 
        
        // Do the search
        if (water.ProjectPointOnWaterSurface(searchParameters, out searchResult))
        {
            positionWS = searchResult.projectedPositionWS;   
            currentDirectionWS = searchResult.currentDirectionWS;
            normalWS = searchResult.normalWS;
        }
        
        return positionWS;
    }
}
