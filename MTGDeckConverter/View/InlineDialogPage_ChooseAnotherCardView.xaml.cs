using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MTGDeckConverter.ViewModel;
using MTGDeckConverter.Model;

namespace MTGDeckConverter.View
{
    /// <summary>
    /// Interaction logic for InlineDialogPage_ChooseAnotherCardView.xaml
    /// </summary>
    public partial class InlineDialogPage_ChooseAnotherCardView : UserControl
    {
        public InlineDialogPage_ChooseAnotherCardView()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.DataContext is InlineDialogPage_ChooseAnotherCardVM)
            {
                InlineDialogPage_ChooseAnotherCardVM dc = this.DataContext as InlineDialogPage_ChooseAnotherCardVM;
                TreeView tv = sender as TreeView;
                dc.SelectedConverterCard = tv.SelectedItem is ConverterCard ?
                    tv.SelectedItem as ConverterCard :
                    null;
            }
        }

        private void ConverterCard_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.DataContext is InlineDialogPage_ChooseAnotherCardVM)
            {
                InlineDialogPage_ChooseAnotherCardVM dc = this.DataContext as InlineDialogPage_ChooseAnotherCardVM;
                TextBlock tb = sender as TextBlock;
                dc.MouseOverConverterCard = tb.DataContext as ConverterCard;
            }
        }

        private void ConverterCard_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.DataContext is InlineDialogPage_ChooseAnotherCardVM)
            {
                InlineDialogPage_ChooseAnotherCardVM dc = this.DataContext as InlineDialogPage_ChooseAnotherCardVM;
                dc.MouseOverConverterCard = null;
            }
        }
    }
}
