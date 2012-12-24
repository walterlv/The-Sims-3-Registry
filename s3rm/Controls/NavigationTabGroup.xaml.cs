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

namespace s3rm.Controls
{
    public class NavigationTabArgs : EventArgs
    {
        public readonly int SelectedIndex;
        public readonly int OldSelectedIndex;
        public readonly NavigationTabItem SelectedTab;
        public readonly NavigationTabItem OldSelectedTab;
        private bool cancel;
        public bool Cancel
        {
            get { return cancel; }
            set { cancel = value; }
        }
        public NavigationTabArgs(int index, int oldIndex, NavigationTabItem tab, NavigationTabItem oldTab)
        {
            SelectedIndex = index;
            OldSelectedIndex = oldIndex;
            SelectedTab = tab;
            OldSelectedTab = oldTab;
            cancel = false;
        }
    }


    /// <summary>
    /// NavigationTabGroup.xaml 的交互逻辑
    /// </summary>
    public partial class NavigationTabGroup : UserControl
    {
        public NavigationTabGroup()
        {
            InitializeComponent();
        }

        public event EventHandler<NavigationTabArgs> NavigationTabSelected;

        public List<NavigationTabItem> Children = new List<NavigationTabItem>();
        private int selectedIndex = -1;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                SelectedTab = Children[value];
            }
        }
        private NavigationTabItem selectedTab;
        public NavigationTabItem SelectedTab
        {
            get { return selectedTab; }
            set
            {
                if (selectedTab == value) return;
                selectedIndex = Children.IndexOf(value);
                bool isCancel = false;
                if (NavigationTabSelected != null)
                {
                    NavigationTabArgs arg = new NavigationTabArgs(Children.IndexOf(value), Children.IndexOf(selectedTab), value, selectedTab);
                    NavigationTabSelected(this, arg);
                    isCancel = arg.Cancel;
                }
                if (!isCancel)
                {
                    if (selectedTab != null) selectedTab.IsSelected = false;
                    if (value != null) value.IsSelected = true;
                    selectedTab = value;
                }
            }
        }

        public void AddChild(NavigationTabItem tab)
        {
            tab.MouseLeftButtonDown += tab_MouseLeftButtonDown;
            ContentPanel.Children.Add(tab);
            Children.Add(tab);
        }

        public void AddChild(string text)
        {
            NavigationTabItem naviTab = new NavigationTabItem();
            naviTab.Text = text;
            AddChild(naviTab);
        }

        private void tab_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectedTab = (NavigationTabItem)sender;
        }
    }
}
