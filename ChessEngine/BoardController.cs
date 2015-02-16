using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    public class BoardController
    {
        public  Board           cur_board { get; private set; }
        private Stack<Board>    board_history;

        public BoardController()
        {
            cur_board = new Board();
            board_history = new Stack<Board>();
        }

        /// <summary>
        /// Sets the board up for a new game
        /// </summary>
        public void Reset()
        {
            cur_board.Reset();
            board_history.Clear();
        }

        /// <summary>
        /// Gets all moves on the current board for the specified player
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public List<Move> GetMoves( PlayerColor player )
        {
            return GetMoves( player, cur_board );
        }

        /// <summary>
        /// Makes a move on the current board
        /// </summary>
        /// <param name="m"></param>
        public void MakeMove( Move m )
        {
            board_history.Push( cur_board );
            cur_board = MakeMove( m, cur_board );
        }

        /// <summary>
        /// Undoes the last move, if there was a last move
        /// </summary>
        public void UndoMove()
        {
            if (board_history.Count > 0)
            {
                cur_board = board_history.Pop();
            }
        }

        /// <summary>
        /// Uses the AI to make a move for the player
        /// </summary>
        /// <param name="player"></param>
        public void MakeMove(PlayerColor player)
        {
            Move m = AI.DetermineMove(cur_board, player);

            MakeMove(m);
        }

        /// <summary>
        /// Gets all possible legal moves on the board for the current player
        /// </summary>
        /// <param name="player"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        internal static List<Move> GetMoves( PlayerColor player, Board board )
        {
            List<Move> ret = new List<Move>();

            PlayerColor enemy;
            int player_king_index;

            if( player == PlayerColor.White )
            {
                enemy = PlayerColor.Black;
                player_king_index = Board.INDEX_W_KING;
            }
            else
            {
                enemy = PlayerColor.White;
                player_king_index = Board.INDEX_B_KING;
            }

            //get a list of all possible moves for the player
            List<Move> moves = board.GetMoves( player );

            //Perform each move, and make sure its legal
            foreach( Move m in moves )
            {
                Board next = MakeMove( m, board );

                if (!next.IsAttacked(enemy, next.pieces[player_king_index]))
                {
                    ret.Add( m );
                }
            }

            return ret;
        }

        /// <summary>
        /// Performs the move on the board, and returns the result
        /// </summary>
        /// <param name="m"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        internal static Board MakeMove( Move m, Board board )
        {
            Board next_board = new Board();

            //copy piece positions and castle possibilities
            //NOTE: do not copy en passant possibility, as it resets after a turn
            Array.Copy( board.pieces, next_board.pieces, Board.INDEX_COUNT );
            next_board.castle       = board.castle;
            next_board.enPassant    = 0;

            switch( m.type )
            {
                case MoveType.SinglePush:
                    if( m.promote_piece_index != -1 )
                    {
                        //promote the pawn
                        next_board.pieces[m.piece_index] &= ~m.from;
                        next_board.pieces[m.promote_piece_index] |= m.to;
                    }
                    else
                    {
                        //move the piece
                        next_board.pieces[m.piece_index] &= ~m.from;
                        next_board.pieces[m.piece_index] |= m.to;
                    }
                    break;

                case MoveType.DoublePush:
                    //move the piece
                    next_board.pieces[m.piece_index] &= ~m.from;
                    next_board.pieces[m.piece_index] |= m.to;

                    //set en passant
                    next_board.enPassant = m.to;
                    break;

                case MoveType.Slide:
                    //move the piece
                    next_board.pieces[m.piece_index] &= ~m.from;
                    next_board.pieces[m.piece_index] |= m.to;
                    break;

                case MoveType.Capture:
                    //remove the captured piece
                    next_board.RemovePiece( m.to );

                    if (m.promote_piece_index != -1)
                    {
                        //promote the pawn
                        next_board.pieces[m.piece_index] &= ~m.from;
                        next_board.pieces[m.promote_piece_index] |= m.to;
                    }
                    else
                    {
                        //move the piece
                        next_board.pieces[m.piece_index] &= ~m.from;
                        next_board.pieces[m.piece_index] |= m.to;
                    }
                    break;

                case MoveType.EnPassant:
                    //remove the en passant piece
                    next_board.RemovePiece( board.enPassant );

                    //move the piece
                    next_board.pieces[m.piece_index] &= ~m.from;
                    next_board.pieces[m.piece_index] |= m.to;
                    break;

                case MoveType.CASTLE_W_EAST:
                    //move the king
                    next_board.pieces[m.piece_index] &= ~m.from;
                    next_board.pieces[m.piece_index] |= m.to;

                    //move the rook
                    next_board.pieces[Board.INDEX_W_ROOKS] &= 0xFFFFFFFFFFFFFF7F;
                    next_board.pieces[Board.INDEX_W_ROOKS] |= 0x0000000000000020;

                    //update castling
                    next_board.castle &= ~( Board.CASTLE_BIT_W_EAST | Board.CASTLE_BIT_W_WEST );
                    break;

                case MoveType.CASTLE_W_WEST:
                    //move the king
                    next_board.pieces[m.piece_index] &= ~m.from;
                    next_board.pieces[m.piece_index] |= m.to;

                    //move the rook
                    next_board.pieces[Board.INDEX_W_ROOKS] &= 0xFFFFFFFFFFFFFFFE;
                    next_board.pieces[Board.INDEX_W_ROOKS] |= 0x0000000000000008;

                    //update castling
                    next_board.castle &= ~( Board.CASTLE_BIT_W_EAST | Board.CASTLE_BIT_W_WEST );
                    break;

                case MoveType.CASTLE_B_EAST:
                    //move the king
                    next_board.pieces[m.piece_index] &= ~m.from;
                    next_board.pieces[m.piece_index] |= m.to;

                    //move the rook
                    next_board.pieces[Board.INDEX_B_ROOKS] &= 0x7FFFFFFFFFFFFFFF;
                    next_board.pieces[Board.INDEX_B_ROOKS] |= 0x2000000000000000;

                    //update castling
                    next_board.castle &= ~(Board.CASTLE_BIT_B_EAST | Board.CASTLE_BIT_B_WEST);
                    break;

                case MoveType.CASTLE_B_WEST:
                    //move the king
                    next_board.pieces[m.piece_index] &= ~m.from;
                    next_board.pieces[m.piece_index] |= m.to;

                    //move the rook
                    next_board.pieces[Board.INDEX_B_ROOKS] &= 0xFEFFFFFFFFFFFFFF;
                    next_board.pieces[Board.INDEX_B_ROOKS] |= 0x0800000000000000;

                    //update castling
                    next_board.castle &= ~(Board.CASTLE_BIT_B_EAST | Board.CASTLE_BIT_B_WEST);
                    break;
            }

            return next_board;
        }
    }
}
