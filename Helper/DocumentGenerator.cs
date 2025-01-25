using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using Dossier_Registratie.ViewModels;
using Microsoft.Office.Interop.Word;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Application = Microsoft.Office.Interop.Word.Application;
using Range = Microsoft.Office.Interop.Word.Range;
using Task = System.Threading.Tasks.Task;

namespace Dossier_Registratie.Helper
{
    [SupportedOSPlatform("windows")]
    public class DocumentGenerator : ViewModelBase
    {
        private OverledeneBijlagesModel? _bijlageModel;
        public OverledeneBijlagesModel? BijlageModel
        {
            get { return _bijlageModel; }
            set { _bijlageModel = value; OnPropertyChanged(nameof(BijlageModel)); }
        }
        private readonly MiscellaneousAndDocumentOperations miscellaneousRepository;
        public DocumentGenerator()
        {
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
        }

        public async Task<OverledeneBijlagesModel> UpdateOverdracht(OverdrachtDocument overdracht)
        {
            var bijlageModel = new OverledeneBijlagesModel();
            Application? app = null;
            Document? doc = null;

            try
            {
                app = new Application();
                doc = await DocumentFunctions.OpenDocumentAsync(app, overdracht.DestinationFile);
                if (doc == null) return bijlageModel;

                var bookmarks = CreateOverdrachtBookmarks(overdracht);
                DocumentFunctions.UpdateBookmarks(doc, bookmarks);

                var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");
                if (documentData != null)
                    DocumentFunctions.AddImageToDocumentHeaders(doc, documentData, documentType, 1.94f, 9.7f);

                DocumentFunctions.SaveAndCloseDocument(doc);
                doc = null;

                bijlageModel = DocumentFunctions.GenerateBijlageModel(overdracht);
            }
            catch (Exception ex)
            {
                await ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                DocumentFunctions.CleanupResources(app, doc);
            }

            return bijlageModel;
        }
        public async Task<OverledeneBijlagesModel> UpdateDienst(DienstDocument dienst)
        {
            var bijlageModel = new OverledeneBijlagesModel();
            Application? app = null;
            Document? doc = null;

            try
            {
                app = new Application();
                doc = await DocumentFunctions.OpenDocumentAsync(app, dienst.DestinationFile);
                if (doc == null) return bijlageModel;

                string opdrachtgeverAdresFull = DocumentFunctions.CreateOpdrachtgeverAdres(dienst);

                var bookmarks = CreateDienstBookmarks(dienst, opdrachtgeverAdresFull);
                DocumentFunctions.UpdateBookmarks(doc, bookmarks);

                var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");
                if (documentData != null && documentData.Length > 0)
                {
                    try
                    {
                        DocumentFunctions.AddImageToDocumentHeaders(doc, documentData, documentType, 1.94f, 9.7f);
                    }
                    catch (Exception ex)
                    {
                        await ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    }
                }

                DocumentFunctions.SaveAndCloseDocument(doc);
                doc = null;

                bijlageModel = DocumentFunctions.GenerateBijlageModel(dienst);
            }
            catch (Exception ex)
            {
                await ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                DocumentFunctions.CleanupResources(app, doc);
            }

            return bijlageModel;
        }
        public async Task<OverledeneBijlagesModel> UpdateDocument(DocumentDocument document)
        {
            var bijlageModel = new OverledeneBijlagesModel();
            Application? app = null;
            Document? doc = null;

            try
            {
                app = new Application();
                doc = await DocumentFunctions.OpenDocumentAsync(app, document.DestinationFile);
                if (doc == null) return bijlageModel;

                string OrganizationAdress = DocumentFunctions.GenerateOrganizationAddress();

                var bookmarks = CreateDocumentBookmarks(document, OrganizationAdress);
                DocumentFunctions.UpdateBookmarks(doc, bookmarks);

                var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");
                if (documentData != null && documentData.Length > 0)
                {
                    try
                    {
                        DocumentFunctions.AddImageToDocumentHeaders(doc, documentData, documentType, 1.94f, 9.7f);
                    }
                    catch (Exception ex)
                    {
                        await ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    }
                }

                DocumentFunctions.SaveAndCloseDocument(doc);
                doc = null;

                bijlageModel = DocumentFunctions.GenerateBijlageModel(document);
            }
            catch (Exception ex)
            {
                await ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                DocumentFunctions.CleanupResources(app, doc);
            }

            return bijlageModel;
        }
        public async Task<OverledeneBijlagesModel> UpdateKoffie(KoffieKamerDocument koffieKamer)
        {
            var bijlageModel = new OverledeneBijlagesModel();
            Application? app = null;
            Document? doc = null;

            try
            {
                app = new Application();
                doc = await DocumentFunctions.OpenDocumentAsync(app, koffieKamer.DestinationFile);
                if (doc == null) return bijlageModel;

                var bookmarks = CreateKoffieKamerBookmarks(koffieKamer);

                DocumentFunctions.UpdateBookmarks(doc, bookmarks);

                var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");
                if (documentData != null && documentData.Length > 0)
                {
                    try
                    {
                        DocumentFunctions.AddImageToDocumentHeaders(doc, documentData, documentType, 1.94f, 9.7f);
                    }
                    catch (Exception ex)
                    {
                        await ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    }
                }

                DocumentFunctions.SaveAndCloseDocument(doc);
                doc = null;

                bijlageModel = DocumentFunctions.GenerateBijlageModel(koffieKamer);
            }
            catch (Exception ex)
            {
                await ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                DocumentFunctions.CleanupResources(app, doc);
            }

            return bijlageModel;
        }
        public async Task<OverledeneBijlagesModel> UpdateBezittingen(BezittingenDocument bezittingen)
        {
            var bijlageModel = new OverledeneBijlagesModel();
            Application? app = null;
            Document? doc = null;

            try
            {
                app = new Application();
                doc = await DocumentFunctions.OpenDocumentAsync(app, bezittingen.DestinationFile);
                if (doc == null) return bijlageModel;

                var bookmarks = CreateBezittingenBookmarks(bezittingen);

                DocumentFunctions.UpdateBookmarks(doc, bookmarks);

                var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");
                if (documentData != null && documentData.Length > 0)
                {
                    try
                    {
                        DocumentFunctions.AddImageToDocumentHeaders(doc, documentData, documentType, 1.94f, 9.7f);
                    }
                    catch (Exception ex)
                    {
                        await ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    }
                }

                DocumentFunctions.SaveAndCloseDocument(doc);
                doc = null;

                bijlageModel = DocumentFunctions.GenerateBijlageModel(bezittingen);
            }
            catch (Exception ex)
            {
                await ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                DocumentFunctions.CleanupResources(app, doc);
            }

            return bijlageModel;
        }
        public async Task<OverledeneBijlagesModel> UpdateCrematie(CrematieDocument crematie, FactuurInfoCrematie factuur)
        {
            var bijlageModel = new OverledeneBijlagesModel();
            Application? app = null;
            Document? doc = null;

            try
            {
                if (crematie.DestinationFile != null)
                {
                    app = new Application();
                    doc = await DocumentFunctions.OpenDocumentAsync(app, crematie.DestinationFile);
                    if (doc == null) return bijlageModel;

                    string organizationAdress = DocumentFunctions.GenerateOrganizationAddress();

                    var bookmarks = CreateCrematieBookmarks(crematie, organizationAdress);

                    if (factuur != null && factuur.FactuurAdresOverride)
                    {
                        var factuurBookmarks = AddFactuurBookmarks(factuur);
                        foreach (var factuurMark in factuurBookmarks)
                            bookmarks[factuurMark.Key] = factuurMark.Value;
                    }

                    DocumentFunctions.UpdateBookmarks(doc, bookmarks);

                    var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");
                    if (documentData != null)
                        DocumentFunctions.AddImageToDocumentHeaders(doc, documentData, documentType, 1.94f, 9.7f);

                    DocumentFunctions.SaveAndCloseDocument(doc);
                    doc = null;

                    bijlageModel = DocumentFunctions.GenerateBijlageModel(crematie);
                }
            }
            catch (Exception ex)
            {
                await ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                DocumentFunctions.CleanupResources(app, doc);
            }

            return bijlageModel;
        }
        public async Task<OverledeneBijlagesModel> UpdateBegrafenis(BegrafenisDocument begrafenis)
        {
            var bijlageModel = new OverledeneBijlagesModel();
            Application? app = null;
            Document? doc = null;

            try
            {
                app = new Application();
                doc = await DocumentFunctions.OpenDocumentAsync(app, begrafenis.DestinationFile);
                if (doc == null) return bijlageModel;

                string huurgrafActief = (begrafenis.SoortGraf == "Huurgraf") ? "Ja" : string.Empty;
                string aulaActief = !string.IsNullOrEmpty(begrafenis.AulaNaam) ? "Ja" : string.Empty;
                string organizationAdress = DocumentFunctions.GenerateOrganizationAddress();

                var bookmarks = CreateBegrafenisBookmarks(begrafenis, huurgrafActief, aulaActief, organizationAdress);
                DocumentFunctions.UpdateBookmarks(doc, bookmarks);

                var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");
                if (documentData != null)
                    DocumentFunctions.AddImageToDocumentHeaders(doc, documentData, documentType, 1.94f, 9.7f);

                DocumentFunctions.SaveAndCloseDocument(doc);
                doc = null;

                bijlageModel = DocumentFunctions.GenerateBijlageModel(begrafenis);
            }
            catch (Exception ex)
            {
                await ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                DocumentFunctions.CleanupResources(app, doc);
            }

            return bijlageModel;
        }
        public async Task<OverledeneBijlagesModel> UpdateTerugmelding(TerugmeldingDocument terugmelding)
        {
            Application? app = null;
            Document? doc = null;

            try
            {
                string fileName = Path.GetFileName(terugmelding.DestinationFile);
                string extractedVerzekeringName = fileName.Split('_')[0];

                var verzekeringData = JsonConvert.DeserializeObject<List<PolisVerzekering>>(terugmelding.Polisnummer);
                if (verzekeringData == null)
                {
                    throw new Exception("No verzekering data found.");
                }

                var verzekering = verzekeringData.FirstOrDefault(v =>
                    string.Equals(v.VerzekeringName, extractedVerzekeringName, StringComparison.Ordinal));

                var validPolissen = verzekering.PolisInfoList?
                    .Where(p => !string.IsNullOrEmpty(p.PolisNr))
                    .ToList();

                if (!validPolissen.Any())
                {
                    throw new Exception($"No valid polissen found for verzekering {verzekering.VerzekeringName}.");
                }

                string documentName = $"{verzekering.VerzekeringName}_Terugmelding.docx";
                string destinationFile = Path.Combine(Path.GetDirectoryName(terugmelding.DestinationFile), documentName);
                Debug.WriteLine($"Processing document: {terugmelding.DestinationFile}");
                app = new Application();
                doc = await DocumentFunctions.OpenDocumentAsync(app, terugmelding.DestinationFile);
                if (doc == null)
                {
                    Debug.WriteLine($"Document not found: {terugmelding.DestinationFile}");
                    throw new Exception($"Failed to open document {terugmelding.DestinationFile}.");
                }

                var bookmarks = CreateTerugmeldingBookmarks(terugmelding);

                bookmarks["polisNummer"] = string.Join(", ", validPolissen
                    .Where(p => !string.IsNullOrEmpty(p.PolisNr))
                    .Select(p => p.PolisNr));


                DocumentFunctions.UpdateBookmarks(doc, bookmarks);

                var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");
                if (documentData != null)
                    DocumentFunctions.AddImageToDocumentHeaders(doc, documentData, documentType, 1.94f, 9.7f);

                DocumentFunctions.SaveAndCloseDocument(doc);
                doc = null;
                Debug.WriteLine($"Document successfully created: {destinationFile}");
                return new OverledeneBijlagesModel
                {
                    DocumentUrl = destinationFile,
                    DocumentName = documentName,
                };
            }
            catch (Exception ex)
            {
                await ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                throw;
            }
            finally
            {
                DocumentFunctions.CleanupResources(app, doc);
            }
        }
        public async Task<OverledeneBijlagesModel> UpdateTevredenheid(TevredenheidDocument tevredenheid)
        {
            var bijlageModel = new OverledeneBijlagesModel();
            Application? app = null;
            Document? doc = null;

            try
            {
                if(tevredenheid.DestinationFile != null)
                {
                    app = new Application();
                    doc = await DocumentFunctions.OpenDocumentAsync(app, tevredenheid.DestinationFile);
                    if (doc == null) return bijlageModel;

                    var bookmarks = CreateTevredenheidBookmarks(tevredenheid);
                    DocumentFunctions.UpdateBookmarks(doc, bookmarks);

                    var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");
                    if (documentData != null)
                        DocumentFunctions.AddImageToDocumentHeaders(doc, documentData, documentType, 1.94f, 9.7f);

                    DocumentFunctions.SaveAndCloseDocument(doc);
                    doc = null;

                    bijlageModel = DocumentFunctions.GenerateBijlageModel(tevredenheid);
                }
            }
            catch (Exception ex)
            {
                await ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                DocumentFunctions.CleanupResources(app, doc);
            }

            return bijlageModel;
        }
        public async Task<OverledeneBijlagesModel> UpdateBloemen(BloemenDocument bloemen)
        {
            var bijlageModel = new OverledeneBijlagesModel();
            Application? app = null;
            Document? doc = null;

            try
            {
                app = new Application();
                doc = await DocumentFunctions.OpenDocumentAsync(app, bloemen.DestinationFile);
                if (doc == null) return bijlageModel;

                var bookmarks = CreateBloemenBookmarks(bloemen);
                DocumentFunctions.UpdateBookmarks(doc, bookmarks);

                var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");
                if (documentData != null)
                    DocumentFunctions.AddImageToDocumentHeaders(doc, documentData, documentType, 1.94f, 9.7f);

                DocumentFunctions.SaveAndCloseDocument(doc);
                doc = null;

                bijlageModel = DocumentFunctions.GenerateBijlageModel(bloemen);
            }
            catch (Exception ex)
            {
                await ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                DocumentFunctions.CleanupResources(app, doc);
            }

            return bijlageModel;
        }
        public async Task<OverledeneBijlagesModel> UpdateChecklist(ChecklistDocument checklist, List<ChecklistOpbarenDocument> werknemers)
        {
            var bijlageModel = new OverledeneBijlagesModel();
            Application? app = null;
            Document? doc = null;

            try
            {
                app = new Application();
                doc = await DocumentFunctions.OpenDocumentAsync(app, checklist.DestinationFile);
                if (doc == null) return bijlageModel;

                var bookmarks = CreateChecklistBookmarks(checklist, werknemers);
                DocumentFunctions.UpdateBookmarks(doc, bookmarks);

                var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");
                if (documentData != null)
                {
                    DocumentFunctions.AddImageToDocumentHeaders(doc, documentData, documentType, 1.94f, 9.7f);
                }

                DocumentFunctions.SaveAndCloseDocument(doc);
                doc = null;

                bijlageModel = DocumentFunctions.GenerateBijlageModel(checklist);
            }
            catch (Exception ex)
            {
                await ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                DocumentFunctions.CleanupResources(app, doc);
            }

            return bijlageModel;
        }
        public async Task<OverledeneBijlagesModel> UpdateAangifte(AangifteDocument aangifte)
        {
            var bijlageModel = new OverledeneBijlagesModel();
            Application? app = null;
            Document? doc = null;

            try
            {
                app = new Application();
                doc = await DocumentFunctions.OpenDocumentAsync(app, aangifte.DestinationFile);
                if (doc == null) return bijlageModel;

                var bookmarks = CreateAangifteBookmarks(aangifte);
                DocumentFunctions.UpdateBookmarks(doc, bookmarks);

                var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");
                if (documentData != null)
                    DocumentFunctions.AddImageToDocumentHeaders(doc, documentData, documentType, 1.94f, 9.7f);

                DocumentFunctions.SaveAndCloseDocument(doc);
                doc = null;

                bijlageModel = DocumentFunctions.GenerateBijlageModel(aangifte);

            }
            catch (Exception ex)
            {
                await ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                DocumentFunctions.CleanupResources(app, doc);
            }

            return bijlageModel;
        }
        public async Task<OverledeneBijlagesModel> UpdateAkte(AkteDocument akte)
        {
            Application? app = null;
            Document? doc = null;

            try
            {
                string extractedVerzekeringName = Path.GetFileNameWithoutExtension(akte.DestinationFile).Split('_')[1];

                var verzekeringData = JsonConvert.DeserializeObject<List<PolisVerzekering>>(akte.VerzekeringInfo);
                if (verzekeringData == null || !verzekeringData.Any())
                    throw new Exception("No verzekering data found.");

                var matchingVerzekeringen = verzekeringData
                    .Where(v => string.Equals(v.VerzekeringName, extractedVerzekeringName, StringComparison.Ordinal))
                    .ToList();

                if (!matchingVerzekeringen.Any())
                    throw new Exception($"No verzekering found with the name '{extractedVerzekeringName}'.");

                var validPolissen = matchingVerzekeringen
                    .SelectMany(v => v.PolisInfoList ?? Enumerable.Empty<Polis>())
                    .Where(p => !string.IsNullOrEmpty(p.PolisNr) && !string.IsNullOrEmpty(p.PolisBedrag))
                    .ToList();

                if (!validPolissen.Any())
                    throw new Exception($"No valid polissen found for verzekering '{extractedVerzekeringName}'.");

                string documentName = $"{extractedVerzekeringName}_AkteVanCessie.docx";
                string destinationFile = Path.Combine(Path.GetDirectoryName(akte.DestinationFile) ?? string.Empty, documentName);

                app = new Application();
                doc = await DocumentFunctions.OpenDocumentAsync(app, akte.DestinationFile);
                if (doc == null)
                    throw new Exception($"Failed to open document {akte.DestinationFile}.");

                Debug.WriteLine(extractedVerzekeringName);
                Debug.WriteLine(akte.DestinationFile);
                var bookmarks = CreateAkteBookmarks(akte, extractedVerzekeringName);

                if (doc.Bookmarks.Exists("PolisTable"))
                {
                    var range = doc.Bookmarks["PolisTable"].Range;
                    DocumentFunctions.AddPolisTableToAkteDocument(doc, validPolissen, range);
                }

                DocumentFunctions.UpdateBookmarks(doc, bookmarks);

                var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");
                if (documentData != null)
                    DocumentFunctions.AddImageToDocumentHeaders(doc, documentData, documentType, 1.94f, 9.7f);

                DocumentFunctions.SaveAndCloseDocument(doc);
                doc = null;

                return new OverledeneBijlagesModel
                {
                    DocumentUrl = destinationFile,
                    DocumentName = documentName,
                };
            }
            catch (Exception ex)
            {
                await ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                throw;
            }
            finally
            {
                DocumentFunctions.CleanupResources(app, doc);
            }
        }

