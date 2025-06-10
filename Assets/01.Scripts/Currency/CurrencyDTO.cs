using UnityEngine;

public class CurrencyDTO
{

    public readonly ECurrencyType Type;
    public readonly int Value;

    public CurrencyDTO(ECurrencyType type, int value)
    {
        Type = type;
        Value = value;
    }

    public CurrencyDTO(Currency currency)
    {
        Type = currency.Type;
        Value = currency.Value;
    }

    public bool HaveEnough(int amount)
    {
        return Value >= amount;
    }
}
