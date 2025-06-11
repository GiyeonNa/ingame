using System;
using System.Collections.Generic;

public class AttendanceDTO
{
    // 날짜별 출석 및 보상 수령 여부
    public Dictionary<DateTime, bool> AttendanceRewardClaimed { get; set; } = new();

    public DateTime LastAttendanceDate { get; set; }
    // 연속 출석 일수
    public int StreakCount { get; set; }

    private bool _isAttendanced;
    public bool IsAttendanced => _isAttendanced;

    private bool _isRewarded;
    public bool IsRewarded => _isRewarded;

    // 특정 날짜의 보상을 받을 수 있는지 확인
    public bool CanClaimReward(DateTime date)
    {
        // 출석은 했고, 아직 보상을 받지 않은 경우
        return AttendanceRewardClaimed.TryGetValue(date.Date, out bool claimed) && !claimed;
    }

    // 특정 날짜의 보상 수령 처리
    public void ClaimReward(DateTime date)
    {
        if (AttendanceRewardClaimed.ContainsKey(date.Date) && !AttendanceRewardClaimed[date.Date])
        {
            AttendanceRewardClaimed[date.Date] = true;
        }
    }
}
