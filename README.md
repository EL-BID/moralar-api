[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=EL-BID_moralar-api&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=EL-BID_moralar-api)

# moralar-api

This is the server that provides access to all of Moralar Database.

### Moralar Project
Here are the repositories used in the project:

- Moralar App for end user - https://github.com/EL-BID/moralar-appusuario-flutter
- Morar App for Field Agent (TTS) - https://github.com/EL-BID/moralar-apptts-flutter
- Moralar Web App for Admins, Field Agents and Public managers - https://github.com/EL-BID/moralar-admin
- Web Server for All applications - https://github.com/EL-BID/moralar-api

  
## Requirements
- .NET 1.1
- A Mongo Database with SRV connection.

## Database

It's important that the Database has an admin user to be able to create all the entities using the Admin UI.

Make sure that the database has a Collection named UserAdministrator.

Create a document with this format:

```json
{
  "Name": "admin name",
  "Email": "admin@domain",
  "Level": 0,
  "TypeProfile": 2,
  "DataBlocked": null,
  "Created": timestamp,
  "LastUpdate": null,
  "Login": null,
  "Password": "admin-password-here"
}

```



## Setup
- Create a Mongo Database and paste on the `DATABASE.CONNECTIONSTRING` variable on `src\Moralar.WebApi\appsettings.${env}.json`, env meaning the environment you want to associate that Mongo connection.
  - That allows you to isolate Dev, QA and Production environment Databases.
- Update the packages with NuGet.

### appsettings

Update the following values with the ones of your deployment:

```
...
"DATABASE.CONNECTIONSTRING": "mongodb+srv://rest_of_url_of_your_mongo_database"
...
"Config.BaseUrl":"https://209-94-63-45.us-sjo1.upcloud.host/ApiMoralarDev/content/upload/"
"Config.DefaultUrl":"https://209-94-63-45.us-sjo1.upcloud.host/ApiMoralarDev/content/images/default.png"
"Config.CustomUrls": [
      "https://209-94-63-45.us-sjo1.upcloud.host/ApiMoralarDev/"
    ]
```

## Getting started

- In Visual Studio, run `IIS - Development`.
- The screen that will popup is the Swagger documentation, take a moment to check all endpoints and test them there, if you want.
- Is possible that, when loading the solution within Visual Studio, the module src/Molarar.Data is not loaded, manually add it.

## Deployment in IIS

- Before building, modify project file to include the following:

```
	<PropertyGroup>
		<PublishWithAspNetTargetManifest>false</PublishWithAspNetTargetManifest>
	</PropertyGroup>

	<PropertyGroup>
		<CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
	</PropertyGroup>
```

- Deployment was tested with a IIS Server v10

- Install SDK for .NET 1.1 from [https://dotnet.microsoft.com/en-us/download/dotnet/1.1](https://dotnet.microsoft.com/en-us/download/dotnet/1.1)

- Application Pool

  * .NET CLR version: No Managed Code
  * Managred pipeline mode: Integrated

- Make sure that you that the folder inclueds a web config file for IIS, if not, create it. Below and example, but customize if needed:

```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <handlers>
      <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModule" resourceType="Unspecified" />
    </handlers>
    <aspNetCore processPath="dotnet" arguments=".\Moralar.WebApi.dll" stdoutLogEnabled="true" stdoutLogFile=".\logs\stdout">
	<environmentVariables>
    <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
     </environmentVariables>
	</aspNetCore>
  </system.webServer>
</configuration>
```

- Create the following folders in the application root folder and make sure that the IIS_IUSRS has Write permissions.

    * ExportFiles
    * Content
    * Content/ExportFiles
    * Content/images
    * Content/upload

## Acknowledgments / Reconocimientos

**Copyright © [2025]. Inter-American Development Bank ("IDB"). Authorized Use.**  
The procedures and results obtained based on the execution of this software are those programmed by the developers and do not necessarily reflect the views of the IDB, its Board of Executive Directors or the countries it represents.

**Copyright © [2025]. Banco Interamericano de Desarrollo ("BID"). Uso Autorizado.**  
Los procedimientos y resultados obtenidos con la ejecución de este software son los programados por los desarrolladores y no reflejan necesariamente las opiniones del BID, su Directorio Ejecutivo ni los países que representa.

### Support and Usage Documentation / Documentación de Soporte y Uso

**Copyright © [2025]. Inter-American Development Bank ("IDB").** The Support and Usage Documentation is licensed under the Creative Commons License CC-BY 4.0 license. The opinions expressed in the Support and Usage Documentation are those of its authors and do not necessarily reflect the opinions of the IDB, its Board of Executive Directors, or the countries it represents.

**Copyright © [2025]. Banco Interamericano de Desarrollo (BID).** La Documentación de Soporte y Uso está licenciada bajo la licencia Creative Commons CC-BY 4.0. Las opiniones expresadas en la Documentación de Soporte y Uso son las de sus autores y no reflejan necesariamente las opiniones del BID, su Directorio Ejecutivo ni los países que representa.
