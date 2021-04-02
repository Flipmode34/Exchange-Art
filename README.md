# Project
Een webapplicatie om Kunstwerken uit te lenen tussen verschillende geregistreerde users.

## Functionaliteiten
* Register and Login of users
* Create/Delete your own FlipCoin wallet
* Upload/Delete a Artpiece into the database
* Lease Artpieces from other users

## Technologien
* Microsoft .NET Core v3.1
* Bootstrap v4.0

## Requirements
* MVC patroon gebruikt
* Applicatie is als broncode zichtbaar en te downloaden vanaf Github
* Versiebeheer zichtbaar op de Github pagina
* Kunstwerken kunnen worden uitgeleend tussen een periode van 3 tot 6 maanden
* De eigenaar van het kunstwerk kan zelf de prijs bepalen
* Transacies vinden plaats d.m.v. een cryptocurrency naar keuze
* * Eigen simpele Cryptocoin gebruikt die niet van een PeertoPeer netwerk gebruik maakt maar het simuleert en de Blocks/Transactions opslaat in de database.
* Na het registreren van de User is de wallet zelf aan te maken op de "Wallet" pagina van de User
* Het is mogelijk om cryptocurrency te storten en op te nemen (hetzij gesimuleert)

## Installatie
### Alleen getest op Windows met Visual Studio Community
* Download ZIP file van solution vanaf deze pagina (groene knop genaamd " Code"
* Open de solution in Visual Studio Community
* Open de Developer Powershell
* Ga naar de Project directory middels dit commando: `cd .\Exchange-Art\`
* Dubbelcheck dat je in de juiste directory zit: `ls`
* Installeer dotnet indien nodig via de Developer Powershell in VS: `dotnet tool install`
* Installeer dotnet ef tools via de Developer Powershell in VS: `dotnet tool install --global dotnet-ef`
* Voer een Initieele migratie uit van het Model van de webapp via de Developer Powershell in VS: `dotnet ef migrations add InitialCreate`
* Update de database met de informatie van de InitialCreate migratie: `dotnet update database`
* Start the app
