using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.UI.Xaml.Shapes;
using SQLite;


// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Time_Management_Program
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class GraphOfTheCurrentAnalise : Page
    {
        #region Поля класса
       
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        #endregion

        #region Код, сгенерированный Visual Studio
        public GraphOfTheCurrentAnalise()
        {
            this.InitializeComponent();            
        }

        /// <summary>
        /// Вызывается перед отображением этой страницы во фрейме.
        /// </summary>
        /// <param name="e">Данные о событиях, описывающие, каким образом была достигнута эта страница.  Свойство Parameter
        /// обычно используется для настройки страницы.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ResultsListViewFilling();     
        }
        #endregion

        #region Buttons!

        private void GoBack(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }


        #endregion

        #region Вспомогательные методы

        private void ResultsListViewFilling() {
            List<Actions> actionsList = GetListOfActionsFromDataBase();
            int totalSpendedTime = 0;
            resultsListView.Items.Clear();
            foreach (Actions iterAct in actionsList) {
                totalSpendedTime += iterAct.SpendedTimeInSeconds;
            }
            foreach (Actions iterAct in actionsList) {
                TextBlock tempTextBlock = new TextBlock() { Height = 70, Width = 300, Text = iterAct.Title, Margin = new Thickness(5, 0, 0, 0), FontSize = 23 };
                TextBlock tempPercentTextBlock = new TextBlock() { Height = 70, Width = 60, TextAlignment = TextAlignment.Center, FontSize = 20 };
                Rectangle tempRectangle = new Rectangle() { Height = 70, Fill = new SolidColorBrush(Windows.UI.Colors.SkyBlue), Margin = new Thickness(5, 0, 0, 0) };
                if (iterAct.SpendedTimeInSeconds == 0)
                {
                    tempRectangle.Width = 0;
                    tempPercentTextBlock.Text = "0%";
                }
                else
                {
                    tempRectangle.Width = (((double)iterAct.SpendedTimeInSeconds) / (double)totalSpendedTime) * (double)700;
                    tempPercentTextBlock.Text = (((double)iterAct.SpendedTimeInSeconds / (double)totalSpendedTime) * (double)100).ToString("F1") + "%";
                }
                StackPanel tempStackPanel = new StackPanel() { Orientation = Orientation.Horizontal};
                tempStackPanel.Children.Add(tempTextBlock);
                tempStackPanel.Children.Add(tempPercentTextBlock);
                tempStackPanel.Children.Add(tempRectangle);
                resultsListView.Items.Add(tempStackPanel);
            }
        }

        private List<Actions> GetListOfActionsFromDataBase() {
            List<Actions> actList = new List<Actions>();
            using (var db = new SQLiteConnection(localSettings.Values["ActionsDBPath"] as string))
            {
                var actionsListFromDB = db.Query<Actions>("SELECT * FROM Actions");
                foreach (Actions iter in actionsListFromDB)
                {
                    Actions tempAction = new Actions() { Title = iter.Title, SpendedTimeInSeconds = iter.SpendedTimeInSeconds };
                    actList.Add(tempAction);
                }
            }
            return actList;
        }

        private double CalculatePercentOfTimeWhichWasSpendedForActions(double total, double spendedForAction) {
            return (spendedForAction / total) * 100;
        }

        #endregion
    }
}
