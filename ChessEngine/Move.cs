using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    //Possible types of moves
    public enum MoveType
    {
        SinglePush,
        DoublePush,
        Slide,
        Capture,
        EnPassant,
        CASTLE_W_EAST,
        CASTLE_W_WEST,
        CASTLE_B_EAST,
        CASTLE_B_WEST
    }

    public class Move
    {
        public MoveType type;                   //the type of move
        public int      piece_index;            //the index of the piece moving
        public ulong    from;                   //from location
        public ulong    to;                     //to location
        public int      promote_piece_index;    //index of piece to promote to, -1 when not used

        public Move( MoveType type, int piece_index, ulong from, ulong to )
        {
            this.type                   = type;
            this.piece_index            = piece_index;
            this.from                   = from;
            this.to                     = to;
            this.promote_piece_index    = -1;

        }

        public Move( MoveType type, int piece_index, ulong from, ulong to, int promote_piece_index )
        {
            this.type                   = type;
            this.piece_index            = piece_index;
            this.from                   = from;
            this.to                     = to;
            this.promote_piece_index    = promote_piece_index;
        }
    }
}
