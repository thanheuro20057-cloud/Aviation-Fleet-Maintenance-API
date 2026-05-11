# Aviation Fleet Maintenance Intelligence

Business-driven fleet maintenance platform for companies that need to protect aircraft uptime, assign certified technical crews efficiently, and turn maintenance activity into operational decisions.

## Product Value

- Optimizes resource allocation by comparing certified crew capacity against active and unassigned maintenance demand.
- Identifies bottlenecks by certification type: avionics, engine, and hydraulics.
- Detects predictive maintenance risk before aircraft pass airframe or component thresholds.
- Quantifies estimated protected downtime value so leadership can see financial impact.
- Converts raw tickets, crew workload, and flight-hour data into recommended actions for planners and fleet managers.
- Ships with demo data so buyers can immediately see a realistic operations command center.

## Business Dashboard

Open the web app and select **Business dashboard**. It shows:

- Fleet availability percentage and ready aircraft count.
- Crew utilization percentage based on max active jobs per mechanic.
- Open backlog and unassigned tickets.
- Estimated monthly savings from protected downtime.
- Resource allocation by certification, including capacity remaining and backlog.
- Actionable insights with business impact statements.
- Trend signals for predictive coverage, aircraft due soon, backlog density, and availability pressure.
- Prioritized recommendations with owner and expected outcome.

## Key API Endpoints

- `GET /api/business/dashboard` returns KPIs, resource allocation, insights, trends, and recommended actions.
- `GET /api/aircraft` returns aircraft status and component thresholds.
- `GET /api/maintenance/warnings` returns predictive maintenance warning roster.
- `GET /api/maintenance/tickets` returns detailed maintenance ticket history.
- `POST /api/maintenance/ticket` creates and dispatches a ticket to a qualified mechanic when capacity exists.
- `PUT /api/settings/fleet` sets max active jobs per mechanic, used by dispatch and utilization metrics.

## Run

```powershell
dotnet run --project .\AviationFleet.Api\AviationFleet.Api.csproj
```

Then open the local URL printed by ASP.NET Core. The API serves the static web dashboard from `wwwroot`.
