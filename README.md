# PartnerAPI 📎

A simple application using .NET Core 8.0 to demonstrate creating API.

## Features 🛠️
1. Added APIs for PartnerAPI (✅)
2. Added unit testing for Calculation (✅)
3. Added log4net log message for easier troubleshooting (✅)
4. Added docker to containerizing the application (✅)

## Tips and Guide: 😎
#### Deploy application in Docker
1. Go to the root folder, where `PartnerAPI.sln` is located and open terminal.
2. Insert `docker build -t partnerapi .` followed by `docker run -d -p 5000:5000 --name partnerapi-container partnerapi`
3. Access to `http://localhost:5000/swagger/index.html` should be successful.
