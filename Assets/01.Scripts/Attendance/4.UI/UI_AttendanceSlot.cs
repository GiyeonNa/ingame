using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_AttendanceSlot : MonoBehaviour
{
    [Header("UI 참조")]
    public TextMeshProUGUI dayText;
    public Image rewardIcon;
    public TextMeshProUGUI rewardValueText;
    public GameObject claimedMark;
    public Button claimButton;
    public Image claimableImg;

    private int _day;



    public void SetData(AttendanceRewardInfo rewardInfo, bool isClaimed = false, bool isClaimable = false)
    {
        _day = rewardInfo.Day;
        if (dayText != null)
            dayText.text = $"{rewardInfo.Day}일";

        if (rewardValueText != null)
            rewardValueText.text = rewardInfo.RewardValue.ToString();

        // rewardIcon은 CSV 기반에서는 Sprite 정보가 없으므로, 필요시 별도 매핑 필요
        // rewardIcon.sprite = ...;

        if (claimedMark != null)
            claimedMark.SetActive(isClaimed);

        if (claimButton != null)
            claimButton.interactable = isClaimable;

        if (claimButton != null)
        {
            claimButton.interactable = !isClaimed && isClaimable;
            claimButton.onClick.RemoveAllListeners();
            if (!isClaimed && isClaimable)
            {
                claimButton.onClick.AddListener(() =>
                {
                    AttendanceManager.Instance.ClaimReward(rewardInfo.Day);
                    // 필요시 슬롯 갱신 또는 전체 UI 갱신 호출
                });
            }
        }

        if (claimableImg != null)
            claimableImg.gameObject.SetActive(isClaimable);
    }
}
