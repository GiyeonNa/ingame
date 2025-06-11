using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_AttendanceSlot : MonoBehaviour
{
    [Header("UI ����")]
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
            dayText.text = $"{rewardInfo.Day}��";

        if (rewardValueText != null)
            rewardValueText.text = rewardInfo.RewardValue.ToString();

        // rewardIcon�� CSV ��ݿ����� Sprite ������ �����Ƿ�, �ʿ�� ���� ���� �ʿ�
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
                    // �ʿ�� ���� ���� �Ǵ� ��ü UI ���� ȣ��
                });
            }
        }

        if (claimableImg != null)
            claimableImg.gameObject.SetActive(isClaimable);
    }
}
