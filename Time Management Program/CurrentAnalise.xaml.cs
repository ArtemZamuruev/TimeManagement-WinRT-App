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

using SQLite;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Time_Management_Program
{
    public sealed partial class CurrentAnalise : Page
    {

        #region Поля класса

        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;  

        #endregion

        #region Код, автоматически сгенерированный Visual Studio


        public CurrentAnalise()
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
            if (ActionsDataBaseIsNotEmty())
            {
                FormEnablingButtonsView();
                ShowDataBaseInComboBoxes();
            }
            else
            {
                FormEnablingButtonsView();
                createNewAnaliseButton.IsEnabled = false;
                var errorMessage = new Windows.UI.Popups.MessageDialog("Невозможно работать с анализом, пока список дел пуст", "Заполните список дел");
                errorMessage.ShowAsync();
            }
        }

        #endregion

        #region Buttons!

        private void GoBack(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
        //Buttons from Main Panel:
        private void createNewAnaliseButton_Click(object sender, RoutedEventArgs e)
        {
            newAnaliseGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
            changeAnaliseGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void changeCurrentAnaliseButton_Click(object sender, RoutedEventArgs e)
        {
            newAnaliseGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            changeAnaliseGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;        }

        private void endAnaliseButton_Click(object sender, RoutedEventArgs e)
        {
            newAnaliseGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            changeAnaliseGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            UpdateTimeInSecondWhichWaseSpendedForLastAction();
            SaveThisAnaliseToBaseOfAnalises();
            localSettings.Values["IsSetAnaliseInProgress"] = null;
            localSettings.Values["CurrentAction"] = null;
            localSettings.Values["DateTimeOfCurrentAnaliseStarted"] = null;
            ResetCurrentAnaliseTable();
            FormEnablingButtonsView();
        }

        private void gotoGraphOfCurrentAnalise_Click(object sender, RoutedEventArgs e)
        {
            UpdateTimeInSecondWhichWaseSpendedForLastAction();
            UpdateDateTimeOfStartForCurrentAction();
            Frame.Navigate(typeof(GraphOfTheCurrentAnalise));
        }


        //Buttons from grids:

        private void startNewAnaliseButton_Click(object sender, RoutedEventArgs e)
        {
            //Сохраняем в настройках программы, что в данный момент идет анализ
            localSettings.Values["IsSetAnaliseInProgress"] = true;

            localSettings.Values["DateTimeOfCurrentAnaliseStarted"] = DateTime.Now.ToString();

            //Сохраняем в настройках программы имя текущего действия
            localSettings.Values["CurrentAction"] = choosingActionTOStartAnalise.SelectedItem.ToString();

            // Делаем апдейт для базы данных, изменяя датувремя начала выполнения дела
            UpdateDateTimeOfStartForCurrentAction();

            //Делаем поле для начала нового анализа невидымым
            newAnaliseGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            
            //Обновляем доступность кнопок
            FormEnablingButtonsView();
        }

        private void applyChoosenActionAsCurrent_Click(object sender, RoutedEventArgs e)
        {
            UpdateTimeInSecondWhichWaseSpendedForLastAction();
            localSettings.Values["CurrentAction"] = choosingActionToChangeCurrentAction.SelectedItem.ToString();
            UpdateDateTimeOfStartForCurrentAction();
            changeAnaliseGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        #endregion

        #region Вспомогательные методы

        private bool ActionsDataBaseIsNotEmty() {
            using (var db = new SQLiteConnection(localSettings.Values["ActionsDBPath"] as string))
            {
                var actionsListFromDB = db.Query<Actions>("SELECT * FROM Actions");
                if (actionsListFromDB.Count == 0) return false;
                else return true;
            }
        }

        private void FormEnablingButtonsView() {
            if (localSettings.Values["IsSetAnaliseInProgress"] == null)
            {
                changeCurrentAnaliseButton.IsEnabled = false;
                endAnaliseButton.IsEnabled = false;
                gotoGraphOfCurrentAnalise.IsEnabled = false;
                createNewAnaliseButton.IsEnabled = true;
            }
            else {
                createNewAnaliseButton.IsEnabled = false;
                changeCurrentAnaliseButton.IsEnabled = true;
                endAnaliseButton.IsEnabled = true;
                gotoGraphOfCurrentAnalise.IsEnabled = true;
            }
        }

        private void ShowDataBaseInComboBoxes() {
            using (var db = new SQLiteConnection(localSettings.Values["ActionsDBPath"] as string)) {
                var actionsListFromDB = db.Query<Actions>("SELECT * FROM Actions");
                foreach (Actions iter in actionsListFromDB) {
                    choosingActionToChangeCurrentAction.Items.Add(iter.Title.ToString());
                    choosingActionTOStartAnalise.Items.Add(iter.Title.ToString());
                }
                
            }
            choosingActionToChangeCurrentAction.SelectedIndex = 0;
            choosingActionTOStartAnalise.SelectedIndex = 0;
        }

        private void ResetCurrentAnaliseTable() {
            using (var db = new SQLiteConnection(localSettings.Values["ActionsDBPath"] as string))
            {
                db.Query<Actions>("UPDATE Actions SET SpendedTimeInSeconds = '0'");
                db.Query<Actions>("UPDATE Actions SET DateTimeOfStart = '"+DateTime.MinValue+"'");
            
            }
        }

        private void UpdateDateTimeOfStartForCurrentAction() {
            using (var db = new SQLiteConnection(localSettings.Values["ActionsDBPath"] as string)) {
                db.Query<Actions>("UPDATE Actions SET DateTimeOfStart = '"+DateTime.Now+"' WHERE Title ='"+localSettings.Values["CurrentAction"].ToString()+"'");
            }
        }

        private void UpdateTimeInSecondWhichWaseSpendedForLastAction() {

            using (var db = new SQLiteConnection(localSettings.Values["ActionsDBPath"] as string))
            {
                string tempstring = localSettings.Values["CurrentAction"] as string;
                var currentActionRow = from action in db.Table<Actions>() where action.Title == tempstring  select action;
                Actions temp = currentActionRow.FirstOrDefault();
                int timeSpendedForActionInPast = temp.SpendedTimeInSeconds;
                DateTime dateTimeOfCurrentActionStart = temp.DateTimeOfStart;
                TimeSpan spendedTime = DateTime.Now.Subtract(dateTimeOfCurrentActionStart);
                int newTimeSpendedForAction = spendedTime.Hours * 3600 + spendedTime.Minutes * 60 + spendedTime.Seconds+timeSpendedForActionInPast;
                db.Query<Actions>("UPDATE Actions SET SpendedTimeInSeconds = '" + newTimeSpendedForAction.ToString() + "' WHERE Title ='" + localSettings.Values["CurrentAction"].ToString() + "'");
            }
        }

        private void SaveThisAnaliseToBaseOfAnalises() {
            string allActionsOfThisAnalise = "";
            string allTimesPeriodsOfThisAnalise = "";
            using (var dbActions = new SQLite.SQLiteConnection(localSettings.Values["ActionsDBPath"] as string))
            {
                var allActions = dbActions.Query<Actions>("SELECT * FROM Actions");
                foreach (Actions iter in allActions) { 
                    allActionsOfThisAnalise += iter.Title + ";";
                    allTimesPeriodsOfThisAnalise += iter.SpendedTimeInSeconds.ToString()+";";
                }
            }
            string analiseName = FormStringOfAnaliseNameFromCurrentDate();
            using (var db = new SQLite.SQLiteConnection(localSettings.Values["OldAnalisesDBPath"] as string))
            {
                var analise = new OldAnalises() { Title = analiseName.ToString(), ActionsList = allActionsOfThisAnalise, TimesForActions = allTimesPeriodsOfThisAnalise, DateTimeOfSave = DateTime.Now};
                db.Insert(analise);
            }
        }

        private string FormStringOfAnaliseNameFromCurrentDate() {
            string result = "";
            string dateTimeOfCurrentAnaliseFinished = DateTime.Now.ToString();
            dateTimeOfCurrentAnaliseFinished = dateTimeOfCurrentAnaliseFinished.Replace(" ", "_");
            result = dateTimeOfCurrentAnaliseFinished;
            using (var db = new SQLite.SQLiteConnection(localSettings.Values["OldAnalisesDBPath"] as string))
            {
                var allOldAnalises = db.Query<OldAnalises>("SELECT * FROM OldAnalises");
                foreach (OldAnalises iter in allOldAnalises)
                {
                    if (result == iter.Title) result += "_New";
                }
            }
            return result;
        }
        
        #endregion

    }
}
