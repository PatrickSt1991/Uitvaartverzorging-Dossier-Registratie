using Dossier_Registratie.ViewModels;

namespace Dossier_Registratie.Models
{
    public class GitHubModel : ViewModelBase
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string State { get; set; }
    }
}
