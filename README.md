# Dream Soccer API

## Dream Soccer Core
This project use for all thing related with entity, shared able, function use by other project, contract for All repository and Service, so if we want to create new Repository (to NO SQL) or Change the implementation for new tenant we can just reference this project. because all thing releated our business value already in this project
### Folder Configurations
Use for all configuration we have. ex. for Auto mapper
### Folder Contracts
Use When we have new repository or service, we can put contract in this folder

### Folder Dtos
Use when we want transfer object from Controller to Service
### Folder Entities
Use when we want to have new table (class related with Database/Storage)
### Folder Extensions
Use when we have one function use by other class and we want 

### Folder Requests
Use when we want receive request from API, and we transform it to class, we can put in her

### Folder Response
Use when we want expose the result to the API

## Drean Soccer Repository
This project use for communication with database / storage, example if in the feature we will have change the database to NO SQL, we need implementing new class to read and write in NO SQL

## Dream Soccer Service
This is project for Logical Business what we want to have, all thing related with Business Logic we put in this project. If we want to have two logic different, we can write two class inheritance one, and we can configure in API

## Dream Soccer Server Test
This project use for make sure all the logic we write in the Service working like our expectation

## Dream Soccer API
This project use for expose to outside our business logic we have, we can validation user call our service, Generate Token for authorization etc

## Dream Soccer API End To End
This project use for make sure all API we have working like our expectation, we test from login until the end process

## Dream Soccer Server Test
This project use for make sure all the api we write in the Controller working like our expectation


# How to 
## Add migration 
If we have create new schema and we want to create the migration, we need goes to folder Dream Soccer Repository and run this command 
```sh 
cd DreamSoccer.Repository
dotnet-ef migrations add xxx
```

## Update Migration
If we have create new schema and we want to create the migration, we need goes to folder Dream Soccer Repository and run this command 
```sh 
cd DreamSoccer.Repository
dotnet-ef database update
```

## Add Run App as Development Mode
If we want to run app as development mode, we can use this script

```sh 
cd DreamSoccerApi
dotnet restore
dotnet run
```

## Add Run App as Production Mode
If we want to run app as development mode, we can use this script

```sh 
cd DreamSoccerApi
dotnet restore
dotnet build
dotnet publish
cd bin\Debug\net5.0\publish\
dotnet DreamSoccerApi.dll
```

## Add Run Unit testing
If we want to run app as development mode, we can use this script

```sh 
cd DreamSoccer.Services.Tests
dotnet restore
dotnet test
```
OR
```sh 
cd DreamSoccerApi-Test
dotnet restore
dotnet test
```

## Add Run E2E Testing
Create new database first, and Follow run step for "Update Migration", 

```sh 
cd DreamSoccerApi.EndToEnd
dotnet restore
dotnet test
```
