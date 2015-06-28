namespace ChessEngine
{
    public enum PlayerColor
    {
        White,
        Black
    }

    internal static class Shift
    {
        internal const ulong notAfile   = 0xFEFEFEFEFEFEFEFEul;
        internal const ulong notHfile   = 0x7F7F7F7F7F7F7F7Ful;
        internal const ulong notABfile  = 0xFCFCFCFCFCFCFCFCul;
        internal const ulong notGHfile  = 0x3F3F3F3F3F3F3F3Ful;

        internal const ulong rank1      = 0x00000000000000FFul;
        internal const ulong rank2      = 0x000000000000FF00ul;
        internal const ulong rank3      = 0x0000000000FF0000ul;
        internal const ulong rank4      = 0x00000000FF000000ul;
        internal const ulong rank5      = 0x000000FF00000000ul;
        internal const ulong rank6      = 0x0000FF0000000000ul;
        internal const ulong rank7      = 0x00FF000000000000ul;
        internal const ulong rank8      = 0xFF00000000000000ul;

        internal static ulong noWe( ulong b ) { return( b << 7 ); }
        internal static ulong nort( ulong b ) { return( b << 8 ); }
        internal static ulong noEa( ulong b ) { return( b << 9 ); }
        internal static ulong east( ulong b ) { return( b << 1 ); }
        internal static ulong soEa( ulong b ) { return( b >> 7 ); }
        internal static ulong sout( ulong b ) { return( b >> 8 ); }
        internal static ulong soWe( ulong b ) { return( b >> 9 ); }
        internal static ulong west( ulong b ) { return( b >> 1 ); }

        internal static ulong noWeWe( ulong b ) { return( b << 6  ); }
        internal static ulong noNoWe( ulong b ) { return( b << 15 ); }
        internal static ulong noNoEa( ulong b ) { return( b << 17 ); }
        internal static ulong noEaEa( ulong b ) { return( b << 10 ); }
        internal static ulong soEaEa( ulong b ) { return( b >> 6  ); }
        internal static ulong soSoEa( ulong b ) { return( b >> 15 ); }
        internal static ulong soSoWe( ulong b ) { return( b >> 17 ); }
        internal static ulong soWeWe( ulong b ) { return( b >> 10 ); }
    }
}
