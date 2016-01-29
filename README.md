# DbUp Migrator

An example DbUp exe and reset/update the world scripts, including Seq integration.

The reset/update the world Powershell scripts assume Visual Studio 2015. They perform a release build and execute the migrator from `\bin\release`. Visual Studio is not required to run the migrator during deployment.

The project includes Octopack so it will create a NuGet package when built by TeamCity (with the correct configuration), which can be consumed by Octopus Deploy.


## Instructions

1. Copy the DataMigrator project into your solution
2. Copy `reset-the-world.*` and `update-the-world.*` into the root of your solution.
3. Configure `app.config` with your Seq server URL, API key, and database connection string
4. Add migrations (`.sql` files) to the `Migrations` folder, making sure the files have "Embedded Resource" as the build action
5. In your deploy pipeline, execute `DataMigrations.exe` to update the database specified in `app.config`
6. During development, use `update-the-world.ps1` to apply new migrations and `reset-the-world.ps1` to drop and recreate the entire database before applying all migrations.

To update the database when your application runs, reference the DataMigrations project and do this:

```
var migrator = new Migrator("connection string");
migrator.Boom(resetTheWorld: false);
```


## To do

- scripts that will run each time the database is updated (data migrations, stored procedures, updating lookup tables)
- scripts that optionally run when resetting the world (test data)

