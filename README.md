# Common.Library
Common libraries used by other services.

## Variables
```powershell
$version="1.0.8"
$owner="SampleMicroserviceShop"
$gh_pat="[PAT HERE]"
```

## Create and publish package
```powershell
dotnet pack --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/Common.Library -o ..\..\packages\$owner
```

 ## Add the GitHub package source
```powershell
dotnet nuget add source --username USERNAME --password $gh_pat --store-password-in-clear-text --name github https://nuget.pkg.github.com/$owner/index.json
```

 ## Push Package to GitHub
```powershell
dotnet nuget push ..\..\packages\$owner\Common.Library.$version.nupkg --api-key $gh_pat --source "github"
```
