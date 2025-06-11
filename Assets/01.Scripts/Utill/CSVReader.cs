using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static AchievementManager;

public class CSVReader : MonoBehaviour
{
    public static IEnumerator ParseFromAddressable<T>(string address, Action<List<T>> onComplete)
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>(address);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var csvFile = handle.Result;
            var list = Parse<T>(csvFile);
            onComplete?.Invoke(list);
        }
        else
        {
            Debug.LogError($"CSV 파일({address}) 로드 실패");
            onComplete?.Invoke(null);
        }
    }

    public static List<AttendanceRewardInfo> ParseAttendanceCSV(string csvText)
    {
        var list = new List<AttendanceRewardInfo>();
        var lines = csvText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < lines.Length; i++) // 0번은 헤더
        {
            var cols = lines[i].Split(',');
            if (cols.Length < 3) continue;
            if (int.TryParse(cols[0], out int day) && int.TryParse(cols[2], out int value))
            {
                if (Enum.TryParse(cols[1], out ECurrencyType type))
                {
                    list.Add(new AttendanceRewardInfo
                    {
                        Day = day,
                        RewardCurrencyType = type,
                        RewardValue = value
                    });
                }
            }
        }
        return list;
    }

    public static List<T> Parse<T>(TextAsset csvFile)
    {
        var list = new List<T>();
        if (csvFile == null || string.IsNullOrWhiteSpace(csvFile.text))
            return list;

        var lines = csvFile.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2)
            return list;

        // 헤더 파싱
        var header = lines[0].Split(',');
        var type = typeof(T);
        var members = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Cast<MemberInfo>()
            .Concat(type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            .ToArray();

        // 컬럼명-멤버 매핑
        var columnMap = new Dictionary<int, MemberInfo>();
        for (int i = 0; i < header.Length; i++)
        {
            var col = header[i].Trim();
            foreach (var member in members)
            {
                var attr = member.GetCustomAttribute<CsvColumnAttribute>();
                if (attr != null && attr.ColumnName == col)
                {
                    columnMap[i] = member;
                    break;
                }
            }
        }

        // 데이터 파싱
        for (int i = 1; i < lines.Length; i++)
        {
            var tokens = lines[i].Split(',');
            if (tokens.Length == 0 || string.IsNullOrWhiteSpace(tokens[0]))
                continue;

            var obj = Activator.CreateInstance<T>();
            foreach (var kvp in columnMap)
            {
                int idx = kvp.Key;
                var member = kvp.Value;
                if (idx >= tokens.Length)
                    continue;

                string value = tokens[idx].Trim();
                if (string.IsNullOrEmpty(value))
                    continue;

                Type memberType = member is FieldInfo fi ? fi.FieldType : ((PropertyInfo)member).PropertyType;
                object converted = ConvertValue(value, memberType);

                if (member is FieldInfo field)
                    field.SetValue(obj, converted);
                else if (member is PropertyInfo prop && prop.CanWrite)
                    prop.SetValue(obj, converted);
            }
            list.Add(obj);
        }
        return list;
    }

    private static object ConvertValue(string value, Type type)
    {
        if (type.IsEnum)
            return Enum.Parse(type, value);
        if (type == typeof(int))
            return int.Parse(value);
        if (type == typeof(float))
            return float.Parse(value);
        if (type == typeof(double))
            return double.Parse(value);
        if (type == typeof(bool))
            return bool.Parse(value);
        return value;
    }
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class CsvColumnAttribute : Attribute
{
    public string ColumnName { get; }

    public CsvColumnAttribute(string columnName)
    {
        ColumnName = columnName;
    }
}

[Serializable]
public class AchievementCSVData
{
    [CsvColumn("Id")]
    public string Id;

    [CsvColumn("Name")]
    public string Name;

    [CsvColumn("Description")]
    public string Description;

    [CsvColumn("Condition")]
    public EAchievementCondition Condition;

    [CsvColumn("GoalValue")]
    public int GoalValue;

    [CsvColumn("RewardCurrencyType")]
    public ECurrencyType RewardCurrencyType;

    [CsvColumn("RewardValue")]
    public int RewardValue;
}

[Serializable]
public class ItemCSVData
{
    [CsvColumn("Id")]
    public string Id;

    [CsvColumn("Name")]
    public string Name;

    [CsvColumn("Type")]
    public string Type;

    [CsvColumn("Value")]
    public int Value;
}

[Serializable]
public class AttendanceCSVData
{
    [CsvColumn("Day")]
    public int Day;

    [CsvColumn("RewardCurrencyType")]
    public ECurrencyType RewardCurrencyType;

    [CsvColumn("RewardValue")]
    public int RewardValue;
}

