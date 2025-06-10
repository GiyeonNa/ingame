using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    //TODO :: Change to SCV
    //[SerializeField]
    //private List<AchievementSO> _metaData = new List<AchievementSO>();

    private List<Achievement> _achievements = new List<Achievement>();
    public List<AchievementDTO> Achievements => _achievements.ConvertAll((x) => new AchievementDTO(x));

    public event Action DataChange;
    public event Action<AchievementDTO> AchievementClaimed;

    private AchievementRepository _achievementRepository;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        Init();
    }

    private void Init()
    {
        StartCoroutine(LoadAchievementsFromCSV());
        //_achievementRepository = new AchievementRepository();

        //List<AchievementSaveData> savedAchievements = _achievementRepository.Load();

        //if (savedAchievements == null || savedAchievements.Count == 0)
        //{
        //    foreach (var metaData in _metaData)
        //    {
        //        if (metaData == null)
        //        {
        //            Debug.LogError("AchievementSO is null in the metadata list.");
        //            continue;
        //        }

        //        Achievement duplicate = FindByID(metaData.Id);
        //        if(duplicate != null)
        //        {
        //            Debug.LogWarning($"Achievement with ID {metaData.Id} already exists. Skipping duplicate.");
        //            continue;
        //        }

        //        Achievement achievement = new Achievement(metaData);
        //        _achievements.Add(achievement);
        //    }
        //}
        //else
        //{
        //    foreach(var data in savedAchievements)
        //    {
        //        AchievementSO metaData = _metaData.Find(m => m.Id == data.Id);
        //        if (metaData == null)
        //        {
        //            Debug.LogWarning($"No metadata found for achievement ID {data.Id}. Skipping.");
        //            continue;
        //        }
        //        Achievement achievement = new Achievement(metaData, data);
        //        _achievements.Add(achievement);
        //    }
        //}
    }

    public void Increase(EAchievementCondition condition, int value)
    {
        foreach (var achievement in _achievements)
        {
            if (achievement.Condition == condition)
            {
                bool preValue = achievement.IsCanClaimReward();
                achievement.Increase(value);

                if (!preValue && achievement.IsCanClaimReward())
                {
                    Debug.Log($"Achievement '{achievement.Name}' is now eligible for reward.");
                    // Notify, redDot
                    AchievementClaimed?.Invoke(new AchievementDTO(achievement));
                }

            }
        }
        _achievementRepository.Save(Achievements);
        DataChange?.Invoke();
    }

    public void Increase(ECurrencyType type, int value)
    {
        foreach (var achievement in _achievements)
        {
            // 화폐 타입에 따라 업적 조건 분류
            if (type == ECurrencyType.Gold && achievement.Condition == EAchievementCondition.GoldCollect)
            {
                bool preValue = achievement.IsCanClaimReward();
                achievement.Increase(value);

                if (!preValue && achievement.IsCanClaimReward())
                {
                    Debug.Log($"Achievement '{achievement.Name}' is now eligible for reward.");
                    // Notify, redDot
                    AchievementClaimed?.Invoke(new AchievementDTO(achievement));
                }
            }
            else if (type == ECurrencyType.Diamond && achievement.Condition == EAchievementCondition.GoldCollect)
            {
                // 예시: 다이아몬드로 적립되는 업적이 있다면 여기에 추가
                // bool preValue = achievement.IsCanClaimReward();
                // achievement.Increase(value);
                // if (!preValue && achievement.IsCanClaimReward())
                // {
                //     Debug.Log($"Achievement '{achievement.Name}' is now eligible for reward.");
                //     AchievementClaimed?.Invoke(new AchievementDTO(achievement));
                // }
            }
            else if (type == ECurrencyType.Gem && achievement.Condition == EAchievementCondition.GoldCollect)
            {
                // 예시: 젬으로 적립되는 업적이 있다면 여기에 추가
                // bool preValue = achievement.IsCanClaimReward();
                // achievement.Increase(value);
                // if (!preValue && achievement.IsCanClaimReward())
                // {
                //     Debug.Log($"Achievement '{achievement.Name}' is now eligible for reward.");
                //     AchievementClaimed?.Invoke(new AchievementDTO(achievement));
                // }
            }
            // 필요시 다른 조건도 추가
        }

        _achievementRepository.Save(Achievements);
        DataChange?.Invoke();
    }



    public bool TryClaimReward(AchievementDTO achievementDTO)
    {
        var achive = _achievements.Find(a => a.Id == achievementDTO.Id);

        if (achive == null)
        {
            Debug.LogError($"Achievement with ID {achievementDTO.Id} not found.");
            return false;
        }

        if (achive.TryClaimReward())
        {
            CurrencyManager.Instance.Add(achive.RewardCurrencyType, achive.RewardValue);
            _achievementRepository.Save(Achievements);
            DataChange?.Invoke();
            return true;
        }
        return false;
    }


    private Achievement FindByID(string id)
    {
        return _achievements.Find(a => a.Id == id);

    }

    public void ResetAchievements()
    {
        _achievementRepository.Clear();
        _achievements.Clear();
        Init(); // 메타데이터로 다시 초기화
        DataChange?.Invoke();
    }

    [Serializable]
    public class AchievementCSVData
    {
        public string Id;
        public string Name;
        public string Description;
        public EAchievementCondition Condition;
        public int GoalValue;
        public ECurrencyType RewardCurrencyType;
        public int RewardValue;
    }

    [SerializeField] private string _csvAddress = "AchievementCSV"; // Addressable 주소

    private IEnumerator LoadAchievementsFromCSV()
    {
        _achievementRepository = new AchievementRepository();
        List<AchievementSaveData> savedAchievements = _achievementRepository.Load();

        var handle = Addressables.LoadAssetAsync<TextAsset>(_csvAddress);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var csvText = handle.Result.text;
            var achievementList = ParseCSV(csvText);
            _achievements.Clear();

            foreach (var data in achievementList)
            {
                var saveData = savedAchievements?.Find(s => s.Id == data.Id);
                Achievement achievement;
                if (saveData != null)
                    achievement = new Achievement(data, saveData.Value);
                else
                    achievement = new Achievement(data);

                _achievements.Add(achievement);
            }
            DataChange?.Invoke();
        }
        else
        {
            Debug.LogError("CSV 파일 로드 실패");
        }
    }


    private List<AchievementCSVData> ParseCSV(string csvText)
    {
        var lines = csvText.Split('\n').Skip(1); // 첫 줄은 헤더
        var list = new List<AchievementCSVData>();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var tokens = line.Split(',');
            var data = new AchievementCSVData
            {
                Id = tokens[0],
                Name = tokens[1],
                Description = tokens[2],
                Condition = Enum.Parse<EAchievementCondition>(tokens[3]),
                GoalValue = int.Parse(tokens[4]),
                RewardCurrencyType = Enum.Parse<ECurrencyType>(tokens[5]),
                RewardValue = int.Parse(tokens[6])
            };
            list.Add(data);
        }
        return list;
    }
}
