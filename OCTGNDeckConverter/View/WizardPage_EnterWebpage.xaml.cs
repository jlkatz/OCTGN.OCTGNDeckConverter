// -----------------------------------------------------------------------
// <copyright file="WizardPage_EnterWebpage.xaml.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace OCTGNDeckConverter.View
{
    /// <summary>
    /// Interaction logic for WizardPage_EnterWebpage.xaml
    /// </summary>
    public partial class WizardPage_EnterWebpage : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the WizardPage_EnterWebpage class.
        /// </summary>
        public WizardPage_EnterWebpage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Fired when the user clicks on a hyperlink control that has been piped to call this method.
        /// Opens the specified Uri in the default web browser.
        /// </summary>
        /// <param name="sender">The sender of the Event</param>
        /// <param name="e">The RequestNavigateEventArgs property</param>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
        }
    }
}
