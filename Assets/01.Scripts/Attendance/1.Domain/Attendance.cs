using System;
using System.Collections.Generic;

public class Attendance
{
    //���� ���� ����?
    public readonly string Day;
    public ECurrencyType RewardCurrencyType;
    public int RewardValue;

    // �⼮ ��� (���� �⼮��)
    private readonly List<DateTime> _attendanceDates = new();

    // ���� ���� ��� (����)
    private readonly Dictionary<DateTime, bool> _rewardClaimedHistory = new();

    // ��¥�� �⼮ �� ���� ���� ����
    //private readonly Dictionary<DateTime, bool> _attendanceRewardClaimed = new();
    // ������ �⼮ ����
    public DateTime LastAttendanceDate { get; private set; }
    // ���� �⼮ �ϼ�
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

    // �⼮ ó��
    public void MarkAttendance(DateTime date)
    {
        if (_attendanceDates.Contains(date.Date))
            return; // �̹� �⼮ ó���� ��¥

        // ���� �⼮ üũ
        bool isStreak = _attendanceDates.Count > 0 && _attendanceDates[_attendanceDates.Count - 1].Date == date.AddDays(-1).Date;

        if (isStreak)
        {
            StreakCount++;
        }
        else
        {
            // ���� �⼮�� �ƴϸ� �⼮ ��ϸ� �ʱ�ȭ (���� ���� ����� ����)
            _attendanceDates.Clear();
            StreakCount = 1;
        }

        LastAttendanceDate = date.Date;
        _attendanceDates.Add(date.Date);

        // ���� ���� ����� ������ �̼������� �߰�
        if (!_rewardClaimedHistory.ContainsKey(date.Date))
            _rewardClaimedHistory[date.Date] = false;
    }

    // Ư�� ��¥�� ���� ���� ó��
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

    // Ư�� ��¥�� ���� ���� ���� ����
    public bool CanClaimReward(DateTime date)
    {
        //return _attendanceRewardClaimed.TryGetValue(date.Date, out bool claimed) && !claimed;
        return _rewardClaimedHistory.TryGetValue(date.Date, out bool claimed) && !claimed;
    }

    // Ư�� ��¥�� �⼮ ����
    public bool IsAttended(DateTime date)
    {
        //return _attendanceRewardClaimed.ContainsKey(date.Date);
        return _attendanceDates.Contains(date.Date);
    }

    // ���� ������(�⼮������ �̼���) ��¥ ����Ʈ
    public List<DateTime> GetAllCanClaimRewardDates()
    {
        //var result = new List<DateTime>();
        //foreach (var kvp in _rewardClaimedHistory)
        //{
        //    if (kvp.Value) // ���� ���� �Ϸ�
        //        result.Add(kvp.Key);
        //}
        //return result;

        var result = new List<DateTime>();
        foreach (var kvp in _rewardClaimedHistory)
        {
            if (!kvp.Value) // �⼮������ ���� �̼���
                result.Add(kvp.Key);
        }
        return result;
    }


    // �̹� ������ ��¥ ����Ʈ
    public List<DateTime> GetAllClaimedRewardDates()
    {
        var result = new List<DateTime>();
        foreach (var kvp in _rewardClaimedHistory)
        {
            if (kvp.Value) // ���� ���� �Ϸ�
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
        //        Date = kvp.Key.ToString("o"), // ISO 8601 ����
        //        Claimed = kvp.Value
        //    });
        //}
        //return list;

        var list = new List<AttendanceRepository.DateRewardPair>();
        foreach (var kvp in _rewardClaimedHistory)
        {
            list.Add(new AttendanceRepository.DateRewardPair
            {
                Date = kvp.Key.ToString("o"), // ISO 8601 ����
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
        // _attendanceDates�� ���� �⼮�� ���۵� ���� ä�����Ƿ�, ����/�ε� ����� �ƴ�
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
        // LastAttendanceDate�� �״�� �ΰų�, �ʿ�� DateTime.MinValue�� �ʱ�ȭ ����
    }

}
