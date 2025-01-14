using Dossier_Registratie.Interfaces;
using System;
using System.Data.SqlClient;

namespace Dossier_Registratie.Repositories
{
    public class DeleteAndActivateDisableOperations : RepositoryBase, IDeleteAndActivateDisableOperations
    {
        public void CloseDossier(Guid uitvaartId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [OverledeneUitvaartleider] SET dossierCompleted = 1 WHERE UitvaartId = @UitvaartId";
                command.Parameters.AddWithValue("@UitvaartId", uitvaartId);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("ClosingDossierFailed");
                }
            }
        }
        public void ReOpenDossier(Guid uitvaartId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [OverledeneUitvaartleider] SET dossierCompleted = 0 WHERE UitvaartId = @UitvaartId";
                command.Parameters.AddWithValue("@UitvaartId", uitvaartId);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("ReOpeningDossierFailed");
                }
            }
        }
        public void ActivateEmployee(Guid employeeId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [ConfigurationPersoneel] SET isDeleted = 0 WHERE Id = @employeeId";
                command.Parameters.AddWithValue("@employeeId", employeeId);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("EmployeeActivateFailed");
                }
            }
        }
        public void ActivateKist(Guid kistId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [ConfigurationKisten] SET isDeleted = 0 WHERE Id = @kistId";
                command.Parameters.AddWithValue("@kistId", kistId);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("CreatKistActivateFailed");
                }
            }
        }
        public void ActivateAsbestemming(Guid asbestemmingId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [ConfigurationAsbestemming] SET isDeleted = 0 WHERE asbestemmingId = @asbestemmingId";
                command.Parameters.AddWithValue("@asbestemmingId", asbestemmingId);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("CreateAsbestemmingActivateFailed");
                }
            }
        }
        public void ActivateVerzekeraar(Guid verzekeraarId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [ConfigurationVerzekeraar] SET isDeleted = 0 WHERE Id = @verzekeraarId";
                command.Parameters.AddWithValue("@verzekeraarId", verzekeraarId);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("VerzekeraarActivateFailed");
                }
            }
        }
        public void ActivateLeverancier(Guid leverancierId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [ConfigurationLeveranciers] SET isDeleted = 0 WHERE leverancierId = @leverancierId";
                command.Parameters.AddWithValue("@leverancierId", leverancierId);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("LeverancierActivateFailed");
                }
            }
        }
        public void ActivatePriceComponent(Guid componentId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [ConfigurationFactuurComponent] SET isDeleted = 0 WHERE ComponentId = @componentId";
                command.Parameters.AddWithValue("@componentId", componentId);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("ComponentActivateFailed");
                }
            }
        }
        public void ActivateSuggestion(Guid suggestionId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [ConfigurationOverledenLocaties] SET isDeleted = 0 WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", suggestionId);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("SuggestionActivationFailed");
                }
            }
        }
        public void ActivateRouwbrief(Guid rouwbriefId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [ConfigurationRouwbrieven] SET isDeleted = 0 WHERE rouwbrievenId = @rouwbriefId";
                command.Parameters.AddWithValue("@rouwbriefId", rouwbriefId);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("RouwbriefActivateFailed");
                }
            }
        }
        public void DisableEmployee(Guid employeeId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [ConfigurationPersoneel] SET isDeleted = 1 WHERE Id = @employeeId";
                command.Parameters.AddWithValue("@employeeId", employeeId);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("KistDisabledFailed");
                }
            }
        }
        public void DisableKist(Guid kistId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [ConfigurationKisten] SET isDeleted = 1 WHERE Id = @kistId";
                command.Parameters.AddWithValue("@kistId", kistId);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("KistDisabledFailed");
                }
            }
        }
        public void DisableAsbestemming(Guid asbestemmingId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [ConfigurationAsbestemming] SET isDeleted = 1 WHERE asbestemmingId = @asbestemmingId";
                command.Parameters.AddWithValue("@asbestemmingId", asbestemmingId);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("AsbestemmingDisabledFailed");
                }
            }
        }
        public void DisableVerzekeraar(Guid verzekeraarId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [ConfigurationVerzekeraar] SET isDeleted = 1 WHERE Id = @verzekeraarId";
                command.Parameters.AddWithValue("@verzekeraarId", verzekeraarId);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("VerzekeraarDisabledFailed");
                }
            }
        }
        public void DisableLeverancier(Guid leverancierId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [ConfigurationLeveranciers] SET isDeleted = 1 WHERE leverancierId = @leverancierId";
                command.Parameters.AddWithValue("@leverancierId", leverancierId);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("LeverancierDisableFailed");
                }
            }
        }
        public void DisablePriceComponent(Guid componentId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [ConfigurationFactuurComponent] SET isDeleted = 1 WHERE ComponentId = @componentId";
                command.Parameters.AddWithValue("@componentId", componentId);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("ComponentDisabledFailed");
                }
            }
        }
        public void DisableRouwbrief(Guid rouwbriefId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [ConfigurationRouwbrieven] SET isDeleted = 1 WHERE rouwbrievenId = @rouwbriefId";
                command.Parameters.AddWithValue("@rouwbriefId", rouwbriefId);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("RouwbriefDisabledFailed");
                }
            }
        }
        public void SetDocumentInconsistent(Guid DocumentId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [OverledeneBijlages] " +
                                        "SET DocumentInconsistent = 1 " +
                                        "WHERE BijlageId = @DocumentId";
                command.Parameters.AddWithValue("@DocumentId", DocumentId);

                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("UpdateDocumentInconsistentFailed");
                }
            }
        }
        public void SetDocumentDeleted(Guid UitvaartId, string DocumentName)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [OverledeneBijlages] SET isDeleted = 1 WHERE UitvaartId = @UitvaartId AND DocumentName = @DocumentName AND isDeleted = 0";
                command.Parameters.AddWithValue("@UitvaartId", UitvaartId);
                command.Parameters.AddWithValue("@DocumentName", DocumentName);

                if (command.ExecuteNonQuery() == 0)
                    throw new InvalidOperationException("DeletingDocumentFailed");
            }
        }
        public void DisableSuggestion(Guid suggestionId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [ConfigurationOverledenLocaties] SET isDeleted = 1 WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", suggestionId);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("SuggestionDisabledFailed");
                }
            }
        }
    }
}
