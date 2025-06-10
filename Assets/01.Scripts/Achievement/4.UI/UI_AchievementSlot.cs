using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SocialPlatforms.Impl;

public class UI_AchievementSlot : MonoBehaviour
{
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI DescriptionText;
    public TextMeshProUGUI RewardText;
    public Button ClaimButton;
    public Image IconImage;
    public Slider ProgressSlider;
    public TextMeshProUGUI ProgressText;

    private AchievementDTO _achievementDTO;

    public void SetData(AchievementDTO achievementDTO)
    {
        _achievementDTO = achievementDTO;

        TitleText.text = achievementDTO.Name;
        DescriptionText.text = achievementDTO.Description;
        RewardText.text = $"{achievementDTO.RewardValue} {achievementDTO.RewardCurrencyType}";
        ProgressSlider.maxValue = achievementDTO.GoalValue;
        ProgressSlider.value = achievementDTO.CurrentValue;
        ProgressText.text = $"{achievementDTO.CurrentValue} / {achievementDTO.GoalValue}";
        IconImage.sprite = null; // 아이콘은 별도로 설정하지 않음

        UpdateState(achievementDTO);

        ClaimButton.onClick.RemoveAllListeners();
        ClaimButton.onClick.AddListener(OnClaimButtonClicked); 
    }

    private void UpdateState(AchievementDTO achievementDTO)
    {
        var btnText = ClaimButton.GetComponentInChildren<TextMeshProUGUI>();

        if (achievementDTO.IsRewarded)
        {
            ClaimButton.interactable = false;
            if (btnText != null)
                btnText.text = "보상완료";
        }
        else if (achievementDTO.IsCompleted)
        {
            ClaimButton.interactable = true;
            if (btnText != null)
                btnText.text = "보상 받기";
        }
        else
        {
            ClaimButton.interactable = false;
            if (btnText != null)
                btnText.text = "미달성";
        }

        ProgressText.text = $"{achievementDTO.CurrentValue} / {achievementDTO.GoalValue}";
    }


    private void OnClaimButtonClicked()
    {
        if (_achievementDTO == null)
            return;

        if(AchievementManager.Instance.TryClaimReward(_achievementDTO))
        {
            ClaimButton.interactable = false;
        }
    }
}
