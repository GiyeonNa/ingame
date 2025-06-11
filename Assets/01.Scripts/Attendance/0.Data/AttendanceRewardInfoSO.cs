using UnityEngine;

[CreateAssetMenu(fileName = "AttendanceRewardInfoSO", menuName = "Attendance/RewardInfo", order = 0)]
public class AttendanceRewardInfoSO : ScriptableObject
{
    [Header("�⼮ ���� (1�Ϻ��� ����)")]
    public int Day;

    [Header("���� Ÿ��")]
    public ECurrencyType RewardCurrencyType;

    [Header("���� ��ġ")]
    public int RewardValue;

    [Header("���� ������")]
    public Sprite RewardIcon;
    // �ʿ�� ������ID, ���� �� �߰� ����
}
