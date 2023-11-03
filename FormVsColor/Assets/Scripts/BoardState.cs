using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

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

    private void HandleCandidateBoxForSequence(List<BoxStatus> targetStatuses, List<BoxPosition> ongoingSequence,
        List<List<BoxPosition>> sequences, BoxPosition candidateBoxPosition, int? maxSequenceLength = null) {
        BoxStatus boxStatus = GetBoxStatus(candidateBoxPosition.row, candidateBoxPosition.col);
        if (targetStatuses.Contains(boxStatus)) {
            // If the current box status is a target status, then we add it to the ongoing sequence.
            ongoingSequence.Add(new BoxPosition(candidateBoxPosition.row, candidateBoxPosition.col));
            if (maxSequenceLength != null && ongoingSequence.Count > maxSequenceLength) {
                sequences.Add(new List<BoxPosition>(ongoingSequence));
                ongoingSequence.Clear();
            }
        }
        else if (ongoingSequence.Count > 0) {
            // If the current box is NOT in the group, but we have a sequence going on,
            // then we have reached the end of the sequence
            sequences.Add(new List<BoxPosition>(ongoingSequence));
            ongoingSequence.Clear();
        }
    }

    public List<List<BoxPosition>> GetConsecutiveSequences(List<BoxStatus> group, int? maxSequenceLength = null) {
        List<List<BoxPosition>> sequences = new List<List<BoxPosition>>();
        List<BoxPosition> currentRowSequence = new List<BoxPosition>();
        List<BoxPosition> currentColSequence = new List<BoxPosition>();
        List<BoxPosition> currentPrimaryDiagonalSequence = new List<BoxPosition>();
        List<BoxPosition> currentSecondaryDiagonalSequence = new List<BoxPosition>();

        for (int row = 0; row < GetSize(); row++) {
            HandleCandidateBoxForSequence(group, currentPrimaryDiagonalSequence, sequences, new BoxPosition(row, row),
                maxSequenceLength);
            HandleCandidateBoxForSequence(group, currentSecondaryDiagonalSequence, sequences,
                new BoxPosition(row, GetSize() - row - 1), maxSequenceLength);
            for (int col = 0; col < GetSize(); col++) {
                HandleCandidateBoxForSequence(group, currentRowSequence, sequences, new BoxPosition(row, col),
                    maxSequenceLength);
                HandleCandidateBoxForSequence(group, currentColSequence, sequences, new BoxPosition(col, row),
                    maxSequenceLength);
            }
        }
        
        if(currentRowSequence.Count > 0) {
            sequences.Add(new List<BoxPosition>(currentRowSequence));
        }
        if(currentColSequence.Count > 0) {
            sequences.Add(new List<BoxPosition>(currentColSequence));
        }
        if(currentPrimaryDiagonalSequence.Count > 0) {
            sequences.Add(new List<BoxPosition>(currentPrimaryDiagonalSequence));
        }
        if(currentSecondaryDiagonalSequence.Count > 0) {
            sequences.Add(new List<BoxPosition>(currentSecondaryDiagonalSequence));
        }
        
        return sequences;
    }

    private void AppDomainUnloadedException() {
        throw new NotImplementedException();
    }
}