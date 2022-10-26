using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;
using System;
using MoreMountains.NiceVibrations;
using System.Web.Razor.Generator;

public class Player : MonoBehaviour
{
	[Serializable]
	public struct TicketMachine
	{
		public GameObject ticketMachine;
		public List<GameObject> pathMouths;
		public GameObject staminaBar;
		public GameObject moneyEffectSpawnPos;
		public int machineLevel;
		public float moneyIncrease;
		public List<TicketFlow> ticketPaths;
	}

	public CinemachineTargetGroup targetGroup;
	public CinemachineVirtualCamera inGameCamCont;
	public CinemachineVirtualCamera startCamCont;
	public bool isVibrate = true;

	private bool isStarted = false;

	[Header("Ticket Machine")]
	public List<TicketMachine> ticketMachines;
	public float scaleXTarget = 1.1f;
	public float scaleYTarget = 1.1f;
	public float scaleZTarget = 1.1f;
	public float normalScaleDuration = 0.25f;
	public float hotScaleDuration = 0.1f;

	[Space]
	public Color normalColor;
	public Color hotColor;
	public float colorChangeSpeed = 5;

	private float scaleDuration = 0.1f;
	private bool canScaleChange = true;
	private TicketMachine currentMachine;
	private TicketMachine lastMachine;
	private MeshRenderer machineRenderer;
	private GameObject currentstaminaBar;

	[Header("Effects")]
	public ParticleSystem moneyEffect;
	public TextMeshPro moneyTextEffect;
	public ParticleSystem ticketEffect;
	public ParticleSystem smokeEffect;
	public ParticleSystem fireEffect;

	[Header("Buttons")]
	public List<ScaleButton> scaleButtons;

	[Header("Stats")]
    public float money = 0;
    public int staminaLevel = 0;
	public int incomeLevel = 0;
	public int machineLevel = 0;
	public float staminaRefullSpeed = 1;

	[HideInInspector] public bool canTouch = true;
	private bool isPlay = false;

    private float currentMaxStamina = 0;
	private float currentStamina = 0;
    private float currentMoneyIncrease = 1;
	private int currentMachineLevel = 0;

    [Header("Level Increase Counts")]
    public float staminaIncreaseByLevel = 50;
	public float incomeIncreaseByLevel = 0.5f;

	[Header("Level Money Counts")]
	public float staminaMoneyByLevel = 50;
	public float incomeMoneyByLevel = 50;
	public float machineMoneyByLevel = 50;

	private float currentStaminaLevelMoney = 50;
	private float currentIncomeLevelMoney = 50;
	private float currentMachineMoney = 50;

	[Header("Ticket Paths")]
	public float fastFlowSpeed = 5;
	public float slowFlowSpeed = 2;

	[Header("UI")]
    public TextMeshProUGUI moneyText;
	public TextMeshProUGUI staminaMoneyText;
	public TextMeshProUGUI incomeMoneyText;
	public TextMeshProUGUI machineMoneyText;
	public TextMeshProUGUI staminaLevelText;
	public TextMeshProUGUI incomeaLevelText;
	public TextMeshProUGUI machineLevelText;
	public Slider staminaSlider;
	public Button staminaButton;
	public Button machineButton;
	public Button incomeButton;
	public Color notInteractableColor;
	public Color interactableColor;
	public Image staminaImage;
	public Image machineImage;
	public Image incomeImage;
	public GameObject newMachinePanel;
	public float newMachinePanelDuration;
	public Ease newMachinePanelEase;
	public GameObject holdImage;

	private float TextedMoney(float money)
	{
		return (float)Math.Round((decimal)money, 2);
	}

	public void addMoney(int moneyToAdd)
	{
        if (money >= 0)
        {
            money += moneyToAdd;
            Vibrate();
            CheckLevels();
        }
        Save();
	}

	public static Player player { get; private set; }

	private bool IsMachineMaxLevel()
	{
		return ticketMachines[ticketMachines.Count - 1].machineLevel + ticketMachines[ticketMachines.Count - 1].ticketPaths.Count - 1 <= machineLevel;
	}

