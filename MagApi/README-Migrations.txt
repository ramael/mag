**IDENTITY_DB**
Add-Migration "FirstCreate-IdentityDB" -Context MagIdentityDbContext
UNDO: Remove-Migration
Update-Database "FirstCreate-IdentityDB" -Context MagIdentityDbContext

**DB**
Add-Migration "FirstCreate-DB" -Context MagDbContext
UNDO: Remove-Migration
Update-Database "FirstCreate-DB" -Context MagDbContext

For testing environments it's ok to apply migration programmatically, but keep in mind:
- If multiple instances of your application are running, both applications could attempt to apply the migration concurrently and fail (or worse, cause data corruption).
- Similarly, if an application is accessing the database while another application migrates it, this can cause severe issues.
- The application must have elevated access to modify the database schema. It's generally good practice to limit the application's database permissions in production.
- It's important to be able to roll back an applied migration in case of an issue. The other strategies provide this easily and out of the box.
- The SQL commands are applied directly by the program, without giving the developer a chance to inspect or modify them. This can be dangerous in a production environment.

For real production environments ALWAYS apply migration manually:
- Script-Migration <from> <to>: to migrate to latest version, or from one version to another
- Script-Migrazion -Idempotent: internally check which migrations have already been applied (via the migrations history table), and only apply missing ones. 
								This is useful if you don't exactly know what the last migration applied to the database was, or if you are deploying to multiple databases 
								that may each be at a different migration.