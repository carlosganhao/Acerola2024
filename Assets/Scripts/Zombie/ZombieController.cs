using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour, IDamageable
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Room _containingRoom;
    // [SerializeField] private float _detectionRadius = 5;
    // [SerializeField] private LayerMask _detenctionLayerMask;
    [SerializeField] private int _maxHealth = 5;
    [SerializeField] private float _maxVelocity = 2;
    private CharacterController _characterController;
    private int _health;
    private Transform _currentTarget;

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _health = _maxHealth;

        _containingRoom.PlayerDetected += SetTarget;
    }

    void OnDestroy()
    {
        _containingRoom.PlayerDetected -= SetTarget;
    }

    // Update is called once per frame
    void Update()
    {
        if(_currentTarget != null)
        {
            _animator.SetBool("Walking", true);
            LookAtWaypoint();
        }
    }

    void FixedUpdate()
    {
        if(_currentTarget != null)
        {
            var direction = (_currentTarget.position - transform.position).normalized;
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

    private void SetTarget(Transform transform)
    {
        _currentTarget = transform;
    }

    private void LookAtWaypoint()
    {
        Vector3 intendedDirection = _currentTarget.position - transform.position;

        float newAngle = Quaternion.LookRotation(intendedDirection, Vector3.up).eulerAngles.y;

        transform.rotation = Quaternion.Euler(0, newAngle, 0);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}
