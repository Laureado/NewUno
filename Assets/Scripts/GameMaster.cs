using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    public Transform PlayerListContainer;
    public Transform VisualCardsContainer;
    public Transform TurnPointer;
    public GameObject PlayerPanel;
    public GameObject DeckOfCardsObject;
    public GameObject PassTurnButton;
    public GameObject CongratulationsPanel;
    public GameObject SelectColorPanel;
    public Text ErrorPanel;
    public Text NumeroPanel;
    public SpriteRenderer StackCardsVisual;

    public Jugador JugadorPrefab;
    public VisualCard VisualCardPrefab;
    public IA GameIA;

    public byte TotalPlayers = 0;
    byte WonPlayer = 0;
    public int MainPlayerIndex = 0;
    public byte TurnNumber = 0;

    string comodinColor = "";

    bool IAturn = false;
    public bool cardPicked = false;
    public bool nextTurn = false;
    bool comodinWait = false;

    bool cancelTurn = false;
    bool reverseOrder = false;

    public List<Jugador> Jugadores = new List<Jugador>();
    public List<Cards> DeckCards;
    public List<Cards> StackCards = new List<Cards>();

    public GameState GameSt = GameState.IATurn;
    public enum GameState {Start,  PlayerTurn, IATurn, Idle };


    void Update()
    {
        if (GameSt != GameState.Start)
        {
            if (comodinWait)
            {
                nextTurn = false;
            }

            if (IAturn)
            {
                IAturn = false;
                StartCoroutine( HandleIt());
            }

            if (nextTurn)
            {
                nextTurn = false;
                turnPlayerSelection();
            }

            ArrowTurnPoint();
        }

        
    }


    private IEnumerator HandleIt()
    {
        yield return new WaitForSeconds(1.0f);
        GameIA.simulateTurn();
    }

    private void ArrowTurnPoint()
    {
        if(GameSt == GameState.IATurn)
        {
            TurnPointer.position = new Vector2(GameIA.jg.GetComponent<Transform>().position.x - 1f, GameIA.jg.GetComponent<Transform>().position.y);
        }
        else
        {
            TurnPointer.position = new Vector2(Jugadores[MainPlayerIndex].GetComponent<Transform>().position.x - 1f, Jugadores[MainPlayerIndex].GetComponent<Transform>().position.y);
        }
    }

    private void Awake()
    {
        GameSt = GameState.Start;
    }

    //Method that is activated after clicking the "continuar" button, it set all players, sort them, and sort all the cards.
    public void startGame()
    {
        SelectColorPanel.GetComponent<Transform>().SetAsFirstSibling();

        //validation of the input text.
        if (NumeroPanel.text == "")
            return;

        byte playerNumbers = byte.Parse(NumeroPanel.text);
        if (playerNumbers < 2 || playerNumbers > 6)
        {
            ErrorPanel.gameObject.SetActive(true);
        }
        else
        {
            //create all the players.
            TotalPlayers = playerNumbers;
            PlayerPanel.SetActive(false);

            for (int i = 0; i < TotalPlayers; i++)
            {
                Jugadores.Add(Instantiate(JugadorPrefab, new Vector2(0, 0), Quaternion.identity));
                if (i == 0) Jugadores[0].Player = true;
            }

            //random sort all the players.
            Random rng = new Random();

            int n = Jugadores.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n);
                Jugador value = Jugadores[k];
                Jugadores[k] = Jugadores[n];
                Jugadores[n] = value;
            }

            //Gathering all cards and sorting them.
            DeckCards = new List<Cards>(Object.FindObjectsOfType<Cards>());
            sortCards();

            //showing players in screen and giving them the cards.
            int turnNumber = 0;
            foreach (Jugador jgd in Jugadores)
            {
                jgd.transform.SetParent(PlayerListContainer, false);
                jgd.PlayerNumber = turnNumber;
                turnNumber++;
                pickCards(jgd, 7);
                //Give the deck of card who is the main player.
                if (jgd.Player)
                {
                    DeckOfCardsObject.GetComponent<DeckCard>().playerIndex = Jugadores.IndexOf(jgd);
                    jgd.GetComponent<Image>().color = Color.yellow;
                    MainPlayerIndex = Jugadores.IndexOf(jgd);
                }
            }

            //Throw first card to the stack.
            bool isColorful = false;
            int indexCard = 0;
            while (isColorful)
            {
                if (DeckCards[indexCard].CartaNumero < 10)
                {
                    isColorful = true;
                }
                else
                {
                    indexCard++;
                }
            }
            StackCards.Add(DeckCards[indexCard]);
            DeckCards.RemoveAt(indexCard);
            StackCardsVisual.sprite = StackCards[0].GetComponent<SpriteRenderer>().sprite;
            DeckOfCardsObject.SetActive(true);

            GameSt = GameState.Idle;
            nextTurn = true;

        }
    }

    //Game selects the next player.
    public void turnPlayerSelection()
    {
        cardPicked = false;
        PassTurnButton.SetActive(false);

        while (Jugadores[TurnNumber].CardsList.Count == 0)
        {
            selectNextPlayerTurn();
        }

        if (cancelTurn)
        {
            selectNextPlayerTurn();
            cancelTurn = false;
            nextTurn = true;
            return;
        }

        if (Jugadores[TurnNumber].Player)
        {
            GameSt = GameState.PlayerTurn;
        }
        else
        {
            GameSt = GameState.IATurn;
            GameIA.jg = Jugadores[TurnNumber];
            GameIA.PlayerIndex = TurnNumber;
            IAturn = true;
        }


        selectNextPlayerTurn();
    
    }

    //Select who will be the next player.
    void selectNextPlayerTurn()
    {
        do {
            if (!reverseOrder)
            {
                if (Jugadores.Count - 1 == TurnNumber)
                    TurnNumber = 0;
                else
                    TurnNumber++;
            }
            else
            {
                if (TurnNumber == 0)
                    TurnNumber = System.Convert.ToByte(Jugadores.Count - 1);
                else
                    TurnNumber--;
            }
        } while (Jugadores[TurnNumber].CardsList.Count == 0);

    }

    //It will trigger the next turn, can be activated by a IA or by the player.
    public void passTurnButtonReact()
    {
        if(GameSt != GameState.Idle)
            nextTurn = true;
    }


    //Method used for sorting all the cards.
    void sortCards()
    {
        Random rng = new Random();

        int n = DeckCards.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n);
            Cards value = DeckCards[k];
            DeckCards[k] = DeckCards[n];
            DeckCards[n] = value;
        }

    }

    //Give the selected player the amount of cards asked.
    public void pickCards(Jugador player, int cardsPicked)
    {
        if (cardPicked)
            return;

        while (cardsPicked>0)
        {

            cardsPicked--;
            player.giveCard(DeckCards[0]);

            //In case that the player that is given card to is the main player, then it will visually give the card.
            if (player.Player)
            {
                VisualCard vc = Instantiate(VisualCardPrefab, new Vector2(0, 0), Quaternion.identity);
                vc.transform.SetParent(VisualCardsContainer, false);
                vc.crearCarta(DeckCards[0], Jugadores.IndexOf(player));
            }
            DeckCards.RemoveAt(0);


            if (DeckCards.Count == 0)
            {
                Cards savedCard = StackCards[StackCards.Count - 1];
                StackCards.RemoveAt(StackCards.Count - 1);
                DeckCards.Clear();
                DeckCards.AddRange(StackCards);
                StackCards.Clear();
                Debug.Log(savedCard.CartaColor + savedCard.CartaNumero);
                StackCards.Add(savedCard);
                Debug.Log(StackCards[0].CartaNumero + StackCards[0].CartaColor);
            }
        }
    }

    //When the player pick the card from the deck, then this will give the player who did that the card.
    public void playerPickCardFromDeck(int playerIndex)
    {
        pickCards(Jugadores[playerIndex], 1);
    }

    //The player clicked a card, this card is discarded from the player's hand and thrown at the stack.
    public bool playerThrowCard(int playerIndex, Cards card)
    {
        int lastStack = StackCards.Count - 1;

        
        if (card.CartaNumero == StackCards[lastStack].CartaNumero || card.CartaColor == StackCards[lastStack].CartaColor || card.CartaColor == "comodin"  || card.CartaColor == comodinColor)
        {
            comodinColor = "";
            Jugadores[playerIndex].removeCard(card);
            StackCards.Add(card);
            StackCardsVisual.sprite = StackCards[lastStack+1].GetComponent<SpriteRenderer>().sprite;

            winCondition(playerIndex);

            switch (card.CartaNumero)
            {
                case 10:
                    cancelTurn = true;
                    break;

                case 11:
                    if (reverseOrder)
                        reverseOrder = false;
                    else
                        reverseOrder = true;

                    if (TotalPlayers - WonPlayer == 2)
                        cancelTurn = true;
                    break;

                case 12:
                    cardPicked = false;
                    pickCards(Jugadores[TurnNumber], 2);
                    cardPicked = true;
                    cancelTurn = true;
                    break;

                case 13:
                    if (Jugadores[playerIndex].Player)
                    {
                        GameSt = GameState.Idle;
                        SelectColorPanel.SetActive(true);
                        comodinWait = true;
                    }
                    else
                    {
                        int randomColor = Random.Range(0, 3);

                        switch (randomColor)
                        {
                            case 0:
                                comodinColor = "rojo";
                                break;

                            case 1:
                                comodinColor = "azul";
                                break;

                            case 2:
                                comodinColor = "amarillo";
                                break;

                            case 3:
                                comodinColor = "verde";
                                break;
                        }
                    }
                    break;

                    //por ahora el +4 actua como comodin
                case 14:

                    cardPicked = false;
                    pickCards(Jugadores[TurnNumber], 4);
                    cardPicked = true;
                    cancelTurn = true;

                    if (Jugadores[playerIndex].Player)
                    {
                        GameSt = GameState.Idle;
                        SelectColorPanel.SetActive(true);
                        comodinWait = true;
                    }
                    else
                    {
                        int randomColor = Random.Range(0, 3);

                        switch (randomColor)
                        {
                            case 0:
                                comodinColor = "rojo";
                                break;

                            case 1:
                                comodinColor = "azul";
                                break;

                            case 2:
                                comodinColor = "amarillo";
                                break;

                            case 3:
                                comodinColor = "verde";
                                break;
                        }

                    }
                    break;

                default:
                    break;
            }

            
            return true;
        }
        else
        {
            Debug.Log("La carta " + card.CartaNumero + ". " + card.CartaColor +  " no se puede colocar.");
            return false;
        }

    }

    //This will activate when the player loose or win the game.
    void winCondition(int playerIndex)
    {
        if (Jugadores[playerIndex].CardsList.Count == 0)
        {
            if (Jugadores[playerIndex].Player)
            {
                GameSt = GameState.Start;
                IAturn = false;
                nextTurn = false;
                CongratulationsPanel.SetActive(true);
                DeckOfCardsObject.SetActive(false);
                StackCardsVisual.gameObject.SetActive(false);
                VisualCardsContainer.gameObject.SetActive(false);
                WonPlayer = 100;
            }
            WonPlayer++;

            if(WonPlayer == TotalPlayers-1)
            {
                GameSt = GameState.Start;
                IAturn = false;
                nextTurn = false;
                CongratulationsPanel.SetActive(true);
                CongratulationsPanel.GetComponentInChildren<Text>().text = "Perdiste";
                DeckOfCardsObject.SetActive(false);
                StackCardsVisual.gameObject.SetActive(false);
                VisualCardsContainer.gameObject.SetActive(false);
            }

        }
    }

    //This will tell the game wich color the player selected
    public void playerColorSelect(string color)
    {
        comodinColor = color;
        SelectColorPanel.SetActive(false);
        comodinWait = false;
        nextTurn = true;
    }

    public void replayButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
