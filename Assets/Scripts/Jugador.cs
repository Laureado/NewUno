using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Jugador : MonoBehaviour
{
    public bool Player = false;
    public int PlayerNumber = 0;
    public int NumberCards = 0;

    Text CardsText;

    public List<Cards> CardsList = new List<Cards>();

    private void Awake()
    {
        CardsText = GetComponentInChildren<Text>();
    }

    public void giveCard(Cards card)
    {
        CardsList.Add(card);
        NumberCards++;
        CardsText.text = NumberCards.ToString();
    }

    public void removeCard(Cards card)
    {
        CardsList.Remove(card);
        NumberCards--;
        CardsText.text = NumberCards.ToString();
    }

}
