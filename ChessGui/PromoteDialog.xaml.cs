using ChessEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChessGui
{
    /// <summary>
    /// Interaction logic for PromoteDialog.xaml
    /// </summary>
    public partial class PromoteDialog : Window
    {
        public int chosen_piece_index;
        private PlayerColor cur_player;

        public PromoteDialog( PlayerColor cur_player )
        {
            InitializeComponent();

            chosen_piece_index = -1;
            this.cur_player = cur_player;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)rb_queen.IsChecked)
            {
                chosen_piece_index = ( cur_player == PlayerColor.White ) ? Board.INDEX_W_QUEENS : Board.INDEX_B_QUEENS;
            }
            else if ((bool)rb_knight.IsChecked)
            {
                chosen_piece_index = (cur_player == PlayerColor.White) ? Board.INDEX_W_KNIGHTS : Board.INDEX_B_KNIGHTS;
            }
            else if ((bool)rb_rook.IsChecked)
            {
                chosen_piece_index = (cur_player == PlayerColor.White) ? Board.INDEX_W_ROOKS : Board.INDEX_B_ROOKS;
            }
            else if((bool)rb_bishop.IsChecked)
            {
                chosen_piece_index = (cur_player == PlayerColor.White) ? Board.INDEX_W_BISHOPS : Board.INDEX_B_BISHOPS;
            }

            this.Close();
        }
    }
}
