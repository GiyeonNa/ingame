using UnityEngine;

[CreateAssetMenu(fileName = "AchievementSO", menuName = "Scriptable Objects/AchievementSO")]
public class AchievementSO : ScriptableObject
{
    [SerializeField]
    private string _id;
    public string Id => _id;

    [SerializeField]
    private string _name;
    public string Name => _name;

    [SerializeField]
    private string _description;
    public string Description => _description;

    [SerializeField]
    private EAchievementCondition _condition;
    public EAchievementCondition Condition => _condition;

    [SerializeField]
    private int _goalValue;
    public int GoalValue => _goalValue;

    [SerializeField]
    private ECurrencyType _rewardCurrencyType;
    public ECurrencyType RewardCurrencyType => _rewardCurrencyType;

    [SerializeField]
    private int _rewardValue;
    public int RewardValue => _rewardValue;

}