        private static Dictionary<string, string> CreateAkteBookmarks(AkteDocument akteContent, string verzekeringName)
        {
            return new Dictionary<string, string>
    {
        { "verzekeringMaatschappij", verzekeringName },
        { "ondergetekendeNaamEnVoorletters", akteContent.OpdrachtgeverNaam ?? string.Empty },
        { "ondergetekendeAdres", akteContent.OpdrachtgeverAdres ?? string.Empty },
        { "ondergetekendeRelatieTotVerzekende", akteContent.OpdrachtgeverRelatie ?? string.Empty },
        { "verzekerdeGeslotenOpHetLevenVan", akteContent.GeslotenOpHetLevenVan ?? string.Empty },
        { "verzekerdeGeborenOp", akteContent.OverledenGeboorteDatum.ToString("dd-MM-yyyy") },
        { "verzekerdeOverledenOp", akteContent.OverledenOpDatum.ToString("dd-MM-yyyy") },
        { "verzekerdeAdres", akteContent.OverledenOpAdres ?? string.Empty },
        { "vermeldingNummer", Globals.UitvaartCode },
        { "OrganisatieNaam", DataProvider.OrganizationName },
        { "OrganisatieIban", DataProvider.OrganizationIban }
    };
        }
        private static Dictionary<string, string> CreateOverdrachtBookmarks(OverdrachtDocument overdracht)
        {
            return new Dictionary<string, string>
    {
        { "OverdrachtType", overdracht.OverdrachtType },
        { "OverledeneAanhef", overdracht.OverledeAanhef },
        { "OverledeneNaam", overdracht.OverledeneAchternaam },
        { "OverledeneVoornaam", overdracht.OverledeneVoornaam },
        { "OverledeneDossiernummer", overdracht.UitvaartNummer },
        { "OverdragendePartij", overdracht.OverdragendePartij + " ( " + DataProvider.OrganizationName + " )" }
    };
        }
        private static Dictionary<string, string> CreateDienstBookmarks(DienstDocument dienst, string opdrachtgeverAdresFull)
        {
            return new Dictionary<string, string>
    {
        { "datumUitvaart", dienst.DatumUitvaart.ToString("dd-MM-yyyy") },
        { "naamUitvaart", dienst.NaamUitvaart },
        { "locatieUitvaart", dienst.LocatieDienst },
        { "datumDienst", dienst.DatumDienst.ToString("dd-MM-yyyy") },
        { "aanvang", dienst.Aanvang.ToString("HH:mm") },
        { "opdrachtgeverAdres", opdrachtgeverAdresFull },
        { "opdrachtgeverTelefoon", dienst.OpdrachtgeverTelefoon },
        { "naamOpdrachtgever", dienst.OpdrachtgeverNaam }
    };
        }
        private static Dictionary<string, string> CreateDocumentBookmarks(DocumentDocument document, string OrganizationAdress)
        {
            

            return new Dictionary<string, string>
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
        { "OrganisatieTelefoon", DataProvider.OrganizationPhoneNumber },
        { "Uitvaartnummer", document.UitvaartNummer }
    };
        }
        private static Dictionary<string, string> CreateKoffieKamerBookmarks(KoffieKamerDocument koffieKamer)
        {
            return new Dictionary<string, string>
    {
        { "koffieDatum", koffieKamer.DatumUitvaart.ToString("dd-MM-yyyy") },
        { "koffieNaam", koffieKamer.Naam },
        { "Locatie", koffieKamer.DienstLocatie },
        { "Datum", koffieKamer.DienstDatum.ToString("dd-MM-yyyy") },
        { "Aanvang", koffieKamer.DienstTijd.ToString("HH:mm") },
        { "OpdrachtgeverAdres", koffieKamer.OpdrachtgeverAdres },
        { "OpdrachtgeverTelefoon", koffieKamer.OpdrachtgeverTelefoon },
        { "OpdrachtgeverNaam", koffieKamer.Opdrachtgever }
    };
        }
        private static Dictionary<string, string> CreateBezittingenBookmarks(BezittingenDocument bezittingen)
        {
            return new Dictionary<string, string>
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
        { "opdrachtgeverAdres", bezittingen.OpdrachtgeverAdres },
        { "opdrachtgeverNaamLetters", bezittingen.OpdrachtgeverNaamVoorletters }
    };
        }
        private static Dictionary<string, string> CreateCrematieBookmarks(CrematieDocument crematie, string organizationAdress)
        {
            return new Dictionary<string, string>
    {
        { "OrganisatieNaam", DataProvider.OrganizationName },
        { "OrganisatieStraat", organizationAdress },
        { "OrganisatiePostcode", DataProvider.OrganizationZipcode },
        { "OrganisatieTelefoon", DataProvider.OrganizationPhoneNumber },
        { "OrganisatiePlaats", DataProvider.OrganizationCity },
        { "CrematieAanvangstijd", crematie.Aanvangstrijd.ToString("HH:mm") },
        { "CrematieStartAula", crematie.StartAula.ToString("HH:mm") },
        { "CrematieStartKoffie", crematie.StartKoffie.ToString("HH:mm") },
        { "CrematieAulaNaam", crematie.AulaNaam ?? string.Empty },
        { "CrematieAantalPersonenAula", crematie.AulaPersonen.ToString() },
        { "CrematieLocatie", crematie.CrematieLocatie ?? string.Empty },
        { "CrematieOpdrachtVoor", crematie.CrematieVoor ?? string.Empty },
        { "CrematieDatum", crematie.CrematieDatum.ToString("dd-MM-yyyy") },
        { "CrematieDossierNr", crematie.CrematieDossiernummer ?? string.Empty },
        { "OverledeneNaam", crematie.OverledeneNaam ?? string.Empty },
        { "OverledeneVoornaam", crematie.OverledeneVoornaam ?? string.Empty },
        { "OverledeneBurgStaat", crematie.OverledeneBurgStaat ?? string.Empty },
        { "OverledeneGebDatum", crematie.OverledeneGebDatum.ToString("dd-MM-yyyy") },
        { "OverledeneGebPlaats", crematie.OverledeneGebPlaats ?? string.Empty },
        { "OverledeneStraat", crematie.OverledeneStraat ?? string.Empty },
        { "OverledenePostcode", crematie.OverledenePostcode ?? string.Empty },
        { "OverledeneLevensovertuiging", crematie.OverledeneLevensovertuiging ?? string.Empty },
        { "OverledeneGeslacht", crematie.OverledeneGeslacht ?? string.Empty },
        { "OverledeneLeeftijd", crematie.OverledeneLeeftijd ?? string.Empty },
        { "OverledeneOverlDatum", crematie.OverledeneDatum.ToString("dd-MM-yyyy") },
        { "OverledeneOverlPlaats", crematie.OverledenePlaats ?? string.Empty },
        { "OverledenePlaats", crematie.OverledeneWoonplaats ?? string.Empty },
        { "OpdrachtgeverNaam", crematie.OpdrachtgeverNaam ?? string.Empty },
        { "OpdrachtgeverGeslacht", crematie.OpdrachtgeverGeslacht ?? string.Empty },
        { "OpdrachtgeverGebDatum", crematie.OpdrachtgeverGebDatum.ToString("dd-MM-yyyy") },
        { "OpdrachtgeverStraat", crematie.OpdrachtgeverStraat ?? string.Empty },
        { "OpdrachtgeverPostcode", crematie.OpdrachtgeverPostcode ?? string.Empty },
        { "OpdrachtgeverRelatieTotOverl", crematie.OpdrachtgeverRelatie ?? string.Empty },
        { "OpdrachtgeverVoornamen", crematie.OpdrachtgeverVoornamen ?? string.Empty },
        { "OpdrachtgeverTelefoon", crematie.OpdrachtgeverTelefoon ?? string.Empty },
        { "OpdrachtgeverPlaats", crematie.OpdrachtgeverPlaats ?? string.Empty },
        { "OpdrachtgeverEmail", crematie.OpdrachtgeverEmail ?? string.Empty },
        { "Uitvaartverzorger", crematie.Uitvaartverzorger ?? string.Empty },
        { "Asbestemming", crematie.Asbestemming ?? string.Empty },
        { "Consumpties", crematie.Consumpties ?? string.Empty }
    };
        }
        private static Dictionary<string, string> AddFactuurBookmarks(FactuurInfoCrematie factuur)
        {
            return new Dictionary<string, string>
            {
                { "FactuuradresNaam", factuur.FactuurAdresNaam ?? string.Empty },
                { "FactuuradresRelatietotoverl", factuur.FactuurAdresRelatie ?? string.Empty },
                { "FactuuradresStraat", factuur.FactuurAdresStraat ?? string.Empty },
                { "FactuuradresPostcode", factuur.FactuurAdresPostcode ?? string.Empty },
                { "FactuuradresGeslacht", factuur.FactuurAdresGeslacht ?? string.Empty },
                { "FactuuradresTelefoon", factuur.FactuurAdresTelefoon ?? string.Empty },
                { "FactuuradresPlaats", factuur.FactuurAdresPlaats ?? string.Empty },
            };
        }
        private static Dictionary<string, string> CreateBegrafenisBookmarks(BegrafenisDocument begrafenis, string huurgrafActief, string aulaActief, string organizationAdress)
        {
            return new Dictionary<string, string>
    {
        { "OpdrachtgeverNaam", begrafenis.NaamOpdrachtgever },
        { "OpdrachtgeverAdres", begrafenis.AdresOpdrachtgever },
        { "BegrafenisDatumTijdUitvaart", $"{begrafenis.DatumUitvaart:dd-MM-yyyy} om {begrafenis.TijdUitvaart:HH:mm}" },
        { "BegrafenisSoortGraf", begrafenis.SoortGraf },
        { "BegrafenisNummer", begrafenis.NrGraf },
        { "OverledeneNaam", begrafenis.NaamOverledene },
        { "OverledeneVoornamen", begrafenis.VoornamenOverledene },
        { "OverledeneDatumPlaatsGeboorte", $"{begrafenis.DatumGeboorte:dd-MM-yyyy} te {begrafenis.PlaatsGeboorte}" },
        { "OverledeneDatumPlaatsOverlijden", $"{begrafenis.DatumOverlijden:dd-MM-yyyy} te {begrafenis.PlaatsOverlijden}" },
        { "OverledeneBSN", begrafenis.BsnOverledene },
        { "UitvaartondernemingUitvaartverzorger", begrafenis.UitvaartLeider },
        { "UitvaartondernemingUitvaartverzorgerMail", begrafenis.UitvaartLeiderEmail },
        { "UitvaartondernemingUitvaartverzorgerMob", begrafenis.UitvaartLeiderMobiel },
        { "OrganisatieNaam", DataProvider.OrganizationName },
        { "OrganisatieAdres", organizationAdress },
        { "OrganisatieTelefoon", DataProvider.OrganizationPhoneNumber },
        { "BegraafplaatsLocatie", begrafenis.Begraafplaats },
        { "BegraafplaatsKistDalen", begrafenis.KistType },
        { "BegraafplaatsHuurgraf", huurgrafActief },
        { "DienstAula", aulaActief },
        { "DienstPersonen", begrafenis.AantalPersonen.ToString() }
    };
        }
        private static Dictionary<string, string> CreateTerugmeldingBookmarks(TerugmeldingDocument terugmelding)
        {
            string OrganizationAdress = $"{DataProvider.OrganizationStreet} {DataProvider.OrganizationHouseNumber}";

            if (!string.IsNullOrEmpty(DataProvider.OrganizationHouseNumberAddition))
                OrganizationAdress = $"{DataProvider.OrganizationStreet} {DataProvider.OrganizationHouseNumber} {DataProvider.OrganizationHouseNumberAddition}";

            return new Dictionary<string, string>
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
        { "organisatieAdres", OrganizationAdress },
        { "organisatiePostcode", DataProvider.OrganizationZipcode },
        { "organisatiePlaats", DataProvider.OrganizationCity },
        { "organisatieTelefoon", DataProvider.OrganizationPhoneNumber },
        { "organisatieEmail", DataProvider.OrganizationEmail }
    };
        }
        private static Dictionary<string, string> CreateTevredenheidBookmarks(TevredenheidDocument tevredenheid)
        {
            return new Dictionary<string, string>
    {
        { "Dossiernummer", tevredenheid.Dossiernummer ?? string.Empty },
        { "ingevuldDoorAdres", tevredenheid.IngevuldDoorAdres ?? string.Empty },
        { "ingevuldDoorNaam", tevredenheid.IngevuldDoorNaam ?? string.Empty },
        { "ingevuldDoorTelefoon", tevredenheid.IngevuldDoorTelefoon ?? string.Empty },
        { "ingevuldDoorWoonplaats", tevredenheid.IngevuldDoorWoonplaats ?? string.Empty },
        { "Uitvaartverzorger", tevredenheid.Uitvaartverzorger ?? string.Empty },
        { "OrganisatieNaam1", DataProvider.OrganizationName },
        { "OrganisatieNaam2", DataProvider.OrganizationName },
        { "OrganisatieNaam3", DataProvider.OrganizationName }
    };
        }
        private static Dictionary<string, string> CreateBloemenBookmarks(BloemenDocument bloemen)
        {
            static string FormatDate(DateTime date, string format) => date != DateTime.MinValue ? date.ToString(format) : string.Empty;

            var lintTexts = DocumentFunctions.ParseLintTexts(bloemen.LintJson);
            var lint1 = lintTexts.ElementAtOrDefault(0) ?? string.Empty;
            var lint2 = lintTexts.ElementAtOrDefault(1) ?? string.Empty;
            var lint3 = lintTexts.ElementAtOrDefault(2) ?? string.Empty;
            var lint4 = lintTexts.ElementAtOrDefault(3) ?? string.Empty;

            string organizationAddress = DocumentFunctions.GenerateOrganizationAddress();

            return new Dictionary<string, string>
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
        { "DatumBezorgen", FormatDate(bloemen.DatumBezorgen, "dd-MM-yyyy") },
        { "OrganisatieNaam", DataProvider.OrganizationName },
        { "OrganisatieAdres", organizationAddress },
        { "OrganisatieTelefoon", DataProvider.OrganizationPhoneNumber }
    };
        }
        private static Dictionary<string, string> CreateChecklistBookmarks(ChecklistDocument checklist, List<ChecklistOpbarenDocument> werknemers)
        {
            static string FormatDate(string dateTimeString) => dateTimeString.Split(' ')[0];

            var bookmarks = new Dictionary<string, string>
    {
        { "OverledeneNaam", checklist.VolledigeNaam },
        { "UitvaartDatum", FormatDate(checklist.DatumUitvaart) },
        { "OverlijdensDatum", FormatDate(checklist.OverledenDatum) },
        { "RegistratieNummer", checklist.UitvaartNummer },
        { "Herkomst", checklist.Herkomst },
        { "UitvaartType", checklist.UitvaartType },
        { "UitvaartLeider", checklist.UitvartLeider }
    };

            if (werknemers.Count > 0)
            {
                var werknemerBookmarks = werknemers
                    .Select((werknemer, index) => new
                    {
                        BookmarkName = "Verzorging" + (index + 1),
                        VerzorgerName = werknemer.WerknemerName
                    })
                    .Where(x => !bookmarks.ContainsKey(x.BookmarkName))
                    .ToList();

                foreach (var item in werknemerBookmarks)
                {
                    bookmarks.Add(item.BookmarkName, item.VerzorgerName);
                }
            }

            return bookmarks;
        }
        private static Dictionary<string, string> CreateAangifteBookmarks(AangifteDocument aangifte)
        {
            static string FormatDate(DateTime date, string format) => date != DateTime.MinValue ? date.ToString(format) : string.Empty;

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

            if (aangifte.Burgelijkestaat == "Partnerschap")
            {
                bookmarks["overledeneGeregistreerdpartner"] = $"{aangifte.VoornamenWederhelft} {aangifte.NaamWederhelft}";
            }
            else
            {
                bookmarks["overledeneNaamEchtgenoot"] = aangifte.NaamWederhelft;
                bookmarks["overledeneVoornamenEchtgenoot"] = aangifte.VoornamenWederhelft;
            }

            return bookmarks;
        }
    }
}
