# mag
Asp.Net Core WebApi + Knockout

## Backend
Il backend presenta le seguenti caratteristiche:
* db Sql Server Express
* gestione classi modello db con annotazioni da libreria Toolbelt.ComponentModel.DataAnnotations per definizione dichiarativa non basata su convenzioni
* hosting su Kestrel standalone
* configurazione filtri CORS
* identity framework con token di autenticazione JWT e autorizzazione basata su ruoli:
	- WarehouseManager: accesso a tutte le api in GET, accesso alle api di gestione magazzino (warehouse, area, location) in POST, PUT e DELETE
	- CartManager: accesso a tutte le api in GET, accesso alle api di gestione carrello (loadedcart) in POST, PUT e DELETE
	- User: accesso a tutte le api in GET
	- ...: accesso libero alle api per la gestione CRUD dei componenti
* gestione proprietà appsettings.json
* gestione log con NLog con proprietà nlog.config
* gestione errori globale
* serializzazione/deserializzazione personalizzata con NewtonSoft.JSON
* migrazione e seeding automatico allo startup

## Frontend
Il fronend presenta le seguenti caratteristiche:
* layout Bootstrap
* routing in app con Sammyjs
* registrazione di nuovi componenti di pagina allo startup
* gestione chiamate Ajax centralizzata con trasformazione payload
* gestione dialog centralizzata (message, confirm, complessa)

## Applicazione
L'applicazione viene inizializzata con le informazioni su aree, locations, components e carts prese dal testo dell'esercizio

L'applicazione viene inizializzata con i seguenti utenti:
* t1 -> pwd: Test123!, role: WarehouseManager
* t2 -> pwd: Test123!, role: CartManager
* t1 -> pwd: Test123!, role: User

### Possibili sviluppi
* api di ricerca paginate e filtrate
* widget di griglia paginati e filtrati
* widget di dropdown con typeahead
