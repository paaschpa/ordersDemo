Using ASP.NET plus these additional softwares:

- MVC 4 - not doing much. Views pull data via AngularJS.
- ServiceStack (hosted at /api)
- SQLite - persistence for Orders/Fulfillments
- AngularJS - handling all the View/UI stuff
- Redis - Cache, handling Pub/Sub and holding data for Orders Queue
- SignalR - realtime refreshing of View grid data

**Overall Concept**:  
Demo project attempting to show 3 separate systems (Ordering/Fulfilling/Customer Queue)
that only know about each via their API.

**Ordering Concept**:

- Any user can create an Order 
- Orders get written to the database and displayed in the View
- Orders are not editable (currently)
- When an Order is placed a 'NewOrder' message is published

**Fulfillment Concept**:

- A 'Fulfiller' must log in. 
- Fulfillments are created by 'listening' for 'NewOrder' message
- Once logged in a Fulfiller can view Orders that have not been fulfilled
- When a fulfillment is updated a 'UpdateFulfillment' message is published

**Orders Queue Concept**:

- Shows all partial Order data (customer name, item, status) that have not been fulfilled

**Issues**: 
- If Redis isn't running Visual Studio seems to crash

