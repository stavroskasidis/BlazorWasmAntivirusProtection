﻿# Blazor Wasm Antivirus Protection

[![Nuget (with prereleases)](https://img.shields.io/nuget/v/BlazorWasmAntivirusProtection.svg?logo=nuget)](https://www.nuget.org/packages/BlazorWasmAntivirusProtection)  [![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=7CRGWPYB5AKJQ&currency_code=EUR&source=url)

> ⚠️⚠️ **NOT RECOMMENDED FOR .NET 8.0+** ⚠️⚠️:
> Please use the new Webcil format that is enabled by default in .NET 8.0. More info [here](https://github.com/dotnet/runtime/issues/80807).

This package attempts to guard against false positives from antiviruses that flag Blazor Wasm as malware (up to .NET version 7.0).


## Confirmed success against:
1. BitDefender Total Security (v26.0.10.45)
2. BitDefender Endpoint Security Tool (v7.4.3.146)
3. Smoothwall Firewall - [Confirmed by](https://github.com/MudBlazor/MudBlazor/issues/3883#issuecomment-1031357095) [peterthorpe81](https://github.com/peterthorpe81)
4. Sophos Endpoint Agent - [Confirmed by](https://github.com/stavroskasidis/BlazorWasmAntivirusProtection/issues/12) [peterthorpe81](https://github.com/peterthorpe81)
5. Forcepoint Firewall - [Confirmed by](https://github.com/dotnet/aspnetcore/issues/36978#issuecomment-1109822288) [egil](https://github.com/egil)
6. GDATA Internet Security - [Confirmed by](https://github.com/stavroskasidis/BlazorWasmAntivirusProtection/issues/6) [paulguz-datapa](https://github.com/paulguz-datapa)

> 📣 *If you have used this package and has helped you bypass any false positives from other security software, please consider creating an issue with your experience to contribute to this list.*

> 🛡️ *You can use [virustotal](https://www.virustotal.com/gui/home/url)'s online scanner for some indication of how various antiviruses view your site.*

## What does this package do ?
This package injects some custom MSBuild tasks that do the following during publishing:
1. Obfuscates all client assemblies so that firewalls and antiviruses don't see them as executables. Obfuscation methods supported:
   * Using a key to XOR all client assemblies (**default**) .
   * **OR**
   * Changing the MZ header of all client assemblies to BZ, a custom header (less aggressive - more info [here](https://en.wikipedia.org/wiki/DOS_MZ_executable)) .
2. Renames the extension of all client assemblies from **.dll** to **.bin** .
3. Swaps Blazor's default caching mechanism with a custom one that saves the obfuscated assemblies to the cache instead of the unobfuscated ones. This is because some antiviruses are flaging the cached Blazor files that are being saved on the disk by the browser.
4. Adds a `beforeStart` Blazor initialization method (more info [here](https://docs.microsoft.com/en-us/aspnet/core/blazor/javascript-interoperability/?view=aspnetcore-6.0#javascript-initializers)), that uses a custom `loadBootResource` function to restore the obfuscation of the assemblies after downloaded, but before loaded by dotnet.wasm.

## How to use
1. Add the nuget package in your **Client** (wasm) **AND** your **Server** (if using Blazor wasm hosted) projects.
```
dotnet add package BlazorWasmAntivirusProtection
``` 
2. (Progressive Web Applications **only**): If you are using the Blazor Wasm PWA template, update the following line in your `service-worker.published.js` file to include `.bin` files:

```js
const offlineAssetsInclude = [/\.bin$/, /\.dll$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.blat$/, /\.dat$/ ];
```

3. Publish your app in Release mode and test it!
```
dotnet publish Server\BlazorHostedSampleApp.Server.csproj -c Release
```
*Nuget package page can be found [here](https://www.nuget.org/packages/BlazorWasmAntivirusProtection).*

## Considerations
While using XOR obfuscation does not change the actual size of a dll file, it **does** affect its compressed size. The bigger the XOR key used, the worse the compression gets.

The default XOR key is the string `bwap`.

## Known Issues
Disabling trimming in your project and using this package at the same time is not supported. 
There is an issue tracking this limitation [here](https://github.com/stavroskasidis/BlazorWasmAntivirusProtection/issues/24).
```
<!-- Not Supported -->
<PublishTrimmed>false</PublishTrimmed>
````

## Configuration
The following options allow you to customize the tasks executed by this package.
### **Custom dll rename extension**
If you want to use a different extension for renaming dlls, for example ".blz", add the following property in the **published** project's .csproj file (**Server** project if using Blazor hosted).
```xml
<RenameDllsTo>blz</RenameDllsTo>
```

### **Disable dll rename**
You can disable dll renaming by adding the following property in the **published** project's .csproj file (**Server** project if using Blazor hosted).
```xml
<DisableRenamingDlls>true</DisableRenamingDlls>
```

### **Changing or disabling dll obfuscation**
You can change or disable dll obfuscation by adding the following property in your **Client** project's .csproj file. Supported values: 
- `None`
- `ChangeHeaders`
- `Xor` (default)
```xml
<!-- Disables dll obfuscation -->
<ObfuscationMode>None</ObfuscationMode> 
```

### **Changing the XOR obfuscation key**
You can change the key that is used for the XOR obfuscation adding the following property in your **Client** project's .csproj file.
```xml
<!-- Changes the dll obfuscation xor key -->
<XorKey>mykey</XorKey>
```

### **Disable caching**
You can disable boot resources caching by using the following property in your Client project's .csproj file, just as you would in any Blazor project. More info [here](https://docs.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/webassembly?view=aspnetcore-6.0#disable-integrity-checking-for-non-pwa-apps).
```xml
<BlazorCacheBootResources>false</BlazorCacheBootResources>
```

## Special Thanks
This work was inspired by the post in https://github.com/dotnet/aspnetcore/issues/31048#issuecomment-915152791  by github user [tedd](https://github.com/tedd)

## Release Notes
<details open="open"><summary>2.4.5</summary>
    
>- Fixes [#48](https://github.com/stavroskasidis/BlazorWasmAntivirusProtection/issues/48) (Contribution by [sykesbPragmatics](https://github.com/sykesbPragmatics))
</details>


<details><summary>2.4.0</summary>
    
>- Fix for gzip satellite assemblies being compressed from original verion instead of obfuscated one.
>- Changed target back to .net 6.0.
</details>

<details><summary>2.3.0</summary>
    
>- Removed Brotli.NET that depends on native libs and added BrotliCompress tool that uses .net native brotli compression.
  Fixes [#36](https://github.com/stavroskasidis/BlazorWasmAntivirusProtection/issues/36), [#42](https://github.com/stavroskasidis/BlazorWasmAntivirusProtection/issues/42)
  (Contribution by [jsakamoto](https://github.com/jsakamoto))
</details>

<details><summary>2.2.0</summary>
    
>- Fix when publishing from Visual Studio [#36](https://github.com/stavroskasidis/BlazorWasmAntivirusProtection/issues/36)
</details>

<details><summary>2.1.0</summary>
    
>- Fix targeting .net 7.0 correctly
>- Changed default Xor key to be a bit more complex.
</details>

<details><summary>2.0.0</summary>
    
>- Upgraded to .net 7.0
</details>


<details><summary>1.9.0</summary>
    
>- Changed "blazor.boot.json.gz" and "blazor.boot.json.br" to be recompressed instead of deleted. (Contribution by [jsakamoto](https://github.com/jsakamoto))
</details>

<details><summary>1.8.5</summary>
    
>- Changed default Xor key to be smaller so that the resulting obfuscated dlls are more compression friendly.
</details>

<details><summary>1.8</summary>
    
>- Fix: There was a problem caching the boot resources when a custom `loadBootResource` method was given in `Blazor.start()`.
</details>

<details><summary>1.7</summary>
    
>- New feature: Swaped Blazor's default caching mechanism with a custom one that saves the obfuscated assemblies on the cache instead of the unobfuscated ones. This is because some antiviruses are flaging the cached Blazor files that are being saved on the disk by the browser.
</details>

<details><summary>1.6</summary>
    
>- Fix for publishing twice before cleaning (regression) [#13](https://github.com/stavroskasidis/BlazorWasmAntivirusProtection/issues/13)
</details>

<details><summary>1.5</summary>
    
>- Added support for multiple dll obfuscations, changing the default to XORing the dlls instead of just changing the headers.
</details>

<details><summary>1.4</summary>
    
>- Added support for Multiple Blazor Wasm apps under the same Server project [#8](https://github.com/stavroskasidis/BlazorWasmAntivirusProtection/issues/8)
</details>


<details><summary>1.3</summary>
    
>- Added support for Blazor Wasm PWA apps
</details>


<details><summary>1.2</summary>
    
>- Fixed sequential publishing issue.
</details>

<details><summary>1.0</summary>
    
>- Added customization options.
</details>

<details><summary>0.1</summary>
    
>- Initial release.
</details>
