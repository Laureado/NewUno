using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualCard : MonoBehaviour
{
    VisualCardPanel Vcp;
    public Cards Card;
    public int PlayerIndex;

    //When created, it takes the player index and the card propierties.
    public void crearCarta(Cards card, int playerIndex)
    {
        PlayerIndex = playerIndex;
        Card = card;

        GetComponent<SpriteRenderer>().sprite = card.GetComponent<SpriteRenderer>().sprite;
        Vcp = GetComponentInParent<VisualCardPanel>();
    }

    //When clicked, it will confirm if it can be thrown, and if true, throw it.
    private void OnMouseDown()
    {
        if (Vcp.throwCard(PlayerIndex, Card)) Destroy(gameObject);
    }
}
