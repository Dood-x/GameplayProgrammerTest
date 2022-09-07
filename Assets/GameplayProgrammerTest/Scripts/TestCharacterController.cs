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

		// get the animator to animate movement or go back to idle

		// scale the input the the expected animation speed value
		float animationSpeed = movementMagnitude * 6.0f;
		animator.SetFloat("Speed", animationSpeed);

		Debug.Log(animationSpeed);


		moveDirection = Vector3.zero;

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

        cc.Move(moveDirection);
        //animator.SetBool("Moving", movementMagnitude > 0.1 ? true : false);
        //Vector3.Slerp(transform.position + transform.forward, )

        Quaternion turnDirection = moveDirection == Vector3.zero ? transform.rotation : Quaternion.LookRotation(moveDirection, Vector3.up);

        smoothCharacterTurning = Quaternion.Slerp(transform.rotation, turnDirection, TurnSpeed * Time.deltaTime);

        transform.rotation = smoothCharacterTurning;
        //where the character is facing
        //transform.LookAt(transform.position + moveDirection);
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
}

