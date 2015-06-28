namespace ChessEngine
{
    public enum MoveType : byte
    {
        SinglePush,         //pawn one space forward
        DoublePush,         //pawn two spaces forward
        Slide,              //rook, knight, bishop, queen, and king
        Capture,            //a piece was captured
        EnPassant,          //enpassant move
        CASTLE_W_EAST,      //castle white east
        CASTLE_W_WEST,      //castle white west
        CASTLE_B_EAST,      //castle black east
        CASTLE_B_WEST       //castle black west
    }

    public class Move
    {
        public MoveType type;                   //the type of move
        public byte     piece_index;            //the index of the piece moving
        public ulong    from;                   //from location
        public ulong    to;                     //to location
        public byte     promote_piece_index;    //index of piece to promote to, INDEX_COUNT when not used

        public Move(MoveType type, byte piece_index, ulong from, ulong to, byte promote_piece_index = Board.INDEX_COUNT)
        {
            this.type                   = type;
            this.piece_index            = piece_index;
            this.from                   = from;
            this.to                     = to;
            this.promote_piece_index    = promote_piece_index;
        }
    }
}
