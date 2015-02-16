using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    //internal static class AI
    public static class AI
    {
        //http://chessprogramming.wikispaces.com/Alpha-Beta
        internal static Move DetermineMove( Board cur_board, PlayerColor player )
        {
            throw new NotImplementedException();
        }

        //https://chessprogramming.wikispaces.com/Evaluation
        private static int EvaluateBoard( Board board )
        {
            throw new NotImplementedException();   
        }

        //sparse ones - this will probably be fastest since most of the bits are 0
        //http://gurmeet.net/puzzles/fast-bit-counting-routines/
        //http://stackoverflow.com/questions/109023/how-to-count-the-number-of-set-bits-in-a-32-bit-integer
        public static int BitCount( ulong n )
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
