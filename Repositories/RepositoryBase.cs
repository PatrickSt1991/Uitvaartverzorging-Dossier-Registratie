using Dossier_Registratie.Helper;
using System.Data.SqlClient;

namespace Dossier_Registratie.Repositories
{
    public abstract class RepositoryBase
    {
        protected SqlConnection GetConnection()
        {
            return new SqlConnection(DataProvider.ConnectionString);
        }
    }
}
