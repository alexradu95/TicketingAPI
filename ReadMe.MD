1) Applying the design-first approach, using an API design tool of your choice, please design a RESTful "Ticketing" API for the following requirements:

- Everyone can see the tickets -> Done

- Only authenticated users can sale or purchase tickets ->

  * de facut entry-point pentru sell si buy
  * facut trei roluri, unul de admin, unul de seller si unul de buyer
  * securizat datele folosind un DTO pentru a ascunde informatiile neesentiale

- Only the ticket owner can modify the ticket (e.g. change price) -> Done

  * implementan sistem de autentificare cu JWT Token
  * pus in claim numele si rolul utilizatorului
  * pus restrictii pe entry-point-ul de PUT

2) Please implement only the endpoint for updating a ticket described by your design using ASP.NET WebAPI, preferably in C#.


Expectations:

- API design specification for the indicated requirements

- API usage workflows (simplified schema or description) that you envisioned for your design proposal

- Implementation of the indicated endpoint (updating a ticket), functional, to be tested with Postman