using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ZombieController : MonoBehaviour, IDamageable
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Room _containingRoom;
    // [SerializeField] private float _detectionRadius = 5;
    // [SerializeField] private LayerMask _detenctionLayerMask;
    [SerializeField] private int _maxHealth = 5;
    [SerializeField] private float _maxVelocity = 2;
    [SerializeField] private float _attackCooldown = 1;
    private CharacterController _characterController;
    private CinemachineImpulseSource _impulseSource;
    private AudioSource _audioSource;
    private float _attackElapsedTime;
    private int _health;
    private Transform _currentTarget;

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _health = _maxHealth;
        _attackElapsedTime = _attackCooldown;

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

            var colliders = Physics.OverlapBox(transform.position + transform.forward * 0.25f, new Vector3(0.25f, 0.25f, 0.25f), transform.rotation);
        
            foreach (var collider in colliders)
            {
                if(collider.gameObject.layer == 6 && _attackElapsedTime < 0)
                    {
                        collider.GetComponent<PlayerController>().TakeDamage(1);
                        _impulseSource.GenerateImpulse(4);
                        _animator.SetTrigger("Stab");
                        _attackElapsedTime = _attackCooldown;
                    }
            }
        }

        _attackElapsedTime -= Time.deltaTime;
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
        _audioSource.PlayDelayed(Random.Range(0.0f, 0.1f));
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
