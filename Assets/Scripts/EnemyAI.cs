
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public GameObject BulletPrefab;
    public Transform LeftBulletStartPlace;
    public Transform RightBulletStartPlace;
    public LayerMask PlayerLayer;
    public LayerMask FloorLayer;

    [Range(0, 100)]
    public float Health = 100;
    [Range(0, 1)]
    public float ShotDuration = 0.5f;
    [Range(0, 1000)]
    public float ShotForce = 600f;
    [Range(0, 1)]
    public float animationSmooth = 0.2f;
    [Range(0, 20)]
    public float SightRange = 10;
    [Range(0, 20)]
    public float AttackRange = 5;
    public bool IsPlayerInSightRange;
    public bool IsPlayerInAttackRange;

    private Animator _animator;
    private Rigidbody _rigidbody;                                                    

    private float _lastShotTime = 0;
    private bool _lastShotWasLeft = false;
    private const string PlayerBulletTag = "PlayerBullet";

    //public Vector3 walkPoint;
    //bool walkPointSet;
    //public float walkPointRange;

    private void Awake()
    {
        if (PlayerLayer == 0)
        {
            PlayerLayer = LayerMask.GetMask("Player");
        }
        if (FloorLayer == 0)
        {
            FloorLayer = LayerMask.GetMask("Floor");
        }

        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;

        _animator = GetComponent<Animator>();
        _animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
    }

    private void Update()
    {
        //Check for sight and attack range
        IsPlayerInSightRange = Physics.CheckSphere(transform.position, SightRange, PlayerLayer);
        IsPlayerInAttackRange = Physics.CheckSphere(transform.position, AttackRange, PlayerLayer);

        if (IsPlayerInSightRange)
        {
            Debug.Log("Player In Sight Range");

            transform.LookAt(player);
            agent.SetDestination(player.position);

            if (agent.remainingDistance > agent.stoppingDistance)
            {
                agent.isStopped = false;
            }
            else
            {
                agent.isStopped = true;
            }

            if (IsPlayerInAttackRange)
            {
                AttackPlayer();
            }
        }
        else
        {
            agent.isStopped = true;
        }

        //if (!playerInSightRange && !playerInAttackRange)
        //{
        //    Patroling();
        //}
        UpdateAnimator();
    }

    //private void Patroling()
    //{
    //    if (!walkPointSet) 
    //        SearchWalkPoint();

    //    if (walkPointSet)
    //        agent.SetDestination(walkPoint);

    //    Vector3 distanceToWalkPoint = transform.position - walkPoint;

    //    //Walkpoint reached
    //    if (distanceToWalkPoint.magnitude < 1f)
    //        walkPointSet = false;
    //}
    //private void SearchWalkPoint()
    //{
    //    float randomZ = Random.Range(-walkPointRange, walkPointRange);
    //    float randomX = Random.Range(-walkPointRange, walkPointRange);

    //    walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

    //    if (Physics.Raycast(walkPoint, -transform.up, 2f, FloorLayer))
    //    {
    //        walkPointSet = true;
    //    }
    //}

    private void AttackPlayer()
    {
        Debug.Log("AttackPlayer");
        if (_lastShotTime > ShotDuration)
        {
            var bullet = Instantiate(BulletPrefab);
            var bulletStartPlace = LeftBulletStartPlace;
            _lastShotTime = 0;

            if (_lastShotWasLeft)
            {
                bulletStartPlace = RightBulletStartPlace;
            }

            bullet.transform.SetPositionAndRotation(bulletStartPlace.position, bulletStartPlace.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(bulletStartPlace.up * ShotForce);
            _lastShotWasLeft = !_lastShotWasLeft;
            //Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
        else
        {
            _lastShotTime += Time.deltaTime;
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"{gameObject.name}. Health = {Health}");

        Health -= damage;

        if (Health <= 0)
        {
            Invoke(nameof(DestroyEnemy), 0.5f);
        }
    }

    public void UpdateAnimator()
    {
        if (_animator == null || !_animator.enabled)
        {
            return;
        }

        _animator.SetFloat(vAnimatorParameters.InputMagnitude, agent.isStopped ? 0 : 0.5f, animationSmooth, Time.deltaTime);
    }

    public static partial class vAnimatorParameters
    {
        public static int InputMagnitude = Animator.StringToHash("InputMagnitude");
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, SightRange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(PlayerBulletTag))
        {
            TakeDamage(25);
        }
    }

}
