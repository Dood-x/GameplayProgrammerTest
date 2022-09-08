using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacterController : MonoBehaviour
{
    [System.Serializable]
    public enum PlayerMovementState
    {
        Walking,
        Zipline
    }

    private Animator animator;
	private float h;
	private float v;


	private Camera cam = null;

	[Tooltip("Speed of movement")]
	public float moveSpeed = 8f;
    public float TurnSpeed = 20.0f;


    private float movementMagnitude;
	private Vector3 moveDirection;
    private CharacterController cc;

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;

    private Quaternion smoothCharacterTurning;

    /// <summary>
    /// Jumping
    /// </summary>
    public Transform groundCheck;
    public float GroundRadius = 0.5f;
    public LayerMask GroundLayers;
    private bool isGrounded = false;
    private bool isJumping = false;
   

    public float jumpSpeed = 20.0f;
    private float jumpVelocity;
    private float gravitySpeed;
    public float gravity = -20.0f;

    /// <summary>
    /// Zipline
    /// </summary>
    private PlayerMovementState playerMovementState;

    private TestZipline ziplineScript;
    public float ziplineSpeed = 40.0f;

    // players 'socket' for zipline
    public Transform ziplineAttachementPoint;

    // local rigidbody to move the player across the zipline
    private GameObject localZiplineBody;


    // Start is called before the first frame update
    void Start()
	{
		animator = GetComponent<Animator>();

		cam = Camera.main;
        cc = GetComponent<CharacterController>();
        playerMovementState = PlayerMovementState.Walking;
    }

    // Update is called once per frame
    void Update()
	{
        switch(playerMovementState)
        {
            case PlayerMovementState.Zipline:
                Zipline();
                break;
            case PlayerMovementState.Walking:
            default:
                Walking();
                break;
        }		
	}

    void Walking()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        movementMagnitude = Mathf.Max(Mathf.Abs(h), Mathf.Abs(v));


        bool jumpInput = Input.GetButtonDown("Jump");
        animator.SetBool("Jump", jumpInput);

        // get the animator to animate movement or go back to idle

        // scale the input the the expected animation speed value
        float animationSpeed = movementMagnitude * 6.0f;
        animator.SetFloat("Speed", animationSpeed);

        // map the input onto a movement vector
        moveDirection.Set(h, 0, v);
        // normalizaion preserves direction and clamps movement to to exceed a magnitude of 1
        // this prevents diagonals being faster
        moveDirection.Normalize();
        // multply by the strongest input 
        moveDirection *= movementMagnitude;

        // the true move direction is the cameras y roation euler rotated by the move input euler!
        // this makes the movement input be relative to the camera Y rotation 
        moveDirection = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0) * moveDirection;

        moveDirection *= Time.deltaTime * moveSpeed;


        /// TURNING
        Quaternion turnDirection = moveDirection == Vector3.zero ? transform.rotation : Quaternion.LookRotation(moveDirection, Vector3.up);
        // turn direction is the direction pointed by the move direction
        smoothCharacterTurning = Quaternion.Slerp(transform.rotation, turnDirection, TurnSpeed * Time.deltaTime);
        transform.rotation = smoothCharacterTurning;


        ///JUMPING
        // the character is boosting up in the air
        if (isJumping)
        {
            jumpVelocity = jumpSpeed * Time.deltaTime;
            //Debug.Log(animator.GetCurrentAnimatorStateInfo(0).IsName("InAir"));
            //isJumping = !animator.GetCurrentAnimatorStateInfo(0).IsName("InAir");
            moveDirection.y = jumpVelocity;
        }
        // apply gravity to vertical movement when off the ground

        if (isGrounded)
        {
            gravitySpeed = 0;
        }
        else
        {
            // speed = a * t * t;
            gravitySpeed += gravity * Time.deltaTime * Time.deltaTime;
        }
        moveDirection.y += gravitySpeed;

        if (moveDirection.y < 0f)
        {
            isJumping = false;
        }

        cc.Move(moveDirection);
        //animator.SetBool("Moving", movementMagnitude > 0.1 ? true : false);

    }
    void Zipline()
    {
        if (ziplineScript == null ||ziplineScript.ziplineDestination == null || ziplineScript.ziplineStart == null || localZiplineBody == null)
            return;

        Vector3 ziplineDirection = ziplineScript.ziplineDestination.position - ziplineScript.ziplineStart.position;
        ziplineDirection.Normalize();
        Rigidbody zipBody = localZiplineBody.GetComponent<Rigidbody>() as Rigidbody;
        zipBody.AddForce(ziplineDirection * Time.fixedDeltaTime * ziplineSpeed, ForceMode.Acceleration);
        if (Vector3.Distance(localZiplineBody.transform.position, ziplineScript.ziplineDestination.transform.position) < 0.5f)
        {
            StopZipline();
        }
    }

    void FixedUpdate()
    {
        // set sphere position, with offset
        isGrounded = Physics.CheckSphere(groundCheck.position, GroundRadius, GroundLayers, QueryTriggerInteraction.Ignore);

        // update animator if using character
        if (animator)
        {
            animator.SetBool("Grounded", isGrounded);
            animator.SetBool("FreeFall", !isGrounded && moveDirection.y == 0);
        }
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(cc.center), 0.5f);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(cc.center), 0.5f);
        }
    }

    // called in animation when character is off the ground
    void Leap()
    {
        jumpVelocity = jumpSpeed * Time.deltaTime;
        moveDirection = Vector3.zero;
        moveDirection.y = jumpVelocity;

        cc.Move(moveDirection);

        isJumping = true;

    }

    public void StartZipline(GameObject zipline)
    {
        //Debug.Log("StartZipline");

        if (zipline == null) return;

        if (playerMovementState == PlayerMovementState.Zipline) return;

        playerMovementState = PlayerMovementState.Zipline;
        if (animator)
            animator.SetBool("Zipline", true);

        ziplineScript = zipline.GetComponent<TestZipline>();

        if (ziplineScript == null || ziplineScript.ziplineStart == null || ziplineScript.ziplineDestination == null)
        {
            Debug.LogWarning("Zipline Script not set! Start Zipline called.");
            return;
        }

        localZiplineBody = new GameObject("ZiplineRigidbody");
        //localZiplineBody = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        localZiplineBody.transform.position = ziplineScript.ziplineStart.position;
        Rigidbody localzipBody = localZiplineBody.AddComponent<Rigidbody>() as Rigidbody;
        localzipBody.useGravity = false;
        localzipBody.mass = 5f;
        localzipBody.interpolation = RigidbodyInterpolation.Interpolate;
        localzipBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        cc.enabled = false;
        if (ziplineAttachementPoint)
        {
            Vector3 offset = transform.position - ziplineAttachementPoint.position;
            transform.position = ziplineScript.ziplineStart.position + offset;
            transform.rotation = ziplineScript.ziplineStart.rotation;
        }
        transform.parent = localzipBody.transform;
        if(ziplineScript.Handle)
        {
            ziplineScript.Handle.transform.parent = localzipBody.transform;
        }

    }

    public void StopZipline()
    {
        if (playerMovementState == PlayerMovementState.Walking) return;

        playerMovementState = PlayerMovementState.Walking;
        if (animator)
            animator.SetBool("Zipline", false);

        if (ziplineScript.Handle)
        {
            ziplineScript.Handle.transform.position = ziplineScript.ziplineStart.transform.position;
            ziplineScript.Handle.transform.parent = ziplineScript.gameObject.transform.parent;
        }

        transform.parent = null;
        Destroy(localZiplineBody);
        localZiplineBody = null;
        cc.enabled = true;

        
    }
}

