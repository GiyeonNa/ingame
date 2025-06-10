using System;
using System.Collections.Generic;
using System.Linq;
using Unity.FPS.Game;
using Unity.VisualScripting;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    private Dictionary<ECurrencyType, Currency> _currencies = new Dictionary<ECurrencyType, Currency>();

    public event Action OnCurrencyChanged;

    private CurrencyRepository _currencyRepository;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        Init();
    }

    private void OnEnable()
    {
        EventManager.AddListener<CurrencyEvent>(OnCurrencyEarned);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener<CurrencyEvent>(OnCurrencyEarned);
    }

    private void OnCurrencyEarned(CurrencyEvent evt)
    {
        Add(evt.CurrencyType, evt.Amount);
    }


    private void Init()
    {
        _currencyRepository = new CurrencyRepository();
        List<CurrencyDTO> savedCurrencies = _currencyRepository.Load();

        if(savedCurrencies==null)
        {
            for (int i = 0; i < (int)ECurrencyType.Count; ++i)
            {
                ECurrencyType type = (ECurrencyType)i;
                Currency currency = new Currency(type, 0);
                _currencies.Add(type, currency);
            }
        }
        else
        {
            foreach (var data in savedCurrencies)
            {
                Currency currency = new Currency(data.Type, data.Value);
                _currencies.Add(currency.Type, currency);
            }
        }
    }

    public CurrencyDTO Get(ECurrencyType type)
    {
        return new CurrencyDTO(_currencies[type]);
    }

    private List<CurrencyDTO> ToDTOList()
    {
        return _currencies.ToList().ConvertAll(pair => new CurrencyDTO(pair.Value));
    }

    public void Add(ECurrencyType type, int value)
    {
        if (_currencies.TryGetValue(type, out Currency currency))
        {
            currency.Add(value);
            Debug.Log($"Added {value} to {type}. New value: {currency.Value}");
        }
        else
        {
            Debug.LogWarning($"Currency type {type} not found.");
        }

        _currencyRepository.Save(ToDTOList());
        AchievementManager.Instance.Increase(type, value);
        OnCurrencyChanged?.Invoke();
    }

    public void Subtract(ECurrencyType type, int value)
    {
        if (_currencies.TryGetValue(type, out Currency currency))
        {
            currency.Subtract(value);
            Debug.Log($"Subtracted {value} from {type}. New value: {currency.Value}");
        }
        else
        {
            Debug.LogWarning($"Currency type {type} not found.");
        }

        _currencyRepository.Save(ToDTOList());
        OnCurrencyChanged?.Invoke();
    }

    public bool TryBuy(ECurrencyType type, int value)
    {
        if (!_currencies[type].TryBuy(value))
        {
            return false;
        }
        else
        {
            OnCurrencyChanged?.Invoke();
            _currencyRepository.Save(ToDTOList());
            return true;
        }
    }
}
