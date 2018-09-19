﻿using System.Collections.Generic;

namespace SixNations.Desktop.Interfaces
{
    public interface IHttpDataServiceModel : IDataServiceModel
    {
        void Initialise(IDataTransferObject dto);

        string Error { get; set; }

        bool IsLockedForEditing { get; }

        IDictionary<string, object> GetData();
    }
}
