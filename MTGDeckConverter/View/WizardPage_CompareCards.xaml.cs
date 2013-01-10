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
using MTGDeckConverter.Model;

namespace MTGDeckConverter.View
{
    /// <summary>
    /// Interaction logic for WizardPage_CompareCards.xaml
    /// </summary>
    public partial class WizardPage_CompareCards : UserControl
    {
        public WizardPage_CompareCards()
        {
            InitializeComponent();
        }

        private void ConverterDataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            DataGridRow dataGridRow = (sender as DataGridRow);
            if (dataGridRow != null)
            {
                ConverterMapping converterMapping = dataGridRow.DataContext as ConverterMapping;
                this.MouseOverConverterCard
                (
                    converterMapping != null ?
                        converterMapping.SelectedOCTGNCard :
                        null
                );
            }
        }

        private void ConverterDataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            DataGridRow dataGridRow = (sender as DataGridRow);
            if (dataGridRow != null)
            {
                this.MouseOverConverterCard(null);
            }
        }

        private void ComboBox_MouseMove(object sender, MouseEventArgs e)
        {
            var container = (sender as ComboBox).ContainerFromElement((DependencyObject)e.OriginalSource);
            if (container != null)
            {
                ComboBoxItem comboBoxItem = (container as ComboBoxItem);
                if (comboBoxItem != null)
                {
                    this.MouseOverConverterCard(comboBoxItem.Content as ConverterCard);
                }
            }
        }

        private void MouseOverConverterCard(ConverterCard converterCard)
        {
            ViewModel.WizardPage_CompareCards dc = this.DataContext as ViewModel.WizardPage_CompareCards;
            if (dc != null)
            {
                dc.MouseOverConverterCard = converterCard;
            }
        }
    }
}
