using Dossier_Registratie.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.IO;

namespace Dossier_Registratie.Helper
{
    public class DataProvider
    {
        private static IConfiguration _config;
        private static IConfiguration InstanceConfig
        {
            get
            {
                if (_config == null)
                {
                    _config = new ConfigurationBuilder()
                        .AddJsonFile("AppConnectionSettings.json")
                        .Build();
                }
                return _config;
            }
        }
        public static string PdfArchiveBaseFolder
        {
            get { return InstanceConfig["SystemConfiguration:PdfArchive"]; }
        }
        public static string AccessDatabase2024
        {
            get
            {
                return InstanceConfig["SystemConfiguration:Database2024"];
            }
        }
        public static string AccessDatabase2023
        {
            get
            {
                return InstanceConfig["SystemConfiguration:Database2023"];
            }
        }
        public static string SystemTitle
        {
            get
            {
                return InstanceConfig["SystemSettings:SystemTitle"];
            }
        }
        public static string OrganizationName
        {
            get
            {
                return InstanceConfig["CompanySettings:Name"];
            }
        }
        public static string OrganizationStreet
        {
            get
            {
                return InstanceConfig["CompanySettings:Street"];
            }
        }
        public static string OrganizationHouseNumber
        {
            get
            {
                return InstanceConfig["CompanySettings:HouseNumber"];
            }
        }
        public static string OrganizationHouseNumberAddition
        {
            get
            {
                return InstanceConfig["CompanySettings:HouseNumberAddition"];
            }
        }
        public static string OrganizationZipcode
        {
            get
            {
                return InstanceConfig["CompanySettings:Zipcode"];
            }
        }
        public static string OrganizationCity
        {
            get
            {
                return InstanceConfig["CompanySettings:City"];
            }
        }
        public static string OrganizationPhoneNumber
        {
            get
            {
                return InstanceConfig["CompanySettings:PhoneNumber"];
            }
        }
        public static string OrganizationEmail
        {
            get
            {
                return InstanceConfig["CompanySettings:Email"];
            }
        }
        public static string OrganizationIban
        {
            get
            {
                return InstanceConfig["CompanySettings:IBAN"];
            }
        }
        public static string GithubKey
        {
            get
            {
                return InstanceConfig["SystemSettings:GithubKey"];
            }
        }
        public static string GithubOwner
        {
            get
            {
                return InstanceConfig["SystemSettings:GithubOwner"];
            }
        }
        public static string GithubRepo
        {
            get
            {
                return InstanceConfig["SystemSettings:GithubRepo"];
            }
        }
        public static string GithubProduct
        {
            get
            {
                return InstanceConfig["SystemSettings:GithubProduct"];
            }
        }
        public static bool SetupComplete
        {
            get
            {
                return InstanceConfig.GetValue<bool>("SystemSettings:SetupComplete");
            }
        }
        public static bool GithubEnabled
        {
            get
            {
                return InstanceConfig.GetValue<bool>("SystemSettings:GithubEnabled");
            }
        }
        public static string TemplateFolder
        {
            get { return InstanceConfig["SystemConfiguration:TemplateFolder"]; }
        }
        public static string DocumentenOpslag
        {
            get { return InstanceConfig["SystemConfiguration:DocumentenOpslag"]; }
        }
        public static string FactuurOpslag
        {
            get { return InstanceConfig["SystemConfiguration:FactuurOpslag"]; }
        }
        public static bool MaintenanceCheckEnabled
        {
            get{ return InstanceConfig.GetValue<bool>("MaintenanceConfiguration:MaintenanceCheckEnabled"); }
        }
        public static string MaintenanceUrl
        {
            get { return InstanceConfig["MaintenanceConfiguration:MaintenanceUrl"]; }
        }
        public static string MaintenanceUser
        {
            get { return InstanceConfig["MaintenanceConfiguration:MaintenanceUser"]; }
        }
        public static string MaintenancePassword
        {
            get { return InstanceConfig["MaintenanceConfiguration:MaintenancePassword"]; }
        }
        public static string ApplicationName
        {
            get { return InstanceConfig["SystemConfiguration:ApplicationName"]; }
        }
        public static bool SmtpEnabled
        {
            get { return InstanceConfig.GetValue<bool>("SmtpConfiguration:Enabled"); }
        }
        public static string SmtpServer
        {
            get { return InstanceConfig["SmtpConfiguration:Server"]; }
        }
        public static int SmtpPort
        {
            get{ return InstanceConfig.GetValue<int>("SmtpConfiguration:Port"); }
        }
        public static string SmtpUsername
        {
            get { return InstanceConfig["SmtpConfiguration:Username"]; }
        }
        public static string SmtpPassword
        {
            get { return InstanceConfig["SmtpConfiguration:Password"]; }
        }
        public static string SmtpReciever
        {
            get { return InstanceConfig["SmtpConfiguration:Reciever"]; }
        }
        public static string ConnectionString { get; } = InstanceConfig.GetConnectionString("DossierRegistratieConnectionString");
        public static string ShutdownFile { get; } = GetShutdownFilePath();
        public static string GetShutdownFilePath()
        {
            string shutdownFile = InstanceConfig["SystemConfiguration:ShutdownFile"];

            if (!string.IsNullOrEmpty(shutdownFile))
            {
                string directory = Path.GetDirectoryName(shutdownFile);

                try
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    if (File.Exists(shutdownFile))
                    {
                        File.WriteAllText(shutdownFile, string.Empty);
                    }
                    else
                    {
                        using (File.Create(shutdownFile)) { }
                    }
                }
                catch (Exception ex)
                {
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                }

                return shutdownFile;
            }
            return string.Empty;
        }
        public static int ExecuteNonQuery(string query)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public static SqlDataReader ExecuteReader(string query)
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                return command.ExecuteReader();
            }
            catch (Exception ex)
            {
                connection.Close();
                throw ex;
            }
        }
    }
}
