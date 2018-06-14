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
    public sealed partial class OldGraphicsPage : Page
    {

        #region Поля класса

        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        #endregion

        #region Код, автоматически сгенерированный Visual Studio

        public OldGraphicsPage()
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
            FormViewOfListView();
        }

        #endregion

        #region Buttons!

        private void GoBack(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void resetStatisticButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new SQLiteConnection(localSettings.Values["OldAnalisesDBPath"] as string))
            {
                analiseDiagramView.Items.Clear();
                listOfAnalisesView.Items.Clear();
                db.DeleteAll<OldAnalises>();
                FormViewOfListView();
            }

        }

        #endregion

        #region Вспомогательные методы

        private void FormViewOfListView()
        {
            ShowDataBaseInActionsListView();
            if (listOfAnalisesView.Items.Count == 0)
            {
                textBoxListIsEmpty.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                textBoxListIsEmpty.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }

        }

        private void ShowDataBaseInActionsListView()
        {
            listOfAnalisesView.Items.Clear();
            using (var db = new SQLiteConnection(localSettings.Values["OldAnalisesDBPath"] as string))
            {
                var analisesListFromDB = db.Query<OldAnalises>("SELECT * FROM OldAnalises");
                foreach (OldAnalises iteration in analisesListFromDB)
                {
                    listOfAnalisesView.Items.Add(iteration.Title.ToString());
                }
            }
        }

        private void ResultsListViewFilling(string checkedAnaliseTitle)
        {
            List<Actions> actionsList = GetActionsListForCheckedAnalise(checkedAnaliseTitle);
            int totalSpendedTime = 0;
            analiseDiagramView.Items.Clear();
            foreach (Actions iterAct in actionsList)
            {
                totalSpendedTime += iterAct.SpendedTimeInSeconds;
            }
            foreach (Actions iterAct in actionsList)
            {
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
                StackPanel tempStackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
                tempStackPanel.Children.Add(tempTextBlock);
                tempStackPanel.Children.Add(tempPercentTextBlock);
                tempStackPanel.Children.Add(tempRectangle);
                analiseDiagramView.Items.Add(tempStackPanel);
            }
        }

        private List<Actions> GetActionsListForCheckedAnalise(string checkedAnaliseTitle) {
            List<Actions> tempList = new List<Actions>();

            List<string> actionsTitles = new List<string>();
            List<int> actionsTimes = new List<int>();

            using (var db = new SQLiteConnection(localSettings.Values["OldAnalisesDBPath"] as string))
            {
                var analisesListFromDB = from an in db.Table<OldAnalises>() where an.Title == checkedAnaliseTitle select an;
                OldAnalises analise = analisesListFromDB.FirstOrDefault();
                string[] actionsStringArray = analise.ActionsList.Split(';');
                for (int a = 0; a < actionsStringArray.Length - 1; a++)
                    actionsTitles.Add(actionsStringArray[a]);
                string[] actionsTimesStringArray = analise.TimesForActions.Split(';');
                for (int a = 0; a < actionsTimesStringArray.Length - 1; a++) {
                    int temp;
                    int.TryParse(actionsTimesStringArray[a], out temp);
                    actionsTimes.Add(temp);
                }
                for (int c = 0; c < actionsTitles.Count; c++) {
                    tempList.Add(new Actions() { Title = actionsTitles[c], SpendedTimeInSeconds = actionsTimes[c] });
                }
                return tempList;
            }
        }

        #endregion

        private void listOfAnalisesView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ResultsListViewFilling(e.ClickedItem.ToString());
        }

        private void listOfAnalisesView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResultsListViewFilling(listOfAnalisesView.SelectedItem.ToString());
        }
    }
}
