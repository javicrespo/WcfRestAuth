
HTTP Authentication for WCF REST services
------------
This project implements the following authentication method for WCF REST Services:

- HTTP Basic Authentication
- HTTP Digest Authentication
- WSSE Username Token

Solution Contents
------------
* WcfHttpAuth - Core implementation
* Server - A Website that contains a sample WCF REST service for each of the authentication methods implemented
* WcfHttAuth.IntegrationTests - BDD specs that test the different authentication methods used in the Server site.

References
-----------
This projects makes use of the WCF REST Starter Kit and was inspired by this [post](http://weblogs.asp.net/cibrax/archive/2009/03/20/custom-basic-authentication-for-restful-services.aspx)

