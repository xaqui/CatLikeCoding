using UnityEngine;

public class MoveSpherePhysics : MonoBehaviour
{
    [SerializeField]
    Transform playerInputSpace = default;
    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f, maxAirAcceleration = 1f;
    [SerializeField, Range(0f, 10f)]
    float jumpHeight = 2f;
    [SerializeField, Range(0, 5)]
    int maxAirJumps = 0;
    [SerializeField, Range(90, 180)]
    float maxClimbAngle = 140f;
    [SerializeField]
    Material normalMaterial = default, climbingMaterial = default;

    [Header("Ground Snapping Settings Parameters")]
    [SerializeField, Range(0f, 90f)]
    float maxGroundAngle = 25f, maxStairsAngle = 50f;
    [SerializeField, Range(0f, 100f)]
    float maxSnapSpeed = 100f;
    [SerializeField, Min(0f)]
    float probeDistance = 1f;
    [SerializeField]
    LayerMask probeMask = -1, stairsMask = -1, climbMask = -1;

    Vector3 upAxis, rightAxis, forwardAxis;
    Vector3 velocity, desiredVelocity, connectionVelocity;
    Rigidbody body, connectedBody, previousConnectedBody;
    Vector3 connectionWorldPosition, connectionLocalPosition;
    bool desiredJump;
    int jumpPhase;
    float minGroundDotProduct, minStairsDotProduct, minClimbDotProduct;
    Vector3 contactNormal, steepNormal, climbNormal;
    int groundContactCount, steepContactCount, climbContactCount;
    int stepsSinceLastGrounded, stepsSinceLastJump;

    bool OnGround => groundContactCount > 0;
    bool OnSteep => steepContactCount > 0;
    bool Climbing => climbContactCount > 0;

    MeshRenderer meshRenderer;

    void OnValidate() {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
        minClimbDotProduct = Mathf.Cos(maxClimbAngle * Mathf.Deg2Rad);
    }
    void Awake() {
        body = GetComponent<Rigidbody>();
        // Disable Unity standard gravity to override with our custom gravity
        body.useGravity = false;
        meshRenderer = GetComponent<MeshRenderer>();
        OnValidate();
        
    }

