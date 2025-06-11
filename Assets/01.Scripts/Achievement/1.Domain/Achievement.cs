using System;
using UnityEngine;
using static AchievementManager;

public enum EAchievementCondition
{
    GoldCollect = 0,
    DronKillCount = 1,
    BossKillCount = 2,
    PlayTime = 3,
    Trigger = 4
}

public class Achievement 
{
    public readonly string Id;
    public readonly string Name;
    public readonly string Description;
    public readonly EAchievementCondition Condition;
    public int GoalValue;
    public ECurrencyType RewardCurrencyType;
    public int RewardValue;


    private int _currentValue;
    public int CurrentValue => _currentValue;

    private bool _isCompleted;
    public bool IsCompleted => _isCompleted;

    private bool _isRewarded;
    public bool IsRewarded => _isRewarded;

    //public Achievement(AchievementSO achievementSO)
    //{
    //    if (achievementSO == null)
    //    {
    //        throw new ArgumentNullException(nameof(achievementSO), "AchievementSO cannot be null");
    //    }
    //    //ID는 중복이 안됨

    //    Id = achievementSO.Id;
    //    Name = achievementSO.Name;
    //    Description = achievementSO.Description;
    //    Condition = achievementSO.Condition;
    //    GoalValue = achievementSO.GoalValue;
    //    RewardCurrencyType = achievementSO.RewardCurrencyType;
    //    RewardValue = achievementSO.RewardValue;
    //    _currentValue = 0;
    //    _isCompleted = false;
    //}

    //public Achievement(AchievementSO metaData, AchievementSaveData saveData)
    //{
    //    if (metaData == null)
    //    {
    //        throw new ArgumentNullException(nameof(metaData), "AchievementSO cannot be null");
    //    }

    //    Id = metaData.Id;
    //    Name = metaData.Name;
    //    Description = metaData.Description;
    //    Condition = metaData.Condition;
    //    GoalValue = metaData.GoalValue;
    //    RewardCurrencyType = metaData.RewardCurrencyType;
    //    RewardValue = metaData.RewardValue;

    //    _currentValue = saveData.CurrentValue;
    //    _isCompleted = saveData.IsCompleted;
    //    _isRewarded = saveData.IsRewarded;
    //}

    public Achievement(AchievementCSVData data)
    {
        Id = data.Id;
        Name = data.Name;
        Description = data.Description;
        Condition = data.Condition;
        GoalValue = data.GoalValue;
        RewardCurrencyType = data.RewardCurrencyType;
        RewardValue = data.RewardValue;
        // 나머지 필드 초기화
    }

    public Achievement(AchievementCSVData data, AchievementSaveData saveData)
    {
        Id = data.Id;
        Name = data.Name;
        Description = data.Description;
        Condition = data.Condition;
        GoalValue = data.GoalValue;
        RewardCurrencyType = data.RewardCurrencyType;
        RewardValue = data.RewardValue;

        _currentValue = saveData.CurrentValue;
        _isCompleted = saveData.IsCompleted;
        _isRewarded = saveData.IsRewarded;
    }

    public void Increase(int value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Increase value cannot be negative");
        }
        _currentValue += value;
        if (_currentValue >= GoalValue)
        {
            _isCompleted = true;
            // Trigger reward logic here, e.g., give currency to player
            Debug.Log($"Achievement '{Name}' completed! Reward: {RewardValue} of {RewardCurrencyType}");
        }
    }

    public bool IsCanClaimReward()
    {
        return _isCompleted && !IsRewarded;
    }

    public bool TryClaimReward()
    {
        if (IsCanClaimReward())
        {
            _isRewarded = true;
            // Logic to give the reward to the player, e.g., CurrencyManager.Instance.Add(RewardCurrencyType, RewardValue);
            Debug.Log($"Reward claimed for achievement '{Name}': {RewardValue} of {RewardCurrencyType}");
            return true;
        }
        else
        {
            Debug.LogWarning($"Cannot claim reward for achievement '{Name}'. IsCompleted: {_isCompleted}, isRewarded: {IsRewarded}");
            return false;
        }
    }
}
