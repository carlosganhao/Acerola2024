using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private List<Transform> _spawnPositions;
    [SerializeField] private List<GameObject> _itemsPrefabs;
    private GameObject _spawnedObject;

    // Start is called before the first frame update
    void Start()
    {
        EventBroker.DetachChaser += DetachChaser;
    }

    private void DetachChaser()
    {
        SpawnObject();
    }

    private void SpawnObject()
    {
        _spawnedObject = Instantiate(_itemsPrefabs[Random.Range(0, _itemsPrefabs.Count)], _spawnPositions[Random.Range(0, _spawnPositions.Count)]);
    }

    private void ObjectPickedUp()
    {
        SpawnObject();
    }
}
