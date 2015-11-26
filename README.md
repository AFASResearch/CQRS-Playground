# CQRS-Microservices

This is a sample .NET code for microservices based on the CQRS pattern. We've created it to illustrate our approach. 
Please note that this is sample code. All kinds of essential stuff is missing. 
We didn't implement error handling, retry logic, and we haven't added persistence in a proper way.

But still, we think that this sample shows how you could lever Service Fabric to make your CQRS application scalable.
When you have questions or other remarks, just submit an issue. 
Pull requests for obvious bugs are welcome, pull requests that try to transform this into shippable code are not.

You will need the following things:

* Visual Studio 2015
* [Service Fabric Preview SDK 1.4.87](https://azure.microsoft.com/en-us/services/service-fabric/)

You can either run CQRSMicroservices.Console or CQRSMicroservices.ServiceFabric.Application as startup project.

The console application can run in-process or can connect to a HTTP webserver (running in ServiceFabric). Example:

    CQRSMicroservices.Console.exe "http://localhost:12121/"

The ServiceFabric webservice runs on port 12121. See the console application for example code that sends commands or queries.