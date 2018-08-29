# SixNations
Requirements management loosely based on SCRUM.

SixNations, is designed as lean tooling for an Agile development 
team.

It is designed to consume a RESTful data service but could be adapted to use something like Entity Framework.

It is a work-in-progress but should compile after a "Restore NuGet Packages" and
one must add a file in `Services` named `AuthTokenService.partial.cs` with the following contents:

```using System;

namespace SixNations.Desktop.Services
{
    partial class AuthTokenService
    {
        private const string ClientSecret = "your token here";
    }
}
```

Don't forget to add it to your `.gitignore` file, otherwise the world will know your secret!

---

Copyright (c) 2018 David Savage.
Please see the contents of the file 'COPYING'.

SixNations is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published 
by the Free Software Foundation, either version 3 of the License, 
or (at your option) any later version.

SixNations is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with SixNations.  If not, see <http://www.gnu.org/licenses/>.
