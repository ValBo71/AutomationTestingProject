# 01 - API testing (Swagger Petstore)

A deliberately raw starting point: one NUnit test with the requests written inline, no API clients, models or
helpers. It is kept as the "before" half of a before/after comparison with
[`04-automationexercise-api-csharp`](../04-automationexercise-api-csharp), which shows the same kind of work with
a proper structure. The Postman collection next to it covers the same user lifecycle.

## What it covers

`SwaggerPetstoreApiTests.UserLifecycleTest` against the public [Swagger Petstore](https://petstore.swagger.io):
create a user, read it back, update it, read the update back, log in, log out, delete it and confirm it is gone.

Two things about the public Petstore shape the test:

* **It does not check credentials.** `/user/login` answers with a session for any username and password, so the
  login step can only check the shape of the answer. That the updated password was stored is proven by reading
  the user back after the update.
* **An update needs the user's `id` in the body**, otherwise Petstore keeps the old values, and a write is not
  always visible on the very next request. Reads after a write are therefore polled for a few seconds instead
  of waiting a fixed time.

Whatever fails, the user is deleted at the end, so nothing is left on the shared sandbox.

## Running

The folder holds both the solution and the project file, so name one of them - a bare `dotnet test` stops with
`MSB1011` (more than one project or solution file):

```bash
dotnet test ApiTesting.sln
```

The Postman collection runs with Newman:

```bash
npx newman run "Swagger Petstore.postman_collection.json" -e "Swagger PetstoreENV.postman_environment.json"
```
