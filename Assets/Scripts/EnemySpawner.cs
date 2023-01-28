using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private const float SpawnRadius = 3;

    private Dictionary<Vector3, bool> _spawnPlacesAvailability;

    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private LayerMask _enemyLayer;
    [SerializeField]
    private LayerMask _playerLayer;
    [SerializeField]
    private int _enemiesNumber = 3;
    [SerializeField]
    private float _spawnDelay = 5;
    [SerializeField]
    private List<float> _remainingTimeToSpawn;
    [SerializeField]
    private List<GameObject> _enemies;

    private void Start()
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

        _spawnPlacesAvailability = new Dictionary<Vector3, bool>
        {
            { new Vector3(-4, 3, 2), true },
            { new Vector3(-4, 3, -2), true },
            { new Vector3(0, 3, 0), true },
            { new Vector3(4, 3, 2), true },
            { new Vector3(4, 3, -2), true }
        };

        for (int i = 0; i < _enemiesNumber; i++)
        {
            _remainingTimeToSpawn.Add(_spawnDelay);
            _enemies.Add(Spawn());
        }
    }

    private void Update()
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            if (!_enemies[i].activeSelf)
            {
                _spawnPlacesAvailability[_enemies[i].GetComponent<EnemyAI>().SpawnPlace] = true;
                _remainingTimeToSpawn[i] -= Time.deltaTime;

                if (_remainingTimeToSpawn[i] <= 0)
                {
                    _enemies[i] = Spawn();
                    _remainingTimeToSpawn[i] = _spawnDelay;
                }
            }
            else
            {
                _spawnPlacesAvailability[_enemies[i].GetComponent<EnemyAI>().SpawnPlace] = false;
            }
        }
    }

    private GameObject Spawn()
    {
        Vector3 spawnPlace = Vector3.zero;

        for (int count = 0; spawnPlace == Vector3.zero; count++)
        {
            var placeIndex = Random.Range(0, _spawnPlacesAvailability.Count);
            var spawnPlaces = _spawnPlacesAvailability.Keys.ToList();

            if (!Physics.CheckSphere(spawnPlaces[placeIndex], SpawnRadius, _enemyLayer) && 
                !Physics.CheckSphere(spawnPlaces[placeIndex], SpawnRadius, _playerLayer) &&
                _spawnPlacesAvailability[spawnPlaces[placeIndex]] || count >= 10)
            {
                spawnPlace = spawnPlaces[placeIndex];
            }
        }

        _spawnPlacesAvailability[spawnPlace] = false;
        return Instantiate(_enemyPrefab, spawnPlace, Quaternion.identity);
    }
}
