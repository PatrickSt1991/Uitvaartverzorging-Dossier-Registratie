using Microsoft.Office.Interop.Access;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

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
                else
                {
                    Debug.WriteLine("No records found with the provided uitvaartnummer.");
                }
            }
            catch (COMException comEx)
            {
                Debug.WriteLine($"COMException: {comEx.Message}");
                Debug.WriteLine($"Error Code: {comEx.ErrorCode}");
                Debug.WriteLine($"Stack Trace: {comEx.StackTrace}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }

    }
}
