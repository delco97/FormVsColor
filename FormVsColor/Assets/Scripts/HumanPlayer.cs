
public class HumanPlayer : IPlayer {
    private bool moveDone;

    public bool IsAI() {
        return false;
    }

    public GameState Play(GameState gameState) {
        moveDone = false;
        return gameState;
    }

    public bool HasMoved() {
        return moveDone;
    }
}