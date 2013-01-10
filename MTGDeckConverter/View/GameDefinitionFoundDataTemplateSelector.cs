using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MTGDeckConverter.Model;

namespace MTGDeckConverter.View
{
    public class GameDefinitionFoundDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FoundTemplate
        { get; set; }

        public DataTemplate NotFoundTemplate
        { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return ConverterDatabase.SingletonInstance.GameDefinition != null ?
                this.FoundTemplate :
                this.NotFoundTemplate;
        }
    }
}