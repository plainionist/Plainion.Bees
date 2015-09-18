using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Plainion.GatedCheckIn
{
    //https://michlg.wordpress.com/2010/01/17/listbox-automatically-scroll-to-bottom/
    public class ListBoxExtenders : DependencyObject
    {
        public static readonly DependencyProperty AutoScrollToEndProperty = DependencyProperty.RegisterAttached("AutoScrollToEnd", typeof(bool), typeof(ListBoxExtenders), new UIPropertyMetadata(default(bool), OnAutoScrollToEndChanged));

        public static bool GetAutoScrollToEnd(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollToEndProperty);
        }

        public static void SetAutoScrollToEnd(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollToEndProperty, value);
        }

        public static void OnAutoScrollToEndChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var listBox = (ListBox)sender;

            var scrollToEndHandler = new NotifyCollectionChangedEventHandler((s1, e1) =>
                {
                    if (listBox.Items.Count > 0)
                    {
                        var listBoxItems = listBox.Items;
                        listBoxItems.MoveCurrentToLast();
                        listBox.ScrollIntoView(listBoxItems.CurrentItem);
                        Debug.WriteLine(listBoxItems.CurrentPosition);
                    }
                });

            var data = (INotifyCollectionChanged)listBox.Items.SourceCollection;
            if ((bool)e.NewValue)
            {
                data.CollectionChanged += scrollToEndHandler;
            }
            else
            {
                data.CollectionChanged -= scrollToEndHandler;
            }
        }
    }
}
