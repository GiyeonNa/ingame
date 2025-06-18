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
    private TMP_InputField inputDate; // ���ϴ� ��¥ �Է¿�
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
                // �׽�Ʈ��: ���� ��¥�� �⼮ ó��
                AttendanceManager.Instance.MarkTodayAttendance();
                Refresh();
            });
        }

        //�⼮�� ��¥�� �������� -> ���ݳ��� ������?
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
                    Debug.LogWarning("��¥ ������ �ùٸ��� �ʽ��ϴ�. ��: 2025-06-15");
                }
            });
        }


        if (butonClear != null)
        {
            butonClear.onClick.AddListener(() =>
            {
                // ����� �⼮ ������ �ʱ�ȭ
                new AttendanceRepository().Reset();
                // AttendanceManager�� �⼮ ������ ���� �ε�
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
        // �ʱ� ������ ����
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
                    isClaimed = !AttendanceManager.Instance.CanClaimReward(date.Value); // �̹� ���������� true
                    isClaimable = AttendanceManager.Instance.CanClaimReward(date.Value); // ���� �����ϸ� true
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
            _attendanceStreak.text = $"���� �⼮: {streak}��";
        }
    }
}
