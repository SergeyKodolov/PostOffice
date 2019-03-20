using PostOfficeClient.Tables_Providers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace PostOfficeClient.Windows.Admin_Windows
{
    /// <summary>
    /// Логика взаимодействия для Search.xaml
    /// </summary>
    public partial class Search : Window
    {
        Window previousForm;
        int index = 0;        
        MailingsProvider mailings;
        TrackingProvider tracking;
        UsersProvider users;
        PostOfficesProvider postOffices;

        public Search(object o, int _index)
        {
            InitializeComponent();
            mailings = new MailingsProvider();
            tracking = new TrackingProvider();
            users = new UsersProvider();
            postOffices = new PostOfficesProvider();
            index = _index;
            if (index != 0)
            {
                selectedOffice.Items.Add(index);
                selectedOffice.SelectedIndex = 0;                
            }
            else
            {
                selectedOffice.ItemsSource = postOffices.GetIndexesOfOffices();
            }

            usersTable.ItemsSource = users.Table.Rows;
            officesTable.ItemsSource = postOffices.Table.Rows;
            mailingsTable.ItemsSource = mailings.Table.Rows;

            if (o is Admin)
            {
                previousForm = (Admin)o;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            previousForm.Show();
            previousForm.Activate();
            base.OnClosed(e);
        }
        
        private void UsersTable_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            DataRow selectedUser = (DataRow)usersTable.SelectedItem;

            mailingsToUser.ItemsSource = mailings.Table.Select("AddresseeEmail='" + selectedUser["EMail"].ToString() + "'");
            mailingsFromUser.ItemsSource = mailings.Table.Select("AddresserEmail='" + selectedUser["EMail"].ToString() + "'");
        }

        private void OfficesTable_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            DataRow selectedOffice = (DataRow)officesTable.SelectedItem;

            mailingsInOffice.ItemsSource = GetMailingsList(selectedOffice);
        }

        private List<DataRow> GetMailingsList(DataRow selectedOffice)
        {
            List<DataRow> mailingsList = new List<DataRow>();

            if (mailings.Table != null)
            {
                foreach (DataRow mailing in mailings.Table.Rows)
                {
                    if ((bool)mailing["Delivered"])
                        continue;

                    if (DataProcessing.isMailingInOffice(mailing["TrackingNumber"].ToString(), tracking.Table.Rows, (int)selectedOffice["Index"]))
                    {
                        mailingsList.Add(mailing);
                    }
                }
            }

            return mailingsList;
        }

        private void MailingsTable_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            office.ItemsSource = null;
            DataRow selectedMailing = (DataRow)mailingsTable.SelectedItem;
            string trackNumber = selectedMailing["TrackingNumber"].ToString();
            trackOfMailing.ItemsSource = tracking.Table.Select("TrackNumber=" + trackNumber, "RegistrationDateTime ASC");
        }

        private void TrackOfMailing_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
           
            DataRow selectedOffice = (DataRow)trackOfMailing.SelectedItem;
            if (selectedOffice != null)
            {
                string index = selectedOffice["Index"].ToString();
                office.ItemsSource = postOffices.Table.Select("Index=" + index);
            }          
        }

        private void Selection_SelecteChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (selectedDate.SelectedDate != null && selectedOffice.SelectedIndex == -1)
            {
                string _selectedDate = string.Format("{0:MM.dd.yyyy}", selectedDate.SelectedDate.Value);
                trackInSelectedDate.ItemsSource = tracking.GetTrackingForSelectedDate(_selectedDate).Rows;
            }
            else if (selectedOffice.SelectedIndex != -1 && selectedDate.SelectedDate == null)
            {
                int _selectedOffice = (int)selectedOffice.SelectedValue;
                trackInSelectedDate.ItemsSource = tracking.GetTrackingForSelectedDate(_selectedOffice).Rows;
            }
            else if (selectedDate.SelectedDate != null && selectedOffice.SelectedIndex != -1)
            {
                string _selectedDate = string.Format("{0:MM.dd.yyyy}", selectedDate.SelectedDate.Value);
                int _selectedOffice = (int)selectedOffice.SelectedValue;
                trackInSelectedDate.ItemsSource = tracking.GetTrackingForSelectedDate(_selectedDate, _selectedOffice).Rows;
            }            
        }

        private void SelectedOffice_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete || e.Key == Key.Back)
                selectedOffice.SelectedIndex = -1;
        }
    }
}
