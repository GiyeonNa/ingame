using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UI_Achievement : MonoBehaviour
{
    [SerializeField]
    private List<UI_AchievementSlot> _achievementSlots = new List<UI_AchievementSlot>();

    private void Start()
    {
        // 초기 데이터 설정
        Refresh();

        AchievementManager.Instance.DataChange += Refresh;
    }


    private void Refresh()
    {
        List<AchievementDTO> achievements = AchievementManager.Instance.Achievements;

        for (int i = 0; i < _achievementSlots.Count; i++)
        {
            if (i < achievements.Count)
            {
                _achievementSlots[i].gameObject.SetActive(true);
                _achievementSlots[i].SetData(achievements[i]);
            }
            else
            {
                _achievementSlots[i].gameObject.SetActive(false);
            }
        }
    }
}
