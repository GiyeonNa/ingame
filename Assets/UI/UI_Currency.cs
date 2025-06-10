using UnityEngine;
using TMPro;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;

public class UI_Currency : MonoBehaviour
{
    public TextMeshProUGUI GoldCountText;
    public TextMeshProUGUI DiamondCountText;
    public TextMeshProUGUI HealthCountText;
    public TextMeshProUGUI GemCountText;


    private void Start()
    {
        if (CurrencyManager.Instance == null)
        {
            Debug.LogError("CurrencyManager instance is not found. Please ensure it is initialized before UI_Currency.");
            return;
        }
        Refresh();
        CurrencyManager.Instance.OnCurrencyChanged += Refresh;
    }

    private void Refresh()
    {
        var gold = CurrencyManager.Instance.Get(ECurrencyType.Gold);
        var diamond = CurrencyManager.Instance.Get(ECurrencyType.Diamond);
        var gem = CurrencyManager.Instance.Get(ECurrencyType.Gem);

        GoldCountText.text = $"Gold : {gold.Value}";
        DiamondCountText.text = $"Diamond : {diamond.Value}";
        GemCountText.text = $"Gem : {gem.Value}";

        HealthCountText.color = gold.HaveEnough(300) ? Color.green : Color.red;
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            // For testing purposes, add some gold and diamond
            CurrencyManager.Instance.Add(ECurrencyType.Gold, 200);
            CurrencyManager.Instance.Add(ECurrencyType.Diamond, 5);
            CurrencyManager.Instance.Add(ECurrencyType.Gem, 3);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            BuyHealth();
        }
    }

    public void BuyHealth()
    {
        if(CurrencyManager.Instance.TryBuy(ECurrencyType.Gold,300))
        {
            var player = GameObject.FindFirstObjectByType<PlayerCharacterController>();
            Health playerHealth = player.GetComponent<Health>();
            playerHealth.Heal(50);
        }
        else
        {
            Debug.Log("Not enough gold to buy health.");
        }


    }
}
