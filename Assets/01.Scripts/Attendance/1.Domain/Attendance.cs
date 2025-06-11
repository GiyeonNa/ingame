using System;
using System.Collections.Generic;

public class Attendance
{
    //보상에 대한 정보?
    public readonly string Day;
    public ECurrencyType RewardCurrencyType;
    public int RewardValue;

    // 출석 기록 (연속 출석용)
    private readonly List<DateTime> _attendanceDates = new();

    // 보상 수령 기록 (누적)
    private readonly Dictionary<DateTime, bool> _rewardClaimedHistory = new();

    // 날짜별 출석 및 보상 수령 여부
    //private readonly Dictionary<DateTime, bool> _attendanceRewardClaimed = new();
    // 마지막 출석 일자
    public DateTime LastAttendanceDate { get; private set; }
    // 연속 출석 일수
    public int StreakCount { get; private set; }

    private bool _isAttendanced;
    public bool IsAttendanced => _isAttendanced;

    private bool _isRewarded;
    public bool IsRewarded => _isRewarded;

    public Attendance()
    {
        LastAttendanceDate = DateTime.MinValue;
        StreakCount = 0;
    }

    // 출석 처리
    public void MarkAttendance(DateTime date)
    {
        if (_attendanceDates.Contains(date.Date))
            return; // 이미 출석 처리된 날짜

        // 연속 출석 체크
        bool isStreak = _attendanceDates.Count > 0 && _attendanceDates[_attendanceDates.Count - 1].Date == date.AddDays(-1).Date;

        if (isStreak)
        {
            StreakCount++;
        }
        else
        {
            // 연속 출석이 아니면 출석 기록만 초기화 (보상 수령 기록은 유지)
            _attendanceDates.Clear();
            StreakCount = 1;
        }

        LastAttendanceDate = date.Date;
        _attendanceDates.Add(date.Date);

        // 보상 수령 기록이 없으면 미수령으로 추가
        if (!_rewardClaimedHistory.ContainsKey(date.Date))
            _rewardClaimedHistory[date.Date] = false;
    }

    // 특정 날짜의 보상 수령 처리
    public void ClaimReward(DateTime date)
    {
        //if (_attendanceRewardClaimed.ContainsKey(date.Date) && !_attendanceRewardClaimed[date.Date])
        //{
        //    _attendanceRewardClaimed[date.Date] = true;

        //}
        if (_rewardClaimedHistory.ContainsKey(date.Date) && !_rewardClaimedHistory[date.Date])
        {
            _rewardClaimedHistory[date.Date] = true;
        }
    }

    // 특정 날짜의 보상 수령 가능 여부
    public bool CanClaimReward(DateTime date)
    {
        //return _attendanceRewardClaimed.TryGetValue(date.Date, out bool claimed) && !claimed;
        return _rewardClaimedHistory.TryGetValue(date.Date, out bool claimed) && !claimed;
    }

    // 특정 날짜의 출석 여부
    public bool IsAttended(DateTime date)
    {
        //return _attendanceRewardClaimed.ContainsKey(date.Date);
        return _attendanceDates.Contains(date.Date);
    }

    // 수령 가능한(출석했으나 미수령) 날짜 리스트
    public List<DateTime> GetAllCanClaimRewardDates()
    {
        //var result = new List<DateTime>();
        //foreach (var kvp in _rewardClaimedHistory)
        //{
        //    if (kvp.Value) // 보상 수령 완료
        //        result.Add(kvp.Key);
        //}
        //return result;

        var result = new List<DateTime>();
        foreach (var kvp in _rewardClaimedHistory)
        {
            if (!kvp.Value) // 출석했으나 보상 미수령
                result.Add(kvp.Key);
        }
        return result;
    }


    // 이미 수령한 날짜 리스트
    public List<DateTime> GetAllClaimedRewardDates()
    {
        var result = new List<DateTime>();
        foreach (var kvp in _rewardClaimedHistory)
        {
            if (kvp.Value) // 보상 수령 완료
                result.Add(kvp.Key);
        }
        return result;
    }

    public List<AttendanceRepository.DateRewardPair> GetAttendanceRewardClaimedDict()
    {
        //var list = new List<AttendanceRepository.DateRewardPair>();
        //foreach (var kvp in _attendanceRewardClaimed)
        //{
        //    list.Add(new AttendanceRepository.DateRewardPair
        //    {
        //        Date = kvp.Key.ToString("o"), // ISO 8601 형식
        //        Claimed = kvp.Value
        //    });
        //}
        //return list;

        var list = new List<AttendanceRepository.DateRewardPair>();
        foreach (var kvp in _rewardClaimedHistory)
        {
            list.Add(new AttendanceRepository.DateRewardPair
            {
                Date = kvp.Key.ToString("o"), // ISO 8601 형식
                Claimed = kvp.Value
            });
        }
        return list;
    }


    public void LoadFromSaveData(AttendanceDTO data)
    {
        //if (data == null)
        //    return;

        //LastAttendanceDate = data.LastAttendanceDate;
        //StreakCount = data.StreakCount;
        //_attendanceRewardClaimed.Clear();
        //if (data.AttendanceRewardClaimed != null)
        //{
        //    foreach (var kvp in data.AttendanceRewardClaimed)
        //    {
        //        _attendanceRewardClaimed[kvp.Key] = kvp.Value;
        //    }
        //}

        if (data == null)
            return;

        LastAttendanceDate = data.LastAttendanceDate;
        StreakCount = data.StreakCount;
        _rewardClaimedHistory.Clear();
        if (data.AttendanceRewardClaimed != null)
        {
            foreach (var kvp in data.AttendanceRewardClaimed)
            {
                _rewardClaimedHistory[kvp.Key] = kvp.Value;
            }
        }
        // _attendanceDates는 연속 출석이 시작될 때만 채워지므로, 저장/로드 대상이 아님
    }

    public DateTime? GetAttendanceDateByDay(int day)
    {
        //var dates = new List<DateTime>(_attendanceRewardClaimed.Keys);
        //dates.Sort();
        //if (day - 1 < 0 || day - 1 >= dates.Count)
        //    return null;
        //return dates[day - 1];

        var dates = new List<DateTime>(_attendanceDates);
        dates.Sort();
        if (day - 1 < 0 || day - 1 >= dates.Count)
            return null;
        return dates[day - 1];
    }

    public int GetAttendanceDayByDate(DateTime date)
    {
        //var dates = new List<DateTime>(_attendanceRewardClaimed.Keys);
        //dates.Sort();
        //for (int i = 0; i < dates.Count; i++)
        //{
        //    if (dates[i].Date == date.Date)
        //        return i + 1; // 1-based
        //}
        //return -1; // Not found

        var dates = new List<DateTime>(_attendanceDates);
        dates.Sort();
        for (int i = 0; i < dates.Count; i++)
        {
            if (dates[i].Date == date.Date)
                return i + 1; // 1-based
        }
        return -1; // Not found
    }

    public void ResetStreak()
    {
        StreakCount = 1;
        _attendanceDates.Clear();
        LastAttendanceDate = DateTime.MinValue;

        //StreakCount = 1;
        // LastAttendanceDate는 그대로 두거나, 필요시 DateTime.MinValue로 초기화 가능
    }

}
