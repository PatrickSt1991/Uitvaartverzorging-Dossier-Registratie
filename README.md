# Uitvaartdiensten Beheersoftware

Deze op maat gemaakte software is ontworpen voor een uitvaartonderneming om de dagelijkse activiteiten effectief te beheren. De software is gebouwd met WPF (Windows Presentation Foundation) volgens het MVVM (Model-View-ViewModel)-patroon en vereist .NET 6.0 en een MSSQL-database.

## Functionaliteiten

- Beheer van uitvaartdiensten, klantgegevens en planningen.

- Aanpasbare rapporten en data-export.

- Gebruiksvriendelijke WPF-interface met modern ontwerp.

- Veilige data-afhandeling met MSSQL-integratie.

- Ondersteuning voor real-time updates en meldingen.


## Vereisten

- **.NET 6.0** SDK en runtime

- **MSSQL Database** (versie naar keuze)

- Visual Studio 2022 (of hoger) wordt aanbevolen voor ontwikkeling en debugging.


## Installatie

1. Kloon de repository:

```
git clone https://github.com/jouwgebruikersnaam/uitvaartdiensten-software.git
```

2. Stel de database in:

  - Maak een MSSQL-database aan.

  - Implementeer het SQL-schema dat te vinden is in de map /Database om de benodigde tabellen in te stellen.



3. Configureer verbindingsreeksen:

  - Werk het appsettings.json bestand bij met de verbindingsgegevens van jouw database.



4. Voer de applicatie uit:

  - Open de oplossing in Visual Studio 2022.

  - Herstel de NuGet-pakketten.

  - Bouw en voer het project uit.

## Gebruik

Zodra de applicatie draait, kunnen gebruikers:

- Inloggen om toegang te krijgen tot de functies voor het beheer van uitvaartdiensten.

- Gegevens van uitvaartdiensten toevoegen, bewerken of verwijderen.

- Klantgegevens beheren.

- Diensten plannen en de benodigde documenten afdrukken.

- Rapporten genereren en exporteren in verschillende formaten.


## Bijdragen

Als je wilt bijdragen aan het project, fork dan de repository en stuur een pull request met een duidelijke beschrijving van de wijzigingen.

## Licentie

Dit project is gelicentieerd onder de GNU Affero General Public License v3.0. Zie het [LICENSE](LICENSE) bestand voor meer details.
