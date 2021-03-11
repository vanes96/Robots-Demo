
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody _rigidbody;
    private Vector3 _spawnPlace;
    private bool _isPlayerInSightRange;
    private bool _isPlayerInAttackRange;
    private float _lastShotTime = 0;
    private bool _lastShotWasLeft = false;

    [SerializeField]
    private NavMeshAgent _agent;
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private GameObject _bulletPrefab;
    [SerializeField]
    private Transform _leftBulletStartPlace;
    [SerializeField]
    private Transform _rightBulletStartPlace;
    [SerializeField]
    private LayerMask _playerLayer;
    [SerializeField]
    private LayerMask _floorLayer;

    [SerializeField]
    [Range(0, 100)]
    private float _health = 100;
    [SerializeField]
    [Range(0, 1)]
    private float _shotDuration = 0.5f;
    [SerializeField]
    [Range(0, 1000)]
    private float _shotForce = 600f;
    [SerializeField]
    [Range(0, 1)]
    private float _animationSmooth = 0.2f;
    [SerializeField]
    [Range(0, 20)]
    private float _sightRadius = 10;
    [SerializeField]
    [Range(0, 20)]
    private float _attackRadius = 5;
    [SerializeField]
    [Range(0, 10)]
    private float _stoppingDistance = 4;

    public Vector3 SpawnPlace => _spawnPlace;
    //public Vector3 walkPoint;
    //bool walkPointSet;
    //public float walkPointRange;

    private void Awake()
    {
        if (_playerLayer == 0)
        {
            _playerLayer = LayerMask.GetMask(Constanter.PlayerLayerName);
        }

        if (_floorLayer == 0)
        {
            _floorLayer = LayerMask.GetMask(Constanter.FloorLayerName);
        }

        if (!_player)
        {
            _player = GameObject.FindGameObjectWithTag(Constanter.PlayerTagName).transform;
        }

        _spawnPlace = transform.position;

        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
        _agent.isStopped = true;
    }

    private void Update()
    {
        //Check for sight and attack range
        _isPlayerInSightRange = Physics.CheckSphere(transform.position, _sightRadius, _playerLayer);
        _isPlayerInAttackRange = Physics.CheckSphere(transform.position, _attackRadius, _playerLayer);

        if (_isPlayerInSightRange)
        {
            //Debug.Log("Player In Sight Range");

            transform.LookAt(_player);
            _agent.SetDestination(_player.position);

            if (_agent.remainingDistance > _agent.stoppingDistance)
            {
                _agent.isStopped = false;
            }
            else
            {
                _agent.isStopped = true;
            }

            if (_isPlayerInAttackRange)
            {
                AttackPlayer();
            }
        }
        else if (!_agent.isStopped)
        {
            _agent.SetDestination(_spawnPlace);
            _agent.stoppingDistance = 0.1f;

            if (_agent.remainingDistance < _agent.stoppingDistance)
            {
                _agent.isStopped = true;
                _agent.stoppingDistance = _stoppingDistance;
            }
        }

        UpdateAnimator();
    }

    private void AttackPlayer()
    {
        //Debug.Log("AttackPlayer");
        if (_lastShotTime > _shotDuration)
        {
            var bullet = Instantiate(_bulletPrefab);
            var bulletStartPlace = _leftBulletStartPlace;
            _lastShotTime = 0;

            if (_lastShotWasLeft)
            {
                bulletStartPlace = _rightBulletStartPlace;
            }

            bullet.transform.SetPositionAndRotation(bulletStartPlace.position, bulletStartPlace.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(bulletStartPlace.up * _shotForce);
            _lastShotWasLeft = !_lastShotWasLeft;
            //Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
        else
        {
            _lastShotTime += Time.deltaTime;
        }
    }

    public void TakeDamage(float damage)
    {
        //Debug.Log($"{gameObject.name}. Health = {_health}");

        _health -= damage;

        if (_health <= 0)
        {
            Invoke(nameof(Die), 0.2f);
        }
    }

    public void UpdateAnimator()
    {
        if (_animator == null || !_animator.enabled)
        {
            return;
        }

        _animator.SetFloat(vAnimatorParameters.InputMagnitude, _agent.isStopped ? 0 : 0.5f, _animationSmooth, Time.deltaTime);
    }

    public static partial class vAnimatorParameters
    {
        public static int InputMagnitude = Animator.StringToHash("InputMagnitude");
    }

    private void Die()
    {
        gameObject.SetActive(false);
        Destroy(gameObject, 10);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _sightRadius);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var collider = collision.gameObject;

        if (collider.CompareTag(Constanter.PlayerBulletTagName))
        {
            TakeDamage(collider.GetComponent<Bullet>().Damage);
        }
    }

}
