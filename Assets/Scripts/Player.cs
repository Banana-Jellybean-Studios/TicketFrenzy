using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    public int money = 0;
    public int moneyIncreaseOnTicket = 1;

    [Header("Ticket Paths")]
    public List<TicketFlow> ticketPaths;
    public float normalFlowSpeed = 2;
	public float fastFlowSpeed = 5;

    [Header("UI")]
    public TextMeshProUGUI moneyText;

    public static Player player { get; private set; }

    private void Awake()
    {
        if(player == null) player = this;
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
        money += moneyIncreaseOnTicket;
    }
}
