# Common.Library
Common libraries used by other services.

## Create and publish package
```powershell
$version="1.0.6"
$owner="samplemicroserviceshop"
dotnet pack --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/Common.Library -o ..\..\packages\SampleMicroserviceShop
```
