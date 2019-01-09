# SixNations

Requirements management loosely based on SCRUM.

SixNations, is designed as lean tooling for an Agile development 
team.

It is designed to consume a RESTful data service but could be 
adapted to use something like Entity Framework. The data service
is not yet available, however when time permits it will follow.

This project should compile after a "Restore NuGet Packages" and 
one must add a couple of files. The first in `Services` named 
`AuthTokenService.partial.cs` with the contents:

```using System;

namespace SixNations.Data.Services
{
    public partial class AuthTokenService
    {
        private const string ClientSecret = "your token here";
    }
}
```

The `.gitignore` file under this repository includes the extension
`.partial.cs`, otherwise the world would know your secret!

And the second partial in `Constants` named `Props.partial.cs`
with the following contents:

```
namespace SixNations.API.Constants
{
    public static partial class Props
    {
#if DEBUG
        /// <summary>
        /// Can use a Url or the word "Mocked/" for dummy services
        /// </summary>
        public static readonly string ApiBaseURL = MOCKED; 
		// or something like "http://homestead.test/";
#else
        public static readonly string ApiBaseURL = 
		    "http://192.168.0.22/";
#endif
    }
}
```
Or similar; one can change these 'developer settings' as much as
desired and not effect other developers because the extension
`.partial.cs` should be in the `.gitignore` file.

---

[![Join the chat at https://gitter.im/savagedinfo/SixNations](https://badges.gitter.im/savagedinfo/SixNations.svg)](https://gitter.im/savagedinfo/SixNations?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

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
