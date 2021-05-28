using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualCardPanel : MonoBehaviour
{
    public GameMaster gm;

    //Method for the childs to use.
    public bool throwCard(int playerIndex, Cards card)
    {
        if(gm.GameSt == GameMaster.GameState.PlayerTurn)
        {
            if(gm.playerThrowCard(playerIndex, card))
            {
                gm.nextTurn = true;
                return true;
            }
            else
            {
                return false;
            }

        }
        else
        {
            Debug.Log("No es el turno del jugador");
            return false;
        }
    }
}
