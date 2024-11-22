using Hotel.AppData;
using System;
using System.Linq;
using System.Windows;

namespace Hotel.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        int loginAttempCount = 0;
        public AuthorizationWindow()
        {
            InitializeComponent();

            BlockingUserByDate();
        }

        private void EntryBtn_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Teкст сообщения","Заголовок" , MessageBoxButton.OK,MessageBoxImage.Error);
            //FeedBack.Error("Неверный логин или пароль!");
            //FeedBack.Warning("Вы не ввели логин или пароль.");
            //FeedBack.Information("Погода на улице не айс");
            if (Validation() == true)
            {
                Authentication();
            }



        }

        public void BlockingUserByDate()
        {
            foreach (var user in App.context.User)
            {
                if (user.RegistrationDate.AddMonths(1) < DateTime.Now)
                {
                    user.IsBlocked = true;
                }
            }
        }

        public bool Validation()
        {
            if (LoginTb.Text == string.Empty && PasswordPb.Password == string.Empty)
            {
                FeedBack.Warning("Поля для ввода логина и пароля обязательны для заполнения. Введите логин и пароль");
                return false;
            }
            else if (LoginTb.Text == string.Empty)
            {
                FeedBack.Warning("Поле для ввода логина обязательны для заполнения. Введите логин");
                return false;
            }
            else if (PasswordPb.Password == string.Empty)
            {
                FeedBack.Warning("Поле для ввода пароля обязательны для заполнения. Введите пароль");
                return false;
            }
            return true;
        }

        public void Authentication()
        {

            App.currentUser = App.context.User.FirstOrDefault(user => user.Login == LoginTb.Text && user.Password == PasswordPb.Password);
            if (App.currentUser == null)
            {
                loginAttempCount++;
                FeedBack.Error($"Вы ввели неверный логин или пароль. Пожалуйста проверьте еще раз введенные данные. Попытка: {loginAttempCount} из 3");
                if (loginAttempCount == 3)
                {
                    //App.currentUser.IsBlocked = true;
                    loginAttempCount = 0;
                    FeedBack.Error("Вы заблокированы");
                    Close();
                }
            }
            else if (App.currentUser.IsBlocked == true)
            {
                FeedBack.Error("Вы заблокированы. Обратитесь к администратору!");
            }
            else if (App.currentUser.IsActivated == false)
            {
                ChangePasswordWindow changePasswordWindow = new ChangePasswordWindow();
                changePasswordWindow.Show();
                Hide();
            }
            else
            {
                FeedBack.Information("вы успешно авторизовались");

            }
        }
        public void Authorization()
        {
            switch (App.currentUser.RoleId)
            {
                case 1:
                    AdministratorWindow administratorWindow = new AdministratorWindow();
                    administratorWindow.Show();
                    break;
                case 2:
                    FeedBack.Information("Вход в качестве пользователя");
                    break;
                default:
                    FeedBack.Error("Доступ запрещен");
                    break;
            }
        }
    }
}
