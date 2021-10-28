# This app is to prove that you can have a stand alone .net core app on you machine and you can package it and deploy it to AWS Lamda and access it using an API Gateway.

We will start with a new dotnet core project ( version 3.1).
"dotnet new mvc --name Poc.Web"

Just to test this works , we will try to try to publish the content and check if the project works ( without IIS).
So , in the same folder we do something like 
"dotnet publish --output C:\Temp\MyWebsite --configuration Release"

And now we navigate to the folder and run it 
"dotnet Poc.Web.dll"

So , now we have a boilerplate .net core project which runs! Will try to add steps to package it.

We will need package.json to download Bootstrap.css and copy it to the correct folder.

Let's begin building and creating an artifact which can be uploaded to AWS Lamda.

Amazon has bunch of NuGet packages which it uses to allow http calls to Lamda ( since Lamda is a function)

https://aws.amazon.com/blogs/developer/updates-for-serverless-asp-net-core/

So , in the cs proj file , We add a bunch of dependencies which would help us achieve AWS integration.

We then define a class that would receive the Lamda function calls and then hand it over to the Startup class and thats how we are "through". it is named as "LambdaEntryPoint".

This class would extend Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction and override two methods , the init and the MarshallRequest.

We would also need another package for the AWS runtime env and that is "Amazon.Lambda.RuntimeSupport" and as a part of this , 
We get a bare-bone linux distribution and the application needs to be "self contained" along with a script ( bootstrap) to tell Lamda how to launch the application.

But that means that we need to listen to lamda events if we are running this program as part of lamda. So , in the Program.cs we instantiate an intance for our lamda class LambdaEntryPoint
when we detect that the app is being run as part of Lamda function.( more details in the code). 

In order for that we create a shell script named bootstrap which gets executed by AWS Lamda and I sneak in "lamda" as the command line parameter.( file in the project )

Now comes the final part. "Publish". We can use dotnet publish but AWS does not recommend it because the published artifact might not have the 
required file persmission access to the runtime files.

What AWS recommends is a tool called dotnet-lamda which would do the same things which publish will do under the hood , but in AWS recommended way.

"dotnet tool install -g Amazon.Lambda.Tools"
dotnet-lambda package -c Release -pl ./Poc.Web --msbuild-parameters "--self-contained true --runtime rhel-x64"

This would generate the package(zip) file. Upload it to AWS Lamda and Done!!

Use the test button on the AWS Lamda UI to test it.

Happy AWSing!

