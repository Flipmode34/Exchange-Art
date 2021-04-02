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
* Start the app (Ik heb Kestrel gebruikt omdat ISS Express vaak niet werkte)

## Gebruik
* Login
Je kunt inloggen middels de vooraf aangemaakt Users: Admin, teacher of Student
Login (email): admin@novi.nl / teacher@novi.nl / student@novi.nl
Passwords: Pas.@l1ve / Tea.@l1ve / Stu.@l1ve

### Art uploaden
In de map `Exchange-Art-master\Exchange-Art\wwwroot\images\` zijn tien plaatjes te vinden,
die je kunt uploaden als Kunstwerk middels het menu in de webapp: `Art -> Upload Art`, je kunt ook eigen plaatjes gebruiken natuurlijk.
Als het plaatje is geupload moet je het Kunstwerk een prijs per maand meegeven in de detail pagina van het Kunstwerk:  `Art -> Art Overview -> Details (van een plaatje)`

De uploads zijn daarna te vinden in het 'Art Overview' via menu: `Art -> Art Overview`
Middels die pagina kun je een LeaseRequest initieren door op de Lease knop te drukken en dan op 'Request Lease' (Je kunt niet je eigen kunstwerk lenen)
Op de Request Lease pagina kun je de hoeveelheid maanden specifieren en het request submitten.

### Art Leases
Art Leases zijn terug te vinden op de Lease Overview pagina: `Art -> Lease Overview`
Art Leases zijn alleen te verwijderen door user met de rol "Admin".
Dit is aan te passen middels de 'Roles' pagina.
Art Leases expireren ook als de Eind datum van de Lease ouder dan de huidige datum is. (Zie: ArtController.cs regel 113)

### Betaling
De betaling van het lenen gaat per 'FlipCoin', een simpele cryptocurrency die zijn gegevens in de database opslaat ipv een online ledger.
Het verwerken van de FlipCoin transacties moet handmatig via de Transactions pagina: `Art -> Transactions` middels de knop 'Process Pending Transactions'.
Er wordt dan een 'Block' aangemaakt met alle transacties die nog niet verwerkt zijn. Tevens wordt er na het aanmaken van het 'Block' een reward transactie gemaakt met de waarde van 1 FlipCoin en toegewezen aan een random Wallet adres in de database om het Minen te simuleren.
## Zorg er wel voor dat er wallets aanwezig zijn voor beide users!
