# RaceDay — Part 3: MVC Web Application

RaceDay is a role-aware event platform for South African running, walking and cycling. The repository includes Part 1 planning artefacts, the Part 2 REST API and tests, and the Part 3 MVC client.

## Implemented Part 3 features
- ASP.NET Core MVC consumes the Part 2 API for all data operations; MVC has no direct database connection.
- Registration/login through the API, MVC session state, role-specific navigation and logout.
- Organiser dashboard, event create/edit/delete, category management, enrolment lists and results entry.
- Participant event discovery/filtering, event detail and category entry, My Events, My Results, profile editing and profile picture upload through the API.
- Event banner upload from MVC through the API. API stores images in Azure Blob Storage when configured, with local storage fallback for development.
- Responsive Bootstrap UI, sporting RaceDay styling and colour-coded enrolment statuses.
- Dockerfile for MVC and GitHub Actions workflow that builds the solution, runs API tests and builds the MVC container.

## Prerequisites
.NET 8 SDK, SQL Server/SQL Server Express/LocalDB, Docker Desktop. An Azure Storage account is needed to demonstrate cloud banner storage.

## Run locally
1. Open `RaceDay.sln` in Visual Studio.
2. Set `RaceDay.Api` and `RaceDay.Mvc` as multiple startup projects.
3. Configure `ConnectionStrings:RaceDayDb` in `src/RaceDay.Api/appsettings.json` or User Secrets. Do not commit credentials.
4. API launch profile uses `http://localhost:5180` and `https://localhost:7180`. MVC's `RaceDayApi:BaseUrl` defaults to `https://localhost:7180/` for local Visual Studio use.
5. Start both projects. Open the MVC URL shown by Visual Studio. In Development, API Swagger is at `/swagger`.
6. Register an Organiser to create events; register a Participant to browse and enter them.

## Azure Blob Storage
Configure the API using User Secrets or environment variables:
- `AzureBlobStorage:ConnectionString` / `AzureBlobStorage__ConnectionString`
- `AzureBlobStorage:Container` / `AzureBlobStorage__Container` (recommended: `raceday-banners`)

From the API project directory:
```bash
dotnet user-secrets init
dotnet user-secrets set "AzureBlobStorage:ConnectionString" "<your-connection-string>"
dotnet user-secrets set "AzureBlobStorage:Container" "raceday-banners"
```
Never commit a real connection string. MVC sends the file to `POST /api/events/{id}/banner`; the API stores it and returns a URL. MVC does not call Azure directly. Without the connection string, local file storage is used for development.

## Docker
Run from the repository root:
```bash
docker build -f src/RaceDay.Mvc/Dockerfile -t raceday-mvc .
docker run --rm -p 8080:8080 -e RaceDayApi__BaseUrl=http://host.docker.internal:5180/ raceday-mvc
```
Open `http://localhost:8080`. The API must be running and reachable. On Linux, add `--add-host=host.docker.internal:host-gateway` if required. For a fully containerised deployment, configure both containers on a shared Docker network and set the MVC API URL to the API service name.

## CI/CD
`.github/workflows/part-3-ci.yml` restores/builds the solution, runs the API unit tests and builds the MVC Docker image on pushes and pull requests. Push the project, open GitHub Actions, and capture a screenshot only after a successful green run.

## Video presentation
Replace `VIDEO_LINK_TO_BE_ADDED` below with your own unlisted YouTube video after recording the actual app.
`VIDEO_LINK_TO_BE_ADDED`

## Evidence
Use `docs/part-3-evidence.md` as the evidence register. Insert genuine screenshots of the running MVC pages, Azure Storage, Docker and the green GitHub Actions workflow.

## Important verification notes
The supplied Part 2 API uses its own server-side session cookie (`.RaceDay.Session`). The MVC client forwards that cookie on API calls and separately maintains role-aware MVC session state. Test the app and resolve any compile/runtime errors in Visual Studio before submission. This execution environment has no .NET SDK, so source could not be compiled here. A profile-image upload endpoint has been added to the API so profile pictures and event banners both travel through the API storage service.

## AI-use disclosure
AI assistance was used to scaffold and document parts of this project. Review, understand, test and adapt the code and disclose tool use in accordance with the POE instructions.
