using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckCard : MonoBehaviour
{
    public GameMaster gm;
    public int playerIndex;


    //When clicked, it will send a command to the game master for it to send a card to the current player.
    void OnMouseDown()
    {
        if(gm.GameSt == GameMaster.GameState.PlayerTurn && gm.cardPicked == false)
        {
            gm.playerPickCardFromDeck(playerIndex);
            gm.cardPicked = true;
            gm.PassTurnButton.SetActive(true);
        }
        else
        {
            Debug.Log("No es el turno del jugador.");
        }
    }
}
