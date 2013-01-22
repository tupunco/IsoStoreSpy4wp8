using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using System.Collections.Specialized;

namespace IsoStoreSpy.Tools
{
    public static class SelectedItemsBehavior
    {


        public static IList GetSelectedItems(DependencyObject obj)
        {
            return (IList)obj.GetValue(SelectedItemsProperty);
        }

        public static void SetSelectedItems(DependencyObject obj, IList value)
        {
            obj.SetValue(SelectedItemsProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectedItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.RegisterAttached("SelectedItems", typeof(IList), typeof(SelectedItemsBehavior), new PropertyMetadata(null, OnSelectedItemsChanged));

        /// <summary>
        /// SelectedItemsChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        public static void OnSelectedItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;


            if (e.OldValue != null)
            {
                listBox.SelectionChanged -= new SelectionChangedEventHandler(listBox_SelectionChanged);
            }

            // Initialisation des données
            listBox.RefreshSelectedItems();

            // Ajout
            
            if (e.NewValue != null)
            {
                listBox.SelectionChanged += new SelectionChangedEventHandler(listBox_SelectionChanged);
            }
        }

        /// <summary>
        /// Rafraichissement
        /// </summary>
        /// <param name="listbox"></param>

        public static void RefreshSelectedItems( this ListBox listbox )
        {
            IList values = listbox.GetValue(SelectedItemsBehavior.SelectedItemsProperty) as IList;

            listbox.SelectedItems.Clear();

            if (values != null)
            {
                foreach (object value in values)
                {
                    listbox.SelectedItems.Add(value);
                }
            }
        }

        /// <summary>
        /// Changement dans SelectionChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        static void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listbox = sender as ListBox;

            IList values = listbox.GetValue(SelectedItemsBehavior.SelectedItemsProperty) as IList;

            foreach (object addedItem in e.AddedItems)
            {
                values.Add(addedItem);
            }

            foreach (object removedItem in e.RemovedItems)
            {
                values.Remove(removedItem);
            }
        }        
    }
}
