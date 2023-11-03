using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameState {
    private BoardState boardState;
    private bool isFormPlayerTurn;
    private int formPlayerPiecesLeft;
    private int colorPlayerPiecesLeft;
    private BoxStatus formPlayerSelectedPiece;
    private BoxStatus colorPlayerSelectedPiece;


    public GameState(BoardState boardState, bool isFormPlayerTurn, int formPlayerPiecesLeft,
        int colorPlayerPiecesLeft, BoxStatus formPlayerSelectedPiece, BoxStatus colorPlayerSelectedPiece) {
        this.boardState = boardState;
        this.isFormPlayerTurn = isFormPlayerTurn;
        this.formPlayerPiecesLeft = formPlayerPiecesLeft;
        this.colorPlayerPiecesLeft = colorPlayerPiecesLeft;
        this.formPlayerSelectedPiece = formPlayerSelectedPiece;
        this.colorPlayerSelectedPiece = colorPlayerSelectedPiece;
    }

    public GameState(GameState other) {
        boardState = new BoardState(other.GetBoardState());
        isFormPlayerTurn = other.isFormPlayerTurn;
        formPlayerPiecesLeft = other.formPlayerPiecesLeft;
        colorPlayerPiecesLeft = other.colorPlayerPiecesLeft;
        formPlayerSelectedPiece = other.formPlayerSelectedPiece;
        colorPlayerSelectedPiece = other.colorPlayerSelectedPiece;
    }

    public BoardState GetBoardState() {
        return boardState;
    }

    public bool IsFormPlayerTurn() {
        return isFormPlayerTurn;
    }

    public bool IsColorPlayerTurn() {
        return !isFormPlayerTurn;
    }

    public int GetWinningSequenceLength() {
        return boardState.GetSize();
    }

    public void ChangeTurn() {
        isFormPlayerTurn = !isFormPlayerTurn;
    }

    public int GetFormPlayerPiecesLeft() {
        return formPlayerPiecesLeft;
    }

    public int GetColorPlayerPiecesLeft() {
        return colorPlayerPiecesLeft;
    }

    public void SetColorPlayerSelectedPiece(BoxStatus colorPlayerSelectedPiece) {
        if (!BoxStatusManager.GetColorStatuses().Contains(colorPlayerSelectedPiece))
            throw new Exception($"Selected invalid status {colorPlayerSelectedPiece} for color player");
        this.colorPlayerSelectedPiece = colorPlayerSelectedPiece;
    }

    public BoxStatus GetColorPlayerSelectedPiece() {
        return colorPlayerSelectedPiece;
    }

    public void SetFormPlayerSelectedPiece(BoxStatus formPlayerSelectedPiece) {
        if (!BoxStatusManager.GetFormStatuses().Contains(formPlayerSelectedPiece))
            throw new Exception($"Selected invalid status {formPlayerSelectedPiece} for form player");
        this.formPlayerSelectedPiece = formPlayerSelectedPiece;
    }

    public BoxStatus GetFormPlayerSelectedPiece() {
        return formPlayerSelectedPiece;
    }


    public List<Move> GetPossibleMoves() {
        List<Move> possibleMoves = new List<Move>();

        List<BoxStatus> allowedBoxTypeClick = new List<BoxStatus>();
        allowedBoxTypeClick.Add(BoxStatus.EMPTY);
        if (IsColorPlayerTurn()) {
            allowedBoxTypeClick.AddRange(BoxStatusManager.GetColorStatuses());
        }
        else {
            allowedBoxTypeClick.AddRange(BoxStatusManager.GetFormStatuses());
        }

        BoardState boardState = GetBoardState();
        for (int row = 0; row < boardState.GetSize(); row++) {
            for (int col = 0; col < boardState.GetSize(); col++) {
                BoxStatus boxStatus = boardState.GetBoxStatus(row, col);
                if (allowedBoxTypeClick.Contains(boxStatus)) {
                    if (boxStatus == BoxStatus.EMPTY) {
                        possibleMoves.Add(new Move(row, col, MoveType.ADD));
                    }
                    else {
                        possibleMoves.Add(new Move(row, col, MoveType.FLIP));
                    }
                }
            }
        }

        return possibleMoves;
    }

    public bool IsValidMove(Move move) {
        List<Move> possibleMoves = GetPossibleMoves();
        return possibleMoves.Contains(move);
    }

    protected void AddPiece(int row, int col) {
        if (IsFormPlayerTurn()) {
            boardState.SetBoxStatus(row, col, formPlayerSelectedPiece);
            formPlayerPiecesLeft--;
        }
        else {
            boardState.SetBoxStatus(row, col, colorPlayerSelectedPiece);
            colorPlayerPiecesLeft--;
        }
    }

    protected void FlipPiece(int row, int col) {
        if (IsFormPlayerTurn()) {
            boardState.SetBoxStatus(row, col,
                boardState.GetBoxStatus(row, col) == BoxStatus.FORM_BLACK_CROSS
                    ? BoxStatus.FORM_WHITE_CIRCLE
                    : BoxStatus.FORM_BLACK_CROSS);
        }
        else {
            boardState.SetBoxStatus(row, col,
                boardState.GetBoxStatus(row, col) == BoxStatus.COLOR_WHITE_CROSS
                    ? BoxStatus.COLOR_BLACK_CIRCLE
                    : BoxStatus.COLOR_WHITE_CROSS);
        }
    }

    public GameState ApplyMove(Move move) {
        if (!IsValidMove(move)) {
            throw new Exception($"Invalid move {move}");
        }

        GameState nextState = new GameState(this);
        if (move.moveType == MoveType.ADD) {
            nextState.AddPiece(move.posX, move.posY);
        }
        else {
            nextState.FlipPiece(move.posX, move.posY);
        }

        nextState.ChangeTurn();
        return nextState;
    }

    public Move GetMove(int row, int col) {
        BoardState boardState = GetBoardState();
        BoxStatus selectedBoxStatus = boardState.GetBoxStatus(row, col);

        if (selectedBoxStatus == BoxStatus.EMPTY) {
            return new Move(row, col, MoveType.ADD);
        }

        return new Move(row, col, MoveType.FLIP);
    }

    public bool FormPlayerWon() {
        BoardState boardState = GetBoardState();
        List<List<BoxPosition>> formCandidateSequencesCross = boardState.GetConsecutiveSequences(
            new List<BoxStatus> { BoxStatus.FORM_BLACK_CROSS, BoxStatus.COLOR_WHITE_CROSS },
            GetWinningSequenceLength()
        );
        List<List<BoxPosition>> formCandidateSequencesCircle = boardState.GetConsecutiveSequences(
            new List<BoxStatus> { BoxStatus.FORM_WHITE_CIRCLE, BoxStatus.COLOR_BLACK_CIRCLE },
            GetWinningSequenceLength()
        );
        return formCandidateSequencesCross.Any(seq => seq.Count == GetWinningSequenceLength())
               || formCandidateSequencesCircle.Any(seq => seq.Count == GetWinningSequenceLength());
    }

    public bool ColorPlayerWon() {
        BoardState boardState = GetBoardState();
        List<List<BoxPosition>> colorCandidateSequencesCross = boardState.GetConsecutiveSequences(
            new List<BoxStatus> { BoxStatus.FORM_BLACK_CROSS, BoxStatus.COLOR_BLACK_CIRCLE },
            GetWinningSequenceLength()
        );
        List<List<BoxPosition>> colorCandidateSequencesCircle = boardState.GetConsecutiveSequences(
            new List<BoxStatus> { BoxStatus.COLOR_WHITE_CROSS, BoxStatus.FORM_WHITE_CIRCLE },
            GetWinningSequenceLength()
        );
        return colorCandidateSequencesCross.Any(seq => seq.Count == GetWinningSequenceLength())
               || colorCandidateSequencesCircle.Any(seq => seq.Count == GetWinningSequenceLength());
    }


    public MatchResult GetMatchResult() {
        if (FormPlayerWon()) {
            return MatchResult.FORM_WIN;
        }

        if (ColorPlayerWon()) {
            return MatchResult.COLOR_WIN;
        }

        // TODO: check if it's a draw. Define what a draw is.
        return MatchResult.ONGOING;
    }

    public override string ToString() {
        return $"GameState: {boardState}\n" +
               $"IsFormPlayerTurn: {isFormPlayerTurn}\n" +
               $"FormPlayerPiecesLeft: {formPlayerPiecesLeft}\n" +
               $"ColorPlayerPiecesLeft: {colorPlayerPiecesLeft}\n" +
               $"FormPlayerSelectedPiece: {formPlayerSelectedPiece}\n" +
               $"ColorPlayerSelectedPiece: {colorPlayerSelectedPiece}\n";
    }
}