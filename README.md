# Blazor Dialog

[![Build status](https://dev.azure.com/stavros-kasidis/Blazor%20Dialog/_apis/build/status/Blazor%20Dialog-CI)](https://dev.azure.com/stavros-kasidis/Blazor%20Dialog/_build/latest?definitionId=16) [![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/BlazorDialog.svg?logo=nuget)](https://www.nuget.org/packages/BlazorDialog) [![Nuget](https://img.shields.io/nuget/dt/BlazorDialog.svg?logo=nuget)](https://www.nuget.org/packages/BlazorDialog) [![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=7CRGWPYB5AKJQ&currency_code=EUR&source=url)

Dialog component as a service for [Blazor](https://blazor.net)!

![demo-img](ReadmeResources/dialog-demo.gif)

## Features
* Call a dialog procedurally as a service and `await` for the result !
* Can use dialogs as normal components (if you don't want to use as a service).
* Build-in modal dialog with optional helper components (`Header`, `Body`, `Footer`).
* Option to use completely custom markup/css (without using the build-in opinionated css and html).


## Samples / Demo
You can find code samples and demos [here](https://blazor-dialog-demo.azurewebsites.net/).

## Installation
**1. Add the nuget package in your Blazor project**
```
> dotnet add package BlazorDialog

OR

PM> Install-Package BlazorDialog
```
*Nuget package page can be found [here](https://www.nuget.org/packages/BlazorDialog).*

**2. Add the following line in your Blazor project in either `Startup.cs` (blazor server-side) or `Program.cs` (blazor wasm)**

**- Blazor server-side: `Startup.cs`**
```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // other code
        services.AddBlazorDialog();
        // other code
    }
}
```

**- Blazor wasm: `Program.cs`**
```csharp
public class Program
{
    public static async Task Main(string[] args)
    {
       // other code
        builder.Services.AddBlazorDialog();
        // other code
    }
}
```
**3. Add the following line in your `_Imports.razor`**
```csharp
@using BlazorDialog
```
**4. Reference the css file**

Add the following static file reference in your `_Host.cshtml` (server-side blazor) or in your `index.html` (client-side blazor). 
Make sure that there is a call to `app.UseStaticFiles();` in your server project's `Startup.cs`.

```html
<link href="_content/BlazorDialog/styles.min.css" rel="stylesheet" />
```

## ⚠️ Breaking changes ⚠️

<details open="open"><summary>Upgrading from 0.3 to 1.0</summary>

>- Removed `PreventShow` and `PreventHide` from `OnBeforeShow` and `OnBeforeHide` respectively. Too many cases where infinite loops could happen.
</details>

## Release Notes

<details open="open"><summary>1.5</summary>
    
>- Added css class parameter to all helper components (header/body/footer).
</details>

<details><summary>1.4</summary>
    
>- Added css class parameter to dialog component.
</details>

<details><summary>1.3</summary>
    
>- Added base z-index parameter to dialog component.
</details>

<details><summary>1.2</summary>
    
>- Added fullscreen mode.
</details>

<details ><summary>1.1</summary>
    
>- Added support for dialog-in-dialog.
</details>

<details><summary>1.0</summary>
    
>- Fixed an infinite loop bug with `OnBeforeShow` event.
>- Fixed css bugs.
>- Removed `PreventShow` and `PreventHide` from `OnBeforeShow` and `OnBeforeHide` respectively. Too many cases where infinite loops could happen.
</details>

<details><summary>0.3</summary>
    
>- Upgrated to 3.1.
>- Added new helper components: `DialogHeader`, `DialogBody`, `DialogFooter`
</details>

<details><summary>0.1</summary>
    
>- Initial release.
</details>
