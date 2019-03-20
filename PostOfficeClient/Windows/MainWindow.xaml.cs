using PostOfficeClient.Tables_Providers;
using PostOfficeClient.Windows.Admin_Windows;
using System;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace PostOfficeClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MD5 md5Hasher;

        public UsersProvider Users { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Users = new UsersProvider();
            md5Hasher = MD5.Create();
        }

        private void Enter(object sender, RoutedEventArgs e)
        {
            if (login.Text == "")
            {
                MessageBox.Show("Введите логин!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password.Password == "")
            {
                MessageBox.Show("Введите пароль!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            foreach (DataRow curUser in Users.Table.Rows)
            {
                if (curUser["EMail"].ToString() == login.Text)
                {
                    byte[] leftHash = (byte[])curUser["MD5Password"];
                    byte[] rightHash = md5Hasher.ComputeHash(Encoding.Default.GetBytes(password.Password));

                    if (ByteArrayCompareWithSequenceEqual(leftHash, rightHash))
                    {
                        bool isAdmin = (Boolean)curUser["Admin"];
                        if (isAdmin)
                        {
                            Admin admin = new Admin(this);
                            this.Hide();
                            admin.Show();
                        }
                        else
                        {
                            User user = new User(this);
                            this.Hide();
                            user.Show();
                        }
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Неверный пароль!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }
            MessageBox.Show("Введена неверная почта!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static bool ByteArrayCompareWithSequenceEqual(byte[] p_BytesLeft, byte[] p_BytesRight)
        {
            return p_BytesLeft.SequenceEqual(p_BytesRight);
        }

        private void Registration(object sender, RoutedEventArgs e)
        {
            Registration reg = new Registration(this, Users);
            Hide();
            reg.Show();
        }
    }
}
