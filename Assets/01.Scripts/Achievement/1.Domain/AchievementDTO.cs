using UnityEngine;

public class AchievementDTO
{
    public readonly string Id;
    public readonly string Name;
    public readonly string Description;
    public readonly EAchievementCondition Condition;
    public readonly int GoalValue;
    public readonly ECurrencyType RewardCurrencyType;
    public readonly int RewardValue;
    public readonly int CurrentValue;
    public readonly bool IsCompleted;
    public readonly bool IsRewarded; 

    public AchievementDTO(
        string id,
        string name,
        string description,
        EAchievementCondition condition,
        int goalValue,
        ECurrencyType rewardCurrencyType,
        int rewardValue,
        int currentValue,
        bool isCompleted,
        bool isRewarded) 
    {
        Id = id;
        Name = name;
        Description = description;
        Condition = condition;
        GoalValue = goalValue;
        RewardCurrencyType = rewardCurrencyType;
        RewardValue = rewardValue;
        CurrentValue = currentValue;
        IsCompleted = isCompleted;
        IsRewarded = isRewarded; 
    }

    public AchievementDTO(Achievement achievement)
        : this(
            achievement.Id,
            achievement.Name,
            achievement.Description,
            achievement.Condition,
            achievement.GoalValue,
            achievement.RewardCurrencyType,
            achievement.RewardValue,
            achievement.CurrentValue,
            achievement.IsCompleted,
            achievement.IsRewarded) 
    {
    }

    public bool IsCanClaimReward()
    {
        return IsCompleted && !IsRewarded; 
    }
}
