using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    public float money = 0;
    public int staminaLevel = 0;
	public int incomeLevel = 0;
	public int slotCount = 1;

    private float currentMaxStamina = 0;
	private float currentStamina = 0;
    private float currentMoneyIncrease = 1;

    [Header("Increase Counts")]
    public float staminaIncreaseByLevel = 50;
	public float incomeIncreaseByLevel = 0.5f;

	[Header("Level Money Counts")]
	public float staminaMoneyByLevel = 50;
	public float incomeMoneyByLevel = 50;
	public float slotMoneyByLevel = 50;

	private float currentStaminaLevelMoney = 50;
	private float currentIncomeLevelMoney = 50;
	private float currentSlotMoney = 50;

	[Header("Ticket Paths")]
    public List<TicketFlow> ticketPaths;
    public float normalFlowSpeed = 2;
	public float fastFlowSpeed = 5;

    [Header("UI")]
    public TextMeshProUGUI moneyText;
	public TextMeshProUGUI staminaMoneyText;
	public TextMeshProUGUI incomeMoneyText;
	public TextMeshProUGUI slotMoneyText;
	public TextMeshProUGUI staminaLevelText;
	public TextMeshProUGUI incomeaLevelText;
	public TextMeshProUGUI slotCountText;

	public static Player player { get; private set; }

    private void Awake()
    {
        if(player == null) player = this;
    }

    private void Start()
    {
		staminaLevel = 0;
		incomeLevel = 0;
		slotCount = 1;

		Load();

		CheckLevels();
        CheckSlots();
	}

    private void Update()
    {
        if (Input.touchCount != 0)
        {
            for (int i = 0; i < ticketPaths.Count; i++)
            {
                ticketPaths[i].flowSpeed = fastFlowSpeed;
            }
        }
        else
        {
			for (int i = 0; i < ticketPaths.Count; i++)
			{
				ticketPaths[i].flowSpeed = normalFlowSpeed;
			}
		}

        moneyText.text = money.ToString();
	}

    public void OnTicketInMachine()
    {
        money += currentMoneyIncrease;
    }

	private void CheckLevels()
    {
		currentMaxStamina = staminaLevel * staminaIncreaseByLevel + staminaIncreaseByLevel;
        currentMoneyIncrease = incomeLevel * incomeIncreaseByLevel + incomeIncreaseByLevel;

        currentStaminaLevelMoney = (staminaLevel + 1) * staminaMoneyByLevel;
		currentIncomeLevelMoney = (incomeLevel + 1) * incomeMoneyByLevel;

		staminaLevelText.text = (staminaLevel + 1).ToString();
		incomeaLevelText.text = (incomeLevel + 1).ToString();
		slotCountText.text = (slotCount + 1).ToString();

		staminaMoneyText.text = (currentStaminaLevelMoney).ToString();
		incomeMoneyText.text = (currentIncomeLevelMoney).ToString();
		slotMoneyText.text = (currentSlotMoney).ToString();
	}

    private void CheckSlots()
    {
		for (int i = 0; i < ticketPaths.Count; i++)
		{
			ticketPaths[i].isFlowing = false;
			ticketPaths[i].gameObject.SetActive(false);
		}

		for (int i = 0; i < slotCount; i++)
		{
			ticketPaths[i].isFlowing = true;
			ticketPaths[i].gameObject.SetActive(true);
		}

		slotCountText.text = (slotCount + 1).ToString();
		slotMoneyText.text = (currentSlotMoney).ToString();
	}

    public void BuyStaminaUpgrade()
    {
        if (money >= currentStaminaLevelMoney)
        {
            staminaLevel++;
            money -= currentStaminaLevelMoney;
            CheckLevels();
        }
    }

	public void BuyIncomeUpgrade()
	{
		if (money >= currentIncomeLevelMoney)
		{
			incomeLevel++;
			money -= currentIncomeLevelMoney;
			CheckLevels();
		}
	}

	public void BuySlotUpgrade()
	{
		if (money >= currentSlotMoney)
		{
			slotCount++;
			money -= currentSlotMoney;
			CheckSlots();
		}
	}

	public void Save()
	{
		PlayerPrefs.SetFloat("Money", money);
		PlayerPrefs.SetInt("StaminaLevel", staminaLevel);
		PlayerPrefs.SetInt("IncomeLevel", incomeLevel);
		PlayerPrefs.SetInt("SlotCount", slotCount);
	}

	public void Load()
	{
		if (PlayerPrefs.HasKey("Money")) money = PlayerPrefs.GetFloat("Money");
		if (PlayerPrefs.HasKey("StaminaLevel")) staminaLevel = PlayerPrefs.GetInt("StaminaLevel");
		if (PlayerPrefs.HasKey("IncomeLevel")) incomeLevel = PlayerPrefs.GetInt("IncomeLevel");
		if (PlayerPrefs.HasKey("SlotCount")) slotCount = PlayerPrefs.GetInt("SlotCount");
	}

	[ContextMenu("Reset Save")]
	public void ResetSave()
	{
		PlayerPrefs.DeleteAll();
	}

	private void OnApplicationQuit()
	{
		Save();
	}
}
