using System;
using System.Windows;
using System.Windows.Input;
namespace EXEIntegrator
{
    public partial class LoadingWindow : Window
    {
        //
        public LoadingWindow()
        {
            InitializeComponent();
        }
        //
        public void SetLoad(int percentage, string text)
        {
            LoadingBar.Value = percentage;
            LoadingText.Content = text;
        }
        //
        private void Background_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        /*
        public void test()
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                SelectionWindow selectionWindow = new SelectionWindow();
                selectionWindow.ApplicationTable.ItemsSource = (ApplicationInfoContainer[])e.Result;
                selectionWindow.Show();
            }));
        }*/
    }
}