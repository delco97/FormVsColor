using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] private int boardSize = 4;
    private bool isGamePaused;

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
        colorPlayerTurnIndicator,
        gameMenu,
        endMenu;

    void Awake() {
        MakeSingleton();
        // Setup event handlers
        board.GetComponent<Board>().OnBoxClicked += HandleBoxClicked;                                   
        colorPlayerPieceIndicator.GetComponent<Box>().OnBoxClicked += HandleColorPlayerPieceClicked;    
        formPlayerPieceIndicator.GetComponent<Box>().OnBoxClicked += HandleFormPlayerPieceClicked;              
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
        isGamePaused = false;
        gameMenu.SetActive(false);
        BoardState firstBoardState = new BoardState(boardSize);
        BoxStatus formPlayerSelectedPiece = GetSelectedFormPlayerPiece();
        BoxStatus colorPlayerSelectedPiece = GetSelectedColorPlayerPiece();
        GameState firstGameState = new GameState(firstBoardState, isFormPlayerStarting, formPlayerStartingPieces,
            colorPlayerStartingPieces, formPlayerSelectedPiece, colorPlayerSelectedPiece);
        historyManager = new HistoryManager(firstGameState);

        // TODO: Select player type from match settings
        formPlayer = new HumanPlayer();
        colorPlayer = new HumanPlayer();
        UpdateUI();  
    }

    public void Update() {
        IPlayer currentPlayer = GetCurrentPlayer();
        if (!isGamePaused && !currentPlayer.HasMoved() && currentPlayer.IsAI()) {
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
        colorPlayerPieceIndicator.GetComponent<Box>().SetStatus(currentState.GetColorPlayerSelectedPiece());
        formPlayerPieceIndicator.GetComponent<Box>().SetStatus(currentState.GetFormPlayerSelectedPiece());
        redo.GetComponent<Button>().interactable = historyManager.RedoIsPossible();
        undo.GetComponent<Button>().interactable = historyManager.UndoIsPossible();
        MatchResult matchResult = currentState.GetMatchResult();
        if(matchResult != MatchResult.ONGOING) {
            isGamePaused = true;
            endMenu.SetActive(true);
            endMenu.GetComponent<EndMenu>().Initialize(matchResult);
        }
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
        IPlayer currentPlayer = GetCurrentPlayer();
        GameState currentState = historyManager.GetCurrentState();
        if (!isGamePaused && !currentPlayer.HasMoved() && !currentPlayer.IsAI()) {
            Move requestedMove = currentState.GetMove(row, col);
            GameState nextState = currentState.ApplyMove(requestedMove);
            historyManager.AddState(nextState);
            currentPlayer.Play(currentState);
            UpdateUI();
        }
    }

    public void HandleUndoButtonClicked() {
        historyManager.Undo();
        UpdateUI();
    }

    public void HandleRedoButtonClicked() {
        historyManager.Redo();
        UpdateUI();
    }

    public void HandleMenuButtonClicked() {
        gameMenu.SetActive(!gameMenu.activeSelf);
        isGamePaused = gameMenu.activeSelf;
    }

    public void HandleResumeButtonClicked() {
        isGamePaused = false;
        gameMenu.SetActive(false);
    }

    public void HandleResetButtonClicked() {
        InitializeGame();
    }

    public void HandleHomeButtonClicked() {
        SceneManager.LoadScene("MainMenu");
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