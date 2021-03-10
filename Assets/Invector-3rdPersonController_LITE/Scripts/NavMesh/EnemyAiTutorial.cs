
using UnityEngine;
using UnityEngine.AI;

public class EnemyAiTutorial : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public GameObject BulletPrefab;
    public Transform LeftBulletStartPlace;
    public Transform RightBulletStartPlace;

    public LayerMask whatIsGround, whatIsPlayer;
    public vMovementSpeed freeSpeed;
    public const float walkSpeed = 0.5f;
    [Range(0, 1)]
    public float ShotDuration;

    internal float moveSpeed;                           // set the current moveSpeed for the MoveCharacter method

    internal Animator animator;
    internal Rigidbody _rigidbody;                                                      // access the Rigidbody component
    internal PhysicMaterial frictionPhysics, maxFrictionPhysics, slippyPhysics;         // create PhysicMaterial for the Rigidbody
    internal CapsuleCollider _capsuleCollider;
    internal Vector3 moveDirection;                     // used to know the direction you're moving 

    internal float inputMagnitude;                      // sets the inputMagnitude to update the animations in the animator controller
    internal float verticalSpeed;                       // set the verticalSpeed based on the verticalInput
    internal float horizontalSpeed;                     // set the horizontalSpeed based on the horizontalInput  
    public bool IsMoving = false;
    private float _currentDistanceToPlayer;

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
    public float sightRange, attackRange, MinDistanceToPlayer;
    public bool playerInSightRange, playerInAttackRange;

    private float lastShotTime = 0;
    private bool lastShotWasLeft = false;

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
        //_currentDistanceToPlayer = Vector3.Distance(player.position, transform.position);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        IsMoving = false;
        if (playerInSightRange)
        {
            transform.LookAt(player);
        }

        //if (!playerInSightRange && !playerInAttackRange)
        //{
        //    Patroling();
        //}
        //Debug.Log(agent.remainingDistance);
        agent.SetDestination(player.position);
        if (playerInSightRange && agent.remainingDistance > agent.stoppingDistance)
        {
            ChasePlayer(); 
        }
        else
        {
            agent.isStopped = true;
        }
        if (playerInAttackRange && playerInSightRange)
        {
            AttackPlayer();
        }

        UpdateAnimator();
    }

    private void Patroling()
    {
        Debug.Log("Patroling");

        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;

        IsMoving = true;
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
        Debug.Log("ChasePlayer");       
        transform.LookAt(player);
        agent.isStopped = false;
        IsMoving = true;
    }

    private void AttackPlayer()
    {
        Debug.Log("AttackPlayer");
        //Make sure enemy doesn't move
        //agent.SetDestination(transform.position);

        transform.LookAt(player);

        //if (!alreadyAttacked) 
            if (lastShotTime > ShotDuration)
        {
            ///Attack code here
            var bullet = Instantiate(BulletPrefab);
            var bulletStartPlace = LeftBulletStartPlace;
            lastShotTime = 0;
            if (lastShotWasLeft)
            {
                bulletStartPlace = RightBulletStartPlace;
            }

            bullet.transform.SetPositionAndRotation(bulletStartPlace.position, bulletStartPlace.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(bulletStartPlace.up * ShotForce);
            lastShotWasLeft = !lastShotWasLeft;

            //Rigidbody bullet = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            //bullet.AddForce(transform.forward * ShotForce, ForceMode.Impulse);
            //bullet.AddForce(transform.up * ShotForce / 4f, ForceMode.Impulse);
            ///End of attack code

            //alreadyAttacked = true;
            //Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
        else
        {
            lastShotTime += Time.deltaTime;
        }
        //isMoving = true;
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

    public void UpdateAnimator()
    {
        if (animator == null || !animator.enabled) return;

        //animator.SetBool(vAnimatorParameters.IsGrounded, isGrounded);
        //animator.SetFloat(vAnimatorParameters.GroundDistance, groundDistance);
        //animator.SetFloat(vAnimatorParameters.InputVertical, stopMove ? 0 : verticalSpeed, freeSpeed.animationSmooth, Time.deltaTime);
        //animator.SetFloat(vAnimatorParameters.InputMagnitude, stopMove ? 0f : inputMagnitude, freeSpeed.animationSmooth, Time.deltaTime);
        verticalSpeed = 1;
        inputMagnitude = 0.5f;

        //animator.SetFloat(vAnimatorParameters.InputVertical, verticalSpeed, freeSpeed.animationSmooth, Time.deltaTime);
        animator.SetFloat(vAnimatorParameters.InputMagnitude, IsMoving ? inputMagnitude : 0, freeSpeed.animationSmooth, Time.deltaTime);
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
