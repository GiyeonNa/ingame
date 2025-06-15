using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

public class AttendanceRewardInfo
{
    public int Day;
    public ECurrencyType RewardCurrencyType;
    public int RewardValue;
}


public class AttendanceManager : MonoBehaviour
{
    public static AttendanceManager Instance { get; private set; }

    private Attendance _attendance = new Attendance();

    private List<AttendanceRewardInfo> _attendanceRewardInfoList = new List<AttendanceRewardInfo>();
    public List<AttendanceRewardInfo> AttendanceRewardInfoList => _attendanceRewardInfoList;


    public event Action AttendanceChange;
    private AttendanceRepository _attendanceRepository = new AttendanceRepository();

    private string _csvAddress = "AttendanceCSV"; // Addressable key for the CSV


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(LoadAttendanceCSV());
        }
        else
        {
            Destroy(gameObject); ;
        }

    }

    private System.Collections.IEnumerator LoadAttendanceCSV()
    {
        yield return CSVReader.ParseFromAddressable<AttendanceCSVData>(_csvAddress, list =>
        {
            if (list != null)
            {
                _attendanceRewardInfoList = new List<AttendanceRewardInfo>();
                foreach (var data in list)
                {
                    _attendanceRewardInfoList.Add(new AttendanceRewardInfo
                    {
                        Day = data.Day,
                        RewardCurrencyType = data.RewardCurrencyType,
                        RewardValue = data.RewardValue
                    });
                }
                AttendanceChange?.Invoke();
            }
            else
            {
                Debug.LogError("Attendance CSV �ε� ����");
            }
        });

        // �⼮ ���� �ε�
        _attendance = _attendanceRepository.Load();
    }
    // ���� �⼮ ó��
    public void MarkTodayAttendance()
    {
        DateTime today = DateTime.Today;
        _attendance.MarkAttendance(today);
        _attendanceRepository.Save(_attendance);
    }

    // Ư�� ��¥ �⼮ ó�� (�׽�Ʈ/������)
    public void MarkAttendance(DateTime date)
    {
        _attendance.MarkAttendance(date);
        _attendanceRepository.Save(_attendance);
    }

    // ���� ���� ����
    public void ClaimTodayReward()
    {
        ClaimReward(DateTime.Today);
    }

    // Ư�� ��¥�� ���� ����
    public void ClaimReward(DateTime date)
    {
        if (!_attendance.IsAttended(date))
        {
            Debug.Log($"{date.ToShortDateString()}���� �⼮ ����� �����ϴ�.");
            return;
        }

        if (_attendance.CanClaimReward(date))
        {
            int day = _attendance.GetAttendanceDayByDate(date);
            var rewardInfo = _attendanceRewardInfoList.Find(x => x.Day == day);
            if (rewardInfo != null)
            {
                CurrencyManager.Instance.Add(rewardInfo.RewardCurrencyType, rewardInfo.RewardValue);
                Debug.Log($"{rewardInfo.RewardCurrencyType} {rewardInfo.RewardValue} ���� (�⼮ {day}����)");
            }

            _attendance.ClaimReward(date);
            _attendanceRepository.Save(_attendance);
            Debug.Log($"���� ���� �Ϸ�: {date.ToShortDateString()}");
            AttendanceChange?.Invoke();
        }
        else
        {
            Debug.Log($"{date.ToShortDateString()}�� ������ �̹� �����Ͽ����ϴ�.");
        }
    }

    public void ClaimReward(int day)
    {
        DateTime? date = _attendance.GetAttendanceDateByDay(day);
        if (date.HasValue)
        {
            ClaimReward(date.Value);
        }
        else
        {
            Debug.Log($"{day}���� �⼮ ����� �����ϴ�.");
        }
    }



    public void ClaimAllAvailableRewards()
    {
        var claimableDates = _attendance.GetAllCanClaimRewardDates();
        foreach (var date in claimableDates)
        {
            // ���� ���� ���� ���� �ʿ� (��: RewardService.GiveReward(...))
            _attendance.ClaimReward(date);
            Debug.Log($"{date.ToShortDateString()} ���� ���� �Ϸ�");
        }
        if (claimableDates.Count == 0)
        {
            Debug.Log("���� ������ ������ �����ϴ�.");
        }
    }

    public DateTime? GetAttendanceDateByDay(int day)
    {
        return _attendance.GetAttendanceDateByDay(day);
    }

    public void ResetStreak()
    {
        _attendance.ResetStreak();
        // �ʿ�� ���� �� UI ���� �̺�Ʈ ȣ��
        _attendanceRepository.Save(_attendance);
        AttendanceChange?.Invoke();
    }

    public bool TryGetRewardStatusByDay(int day, out DateTime date, out bool claimed)
    {
        return _attendance.TryGetRewardStatusByDay(day, out date, out claimed);
    }

    // �⼮ ���� ��ȸ
    public int GetStreakCount() => _attendance.StreakCount;
    public DateTime GetLastAttendanceDate() => _attendance.LastAttendanceDate;
    public bool IsAttended(DateTime date) => _attendance.IsAttended(date);
    public bool CanClaimReward(DateTime date) => _attendance.CanClaimReward(date);
}
