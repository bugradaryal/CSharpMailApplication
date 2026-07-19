# C# Desktop Email Application - "XyzMail-Desktop"

A Windows Forms desktop email client built on top of Gmail (IMAP/SMTP), with local caching of messages in a MySQL database via Entity Framework Core.

## Features

- Login with Gmail credentials, with a 2‑step email verification code sent before granting access
- Fetches Inbox, Sent, and Trash folders over IMAP (via MailKit)
- Sends mail (with attachments and inline images) over SMTP (via MailKit/MimeKit)
- Rich text (RTF) email composition, including inline image embedding
- HTML‑to‑RTF / RTF‑to‑HTML conversion for reading and composing formatted mail
- Local caching of Inbox / Sent / Trash in MySQL so the mailbox is browsable offline
- Move to Trash / Restore / Permanent delete, synced with the actual Gmail folders
- Background sync (`BackgroundWorker`) so the UI doesn't freeze while mail is being fetched

## Tech Stack

- **.NET Framework 4.7.2**, WinForms
- **Entity Framework Core 3.1** — **Code First** (see below)
- **MySQL** (LocalDB)
- **MailKit** / **MimeKit** — IMAP & SMTP protocol handling
- **MarkupConverter** — HTML ⇄ RTF conversion for the rich text editor

## Database — Code First

The database schema is defined entirely in code and generated via EF Core Migrations — there is no hand‑written SQL schema.

- `DataDbContext` (`DataDbContext.cs`) is the EF Core `DbContext`. It configures all entities and relationships in `OnModelCreating`.
- Tables:
  - `login_user` — stores account credentials, IP/machine binding, and last login timestamp
  - `mail_get_user` / `mail_get_user_dosyalar` (attachments) / `mail_get_user_bodyfile` (inline images) — cached Inbox
  - `mail_send_user` / `mail_send_user_dosyalar` / `mail_send_user_bodyfile` — cached Sent folder
  - `trash_get_user` / `trash_get_user_dosyalar` / `trash_get_user_bodyfile` — cached Trash folder
- Relationships between each mail table and its attachments/bodyfiles are one‑to‑many with cascade delete; the relation to `login_user` is restrict‑delete.
- Migrations live under `Migrations/` (e.g. `InitialCreate`) and are applied automatically — `DataDbContext.OnConfiguring` calls `UseMySql(...)`, and EF Core will create/update the database from the model on first run.

### Connection string & configuration

The connection string and mail credentials are **not** committed to source control. They live in `Secrets.config`, which is git‑ignored and referenced from `App.config`:

```xml
<!-- App.config -->
<configuration>
  <appSettings file="Secrets.config" />
</configuration>
```

```xml
<!-- Secrets.config (not committed — see .gitignore) -->
<appSettings>
  <add key="MailUser" value="your-account@gmail.com" />
  <add key="MailPassword" value="..." />
  <add key="MailDbConnection" value="Server=localhost;Port=3306;Database=Mail;User=root;Password=..." />
</appSettings>
```

To run the project locally, create your own `Secrets.config` in the project root with the three keys above.

## Architecture

The project follows a simple layered structure:

```
Forms (UI)  →  Services (business logic)  →  DataDbContext (EF Core / MySQL)
                     ↑
              Service interfaces (for DI / testability)
```

- **DTOs** (`mail.DTO` namespace) — plain data holders used for staging data fetched live from IMAP before it's synced into the database (`mail_tut`, `mail_bodyfile_tut`, `mail_attachment_tut`), plus a form‑local transfer object used while composing rich text mail (`mail_file_transfer`).
- **Services** (`mail.Services` namespace) — one service per mailbox concern:
  - `LoginService` — authentication, account provisioning/update, device (IP/machine) tracking
  - `InboxService` / `SentService` / `TrashService` — sync fetched mail into the DB, read cached mail back out, and move mail between Inbox/Sent and Trash
- **Service interfaces** (`mail.Services.Interfaces`) — `ILoginService`, `IInboxService`, `ISentService`, `ITrashService`. Forms depend on these interfaces rather than concrete classes, injected through the constructor.
- **Forms** — `LoginScreen` → `SecurityCode` (verification) → `GetMail` (mailbox) → `SendMail` (compose). Each form receives the services it needs via constructor injection; `Program.cs` acts as the composition root, creating the concrete service instances once and wiring up the form graph.

## Getting Started

1. Clone the repository.
2. Create `Secrets.config` in the project root (see [Connection string & configuration](#connection-string--configuration) above).
3. Make sure a MySQL instance (LocalDB) is reachable at the connection string you configured.
4. Restore NuGet packages and build in Visual Studio.
5. Run — on first launch EF Core will create the database schema from the current model/migrations.

> **Note:** Gmail requires an **App Password** (not your regular account password) for SMTP/IMAP access when 2‑Step Verification is enabled on the Google account.

## Known limitations

- Gmail is currently the only supported provider (IMAP/SMTP hosts are hardcoded to `imap.gmail.com` / `smtp.gmail.com`).
- Credentials are stored in plain text in `Secrets.config`; this is fine for local development but not intended for distribution.
