using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{

    [SerializeField] GameObject buttonContainer;
    [SerializeField] TMP_Text resultText;

    // Player 1 is represented by 1, and Player 2 (AI) is represented by 2
    const int PLAYER = 1;
    const int AI = 2;

    // array of player chosed tic tac toe square indexes
    public int[] playerChoices = new int[5];
    // array of AI chosed tic tac toe square indexes
    public int[] aiChoices = new int[5];

    // array of the boards current state 
    public int[] boardState = new int[9];

    // Define the Tic Tac Toe board
    //int[] board = new int[9];

    // win conditions for tic tac toe
    //public int[] winConditions = new int[];
    
    int[,] winningConditions = new int[8, 3] { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 }, { 0, 4, 8 }, { 2, 4, 6 } };

    // Start is called before the first frame update
    void Start()
    {
        InitialiseGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitialiseGame()
    {
        resultText.text = "";

        //reset the playerChoices array
        for (int i = 0; i < playerChoices.Length; i++)
        {
            playerChoices[i] = 0;
        }

        //reset the aiChoices array
        for (int i = 0; i < aiChoices.Length; i++)
        {
            aiChoices[i] = 0;
        }

        // initialise the boardState array
        for (int i = 0; i < boardState.Length; i++)
        {
            boardState[i] = 0;
        }

        // Enable the button container so the user can click on the tic tac toe buttons
        buttonContainer.SetActive(true);

        // set text of each button to empty string
        foreach (Button button in buttonContainer.GetComponentsInChildren<Button>())
        {
            button.interactable = true;
            button.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
    }

    // This method is called when the user clicks on one of the tic tac toe buttons
    public void ButtonClicked(Button button)
    {
        // Get the text component of the button
        button.GetComponentInChildren<TextMeshProUGUI>().text = "X";

        // Disable the button so it can't be clicked again
        button.interactable = false;

        // append the index of the button to the playerChoices array
        for (int i = 0; i < playerChoices.Length; i++)
{
            if (playerChoices[i] == 0)
            {
                playerChoices[i] = int.Parse(button.name)-1; // Assign the value 5 to the first empty element
                updateCurrentBoardState(int.Parse(button.name)-1, 1);
                break; // Exit the loop after assigning the value
            }
        }

        //Check if the player has won
        if (CheckForWin(playerChoices))
        {
            // Display the result text
            resultText.text = "You Win!";

            // Disable the button container so the user can't click on the tic tac toe buttons
            buttonContainer.SetActive(false);

            Invoke("InitialiseGame", 3);
        }
        else
        {
            // AI turn to make move
            AITurn();
        }
    }

    // AI method to choose tic tac toe square
    private void AITurn()
    {
        // Get the index of the best move from the AI
        int index = FindBestMove();
        Debug.Log("AI chose index: " + index);

        // Get the button with the same index as the best move
        Button button = buttonContainer.transform.GetChild(index).GetComponent<Button>();

        // Set the text of the button to O
        button.GetComponentInChildren<TextMeshProUGUI>().text = "O";

        // Disable the button so it can't be clicked again
        button.interactable = false;

        // Update the current board state and the AI choices array
        updateCurrentBoardState(index, 2);
        aiChoices[index] = index;

        if (CheckForWin(aiChoices))
        {
            // Display the result text
            resultText.text = "You Lose!";

            // Disable the button container so the user can't click on the tic tac toe buttons
            buttonContainer.SetActive(false);

            Invoke("InitialiseGame", 3);
        }
    }


    private void updateCurrentBoardState(int index, int person)
    {
        boardState[index] = person;
    }

    private int[] getEmptyBoardSquares()
    {
        int[] emptySquares = new int[9];

        for (int i = 0; i < boardState.Length; i++)
        {
            if (boardState[i] == 0)
            {
                emptySquares[i] = i + 1;
            }
        }

        return emptySquares;
    }

    // Check win state of tic tac toe
    private bool CheckForWin(int[] choices)
    {
        // check the player choices array if any of the win conditions are met
        if(choices.Contains(1) && choices.Contains(2) && choices.Contains(3))
        {
            return true;
        }
        else if (choices.Contains(4) && choices.Contains(5) && choices.Contains(6))
        {
            return true;
        }
        else if (choices.Contains(7) && choices.Contains(8) && choices.Contains(9))
        {
            return true;
        }
        else if (choices.Contains(1) && choices.Contains(4) && choices.Contains(7))
        {
            return true;
        }
        else if (choices.Contains(2) && choices.Contains(5) && choices.Contains(8))
        {
            return true;
        }
        else if (choices.Contains(3) && choices.Contains(6) && choices.Contains(9))
        {
            return true;
        }
        else if (choices.Contains(1) && choices.Contains(5) && choices.Contains(9))
        {
            return true;
        }
        else if (choices.Contains(3) && choices.Contains(5) && choices.Contains(7))
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    // This function returns the best move for the AI using the Minimax algorithm
    int FindBestMove()
    {
        float bestScore = -Mathf.Infinity;
        int bestMove = -1;

        Debug.Log("Board State: " + boardState);

        // Iterate through all empty cells on the board
        for (int i = 0; i < 9; i++)
        {
            if (boardState[i] == 0)
            {
                Debug.Log("Trying move: " + i);
                // Try this move for the AI
                boardState[i] = AI;

                // Calculate the score for this move
                int score = Minimax(boardState, 0, false);
                Debug.Log("Score: " + score);
                // Undo the move
                boardState[i] = 0;

                // Update the best score and best move
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = i;
                    Debug.Log("Best score: " + bestScore);
                }
            }
        }
        Debug.Log("Best score: " + bestMove);
        return (int)bestMove;
    }

    // This function implements the Minimax algorithm for Tic Tac Toe
    int Minimax(int[] board, int depth, bool isMaximising)
    {
        // Check if the game is over
        int result = CheckWin(board);
        if (result != 0)
        {
            return result * depth;
        }

        // If it's the AI's turn
        if (isMaximising)
        {
            float bestScore = -Mathf.Infinity;

            // Iterate through all empty cells on the board
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == 0)
                {
                    // Try this move for the AI
                    board[i] = AI;

                    // Recursively call Minimax to calculate the score for this move
                    int score = Minimax(board, depth + 1, false);

                    // Undo the move
                    board[i] = 0;

                    // Update the best score
                    bestScore = Mathf.Max(bestScore, score);
                }
            }

            return (int)bestScore;
        }
        // If it's the player's turn
        else
        {
            float bestScore = Mathf.Infinity;

            // Iterate through all empty cells on the board
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == 0)
                {
                    // Try this move for the player
                    board[i] = PLAYER;

                    // Recursively call Minimax to calculate the score for this move
                    int score = Minimax(board, depth + 1, true);

                    // Undo the move
                    board[i] = 0;

                    // Update the best score
                    bestScore = Mathf.Min(bestScore, score);
                }
            }

            return (int)bestScore;
        }
    }

    // This function checks if a player has won the game
    int CheckWin(int[] board)
    {
        // Check rows
        for (int i = 0; i < 9; i += 3)
        {
            if (board[i] != 0 && board[i] == board[i + 1] && board[i] == board[i + 2])
            {
                return board[i];
            }
        }

        // Check columns
        for (int i = 0; i < 3; i++)
        {
            if (board[i] != 0 && board[i] == board[i + 3] && board[i] == board[i + 6]) {
                return board[i];
            }
        }

        // Check diagonals
        if (board[0] != 0 && board[0] == board[4] && board[0] == board[8])
        {
            return board[0];
        }
        if (board[2] != 0 && board[2] == board[4] && board[2] == board[6])
        {
            return board[2];
        }

        // Check for a tie
        bool isTie = true;
        for (int i = 0; i < 9; i++)
        {
            if (board[i] == 0)
            {
                isTie = false;
                break;
            }
        }
        if (isTie)
        {
            return 0;
        }

        // If no winner yet
        return -1;
    }

    //
}
