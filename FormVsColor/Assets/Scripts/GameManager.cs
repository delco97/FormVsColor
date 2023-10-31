using System;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { private set; get; }
    private HistoryManager historyManager;
    
    private IPlayer formPlayer;
    private IPlayer colorPlayer;
    
    // Starting game state
    [SerializeField] private bool isFormPlayerStarting = true;
    [SerializeField] private int formPlayerStartingPieces = 8;
    [SerializeField] private int colorPlayerStartingPieces = 8;
    [SerializeField] private int nRows = 4, nCols = 4;

    // UI elements
    [SerializeField] private GameObject 
        board,
        undo,
        redo,
        formPlayerPieceLeft, 
        colorPlayerPieceLeft,
        formPlayerPieceIndicator,
        colorPlayerPieceIndicator,
        formPlayerTurnIndicator,
        colorPlayerTurnIndicator;

    void Awake() {
        MakeSingleton();
        InitializeGame();
    }

    private void MakeSingleton() {
        if (Instance == null) {
            Instance = GetComponent<GameManager>();
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(Instance.gameObject);
        }
    }

    private void InitializeGame() {
        // Set the first game state 
        BoardState firstBoardState = new BoardState(nRows, nCols);
        BoxStatus formPlayerSelectedPiece = GetSelectedFormPlayerPiece();
        BoxStatus colorPlayerSelectedPiece = GetSelectedColorPlayerPiece();
        GameState firstGameState = new GameState(firstBoardState, isFormPlayerStarting, formPlayerStartingPieces, colorPlayerStartingPieces, formPlayerSelectedPiece, colorPlayerSelectedPiece);
        historyManager = new HistoryManager(firstGameState);
        board.GetComponent<Board>().OnBoxClicked += HandleBoxClicked;
        colorPlayerPieceIndicator.GetComponent<Box>().OnBoxClicked += HandleColorPlayerPieceClicked;
        formPlayerPieceIndicator.GetComponent<Box>().OnBoxClicked += HandleFormPlayerPieceClicked;
        UpdateUI();
        
        // TODO: Select player type from settings
        formPlayer = new HumanPlayer();
        colorPlayer = new HumanPlayer();
    }

    public void Update() {
        IPlayer currentPlayer = GetCurrentPlayer();
        if (!currentPlayer.HasMoved() && currentPlayer.IsAI()) {
            GameState nextState = currentPlayer.Play(historyManager.GetCurrentState());
            historyManager.AddState(nextState);
            UpdateUI();
        }
    }

    private void UpdateUI() {
        GameState currentState = historyManager.GetCurrentState();
        board.GetComponent<Board>().SetState(currentState.GetBoardState());
        formPlayerPieceLeft.GetComponent<TextMeshProUGUI>().text = currentState.GetFormPlayerPiecesLeft().ToString();
        colorPlayerPieceLeft.GetComponent<TextMeshProUGUI>().text = currentState.GetColorPlayerPiecesLeft().ToString();
        formPlayerTurnIndicator.GetComponent<Image>().enabled = currentState.IsFormPlayerTurn();
        colorPlayerTurnIndicator.GetComponent<Image>().enabled = currentState.IsColorPlayerTurn();
        redo.GetComponent<Button>().interactable = historyManager.RedoIsPossible();
        undo.GetComponent<Button>().interactable = historyManager.UndoIsPossible();
    }
    
    private IPlayer GetCurrentPlayer() {
        return historyManager.GetCurrentState().IsFormPlayerTurn() ? formPlayer : colorPlayer;
    }
    
    private BoxStatus GetSelectedFormPlayerPiece() {
        return formPlayerPieceIndicator.GetComponent<Box>().GetStatus();
    }
    
    private BoxStatus GetSelectedColorPlayerPiece() {
        return colorPlayerPieceIndicator.GetComponent<Box>().GetStatus();
    }    

    public void HandleBoxClicked(int row, int col) {
        print("Box pressed in position: " + row + ", " + col);
        IPlayer currentPlayer = GetCurrentPlayer();
        GameState currentState = historyManager.GetCurrentState();
        bool isFormPlayerTurn = currentState.IsFormPlayerTurn();
        if (!currentPlayer.HasMoved() && !currentPlayer.IsAI()) {
            // TODO: Check if move is valid
            currentPlayer.Play(currentState);
        }
    }
    
    public void HandleUndoButtonClicked() {
        Debug.Log("Undo button pressed");
    }
    
    public void HandleRedoButtonClicked() {
        Debug.Log("Redo button pressed");
    }
    
    public void HandleMenuButtonClicked() {
        Debug.Log("Menu button pressed");
    }

    public void HandleColorPlayerPieceClicked(int row, int col) {
        GameState nextState = new GameState(historyManager.GetCurrentState());
        BoxStatus nextStatus = colorPlayerPieceIndicator.GetComponent<Box>().NextStatus();
        nextState.SetColorPlayerSelectedPiece(nextStatus);
        historyManager.AddState(nextState);
    }
    
    public void HandleFormPlayerPieceClicked(int row, int col) {
        GameState nextState = new GameState(historyManager.GetCurrentState());
        BoxStatus nextStatus = formPlayerPieceIndicator.GetComponent<Box>().NextStatus();
        nextState.SetFormPlayerSelectedPiece(nextStatus);
        historyManager.AddState(nextState);
    }    
    
}