    private void Awake()
    {
        if(player == null) player = this;
		newMachinePanel.SetActive(false);
		holdImage.SetActive(true);
		isStarted = false;
		inGameCamCont.gameObject.SetActive(false);
		startCamCont.gameObject.SetActive(true);
	}

    private void Start()
    {
		staminaLevel = 0;
		incomeLevel = 0;
		machineLevel = 0;

		Load();

		for (int i = 0; i < ticketMachines.Count; i++)
		{
			if (machineLevel >= ticketMachines[i].machineLevel)
			{
				currentMachine = ticketMachines[i];
				lastMachine = currentMachine;
			}
			else break;
		}

		CheckLevels();
		CheckMachine();

		currentStamina = 0;
	}

    private void Update()
    {
        if (Input.touchCount != 0 && canTouch && isPlay)
        {
			if (currentStamina < currentMaxStamina)
			{
				currentStamina += (staminaRefullSpeed / (currentMaxStamina / 100)) * Time.deltaTime;
			}
			else if(currentStamina >= currentMaxStamina)
			{
				canTouch = false;
				isPlay = false;
				Save();
				Vibrate();
			}

			for (int i = 0; i < currentMachine.ticketPaths.Count; i++)
            {
				currentMachine.ticketPaths[i].flowSpeed = fastFlowSpeed;
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
			
			if (isStarted)
			{
				for (int i = 0; i < currentMachine.ticketPaths.Count; i++)
				{
					currentMachine.ticketPaths[i].flowSpeed = slowFlowSpeed;
				}
			}
			else
			{
				for (int i = 0; i < currentMachine.ticketPaths.Count; i++)
				{
					currentMachine.ticketPaths[i].flowSpeed = 0;
				}
			}
			
			OpenButtons();
			isPlay = false;
		}
		else
		{
			if (currentStamina > 0)
			{
				currentStamina -= staminaRefullSpeed * (currentMaxStamina / 100) * Time.deltaTime;
			}

			if (isStarted)
			{
				for (int i = 0; i < currentMachine.ticketPaths.Count; i++)
				{
					currentMachine.ticketPaths[i].flowSpeed = slowFlowSpeed;
				}
			}
			else
			{
				for (int i = 0; i < currentMachine.ticketPaths.Count; i++)
				{
					currentMachine.ticketPaths[i].flowSpeed = 0;
				}
			}
		}

		if(!canTouch) OpenButtons();

		MachineEffects();

		moneyText.text = TextedMoney(money).ToString();
		staminaSlider.value = currentStamina / currentMaxStamina;
		machineRenderer.material.color = Color.Lerp(machineRenderer.material.color, Color.Lerp(normalColor, hotColor, staminaSlider.value), colorChangeSpeed * Time.deltaTime);
		scaleDuration = Mathf.Lerp(normalScaleDuration, hotScaleDuration, staminaSlider.value);
		currentMachine.staminaBar.GetComponent<MeshRenderer>().materials[1].SetFloat("_ProgressBorder", staminaSlider.value * 0.525f);
		targetGroup.m_Targets[0].weight = 1 - staminaSlider.value;
		targetGroup.m_Targets[1].weight = staminaSlider.value;

		UIButtons();
	}

	private void UIButtons()
	{
		if (money >= currentStaminaLevelMoney)
		{
			staminaButton.interactable = true;
			staminaImage.color = interactableColor;
		}
		else
		{
			staminaButton.interactable = false;
			staminaImage.color = notInteractableColor;
		}

		if (money >= currentIncomeLevelMoney)
		{
			incomeButton.interactable = true;
			incomeImage.color = interactableColor;
		}
		else
		{
			incomeButton.interactable = false;
			incomeImage.color = notInteractableColor;
		}

		if (money >= currentMachineMoney && !IsMachineMaxLevel())
		{
			machineButton.interactable = true;
			machineImage.color = interactableColor;
		}
		else
		{
			machineButton.interactable = false;
			machineImage.color = notInteractableColor;
		}
	}

	public void NewMachinePanelClose()
	{
		StartCoroutine(NewMachinePanelCloseNum());
	}

	private IEnumerator NewMachinePanelCloseNum()
	{
		newMachinePanel.transform.DOScale(0.001f, newMachinePanelDuration).SetEase(newMachinePanelEase);
		yield return new WaitForSeconds(newMachinePanelDuration);
		newMachinePanel.SetActive(false);
	}

	public void PlayGame()
	{
		if (!isStarted)
		{
			isStarted = true;
			inGameCamCont.gameObject.SetActive(true);
			startCamCont.gameObject.SetActive(false);
		}
		holdImage.SetActive(false);
		isPlay = true;
		Vibrate();
	}

	private void MachineEffects()
	{ 
		if (staminaSlider.value > 0.55f && smokeEffect.isStopped)
		{
			smokeEffect.Play();
		}
		else if (staminaSlider.value <= 0.55f && !smokeEffect.isStopped)
		{
			smokeEffect.Stop();
		}

		if (staminaSlider.value > 0.8f && fireEffect.isStopped)
		{
			fireEffect.Play();
		}
		else if (staminaSlider.value <= 0.8f && !fireEffect.isStopped)
		{
			fireEffect.Stop();
		}
	}

    public void OnTicketInMachine(Vector3 TicketEffectPos, Vector3 moneyTextEffectPos)
    {
		float moneyAmount = 0;

		if (isPlay) moneyAmount = currentMachine.moneyIncrease * currentMoneyIncrease;
		else moneyAmount = (currentMachine.moneyIncrease * currentMoneyIncrease) * 0.6f;

		money += moneyAmount;

		Instantiate(moneyEffect, currentMachine.moneyEffectSpawnPos.transform.position, currentMachine.moneyEffectSpawnPos.transform.rotation);
		Instantiate(moneyTextEffect, moneyTextEffectPos, Quaternion.Euler(0, -90, 0)).text = "$" + TextedMoney(moneyAmount).ToString();
		Instantiate(ticketEffect, TicketEffectPos, Quaternion.identity);

		if (canScaleChange)
		{
			currentMachine.ticketMachine.transform.DOScaleX(scaleXTarget, scaleDuration).SetLoops(2, LoopType.Yoyo);
			currentMachine.ticketMachine.transform.DOScaleY(scaleYTarget, scaleDuration).SetLoops(2, LoopType.Yoyo);
			currentMachine.ticketMachine.transform.DOScaleZ(scaleZTarget, scaleDuration).SetLoops(2, LoopType.Yoyo);

			StartCoroutine(ScaleBlock());
		}
	}

	private void CheckMachine()
	{
		for (int i = 0; i < ticketMachines.Count; i++)
		{
			if (machineLevel >= ticketMachines[i].machineLevel)
			{
				currentMachine = ticketMachines[i];
			}
			else break;
		}

		if (lastMachine.ticketMachine != currentMachine.ticketMachine)
		{
			Vector3 targetScale = newMachinePanel.transform.localScale;
			newMachinePanel.transform.localScale = Vector3.one * 0.001f;
			newMachinePanel.SetActive(true);
			newMachinePanel.transform.DOScale(1, newMachinePanelDuration).SetEase(newMachinePanelEase);
			lastMachine = currentMachine;
			Vibrate();
		}

		for (int i = 0; i < ticketMachines.Count; i++)
		{
			for (int k = 0; k < ticketMachines.Count; k++)
			{
				ticketMachines[i].ticketMachine.SetActive(false);

				for (int j = 0; j < ticketMachines[k].ticketPaths.Count; j++)
				{
					ticketMachines[k].ticketPaths[j].isFlowing = false;
					ticketMachines[k].ticketPaths[j].gameObject.SetActive(false);
				}

				for (int j = 0; j < ticketMachines[k].pathMouths.Count; j++)
				{
					ticketMachines[k].pathMouths[j].SetActive(false);
				}
			}
		}

		currentMachine.ticketMachine.SetActive(true);
		CheckPaths();

		machineRenderer = currentMachine.ticketMachine.GetComponent<MeshRenderer>();

		machineLevelText.text = (machineLevel + 1).ToString();

		currentMachineMoney = 0;

		for (int i = 0; i < machineLevel + 1; i++)
		{
			currentMachineMoney += (machineLevel + 1) * machineMoneyByLevel;
		}

		if (IsMachineMaxLevel())
		{
			machineButton.interactable = false;
			machineMoneyText.text = "Max";
		}
		else
		{
			machineButton.interactable = true;
			machineMoneyText.text = (currentMachineMoney).ToString();
		}

		CheckLevels();
	}

	private void CheckPaths()
	{
		for (int i = 0; i < currentMachine.ticketPaths.Count; i++)
		{
			currentMachine.ticketPaths[i].isFlowing = false;
			currentMachine.ticketPaths[i].gameObject.SetActive(false);

			for (int j = 0; j < currentMachine.ticketPaths[i].spawnedTickets.Count; j++)
			{
				currentMachine.ticketPaths[i].spawnedTickets[j].SetActive(false);
			}
		}

		for (int i = 0; i < currentMachine.pathMouths.Count; i++)
		{
			currentMachine.pathMouths[i].SetActive(false);
		}

		for (int i = 0; i < machineLevel - currentMachine.machineLevel + 1; i++)
		{
			currentMachine.ticketPaths[i].isFlowing = true;
			currentMachine.ticketPaths[i].gameObject.SetActive(true);

			for (int j = 0; j < currentMachine.ticketPaths[i].spawnedTickets.Count; j++)
			{
				currentMachine.ticketPaths[i].spawnedTickets[j].SetActive(true);
			}
		}

		for (int i = 0; i < machineLevel - currentMachine.machineLevel + 1; i++)
		{
			currentMachine.pathMouths[i].SetActive(true);
		}
	}

	private IEnumerator ScaleBlock()
	{
		canScaleChange = false;

		yield return new WaitForSeconds(scaleDuration * 2 + 0.05f);

		canScaleChange = true;
	}

	private void CheckLevels()
    {
		currentMaxStamina = staminaLevel * staminaIncreaseByLevel + staminaIncreaseByLevel * 10;
        currentMoneyIncrease = incomeLevel * incomeIncreaseByLevel + incomeIncreaseByLevel;

		currentStaminaLevelMoney = 0;
		currentIncomeLevelMoney = 0;

		for (int i = 0; i < staminaLevel + 1; i++)
		{
			currentStaminaLevelMoney += (staminaLevel + 1) * staminaMoneyByLevel;
		}

		for (int i = 0; i < incomeLevel + 1; i++)
		{
			currentIncomeLevelMoney += (incomeLevel + 1) * incomeMoneyByLevel;
		}

		staminaLevelText.text = (staminaLevel + 1).ToString();
		incomeaLevelText.text = (incomeLevel + 1).ToString();

		staminaMoneyText.text = (currentStaminaLevelMoney).ToString();
		incomeMoneyText.text = (currentIncomeLevelMoney).ToString();
	}

    public void BuyStaminaUpgrade()
    {
        if (money >= currentStaminaLevelMoney)
        {
            staminaLevel++;
            money -= currentStaminaLevelMoney;
			Vibrate();
            CheckLevels();
        }
		Save();
    }

	public void BuyIncomeUpgrade()
	{
		if (money >= currentIncomeLevelMoney)
		{
			incomeLevel++;
			money -= currentIncomeLevelMoney;
			Vibrate();
			CheckLevels();
		}
		Save();
	}

	public void BuyMachineUpgrade()
	{
		if (money >= currentMachineMoney && !IsMachineMaxLevel())
		{
			machineLevel++;
			money -= currentMachineMoney;
			Vibrate();
			CheckMachine();
		}
		Save();
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
		PlayerPrefs.SetInt("MachineLevel", machineLevel);
	}

	public void Load()
	{
		if (PlayerPrefs.HasKey("Money")) money = PlayerPrefs.GetFloat("Money");
		if (PlayerPrefs.HasKey("StaminaLevel")) staminaLevel = PlayerPrefs.GetInt("StaminaLevel");
		if (PlayerPrefs.HasKey("IncomeLevel")) incomeLevel = PlayerPrefs.GetInt("IncomeLevel");
		if (PlayerPrefs.HasKey("MachineLevel")) machineLevel = PlayerPrefs.GetInt("MachineLevel");
	}

	public void Vibrate()
	{
		if (!isVibrate) return;

		MMVibrationManager.StopAllHaptics();

		MMVibrationManager.TransientHaptic(0.85f, 0.05f, true, this);
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
