// -----------------------------------------------------------------------
// <copyright file="WizardPage_CompareCards.xaml.cs" company="jlkatz">
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
using MTGDeckConverter.Model;

namespace MTGDeckConverter.View
{
    /// <summary>
    /// Interaction logic for WizardPage_CompareCards.xaml
    /// </summary>
    public partial class WizardPage_CompareCards : UserControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the WizardPage_CompareCards class.
        /// </summary>
        public WizardPage_CompareCards()
        {
            this.InitializeComponent();
        }

        #endregion Constructor

        #region Event Handlers

        /// <summary>
        /// Handles the MouseEnter event on the Compare Cards DataGridRow
        /// </summary>
        /// <param name="sender">DataGridRow the mouse is now hovering over</param>
        /// <param name="e">The parameter is not used.</param>
        private void ConverterDataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            DataGridRow dataGridRow = sender as DataGridRow;

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

        /// <summary>
        /// Handles the MouseLeave event on the Compare Cards DataGridRow
        /// </summary>
        /// <param name="sender">DataGridRow the mouse is no longer hovering over</param>
        /// <param name="e">The parameter is not used.</param>
        private void ConverterDataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            DataGridRow dataGridRow = sender as DataGridRow;

            if (dataGridRow != null)
            {
                this.MouseOverConverterCard(null);
            }
        }

        /// <summary>
        /// Handles the MouseMove event on the ComboBox inside a DataGridRow
        /// </summary>
        /// <param name="sender">ComboBox the mouse is hovering over</param>
        /// <param name="e">The parameter is not used.</param>
        private void ComboBox_MouseMove(object sender, MouseEventArgs e)
        {
            var container = (sender as ComboBox).ContainerFromElement((DependencyObject)e.OriginalSource);
            if (container != null)
            {
                ComboBoxItem comboBoxItem = container as ComboBoxItem;

                if (comboBoxItem != null)
                {
                    this.MouseOverConverterCard(comboBoxItem.Content as ConverterCard);
                }
            }
        }

        #endregion Event Handlers

        #region Private Methods

        /// <summary>
        /// Sets the MouseOver ConverterCard on the ViewModel based on mouse events
        /// </summary>
        /// <param name="converterCard">The ConverterCard the mouse is hovering over.  If null, then mouse is not hovering over any.</param>
        private void MouseOverConverterCard(ConverterCard converterCard)
        {
            ViewModel.WizardPage_CompareCards wizardPageCompareCards = this.DataContext as ViewModel.WizardPage_CompareCards;
            if (wizardPageCompareCards != null)
            {
                wizardPageCompareCards.MouseOverConverterCard = converterCard;
            }
        }

        #endregion Private Methods
    }
}
