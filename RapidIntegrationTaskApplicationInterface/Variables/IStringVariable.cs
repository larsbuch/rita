using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RukisIntegrationTaskhandlerInterface.Variables
{
    public interface IStringVariable:IVariable
    {
        string Value { get; set; }
    }
}
