Push-Location $PSScriptRoot

function Write-Message{
    param([string]$message)
    Write-Host
    Write-Host $message
    Write-Host
}
function Confirm-PreviousCommand {
    param([string]$errorMessage)
    if ( $LASTEXITCODE -ne 0) { 
        if( $errorMessage) {
            Write-Message $errorMessage
        }    
        exit $LASTEXITCODE 
    }
}

function Confirm-Process {
    param ([System.Diagnostics.Process]$process,[string]$errorMessage)
    $process.WaitForExit()
    if($process.ExitCode -ne 0){
        Write-Host $process.ExitCode
        if( $errorMessage) {
            Write-Message $errorMessage
        }    
        exit $process.ExitCode 
    }
}

Write-Message "Cleaning ..."
dotnet clean ../sampleapps/BlazorHostedSampleApp/BlazorHostedSampleApp.sln -c Release
Confirm-PreviousCommand

Write-Message "Building ..."
dotnet publish ../sampleapps/BlazorHostedSampleApp/Server/BlazorHostedSampleApp.Server.csproj -c Release -o ../artifacts/sample-apps/BlazorHostedSampleApp
Confirm-PreviousCommand


Write-Message "Build completed successfully"