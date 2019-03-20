using PostOfficeClient.Tables_Providers;
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace PostOfficeClient
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        UsersProvider users;
        Window previousForm;

        public Registration()
        {
            InitializeComponent();
        }

        public Registration(object o, UsersProvider Users)
        {
            InitializeComponent();
            if (o is MainWindow)
            {
                previousForm = (MainWindow)o;
            }
            users = Users;                       
        }

        protected override void OnClosed(EventArgs e)
        {
            previousForm.Show();
            previousForm.Activate();
            base.OnClosed(e);
        }       
        
        private void Register(object sender, RoutedEventArgs e)
        {
            if (lastName.Text == "" || firstName.Text == "" || email.Text == "")
            {
                MessageBox.Show("Пожалуйста, заполните поля, отмеченные звездочкой!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!DataProcessing.isDataCorrect(firstName.Text, typeOfField.Name)
                || !DataProcessing.isDataCorrect(lastName.Text, typeOfField.Name)
                || (!DataProcessing.isDataCorrect(middleName.Text, typeOfField.Name) && middleName.Text != ""))
            {
                MessageBox.Show("Некорректные данные ФИО", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!DataProcessing.isDataCorrect(email.Text, typeOfField.EMail))
            {
                wrongEmail.Text = "Введен некорректный E-Mail!";
                return;
            }

            if (address.Text != "")
                if (!DataProcessing.isDataCorrect(address.Text, typeOfField.Address))
                {
                    MessageBox.Show("Адрес некорректен или неточен!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

            if (wrongEmail.Text != "" || wrongPass.Text != "")
                return;

            if (pass.Password == "" || passRepeat.Password == "")
            {
                MessageBox.Show("Введите пароль!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (users.Table != null)
            {
                foreach (DataRow user in users.Table.Rows)
                {
                    if (user["Email"].ToString() == email.Text)
                    {
                        wrongEmail.Text = "Такой E-Mail уже зарегистрирован!";
                        return;
                    }
                }
            }

            CreateUser();
            if (users.Insert() != null)
                return;

            Hide();
            MessageBox.Show("Регистрация прошла успешно!");
            Close();
        }           

        private void CreateUser()
        {
            DataRow newUser = users.Table.NewRow();

            newUser["Email"] = email.Text.ToLowerInvariant();

            MD5 md5Hasher = MD5.Create();
            newUser["MD5Password"] = md5Hasher.ComputeHash(Encoding.Default.GetBytes(pass.Password));

            newUser["FirstName"] = firstName.Text;
            newUser["LastName"] = lastName.Text;
            newUser["MiddleName"] = middleName.Text;

            newUser["Address"] = address.Text;

            users.Table.Rows.Add(newUser);
        }

        private void Email_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            wrongEmail.Text = null;
        }

        private void PassRepeat_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (pass.Password != passRepeat.Password)
            {
                wrongPass.Text = "Пароли не совпадают!";
                return;
            }

            if (pass.Password == passRepeat.Password)
            {
                wrongPass.Text = null;
            }
        }       

        private void Address_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var response = DataProcessing.api.QueryAddress(address.Text);
                address.ItemsSource = response.suggestions;
                address.IsDropDownOpen = true;
            }                
        }

        private void Address_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (address.SelectedItem != null)
            {
                var suggestion = (suggestionscsharp.SuggestAddressResponse.Suggestions)address.SelectedItem;

                if (suggestion != null)
                    if (suggestion.data.postal_code != null)
                    {
                        var sourceAddress = address.SelectedItem;
                        suggestion.value = suggestion.data.postal_code + " " + sourceAddress;
                        address.SelectedItem = suggestion;
                    }                                                        
            }                        
        }

        private void LastName_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (lastName.Text != "")
            {
                lastName.Text = DataProcessing.NameNormalize(lastName.Text);
            }            
        }

        private void FirstName_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (firstName.Text != "")
            {
                firstName.Text = DataProcessing.NameNormalize(firstName.Text);
            }
        }

        private void MiddleName_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (middleName.Text != "")
            {
                middleName.Text = DataProcessing.NameNormalize(middleName.Text);
            }
        }        
    }
}
