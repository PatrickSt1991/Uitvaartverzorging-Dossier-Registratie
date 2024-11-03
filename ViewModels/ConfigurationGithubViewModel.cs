using CommunityToolkit.Mvvm.Input;
using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Dossier_Registratie.Views;
using Octokit;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Dossier_Registratie.ViewModels
{
    public class ConfigurationGithubViewModel : ViewModelBase
    {
        private static ConfigurationGithubViewModel _gitHubInstance;
        public ICommand CreateFeatureOrIssue { get; }
        public ICommand LoadIssuesCommand { get; }
        public ICommand ReturnToStartCommand { get; }
        private readonly GitHubClient _githubClient;
        private ObservableCollection<GitHubModel> _issues;
        private string _selectedState;
        private bool _bugChecked;
        private bool _featureChecked;
        private bool _omschrijvingEnabled;
        private bool _onderwerpEnabled;
        private bool _sendToGithubEnabled;
        private string _infoText;
        private string _onderwerpText;
        private string _omschrijvingText;
        private string _issueType;
        private string _issueLabel;
        private string _inzender;
        private int _closedIssues;
        private int _openIssues;
        private int _totalIssues;

        public static ConfigurationGithubViewModel GitHubInstance
        {
            get
            {
                if (_gitHubInstance == null)
                {
                    _gitHubInstance = new ConfigurationGithubViewModel();
                }
                return _gitHubInstance;
            }
        }
        public bool BugChecked
        {
            get { return _bugChecked; }
            set { _bugChecked = value; OnPropertyChanged(nameof(BugChecked)); UpdateInfoText(); }
        }
        public bool FeatureChecked
        {
            get { return _featureChecked; }
            set { _featureChecked = value; OnPropertyChanged(nameof(FeatureChecked)); UpdateInfoText(); }
        }
        public bool OmschrijvingEnabled
        {
            get { return _omschrijvingEnabled; }
            set { _omschrijvingEnabled = value; OnPropertyChanged(nameof(OmschrijvingEnabled)); }
        }
        public bool OnderwerpEnabled
        {
            get { return _onderwerpEnabled; }
            set { _onderwerpEnabled = value; OnPropertyChanged(nameof(OnderwerpEnabled)); }
        }
        public bool SendToGithubEnabled
        {
            get { return _sendToGithubEnabled; }
            set { _sendToGithubEnabled = value; OnPropertyChanged(nameof(SendToGithubEnabled)); }
        }
        public string InfoText
        {
            get { return _infoText; }
            set { _infoText = value; OnPropertyChanged(nameof(InfoText)); }
        }
        public string OnderwerpText
        {
            get { return _onderwerpText; }
            set { _onderwerpText = value; OnPropertyChanged(nameof(OnderwerpText)); }
        }
        public string OmschrijvingText
        {
            get { return _omschrijvingText; }
            set { _omschrijvingText = value; OnPropertyChanged(nameof(OmschrijvingText)); }
        }
        public string IssueType
        {
            get { return _issueType; }
            set { _issueType = value; OnPropertyChanged(nameof(IssueType)); }
        }
        public string IssueLabel
        {
            get { return _issueLabel; }
            set { _issueLabel = value; OnPropertyChanged(nameof(IssueLabel)); }
        }
        public string Inzender
        {
            get { return _inzender; }
            set { _inzender = value; OnPropertyChanged(nameof(Inzender)); }
        }
        public int ClosedIssues
        {
            get { return _closedIssues; }
            set { _closedIssues = value; OnPropertyChanged(nameof(ClosedIssues)); }
        }
        public int OpenIssues
        {
            get { return _openIssues; }
            set { _openIssues = value; OnPropertyChanged(nameof(OpenIssues)); }
        }
        public int TotalIssues
        {
            get { return _totalIssues; }
            set { _totalIssues = value; OnPropertyChanged(nameof(TotalIssues)); }
        }
        public ObservableCollection<GitHubModel> Issues
        {
            get => _issues;
            set
            {
                _issues = value;
                OnPropertyChanged(nameof(Issues));
            }
        }
        public ObservableCollection<string> States { get; }
        public string SelectedState
        {
            get => _selectedState;
            set
            {
                _selectedState = value;
                OnPropertyChanged(nameof(SelectedState));

                LoadIssuesCommand.Execute(null);
            }
        }

        private string owner = DataProvider.GithubOwner;
        private string repo = DataProvider.GithubRepo;
        private string product = DataProvider.GithubProduct;
        private string token = DataProvider.GithubKey;

        private void UpdateInfoText()
        {
            if (BugChecked)
            {
                InfoText = "**Beschrijf de fout**\r\nEen duidelijke en beknopte beschrijving van wat het probleem is.\r\n\r\n**Hoe te reproduceren**\r\nStappen om het gedrag te reproduceren:\r\n1. Ga naar '...'\r\n2. Klik op '....'\r\n3. Vul in '....'\r\n\r\n**Verwacht gedrag**\r\nEen duidelijke en beknopte beschrijving van wat u verwachtte dat er zou gebeuren.\r\n\r\n**Screenshots**\r\nVoeg indien van toepassing schermafbeeldingen toe om uw probleem te helpen verklaren.\r\n\r\n**Aanvullende context**\r\nVoeg hier eventuele andere context over het probleem toe.\r\n";
                OnderwerpEnabled = true;
                OmschrijvingEnabled = true;
                SendToGithubEnabled = true;
                IssueType = "[BUG]";
                IssueLabel = "Probleem";

            }
            else if (FeatureChecked)
            {
                InfoText = "**Gewenste Toevoeging**\r\nStel een idee voor dit project voor.\r\n\r\n**Is uw functieverzoek gerelateerd aan een probleem? Beschrijf alsjeblieft.**\r\nEen duidelijke en beknopte omschrijving van wat het probleem is. Ex. Ik ben altijd gefrustreerd als [...]\r\n\r\n**Beschrijf de gewenste oplossing**\r\nEen duidelijke en beknopte omschrijving van wat u wilt dat er gebeurt.\r\n\r\n**Beschrijf alternatieven die u heeft overwogen**\r\nEen duidelijke en beknopte beschrijving van eventuele alternatieve oplossingen of functies die u heeft overwogen.\r\n\r\n**Aanvullende context**\r\nVoeg hier eventuele andere context of schermafbeeldingen over het functieverzoek toe.";
                OnderwerpEnabled = true;
                OmschrijvingEnabled = true;
                SendToGithubEnabled = true;
                IssueType = "[VERZOEK]";
                IssueLabel = "Request";
            }
        }
        public async Task SendStacktraceToGithubRepo(Exception ex)
        {
            if (DataProvider.GithubEnabled)
            {
                var newIssue = new NewIssue($"[Code Error] - {ex.GetType().Name}")
                {
                    Body = $@"Message : {ex.Message}

StackTrace: {ex.StackTrace} 

Veroorzaker: {Environment.UserName}

Datum Tijd: {DateTime.Now}"
                };

                newIssue.Labels.Add("Code Error");

                try
                {
                    var issue = await _githubClient.Issue.Create(owner, repo, newIssue);
                }
                catch (Exception exPost)
                {
                    MessageBox.Show($"Error tijdens het aanmaken van de melding: {exPost.Message}");
                }
            }
        }
        public async Task SendToGithubRepo(ConfigurationGithubViewModel configuratieGithubViewModel)
        {
            if (DataProvider.GithubEnabled)
            {
                var newIssue = new NewIssue(configuratieGithubViewModel.IssueType + " " + configuratieGithubViewModel.OnderwerpText)
                {
                    Body = $"Omschrijving:\r\n{configuratieGithubViewModel.OmschrijvingText}\r\n\r\nDoor: {Environment.UserName}"
                };
                newIssue.Labels.Add(configuratieGithubViewModel.IssueLabel);
                try
                {
                    var issue = await _githubClient.Issue.Create(owner, repo, newIssue);
                    SendNotificationEmailAsync(configuratieGithubViewModel.IssueType, issue.Number);
                    CustomMessageBox.CustomMessageBoxResult result = CustomMessageBox.Show("Melding aangemaakt", "Melding is met succes aangemaakt.", $"Meldingsnummer {issue.Number}", "Terug", "Blijven");
                    if (result == CustomMessageBox.CustomMessageBoxResult.Continue)
                    {
                        IntAggregator.Transmit(0);
                    }
                    else if (result == CustomMessageBox.CustomMessageBoxResult.Stop)
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error tijdens het aanmaken van de melding: {ex.Message}");
                }
            }
        }
        public ConfigurationGithubViewModel()
        {

            _githubClient = new GitHubClient(new ProductHeaderValue(product));
            _githubClient.Credentials = new Credentials(token);


            CreateFeatureOrIssue = new ViewModelCommand(ExecuteCreateFeatureOrIssue);
            ReturnToStartCommand = new RelayCommand(() => IntAggregator.Transmit(0));
            Inzender = Environment.UserName;
            Issues = new ObservableCollection<GitHubModel>();
            States = new ObservableCollection<string> { "Open", "Closed" };
            LoadIssuesCommand = new RelayCommand(async () => await LoadIssuesAsync());
            SelectedState = "Open";
        }
        public async Task LoadIssuesAsync()
        {
            try
            {
                var issueState = SelectedState == "Closed" ? ItemState.Closed : ItemState.Open;

                // Fetch all issues with the selected state
                var issues = await _githubClient.Issue.GetAllForRepository(owner, repo, new RepositoryIssueRequest
                {
                    State = (ItemStateFilter)issueState
                });

                // Fetch all open issues to get the count
                var openIssues = await _githubClient.Issue.GetAllForRepository(owner, repo, new RepositoryIssueRequest
                {
                    State = ItemStateFilter.Open
                });

                // Fetch all closed issues to get the count
                var closedIssues = await _githubClient.Issue.GetAllForRepository(owner, repo, new RepositoryIssueRequest
                {
                    State = ItemStateFilter.Closed
                });

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    // Update the issue list
                    Issues.Clear();

                    foreach (var issue in issues)
                    {
                        Issues.Add(new GitHubModel
                        {
                            Id = issue.Number,
                            Title = issue.Title,
                            State = !string.IsNullOrEmpty(issue.State.StringValue) ? char.ToUpper(issue.State.StringValue[0]) + issue.State.StringValue.Substring(1) : string.Empty
                        });
                    }

                    TotalIssues = openIssues.Count + closedIssues.Count;
                    OpenIssues = openIssues.Count;
                    ClosedIssues = closedIssues.Count;

                });
            }
            catch (Exception ex)
            {
                // Ensure this runs on the UI thread as well
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    Issues.Add(new GitHubModel
                    {
                        Id = 0,
                        Title = "Iets is fout gegaan met het ophalen van de informatie.",
                        State = "Error"
                    });
                    OpenIssues = 0;
                    ClosedIssues = 0;
                    TotalIssues = 0;
                });
            }
        }
        public async void ExecuteCreateFeatureOrIssue(object obj)
        {
            if ((!string.IsNullOrEmpty(OnderwerpText)) && (!string.IsNullOrEmpty(OmschrijvingText)))
            {
                await SendToGithubRepo(this); // Await the async call
                BugChecked = false;
                FeatureChecked = false;
                OnderwerpText = null;
                OmschrijvingText = null;
                SendToGithubEnabled = false;
                OnderwerpEnabled = false;
                OmschrijvingEnabled = false;

                await LoadIssuesAsync(); // Await the async load issues call
            }
            else
            {
                if ((string.IsNullOrEmpty(OnderwerpText)) && string.IsNullOrEmpty(OmschrijvingText))
                {
                    MessageBox.Show("Je hebt geen onderwerp en omschrijving ingevoerd.");
                }
                else if (string.IsNullOrEmpty(OnderwerpText))
                {
                    MessageBox.Show("Je hebt geen onderwerp ingevoerd.");
                }
                else if (string.IsNullOrEmpty(OmschrijvingText))
                {
                    MessageBox.Show("Je hebt geen omschrijving ingevoerd.");
                }
            }
        }
        public static async Task SendNotificationEmailAsync(string issueType, int issueNummer)
        {
            string smtpServer = "smtp.gmail.com";
            int port = 587;
            string senderEmail = "githubdossierregistratie@gmail.com";
            string password = "bgzg dijr ekkz syou";
            string recipientEmail = "patrick.stel@kpnmail.nl";
            string subject = "Eefting Dossier Registratie - " + issueType + " melding.";
            string body = "Er is een nieuwe melding gemaakt voor het Eefting Dossier Registratie Systeem. \r\n" +
                            "Issue nummer is: " + issueNummer;

            using SmtpClient client = new(smtpServer, port);
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(senderEmail, password);
            client.EnableSsl = true;

            MailMessage mailMessage = new(senderEmail, recipientEmail, subject, body);
            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred while sending the email: {ex.Message}");
            }
        }
    }
}
