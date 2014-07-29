// -----------------------------------------------------------------------
// <copyright file="InlineDialogPage_ChooseAnotherCardView.xaml.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

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
using OCTGNDeckConverter.Model;
using OCTGNDeckConverter.ViewModel;

namespace OCTGNDeckConverter.View
{
    /// <summary>
    /// Interaction logic for InlineDialogPage_ChooseAnotherCardView.xaml
    /// </summary>
    public partial class InlineDialogPage_ChooseAnotherCardView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the InlineDialogPage_ChooseAnotherCardView class.
        /// </summary>
        public InlineDialogPage_ChooseAnotherCardView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles the MouseEnter event on the TextBlock with corresponding ConverterCard as DataContext
        /// </summary>
        /// <param name="sender">TextBlock the mouse is now hovering over</param>
        /// <param name="e">The parameter is not used.</param>
        private void ConverterCard_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.DataContext is InlineDialogPage_ChooseAnotherCardVM)
            {
                InlineDialogPage_ChooseAnotherCardVM inlineDialogPageChooseAnotherCardVM = this.DataContext as InlineDialogPage_ChooseAnotherCardVM;
                TextBlock textBlock = sender as TextBlock;
                inlineDialogPageChooseAnotherCardVM.MouseOverConverterCard = textBlock.DataContext as ConverterCard;
            }
        }

        /// <summary>
        /// Handles the MouseLeave event on the TextBlock with corresponding ConverterCard as DataContext
        /// </summary>
        /// <param name="sender">TextBlock the mouse is no longer hovering over</param>
        /// <param name="e">The parameter is not used.</param>
        private void ConverterCard_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.DataContext is InlineDialogPage_ChooseAnotherCardVM)
            {
                InlineDialogPage_ChooseAnotherCardVM inlineDialogPageChooseAnotherCardVM = this.DataContext as InlineDialogPage_ChooseAnotherCardVM;
                inlineDialogPageChooseAnotherCardVM.MouseOverConverterCard = null;
            }
        }

        /// <summary>
        /// Handles the SelectedItemChanged event on the Choose Another Card ListBox
        /// </summary>
        /// <param name="sender">The ListBox firing the event</param>
        /// <param name="e">The parameter is not used.</param>
        private void cardsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.DataContext is InlineDialogPage_ChooseAnotherCardVM)
            {
                InlineDialogPage_ChooseAnotherCardVM inlineDialogPageChooseAnotherCardVM = 
                    this.DataContext as InlineDialogPage_ChooseAnotherCardVM;
                ListBox listBox = sender as ListBox;

                if (listBox.SelectedItem is ConverterCard)
                {
                    inlineDialogPageChooseAnotherCardVM.SelectedConverterCard = listBox.SelectedItem as ConverterCard;
                }
            }
        }
    }
}
