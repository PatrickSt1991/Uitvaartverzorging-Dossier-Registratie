using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using Dossier_Registratie.ViewModels;
using Microsoft.Office.Interop.Word;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Application = Microsoft.Office.Interop.Word.Application;
using Range = Microsoft.Office.Interop.Word.Range;

namespace Dossier_Registratie.Helper
{
    public class DocumentGenerator : ViewModelBase
    {
        private OverledeneBijlagesModel _bijlageModel;
        public OverledeneBijlagesModel BijlageModel
        {
            get { return _bijlageModel; }
            set { _bijlageModel = value; OnPropertyChanged(nameof(BijlageModel)); }
        }
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        public DocumentGenerator()
        {
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
        }
        public async Task<OverledeneBijlagesModel> UpdateOverdracht(OverdrachtDocument overdracht)
        {
            OverledeneBijlagesModel bijlageModel = new OverledeneBijlagesModel();
            Application app = null;
            Document doc = null;

            try
            {
                app = new Application();
                doc = app.Documents.Open(overdracht.DestinationFile);

                var bookmarks = new Dictionary<string, string>
        {
            { "OverdrachtType", overdracht.OverdrachtType },
            { "OverledeneAanhef", overdracht.OverledeAanhef },
            { "OverledeneNaam", overdracht.OverledeneAchternaam },
            { "OverledeneVoornaam", overdracht.OverledeneVoornaam },
            { "OverledeneDossiernummer", overdracht.UitvaartNummer },
            { "OverdragendePartij", overdracht.OverdragendePartij + " ( " + DataProvider.OrganizationName + " )" }
        };

                foreach (var bookmark in bookmarks)
                {
                    Bookmark bm = doc.Bookmarks[bookmark.Key];
                    Range range = bm.Range;
                    range.Text = bookmark.Value;
                    doc.Bookmarks.Add(bookmark.Key, range);
                }


                var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");

                if (documentData != null && documentData.Length > 0)
                {
                    string tempImagePath = Path.Combine(Path.GetTempPath(), "headerImage.", documentType);
                    File.WriteAllBytes(tempImagePath, documentData);

                    foreach (Section section in doc.Sections)
                    {
                        HeaderFooter header = section.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary];
                        Range headerRange = header.Range;
                        InlineShape headerImage = headerRange.InlineShapes.AddPicture(tempImagePath);
                    }
                    File.Delete(tempImagePath);
                }


                doc.Save();
                doc.Close();
                Marshal.ReleaseComObject(doc);
                doc = null; // Avoid further access

                bijlageModel.DocumentHash = Checksum.GetMD5Checksum(overdracht.DestinationFile);
                bijlageModel.UitvaartId = overdracht.UitvaartId;

                if (overdracht.Updated)
                {
                    bijlageModel.BijlageId = Guid.NewGuid();
                }
                else
                {
                    bijlageModel.BijlageId = overdracht.DocumentId;
                    bijlageModel.IsModified = true;
                }
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                // Ensure COM objects are released and resources cleaned up
                if (doc != null)
                {
                    doc.Close(false); // Close without saving changes; set to true if you want to save
                    Marshal.ReleaseComObject(doc);
                    doc = null; // Avoid further access
                }

                if (app != null)
                {
                    app.Quit();
                    Marshal.ReleaseComObject(app);
                    app = null; // Avoid further access
                }

                // Clean up any remaining COM objects
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return bijlageModel;
        }
        public async Task<OverledeneBijlagesModel> UpdateChecklist(ChecklistDocument checklist, List<ChecklistOpbarenDocument> werknemers)
        {
            OverledeneBijlagesModel bijlageModel = new OverledeneBijlagesModel();
            Application app = null;
            Document doc = null;

            try
            {
                app = new Application();

                // Try to open the document
                try
                {
                    doc = app.Documents.Open(checklist.DestinationFile);
                    if (doc == null)
                    {
                        throw new Exception("Failed to open the document: " + checklist.DestinationFile);
                    }
                }
                catch (Exception ex)
                {
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(new Exception("Error opening document: " + ex.Message));
                    throw;
                }

                string formattedOverledenDate = checklist.OverledenDatum.Split(' ')[0];
                string formattedUitvaartDate = checklist.DatumUitvaart.Split(' ')[0];

                // Create the bookmarks dictionary
                var bookmarks = new Dictionary<string, string>
        {
            { "OverledeneNaam", checklist.VolledigeNaam },
            { "UitvaartDatum", formattedUitvaartDate },
            { "OverlijdensDatum", formattedOverledenDate },
            { "RegistratieNummer", checklist.UitvaartNummer },
            { "Herkomst", checklist.Herkomst },
            { "UitvaartType", checklist.UitvaartType },
            { "UitvaartLeider", checklist.UitvartLeider }
        };

                if (werknemers.Count > 0)
                {
                    int werknemerCount = 0;

                    foreach (var werknemer in werknemers)
                    {
                        werknemerCount++;
                        var bookmarkName = "Verzorging" + werknemerCount;
                        var verzorgerName = werknemer.WerknemerName;

                        if (!bookmarks.ContainsKey(bookmarkName))
                            bookmarks.Add(bookmarkName, verzorgerName);
                    }
                }

                // Null check for doc and doc.Bookmarks
                if (doc != null && doc.Bookmarks != null)
                {
                    foreach (var bookmark in bookmarks)
                    {
                        if (doc.Bookmarks.Exists(bookmark.Key)) // Check if bookmark exists before accessing
                        {
                            Bookmark bm = doc.Bookmarks[bookmark.Key];
                            Range range = bm.Range;
                            range.Text = bookmark.Value; // Update the bookmark's text
                        }
                    }
                }
                else
                {
                    // Log or handle the case where the document or bookmarks are null
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(new Exception("Document or Bookmarks collection is null"));
                }

                var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");

                if (documentData != null && documentData.Length > 0)
                {
                    string tempImagePath = Path.Combine(Path.GetTempPath(), "headerImage.", documentType);
                    File.WriteAllBytes(tempImagePath, documentData);

                    foreach (Section section in doc.Sections)
                    {
                        HeaderFooter header = section.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary];
                        Range headerRange = header.Range;
                        InlineShape headerImage = headerRange.InlineShapes.AddPicture(tempImagePath);
                    }
                    File.Delete(tempImagePath);
                }


                doc.Save();
                doc.Close();
                Marshal.ReleaseComObject(doc);
                doc = null; // Avoid further access

                bijlageModel.DocumentHash = Checksum.GetMD5Checksum(checklist.DestinationFile);

                bijlageModel.UitvaartId = checklist.UitvaartId;
                bijlageModel.DocumentName = "Checklist";

                if (checklist.Updated)
                {
                    bijlageModel.BijlageId = Guid.NewGuid();
                }
                else
                {
                    bijlageModel.BijlageId = checklist.DocumentId;
                    bijlageModel.IsModified = true;
                }
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close(false); // Close without saving changes; set to true if you want to save
                    Marshal.ReleaseComObject(doc);
                    doc = null; // Avoid further access
                }

