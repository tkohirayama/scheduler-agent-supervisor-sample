FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY Ordering.Job.Agent/*.csproj ./Ordering.Job.Agent/
COPY Ordering.Job.Domain/*.csproj ./Ordering.Job.Domain/
COPY Ordering.Job.Infrastructure/*.csproj ./Ordering.Job.Infrastructure/
COPY Ordering.Job.Scheduler.Complete/*.csproj ./Ordering.Job.Scheduler.Complete/
COPY Ordering.Job.Scheduler.Start/*.csproj ./Ordering.Job.Scheduler.Start/
COPY Ordering.Job.Supervisor/*.csproj ./Ordering.Job.Supervisor/
RUN dotnet restore

# copy and publish app and libraries
COPY Ordering.Job.Domain/. ./Ordering.Job.Domain/
COPY Ordering.Job.Infrastructure/. ./Ordering.Job.Infrastructure/
COPY Ordering.Job.Scheduler.Complete/. ./Ordering.Job.Scheduler.Complete/
WORKDIR /source/Ordering.Job.Scheduler.Complete
RUN dotnet publish -c release -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app .

ENTRYPOINT ["dotnet", "Ordering.Job.Scheduler.Complete.dll"]