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
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class CurrentActionList : Page
    {

        #region Поля класса

        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        #endregion

        #region Код, автоматически сгенерированный Visual Studio



        public CurrentActionList()
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

        #region Добавление действия

        private void addActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (localSettings.Values["IsSetAnaliseInProgress"] != null) {
                var errorMessage = new Windows.UI.Popups.MessageDialog("Запрещено редактирование списка во время выполнения анализа");
                errorMessage.ShowAsync();
                return;
            }
            addActionFieldsStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void addActionReadyToAddButton_Click(object sender, RoutedEventArgs e)
        {
            string mayBeTitle = addActionNameFiled.Text.ToString();
            if (InputTitleChecking(mayBeTitle.ToString())) {
                InsertActionInDataBase(mayBeTitle.ToString());
                FormViewOfListView();            
            }
            addActionNameFiled.Text = "";
            addActionFieldsStackPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        #endregion

        private void deleteActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (localSettings.Values["IsSetAnaliseInProgress"] != null)
            {
                var errorMessage = new Windows.UI.Popups.MessageDialog("Запрещено редактирование списка во время выполнения анализа");
                errorMessage.ShowAsync();
                return;
            }
            if (actionsListView.SelectedItems.Count != 0)
            {
                RemoveActionFromDataBase(actionsListView.SelectedItem as string);
                actionsListView.Items.Remove(actionsListView.SelectedItem);
            }
            else {
                var errorMessge = new Windows.UI.Popups.MessageDialog("Для того, что бы удалить дело из списка, сначала выделите его щелчком мыши, а затем нажмите кнопку \"Удалить\"", "Выделите дело");
                errorMessge.ShowAsync();
            }

        }

        private void tempClearListButton_Click(object sender, RoutedEventArgs e)
        {
            if (localSettings.Values["IsSetAnaliseInProgress"] != null)
            {
                var errorMessage = new Windows.UI.Popups.MessageDialog("Запрещено редактирование списка во время выполнения анализа");
                errorMessage.ShowAsync();
                return;
            }
            ClearTableInDB();
            FormViewOfListView();
        }
        #endregion

        #region Вспомогательные методы

        private void FormViewOfListView()
        {
            ShowDataBaseInActionsListView();
            if (actionsListView.Items.Count == 0)
            {
                textBoxListIsEmpty.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else {
                textBoxListIsEmpty.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }

        }

        private void ShowDataBaseInActionsListView()
        {
            actionsListView.Items.Clear();
            using (var db = new SQLiteConnection(localSettings.Values["ActionsDBPath"] as string))
            {
                var actionsListFromDB = db.Query<Actions>("SELECT * FROM Actions");
                foreach (Actions iteration in actionsListFromDB)
                {
                    actionsListView.Items.Add(iteration.Title.ToString());
                }
            }
        }

        private void InsertActionInDataBase(string actionName) 
        {
            using (var db = new SQLite.SQLiteConnection(localSettings.Values["ActionsDBPath"] as string))
            {
                var tempAction = new Actions() { Title = actionName.ToString(), SpendedTimeInSeconds = 0};
                db.Insert(tempAction);
            }
        }

        private void RemoveActionFromDataBase(string actionTitle) {
            using (var db = new SQLite.SQLiteConnection(localSettings.Values["ActionsDBPath"] as string))
            {
                db.Query<Actions>("DELETE FROM Actions WHERE Title = '"+actionTitle.ToString()+"'");
            }
        }

        private void ClearTableInDB() {
            using (var db = new SQLite.SQLiteConnection(localSettings.Values["ActionsDBPath"] as string))
            {
                db.DeleteAll<Actions>();
            }
        }

        private bool InputTitleChecking(string input) {

            

            if (String.IsNullOrWhiteSpace(input) == true) {
                var errorMessage = new Windows.UI.Popups.MessageDialog("Введите не пустую строку", "Ошибка ввода");
                errorMessage.ShowAsync();
                return false;
            }
            using (var db = new SQLite.SQLiteConnection(localSettings.Values["ActionsDBPath"] as string))
            {
                var tempActionsList = db.Query<Actions>("SELECT Title FROM Actions");
                foreach (Actions iterFromDB in tempActionsList) {
                    if (iterFromDB.Title == input) {
                        var errorMessage = new Windows.UI.Popups.MessageDialog("Введите уникальное наименования действия", "Ошибка ввода");
                        errorMessage.ShowAsync();
                        return false;
                    }
                }
            }
            foreach (char symb in input) {
                if (((int)symb < 65 | (int)symb > 90))
                    if (((int)symb < 97 | (int)symb >122))
                        if ((int)symb!=95)
                            if ((int)symb != 32)
                            {
                                var errorMessage = new Windows.UI.Popups.MessageDialog("Используйте только символы английского алфавита и пробел для названия дела", "Ошибка ввода");
                                errorMessage.ShowAsync();
                                return false;
                            }
            }
            
            return true;
        }

        #endregion
    }
}
