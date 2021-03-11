using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private List<Vector3> _spawnPlaces;
    [SerializeField]
    private int _enemiesNumber = 3;
    [SerializeField]
    private float _spawnDelay = 5;
    [SerializeField]
    private List<float> _remainingTimeToSpawn;
    [SerializeField]
    private List<GameObject> _enemies;
    [SerializeField]
    private LayerMask _enemyLayer;
    [SerializeField]
    private LayerMask _playerLayer;

    private const float SpawnRadius = 3;

    void Start()
    {
        _enemies = new List<GameObject>();
        _remainingTimeToSpawn = new List<float>();

        if (_enemyLayer == 0)
        {
            _enemyLayer = LayerMask.GetMask(Constanter.EnemyLayerName);
        }
        if (_playerLayer == 0)
        {
            _playerLayer = LayerMask.GetMask(Constanter.PlayerLayerName);
        }

        for (int i = 0; i < _enemiesNumber; i++)
        {
            _remainingTimeToSpawn.Add(_spawnDelay);
            _enemies.Add(Spawn());
        }
    }

    void Update()
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            if (!_enemies[i])
            {
                _remainingTimeToSpawn[i] -= Time.deltaTime;

                if (_remainingTimeToSpawn[i] <= 0)
                {
                    _enemies[i] = Spawn();
                    _remainingTimeToSpawn[i] = _spawnDelay;
                }
            }
        }
    }

    private GameObject Spawn()
    {
        Vector3 spawnPlace = Vector3.zero;
        int placeIndex;

        for (; spawnPlace == Vector3.zero;)
        {
            placeIndex = Random.Range(0, 5);

            if (!Physics.CheckSphere(_spawnPlaces[placeIndex], SpawnRadius, _enemyLayer) && 
                !Physics.CheckSphere(_spawnPlaces[placeIndex], SpawnRadius, _playerLayer))
            {
                spawnPlace = _spawnPlaces[placeIndex];
            }
        }

        return Instantiate(_enemyPrefab, spawnPlace, Quaternion.identity);
    }
}
