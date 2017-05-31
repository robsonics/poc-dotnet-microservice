FROM microsoft/aspnetcore:1.1
ENV ASPNET_ENV Production
WORKDIR /app
COPY published ./
ENTRYPOINT ["dotnet", "Satellite.v2.dll", "--server.urls", "http://0.0.0.0:5000"]
