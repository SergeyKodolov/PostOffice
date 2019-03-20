using System;
using System.Windows.Forms;

namespace PostOfficeDataBase
{
    public partial class DatabaseEditing : Form
    {
        public DatabaseEditing()
        {
            InitializeComponent();
        }
                
        private void DatabaseEditing_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "databaseDataSet.Tracking". При необходимости она может быть перемещена или удалена.
            this.trackingTableAdapter.Fill(this.databaseDataSet.Tracking);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "databaseDataSet.PostOffices". При необходимости она может быть перемещена или удалена.
            this.postOfficesTableAdapter.Fill(this.databaseDataSet.PostOffices);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "databaseDataSet.Mailings". При необходимости она может быть перемещена или удалена.
            this.mailingsTableAdapter.Fill(this.databaseDataSet.Mailings);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "databaseDataSet.Users". При необходимости она может быть перемещена или удалена.
            this.usersTableAdapter.Fill(this.databaseDataSet.Users);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "databaseDataSet.PostOffices". При необходимости она может быть перемещена или удалена.
            this.postOfficesTableAdapter.Fill(this.databaseDataSet.PostOffices);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "databaseDataSet.Tracking". При необходимости она может быть перемещена или удалена.
            this.trackingTableAdapter.Fill(this.databaseDataSet.Tracking);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "databaseDataSet.Users". При необходимости она может быть перемещена или удалена.
            this.usersTableAdapter.Fill(this.databaseDataSet.Users);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "databaseDataSet.Mailings". При необходимости она может быть перемещена или удалена.
            this.mailingsTableAdapter.Fill(this.databaseDataSet.Mailings);

        }

        private void UsersBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.usersBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.databaseDataSet);
        }
        
        private void MailingsBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.mailingsBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.databaseDataSet);
        }

        private void PostOfficesBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.postOfficesBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.databaseDataSet);
        }

        private void TrackingBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.trackingBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.databaseDataSet);
        }
    }
}
