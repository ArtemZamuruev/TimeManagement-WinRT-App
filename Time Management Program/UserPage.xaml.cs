using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;
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
    public sealed partial class UserPage : Page
    {

        #region Поля класса

        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        #endregion

        #region Код, автоматически сгенерированный Visual Studio

        public UserPage()
        {
            this.InitializeComponent();
            FormViewOfInfoBlock();
            #region Если не задано имя пользователя, то спросит его об имени и получить его
            //пусто
            #endregion
        }

        /// <summary>
        /// Вызывается перед отображением этой страницы во фрейме.
        /// </summary>
        /// <param name="e">Данные о событиях, описывающие, каким образом была достигнута эта страница.  Свойство Parameter
        /// обычно используется для настройки страницы.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        #endregion

        #region Buttons!
        private void actionsListButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CurrentActionList));
        }

        private void currentAnaliseButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CurrentAnalise));
        }

        private void graphsBaseButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(OldGraphicsPage));
        }

        #endregion

        #region Вспомогательные методы

        private void FormViewOfInfoBlock()
        {
            if (localSettings.Values["IsSetAnaliseInProgress"] != null)
                isSetActiveAnaliseTextBlock.Text = "Да";
            else isSetActiveAnaliseTextBlock.Text = "Нет";

            if (localSettings.Values["CurrentAction"] != null)
                currentActionTextBox.Text = localSettings.Values["CurrentAction"].ToString();
            else currentActionTextBox.Text = "Нет";

            using (var db = new SQLiteConnection(localSettings.Values["OldAnalisesDBPath"] as string))
            {
                var allOldAnalises = db.Query<OldAnalises>("SELECT * FROM OldAnalises");
                numberOfAnalisesDoneTextBox.Text = allOldAnalises.Count.ToString();
            }
            if (localSettings.Values["DateTimeOfCurrentAnaliseStarted"] != null)
                dateOfStartCurrentAnalise.Text = localSettings.Values["DateTimeOfCurrentAnaliseStarted"].ToString();
            else dateOfStartCurrentAnalise.Text = "Нет";

        }
        #endregion
    }
}
