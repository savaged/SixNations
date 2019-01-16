## Setup ##
* Add a file in SixNations.Data under Services named `AuthTokenService.partial.cs` with these contents:
```
public partial class AuthTokenService
{
    private const string ClientSecret = "your hash here";
}
```
* Alter the file named Props.cs in SixNations.API under Constants, adding your SixNations server URL.
