
using System.Windows.Media;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Collections.Generic;

namespace IsoStoreSpy.Tools
{
    public static class SelectedForegroundBehavior
    {
        /// <summary>
        /// SelectedForeground
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        public static Brush GetSelectedForeground(DependencyObject obj)
        {
            return (Brush)obj.GetValue(SelectedForegroundProperty);
        }

        public static void SetSelectedForeground(DependencyObject obj, Brush value)
        {
            obj.SetValue(SelectedForegroundProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectedForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedForegroundProperty =
            DependencyProperty.RegisterAttached("SelectedForeground", typeof(Brush), typeof(SelectedForegroundBehavior), new UIPropertyMetadata(null, OnSelectedForegroundChange));

        /// <summary>
        /// On attache cette propriété aux ListBoxItems
        /// </summary>

        private static readonly DependencyProperty HaveSelectedForegroundProperty =
            DependencyProperty.RegisterAttached("HaveSelectedForeground", typeof(bool), typeof(ListBoxItem), new UIPropertyMetadata(false));

        /// <summary>
        /// Ch
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private static void OnSelectedForegroundChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;

            element.Loaded += new RoutedEventHandler(element_Loaded);
            element.Unloaded += new RoutedEventHandler(element_Unloaded);
        }

        /// <summary>
        /// Element chargé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        static void element_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;

            // desabonnement
            element.Loaded -= new RoutedEventHandler(element_Loaded);

            // Recherche du ListBoxItem
            ListBoxItem listBoxItem = GetRecursiveParent(element, typeof(ListBoxItem)) as ListBoxItem;

            if (listBoxItem != null)
            {                
                // Un seul evenement selected par ListBoxItem                            
                bool isInitialized = (bool)listBoxItem.GetValue(HaveSelectedForegroundProperty);

                if (isInitialized == false)
                {

                    if (listBoxItem.IsSelected == true)
                    {
                        SwapForegroundToSelectedColor(listBoxItem);
                    }
                    
                    listBoxItem.SetValue(HaveSelectedForegroundProperty, true);
                    
                    listBoxItem.Selected += new RoutedEventHandler(listBoxItem_Selected);
                    listBoxItem.Unselected += new RoutedEventHandler(listBoxItem_Selected);
                }
            }
        }

        /// <summary>
        /// Nettoyage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        static void element_Unloaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;

            // desabonnement
            element.Unloaded -= new RoutedEventHandler(element_Unloaded);

            // Recherche 
            ListBoxItem listBoxItem = GetRecursiveParent(element, typeof(ListBoxItem)) as ListBoxItem;

            if (listBoxItem != null)
            {
                bool isInitialized = (bool)listBoxItem.GetValue(HaveSelectedForegroundProperty);

                if (isInitialized == true)
                {
                    listBoxItem.SetValue(HaveSelectedForegroundProperty, false);

                    listBoxItem.Selected -= new RoutedEventHandler(listBoxItem_Selected);
                    listBoxItem.Unselected -= new RoutedEventHandler(listBoxItem_Selected);
                }
            }
        }

        /// <summary>
        /// Selection : Un seul evenement selected par ListBoxItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        static void listBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            // la selection est trouvée
            ListBoxItem item = sender as ListBoxItem;

            SwapForegroundToSelectedColor(item);
        }

        /// <summary>
        /// Swap
        /// </summary>
        /// <param name="item"></param>

        private static void SwapForegroundToSelectedColor(ListBoxItem item )
        {
            // Comme dans cette evenement on ne connait plus les elements ou se trouve les behaviors
            // on les recherche tous dans le ListBoxItem

            List<DependencyObject> results = new List<DependencyObject>();

            GetRecursiveChildrendWithDependencyProperty(item, SelectedForegroundProperty, results);

            foreach (DependencyObject result in results)
            {
                Brush foregroundBrush = result.GetValue(Control.ForegroundProperty) as Brush;
                Brush selectedForegroundBrush = result.GetValue(SelectedForegroundProperty) as Brush;

                result.SetValue(Control.ForegroundProperty, selectedForegroundBrush);
                result.SetValue(SelectedForegroundProperty, foregroundBrush);
            }
        }

        /// <summary>
        /// Obtenir les enfants
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="property"></param>
        /// <returns></returns>

        private static void GetRecursiveChildrendWithDependencyProperty(DependencyObject reference, DependencyProperty property, List<DependencyObject> children )
        {
            if (reference == null)
            {
                return;
            }

            for (int index = 0; index < VisualTreeHelper.GetChildrenCount(reference); index++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(reference, index);

                object result =  child.GetValue(property);

                if (result != null)
                {
                    children.Add(child);
                }

                GetRecursiveChildrendWithDependencyProperty(child, property, children);
            }
        }

        /// <summary>
        /// GetRecursiveParent
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="typeOfDependencyObject"></param>
        /// <returns></returns>

        private static DependencyObject GetRecursiveParent(DependencyObject reference, Type typeOfDependencyObject)
        {
            if (reference == null)
                return null;

            DependencyObject result = VisualTreeHelper.GetParent(reference);

            if (result.GetType() == typeOfDependencyObject)
            {
                return result;
            }

            return GetRecursiveParent(result, typeOfDependencyObject);
        }
    }
}
