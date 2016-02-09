using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RapidIntegrationTaskApplicationInterface.Variables
{
    public interface IPlaceHolderVariable
    {
        string ParentVariableName { get; }
    }
}
