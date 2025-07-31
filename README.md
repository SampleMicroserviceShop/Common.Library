# Common.Library
Common libraries used by other services.

## Create and publish package
```powershell
$version="1.0.6"
$owner="samplemicroserviceshop"
dotnet pack --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/Common.Library -o ..\..\packages\SampleMicroserviceShop
```

 ## Add the GitHub package source
```powershell
$owner="SampleMicroserviceShop"
$gh_pat="[PAT HERE]"
dotnet nuget add source --username USERNAME --password $gh_pat --store-password-in-clear-text --name github https://nuget.pkg.github.com/$owner/index.json
```