    void Update() {
        //GetComponent<Renderer>().material.SetColor("_Color", Color.white * (groundContactCount * 0.25f));
        //GetComponent<Renderer>().material.SetColor("_Color", OnGround ? Color.black : Color.white);
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);
        if (playerInputSpace) {
            rightAxis = ProjectDirectionOnPlane(playerInputSpace.right, upAxis);
            forwardAxis = ProjectDirectionOnPlane(playerInputSpace.forward, upAxis);
        }
        else {
            rightAxis = ProjectDirectionOnPlane(Vector3.right, upAxis);
            forwardAxis = ProjectDirectionOnPlane(Vector3.forward, upAxis);
        }
        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
        desiredJump |= Input.GetButtonDown("Jump");
        meshRenderer.material = Climbing ? climbingMaterial : normalMaterial;
    }

  
    private void FixedUpdate() {
        Vector3 gravity = CustomGravity.GetGravity(body.position, out upAxis);
        UpdateState();
        AdjustVelocity();
        if (desiredJump) {
            desiredJump = false;
            Jump(gravity);
        }

        velocity += gravity * Time.deltaTime;

        body.velocity = velocity;
        ClearState();
    }
    bool SnapToGround() {
        if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2) {
            return false;
        }
        float speed = velocity.magnitude;
        if (speed > maxSnapSpeed) {
            return false;
        }
        if (!Physics.Raycast(body.position, -upAxis, out RaycastHit hit, probeDistance, probeMask)) {
            return false;
        }
        float upDot = Vector3.Dot(upAxis, hit.normal);
        if (upDot < GetMinDot(hit.collider.gameObject.layer)) {
            return false;
        }
        groundContactCount = 1;
        contactNormal = hit.normal;
        float dot = Vector3.Dot(velocity, hit.normal);
        if (dot > 0f) {
            velocity = (velocity - hit.normal * dot).normalized * speed;
        }
        connectedBody = hit.rigidbody;
        Debug.Log(connectedBody);
        return true;
    }
    void UpdateState() {
        stepsSinceLastGrounded += 1;
        stepsSinceLastJump += 1;
        velocity = body.velocity;
        if (OnGround || SnapToGround() || CheckSteepContacts()) {
            stepsSinceLastGrounded = 0;
            if (stepsSinceLastJump > 1) {
                jumpPhase = 0;
            }
            if (groundContactCount > 1) {
                contactNormal.Normalize();
            }
        } else {
            contactNormal = upAxis;
        }
        if (connectedBody) {
            Debug.Log(connectedBody);
            if (connectedBody.isKinematic || connectedBody.mass >= body.mass) {
                UpdateConnectionState();
            }
        }
    }
    void UpdateConnectionState() {
        if (connectedBody == previousConnectedBody) {
            Vector3 connectionMovement =
                connectedBody.transform.TransformPoint(connectionLocalPosition) -
                connectionWorldPosition;
            connectionVelocity = connectionMovement / Time.deltaTime;
        }
        connectionWorldPosition = body.position;
        connectionLocalPosition = connectedBody.transform.InverseTransformPoint(
            connectionWorldPosition
        );
    }
    void ClearState() {
        groundContactCount = steepContactCount = climbContactCount = 0;
        contactNormal = steepNormal = climbNormal = Vector3.zero;
        previousConnectedBody = connectedBody;
        connectedBody = null;
    }
    void Jump(Vector3 gravity) {
        Vector3 jumpDirection;
        if (OnGround) {
            jumpDirection = contactNormal;
        } else if (OnSteep) {
            jumpDirection = steepNormal;
            jumpPhase = 0;
        } else if (maxAirJumps > 0 && jumpPhase <= maxAirJumps) {
            if (jumpPhase == 0) {
                jumpPhase = 1;
            }
            jumpDirection = contactNormal;
        } else {
            return;
        }
        stepsSinceLastJump = 0;
        jumpPhase += 1;
        float jumpSpeed = Mathf.Sqrt(2f * gravity.magnitude * jumpHeight);
        // Add upward bias to wall jump
        jumpDirection = (jumpDirection + upAxis).normalized;
        float alignedSpeed = Vector3.Dot(velocity, jumpDirection);
        if (alignedSpeed > 0f) {
            jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
        }
        velocity += jumpDirection * jumpSpeed;

    }
    void OnCollisionStay(Collision collision) {
        EvaluateCollision(collision);
    }
    void OnCollisionExit(Collision collision) {
        EvaluateCollision(collision);
    }
    void EvaluateCollision(Collision collision) {
        int layer = collision.gameObject.layer;
        float minDot = GetMinDot(layer);
        for (int i = 0; i < collision.contactCount; i++) {
            Vector3 normal = collision.GetContact(i).normal;
            float upDot = Vector3.Dot(upAxis, normal);
            // Ground contact
            if (upDot >= minDot) {
                groundContactCount += 1;
                contactNormal += normal;
                connectedBody = collision.rigidbody;
            }
            // No ground contact, check for steepness
            else {
                if (upDot > -0.01f) {
                    steepContactCount += 1;
                    steepNormal += normal;
                    if (groundContactCount == 0) {
                        connectedBody = collision.rigidbody;
                    }
                }
                if (upDot >= minClimbDotProduct &&
                    (climbMask & (1 << layer)) != 0 ) {
                    climbContactCount += 1;
                    climbNormal += normal;
                    connectedBody = collision.rigidbody;
                }
            }
        }
    }
    Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal) {
        return (direction - normal * Vector3.Dot(direction, normal)).normalized;
    }
    float GetMinDot(int layer) {
        return (stairsMask & (1 << layer)) == 0 ?
            minGroundDotProduct : minStairsDotProduct;
    }
    void AdjustVelocity() {
        Vector3 xAxis = ProjectDirectionOnPlane(rightAxis, contactNormal);
        Vector3 zAxis = ProjectDirectionOnPlane(forwardAxis, contactNormal);

        Vector3 relativeVelocity = velocity - connectionVelocity;
        float currentX = Vector3.Dot(relativeVelocity, xAxis);
        float currentZ = Vector3.Dot(relativeVelocity, zAxis);

        float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;

        float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
    }

    bool CheckSteepContacts() {
        if (steepContactCount > 1) {
            steepNormal.Normalize();
            float upDot = Vector3.Dot(upAxis, steepNormal);
            if (upDot >= minGroundDotProduct) {
                steepContactCount = 0;
                groundContactCount = 1;
                contactNormal = steepNormal;
                return true;
            }
        }
        return false;
    }

}
