using UnityEngine;

namespace DontTrust.Characters.Main
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]
	public class MainCharacter : MonoBehaviour
	{
		[SerializeField] float m_MovingTurnSpeed = 360;
		[SerializeField] float m_StationaryTurnSpeed = 180;
		[SerializeField] float m_JumpPower = 12f;
		[Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
		[SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
		[SerializeField] float m_MoveSpeedMultiplier = 1f;
		[SerializeField] float m_AnimSpeedMultiplier = 1f;
		[SerializeField] float m_GroundCheckDistance = 0.1f;

		Rigidbody m_Rigidbody;
		Animator m_Animator;
		Transform m_Character;
		public bool m_IsGrounded;
		float m_OrigGroundCheckDistance;
		const float k_Half = 0.5f;
		float m_TurnAmount;
		float m_ForwardAmount;
		int m_CharacterDirection;
		Vector3 m_GroundNormal;
		float m_CapsuleHeight;
		Vector3 m_CapsuleCenter;
		CapsuleCollider m_Capsule;
		bool m_Crouching;
		bool m_WallCollision;


		void Start()
		{
			m_Animator = GetComponent<Animator>();
			m_Rigidbody = GetComponent<Rigidbody>();
			m_Character = this.gameObject.transform.GetChild (0);
			m_Capsule = GetComponent<CapsuleCollider>();
			m_CapsuleHeight = m_Capsule.height;
			m_CapsuleCenter = m_Capsule.center;

			m_Rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			m_OrigGroundCheckDistance = m_GroundCheckDistance;
		}


		public void Move(Vector3 move, bool crouch, bool jump)
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


		void ScaleCapsuleForCrouching(bool crouch)
		{
			if (crouch)//(m_IsGrounded && crouch)
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

		void PreventStandingInLowHeadroom()
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


		void UpdateAnimator(Vector3 move)
		{
			// update the animator parameters
			m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
			m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
			m_Animator.SetBool("Crouch", m_Crouching);
			m_Animator.SetBool("WallSliding", m_WallCollision);
			m_Animator.SetBool("OnGround", m_IsGrounded);
			if (!m_IsGrounded)
			{
				m_Animator.SetFloat("Jump", m_Rigidbody.velocity.y);
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
				m_Animator.speed = m_AnimSpeedMultiplier*Mathf.Abs(m_Rigidbody.velocity.z/30); //Running animation speed is faster as the character moves faster
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


		void HandleAirborneMovement(bool crouch, bool jump)
		{
			// apply extra gravity from multiplier:
			Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
			m_Rigidbody.AddForce(extraGravityForce);

			m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;

			//Debug.Log (m_Rigidbody.velocity.y);
			if (m_Rigidbody.velocity.y < 0f) { //Gravity multiplier when falling (falling is faster than going up)
				m_Rigidbody.AddForce(Physics.gravity* (8+m_Rigidbody.velocity.y/5) );
			}

			if (m_Rigidbody.velocity.z != 0) { //Extra horizontal drag when character is mid-air
				Vector3 v = new Vector3 (0, 0, (-(m_Rigidbody.velocity.z / Mathf.Abs (m_Rigidbody.velocity.z)) * m_MoveSpeedMultiplier / 4.5f) / Time.deltaTime); //Normalize velocity to obtain direction and apply opposite force
				m_Rigidbody.AddForce (v);
			}

			if (m_Rigidbody.velocity.z == 0) { //WallJump (TEMPORAL, DEBE VERIFICAR COLISION CON OBJETO DE TIPO MURO)
				m_WallCollision = true;
			} else {
				m_WallCollision = false;
				//m_Character.rotation = new Quaternion(m_Character.rotation.x, m_Character.rotation.y+0, m_Character.rotation.z, m_Character.rotation.w);
			}

			if (jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo (0).IsName ("WallSliding") && m_CharacterDirection!=0) //Perform walljump (has to be wallSliding and pressing a key -has a direction-)
			{
				float jumpDirection = -1; //Assumes the character is facing right (has to jump to the opposite direction
				if (m_Rigidbody.rotation.y >= 0.8) { //The character is facng left
					jumpDirection = 1;
				}
				Vector3 JumpForce = new Vector3(0, m_JumpPower*2, jumpDirection * m_JumpPower*3);
				m_Rigidbody.AddForce (JumpForce, ForceMode.Impulse);

				m_IsGrounded = false;
				m_Animator.applyRootMotion = false;
				m_GroundCheckDistance = 0.1f;
			}
		}


		void HandleGroundedMovement(bool crouch, bool jump)
		{
			// check whether conditions are right to allow a jump:
			if (jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
			{
				// jump!
				//m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);

				Vector3 JumpForce = new Vector3(0, m_JumpPower*2, 0);
				m_Rigidbody.AddForce (JumpForce, ForceMode.Impulse);

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


		public void OnAnimatorMove()
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
					v = (m_CharacterDirection * Vector3.forward * m_MoveSpeedMultiplier/4) / Time.deltaTime;
				}

				m_Rigidbody.AddForce (v);

				// we preserve the existing y part of the current velocity.
				//v.z += m_Rigidbody.velocity.z/2;
				//v.y = m_Rigidbody.velocity.y;
				//m_Rigidbody.velocity = v;

				/* Character has to stop faster when not running (no button pressed) */
				if (m_CharacterDirection == 0 && Mathf.Abs(m_Rigidbody.velocity.z)>1  && m_IsGrounded==true && m_Crouching==false) { //Velocity has to be greater than one in order to not reduce
																																	//it when the character is stopped. It is not reduced when sliding.
					Vector3 vel = m_Rigidbody.velocity;
					if (vel.z > 0) //Character running right
						vel.z -= 2;
					else //Character running left
						vel.z += 2;
					m_Rigidbody.velocity = vel;
				}
			}
		}


		void CheckGroundStatus()
		{
			RaycastHit hitInfo;
#if UNITY_EDITOR
			// helper to visualise the ground check ray in the scene view
			Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
			// 0.1f is a small offset to start the ray from inside the character
			// it is also good to note that the transform position in the sample assets is at the base of the character
			if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
			{
				m_GroundNormal = hitInfo.normal;
				m_IsGrounded = true;
				m_Animator.applyRootMotion = true;
			}
			else if (Physics.Raycast(transform.position + (Vector3.up * 0.1f) + (Vector3.forward * 0.5f), Vector3.down, out hitInfo, m_GroundCheckDistance))
			{
				m_GroundNormal = hitInfo.normal;
				m_IsGrounded = true;
				m_Animator.applyRootMotion = true;
			}
			else if (Physics.Raycast(transform.position + (Vector3.up * 0.1f) + (Vector3.back * 0.5f), Vector3.down, out hitInfo, m_GroundCheckDistance))
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
