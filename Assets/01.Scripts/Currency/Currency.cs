using UnityEngine;

public enum ECurrencyType
{
    Gold,
    Diamond,
    Gem,

    Count
}

public class Currency 
{
    private ECurrencyType _type;
    public ECurrencyType Type
    {
        get => _type;
        set
        {
            if (!System.Enum.IsDefined(typeof(ECurrencyType), value))
            {
                return;
            }
            _type = value;
        }
    }

    private int _value = 0;
    public int Value
    {
        get => _value;
        set
        {
            if (value < 0)
            {
                return;
            }
            _value = value;
        }
    }

    public Currency(ECurrencyType type, int value)
    {
        if(value < 0)
        {
            value = 0;
        }

        Type = type;
        Value = value;
    }

    public void Add(int value)
    {
        if (value < 0)
        {
            return;
        }
        Value += value;
    }

    public void Subtract(int value)
    {
        if (value < 0)
        {
            return;
        }
        Value -= value;
        if (Value < 0)
        {
            Value = 0;
        }
    }

    public bool TryBuy(int value)
    {
        if (value < 0)
        {
            throw new System.Exception("Cannot buy with negative value.");
        }

        if(_value < value)
        {
            return false;
        }
        _value -= value;
        return true;
    }
}
