public class Move {
    public int posX, posY;
    public MoveType moveType;

    public Move(int posX, int posY, MoveType moveType) {
        this.posX = posX;
        this.posY = posY;
        this.moveType = moveType;
    }

    public override bool Equals(object obj) {
        if (obj is Move otherMove) {
            return posX == otherMove.posX && posY == otherMove.posY && moveType == otherMove.moveType;
        }

        return false;
    }

    public override int GetHashCode() {
        return (posX, posY, moveType).GetHashCode();
    }
}