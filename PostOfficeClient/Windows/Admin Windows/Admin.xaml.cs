using PostOfficeClient.Tables_Providers;
using System;
using System.Windows;

namespace PostOfficeClient.Windows.Admin_Windows
{
    /// <summary>
    /// Логика взаимодействия для Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {
        Window previousForm;
        int index;
        PostOfficesProvider postOffices;

        public Admin()
        {
            InitializeComponent();
        }

        public Admin(object o)
        {
            InitializeComponent();
            postOffices = new PostOfficesProvider();

            curOffice.ItemsSource = postOffices.GetIndexesOfOffices();

            if (o is MainWindow)
            {
                previousForm = (MainWindow)o;
            }
        }        

        protected override void OnClosed(EventArgs e)
        {
            previousForm.Show();
            previousForm.Activate();
            base.OnClosed(e);
        }

        private void ReceiveButton_Click(object sender, RoutedEventArgs e)
        {
            MailingsReceiving window = new MailingsReceiving(this, index);
            Hide();
            window.Show();
        }

        private void CurOffice_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (curOffice.SelectedIndex == -1)
            {
                giveButton.IsEnabled = 
                receiveButton.IsEnabled = 
                receiveOperationButton.IsEnabled = 
                sendOperationButton.IsEnabled = 
                papersButton.IsEnabled = false;
            }
            else
            {
                giveButton.IsEnabled = 
                receiveButton.IsEnabled = 
                receiveOperationButton.IsEnabled = 
                sendOperationButton.IsEnabled = 
                papersButton.IsEnabled = true;

                index = (int)curOffice.SelectedValue;
            }
        }

        private void GiveButton_Click(object sender, RoutedEventArgs e)
        {
            MailingsDelivery window = new MailingsDelivery(this, index);
            Hide();
            window.Show();
        }

        private void ReceiveOperationButton_Click(object sender, RoutedEventArgs e)
        {
            ReceiveOperation window = new ReceiveOperation(this, index);
            Hide();
            window.Show();
        }

        private void SendOperationButton_Click(object sender, RoutedEventArgs e)
        {
            SendOperation window = new SendOperation(this, index);
            Hide();
            window.Show();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Search window = new Search(this, index);
            Hide();
            window.Show();
        }

        private void PapersButton_Click(object sender, RoutedEventArgs e)
        {
            Papers window = new Papers(this, index);
            Hide();
            window.Show();
        }
    }
}
