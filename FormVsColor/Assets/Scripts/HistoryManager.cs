using System.Collections.Generic;

public class HistoryManager {
    private List<GameState> history;
    private int currentStateIndex;
    private int lastValidStateIndex;
    private GameState initialState;

    public HistoryManager(GameState initialState) {
        history = new List<GameState>();
        history.Add(initialState);
        currentStateIndex = 0;
        lastValidStateIndex = 0;
        this.initialState = initialState;
    }

    public void Reset() {
        history.Clear();
        history.Add(initialState);
        currentStateIndex = 0;
        lastValidStateIndex = 0;
    }

    public bool UndoIsPossible() {
        return currentStateIndex > 0;
    }

    public bool RedoIsPossible() {
        return currentStateIndex < lastValidStateIndex;
    }

    public GameState GetCurrentState() {
        return history[currentStateIndex];
    }

    public GameState Undo() {
        if (UndoIsPossible()) {
            return history[--currentStateIndex];
        }

        return null;
    }

    public GameState Redo() {
        if (RedoIsPossible()) {
            return history[++currentStateIndex];
        }

        return null;
    }

    public void AddState(GameState gameState) {
        if (currentStateIndex < history.Count) {
            history.Insert(++currentStateIndex, gameState);
        }
        else {
            history.Add(gameState);
            currentStateIndex++;
        }

        lastValidStateIndex = currentStateIndex;
    }
}