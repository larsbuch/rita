using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RapidIntegrationTaskApplicationInterface.Variables
{
    public interface IBooleanVariable:IVariable
    {
        bool? Value { get; set; }
    }
}
