using System;

public class BoardState {
    private BoxStatus[,] boxes;

    public BoardState(BoxStatus[,] boxes) {
        int rows = boxes.GetLength(0);
        int cols = boxes.GetLength(1);
        if (rows != cols) throw new ArgumentException("Board must be square");
        if ((rows * cols) % 2 != 0) throw new ArgumentException("Board must have an even number of boxes");
        this.boxes = boxes;
    }

    public BoardState(int boardSize) {
        if ((boardSize * boardSize) % 2 != 0) throw new ArgumentException("Board must have an even number of boxes");
        this.boxes = new BoxStatus[boardSize, boardSize];
        for (int i = 0; i < boardSize; i++) {
            for (int j = 0; j < boardSize; j++) {
                boxes[i, j] = BoxStatus.EMPTY;
            }
        }
    }

    public BoardState(BoardState other) {
        this.boxes = new BoxStatus[other.GetSize(), other.GetSize()];
        for (int i = 0; i < other.GetSize(); i++) {
            for (int j = 0; j < other.GetSize(); j++) {
                boxes[i, j] = other.GetBoxStatus(i, j);
            }
        }
    }

    public BoxStatus GetBoxStatus(int row, int col) {
        return boxes[row, col];
    }

    public void SetBoxStatus(int row, int col, BoxStatus status) {
        boxes[row, col] = status;
    }


    public int GetSize() {
        return boxes.GetLength(0);
    }

    /*public List<List<BoxPosition>> GetConsecutiveSequences() {
        List<List<BoxPosition>> lines = new List<List<BoxPosition>>();
        for (int row = 0; row < GetSize(); row++) {
            for (int col = 0; col < GetSize(); col++) {
            }
        }
    }*/
}