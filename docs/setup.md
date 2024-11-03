---
layout: default
title: Installatie
nav_enabled: true
---

## DigiGraf Installatiehandleiding
Welkom bij DigiGraf! Volg de onderstaande stappen om de software correct te installeren en direct te beginnen met het beheren van uw dossiers voor de uitvaartverzorging.

### Systeemvereisten
Voordat u DigiGraf installeert, controleer of uw systeem voldoet aan de volgende minimumvereisten:

- Besturingssysteem: Windows 10 of hoger (64-bit versie aanbevolen)
- Processor: Dual-core of hoger
- RAM: Minimaal 4 GB (8 GB of hoger aanbevolen voor optimale prestaties)
- Opslag: Minimaal 500 MB vrije schijfruimte voor de software, plus extra ruimte voor gegevensopslag
- Netwerk: Internetverbinding vereist voor updates en cloudintegraties
- Database: Microsoft SQL Server (minimaal versie 2019) – de software kan lokaal of op een externe SQL-server worden geïnstalleerd
- .NET Framework: Versie 6.0 of hoger moet geïnstalleerd zijn

### Voorbereiding op de Installatie
1. Zorg ervoor dat alle vereiste componenten, zoals SQL Server en .NET Framework, correct zijn geïnstalleerd op uw systeem.
2. Als DigiGraf verbonden moet worden met een externe database, zorg er dan voor dat u over de juiste verbindingsgegevens beschikt (servernaam, gebruikersnaam, wachtwoord).

### Stap-voor-stap Installatie-instructies
1. Download de Installer

- Ga naar de officiële DigiGraf-pagina of uw klantportaal om de nieuwste versie van DigiGraf te downloaden.
- Klik op de downloadlink en sla het installatiebestand (DigiGrafInstaller.exe) op uw computer op.

### Start de Installatie

- Dubbelklik op het gedownloade installatiebestand om de installatie te starten.
- Volg de instructies in het installatieprogramma om DigiGraf op uw systeem te installeren.
Instellen van Databaseverbinding

 - Tijdens de installatie wordt u gevraagd om de verbindingsinstellingen voor de database in te voeren. Vul de volgende informatie in:
  - Servernaam: De naam of het IP-adres van de SQL Server
  - Database: Naam van de database (standaard: DossierRegistratie)
  - Gebruikersnaam en wachtwoord: Voer de inloggegevens in voor de SQL-databasegebruiker

### Vul de AppConnectionSettings.json in

 - Na de installatie opent DigiGraf de configuratiebestanden. Vul de AppConnectionSettings.json in met de juiste verbindingsinstellingen:
  - ConnectionStrings: Hier vult u de verbinding naar de SQL Server in.
  - SystemSettings en SystemConfiguration: Vul deze in naar de specificaties van uw bedrijf (bijv. SystemTitle, GithubKey, etc.).

### Voltooi de Installatie

 - Zodra de configuratiebestanden zijn ingevuld, voltooit u de installatie door op Voltooien te klikken.
 - Start de applicatie voor de eerste keer op. DigiGraf zal controleren of alle benodigde instellingen aanwezig zijn en u door de initiële configuratie leiden.

### Setup Voltooien en Voorbereiding van Initiele Data

 - Indien dit de eerste keer is dat DigiGraf wordt gestart, zal de applicatie vragen om enkele initiale data toe te voegen. Volg de stappen op het scherm om standaardgegevens in te voeren en de applicatie verder te configureren.

### Veelvoorkomende Problemen en Oplossingen
 - Probleem: DigiGraf kan geen verbinding maken met de database.
  - Oplossing: Controleer of de SQL Server actief is en of de verbindingsinstellingen correct zijn ingevoerd. Zorg ervoor dat de juiste poorten op de firewall zijn geopend.

 - Probleem: .NET Framework 6.0 is niet geïnstalleerd.
  - Oplossing: Download en installeer de laatste versie van .NET Framework 6.0 via de officiële Microsoft-website en probeer de installatie opnieuw.
    
### Ondersteuning
Voor verdere ondersteuning kunt u contact opnemen met ons ondersteuningsteam via e-mail op support@digigraf.nl of telefonisch op +31 (0)20 123 4567.

Bedankt dat u voor DigiGraf heeft gekozen – wij wensen u een succesvolle start met ons dossierregistratiepakket!
