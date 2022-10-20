using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class Player : MonoBehaviour
{
	[Header("Ticket Machine")]
	public GameObject ticketMachine;
	public float scaleXTarget = 1.1f;
	public float scaleYTarget = 1.1f;
	public float scaleZTarget = 1.1f;
	public float normalScaleDuration = 0.25f;
	public float hotScaleDuration = 0.1f;

	[Space]
	public Color normalColor;
	public Color hotColor;

	private float scaleDuration = 0.1f;
	private bool canScaleChange = true;
	private MeshRenderer machineRenderer;

	[Header("Effects")]
	public ParticleSystem moneyEffect;
	public ParticleSystem ticketEffect;
	public GameObject moneyEffectSpawnPos;

	[Header("Buttons")]
	public List<ScaleButton> scaleButtons;

	[Header("Stats")]
    public float money = 0;
    public int staminaLevel = 0;
	public int incomeLevel = 0;
	public int slotCount = 1;
	public float staminaRefullSpeed = 1;

	[HideInInspector] public bool canTouch = true;

    private float currentMaxStamina = 0;
	private float currentStamina = 0;
    private float currentMoneyIncrease = 1;

    [Header("Level Increase Counts")]
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
	public float flowSpeed = 5;

    [Header("UI")]
    public TextMeshProUGUI moneyText;
	public TextMeshProUGUI staminaMoneyText;
	public TextMeshProUGUI incomeMoneyText;
	public TextMeshProUGUI slotMoneyText;
	public TextMeshProUGUI staminaLevelText;
	public TextMeshProUGUI incomeaLevelText;
	public TextMeshProUGUI slotCountText;
	public Slider staminaSlider;

	public static Player player { get; private set; }

    private void Awake()
    {
        if(player == null) player = this;

		machineRenderer = ticketMachine.GetComponent<MeshRenderer>();
    }

    private void Start()
    {
		staminaLevel = 0;
		incomeLevel = 0;
		slotCount = 1;

		Load();

		CheckLevels();
        CheckSlots();

		currentStamina = 0;
	}

    private void Update()
    {
        if (Input.touchCount != 0 && canTouch)
        {
			if (currentStamina < currentMaxStamina)
			{
				currentStamina += staminaRefullSpeed * Time.deltaTime;
			}
			else if(currentStamina >= currentMaxStamina)
			{
				canTouch = false;
			}

			for (int i = 0; i < ticketPaths.Count; i++)
            {
                ticketPaths[i].flowSpeed = flowSpeed;
            }

			CloseButtons();
        }
        else if (Input.touchCount == 0)
        {
			if (currentStamina < currentMaxStamina * 0.6f)
			{
				canTouch = true;
			}

			if (currentStamina > 0)
			{
				currentStamina -= staminaRefullSpeed * (currentMaxStamina / 100) * Time.deltaTime;
			}

			for (int i = 0; i < ticketPaths.Count; i++)
			{
				ticketPaths[i].flowSpeed = 0;
			}

			OpenButtons();
		}
		else
		{
			if (currentStamina > 0)
			{
				currentStamina -= staminaRefullSpeed * (currentMaxStamina / 100) * Time.deltaTime;
			}

			for (int i = 0; i < ticketPaths.Count; i++)
			{
				ticketPaths[i].flowSpeed = 0;
			}
		}

        moneyText.text = money.ToString();
		staminaSlider.value = currentStamina / currentMaxStamina;
		machineRenderer.material.color = Color.Lerp(normalColor, hotColor, staminaSlider.value);
		scaleDuration = Mathf.Lerp(normalScaleDuration, hotScaleDuration, staminaSlider.value);
	}

    public void OnTicketInMachine(Vector3 TicketEffectPos)
    {
        money += currentMoneyIncrease;

		Instantiate(moneyEffect, moneyEffectSpawnPos.transform.position, Quaternion.identity);
		Instantiate(ticketEffect, TicketEffectPos, Quaternion.identity);

		if (canScaleChange)
		{
			ticketMachine.transform.DOScaleX(scaleXTarget, scaleDuration).SetLoops(2, LoopType.Yoyo);
			ticketMachine.transform.DOScaleY(scaleYTarget, scaleDuration).SetLoops(2, LoopType.Yoyo);
			ticketMachine.transform.DOScaleZ(scaleZTarget, scaleDuration).SetLoops(2, LoopType.Yoyo);

			StartCoroutine(ScaleBlock());
		}
	}

	private IEnumerator ScaleBlock()
	{
		canScaleChange = false;

		yield return new WaitForSeconds(scaleDuration + 0.05f);

		canScaleChange = true;
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

	private void OpenButtons()
	{
		for (int i = 0; i < scaleButtons.Count; i++)
		{
			scaleButtons[i].isOpen = true;
		}
	}

	private void CloseButtons()
	{
		for (int i = 0; i < scaleButtons.Count; i++)
		{
			scaleButtons[i].isOpen = false;
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
