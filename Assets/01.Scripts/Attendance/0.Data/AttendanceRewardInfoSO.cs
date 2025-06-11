using UnityEngine;

[CreateAssetMenu(fileName = "AttendanceRewardInfoSO", menuName = "Attendance/RewardInfo", order = 0)]
public class AttendanceRewardInfoSO : ScriptableObject
{
    [Header("출석 일차 (1일부터 시작)")]
    public int Day;

    [Header("보상 타입")]
    public ECurrencyType RewardCurrencyType;

    [Header("보상 수치")]
    public int RewardValue;

    [Header("보상 아이콘")]
    public Sprite RewardIcon;
    // 필요시 아이템ID, 설명 등 추가 가능
}
