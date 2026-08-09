# WebTerriane — Foundation

This is the foundation layer from our whiteboard session: data model, DB context,
and the file storage abstraction (the riskiest piece — Blob Storage vs local disk).
Not a full build; it's the skeleton everything else attaches to.

## What's here

```
WebTerriane/
├── Models/
│   ├── Property.cs
│   ├── PropertyImage.cs
│   ├── Application.cs
│   ├── ApplicationDocument.cs
│   └── SupportingModels.cs      (Administrator, SiteSetting, Testimonial, FAQ, ContactMessage)
├── Data/
│   └── ApplicationDbContext.cs
├── Services/
│   ├── Interfaces/
│   │   ├── IFileStorageService.cs
│   │   └── IReferenceNumberService.cs
│   ├── AzureBlobStorageService.cs   (production)
│   ├── LocalFileStorageService.cs   (dev only — never wire up in prod)
│   └── ReferenceNumberService.cs    (SQL SEQUENCE — atomic under concurrency)
├── Program.cs                        (DI wiring, Identity setup, storage provider switch)
└── appsettings.json                  (per-client config template)
```

## Required NuGet packages (not restorable in this sandbox — no network egress)

```
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools
Microsoft.AspNetCore.Identity.EntityFrameworkCore
Azure.Storage.Blobs
```

## Key decisions baked in

- **Files never live in `wwwroot/uploads` in production.** Two blob containers:
  `property-images` (public, CDN-fronted later) and `application-documents`
  (private, SAS-token access only, 15-minute expiry generated per download click).
- **Reference numbers use a SQL `SEQUENCE`**, not `MAX(Id)+1` — safe under
  concurrent application submissions with zero app-level locking.
- **Administrator is the only identity type.** No student login/registration
  exists in the schema or the auth pipeline — matches the "no student accounts" requirement.
- **`SiteSetting` is a key-value table**, not a dozen single-purpose tables, so
  editable homepage/contact/social content doesn't require a migration per field.
- **Delete behavior is deliberate, not default cascade-everything**: deleting a
  Property cascades its Images (DB rows only — the service layer must delete the
  actual blobs *before* calling `SaveChanges`, or you'll leak orphaned blobs and
  pay for them silently). Deleting a Property with existing Applications is
  `Restrict`, not cascade — application history shouldn't vanish silently.

## Next pieces, in build order

1. `IPropertyRepository` / `PropertyService` — CRUD + image upload orchestration
   (calls `IFileStorageService`, then writes `PropertyImage` rows)
2. `IApplicationService` — handles submission, calls `IReferenceNumberService`,
   saves `ApplicationDocument`s to the private container, triggers `IEmailService`
3. `IEmailService` + HTML templates (Received/Approved/Rejected)
4. Admin area controllers + Identity scaffolding (login only, no register view)
5. Public controllers + Razor views (Home, Accommodation, Property Details, Apply Now)
6. EF Core migration + seed script (first admin account, default `SiteSetting` rows)
7. Bicep template for per-client provisioning (App Service, SQL DB, 2 blob containers, Key Vault)


