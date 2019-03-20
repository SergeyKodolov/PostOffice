using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows;

namespace PostOfficeClient.Tables_Providers
{
    public abstract class TablesProvider
    {
        protected string connectionString;
        protected SqlDataAdapter adapter;
        protected SqlConnection connection;

        public DataTable Table { get; private set; }       

        public TablesProvider(string _sqlCommand)
        {
            var data = File.ReadAllLines(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory()))) + @"\data.txt", Encoding.Default);

            connectionString = data[2];

            string sql = _sqlCommand;
            Table = new DataTable();
            connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, connection);
                adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(Table);

                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(adapter);
                adapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                adapter.InsertCommand = commandBuilder.GetInsertCommand();
                adapter.DeleteCommand = commandBuilder.GetDeleteCommand();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }

        public Exception Insert()
        {
            connection.Open();
            try
            {
                adapter.Update(Table);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return ex;
            }
            finally
            {
                connection.Close();
            }
            return null;
        }
    }
}