                if (app != null)
                {
                    app.Quit();
                    Marshal.ReleaseComObject(app);
                    app = null; // Avoid further access
                }

                // Clean up any remaining COM objects
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return bijlageModel;
        }
        public async Task<OverledeneBijlagesModel> UpdateDienst(DienstDocument dienst)
        {
            OverledeneBijlagesModel bijlageModel = new OverledeneBijlagesModel();
            Application app = null;
            Document doc = null;

            try
            {
                app = new Application();
                doc = app.Documents.Open(dienst.DestinationFile);

                var bookmarks = new Dictionary<string, string>
        {
            //{ "AanvraagDienstTe", dienst.AanvraagDienstTe },
            { "datumUitvaart", dienst.DatumUitvaart.ToString("dd-MM-yyyy") },
            { "naamUitvaart", dienst.NaamUitvaart },
            { "locatieUitvaart", dienst.LocatieDienst },
            { "datumDienst", dienst.DatumDienst.ToString("dd-MM-yyyy") },
            { "aanvang", dienst.Aanvang.ToString("HH:mm") },
            { "opdrachtgeverAdres", dienst.OpdrachtgeverAdres },
            { "opdrachtgeverTelefoon", dienst.OpdrachtgeverTelefoon },
            { "naamOpdrachtgever", dienst.OpdrachtgeverNaam }
        };

                foreach (var bookmark in bookmarks)
                {
                    if (doc.Bookmarks.Exists(bookmark.Key)) // Check if bookmark exists before accessing
                    {
                        Bookmark bm = doc.Bookmarks[bookmark.Key];
                        Range range = bm.Range;
                        range.Text = bookmark.Value;
                        doc.Bookmarks.Add(bookmark.Key, range);
                    }
                }

                var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");

                if (documentData != null && documentData.Length > 0)
                {
                    string tempImagePath = Path.Combine(Path.GetTempPath(), "headerImage.", documentType);
                    File.WriteAllBytes(tempImagePath, documentData);

                    foreach (Section section in doc.Sections)
                    {
                        HeaderFooter header = section.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary];
                        Range headerRange = header.Range;
                        InlineShape headerImage = headerRange.InlineShapes.AddPicture(tempImagePath);
                    }
                    File.Delete(tempImagePath);
                }

                doc.Save();
                doc.Close();
                Marshal.ReleaseComObject(doc);
                doc = null; // Avoid further access

                bijlageModel.DocumentHash = Checksum.GetMD5Checksum(dienst.DestinationFile);
                bijlageModel.UitvaartId = dienst.UitvaartId;

                if (dienst.Updated)
                {
                    bijlageModel.BijlageId = Guid.NewGuid();
                }
                else
                {
                    bijlageModel.BijlageId = dienst.DocumentId;
                    bijlageModel.IsModified = true;
                }
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close(false); // Close without saving changes; set to true if you want to save
                    Marshal.ReleaseComObject(doc);
                    doc = null; // Avoid further access
                }

                if (app != null)
                {
                    app.Quit();
                    Marshal.ReleaseComObject(app);
                    app = null; // Avoid further access
                }

