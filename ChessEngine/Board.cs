using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    // A board is represented as an unsigned long in the following way:
    // 
    //     A    B    C    D    E    F    G    H
    //   +----+----+----+----+----+----+----+----+
    // 8 | 56 | 57 | 58 | 59 | 60 | 61 | 62 | 63 |  8th rank
    //   +----+----+----+----+----+----+----+----+
    // 7 | 48 | 49 | 50 | 51 | 52 | 53 | 54 | 55 |  7th rank
    //   +----+----+----+----+----+----+----+----+
    // 6 | 40 | 41 | 42 | 43 | 44 | 45 | 46 | 47 |  6th rank
    //   +----+----+----+----+----+----+----+----+
    // 5 | 32 | 33 | 34 | 35 | 36 | 37 | 38 | 39 |  5th rank
    //   +----+----+----+----+----+----+----+----+
    // 4 | 24 | 25 | 26 | 27 | 28 | 29 | 30 | 31 |  4th rank
    //   +----+----+----+----+----+----+----+----+
    // 3 | 16 | 17 | 18 | 19 | 20 | 21 | 22 | 23 |  3rd rank
    //   +----+----+----+----+----+----+----+----+
    // 2 |  8 |  9 | 10 | 11 | 12 | 13 | 14 | 15 |  2nd rank
    //   +----+----+----+----+----+----+----+----+
    // 1 |  0 |  1 |  2 |  3 |  4 |  5 |  6 |  7 |  1st rank
    //   +----+----+----+----+----+----+----+----+
    //     A    B    C    D    E    F    G    H - file(s)
    //
    // White is on the bottom, black on top
    // https://chessprogramming.wikispaces.com/Bitboards
    //

    public class Board
    {
        public const int    INDEX_W_PAWNS    = 0;
        public const int    INDEX_W_ROOKS    = 1;
        public const int    INDEX_W_KNIGHTS  = 2;
        public const int    INDEX_W_BISHOPS  = 3;
        public const int    INDEX_W_QUEENS   = 4;
        public const int    INDEX_W_KING     = 5;
        public const int    INDEX_B_PAWNS    = 6;
        public const int    INDEX_B_ROOKS    = 7;
        public const int    INDEX_B_KNIGHTS  = 8;
        public const int    INDEX_B_BISHOPS  = 9;
        public const int    INDEX_B_QUEENS   = 10;
        public const int    INDEX_B_KING     = 11;
        public const int    INDEX_COUNT      = 12;

        public const uint   CASTLE_BIT_W_EAST = 1;
        public const uint   CASTLE_BIT_W_WEST = 2;
        public const uint   CASTLE_BIT_B_EAST = 4;
        public const uint   CASTLE_BIT_B_WEST = 8;

        public ulong[]      pieces;
        public ulong        enPassant;
        public uint         castle;

        internal Board()
        {
            pieces = new ulong[ INDEX_COUNT ];
        }

        /// <summary>
        /// Sets the board up for the start of a new game
        /// </summary>
        internal void Reset()
        {
            pieces[ INDEX_W_PAWNS   ] = 0x000000000000FF00;
            pieces[ INDEX_W_ROOKS   ] = 0x0000000000000081;
            pieces[ INDEX_W_KNIGHTS ] = 0x0000000000000042;
            pieces[ INDEX_W_BISHOPS ] = 0x0000000000000024;
            pieces[ INDEX_W_QUEENS  ] = 0x0000000000000008;
            pieces[ INDEX_W_KING    ] = 0x0000000000000010;

            pieces[ INDEX_B_PAWNS   ] = 0x00FF000000000000;
            pieces[ INDEX_B_ROOKS   ] = 0x8100000000000000;
            pieces[ INDEX_B_KNIGHTS ] = 0x4200000000000000;
            pieces[ INDEX_B_BISHOPS ] = 0x2400000000000000;
            pieces[ INDEX_B_QUEENS  ] = 0x0800000000000000;
            pieces[ INDEX_B_KING    ] = 0x1000000000000000;

            enPassant = 0;

            castle = CASTLE_BIT_W_EAST | CASTLE_BIT_W_WEST | CASTLE_BIT_B_EAST | CASTLE_BIT_B_WEST;
        }

        /// <summary>
        /// Removes all pieces from the specified location
        /// </summary>
        /// <param name="piece"></param>
        internal void RemovePiece( ulong piece )
        {
            for( int i = 0; i < INDEX_COUNT; i++ )
            {
                pieces[ i ] &= ~piece;
            }
        }

        /// <summary>
        /// Is this space attacked by this color?
        /// </summary>
        /// <param name="by"></param>
        /// <param name="space"></param>
        /// <returns></returns>
        internal bool IsAttacked( PlayerColor by, ulong space )
        {
            ulong attacks = 0ul;
            ulong empty = EmptySpaces();

            if( by == PlayerColor.White )
            {
                attacks |= GetAttacks_WhitePawns(pieces[INDEX_W_PAWNS]);

                ulong rooks = pieces[INDEX_W_ROOKS];
                while (rooks != 0)
                {
                    ulong cur_rook = rooks & (ulong)-(long)rooks;

                    attacks |= GetAttacks_Rook(cur_rook, empty);

                    rooks &= ~cur_rook;
                }

                attacks |= GetAttacks_Knight(pieces[INDEX_W_KNIGHTS]);

                ulong bishop = pieces[INDEX_W_BISHOPS];
                while (bishop != 0)
                {
                    ulong cur_bishop = bishop & (ulong)-(long)bishop;

                    attacks |= GetAttacks_Bishop(cur_bishop, empty);

                    bishop &= ~cur_bishop;
                }

                ulong queens = pieces[INDEX_W_QUEENS];
                while (queens != 0)
                {
                    ulong cur_queen = queens & (ulong)-(long)queens;

                    attacks |= GetAttacks_Bishop(cur_queen, empty);
                    attacks |= GetAttacks_Rook(cur_queen, empty);

                    queens &= ~cur_queen;
                }

                attacks |= GetAttacks_King(pieces[INDEX_W_KING]);
            }
            else
            {
                attacks |= GetAttacks_BlackPawns(pieces[INDEX_B_PAWNS]);

                ulong rooks = pieces[INDEX_B_ROOKS];
                while (rooks != 0)
                {
                    ulong cur_rook = rooks & (ulong)-(long)rooks;

                    attacks |= GetAttacks_Rook(cur_rook, empty);

                    rooks &= ~cur_rook;
                }

                attacks |= GetAttacks_Knight(pieces[INDEX_B_KNIGHTS]);

                ulong bishop = pieces[INDEX_B_BISHOPS];
                while (bishop != 0)
                {
                    ulong cur_bishop = bishop & (ulong)-(long)bishop;

                    attacks |= GetAttacks_Bishop(cur_bishop, empty);

                    bishop &= ~cur_bishop;
                }

                ulong queens = pieces[INDEX_B_QUEENS];
                while (queens != 0)
                {
                    ulong cur_queen = queens & (ulong)-(long)queens;

                    attacks |= GetAttacks_Bishop(cur_queen, empty);
                    attacks |= GetAttacks_Rook(cur_queen, empty);

                    queens &= ~cur_queen;
                }

                attacks |= GetAttacks_King(pieces[INDEX_B_KING]);
            }

            if ((attacks & space) != 0) return true;

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    #region board masks

        private ulong WhitePieces()
        {
            return( pieces[ INDEX_W_PAWNS   ] 
                  | pieces[ INDEX_W_ROOKS   ]
                  | pieces[ INDEX_W_KNIGHTS ]
                  | pieces[ INDEX_W_BISHOPS ]
                  | pieces[ INDEX_W_QUEENS  ]
                  | pieces[ INDEX_W_KING    ] ); 
        }

        private ulong BlackPieces()
        {
            return( pieces[ INDEX_B_PAWNS   ] 
                  | pieces[ INDEX_B_ROOKS   ]
                  | pieces[ INDEX_B_KNIGHTS ]
                  | pieces[ INDEX_B_BISHOPS ]
                  | pieces[ INDEX_B_QUEENS  ]
                  | pieces[ INDEX_B_KING    ] );
        }

        private ulong AllPieces()
        {
            return( WhitePieces() | BlackPieces() );
        }

        private ulong EmptySpaces()
        {
            return( ~AllPieces() );
        }

    #endregion

    #region get moves

        /// <summary>
        /// Gets all possible moves for the current player
        /// Note: does not check if move is legal
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        internal List<Move> GetMoves(PlayerColor player)
        {
            List<Move> moves = new List<Move>();

            ulong empty = EmptySpaces();
            ulong enemy;

            switch (player)
            {
                case PlayerColor.White:
                    enemy = BlackPieces();
                    moves.AddRange(GetMoves_W_Pawns(enPassant, pieces[INDEX_W_PAWNS], enemy, empty));
                    moves.AddRange(GetMoves_Rook(INDEX_W_ROOKS, enemy, empty));
                    moves.AddRange(GetMoves_Knight(INDEX_W_KNIGHTS, enemy, empty));
                    moves.AddRange(GetMoves_Bishop(INDEX_W_BISHOPS, enemy, empty));
                    moves.AddRange(GetMoves_Rook(INDEX_W_QUEENS, enemy, empty));
                    moves.AddRange(GetMoves_Bishop(INDEX_W_QUEENS, enemy, empty));
                    moves.AddRange(GetMoves_W_King(enemy, empty));
                    break;

                case PlayerColor.Black:
                    enemy = WhitePieces();
                    moves.AddRange(GetMoves_B_Pawns(enPassant, pieces[INDEX_B_PAWNS], enemy, empty));
                    moves.AddRange(GetMoves_Rook(INDEX_B_ROOKS, enemy, empty));
                    moves.AddRange(GetMoves_Knight(INDEX_B_KNIGHTS, enemy, empty));
                    moves.AddRange(GetMoves_Bishop(INDEX_B_BISHOPS, enemy, empty));
                    moves.AddRange(GetMoves_Rook(INDEX_B_QUEENS, enemy, empty));
                    moves.AddRange(GetMoves_Bishop(INDEX_B_QUEENS, enemy, empty));
                    moves.AddRange(GetMoves_B_King(enemy, empty));
                    break;

                default:
                    return null;
            }

            return moves;
        }

        private static List<Move> GetMoves_W_Pawns( ulong enPassant, ulong w_pawns, ulong black, ulong empty )
        {
            List<Move> moves = new List<Move>();

            while( w_pawns != 0 )
            {
                ulong cur = w_pawns & (ulong)-(long)w_pawns;

                //check single push
                ulong single_push = Shift.nort( cur ) & empty;
                if( single_push != 0 )
                {
                    if( ( single_push & Shift.rank8 ) != 0 )
                    {
                        //promotion moves
                        moves.Add(new Move(MoveType.SinglePush, INDEX_W_PAWNS, cur, single_push, INDEX_W_QUEENS));
                        moves.Add(new Move(MoveType.SinglePush, INDEX_W_PAWNS, cur, single_push, INDEX_W_KNIGHTS));
                        moves.Add(new Move(MoveType.SinglePush, INDEX_W_PAWNS, cur, single_push, INDEX_W_ROOKS));
                        moves.Add(new Move(MoveType.SinglePush, INDEX_W_PAWNS, cur, single_push, INDEX_W_BISHOPS));
                    }
                    else
                    {
                        moves.Add(new Move(MoveType.SinglePush, INDEX_W_PAWNS, cur, single_push));

                        //check double push
                        ulong double_push = Shift.nort(single_push) & empty & Shift.rank4;
                        if (double_push != 0)
                        {
                            moves.Add(new Move(MoveType.DoublePush, INDEX_W_PAWNS, cur, double_push));
                        }
                    }
                }

                //check north east
                ulong capture_noEa = Shift.noEa( cur ) & black & Shift.notAfile;
                if( capture_noEa != 0 )
                {
                    if ((capture_noEa & Shift.rank8) != 0)
                    {
                        //promotion moves
                        moves.Add(new Move(MoveType.Capture, INDEX_W_PAWNS, cur, capture_noEa, INDEX_W_QUEENS));
                        moves.Add(new Move(MoveType.Capture, INDEX_W_PAWNS, cur, capture_noEa, INDEX_W_KNIGHTS));
                        moves.Add(new Move(MoveType.Capture, INDEX_W_PAWNS, cur, capture_noEa, INDEX_W_ROOKS));
                        moves.Add(new Move(MoveType.Capture, INDEX_W_PAWNS, cur, capture_noEa, INDEX_W_BISHOPS));
                    }
                    else
                    {
                        moves.Add( new Move( MoveType.Capture, INDEX_W_PAWNS, cur, capture_noEa ) );
                    }
                }

                //check north west
                ulong capture_noWe = Shift.noWe( cur ) & black & Shift.notHfile;
                if( capture_noWe != 0 )
                {
                    if ((capture_noWe & Shift.rank8) != 0)
                    {
                        //promotion moves
                        moves.Add(new Move(MoveType.Capture, INDEX_W_PAWNS, cur, capture_noWe, INDEX_W_QUEENS));
                        moves.Add(new Move(MoveType.Capture, INDEX_W_PAWNS, cur, capture_noWe, INDEX_W_KNIGHTS));
                        moves.Add(new Move(MoveType.Capture, INDEX_W_PAWNS, cur, capture_noWe, INDEX_W_ROOKS));
                        moves.Add(new Move(MoveType.Capture, INDEX_W_PAWNS, cur, capture_noWe, INDEX_W_BISHOPS));
                    }
                    else
                    {
                        moves.Add( new Move( MoveType.Capture, INDEX_W_PAWNS, cur, capture_noWe ) );
                    }
                }

                //en passante
                if( ( cur & Shift.rank5 ) != 0 )
                {
                    //north east
                    if( ( Shift.east( cur ) & enPassant & Shift.notAfile ) != 0 )
                    {
                        moves.Add( new Move( MoveType.EnPassant, INDEX_W_PAWNS, cur, Shift.noEa( cur ) ) );
                    }

                    //north west
                    if( ( Shift.west( cur ) & enPassant & Shift.notHfile ) != 0 )
                    {
                        moves.Add( new Move( MoveType.EnPassant, INDEX_W_PAWNS, cur, Shift.noWe( cur ) ) );
                    }
                }

                w_pawns &= ~cur;
            }

            return moves;
        }

        private static List<Move> GetMoves_B_Pawns( ulong enPassant, ulong b_pawns, ulong white, ulong empty )
        {
            List<Move> moves = new List<Move>();

            while( b_pawns != 0 )
            {
                ulong cur = b_pawns & (ulong)-(long)b_pawns;

                //check single push
                ulong single_push = Shift.sout( cur ) & empty;
                if( single_push != 0 )
                {
                    if ((single_push & Shift.rank1) != 0)
                    {
                        //promotion moves
                        moves.Add(new Move(MoveType.SinglePush, INDEX_B_PAWNS, cur, single_push, INDEX_B_QUEENS));
                        moves.Add(new Move(MoveType.SinglePush, INDEX_B_PAWNS, cur, single_push, INDEX_B_KNIGHTS));
                        moves.Add(new Move(MoveType.SinglePush, INDEX_B_PAWNS, cur, single_push, INDEX_B_ROOKS));
                        moves.Add(new Move(MoveType.SinglePush, INDEX_B_PAWNS, cur, single_push, INDEX_B_BISHOPS));
                    }
                    else
                    {
                        moves.Add( new Move( MoveType.SinglePush, INDEX_B_PAWNS, cur, single_push ) );

                        //check double push
                        ulong double_push = Shift.sout( single_push ) & empty & Shift.rank5;
                        if( double_push != 0 )
                        {
                            moves.Add( new Move( MoveType.DoublePush, INDEX_B_PAWNS, cur, double_push ) );
                        }
                    }
                }

                //check south east
                ulong capture_soEa = Shift.soEa( cur ) & white & Shift.notAfile;
                if( capture_soEa != 0 )
                {
                    if ((capture_soEa & Shift.rank1) != 0)
                    {
                        //promotion moves
                        moves.Add(new Move(MoveType.Capture, INDEX_B_PAWNS, cur, capture_soEa, INDEX_B_QUEENS));
                        moves.Add(new Move(MoveType.Capture, INDEX_B_PAWNS, cur, capture_soEa, INDEX_B_KNIGHTS));
                        moves.Add(new Move(MoveType.Capture, INDEX_B_PAWNS, cur, capture_soEa, INDEX_B_ROOKS));
                        moves.Add(new Move(MoveType.Capture, INDEX_B_PAWNS, cur, capture_soEa, INDEX_B_BISHOPS));
                    }
                    else
                    {
                        moves.Add( new Move( MoveType.Capture, INDEX_B_PAWNS, cur, capture_soEa ) );
                    }
                }

                //check north west
                ulong capture_soWe = Shift.soWe( cur ) & white & Shift.notHfile;
                if( capture_soWe != 0 )
                {
                    if ((capture_soWe & Shift.rank1) != 0)
                    {
                        //promotion moves
                        moves.Add(new Move(MoveType.Capture, INDEX_B_PAWNS, cur, capture_soWe, INDEX_B_QUEENS));
                        moves.Add(new Move(MoveType.Capture, INDEX_B_PAWNS, cur, capture_soWe, INDEX_B_KNIGHTS));
                        moves.Add(new Move(MoveType.Capture, INDEX_B_PAWNS, cur, capture_soWe, INDEX_B_ROOKS));
                        moves.Add(new Move(MoveType.Capture, INDEX_B_PAWNS, cur, capture_soWe, INDEX_B_BISHOPS));
                    }
                    else
                    {
                        moves.Add( new Move( MoveType.Capture, INDEX_B_PAWNS, cur, capture_soWe ) );
                    }
                }

                //en passante
                if( ( cur & Shift.rank4 ) != 0 )
                {
                    //north east
                    if( ( Shift.east( cur ) & enPassant & Shift.notAfile ) != 0 )
                    {
                        moves.Add( new Move( MoveType.EnPassant, INDEX_B_PAWNS, cur, Shift.soEa( cur ) ) );
                    }

                    //north west
                    if( ( Shift.west( cur ) & enPassant & Shift.notHfile ) != 0 )
                    {
                        moves.Add( new Move( MoveType.EnPassant, INDEX_B_PAWNS, cur, Shift.soWe( cur ) ) );
                    }
                }

                b_pawns &= ~cur;
            }

            return moves;
        }

        private List<Move> GetMoves_Rook( int piece_index, ulong enemy, ulong empty )
        {
            List<Move> moves = new List<Move>();
            ulong p = pieces[ piece_index ];

            while( p != 0 )
            {
                ulong cur_piece = p & (ulong)-(long)p;
                ulong attacks = GetAttacks_Rook( cur_piece, empty );

                while( attacks != 0 )
                {
                    ulong cur_attack = attacks & (ulong)-(long)attacks;

                    if( ( cur_attack & empty ) != 0 )
                    {
                        moves.Add( new Move( MoveType.Slide, piece_index, cur_piece, cur_attack ) );
                    }
                    else
                    {
                        if( ( cur_attack & enemy ) != 0 )
                        {
                            moves.Add( new Move( MoveType.Capture, piece_index, cur_piece, cur_attack ) );
                        }
                    }

                    attacks &= ~cur_attack;
                }

                p &= ~cur_piece;
            }

            return moves;
        }

        private List<Move> GetMoves_Bishop( int piece_index, ulong enemy, ulong empty )
        {
            List<Move> moves = new List<Move>();
            ulong p = pieces[piece_index];

            while (p != 0)
            {
                ulong cur_piece = p & (ulong)-(long)p;
                ulong attacks = GetAttacks_Bishop(cur_piece, empty);

                while( attacks != 0 )
                {
                    ulong cur_attack = attacks & (ulong)-(long)attacks;

                    if( ( cur_attack & empty ) != 0 )
                    {
                        moves.Add( new Move( MoveType.Slide, piece_index, cur_piece, cur_attack ) );
                    }
                    else
                    {
                        if( ( cur_attack & enemy ) != 0 )
                        {
                            moves.Add( new Move( MoveType.Capture, piece_index, cur_piece, cur_attack ) );
                        }
                    }

                    attacks &= ~cur_attack;
                }

                p &= ~cur_piece;
            }

            return moves;
        }

        private List<Move> GetMoves_Knight( int piece_index, ulong enemy, ulong empty )
        {
            List<Move> moves = new List<Move>();

            ulong p = pieces[ piece_index ];

            while( p != 0 )
            {
                ulong cur_piece = p & (ulong)-(long)p;
                ulong attacks = GetAttacks_Knight( cur_piece );

                while( attacks != 0 )
                {
                    ulong cur_attack = attacks & (ulong)-(long)attacks;

                    if( ( cur_attack & empty ) != 0 )
                    {
                        moves.Add( new Move( MoveType.Slide, piece_index, cur_piece, cur_attack ) );
                    }
                    else
                    {
                        if( ( cur_attack & enemy ) != 0 )
                        {
                            moves.Add( new Move( MoveType.Capture, piece_index, cur_piece, cur_attack ) );
                        }
                    }

                    attacks &= ~cur_attack;
                }

                p &= ~cur_piece;
            }

            return moves;
        }

        private List<Move> GetMoves_W_King( ulong enemy, ulong empty )
        {
            List<Move> moves = new List<Move>();

            ulong p = pieces[ INDEX_W_KING ];
            ulong attacks = GetAttacks_King( p );

            //normal moves
            while( attacks != 0 )
            {
                ulong cur_attack = attacks & (ulong)-(long)attacks;

                if( ( cur_attack & empty ) != 0 )
                {
                    moves.Add( new Move( MoveType.Slide, INDEX_W_KING, p, cur_attack ) );
                }
                else
                {
                    if( ( cur_attack & enemy ) != 0 )
                    {
                        moves.Add( new Move( MoveType.Capture, INDEX_W_KING, p, cur_attack ) );
                    }
                }

                attacks &= ~cur_attack;
            }

            //castle east
            if( ( castle & CASTLE_BIT_W_EAST ) != 0 )
            {
                if( ( empty & 0x0000000000000060 ) == 0x0000000000000060 )
                {
                    if( !IsAttacked( PlayerColor.Black, 0x0000000000000070 ) )
                    {
                        moves.Add( new Move( MoveType.CASTLE_W_EAST, INDEX_W_KING, p, 0x0000000000000040 ) );
                    }
                }
            }

            //castle west
            if( ( castle & CASTLE_BIT_W_WEST ) != 0 )
            {
                if( ( empty & 0x000000000000000E ) == 0x000000000000000E )
                {
                    if( !IsAttacked( PlayerColor.Black, 0x000000000000001E ) )
                    {
                        moves.Add( new Move( MoveType.CASTLE_W_WEST, INDEX_W_KING, p, 0x0000000000000004 ) );
                    }
                }
            }

            return moves;
        }

        private List<Move> GetMoves_B_King( ulong enemy, ulong empty )
        {
            List<Move> moves = new List<Move>();

            ulong p = pieces[ INDEX_B_KING ];
            ulong attacks = GetAttacks_King( p );

            //normal moves
            while( attacks != 0 )
            {
                ulong cur_attack = attacks & (ulong)-(long)attacks;

                if( ( cur_attack & empty ) != 0 )
                {
                    moves.Add( new Move( MoveType.Slide, INDEX_B_KING, p, cur_attack ) );
                }
                else
                {
                    if( ( cur_attack & enemy ) != 0 )
                    {
                        moves.Add( new Move( MoveType.Capture, INDEX_B_KING, p, cur_attack ) );
                    }
                }

                attacks &= ~cur_attack;
            }

            //castle east
            if( ( castle & CASTLE_BIT_B_EAST ) != 0 )
            {
                if( ( empty & 0x6000000000000000 ) == 0x6000000000000000 )
                {
                    if( !IsAttacked( PlayerColor.White, 0x7000000000000000 ) )
                    {
                        moves.Add( new Move( MoveType.CASTLE_B_EAST, INDEX_B_KING, p, 0x4000000000000000 ) );
                    }
                }
            }

            //castle west
            if( ( castle & CASTLE_BIT_B_WEST ) != 0 )
            {
                if( ( empty & 0x0E00000000000000 ) == 0x0E00000000000000 )
                {
                    if( !IsAttacked( PlayerColor.White, 0x1E00000000000000 ) )
                    {
                        moves.Add( new Move( MoveType.CASTLE_B_WEST, INDEX_B_KING, p, 0x0400000000000000 ) );
                    }
                }
            }

            return moves;
        }

    #endregion

    #region get attacks

        private static ulong GetAttacks_WhitePawns(ulong pawns)
        {
            ulong attacks = 0L;

            attacks |= Shift.noEa(pawns) & Shift.notAfile;
            attacks |= Shift.noWe(pawns) & Shift.notHfile;

            return attacks;
        }

        private static ulong GetAttacks_BlackPawns(ulong pawns)
        {
            ulong attacks = 0L;

            attacks |= Shift.soEa(pawns) & Shift.notAfile;
            attacks |= Shift.soWe(pawns) & Shift.notHfile;

            return attacks;
        }

        private static ulong GetAttacks_Rook( ulong piece, ulong empty )
        {
            ulong attacks = 0ul;
            ulong slide;

            slide = piece;
            while (slide != 0)
            {
                slide = Shift.nort(slide);
                attacks |= slide;

                if ((slide & empty) == 0) break;
            }

            slide = piece;
            while (slide != 0)
            {
                slide = Shift.sout(slide);
                attacks |= slide;

                if ((slide & empty) == 0) break;
            }

            slide = piece;
            while (slide != 0)
            {
                slide = Shift.east(slide) & Shift.notAfile;
                attacks |= slide;

                if ((slide & empty) == 0) break;
            }

            slide = piece;
            while (slide != 0)
            {
                slide = Shift.west(slide) & Shift.notHfile;
                attacks |= slide;

                if ((slide & empty) == 0) break;
            }

            return attacks;
        }

        private static ulong GetAttacks_Bishop( ulong piece, ulong empty )
        {
            ulong attacks = 0ul;
            ulong slide;

            slide = piece;
            while (slide != 0)
            {
                slide = Shift.noEa(slide) & Shift.notAfile;
                attacks |= slide;

                if ((slide & empty) == 0) break;
            }

            slide = piece;
            while (slide != 0)
            {
                slide = Shift.soEa(slide) & Shift.notAfile;
                attacks |= slide;

                if ((slide & empty) == 0) break;
            }

            slide = piece;
            while (slide != 0)
            {
                slide = Shift.noWe(slide) & Shift.notHfile;
                attacks |= slide;

                if ((slide & empty) == 0) break;
            }

            slide = piece;
            while (slide != 0)
            {
                slide = Shift.soWe(slide) & Shift.notHfile;
                attacks |= slide;

                if ((slide & empty) == 0) break;
            }

            return attacks;
        }

        private static ulong GetAttacks_Knight( ulong piece )
        {
            ulong attacks = 0ul;

            attacks |= Shift.noNoEa(piece) & Shift.notAfile;
            attacks |= Shift.soSoEa(piece) & Shift.notAfile;
            attacks |= Shift.noNoWe(piece) & Shift.notHfile;
            attacks |= Shift.soSoWe(piece) & Shift.notHfile;
            attacks |= Shift.noEaEa(piece) & Shift.notABfile;
            attacks |= Shift.soEaEa(piece) & Shift.notABfile;
            attacks |= Shift.noWeWe(piece) & Shift.notGHfile;
            attacks |= Shift.soWeWe(piece) & Shift.notGHfile;

            return attacks;
        }

        private static ulong GetAttacks_King( ulong piece )
        {
            ulong attacks = 0ul;

            attacks |= Shift.nort(piece);
            attacks |= Shift.sout(piece);
            attacks |= Shift.east(piece) & Shift.notAfile;
            attacks |= Shift.west(piece) & Shift.notHfile;
            attacks |= Shift.noEa(piece) & Shift.notAfile;
            attacks |= Shift.soEa(piece) & Shift.notAfile;
            attacks |= Shift.soWe(piece) & Shift.notHfile;
            attacks |= Shift.noWe(piece) & Shift.notHfile;

            return attacks;
        }

    #endregion
    }
}
