# Blazor Wasm Antivirus Protection

[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=7CRGWPYB5AKJQ&currency_code=EUR&source=url)

This package attempts to guard against false positives from antiviruses that flag Blazor Wasm as malware, until Microsoft gives us an official solution. 
This is a work in progress and success is not guaranteed. 

⚠️USE AT YOUR OWN RISK⚠️

## What does this package do ?
This package injects some custom MSBuild tasks that do the following:
1. Changes the MZ header of all client .dlls to BZ, a custom header, so that firewalls and antiviruses don't see them as executables. (more info [here](https://en.wikipedia.org/wiki/DOS_MZ_executable))
2. Renames all .dll files to .bin
3. Adds a `beforeStart` js blazor initialization module, using a custom `loadBootResource` function to restore the MZ header of the dll after downloaded but before loaded by dotnet.wasm

## How to use
1. Add the nuget package in your Client (wasm) AND your Server (if blazor wasm hosted) projects
```
> dotnet add package BlazorWasmAntivirusProtection

OR

PM> Install-Package BlazorWasmAntivirusProtection
```
2. Publish your app in Release and test!

*Nuget package page can be found [here](https://www.nuget.org/packages/BlazorDialog).*

## Known issues
If you try to publish a project after you published already you have to clean your solution or the publish will fail with the following exception: 

`error MSB4018: The "PInvokeTableGenerator" task failed unexpectedly.`

To prevent this you should perform a clean on your solution before every publish. For example:
```
> dotnet clean BlazorHostedSampleApp.sln -c Release
> dotnet publish .\Server\BlazorHostedSampleApp.Server.csproj -c Release
```


## Samples / Demo
You can find a sample app using this package [here](https://blazor-antivirus-block.azurewebsites.net/).


## Release Notes

<details open="open"><summary>0.1</summary>
    
>- Initial release.
</details>
