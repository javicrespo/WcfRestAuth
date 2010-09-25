
HTTP Authentication methods for WCF REST services
------------
This project implements the following authentication methods for WCF REST Services:

- HTTP Basic Authentication
- HTTP Digest Authentication
- WSSE Username Token

Solution Contents
------------
* WcfRestAuth - Core implementation
* Server - A Website that contains a sample WCF REST service for each of the authentication methods implemented in WcfRestAuth
* WcfRestAuth.IntegrationTests - BDD specs that test the different authentication methods by using the Server site. (Run the Server site before running the tests)

References
-----------
This projects makes use of the WCF REST Starter Kit and was inspired by this [post](http://weblogs.asp.net/cibrax/archive/2009/03/20/custom-basic-authentication-for-restful-services.aspx)
[This is the introductory post from my blog.](http://javiercrespoalvez.com/2010/09/securing-wcf-rest-services-for.html)

