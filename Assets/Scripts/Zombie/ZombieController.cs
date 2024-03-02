using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour, IDamageable
{
    [SerializeField] private float _detectionRadius = 5;
    [SerializeField] private LayerMask _detenctionLayerMask;
    [SerializeField] private int _maxHealth = 5;
    [SerializeField] private float _maxVelocity = 2;
    private CharacterController _characterController;
    private int _health;
    private Vector3 _currentTargetPosition;

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _health = _maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] detectedEntities = Physics.OverlapSphere(transform.position, _detectionRadius, _detenctionLayerMask);

        if(detectedEntities.Length > 0)
        {
            _currentTargetPosition = detectedEntities[0].transform.position;
        }
    }

    void FixedUpdate()
    {
        if(_currentTargetPosition != Vector3.zero)
        {
            var direction = (_currentTargetPosition - transform.position).normalized;
            _characterController.SimpleMove(_maxVelocity * direction);
        }
    }

    public void TakeDamage(GameObject source, int damage)
    {
        _health -= damage;

        if(_health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}
