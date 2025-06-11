using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AttendanceRepository
{
    private const string AttendanceKey = "AttendanceData";

    // �⼮ ���� ����
    public void Save(Attendance attendance)
    {
        // ��¥�� �⼮ �� ���� ���� ���θ� ����ȭ
        var data = new AttendanceSaveData
        {
            LastAttendanceDate = attendance.LastAttendanceDate,
            StreakCount = attendance.StreakCount,
            AttendanceRewardClaimed = attendance.GetAttendanceRewardClaimedDict()
        };
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(AttendanceKey, json);
        PlayerPrefs.Save();
    }

    // �⼮ ���� �ҷ�����
    //public Attendance Load()
    //{
    //    if (!PlayerPrefs.HasKey(AttendanceKey))
    //        return new Attendance();

    //    string json = PlayerPrefs.GetString(AttendanceKey);
    //    var data = JsonUtility.FromJson<AttendanceSaveData>(json);
    //    var attendance = new Attendance();
    //    attendance.LoadFromSaveData(data);
    //    return attendance;
    //}
    public Attendance Load()
    {
        if (!PlayerPrefs.HasKey(AttendanceKey))
            return new Attendance();

        string json = PlayerPrefs.GetString(AttendanceKey);
        var saveData = JsonUtility.FromJson<AttendanceSaveData>(json);

        // �⼮ ��Ͽ��� ���� �ֱ� ��¥ ���
        DateTime lastAttendanceDate = DateTime.MinValue;
        if (saveData.AttendanceRewardClaimed != null && saveData.AttendanceRewardClaimed.Count > 0)
        {
            lastAttendanceDate = saveData.AttendanceRewardClaimed
                .Select(x => DateTime.Parse(x.Date, null, System.Globalization.DateTimeStyles.RoundtripKind))
                .Max();
        }

        // DTO�� ��ȯ
        var dto = new AttendanceDTO
        {
            LastAttendanceDate = lastAttendanceDate,
            StreakCount = saveData.StreakCount,
            AttendanceRewardClaimed = saveData.AttendanceRewardClaimed != null
                ? saveData.AttendanceRewardClaimed
                    .Where(x => !string.IsNullOrEmpty(x.Date))
                    .ToDictionary(
                        x => DateTime.Parse(x.Date, null, System.Globalization.DateTimeStyles.RoundtripKind),
                        x => x.Claimed)
                : new Dictionary<DateTime, bool>()
        };

        var attendance = new Attendance();
        attendance.LoadFromSaveData(dto);
        return attendance;
    }

    public void Reset()
    {
        PlayerPrefs.DeleteKey(AttendanceKey);
        PlayerPrefs.Save();
    }

    // ���� ����� ������ ����
    [Serializable]
    private class AttendanceSaveData
    {
        public DateTime LastAttendanceDate;
        public int StreakCount;
        public List<DateRewardPair> AttendanceRewardClaimed;
    }

    [Serializable]
    public class DateRewardPair
    {
        public string Date; // ISO string
        public bool Claimed;
    }
}
