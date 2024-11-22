using Dossier_Registratie.ViewModels;
using Microsoft.Office.Interop.Access;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Dossier_Registratie.Helper
{
    public class AccessFormHelper
    {
        public static void OpenAccessFormWithFilter(string uitvaartnummer, string applicationPath)
        {
            Application accessApp = new Application();

            try
            {
                accessApp.OpenCurrentDatabase(applicationPath);

                accessApp.Visible = true;

                string whereCondition = $"[Uitvaartnummer] = {uitvaartnummer}";
                string recordCountQuery = $"SELECT Count(*) FROM [Data] WHERE [Uitvaartnummer] = {uitvaartnummer}";

                var recordset = accessApp.CurrentDb().OpenRecordset(recordCountQuery);
                int recordCount = (int)recordset.Fields[0].Value;
                recordset.Close();

                if (recordCount > 0)
                {
                    accessApp.DoCmd.OpenForm("invoeren", (AcFormView)0, "", whereCondition, (AcFormOpenDataMode)0, 0);
                    var myForm = accessApp.Forms["invoeren"];

                    myForm.GetType().InvokeMember("FilterOn", BindingFlags.SetProperty, null, myForm, new object[] { false });
                    myForm.GetType().InvokeMember("FilterOn", BindingFlags.SetProperty, null, myForm, new object[] { true });
                }
            }
            catch (COMException comEx)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(comEx);
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
        }

    }
}
