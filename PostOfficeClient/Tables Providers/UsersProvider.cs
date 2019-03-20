namespace PostOfficeClient.Tables_Providers
{
    public class UsersProvider : TablesProvider
    {
        public UsersProvider(string sqlCommand = "SELECT * FROM Users") : base(sqlCommand)  { }

        public bool IsUserRegistered(string email)
        {
            if (Table.Select("EMail = '" + email + "'").Length == 1)
            {
                return true;
            }

            return false;
        }
    }
}
