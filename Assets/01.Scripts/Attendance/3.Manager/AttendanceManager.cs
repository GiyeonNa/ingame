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
                Debug.LogError("Attendance CSV 로드 실패");
            }
        });

        // 출석 정보 로드
        _attendance = _attendanceRepository.Load();
    }
    // 오늘 출석 처리
    public void MarkTodayAttendance()
    {
        DateTime today = DateTime.Today;
        _attendance.MarkAttendance(today);
        _attendanceRepository.Save(_attendance);
    }

    // 특정 날짜 출석 처리 (테스트/관리용)
    public void MarkAttendance(DateTime date)
    {
        _attendance.MarkAttendance(date);
        _attendanceRepository.Save(_attendance);
    }

    // 오늘 보상 수령
    public void ClaimTodayReward()
    {
        ClaimReward(DateTime.Today);
    }

    // 특정 날짜의 보상 수령
    public void ClaimReward(DateTime date)
    {
        if (!_attendance.IsAttended(date))
        {
            Debug.Log($"{date.ToShortDateString()}에는 출석 기록이 없습니다.");
            return;
        }

        if (_attendance.CanClaimReward(date))
        {
            int day = _attendance.GetAttendanceDayByDate(date);
            var rewardInfo = _attendanceRewardInfoList.Find(x => x.Day == day);
            if (rewardInfo != null)
            {
                CurrencyManager.Instance.Add(rewardInfo.RewardCurrencyType, rewardInfo.RewardValue);
                Debug.Log($"{rewardInfo.RewardCurrencyType} {rewardInfo.RewardValue} 지급 (출석 {day}일차)");
            }

            _attendance.ClaimReward(date);
            _attendanceRepository.Save(_attendance);
            Debug.Log($"보상 수령 완료: {date.ToShortDateString()}");
            AttendanceChange?.Invoke();
        }
        else
        {
            Debug.Log($"{date.ToShortDateString()}의 보상은 이미 수령하였습니다.");
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
            Debug.Log($"{day}일차 출석 기록이 없습니다.");
        }
    }



    public void ClaimAllAvailableRewards()
    {
        var claimableDates = _attendance.GetAllCanClaimRewardDates();
        foreach (var date in claimableDates)
        {
            // 실제 보상 지급 로직 필요 (예: RewardService.GiveReward(...))
            _attendance.ClaimReward(date);
            Debug.Log($"{date.ToShortDateString()} 보상 수령 완료");
        }
        if (claimableDates.Count == 0)
        {
            Debug.Log("수령 가능한 보상이 없습니다.");
        }
    }

    public DateTime? GetAttendanceDateByDay(int day)
    {
        return _attendance.GetAttendanceDateByDay(day);
    }

    public void ResetStreak()
    {
        _attendance.ResetStreak();
        // 필요시 저장 및 UI 갱신 이벤트 호출
        _attendanceRepository.Save(_attendance);
        AttendanceChange?.Invoke();
    }

    public bool TryGetRewardStatusByDay(int day, out DateTime date, out bool claimed)
    {
        return _attendance.TryGetRewardStatusByDay(day, out date, out claimed);
    }

    // 출석 정보 조회
    public int GetStreakCount() => _attendance.StreakCount;
    public DateTime GetLastAttendanceDate() => _attendance.LastAttendanceDate;
    public bool IsAttended(DateTime date) => _attendance.IsAttended(date);
    public bool CanClaimReward(DateTime date) => _attendance.CanClaimReward(date);
}
