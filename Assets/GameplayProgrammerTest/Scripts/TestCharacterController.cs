using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacterController : MonoBehaviour
{

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


    public Transform groundCheck;
    public float GroundRadius = 0.5f;
    public LayerMask GroundLayers;
    private bool isGrounded = false;
    private bool isJumping = false;
   

    public float jumpSpeed = 20.0f;
    private float jumpVelocity;
    private float gravitySpeed;
    public float gravity = -20.0f;

    // Start is called before the first frame update
    void Start()
	{
		animator = GetComponent<Animator>();

		cam = Camera.main;
        cc = GetComponent<CharacterController>();
	}

	// Update is called once per frame
	void Update()
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

        if(isGrounded)
        {
            gravitySpeed = 0;
        }
        else 
        {
            // speed = a * t * t;
            gravitySpeed += gravity * Time.deltaTime * Time.deltaTime;
        }
        moveDirection.y += gravitySpeed;

        if(moveDirection.y < 0f)
        {
            isJumping = false;
        }

        cc.Move(moveDirection);
        //animator.SetBool("Moving", movementMagnitude > 0.1 ? true : false);
        
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
}

