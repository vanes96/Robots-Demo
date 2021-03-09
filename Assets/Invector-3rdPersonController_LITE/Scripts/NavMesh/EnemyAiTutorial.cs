
using UnityEngine;
using UnityEngine.AI;

public class EnemyAiTutorial : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;
    public vMovementSpeed freeSpeed;
    public const float walkSpeed = 0.5f;

    internal float moveSpeed;                           // set the current moveSpeed for the MoveCharacter method

    internal Animator animator;
    internal Rigidbody _rigidbody;                                                      // access the Rigidbody component
    internal PhysicMaterial frictionPhysics, maxFrictionPhysics, slippyPhysics;         // create PhysicMaterial for the Rigidbody
    internal CapsuleCollider _capsuleCollider;
    internal Vector3 moveDirection;                     // used to know the direction you're moving 

    internal float inputMagnitude;                      // sets the inputMagnitude to update the animations in the animator controller
    internal float verticalSpeed;                       // set the verticalSpeed based on the verticalInput
    internal float horizontalSpeed;                     // set the horizontalSpeed based on the horizontalInput  
    bool stopMove = true;

    public float health;
    [Range(0, 2000)]
    public float ShotForce;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        animator = GetComponent<Animator>();
        animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        stopMove = false;
        if (!playerInSightRange && !playerInAttackRange)
        {
            Patroling();
        }
        if (playerInSightRange && !playerInAttackRange)
        { 
            ChasePlayer(); 
        }
        if (playerInAttackRange && playerInSightRange)
        {
            AttackPlayer();
        }

        UpdateAnimator();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;

    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            ///Attack code here
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * ShotForce, ForceMode.Impulse);
            rb.AddForce(transform.up * ShotForce / 4f, ForceMode.Impulse);
            ///End of attack code

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }

    public void SetAnimatorMoveSpeed(vMovementSpeed speed)
    {
        Vector3 relativeInput = transform.InverseTransformDirection(moveDirection);
        verticalSpeed = relativeInput.z;
        horizontalSpeed = relativeInput.x;

        var newInput = new Vector2(verticalSpeed, horizontalSpeed);

        inputMagnitude = Mathf.Clamp(newInput.magnitude, 0, walkSpeed);
        //else
        //    inputMagnitude = Mathf.Clamp(isSprinting ? newInput.magnitude + 0.5f : newInput.magnitude, 0, isSprinting ? sprintSpeed : runningSpeed);
    }

    public void UpdateAnimator(bool stop = false)
    {
        if (animator == null || !animator.enabled) return;

        //animator.SetBool(vAnimatorParameters.IsGrounded, isGrounded);
        //animator.SetFloat(vAnimatorParameters.GroundDistance, groundDistance);
        //animator.SetFloat(vAnimatorParameters.InputVertical, stopMove ? 0 : verticalSpeed, freeSpeed.animationSmooth, Time.deltaTime);
        //animator.SetFloat(vAnimatorParameters.InputMagnitude, stopMove ? 0f : inputMagnitude, freeSpeed.animationSmooth, Time.deltaTime);
        verticalSpeed = 1;
        inputMagnitude = 0.5f;

        //animator.SetFloat(vAnimatorParameters.InputVertical, verticalSpeed, freeSpeed.animationSmooth, Time.deltaTime);
        animator.SetFloat(vAnimatorParameters.InputMagnitude, stop ? 0f : inputMagnitude, freeSpeed.animationSmooth, Time.deltaTime);
    }

    public void SetControllerMoveSpeed(vMovementSpeed speed)
    {
        moveSpeed = Mathf.Lerp(moveSpeed, speed.walkSpeed, speed.movementSmooth * Time.deltaTime);
    }


    public static partial class vAnimatorParameters
    {
        public static int InputHorizontal = Animator.StringToHash("InputHorizontal");
        public static int InputVertical = Animator.StringToHash("InputVertical");
        public static int InputMagnitude = Animator.StringToHash("InputMagnitude"); 

        //public static int IsGrounded = Animator.StringToHash("IsGrounded");
        //public static int GroundDistance = Animator.StringToHash("GroundDistance");
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    [System.Serializable]
    public class vMovementSpeed
    {
        [Range(1f, 20f)]
        public float movementSmooth = 6f;
        [Range(0f, 1f)]
        public float animationSmooth = 0.2f;
        [Tooltip("Rotation speed of the character")]
        public float rotationSpeed = 16f;
        [Tooltip("Rotate with the Camera forward when standing idle")]
        public bool rotateWithCamera = false;
        [Tooltip("Speed to Walk using rigidbody or extra speed if you're using RootMotion")]
        public float walkSpeed = 2f;
        [Tooltip("Speed to Run using rigidbody or extra speed if you're using RootMotion")]
        public float runningSpeed = 4f;
        [Tooltip("Speed to Sprint using rigidbody or extra speed if you're using RootMotion")]
        public float sprintSpeed = 6f;
    }
}
