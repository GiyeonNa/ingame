using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Attendance : MonoBehaviour
{
    [SerializeField]
    private List<UI_AttendanceSlot> _attendanceSlots = new List<UI_AttendanceSlot>();
    [SerializeField]
    private TextMeshProUGUI _attendanceStreak;

    [SerializeField]
    private Button buttonTest;
    [SerializeField]
    private TMP_InputField inputDate; // 원하는 날짜 입력용
    [SerializeField]
    private Button buttonNext;
    [SerializeField]
    private Button butonClear;
    [SerializeField]
    private Button buttonResetStreak;

    private void Awake()
    {
        if (buttonTest != null)
        {
            buttonTest.onClick.AddListener(() =>
            {
                // 테스트용: 오늘 날짜로 출석 처리
                AttendanceManager.Instance.MarkTodayAttendance();
                Refresh();
            });
        }

        //출석한 날짜의 다음날로 -> 지금날의 다음날?
        if (buttonNext != null && inputDate != null)
        {
            buttonNext.onClick.AddListener(() =>
            {
                if (DateTime.TryParse(inputDate.text, out DateTime customDate))
                {
                    AttendanceManager.Instance.MarkAttendance(customDate);
                    Refresh();
                }
                else
                {
                    Debug.LogWarning("날짜 형식이 올바르지 않습니다. 예: 2025-06-15");
                }
            });
        }


        if (butonClear != null)
        {
            butonClear.onClick.AddListener(() =>
            {
                // 저장된 출석 데이터 초기화
                new AttendanceRepository().Reset();
                // AttendanceManager의 출석 정보도 새로 로드
                typeof(AttendanceManager)
                    .GetField("_attendance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(AttendanceManager.Instance, new AttendanceRepository().Load());
                Refresh();
            });
        }

        if (buttonResetStreak != null)
        {
            buttonResetStreak.onClick.AddListener(() =>
            {
                AttendanceManager.Instance.ResetStreak();
                Refresh();
            });
        }
    }

    private void Start()
    {
        // 초기 데이터 설정
        Refresh();
        AttendanceManager.Instance.AttendanceChange += Refresh;
    }

    private void Refresh()
    {
        List<AttendanceRewardInfo> attendances = AttendanceManager.Instance.AttendanceRewardInfoList;
        for (int i = 0; i < _attendanceSlots.Count; i++)
        {
            if (i < attendances.Count)
            {
                int day = attendances[i].Day;
                DateTime? date = AttendanceManager.Instance.GetAttendanceDateByDay(day);

                bool isClaimed = false;
                bool isClaimable = false;

                if (date.HasValue)
                {
                    isClaimed = !AttendanceManager.Instance.CanClaimReward(date.Value); // 이미 수령했으면 true
                    isClaimable = AttendanceManager.Instance.CanClaimReward(date.Value); // 수령 가능하면 true
                }

                _attendanceSlots[i].gameObject.SetActive(true);
                _attendanceSlots[i].SetData(attendances[i], isClaimed, isClaimable);
            }
            else
            {
                _attendanceSlots[i].gameObject.SetActive(false);
            }
        }

        if (_attendanceStreak != null)
        {
            int streak = AttendanceManager.Instance.GetStreakCount();
            _attendanceStreak.text = $"연속 출석: {streak}일";
        }
    }
}