                // Clean up any remaining COM objects
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return bijlageModel;
        }
        public async Task<OverledeneBijlagesModel> UpdateDocument(DocumentDocument document)
        {
            OverledeneBijlagesModel bijlageModel = new OverledeneBijlagesModel();
            Application app = null;
            Document doc = null;

            try
            {
                app = new Application();
                doc = app.Documents.Open(document.DestinationFile);

                string OrganizationAdress = $"{DataProvider.OrganizationStreet} {DataProvider.OrganizationHouseNumber}, {DataProvider.OrganizationZipcode} {DataProvider.OrganizationCity}";

                if (!string.IsNullOrEmpty(DataProvider.OrganizationHouseNumberAddition))
                    OrganizationAdress = $"{DataProvider.OrganizationStreet} {DataProvider.OrganizationHouseNumber} {DataProvider.OrganizationHouseNumberAddition}, {DataProvider.OrganizationZipcode} {DataProvider.OrganizationCity}";

                var bookmarks = new Dictionary<string, string>
        {
            { "DocumentType", document.DocumentType },
            { "DocumentUitvaartVerzorger", document.UitvaartVerzorger },
            { "DocumentUitvaartVerzorgerEmail", document.UitvaartVerzorgerEmail },
            { "DocumentGeslachtsnaamOverledene", document.GeslachtsnaamOverledene },
            { "DocumentVoornaamOverledene", document.VoornaamOverledene },
            { "DocumentGeboortedatumOverledene", document.GeboortedatumOverledene.Date.ToString("dd-MM-yyyy") },
            { "DocumentGeboorteplaatsOverledene", document.GeboorteplaatsOverledene },
            { "DocumentWoonplaatsOverledene", document.WoonplaatsOverledene },
            { "DocumentDatumOverlijden", document.DatumOverlijden.Date.ToString("dd-MM-yyyy") },
            { "DocumentGemeenteOverlijden", document.GemeenteOverlijden },
            { "DocumentPlaatsOverlijden", document.PlaatsOverlijden },
            { "DocumentLocatieUitvaart", document.LocatieUitvaart },
            { "DocumentDatumUitvaart", document.DatumUitvaart.Date.ToString("dd-MM-yyyy") },
            { "DocumentOndergetekende", document.UitvaartVerzorger },
            { "OrganisatieNaam1", DataProvider.OrganizationName },
            { "OrganisatieNaam2", DataProvider.OrganizationName },
            { "OrganisatieAdres", OrganizationAdress },
            { "OrganisatieTelefoon", DataProvider.OrganizationPhoneNumber }
        };

                foreach (var bookmark in bookmarks)
                {
                    Bookmark bm = doc.Bookmarks[bookmark.Key];
                    Range range = bm.Range;
                    range.Text = bookmark.Value;
                    doc.Bookmarks.Add(bookmark.Key, range);
                }

                var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");

                if (documentData != null && documentData.Length > 0)
                {
                    string tempImagePath = Path.Combine(Path.GetTempPath(), "headerImage.", documentType);
                    File.WriteAllBytes(tempImagePath, documentData);

                    foreach (Section section in doc.Sections)
                    {
                        HeaderFooter header = section.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary];
                        Range headerRange = header.Range;
                        InlineShape headerImage = headerRange.InlineShapes.AddPicture(tempImagePath);
                    }
                    File.Delete(tempImagePath);
                }


                doc.Save();
                doc.Close();
                Marshal.ReleaseComObject(doc);
                doc = null; // Avoid further access

                bijlageModel.DocumentHash = Checksum.GetMD5Checksum(document.DestinationFile);
                bijlageModel.DocumentUrl = document.DestinationFile;
                bijlageModel.UitvaartId = document.UitvaartId;
                bijlageModel.DocumentName = "Document";

                if (document.Updated)
                {
                    bijlageModel.BijlageId = Guid.NewGuid();
                }
                else
                {
                    bijlageModel.BijlageId = document.DocumentId;
                    bijlageModel.IsModified = true;
                }
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                // Ensure COM objects are released and resources cleaned up
                if (doc != null)
                {
                    doc.Close(false); // Close without saving changes; set to true if you want to save
                    Marshal.ReleaseComObject(doc);
                }

                if (app != null)
                {
                    app.Quit();
                    Marshal.ReleaseComObject(app);
                }

                // Clean up any remaining COM objects
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return bijlageModel;
        }
        public async Task<OverledeneBijlagesModel> UpdateKoffie(KoffieKamerDocument koffieKamer)
        {
            OverledeneBijlagesModel bijlageModel = new OverledeneBijlagesModel();
            Application app = null;
            Document doc = null;

            try
            {
                app = new Application();
                doc = app.Documents.Open(koffieKamer.DestinationFile);

                var bookmarks = new Dictionary<string, string>
        {
            //{ "koffieLocatie", koffieKamer.DienstLocatie },
            { "koffieDatum", koffieKamer.DatumUitvaart.ToString("dd-MM-yyyy") },
            { "koffieNaam", koffieKamer.Opdrachtgever },
            { "Locatie", koffieKamer.DienstLocatie },
            { "Datum",  koffieKamer.DienstDatum.ToString("dd-MM-yyyy") },
            { "Aanvang", koffieKamer.DienstTijd.ToString("HH:mm") },
            { "OpdrachtgeverAdres", koffieKamer.OpdrachtgeverAdres },
            { "OpdrachtgeverTelefoon", koffieKamer.OpdrachtgeverTelefoon },
            { "OpdrachtgeverNaam", koffieKamer.Naam }
        };

                foreach (var bookmark in bookmarks)
                {
                    if (doc.Bookmarks.Exists(bookmark.Key)) // Check if bookmark exists before accessing
                    {
                        Bookmark bm = doc.Bookmarks[bookmark.Key];
                        Range range = bm.Range;
                        range.Text = bookmark.Value;
                        doc.Bookmarks.Add(bookmark.Key, range);
                    }
                }

                var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");

                if (documentData != null && documentData.Length > 0)
                {
                    string tempImagePath = Path.Combine(Path.GetTempPath(), "headerImage.", documentType);
                    File.WriteAllBytes(tempImagePath, documentData);

                    foreach (Section section in doc.Sections)
                    {
                        HeaderFooter header = section.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary];
                        Range headerRange = header.Range;
                        InlineShape headerImage = headerRange.InlineShapes.AddPicture(tempImagePath);
                    }
                    File.Delete(tempImagePath);
                }


                doc.Save();
                doc.Close();
                Marshal.ReleaseComObject(doc);
                doc = null; // Avoid further access

                bijlageModel.DocumentHash = Checksum.GetMD5Checksum(koffieKamer.DestinationFile);
                bijlageModel.UitvaartId = koffieKamer.UitvaartId;

                if (koffieKamer.Updated)
                {
                    bijlageModel.BijlageId = Guid.NewGuid();
                }
                else
                {
                    bijlageModel.BijlageId = koffieKamer.DocumentId;
                    bijlageModel.IsModified = true;
                }
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close(false); // Close without saving changes if it hasn't been done already
                    Marshal.ReleaseComObject(doc);
                    doc = null; // Avoid further access
                }

                if (app != null)
                {
                    app.Quit();
                    Marshal.ReleaseComObject(app);
                    app = null; // Avoid further access
                }

                // Clean up any remaining COM objects
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return bijlageModel;
        }
        public async Task<OverledeneBijlagesModel> UpdateBezittingen(BezittingenDocument bezittingen)
        {
            var bijlageModel = new OverledeneBijlagesModel();
            Application app = null;
            Document doc = null;

            try
            {
                app = new Application();
                doc = app.Documents.Open(bezittingen.DestinationFile);

                var bookmarks = new Dictionary<string, string>
        {
            { "overledeneNaam", bezittingen.OverledeneNaam },
            { "overledeneVoornaam", bezittingen.OverledeneVoornaam },
            { "overledeneGeborenOp", bezittingen.OverledeneGeborenOp.ToString("dd-MM-yyyy") },
            { "overledeneOverledenOp", bezittingen.OverledeneOverledenOp.ToString("dd-MM-yyyy HH:mm") },
            { "overledeneLocatieOpbaring", bezittingen.OverledeneLocatieOpbaring },
            { "overledenePlaatsOverlijden", bezittingen.OverledenePlaatsOverlijden },
            { "overledeneBezittingen", bezittingen.OverledeneBezittingen },
            { "overledeneRetour", bezittingen.OverledeneRetour },
            { "overledeneRelatie", bezittingen.OverledeneRelatie },
            { "opdrachtgeverAdres", bezittingen.OpdrachtgeverAdres},
            { "opdrachtgeverNaamLetters", bezittingen.OpdrachtgeverNaamVoorletters}
        };

                foreach (var bookmark in bookmarks)
                {
                    if (doc.Bookmarks.Exists(bookmark.Key)) // Check if bookmark exists before accessing
                    {
                        var bm = doc.Bookmarks[bookmark.Key];
                        var range = bm.Range;
                        range.Text = bookmark.Value;
                        doc.Bookmarks.Add(bookmark.Key, range);
                    }
                }

                var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");

                if (documentData != null && documentData.Length > 0)
                {
                    string tempImagePath = Path.Combine(Path.GetTempPath(), "headerImage.", documentType);
                    File.WriteAllBytes(tempImagePath, documentData);

                    foreach (Section section in doc.Sections)
                    {
                        HeaderFooter header = section.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary];
                        Range headerRange = header.Range;
                        InlineShape headerImage = headerRange.InlineShapes.AddPicture(tempImagePath);
                    }
                    File.Delete(tempImagePath);
                }


                doc.Save();
                doc.Close();
                Marshal.ReleaseComObject(doc);
                doc = null; // Avoid further access

                bijlageModel.DocumentHash = Checksum.GetMD5Checksum(bezittingen.DestinationFile);
                bijlageModel.UitvaartId = bezittingen.UitvaartId;

                if (bezittingen.Updated)
                {
                    bijlageModel.BijlageId = Guid.NewGuid();
                }
                else
                {
                    bijlageModel.BijlageId = bezittingen.DocumentId;
                    bijlageModel.IsModified = true;
                }
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close(false); // Close without saving changes if it hasn't been done already
                    Marshal.ReleaseComObject(doc);
                    doc = null; // Avoid further access
                }

                if (app != null)
                {
                    app.Quit();
                    Marshal.ReleaseComObject(app);
                    app = null; // Avoid further access
                }

                // Clean up any remaining COM objects
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return bijlageModel;
        }
        public async Task<OverledeneBijlagesModel> UpdateCrematie(CrematieDocument crematie, FactuurInfoCrematie factuur)
        {
            var bijlageModel = new OverledeneBijlagesModel();
            Application app = null;
            Document doc = null;

            try
            {
                app = new Application();
                doc = app.Documents.Open(crematie.DestinationFile);

                string OrganizationAdress = $"{DataProvider.OrganizationStreet} {DataProvider.OrganizationHouseNumber}";

                if (!string.IsNullOrEmpty(DataProvider.OrganizationHouseNumberAddition))
                    OrganizationAdress = $"{DataProvider.OrganizationStreet} {DataProvider.OrganizationHouseNumber} {DataProvider.OrganizationHouseNumberAddition}";


                var bookmarks = new Dictionary<string, string>
        {
            { "OrganisatieNaam", DataProvider.OrganizationName},
            { "OrganisatieStraat", OrganizationAdress},
            { "OrganisatiePostcode", DataProvider.OrganizationZipcode },
            { "OrganisatieTelefoon", DataProvider.OrganizationPhoneNumber },
            { "OrganisatiePlaats", DataProvider.OrganizationCity },
            { "CrematieAanvangstijd", crematie.Aanvangstrijd.ToString("HH:mm") },
            { "CrematieStartAula", crematie.StartAula.ToString("HH:mm") },
            { "CrematieStartKoffie", crematie.StartKoffie.ToString("HH:mm") },
            { "CrematieAulaNaam", crematie.AulaNaam },
            { "CrematieAantalPersonenAula", crematie.AulaPersonen.ToString() },
            { "CrematieLocatie", crematie.CrematieLocatie },
            { "CrematieOpdrachtVoor", crematie.CrematieVoor },
            { "CrematieDatum", crematie.CrematieDatum.ToString("dd-MM-yyyy") },
            { "CrematieDossierNr", crematie.CrematieDossiernummer },
            { "OverledeneNaam", crematie.OverledeneNaam },
            { "OverledeneVoornaam", crematie.OverledeneVoornaam },
            { "OverledeneBurgStaat", crematie.OverledeneBurgStaat },
            { "OverledeneGebDatum", crematie.OverledeneGebDatum.ToString("dd-MM-yyyy") },
            { "OverledeneGebPlaats", crematie.OverledeneGebPlaats },
            { "OverledeneStraat", crematie.OverledeneStraat },
            { "OverledenePostcode", crematie.OverledenePostcode },
            { "OverledeneLevensovertuiging", crematie.OverledeneLevensovertuiging },
            { "OverledeneGeslacht", crematie.OverledeneGeslacht },
            { "OverledeneLeeftijd", crematie.OverledeneLeeftijd },
            { "OverledeneOverlDatum", crematie.OverledeneDatum.ToString("dd-MM-yyyy") },
            { "OverledeneOverlPlaats", crematie.OverledenePlaats },
            { "OverledenePlaats", crematie.OverledeneWoonplaats },
            { "OpdrachtgeverNaam", crematie.OpdrachtgeverNaam },
            { "OpdrachtgeverGeslacht", crematie.OpdrachtgeverGeslacht },
            { "OpdrachtgeverGebDatum", crematie.OpdrachtgeverGebDatum.ToString("dd-MM-yyyy") },
            { "OpdrachtgeverStraat", crematie.OpdrachtgeverStraat },
            { "OpdrachtgeverPostcode", crematie.OpdrachtgeverPostcode },
            { "OpdrachtgeverRelatieTotOverl", crematie.OpdrachtgeverRelatie },
            { "OpdrachtgeverVoornamen", crematie.OpdrachtgeverVoornamen },
            { "OpdrachtgeverTelefoon", crematie.OpdrachtgeverTelefoon },
            { "OpdrachtgeverPlaats", crematie.OpdrachtgeverPlaats },
            { "OpdrachtgeverEmail", crematie.OpdrachtgeverEmail },
            { "Uitvaartverzorger", crematie.Uitvaartverzorger },
            { "Asbestemming", crematie.Asbestemming },
            { "Consumpties", crematie.Consumpties }
        };

                if (factuur.FactuurAdresOverride)
                {
                    bookmarks.Add("FactuuradresNaam", factuur.FactuurAdresNaam);
                    bookmarks.Add("FactuuradresRelatietotoverl", factuur.FactuurAdresRelatie);
                    bookmarks.Add("FactuuradresStraat", factuur.FactuurAdresStraat); //P/A Oostereinde 11
                    bookmarks.Add("FactuuradresPostcode", factuur.FactuurAdresPostcode);
                    bookmarks.Add("FactuuradresGeslacht", factuur.FactuurAdresGeslacht);
                    bookmarks.Add("FactuuradresTelefoon", factuur.FactuurAdresTelefoon); //"0597 412 628"
                    bookmarks.Add("FactuuradresPlaats", factuur.FactuurAdresPlaats);
                }

                foreach (var bookmark in bookmarks)
                {
                    if (doc.Bookmarks.Exists(bookmark.Key)) // Check if bookmark exists before accessing
                    {
                        var bm = doc.Bookmarks[bookmark.Key];
                        var range = bm.Range;
                        range.Text = bookmark.Value;
                        doc.Bookmarks.Add(bookmark.Key, range);
                    }
                }

                var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");

                if (documentData != null && documentData.Length > 0)
                {
                    string tempImagePath = Path.Combine(Path.GetTempPath(), "headerImage.", documentType);
                    File.WriteAllBytes(tempImagePath, documentData);

                    foreach (Section section in doc.Sections)
                    {
                        HeaderFooter header = section.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary];
                        Range headerRange = header.Range;
                        InlineShape headerImage = headerRange.InlineShapes.AddPicture(tempImagePath);
                    }
                    File.Delete(tempImagePath);
                }


                doc.Save();
                doc.Close();
                Marshal.ReleaseComObject(doc);
                doc = null; // Avoid further access

                bijlageModel.DocumentHash = Checksum.GetMD5Checksum(crematie.DestinationFile);
                bijlageModel.UitvaartId = crematie.UitvaartId;

                if (crematie.Updated)
                {
                    bijlageModel.BijlageId = Guid.NewGuid();
                }
                else
                {
                    bijlageModel.BijlageId = crematie.DocumentId;
                    bijlageModel.IsModified = true;
                }
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close(false); // Close without saving changes if it hasn't been done already
                    Marshal.ReleaseComObject(doc);
                    doc = null; // Avoid further access
                }

                if (app != null)
                {
                    app.Quit();
                    Marshal.ReleaseComObject(app);
                    app = null; // Avoid further access
                }

                // Clean up any remaining COM objects
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return bijlageModel;
        }
        public async Task<OverledeneBijlagesModel> UpdateBegrafenis(BegrafenisDocument begrafenis)
        {
            var bijlageModel = new OverledeneBijlagesModel();
            Application app = null;
            Document doc = null;

            try
            {
                string HuurgrafActief = string.Empty;
                string AulaActief = string.Empty;
                string AantalPersonenString = string.Empty;
                app = new Application();
                doc = app.Documents.Open(begrafenis.DestinationFile);

                string OrganizationAdress = $"{DataProvider.OrganizationStreet} {DataProvider.OrganizationHouseNumber}, {DataProvider.OrganizationZipcode} {DataProvider.OrganizationCity}";

                if (!string.IsNullOrEmpty(DataProvider.OrganizationHouseNumberAddition))
                    OrganizationAdress = $"{DataProvider.OrganizationStreet} {DataProvider.OrganizationHouseNumber} {DataProvider.OrganizationHouseNumberAddition}, {DataProvider.OrganizationZipcode} {DataProvider.OrganizationCity}";

                if (begrafenis.SoortGraf == "Huurgraf")
                    HuurgrafActief = "Ja";

                if (!string.IsNullOrEmpty(begrafenis.AulaNaam))
                    AulaActief = "Ja";

                if (begrafenis.AantalPersonen != 0)
                    AantalPersonenString = begrafenis.AantalPersonen.ToString();

                var bookmarks = new Dictionary<string, string>
        {
            { "OpdrachtgeverNaam", begrafenis.NaamOpdrachtgever },
            { "OpdrachtgeverAdres", begrafenis.AdresOpdrachtgever },
            { "BegrafenisDatumTijdUitvaart", begrafenis.DatumUitvaart.ToString("dd-MM-yyyy") + " om " + begrafenis.TijdUitvaart.ToString("HH:mm") },
            { "BegrafenisSoortGraf", begrafenis.SoortGraf },
            { "BegrafenisNummer", begrafenis.NrGraf },
            { "OverledeneNaam", begrafenis.NaamOverledene },
            { "OverledeneVoornamen", begrafenis.VoornamenOverledene },
            { "OverledeneDatumPlaatsGeboorte", begrafenis.DatumGeboorte.ToString("dd-MM-yyyy") + " te " + begrafenis.PlaatsGeboorte },
            { "OverledeneDatumPlaatsOverlijden", begrafenis.DatumOverlijden.ToString("dd-MM-yyyy") + " te " + begrafenis.PlaatsOverlijden },
            { "OverledeneBSN", begrafenis.BsnOverledene },
            { "UitvaartondernemingUitvaartverzorger", begrafenis.UitvaartLeider },
            { "UitvaartondernemingUitvaartverzorgerMail", begrafenis.UitvaartLeiderEmail },
            { "UitvaartondernemingUitvaartverzorgerMob", begrafenis.UitvaartLeiderMobiel },
            { "OrganisatieNaam", DataProvider.OrganizationName },
            { "OrganisatieAdres", OrganizationAdress},
            { "OrganisatieTelefoon", DataProvider.OrganizationPhoneNumber},
            { "BegraafplaatsLocatie", begrafenis.Begraafplaats },
            { "BegraafplaatsKistDalen", begrafenis.KistType },
            { "BegraafplaatsHuurgraf", HuurgrafActief },
            { "DienstAula", AulaActief },
            { "DienstPersonen", begrafenis.AantalPersonen.ToString() }


        };

                foreach (var bookmark in bookmarks)
                {
                    if (doc.Bookmarks.Exists(bookmark.Key)) // Check if bookmark exists before accessing
                    {
                        var bm = doc.Bookmarks[bookmark.Key];
                        var range = bm.Range;
                        range.Text = bookmark.Value;
                        doc.Bookmarks.Add(bookmark.Key, range);
                    }
                    else
                    {
                        Exception ex = new Exception($"Bookmark '{bookmark.Key}' does not exist in the document.");
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    }
                }

                var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");

                if (documentData != null && documentData.Length > 0)
                {
                    string tempImagePath = Path.Combine(Path.GetTempPath(), "headerImage.", documentType);
                    File.WriteAllBytes(tempImagePath, documentData);

                    foreach (Section section in doc.Sections)
                    {
                        HeaderFooter header = section.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary];
                        Range headerRange = header.Range;
                        InlineShape headerImage = headerRange.InlineShapes.AddPicture(tempImagePath);
                    }
                    File.Delete(tempImagePath);
                }


                doc.Save();
                doc.Close();
                Marshal.ReleaseComObject(doc);
                doc = null; // Set to null to prevent further access

                bijlageModel.DocumentHash = Checksum.GetMD5Checksum(begrafenis.DestinationFile);
                bijlageModel.UitvaartId = begrafenis.UitvaartId;

                if (begrafenis.Updated)
                {
                    bijlageModel.BijlageId = Guid.NewGuid();
                }
                else
                {
                    bijlageModel.BijlageId = begrafenis.DocumentId;
                    bijlageModel.IsModified = true;
                }
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close(false); // Ensure document is closed without saving changes
                    Marshal.ReleaseComObject(doc);
                    doc = null; // Set to null to prevent further access
                }

                if (app != null)
                {
                    app.Quit();
                    Marshal.ReleaseComObject(app);
                    app = null; // Set to null to prevent further access
                }

                // Clean up any remaining COM objects
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return bijlageModel;
        }
        public async Task<OverledeneBijlagesModel> UpdateTerugmelding(TerugmeldingDocument terugmelding)
        {
            var bijlageModel = new OverledeneBijlagesModel();
            Application app = null;
            Document doc = null;

            try
            {
                app = new Application();
                doc = app.Documents.Open(terugmelding.DestinationFile);
                string OrganizationAdress = $"{DataProvider.OrganizationStreet} {DataProvider.OrganizationHouseNumber}";

                if (!string.IsNullOrEmpty(DataProvider.OrganizationHouseNumberAddition))
                    OrganizationAdress = $"{DataProvider.OrganizationStreet} {DataProvider.OrganizationHouseNumber} {DataProvider.OrganizationHouseNumberAddition}";

                // Initialize the bookmarks dictionary
                var bookmarks = new Dictionary<string, string>
        {
            { "Uitvaartnummer", terugmelding.Dossiernummer },
            { "naamUitvaartverzorger", terugmelding.Uitvaartverzorger },
            { "emailUitvaartverzorger", terugmelding.UitvaartverzorgerEmail },
            { "polisNummer", terugmelding.Polisnummer },
            { "overledeneAanhef", terugmelding.OverledeneAanhef },
            { "overledeneNaam", terugmelding.OverledeneNaam },
            { "overledeneVoornaam", terugmelding.OverledeneVoornamen },
            { "overledeneAdres", terugmelding.OverledeneAdres },
            { "overledenePostcode", terugmelding.OverledenePostcode },
            { "overledeneWoonplaats", terugmelding.OverledeneWoonplaats },
            { "overledeneGeborenTe", terugmelding.OverledeneGeborenTe },
            { "overledeneOverledenOp", terugmelding.OverledeneOverledenOp.ToString("dd-MM-yyyy") },
            { "overledeneOverledenTe", terugmelding.OverledeneOverledenTe },
            { "overledeneUitvaartDatum", terugmelding.OverledeneUitvaartDatum.ToString("dd-MM-yyyy") },
            { "overledeneTijdstipUitvaart", terugmelding.OverledeneUitvaartTijd.ToString("HH:mm") },
            { "overledeneBegrafenisCrematie", terugmelding.OverledeneType },
            { "overledeneUitvaartTe", terugmelding.OverledeneUitvaartTe },
            { "opdrachtgeverNaam", terugmelding.OpdrachtgeverNaam },
            { "opdrachtgeverAdres", terugmelding.OpdrachtgeverAdres },
            { "opdrachtgeverPostcode", terugmelding.OpdrachtgeverPostcode },
            { "opdrachtgeverPlaats", terugmelding.OpdrachtgeverPlaats },
            { "opdrachtgeverRelatieTotOverledene", terugmelding.OpdrachtgeverRelatie },
            { "opdrachtgeverTelefoonnummer", terugmelding.OpdrachtgeverTelefoon },
            { "organisatieNaam", DataProvider.OrganizationName },
            { "organisatieAdres", OrganizationAdress},
            { "organisatiePostcode", DataProvider.OrganizationZipcode },
            { "organisatiePlaats", DataProvider.OrganizationCity },
            { "organisatieTelefoon", DataProvider.OrganizationPhoneNumber },
            { "organisatieEmail", DataProvider.OrganizationEmail }
        };

                // Update bookmarks in the document
                foreach (var bookmark in bookmarks)
                {
                    if (doc.Bookmarks.Exists(bookmark.Key)) // Check if bookmark exists before accessing
                    {
                        var bm = doc.Bookmarks[bookmark.Key];
                        var range = bm.Range;
                        range.Text = bookmark.Value;
                        doc.Bookmarks.Add(bookmark.Key, range);
                    }
                    else
                    {
                        Exception ex = new Exception($"Bookmark '{bookmark.Key}' does not exist in the document.");
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    }
                }

                var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");

                if (documentData != null && documentData.Length > 0)
                {
                    string tempImagePath = Path.Combine(Path.GetTempPath(), "headerImage.", documentType);
                    File.WriteAllBytes(tempImagePath, documentData);

                    foreach (Section section in doc.Sections)
                    {
                        HeaderFooter header = section.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary];
                        Range headerRange = header.Range;
                        InlineShape headerImage = headerRange.InlineShapes.AddPicture(tempImagePath);
                    }
                    File.Delete(tempImagePath);
                }


                doc.Save();
                doc.Close();
                Marshal.ReleaseComObject(doc);
                doc = null; // Prevent further access

                bijlageModel.DocumentHash = Checksum.GetMD5Checksum(terugmelding.DestinationFile);
                bijlageModel.UitvaartId = terugmelding.UitvaartId;
                bijlageModel.DocumentUrl = terugmelding.DestinationFile;

                if (terugmelding.Updated)
                {
                    bijlageModel.BijlageId = Guid.NewGuid();
                }
                else
                {
                    bijlageModel.BijlageId = terugmelding.DocumentId;
                    bijlageModel.IsModified = true;
                }
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close(false); // Close without saving changes
                    Marshal.ReleaseComObject(doc);
                    doc = null; // Prevent further access
                }

                if (app != null)
                {
                    app.Quit();
                    Marshal.ReleaseComObject(app);
                    app = null; // Prevent further access
                }

                // Clean up any remaining COM objects
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return bijlageModel;
        }
        public async Task<OverledeneBijlagesModel> UpdateTevredenheid(TevredenheidDocument tevredenheid)
        {
            var bijlageModel = new OverledeneBijlagesModel();
            Application app = null;
            Document doc = null;

            try
            {
                app = new Application();
                doc = app.Documents.Open(tevredenheid.DestinationFile);

                var bookmarks = new Dictionary<string, string>
        {
            { "Dossiernummer", tevredenheid.Dossiernummer },
            { "ingevuldDoorAdres", tevredenheid.IngevuldDoorAdres },
            { "ingevuldDoorNaam", tevredenheid.IngevuldDoorNaam },
            { "ingevuldDoorTelefoon", tevredenheid.IngevuldDoorTelefoon },
            { "ingevuldDoorWoonplaats", tevredenheid.IngevuldDoorWoonplaats },
            { "Uitvaartverzorger", tevredenheid.Uitvaartverzorger },
            { "OrganisatieNaam1", DataProvider.OrganizationName },
            { "OrganisatieNaam2", DataProvider.OrganizationName },
            { "OrganisatieNaam3", DataProvider.OrganizationName },
        };

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
                        Exception ex = new Exception($"Bookmark '{bookmark.Key}' does not exist in the document Tevredenheid.");
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    }
                }

                var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");

                if (documentData != null && documentData.Length > 0)
                {
                    string tempImagePath = Path.Combine(Path.GetTempPath(), "headerImage.", documentType);
                    File.WriteAllBytes(tempImagePath, documentData);

                    foreach (Section section in doc.Sections)
                    {
                        HeaderFooter header = section.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary];
                        Range headerRange = header.Range;
                        InlineShape headerImage = headerRange.InlineShapes.AddPicture(tempImagePath);
                    }
                    File.Delete(tempImagePath);
                }


                doc.Save();
                doc.Close();
                Marshal.ReleaseComObject(doc);
                doc = null; // Prevent further access

                bijlageModel.DocumentHash = Checksum.GetMD5Checksum(tevredenheid.DestinationFile);
                bijlageModel.UitvaartId = tevredenheid.UitvaartId;

                if (tevredenheid.Updated)
                {
                    bijlageModel.BijlageId = Guid.NewGuid();
                }
                else
                {
                    bijlageModel.BijlageId = tevredenheid.DocumentId;
                    bijlageModel.IsModified = true;
                }
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close(false); // Close without saving changes
                    Marshal.ReleaseComObject(doc);
                    doc = null; // Prevent further access
                }

                if (app != null)
                {
                    app.Quit();
                    Marshal.ReleaseComObject(app);
                    app = null; // Prevent further access
                }

                // Clean up any remaining COM objects
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return bijlageModel;
        }
        public async Task<OverledeneBijlagesModel> UpdateBloemen(BloemenDocument bloemen)
        {
            var bijlageModel = new OverledeneBijlagesModel();
            Application app = null;
            Document doc = null;

            string lint1 = string.Empty;
            string lint2 = string.Empty;
            string lint3 = string.Empty;
            string lint4 = string.Empty;

            try
            {
                app = new Application();
                doc = app.Documents.Open(bloemen.DestinationFile);

                string OrganizationAdress = $"{DataProvider.OrganizationStreet} {DataProvider.OrganizationHouseNumber}, {DataProvider.OrganizationZipcode} {DataProvider.OrganizationCity}";

                if (!string.IsNullOrEmpty(DataProvider.OrganizationHouseNumberAddition))
                    OrganizationAdress = $"{DataProvider.OrganizationStreet} {DataProvider.OrganizationHouseNumber} {DataProvider.OrganizationHouseNumberAddition}, {DataProvider.OrganizationZipcode} {DataProvider.OrganizationCity}";

                if (!string.IsNullOrEmpty(bloemen.LintJson))
                {
                    var lintTexts = JsonConvert.DeserializeObject<List<string>>(bloemen.LintJson);
                    if (lintTexts != null)
                    {
                        lint1 = lintTexts.ElementAtOrDefault(0) ?? string.Empty;
                        lint2 = lintTexts.ElementAtOrDefault(1) ?? string.Empty;
                        lint3 = lintTexts.ElementAtOrDefault(2) ?? string.Empty;
                        lint4 = lintTexts.ElementAtOrDefault(3) ?? string.Empty;
                    }
                }

                var bookmarks = new Dictionary<string, string>
        {
            { "Leverancier", bloemen.LeverancierNaam },
            { "Uitvaartleider", bloemen.Uitvaartleider },
            { "EmailUitvaartleider", bloemen.EmailUitvaartleider },
            { "NaamOverledene", bloemen.NaamOverledene },
            { "Bloemstuk", bloemen.Bloemstuk },
            { "Lint", bloemen.Lint ? "Ja" : "Nee" },
            { "LintTekst1", lint1 },
            { "LintTekst2", lint2 },
            { "LintTekst3", lint3 },
            { "LintTekst4", lint4 },
            { "Kaartje", bloemen.Kaart ? "Ja" : "Nee" },
            { "BezorgAdres", bloemen.Bezorgadres },
            { "Telefoonnummer", bloemen.Telefoonnummer },
            { "DatumBezorgen", bloemen.DatumBezorgen.ToString("dd-MM-yyyy") },
            { "OrganisatieNaam", DataProvider.OrganizationName },
            { "OrganisatieAdres", OrganizationAdress },
            { "OrganisatieTelefoon", DataProvider.OrganizationPhoneNumber }

        };

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
                        Exception ex = new Exception($"Bookmark '{bookmark.Key}' does not exist in the document Bloemen.");
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    }
                }

                var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");

                if (documentData != null && documentData.Length > 0)
                {
                    string tempImagePath = Path.Combine(Path.GetTempPath(), "headerImage.", documentType);
                    File.WriteAllBytes(tempImagePath, documentData);

                    foreach (Section section in doc.Sections)
                    {
                        HeaderFooter header = section.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary];
                        Range headerRange = header.Range;
                        InlineShape headerImage = headerRange.InlineShapes.AddPicture(tempImagePath);
                    }
                    File.Delete(tempImagePath);
                }


                doc.Save();
                doc.Close();
                Marshal.ReleaseComObject(doc);
                doc = null; // Prevent further access

                bijlageModel.DocumentHash = Checksum.GetMD5Checksum(bloemen.DestinationFile);
                bijlageModel.UitvaartId = bloemen.UitvaartId;

                if (bloemen.Updated)
                {
                    bijlageModel.BijlageId = Guid.NewGuid();
                }
                else
                {
                    bijlageModel.BijlageId = bloemen.DocumentId;
                    bijlageModel.IsModified = true;
                }
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close(false); // Close without saving changes
                    Marshal.ReleaseComObject(doc);
                    doc = null; // Prevent further access
                }

                if (app != null)
                {
                    app.Quit();
                    Marshal.ReleaseComObject(app);
                    app = null; // Prevent further access
                }

                // Clean up any remaining COM objects
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return bijlageModel;
        }
        public async Task<OverledeneBijlagesModel> UpdateAangifte(AangifteDocument aangifte)
        {
            var bijlageModel = new OverledeneBijlagesModel();
            Application app = null;
            Document doc = null;

            try
            {
                app = new Application();
                doc = app.Documents.Open(aangifte.DestinationFile);

                // Helper function to format dates
                string FormatDate(DateTime date, string format)
                    => date != DateTime.MinValue ? date.ToString(format) : string.Empty;

                // Determine bookmark values for spouse/partner based on Burgelijkestaat
                string overledeneNaamEchtgenoot = string.Empty;
                string overledeneVoornamenEchtgenoot = string.Empty;
                string overledeneGeregistreerdpartner = string.Empty;

                if (aangifte.Burgelijkestaat == "Partnerschap")
                {
                    overledeneGeregistreerdpartner = aangifte.VoornamenWederhelft + " " + aangifte.NaamWederhelft;
                }
                else
                {
                    overledeneNaamEchtgenoot = aangifte.NaamWederhelft;
                    overledeneVoornamenEchtgenoot = aangifte.VoornamenWederhelft;
                }

                // Prepare bookmark values
                var bookmarks = new Dictionary<string, string>
        {
            { "overledeneAanhef", aangifte.OverledeneAanhef },
            { "overledeneNaam", aangifte.OverledeneAchternaam },
            { "overledeneVoornamen", aangifte.OverledeneVoornaam },
            { "overledeneGeboorteplaats", aangifte.OverledeneGeboorteplaats },
            { "overledeneGeboortedatum", FormatDate(aangifte.OverledeneGeboortedatum, "dd-MM-yyyy") },
            { "overledeneAdres", aangifte.OverledeneAdres },
            { "overledenePostcode", aangifte.OverledenePostcode },
            { "overledeneWoonplaats", aangifte.OverledeneWoonplaats },
            { "overledeneBSN", aangifte.OverledeneBSN },
            { "overledeneDagOveleden", FormatDate(aangifte.DatumOverlijden, "dd-MM-yyyy") },
            { "overledeneTijdOveleden", FormatDate(aangifte.DatumOverlijden, "HH:mm") },
            { "overledeneAdresOveleden", aangifte.AdresOverlijden },
            { "overledeneNaamEchtgenoot", overledeneNaamEchtgenoot },
            { "overledeneVoornamenEchtgenoot", overledeneVoornamenEchtgenoot },
            { "overledeneGeregistreerdpartner", overledeneGeregistreerdpartner },
            { "overledeneEersteOuder", aangifte.EersteOuder },
            { "overledeneTweedeOuder", aangifte.TweedeOuder },
            { "overledeneGehuwdGeweestMet", aangifte.GehuwdGeweestMet },
            { "overledeneWeduweVan", aangifte.WeduwenaarVan },
            { "overledeneAantalKinderen", aangifte.AantalKinderen.ToString() },
            { "overledeneAantalKinderenMinderjarig", aangifte.AantalKinderenMinderjarig.ToString() },
            { "overledeneAantalKinderenOverleden", aangifte.AantalKinderenWaarvanOverleden.ToString() },
            { "aangeverNaam", aangifte.AangeverNaam },
            { "aangeverPlaats", aangifte.AangeverPlaats },
            { "erfgenaamNaam", aangifte.ErfgenaamVolledigeNaam },
            { "erfgenaamStraat", aangifte.ErfgenaamStraat },
            { "erfgenaamPostcode", aangifte.ErfgenaamPostcode },
            { "erfgenaamWoonplaats", aangifte.ErfgenaamWoonplaats },
            { "uitvaartDatum", FormatDate(aangifte.DatumUitvaart, "dd-MM-yyyy") },
            { "uitvaartTijd", FormatDate(aangifte.DatumUitvaart, "HH:mm") },
            { "uitvaartLocatie", aangifte.LocatieUitvaart },
            { "uitvaartType", aangifte.TypeUitvaart },
            { "uitvaartSchouwarts", aangifte.Schouwarts },
            { "uitvaartUBS", aangifte.UBS },
            { "uitvaartnummer", aangifte.UitvaartNummer }
        };

                // Update bookmarks in the document
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
                        Exception ex = new Exception($"Bookmark '{bookmark.Key}' does not exist in the document Aangifte.");
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    }
                }

                var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");

                if (documentData != null && documentData.Length > 0)
                {
                    string tempImagePath = Path.Combine(Path.GetTempPath(), "headerImage.", documentType);
                    File.WriteAllBytes(tempImagePath, documentData);

                    foreach (Section section in doc.Sections)
                    {
                        HeaderFooter header = section.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary];
                        Range headerRange = header.Range;
                        InlineShape headerImage = headerRange.InlineShapes.AddPicture(tempImagePath);
                    }
                    File.Delete(tempImagePath);
                }


                doc.Save();
                doc.Close();
                Marshal.ReleaseComObject(doc);
                doc = null; // Prevent further access

                // Populate the result model
                bijlageModel.DocumentHash = Checksum.GetMD5Checksum(aangifte.DestinationFile);
                bijlageModel.UitvaartId = aangifte.UitvaartId;

                if (aangifte.Updated)
                {
                    bijlageModel.BijlageId = Guid.NewGuid();
                }
                else
                {
                    bijlageModel.BijlageId = aangifte.DocumentId;
                    bijlageModel.IsModified = true;
                }
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close(false); // Close without saving changes
                    Marshal.ReleaseComObject(doc);
                    doc = null; // Prevent further access
                }

                if (app != null)
                {
                    app.Quit();
                    Marshal.ReleaseComObject(app);
                    app = null; // Prevent further access
                }

                // Clean up any remaining COM objects
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return bijlageModel;
        }
    }
}
