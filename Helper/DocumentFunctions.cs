﻿using Dossier_Registratie.ViewModels;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows;
using Outlook = Microsoft.Office.Interop.Outlook;
using Word = Microsoft.Office.Interop.Word;
using System.Linq;
using Dossier_Registratie.Models;
using System.Threading.Tasks;
using Application = Microsoft.Office.Interop.Word.Application;
using Task = System.Threading.Tasks.Task;
using Range = Microsoft.Office.Interop.Word.Range;
using Newtonsoft.Json;

namespace Dossier_Registratie.Helper
{
    [SupportedOSPlatform("windows")]
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
            Word.Application? wordApp = null;
            Word.Document? wordDoc = null;

            try
            {
                wordApp = new Word.Application();
                wordDoc = wordApp.Documents.Open(filePath, ReadOnly: true, Visible: false);
                wordDoc.Activate();

                wordDoc.PrintOut(
                    Background: false,
                    PrintToFile: false
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during printing: {ex.Message}");
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex).ConfigureAwait(false);
            }
            finally
            {
                if (wordDoc != null)
                {
                    wordDoc.Close(SaveChanges: false);
                    Marshal.ReleaseComObject(wordDoc);
                }

                if (wordApp != null)
                {
                    wordApp.Quit();
                    Marshal.ReleaseComObject(wordApp);
                }

                GC.WaitForPendingFinalizers();
            }
        }
        public static void OpenFile(string filePath)
        {
            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }
        public static string PreEmailFile(string filePath)
        {
            string pdfPath = string.Empty;

            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo(filePath) { UseShellExecute = true },
                    EnableRaisingEvents = true
                };

                string fileFolder = Path.GetDirectoryName(filePath) ?? string.Empty;
                string fileName = Path.GetFileNameWithoutExtension(filePath) + ".pdf";
                pdfPath = Path.Combine(fileFolder, fileName);

                if (File.Exists(pdfPath))
                    File.Delete(pdfPath);

                process.Exited += (sender, e) =>
                {
                    try
                    {
                        SaveWordFileAsPdf(filePath, pdfPath);
                    }
                    catch (Exception ex)
                    {
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex).ConfigureAwait(false);
                        MessageBox.Show($"Error saving file as PDF: {ex.Message}");
                    }
                };

                process.Start();
            }
            catch (FileNotFoundException fex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(fex).ConfigureAwait(false);
                MessageBox.Show("The specified file was not found.");
            }
            catch (UnauthorizedAccessException aex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(aex).ConfigureAwait(false);
                MessageBox.Show("You do not have the required permissions to access this file.");
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex).ConfigureAwait(false);
                MessageBox.Show($"An unexpected error occurred: {ex.Message}");
            }

            return pdfPath;

        }
        public static void EmailFile(string filePath)
        {
            Outlook.Application? outlookApp = null;
            Outlook.MailItem? mailItem = null;
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
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(comEx).ConfigureAwait(false);
                MessageBox.Show("COM Exception: " + comEx.Message);
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex).ConfigureAwait(false);
                MessageBox.Show("Failed to open Outlook: " + ex.Message);
            }
            finally
            {
                if (mailItem != null)
                    Marshal.ReleaseComObject(mailItem);

                if (outlookApp != null)
                    Marshal.ReleaseComObject(outlookApp);
            }
        }
        public static void PrintFiles(List<string> fileContentList)
        {
            Word.Application? wordApp = new();
            Document? wordDoc = null;

            try
            {
                foreach (var filePath in fileContentList.Where(filePath => File.Exists(filePath)))
                {
                    wordDoc = wordApp.Documents.Open(filePath, ReadOnly: true, Visible: false);
                    wordDoc.Activate();
                    wordDoc.PrintOut(
                                            Background: false,
                                            PrintToFile: false
                                        );
                    wordDoc.Close(SaveChanges: false);
                    Marshal.ReleaseComObject(wordDoc);
                }
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex).ConfigureAwait(false);
                MessageBox.Show("Error during printing: " + ex.Message);
            }
            finally
            {
                if (wordApp != null)
                {
                    wordApp.Quit();
                    Marshal.ReleaseComObject(wordApp);
                }

                GC.WaitForPendingFinalizers();
            }
        }
        public static void OpenFiles(List<string> fileContentList)
        {
            foreach (string fileContent in fileContentList)
                Process.Start(new ProcessStartInfo(fileContent) { UseShellExecute = true });
        }
        public static void EmailFiles(List<string> filePaths)
        {
            Outlook.Application? outlookApp = null;
            Outlook.MailItem? mailItem = null;

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
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(comEx).ConfigureAwait(false);
                MessageBox.Show("COM Exception: " + comEx.Message);
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex).ConfigureAwait(false);
                MessageBox.Show("Exception: " + ex.Message);
            }
            finally
            {
                if (mailItem != null)
                    Marshal.ReleaseComObject(mailItem);

                if (outlookApp != null)
                    Marshal.ReleaseComObject(outlookApp);
            }
        }
        private static void SaveWordFileAsPdf(string wordFilePath, string pdfFilePath)
        {
            Word.Application? wordApp = null;
            Document? wordDoc = null;

            try
            {
                wordApp = new Word.Application
                {
                    Visible = false
                };
                wordDoc = wordApp.Documents.Open(wordFilePath);

                wordDoc.ExportAsFixedFormat(pdfFilePath, WdExportFormat.wdExportFormatPDF);
                EmailFile(pdfFilePath);
            }
            catch (COMException ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex).ConfigureAwait(false);
                MessageBox.Show("Error: " + ex.Message);
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

                GC.WaitForPendingFinalizers();
            }
        }
        public static async Task<Document?> OpenDocumentAsync(Application app, string filePath)
        {
            try
            {
                var doc = app.Documents.Open(filePath);
                if (doc == null)
                {
                    await ReportErrorAsync(new Exception("Failed to open document: " + filePath));
                }
                return doc;
            }
            catch (Exception ex)
            {
                await ReportErrorAsync(ex);
                return null;
            }
        }
        public static void UpdateBookmarks(Document doc, Dictionary<string, string> bookmarks)
        {
            if (doc.Bookmarks == null)
                throw new Exception("Document or Bookmarks collection is null");

            foreach (var bookmark in bookmarks)
            {
                if (doc.Bookmarks.Exists(bookmark.Key))
                {
                    var bm = doc.Bookmarks[bookmark.Key];
                    var range = bm.Range;
                    range.Text = bookmark.Value;
                    doc.Bookmarks.Add(bookmark.Key, range);
                }
                else
                {
                    throw new Exception($"Bookmark '{bookmark.Key}' does not exist in the document.");
                }
            }
        }
        public static void AddImageToDocumentHeaders(Document doc, byte[] imageData, string imageExtension, float heightCm, float widthCm)
        {
            string? tempImagePath = null;

            try
            {
                tempImagePath = Path.Combine(Path.GetTempPath(), $"headerImage.{imageExtension}");
                string tempDir = Path.GetDirectoryName(tempImagePath) ?? string.Empty;


                if (string.IsNullOrEmpty(tempDir) || !Directory.Exists(tempDir))
                    throw new DirectoryNotFoundException($"Temporary directory not found: {tempDir}");

                File.WriteAllBytes(tempImagePath, imageData);

                foreach (Section section in doc.Sections)
                {
                    HeaderFooter header = section.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary];
                    Range headerRange = header.Range;
                    InlineShape picture = headerRange.InlineShapes.AddPicture(tempImagePath);

                    picture.Height = heightCm * 28.35f; // 1 cm = 28.35 points
                    picture.Width = widthCm * 28.35f;

                    headerRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                }
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex).ConfigureAwait(false);
                throw;
            }
            finally
            {
                if (!string.IsNullOrEmpty(tempImagePath) && File.Exists(tempImagePath))
                {
                    try
                    {
                        File.Delete(tempImagePath);
                    }
                    catch (Exception deleteEx)
                    {
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(deleteEx).ConfigureAwait(false);
                    }
                }
            }
        }
        public static void SaveAndCloseDocument(Document? doc)
        {
            if (doc == null) return;

            doc.Save();
            doc.Close();
            Marshal.ReleaseComObject(doc);
        }
        public static void CleanupResources(Application? app, Document? doc)
        {
            if (doc != null)
            {
                doc.Close(false);
                Marshal.ReleaseComObject(doc);
            }

            if (app != null)
            {
                app.Quit();
                Marshal.ReleaseComObject(app);
            }

            GC.WaitForPendingFinalizers();
        }
        public static OverledeneBijlagesModel GenerateBijlageModel<T>(T document) where T : IGenericDocument
        {
            return new OverledeneBijlagesModel
            {
                DocumentHash = Checksum.GetMD5Checksum(document.DestinationFile),
                UitvaartId = document.UitvaartId,
                BijlageId = document.Updated ? Guid.NewGuid() : document.DocumentId,
                IsModified = !document.Updated
            };
        }
        public static string GenerateOrganizationAddress()
        {
            var address = $"{DataProvider.OrganizationStreet} {DataProvider.OrganizationHouseNumber}, {DataProvider.OrganizationZipcode} {DataProvider.OrganizationCity}";
            if (!string.IsNullOrEmpty(DataProvider.OrganizationHouseNumberAddition))
            {
                address = $"{DataProvider.OrganizationStreet} {DataProvider.OrganizationHouseNumber} {DataProvider.OrganizationHouseNumberAddition}, {DataProvider.OrganizationZipcode} {DataProvider.OrganizationCity}";
            }
            return address;
        }
        public static List<string> ParseLintTexts(string? lintJson)
        {
            return string.IsNullOrEmpty(lintJson)
                ? new List<string>()
                : JsonConvert.DeserializeObject<List<string>>(lintJson) ?? new List<string>();
        }
        public static string CreateOpdrachtgeverAdres(DienstDocument dienst)
        {
            return string.Join(", ", new[]
            {
                dienst.OpdrachtgeverAdres,
                dienst.OpdrachtgeverPostcode,
                dienst.OpdrachtgeverPlaats
            }.Where(part => !string.IsNullOrEmpty(part)));
        }
        private static async Task ReportErrorAsync(Exception ex)
        {
            await ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
        }
    }
}
