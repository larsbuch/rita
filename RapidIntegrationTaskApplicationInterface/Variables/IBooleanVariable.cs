using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RukisIntegrationTaskhandlerInterface.Variables
{
    public interface IBooleanVariable:IVariable
    {
        bool? Value { get; set; }
    }
}
