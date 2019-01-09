﻿using System;

namespace SixNations.API.Constants
{
    public enum HttpMethods
    {
        Post,
        Get,
        Put,
        Delete,
        Patch
    }

    public enum RequirementStatus
    {
        _ = 0,
        Prioritised = 1,
        WIP = 2,
        Test = 3,
        Done = 4
    }
}
