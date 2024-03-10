using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Transform _healthIndiciatorContainer;
    [SerializeField] private GameObject _screenCrackPrefab;

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        EventBroker.PlayerHealthChanged += HealthChanged;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        EventBroker.PlayerHealthChanged += HealthChanged;
    }

    void HealthChanged(int healthChange)
    {
        if(healthChange < 0)
        {
            Instantiate(_screenCrackPrefab, _healthIndiciatorContainer);
        }
        else
        {
            _healthIndiciatorContainer.DeleteChild(0);
        }
    }
}
