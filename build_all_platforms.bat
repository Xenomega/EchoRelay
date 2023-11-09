@echo off
REM =========================================================
REM A naive script to build EchoRelay for various platforms
REM ==========================================================
REM Note: EchoRelay.App and EchoRelay.Cli inherently build EchoRelay.Core, as it is a dependency.

REM Build all projects for Windows. This will fail if the correct version of VS is not configured properly.
dotnet build --configuration Release

REM Build various versions of EchoRelay.App
dotnet build --configuration Release --self-contained false .\EchoRelay.App\EchoRelay.App.csproj --runtime win-x86
dotnet build --configuration Release --self-contained false .\EchoRelay.App\EchoRelay.App.csproj --runtime win-x64
dotnet build --configuration Release --self-contained false .\EchoRelay.App\EchoRelay.App.csproj --runtime win-arm64

REM Build various versions of EchoRelay.Cli
dotnet build --configuration Release --self-contained false .\EchoRelay.Cli\EchoRelay.Cli.csproj --runtime win-x86
dotnet build --configuration Release --self-contained false .\EchoRelay.Cli\EchoRelay.Cli.csproj --runtime win-x64
dotnet build --configuration Release --self-contained false .\EchoRelay.Cli\EchoRelay.Cli.csproj --runtime win-arm64
dotnet build --configuration Release --self-contained false .\EchoRelay.Cli\EchoRelay.Cli.csproj --runtime linux-arm
dotnet build --configuration Release --self-contained false .\EchoRelay.Cli\EchoRelay.Cli.csproj --runtime linux-arm64
dotnet build --configuration Release --self-contained false .\EchoRelay.Cli\EchoRelay.Cli.csproj --runtime linux-x64
dotnet build --configuration Release --self-contained false .\EchoRelay.Cli\EchoRelay.Cli.csproj --runtime osx-x64
dotnet build --configuration Release --self-contained false .\EchoRelay.Cli\EchoRelay.Cli.csproj --runtime osx-arm64

REM Output a message for the user.
echo Output has been saved to respective binary/build folders. Please check them.

REM Pause to show the user the output.
pause
@echo on