using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SteuerSoft.Lage.Resources.Util
{
    class TemplateSelector : DataTemplateSelector
    {
        public Dictionary<Type, DataTemplate> Templates { get; set; } = new Dictionary<Type, DataTemplate>();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (Templates.ContainsKey(item.GetType()))
            {
                return Templates[item.GetType()];
            }

            return base.SelectTemplate(item, container);
        }
    }
}
