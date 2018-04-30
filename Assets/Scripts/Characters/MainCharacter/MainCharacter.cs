using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using DontTrust.GameManager;

namespace DontTrust.Characters.Main
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]
	public class MainCharacter : MonoBehaviour
	{
		/* Serialized private fields (not visible for other classes) */
		[SerializeField] float m_MovingTurnSpeed = 360;
		[SerializeField] float m_StationaryTurnSpeed = 180;
		[SerializeField] float m_JumpPower = 12f;
		[Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
		[SerializeField] float m_RunCycleLegOffset = 0.2f; //Specific to the character in sample assets, will need to be modified to work with others
		[SerializeField] float m_MoveSpeedMultiplier = 1f;
		[SerializeField] float m_AnimSpeedMultiplier = 1f;
		[SerializeField] float m_GroundCheckDistance = 0.2f;

		/* Public fields */
		public bool m_IsGrounded; //Flag to indicate if the character is touching the ground. Public in order for other objects to be able to see it.

		/* Private fields */
		Rigidbody m_Rigidbody;
		Animator m_Animator;
		Transform m_Character;
		float m_OrigGroundCheckDistance;
		const float k_Half = 0.5f;
		float m_TurnAmount;
		float m_ForwardAmount;
		int m_CharacterDirection; //Character's facing direction
		Vector3 m_GroundNormal; //Ground normal vector
		float m_CapsuleHeight; //Height of the collider
		Vector3 m_CapsuleCenter; //Center of the collider
		CapsuleCollider m_Capsule; //Character's collider
		bool m_Crouching; //Crouching flag
		bool m_WallCollision; //Wall collision flag
		private sbyte m_Health; //Character's health. 8-bit signed integer (Max. 127). Public for other classes to see it, but not serializable.
		private GameObject m_GameManager;
		private Mechanics m_ManagerMechanics;

		/* Methods */
		void Start() //Initialization method
		{
			m_Animator = GetComponent<Animator>();
			m_Rigidbody = GetComponent<Rigidbody>();
			m_Character = this.gameObject.transform.GetChild (0);
			m_Capsule = GetComponent<CapsuleCollider>();
			m_CapsuleHeight = m_Capsule.height;
			m_CapsuleCenter = m_Capsule.center;

			m_Rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			m_OrigGroundCheckDistance = m_GroundCheckDistance;

			m_Health = 100;
			m_GameManager = GameObject.FindWithTag ("GameController");
			m_ManagerMechanics = m_GameManager.GetComponent<Mechanics> ();
		}


		public void TakeDamage(sbyte damage) //Check the health conditions and update the health indicator in the GUI.
		{
			m_Health -= damage;
			if(m_Health<=0){ //Health cannot be negative
				m_Health = 0; //Health cannot be lower than zero
				m_ManagerMechanics.LoadCheckpoint(); //Return character to checkpoint
				m_Health = 100; //TEMPORAL
			}

			Debug.Log("Damage Received: " + damage);
			Debug.Log("Health = " + m_Health);
		}


		public sbyte GetHealth()
		{
			return m_Health;
		}


		public void Move(Vector3 move, bool crouch, bool jump) //Calls the other movement methods. Is called by the MainCharacterControl FixedUpdate() method
		{

			// convert the world relative moveInput vector into a local-relative
			// turn amount and forward amount required to head in the desired
			// direction.}
			m_CharacterDirection = move.z==0? 0 : (move.z>0? 1 : -1);
			if (move.magnitude > 1f) move.Normalize();
			move = transform.InverseTransformDirection(move);
			CheckGroundStatus();
			move = Vector3.ProjectOnPlane(move, m_GroundNormal);
			m_TurnAmount = Mathf.Atan2(move.x, move.z);
			m_ForwardAmount = move.z;

			ApplyExtraTurnRotation();

			// control and velocity handling is different when grounded and airborne:
			if (m_IsGrounded)
			{
				HandleGroundedMovement(crouch, jump);
			}
			else
			{
				HandleAirborneMovement(crouch, jump);
			}

			ScaleCapsuleForCrouching(crouch);
			PreventStandingInLowHeadroom();

			// send input and other state parameters to the animator
			UpdateAnimator(move);
		}


		void ScaleCapsuleForCrouching(bool crouch) //Scales the character collider when it crouches or slides
		{
			if (crouch && !m_WallCollision) //Can't crouch while colliding with wall //(m_IsGrounded && crouch)
			{
				if (m_Crouching) return;
				m_Capsule.height = m_Capsule.height / 2f;
				m_Capsule.center = m_Capsule.center / 2f;
				m_Crouching = true;
			}
			else
			{
				Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
				{
					m_Crouching = true;
					return;
				}
				m_Capsule.height = m_CapsuleHeight;
				m_Capsule.center = m_CapsuleCenter;
				m_Crouching = false;
			}
		}

		void PreventStandingInLowHeadroom() //Prevents the character to stand up when it's below a collider lower than the character height
		{
			// prevent standing up in crouch-only zones
			if (!m_Crouching)
			{
				Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
				{
					m_Crouching = true;
				}
			}
		}


		void UpdateAnimator(Vector3 move) //Updates the animator variables in order to trigger transitions. Also updates the animator speed.
		{
			// update the animator parameters
			m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
			m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
			m_Animator.SetBool("Crouch", m_Crouching);
			m_Animator.SetBool("WallSliding", m_WallCollision && !m_IsGrounded); //Has to be wall colliding and mid-air to execute a wall-slide
			m_Animator.SetBool("OnGround", m_IsGrounded);
			if (!m_IsGrounded)
			{
				m_Animator.SetFloat("Jump", m_Rigidbody.velocity.y);
				//Debug.Log (m_Rigidbody.velocity.y);
			}

			// calculate which leg is behind, so as to leave that leg trailing in the jump animation
			// (This code is reliant on the specific run cycle offset in our animations,
			// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
			float runCycle =
				Mathf.Repeat(
					m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
			float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
			if (m_IsGrounded)
			{
				m_Animator.SetFloat("JumpLeg", jumpLeg);
			}

			// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
			// which affects the movement speed because of the root motion.
			if (m_IsGrounded && move.magnitude > 0)
			{
				m_Animator.speed = m_AnimSpeedMultiplier*Mathf.Abs(m_Rigidbody.velocity.z/40); //Running animation speed is faster as the character moves faster
				if (m_Animator.speed > 1.5f) { //Upper animation speed limit
					m_Animator.speed = 1.5f;
				}
				if (m_Animator.speed < 0.7f) { //Lower animation speed limit
					m_Animator.speed = 0.7f;
				}
			}
			else
			{
				// don't use that while airborne
				m_Animator.speed = 1;
			}
		}


		void HandleAirborneMovement(bool crouch, bool jump) //Manages the character movement when mid-air
		{
			// apply extra gravity from multiplier:
			Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
			m_Rigidbody.AddForce(extraGravityForce);

			m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.4f;//0.01f

			if (m_Rigidbody.velocity.y < 0f) { //Gravity multiplier when falling (falling is faster than going up)
				m_Rigidbody.AddForce (Physics.gravity * (8 + m_Rigidbody.velocity.y / 5)); //Since y velocity is negative, the gravity multiplier becomes smaller with greater speed
			}
			else if (m_Rigidbody.velocity.y > 0f && !CrossPlatformInputManager.GetButton ("Jump")) { //Gravity multiplier when jumping button is not pressed
				m_Rigidbody.AddForce (Physics.gravity * 3);
			}
			if (m_WallCollision && m_Rigidbody.velocity.y < -2f) {
				m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, -2f, m_Rigidbody.velocity.z); //Gravity multiplier has to be smaller when wallsliding
			}

			if (m_Rigidbody.velocity.z != 0) { //Extra horizontal drag when character is mid-air
				Vector3 v = new Vector3 (0, 0, (-(m_Rigidbody.velocity.z / Mathf.Abs (m_Rigidbody.velocity.z)) * m_MoveSpeedMultiplier / 4.5f) / Time.deltaTime); //Normalize velocity to obtain direction and apply opposite force
				m_Rigidbody.AddForce (v);
			}

			if (jump && (m_Animator.GetCurrentAnimatorStateInfo (0).IsName ("WallSliding") || m_WallCollision) && !m_IsGrounded && m_CharacterDirection!=0) //Perform walljump (has to be wallSliding and pressing a key -has a direction-)
			{
				float jumpDirection = -1; //Assumes the character is facing right (has to jump to the opposite direction
				if (m_Rigidbody.rotation.y >= 0.8) { //The character is facing left
					jumpDirection = 1;
				}
				Vector3 JumpForce = new Vector3(0, m_JumpPower*1.5f, jumpDirection * m_JumpPower*2f);
				m_Rigidbody.AddForce (JumpForce, ForceMode.Impulse);

				m_IsGrounded = false;
				m_Animator.applyRootMotion = false;
				m_GroundCheckDistance = 0.1f;
			}
		}


		void HandleGroundedMovement(bool crouch, bool jump) //Manages the character movement when on ground.
		{
			// check whether conditions are right to allow a jump:
			if (jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
			{
				// jump!
				//m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);

				//Apply the jump force
				Vector3 JumpForce = new Vector3(0, m_JumpPower*2, 0);
				m_Rigidbody.AddForce (JumpForce, ForceMode.Impulse);

				//Clear flags
				m_IsGrounded = false;
				m_Animator.applyRootMotion = false;
				m_GroundCheckDistance = 0.1f;
			}
		}

		void ApplyExtraTurnRotation()
		{
			// help the character turn faster (this is in addition to root rotation in the animation)
			float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
			transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
		}


		public void OnCollisionEnter(Collision col) //Collision with another object is detected
		{
			if (col.gameObject.CompareTag ("Wall")) { //Verify wall collision
				m_WallCollision = true;
			}
		}

		public void OnCollisionExit (Collision col) //Collision with another object finishes
		{
			if (col.gameObject.CompareTag ("Wall")) { //Verify wall collision
				//m_WallCollision = false;
				InvokeRepeating("MakeCollisionFalse", 0.05f, 0); //Delays the turning off of the wall collision flag in order to give the player some time to perform the wall jump.
			}
		}

		void MakeCollisionFalse() //Makes the wall collision flag false. Used to put a delay on this action inside the OnCollisionExit method.
		{
			m_WallCollision = false;
		}

		public void OnAnimatorMove() //Move the character when the walk animation is executed
		{
			// we implement this function to override the default root motion.
			// this allows us to modify the positional speed before it's applied.
			if (Time.deltaTime > 0)
			{
				Vector3 v;
				if (m_IsGrounded) {
					//v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;
					if (!m_Crouching) {
						v = (m_CharacterDirection * Vector3.forward * m_MoveSpeedMultiplier) / Time.deltaTime;
					}
					else{
						v = 0*Vector3.forward;
					}
				}
				else{
					v = (m_CharacterDirection * Vector3.forward * m_MoveSpeedMultiplier / 2) / Time.deltaTime;
				}

				m_Rigidbody.AddForce (v);
				//m_Rigidbody.velocity += v;

				// we preserve the existing y part of the current velocity.
				//v.z += m_Rigidbody.velocity.z/2;
				//v.y = m_Rigidbody.velocity.y;
				//m_Rigidbody.velocity = v;

				//Character has to stop faster when not running (no button pressed)
				if (m_CharacterDirection == 0 && Mathf.Abs(m_Rigidbody.velocity.z)>1  && m_IsGrounded==true && m_Crouching==false) { //Velocity has to be greater than one in order to not reduce
																																	//it when the character is stopped. It is not reduced when sliding.
					Vector3 vel = m_Rigidbody.velocity;
					if (vel.z > 0) //Character running right
						vel.z -= 3f;
					else //Character running left
						vel.z += 3f;
					m_Rigidbody.velocity = vel;
				}
			}
		}


		void CheckGroundStatus() //Checks if the character is on ground.
		{
			RaycastHit hitInfo;
			float upOffset = 0.3f;
			float forwardOffset = 1.2f;
			float backOffset = 0.7f;
#if UNITY_EDITOR
			// Helper to visualise the ground check rays in the scene view
			//Center ray
			Debug.DrawLine(transform.position + (Vector3.up * upOffset), transform.position + (Vector3.up * upOffset) + (Vector3.down * m_GroundCheckDistance)); //If in the editor, draw the raycast line
			//Front ray
			Debug.DrawLine(transform.position + (Vector3.up * upOffset) + (Vector3.forward * forwardOffset), transform.position + (Vector3.up * upOffset) + (Vector3.forward * forwardOffset) + (Vector3.down * m_GroundCheckDistance));
			//Back ray
			Debug.DrawLine(transform.position + (Vector3.up * upOffset) + (Vector3.back * backOffset), transform.position + (Vector3.up * upOffset) + (Vector3.back * backOffset) + (Vector3.down * m_GroundCheckDistance));
#endif
			// 0.1f is a small offset to start the ray from inside the character
			// it is also good to note that the transform position in the sample assets is at the base of the character
			if (Physics.Raycast(transform.position + (Vector3.up * upOffset), Vector3.down, out hitInfo, m_GroundCheckDistance)) //Ray from the center of the character
			{
				m_GroundNormal = hitInfo.normal;
				m_IsGrounded = true;
				m_Animator.applyRootMotion = true;
			}
			else if (Physics.Raycast(transform.position + (Vector3.up * upOffset) + (Vector3.forward * forwardOffset), Vector3.down, out hitInfo, m_GroundCheckDistance)) //Ray from the front of the character
			{
				m_GroundNormal = hitInfo.normal;
				m_IsGrounded = true;
				m_Animator.applyRootMotion = true;
			}
			else if (Physics.Raycast(transform.position + (Vector3.up * upOffset) + (Vector3.back * backOffset), Vector3.down, out hitInfo, m_GroundCheckDistance)) //Ray from the back of the character
			{
				m_GroundNormal = hitInfo.normal;
				m_IsGrounded = true;
				m_Animator.applyRootMotion = true;
			}
			else
			{
				m_IsGrounded = false;
				m_GroundNormal = Vector3.up;
				m_Animator.applyRootMotion = false;
			}
		}
	}
}
