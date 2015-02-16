using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    internal class ScoredMove
    {
        internal Move move;
        internal int score;

        internal ScoredMove( Move move, int score )
        {
            this.move  = move;
            this.score = score;
        }    
    }

    //internal static class AI
    internal static class AI
    {
        private const int AI_DEPTH      = 3;

        private const int WEIGHT_KING   = 200;
        private const int WEIGHT_QUEEN  = 9;
        private const int WEIGHT_ROOK   = 5;
        private const int WEIGHT_KNIGHT = 3;
        private const int WEIGHT_BISHOP = 3;
        private const int WEIGHT_PAWN   = 1;

        //http://chessprogramming.wikispaces.com/Alpha-Beta
        internal static Move DetermineMove( Board board, PlayerColor player )
        {
            List<Move> moves = BoardController.GetMoves( PlayerColor.Black, board );

            ScoredMove best = new ScoredMove( null, int.MinValue );

            foreach( Move m in moves )
            {
                Board next = BoardController.MakeMove( m, board );
                ScoredMove cur = AlphaBeta( next, m, AI_DEPTH, int.MinValue, int.MaxValue, true );

                if( cur.score > best.score )
                {
                    best.score = cur.score;
                    best.move = m;
                }
            }

            return best.move;
        }

        //black is considered the maximizing player
        //http://en.wikipedia.org/wiki/Alpha%E2%80%93beta_pruning
        private static ScoredMove AlphaBeta( Board board, Move move, int depth_left, int alpha, int beta, bool max_player )
        {
            if( depth_left == 0 )
            {
                return new ScoredMove( move, EvaluateBoardBlack( board ) );
            }

            if( max_player )
            {
                ScoredMove best = new ScoredMove( null, int.MinValue );
                List<Move> moves = BoardController.GetMoves( PlayerColor.Black, board );

                foreach( Move m in moves )
                {
                    ScoredMove cur = AlphaBeta( BoardController.MakeMove( m, board ), m, depth_left - 1, alpha, beta, false );

                    if( cur.score  >  best.score  ) 
                    {
                        best.score = cur.score;
                        best.move = m;
                    }
                    if( best.score >  alpha       ) alpha = best.score;
                    if( beta       <= alpha       ) break;
                }

                return best;
            }
            else
            {
                ScoredMove best = new ScoredMove( null, int.MaxValue );
                List<Move> moves = BoardController.GetMoves( PlayerColor.White, board );

                foreach( Move m in moves )
                {
                    ScoredMove cur = AlphaBeta( BoardController.MakeMove( m, board ), m, depth_left - 1, alpha, beta, true );

                    if( cur.score  <  best.score  )
                    {
                        best.score = cur.score;
                        best.move = m;
                    }
                    if( best.score <  alpha       ) alpha = best.score;
                    if( beta       <= alpha       ) break;
                }

                return best;
            }
        }

        /// <summary>
        /// Evaluates a board for the specified player
        /// https://chessprogramming.wikispaces.com/Evaluation
        /// </summary>
        /// <param name="board">the board to be evaluated</param>
        /// <param name="player">+1 for white, -1 for black</param>
        /// <returns></returns>
        private static int EvaluateBoardBlack( Board board )
        {
            //determine material score
            int material = ( WEIGHT_KING   * ( BitCount( board.pieces[ Board.INDEX_B_KING    ] ) - BitCount( board.pieces[ Board.INDEX_W_KING     ] ) ) )
                         + ( WEIGHT_QUEEN  * ( BitCount( board.pieces[ Board.INDEX_B_QUEENS  ] ) - BitCount( board.pieces[ Board.INDEX_W_QUEENS   ] ) ) )
                         + ( WEIGHT_ROOK   * ( BitCount( board.pieces[ Board.INDEX_B_ROOKS   ] ) - BitCount( board.pieces[ Board.INDEX_W_ROOKS    ] ) ) )
                         + ( WEIGHT_KNIGHT * ( BitCount( board.pieces[ Board.INDEX_B_KNIGHTS ] ) - BitCount( board.pieces[ Board.INDEX_W_KNIGHTS  ] ) ) )
                         + ( WEIGHT_BISHOP * ( BitCount( board.pieces[ Board.INDEX_B_BISHOPS ] ) - BitCount( board.pieces[ Board.INDEX_W_BISHOPS  ] ) ) )
                         + ( WEIGHT_PAWN   * ( BitCount( board.pieces[ Board.INDEX_B_PAWNS   ] ) - BitCount( board.pieces[ Board.INDEX_W_PAWNS    ] ) ) );

            //return the score, for the specified player
            return material;
        }

        //sparse ones - this will probably be fastest since most of the bits are 0
        //http://gurmeet.net/puzzles/fast-bit-counting-routines/
        //http://stackoverflow.com/questions/109023/how-to-count-the-number-of-set-bits-in-a-32-bit-integer
        internal static int BitCount( ulong n )
        {
            int count = 0;
            while( n != 0 )
            {
                count++;
                n &= ( n - 1ul );
            }

            return count;
        }
    }
}
