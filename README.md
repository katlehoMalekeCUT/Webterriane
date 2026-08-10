# WebTerriane

WebTerriane is a website for a student accommodation agency (currently modeled around
Bloemfontein, near **Central University of Technology** & University of the Free State).
Students browse verified residences, compare rent and distance from campus, and submit
an application online — no account or login needed. This document covers what's built
on the pages, layout, look and feel, and interactive bits.

## Pages that exist right now

| Page | Route | What it's for |
|---|---|---|
| Home | `/` | Landing page — hero search, featured properties, why-us, stats, testimonials, how-it-works, call to action |
| Accommodation | `/Accommodation` | Listing page (search filters from the homepage will land here) |
| Apply Now | `/Application/Apply` | Application form |
| About | `/Home/About` | About us |
| FAQ | `/Home/Faq` | Asked question |
| Contact | `/Home/Contact` | contact us |
| Privacy | `/Home/Privacy` | Our privacy policy |

Only the **Home** page has real content and design applied so far — it's the design
reference the rest of the site will follow. Everything else is a working link that
loads an empty page.

## Every page shares one layout

All public pages wrap around a single shared layout, so the navbar, footer, and
WhatsApp button appear everywhere automatically:

- **Sticky navbar** — logo mark, links to Home / Accommodation / About / FAQ /
  Contact, and an "Apply Now" button. Gains a soft shadow once you scroll past
  the top of the page. Collapses into a hamburger menu on mobile.
- **Footer** — brand blurb + social icons, an "Explore" link column, a "Legal"
  link column, and a "Get in touch" block with address/email/phone.
- **Floating WhatsApp button** — bottom corner, links out to a WhatsApp chat.

## Home page, section by section

1. **Hero** — Big headline over a background image, with a short pitch line, and
   the signature piece: a search box styled like a **boarding pass / ticket**
   (complete with notched corners and a dashed perforation line). Fields: university,
   area, move-in date, max rent — submits to the Accommodation listing page.
2. **Featured properties** — A 3-up grid of property cards (image, name, area,
   distance from campus, rent per month, a "Details" and an "Apply" button, plus a
   badge showing rooms left). Currently shows 3 hardcoded example listings
   (Kovsie Heights, The Willows Residence, CBD Student Lofts) as placeholders for
   what real data will look like.
3. **Why WebTerriane** — Three feature callouts with icons: every listing is
   physically inspected, distance-to-campus is measured (not estimated), and the
   application is a single form with no account required.
4. **Stats strip** — Four numbers that count up when scrolled into view: residences
   listed, applications processed, occupancy rate, universities covered.
5. **Testimonials** — Three student quotes in card form, each with a name and
   university.
6. **How it works** — A simple 3-step strip (Search & compare → Apply online →
   Get your reference number), connected with a line between steps.
7. **Closing call to action** — One more prompt to start an application, on a
   solid-color banner.

## Look and feel

- **Colors** — Deep navy as the primary brand color, a bright sky blue as the
  accent, warm amber used sparingly, and a soft off-white "paper" tone that
  alternates with white between sections for visual rhythm.
- **Type** — Three fonts working together on purpose:
  - **Fraunces** (a serif with character) for all headings — gives the brand a
    slightly editorial, non-corporate feel.
  - **Inter** for all body copy and UI text — clean and easy to read.
  - **IBM Plex Mono** specifically for numbers — rent prices, distances, stats,
    and step numbers — so figures visually stand out as *data* against the prose.
- **Shape language** — Rounded corners throughout (cards, buttons, the navbar
  logo mark), soft drop shadows rather than hard borders, generous white space.
- **Icons** — Font Awesome throughout (location pin, shield, route, envelope,
  WhatsApp, social icons, etc.).
- **Built on Bootstrap 5** for the grid and responsive behavior, with a custom
  stylesheet layered on top for all the WebTerriane-specific styling — so it's
  responsive out of the box (navbar collapses to a hamburger, cards restack
  on mobile) without extra custom breakpoint work.

## The small interactive touches (site.js)

Two things happen in the browser right now, both intentionally lightweight:

- The **navbar shadow** appears once you've scrolled a little way down the page,
  so it visually lifts off the content beneath it.
- The **stats numbers count up from 0** the moment they scroll into view, rather
  than just appearing — done in a way that respects a visitor's "reduce motion"
  accessibility setting (it just shows the final number instantly for them
  instead of animating).

## What's placeholder vs. real

- All property listings, testimonials, and stats on the homepage are **hardcoded
  example content** standing in for what will eventually come from the database —
  useful for judging the design, not real inventory yet.
- The property photos are temporary stock images.
- Accommodation (listings) and Apply Now (application form) pages exist as
  routes/links but have no content or layout yet — they're next in line to get
  the same design treatment as the homepage.
- About, FAQ, Contact, and Privacy are placeholder pages with nothing on them yet.

## Where things live

```
wwwroot/
├── css/site.css     ← all custom styling: colors, fonts, every .wt- component class
└── js/site.js       ← navbar shadow + animated stat counters

Views/
├── Shared/_Layout.cshtml   ← navbar, footer, WhatsApp button (shared by every page)
└── Home/Index.cshtml       ← the homepage itself, section by section
```

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

## Screenshots
<img width="1366" height="768" alt="image" src="https://github.com/user-attachments/assets/1c50673c-6822-4de6-961b-b94a41b38066" />
<img width="1366" height="768" alt="image" src="https://github.com/user-attachments/assets/68d88162-9ff5-4b72-b2ce-11da9ba0f88b" />
<img width="1366" height="768" alt="image" src="https://github.com/user-attachments/assets/cd93df51-f8ef-4ca8-b5a9-1a512604689e" />
<img width="1366" height="768" alt="image" src="https://github.com/user-attachments/assets/90aa3b4b-e13f-4049-8f0a-bf0cc0d429d2" />
<img width="1366" height="1052" alt="image" src="https://github.com/user-attachments/assets/fd482437-a5cc-4598-ab05-e7dd04776b64" />
<img width="1366" height="768" alt="image" src="https://github.com/user-attachments/assets/923449d8-2c98-479a-a614-730c3d79d869" />
<img width="1366" height="768" alt="image" src="https://github.com/user-attachments/assets/26acab03-6936-4099-ac36-206b649040ee" />
<img width="1366" height="768" alt="image" src="https://github.com/user-attachments/assets/a0f649e9-76ac-4f5d-b353-eca16424f6e8" />
<img width="1366" height="768" alt="image" src="https://github.com/user-attachments/assets/4d2ba303-f333-4baf-876e-f3b42dcbe3e3" />

## About Webterriane
<img width="430" height="430" alt="image" src="https://github.com/user-attachments/assets/a9ee4fe3-cd78-414a-9e2c-07f9d7a070f5" />
<img width="383" height="491" alt="image" src="https://github.com/user-attachments/assets/e9f9a86f-7776-4eaf-a009-61e7dcd57d70" />







