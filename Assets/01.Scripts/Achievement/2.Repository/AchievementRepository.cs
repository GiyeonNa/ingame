using System;
using System.Collections.Generic;
using UnityEngine;

public class AchievementRepository
{
    private const string SAVE_KEY = nameof(AchievementRepository);

    public void Save(List<AchievementDTO> datas)
    {
        AchievementSaveDataList data = new AchievementSaveDataList();
        data.DataList = datas.ConvertAll(dto => new AchievementSaveData
        {
            Id = dto.Id,
            CurrentValue = dto.CurrentValue,
            IsCompleted = dto.IsCompleted,
            IsRewarded = dto.IsRewarded
        });

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
    }

    public List<AchievementSaveData> Load()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            return null;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        AchievementSaveDataList data = JsonUtility.FromJson<AchievementSaveDataList>(json);
    
        return data.DataList ?? new List<AchievementSaveData>();
    }
}

[Serializable]
public struct AchievementSaveData
{
    public string Id;
    public int CurrentValue;
    public bool IsCompleted;
    public bool IsRewarded;
}

[Serializable]
public class AchievementSaveDataList
{
    public List<AchievementSaveData> DataList;
}
