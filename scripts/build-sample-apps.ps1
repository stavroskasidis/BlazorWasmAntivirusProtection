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

Write-Message "Building Hosted Sample App ..."
dotnet publish ../sampleapps/BlazorHostedSampleApp/Server/BlazorHostedSampleApp.Server.csproj -c Release -o ../artifacts/sample-apps/BlazorHostedSampleApp
Confirm-PreviousCommand

Write-Message "Building Hosted PWA Sample App ..."
dotnet publish ../sampleapps/BlazorHostedSamplePwa/Server/BlazorHostedSamplePwa.Server.csproj -c Release -o ../artifacts/sample-apps/BlazorHostedSamplePwa
Confirm-PreviousCommand

Write-Message "Build completed successfully"