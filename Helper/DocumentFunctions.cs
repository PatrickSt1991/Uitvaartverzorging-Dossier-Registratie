using Dossier_Registratie.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using Outlook = Microsoft.Office.Interop.Outlook;
using Word = Microsoft.Office.Interop.Word;

namespace Dossier_Registratie.Helper
{
    public class DocumentFunctions : ViewModelBase
    {
        public static void PrePrintFile(string filePath)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo(filePath) { UseShellExecute = true },
                EnableRaisingEvents = true
            };

            process.Exited += (sender, e) => PrintFile(filePath);
            process.Start();
        }
        public static void PrintFile(string filePath)
        {
            Word.Application wordApp = new Word.Application();
            Word.Document wordDoc = null;

            try
            {
                wordDoc = wordApp.Documents.Open(filePath, ReadOnly: true, Visible: false);
                wordDoc.Activate();

                wordDoc.PrintOut(
                    Background: false,
                    PrintToFile: false
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during printing: " + ex.Message);
            }
            finally
            {
                if (wordDoc != null)
                {
                    wordDoc.Close(SaveChanges: false);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(wordDoc);
                    wordDoc = null;
                }

                // Quit the Word application
                if (wordApp != null)
                {
                    wordApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
                    wordApp = null;
                }

                // Force garbage collection to clean up
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
        public static void OpenFile(string filePath)
        {
            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }
        public static string PreEmailFile(string filePath)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo(filePath) { UseShellExecute = true },
                EnableRaisingEvents = true
            };

            string fileFolder = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath) + ".pdf";
            string pdfPath = Path.Combine(fileFolder, fileName);

            if (File.Exists(pdfPath))
            {
                File.Delete(pdfPath);
            }

            process.Exited += (sender, e) => SaveWordFileAsPdf(filePath, pdfPath);
            process.Start();
            return pdfPath;
        }
        public static void EmailFile(string filePath)
        {
            Outlook.Application outlookApp = null;
            Outlook.MailItem mailItem = null;
            try
            {
                try
                {
                    outlookApp = (Outlook.Application)Marshal.BindToMoniker("!{0006F03A-0000-0000-C000-000000000046}");
                }
                catch (COMException)
                {
                    outlookApp = new Outlook.Application();
                }

                if (outlookApp == null)
                {
                    MessageBox.Show("Failed to initialize Outlook application.");
                    return;
                }
                mailItem = (Outlook.MailItem)outlookApp.CreateItem(Outlook.OlItemType.olMailItem);
                mailItem.Attachments.Add(filePath);
                mailItem.Display(true);
            }
            catch (COMException comEx)
            {
                MessageBox.Show("COM Exception: " + comEx.Message);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Failed to open Outlook: " + ex.Message);
            }
            finally
            {
                if (mailItem != null)
                {
                    Marshal.ReleaseComObject(mailItem);
                    mailItem = null;
                }
                if (outlookApp != null)
                {
                    Marshal.ReleaseComObject(outlookApp);
                    outlookApp = null;
                }
            }
        }
        public static void PrintFiles(List<string> fileContentList)
        {
            // Initialize Word application object
            Word.Application wordApp = new Word.Application();
            Word.Document wordDoc = null;

            try
            {
                foreach (string filePath in fileContentList)
                {
                    MessageBox.Show(filePath);
                    if (System.IO.File.Exists(filePath))
                    {
                        // Open the Word document
                        wordDoc = wordApp.Documents.Open(filePath, ReadOnly: true, Visible: false);
                        wordDoc.Activate();
                        // Print the document
                        wordDoc.PrintOut(
                            Background: false, // Print synchronously
                            PrintToFile: false // Print directly to the printer
                        );

                        // Close the document without saving changes
                        wordDoc.Close(SaveChanges: false);
                        Marshal.ReleaseComObject(wordDoc);
                        wordDoc = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during printing: " + ex.Message);
            }
            finally
            {
                // Quit the Word application
                if (wordApp != null)
                {
                    wordApp.Quit();
                    Marshal.ReleaseComObject(wordApp);
                    wordApp = null;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
        public static void OpenFiles(List<string> fileContentList)
        {
            foreach (string fileContent in fileContentList)
            {
                Process.Start(new ProcessStartInfo(fileContent) { UseShellExecute = true });
            }
        }
        public static void EmailFiles(List<string> filePaths)
        {
            Outlook.Application outlookApp = null;
            Outlook.MailItem mailItem = null;

            try
            {
                try
                {
                    outlookApp = (Outlook.Application)Marshal.BindToMoniker("!{0006F03A-0000-0000-C000-000000000046}");
                }
                catch (COMException)
                {
                    outlookApp = new Outlook.Application();
                }

                if (outlookApp == null)
                {
                    MessageBox.Show("Failed to initialize Outlook application.");
                    return;
                }
                mailItem = (Outlook.MailItem)outlookApp.CreateItem(Outlook.OlItemType.olMailItem);

                foreach (string filePath in filePaths)
                {
                    mailItem.Attachments.Add(filePath, Outlook.OlAttachmentType.olByValue, Type.Missing, Type.Missing);
                }

                mailItem.Display(true);
            }
            catch (COMException comEx)
            {
                MessageBox.Show("COM Exception: " + comEx.Message);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
            finally
            {
                if (mailItem != null)
                {
                    Marshal.ReleaseComObject(mailItem);
                    mailItem = null;
                }
                if (outlookApp != null)
                {
                    Marshal.ReleaseComObject(outlookApp);
                    outlookApp = null;
                }
            }
        }
        private static void SaveWordFileAsPdf(string wordFilePath, string pdfFilePath)
        {
            Word.Application wordApp = null;
            Word.Document wordDoc = null;

            try
            {
                wordApp = new Word.Application();
                wordApp.Visible = false;
                wordDoc = wordApp.Documents.Open(wordFilePath);

                wordDoc.ExportAsFixedFormat(pdfFilePath, Word.WdExportFormat.wdExportFormatPDF);
                EmailFile(pdfFilePath);
            }
            catch (COMException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                if (wordDoc != null)
                {
                    wordDoc.Close(false);
                    Marshal.ReleaseComObject(wordDoc);
                }

                if (wordApp != null)
                {
                    wordApp.Quit(false);
                    Marshal.ReleaseComObject(wordApp);
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

    }
}
