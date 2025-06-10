using UnityEngine;
using TMPro;
using System.Collections;

public class UI_Noty : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI descriptionText;
    [SerializeField]
    private GameObject Bg;

    private Coroutine _hideCoroutine;

    private void Start()
    {
        if (descriptionText == null)
        {
            Debug.LogError("Description TextMeshProUGUI is not assigned in the inspector.");
        }
        if (Bg != null)
            Bg.SetActive(false); // �ʱ⿡�� ��Ȱ��ȭ

        AchievementManager.Instance.AchievementClaimed += OnAchievementClaimed;
    }

    private void OnAchievementClaimed(AchievementDTO achievement)
    {
        Show($"���� �޼�: {achievement.Name}");
    }

    public void Show(string description)
    {
        if (descriptionText == null)
        {
            Debug.LogError("Description TextMeshProUGUI is not assigned in the inspector.");
            return;
        }
        descriptionText.text = description;
        if (Bg != null)
            Bg.SetActive(true);

        if (_hideCoroutine != null)
            StopCoroutine(_hideCoroutine);

        _hideCoroutine = StartCoroutine(HideAfterDelay(2f));
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (Bg != null)
            Bg.SetActive(false);
        _hideCoroutine = null;
    }
}
