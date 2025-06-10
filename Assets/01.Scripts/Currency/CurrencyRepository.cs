using System;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyRepository
{
    private const string SAVE_KEY = nameof(CurrencyRepository);

    public void Save(List<CurrencyDTO> datas)
    {
        CurrencySaveDatas data = new CurrencySaveDatas();
        data.DataList = datas.ConvertAll(datas => new CurrencySaveData
        {
            Type = datas.Type,
            Value = datas.Value
        });

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
    }

    public List<CurrencyDTO> Load()
    {
        if(!PlayerPrefs.HasKey(SAVE_KEY))
        {
            return null;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        CurrencySaveDatas data = JsonUtility.FromJson<CurrencySaveDatas>(json);


        return data.DataList.ConvertAll(d => new CurrencyDTO(d.Type, d.Value));
    }
}


[Serializable]
public struct CurrencySaveData
{
    public ECurrencyType Type;
    public int Value;
}

[Serializable]
public class CurrencySaveDatas
{
    public List<CurrencySaveData> DataList; // 필드로 선언, 타입 일치
}
