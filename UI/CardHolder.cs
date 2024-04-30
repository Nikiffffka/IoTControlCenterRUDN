using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IoTControl.UI
{
    public class CardHolder: UserControl
    {
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Border border = new Border()
            {
                Style = (Style)this.FindResource("BarderUI")
            };
            ContentPresenter content = new ContentPresenter();
            content.Margin = new Thickness(5);
            content.Content = Content;
            border.Child = content;
            Content = border;
        }
    }
}
