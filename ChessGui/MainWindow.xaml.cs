using ChessEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChessGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Brush WHITE_COLOR = Brushes.Tan;
        Brush BLACK_COLOR = Brushes.SaddleBrown;
        Brush HIGHLIGHT_COLOR = Brushes.DarkGreen;

        BitmapImage WHITE_PAWN;
        BitmapImage WHITE_ROOK;
        BitmapImage WHITE_KNIGHT;
        BitmapImage WHITE_BISHOP;
        BitmapImage WHITE_QUEEN;
        BitmapImage WHITE_KING;
        BitmapImage BLACK_PAWN;
        BitmapImage BLACK_ROOK;
        BitmapImage BLACK_KNIGHT;
        BitmapImage BLACK_BISHOP;
        BitmapImage BLACK_QUEEN;
        BitmapImage BLACK_KING;

        BoardController board_controller;
        Rectangle[]     background;
        Rectangle[]     highlights;
        Image[]         foreground;

        PlayerColor     cur_player;
        List<Move>      possible_moves;

        UIElement       selected_piece;
        Image           selected_image;

        public MainWindow()
        {
            InitializeComponent();

            //initialize
            board_controller    = new BoardController();    //handles board interaction
            possible_moves      = new List<Move>();         //list of posssible moves
            selected_piece      = null;                     //nothing is selected initially
            selected_image      = new Image();              //ghost image of moving piece
            background          = new Rectangle[64];        //background color of the squares on a chessboard
            highlights          = new Rectangle[64];        //highlighed squares showing possible moves
            foreground          = new Image[64];            //images of the pieces on the board

            //initialize white images
            WHITE_PAWN = new BitmapImage();
            WHITE_PAWN.BeginInit();
            WHITE_PAWN.UriSource = new Uri(@"Resources/ClassicWhitePawn.png", UriKind.Relative);
            WHITE_PAWN.DecodePixelHeight = 100;
            WHITE_PAWN.DecodePixelWidth = 80;
            WHITE_PAWN.EndInit();

            WHITE_ROOK = new BitmapImage();
            WHITE_ROOK.BeginInit();
            WHITE_ROOK.UriSource = new Uri(@"Resources/ClassicWhiteRook.png", UriKind.Relative);
            WHITE_ROOK.DecodePixelHeight = 100;
            WHITE_ROOK.DecodePixelWidth = 95;
            WHITE_ROOK.EndInit();

            WHITE_KNIGHT = new BitmapImage();
            WHITE_KNIGHT.BeginInit();
            WHITE_KNIGHT.UriSource = new Uri(@"Resources/ClassicWhiteKnight.png", UriKind.Relative);
            WHITE_KNIGHT.DecodePixelHeight = 100;
            WHITE_KNIGHT.DecodePixelWidth = 95;
            WHITE_KNIGHT.EndInit();

            WHITE_BISHOP = new BitmapImage();
            WHITE_BISHOP.BeginInit();
            WHITE_BISHOP.UriSource = new Uri(@"Resources/ClassicWhiteBishop.png", UriKind.Relative);
            WHITE_BISHOP.DecodePixelHeight = 100;
            WHITE_BISHOP.DecodePixelWidth = 95;
            WHITE_BISHOP.EndInit();

            WHITE_QUEEN = new BitmapImage();
            WHITE_QUEEN.BeginInit();
            WHITE_QUEEN.UriSource = new Uri(@"Resources/ClassicWhiteQueen.png", UriKind.Relative);
            WHITE_QUEEN.DecodePixelHeight = 85;
            WHITE_QUEEN.DecodePixelWidth = 80;
            WHITE_QUEEN.EndInit();

            WHITE_KING = new BitmapImage();
            WHITE_KING.BeginInit();
            WHITE_KING.UriSource = new Uri(@"Resources/ClassicWhiteKing.png", UriKind.Relative);
            WHITE_KING.DecodePixelHeight = 100;
            WHITE_KING.DecodePixelWidth = 95;
            WHITE_KING.EndInit();

            //initialize black images
            BLACK_PAWN = new BitmapImage();
            BLACK_PAWN.BeginInit();
            BLACK_PAWN.UriSource = new Uri(@"Resources/ClassicBlackPawn.png", UriKind.Relative);
            BLACK_PAWN.DecodePixelHeight = 100;
            BLACK_PAWN.DecodePixelWidth = 80;
            BLACK_PAWN.EndInit();

            BLACK_ROOK = new BitmapImage();
            BLACK_ROOK.BeginInit();
            BLACK_ROOK.UriSource = new Uri(@"Resources/ClassicBlackRook.png", UriKind.Relative);
            BLACK_ROOK.DecodePixelHeight = 100;
            BLACK_ROOK.DecodePixelWidth = 95;
            BLACK_ROOK.EndInit();

            BLACK_KNIGHT = new BitmapImage();
            BLACK_KNIGHT.BeginInit();
            BLACK_KNIGHT.UriSource = new Uri(@"Resources/ClassicBlackKnight.png", UriKind.Relative);
            BLACK_KNIGHT.DecodePixelHeight = 100;
            BLACK_KNIGHT.DecodePixelWidth = 95;
            BLACK_KNIGHT.EndInit();

            BLACK_BISHOP = new BitmapImage();
            BLACK_BISHOP.BeginInit();
            BLACK_BISHOP.UriSource = new Uri(@"Resources/ClassicBlackBishop.png", UriKind.Relative);
            BLACK_BISHOP.DecodePixelHeight = 100;
            BLACK_BISHOP.DecodePixelWidth = 95;
            BLACK_BISHOP.EndInit();

            BLACK_QUEEN = new BitmapImage();
            BLACK_QUEEN.BeginInit();
            BLACK_QUEEN.UriSource = new Uri(@"Resources/ClassicBlackQueen.png", UriKind.Relative);
            BLACK_QUEEN.DecodePixelHeight = 85;
            BLACK_QUEEN.DecodePixelWidth = 90;
            BLACK_QUEEN.EndInit();

            BLACK_KING = new BitmapImage();
            BLACK_KING.BeginInit();
            BLACK_KING.UriSource = new Uri(@"Resources/ClassicBlackKing.png", UriKind.Relative);
            BLACK_KING.DecodePixelHeight = 100;
            BLACK_KING.DecodePixelWidth = 95;
            BLACK_KING.EndInit();

            //setup selected image
            selected_image.MouseDown    += MainWindow_MouseDown;
            selected_image.MouseUp      += MainWindow_MouseUp;
            selected_image.Opacity      = .5;
            selected_image.Stretch      = Stretch.None;
            selected_image.Source       = null;
            selected_image.SetValue( Canvas.ZIndexProperty, 3 );

            grid_board.Children.Add( selected_image );

            //for each square on the board
            bool is_black = true;
            for( int i = 0; i < background.Length; i++ )
            {
                //determine row and column
                int row = 7 - ( i / 8 );
                int col = i % 8;

                //initialize the space
                background[i] = new Rectangle();
                highlights[i] = new Rectangle();
                foreground[i] = new Image();

                if( is_black )
                {
                    background[i].Fill = BLACK_COLOR;
                }
                else
                {
                    background[i].Fill = WHITE_COLOR;
                }
                highlights[i].Opacity = .85d;
                foreground[i].Stretch = Stretch.None;

                //set events for the space
                background[i].MouseDown += MainWindow_MouseDown;
                highlights[i].MouseDown += MainWindow_MouseDown;
                foreground[i].MouseDown += MainWindow_MouseDown;

                background[i].MouseUp += MainWindow_MouseUp;
                highlights[i].MouseUp += MainWindow_MouseUp;
                foreground[i].MouseUp += MainWindow_MouseUp;

                //set z index of each control
                background[i].SetValue( Canvas.ZIndexProperty, 0 );
                highlights[i].SetValue( Canvas.ZIndexProperty, 1 );
                foreground[i].SetValue( Canvas.ZIndexProperty, 2 );

                //set row and column of each control
                Grid.SetRow( background[i], row );
                Grid.SetColumn( background[i], col );

                Grid.SetRow( highlights[i], row );
                Grid.SetColumn( highlights[i], col );

                Grid.SetRow( foreground[i], row );
                Grid.SetColumn( foreground[i], col );

                //add to the board
                grid_board.Children.Add( background[i] );
                grid_board.Children.Add( highlights[i] );
                grid_board.Children.Add( foreground[i] );

                //color remains the same when moving between rows
                if( col != 7 )
                {
                    is_black = !is_black;
                }
            }
        }

        void MainWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            for( int i = 0; i < highlights.Length; i++ )
            {
                highlights[ i ].Fill = null;
            }

            UIElement destination = sender as UIElement;
            if( destination     != null
             && selected_piece  != null )
            {
                //determine to and from spaces in move
                int from_row = 7 - Grid.GetRow( selected_piece );
                int from_col = Grid.GetColumn( selected_piece );

                int to_row = 7 - Grid.GetRow( destination );
                int to_col = Grid.GetColumn( destination );

                ulong from = ( 1ul << from_col ) << ( 8 * from_row );
                ulong to   = ( 1ul << to_col   ) << ( 8 * to_row   );

                //see if the move is valid
                Move move = possible_moves.Find( m => ( m.to == to && m.from == from ) );

                if( move != null )
                {
                    if( move.promote_piece_index != -1 )
                    {
                        PromoteDialog pd = new PromoteDialog(cur_player);
                        pd.ShowDialog();

                        move = possible_moves.Find(  m => ( m.to == to && m.from == from && m.promote_piece_index == pd.chosen_piece_index ) );
                        if( move == null ) return;
                    }

                    board_controller.MakeMove( move );
                    
                    //mdk - uncomment for no ai
                    //if (cur_player == PlayerColor.White) cur_player = PlayerColor.Black;
                    //else if (cur_player == PlayerColor.Black) cur_player = PlayerColor.White;

                    UpdateBoardGrid();

                    //mdk - uncomment for ai
                    board_controller.MakeMove( PlayerColor.Black );
                    UpdateBoardGrid();
                    
                    possible_moves = board_controller.GetMoves( cur_player );
                }


                selected_image.Visibility = System.Windows.Visibility.Hidden;
                selected_image.Source = null;
                selected_piece = null;
            }
        }

        void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //get the selected piece, row and column
            selected_piece  = sender as UIElement;
            int grid_row    = Grid.GetRow(selected_piece);      //row on the display
            int grid_col    = Grid.GetColumn(selected_piece);   //column on the display
            int index_row   = 7 - grid_row;                     //index of row in board arrays
            int index_col   = grid_col;                         //index of column in board arrays

            //determine the space that was clicked
            ulong from = ( 1ul << index_col ) << ( 8 * index_row );

            //show all moves for that space
            List<Move> moves = possible_moves.FindAll( m => m.from == from );
            foreach( Move m in moves )
            {
                int index = 0;
                ulong cur = 1L;

                while( cur != m.to )
                {
                    index++;
                    cur <<= 1;
                }

                highlights[index].Fill = HIGHLIGHT_COLOR;
            }
            
            //move selected image to correct location
            Grid.SetRow(selected_image, grid_row);
            Grid.SetColumn(selected_image, grid_col);

            //show selected image
            selected_image.Source = foreground[(index_row * 8) + index_col].Source;
            selected_image.Visibility = System.Windows.Visibility.Visible;
        }

        private void grid_board_MouseMove(object sender, MouseEventArgs e)
        {
            if( selected_image.Source != null )
            {
                // Get the x and y coordinates of the mouse pointer.
                System.Windows.Point position = e.GetPosition(this);
                double pX = ( position.X - grid_board.Margin.Left ) / 100;
                double pY = ( position.Y - grid_board.Margin.Top  ) / 100;

                //move the selected image
                Grid.SetRow( selected_image, (int)pY );
                Grid.SetColumn( selected_image, (int)pX );
            }
        }

        private void UpdateBoardGrid()
        {
            ulong[] cur_board = board_controller.cur_board.pieces;

            //update each square on the board with the appropriate piece
            ulong cur = 1ul;
            for( int i = 0; i < 64; i++ )
            {
                if( ( cur_board[ Board.INDEX_W_PAWNS ] & cur ) != 0 )
                {
                    foreground[i].Source = WHITE_PAWN;
                }
                else if( ( cur_board[ Board.INDEX_W_ROOKS ] & cur ) != 0 )
                {
                    foreground[i].Source = WHITE_ROOK;
                }
                else if( ( cur_board[ Board.INDEX_W_KNIGHTS ] & cur ) != 0 )
                {
                    foreground[i].Source = WHITE_KNIGHT;
                }
                else if( ( cur_board[ Board.INDEX_W_BISHOPS ] & cur ) != 0 )
                {
                    foreground[i].Source = WHITE_BISHOP;
                }
                else if( ( cur_board[ Board.INDEX_W_QUEENS ] & cur ) != 0 )
                {
                    foreground[i].Source = WHITE_QUEEN;
                }
                else if( ( cur_board[ Board.INDEX_W_KING ] & cur ) != 0 )
                {
                    foreground[i].Source = WHITE_KING;
                }
                else if( ( cur_board[ Board.INDEX_B_PAWNS ] & cur ) != 0 )
                {
                    foreground[i].Source = BLACK_PAWN;
                }
                else if( ( cur_board[ Board.INDEX_B_ROOKS ] & cur ) != 0 )
                {
                    foreground[i].Source = BLACK_ROOK;
                }
                else if( ( cur_board[ Board.INDEX_B_KNIGHTS ] & cur ) != 0 )
                {
                    foreground[i].Source = BLACK_KNIGHT;
                }
                else if( ( cur_board[ Board.INDEX_B_BISHOPS ] & cur ) != 0 )
                {
                    foreground[i].Source = BLACK_BISHOP;
                }
                else if( ( cur_board[ Board.INDEX_B_QUEENS ] & cur ) != 0 )
                {
                    foreground[i].Source = BLACK_QUEEN;
                }
                else if( ( cur_board[ Board.INDEX_B_KING ] & cur ) != 0 )
                {
                    foreground[i].Source = BLACK_KING;
                }
                else
                {
                    foreground[i].Source = null;
                }

                cur <<= 1;   
            }
        }

        private void menu_exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void menu_undo_Click(object sender, RoutedEventArgs e)
        {
            //undo the ai's last move, then the players last move
            board_controller.UndoMove();
            board_controller.UndoMove();

            //reset to that board
            possible_moves = board_controller.GetMoves(cur_player);
            UpdateBoardGrid();

        }

        private void menu_new_game_Click(object sender, RoutedEventArgs e)
        {
            //reset the board
            board_controller.Reset();
            cur_player      = PlayerColor.White;
            possible_moves  = board_controller.GetMoves(cur_player);

            UpdateBoardGrid();
        }
    }
}
