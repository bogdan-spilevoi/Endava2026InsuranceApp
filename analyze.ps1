$envPath = Join-Path $PSScriptRoot ".env"

if (-not (Test-Path $envPath)) {
    Write-Error ".env file not found at $envPath"
    exit 1
}

Get-Content $envPath | ForEach-Object {
    if ($_ -match '^\s*#' -or $_ -notmatch '=') { return }

    $k, $v = $_ -split '=', 2
    $k = $k.Trim()
    $v = $v.Trim().Trim('"').Trim("'")

    [System.Environment]::SetEnvironmentVariable($k, $v, "Process")
}

if (-not $env:SONAR_TOKEN) {
    Write-Error "SONAR_TOKEN not set (check .env)"
    exit 1
}

if (-not $env:SONAR_HOST_URL) {
    $env:SONAR_HOST_URL = "http://localhost:9000"
}


$ProjectKey     = "InsuranceApp"
$TestResultsDir = Join-Path $PSScriptRoot "TestResults"

if (Test-Path $TestResultsDir) {
    Remove-Item $TestResultsDir -Recurse -Force
}


dotnet sonarscanner begin `
  /k:$ProjectKey `
  /d:sonar.host.url="$env:SONAR_HOST_URL" `
  /d:sonar.token="$env:SONAR_TOKEN" `
  /d:sonar.cs.opencover.reportsPaths="TestResults/**/coverage.opencover.xml" `
  /d:sonar.cs.vstest.reportsPaths="TestResults/*.trx"

if ($LASTEXITCODE -ne 0) { exit 1 }


dotnet build
if ($LASTEXITCODE -ne 0) { exit 1 }


dotnet test `
  --logger "trx;LogFileName=tests.trx" `
  --results-directory $TestResultsDir `
  --collect:"XPlat Code Coverage" `
  -- `
  DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover

if ($LASTEXITCODE -ne 0) { exit 1 }


dotnet sonarscanner end `
  /d:sonar.token="$env:SONAR_TOKEN"

if ($LASTEXITCODE -ne 0) { exit 1 }

Write-Host "SonarQube analysis completed successfully."
