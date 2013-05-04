This is the demo application used in my ServiceStack talk at Chicago Code Camp
Running version should be availabe at [ccdemo.apphb.com](http://ccdemo.apphb.com)

**Scenario:**
Think of it like a Starbucks. Orders are entered by a user (or taken by a cashier).
A fulfiller (barista) starts processing/fulfilling the order. The user can watch the status of their order by watching a queue. Using ServicStack I want to try to create 3 separate systems (Ordering/Fulfilling/Customer Queue) that only know about each via their API.

**Notes/Examples:**  

* Uses the recommended ServiceStack [project structure](https://github.com/ServiceStack/ServiceStack/wiki/Physical-project-structure) 
* [ServiceStack Registration and Authentication](https://github.com/ServiceStack/ServiceStack/wiki/Authentication-and-authorization) within MVC. 
* [Redis Pub/Sub](https://github.com/ServiceStack/ServiceStack.Redis/wiki/RedisPubSub)
* SignalR to make live updates in views

**Additional softwares used:**

- MVC 4 - not doing much. Views pull data via AngularJS.
- ServiceStack (hosted at /api)
- SQLite - persistence for Orders/Fulfillments
- AngularJS - handling all the View/UI stuff
- Redis - Cache, handling Pub/Sub and holding data for Orders Queue
- SignalR - realtime refreshing of View grid data

**Ordering System Concept**:

- Any user can create an Order 
- Orders get written to the database and displayed in the View
- Orders are not editable (currently)
- When an Order is placed a 'NewOrder' message is published

![Ordering](https://raw.github.com/paaschpa/ordersDemo/master/orders.jpg)

**Fulfillment System Concept**:

- A 'Fulfiller' must log in. 
- Fulfillments are created by 'listening' for 'NewOrder' message
- Once logged in a Fulfiller can view Orders that have not been fulfilled
- When a fulfillment is updated a 'UpdateFulfillment' message is published

![Fulfillment](https://raw.github.com/paaschpa/ordersDemo/master/fulfillment.jpg)

**Orders Queue Concept**:

- Shows all partial Order data (customer name, item, status) that have not been fulfilled

![Queue](https://raw.github.com/paaschpa/ordersDemo/master/queue.jpg)

**Issues**: 
- If Redis isn't running Visual Studio seems to crash

