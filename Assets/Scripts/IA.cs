using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA : MonoBehaviour
{
    public int PlayerIndex;

    public Jugador jg;
    public GameMaster gm;

    public void simulateTurn()
    {
        bool cardThrowed = false;


        foreach (Cards card in jg.CardsList)
        {
            cardThrowed = gm.playerThrowCard(PlayerIndex, card);
            if (cardThrowed)
                break;
        }

        if (!cardThrowed)
        {
            gm.cardPicked = false;
            gm.pickCards(jg, 1);
            gm.playerThrowCard(PlayerIndex, jg.CardsList[jg.CardsList.Count-1]);
        }

        gm.nextTurn = true;
    }
}
