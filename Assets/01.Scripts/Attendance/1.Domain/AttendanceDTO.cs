using System;
using System.Collections.Generic;

public class AttendanceDTO
{
    // ��¥�� �⼮ �� ���� ���� ����
    public Dictionary<DateTime, bool> AttendanceRewardClaimed { get; set; } = new();

    public DateTime LastAttendanceDate { get; set; }
    // ���� �⼮ �ϼ�
    public int StreakCount { get; set; }

    private bool _isAttendanced;
    public bool IsAttendanced => _isAttendanced;

    private bool _isRewarded;
    public bool IsRewarded => _isRewarded;

    // Ư�� ��¥�� ������ ���� �� �ִ��� Ȯ��
    public bool CanClaimReward(DateTime date)
    {
        // �⼮�� �߰�, ���� ������ ���� ���� ���
        return AttendanceRewardClaimed.TryGetValue(date.Date, out bool claimed) && !claimed;
    }

    // Ư�� ��¥�� ���� ���� ó��
    public void ClaimReward(DateTime date)
    {
        if (AttendanceRewardClaimed.ContainsKey(date.Date) && !AttendanceRewardClaimed[date.Date])
        {
            AttendanceRewardClaimed[date.Date] = true;
        }
    }
}
