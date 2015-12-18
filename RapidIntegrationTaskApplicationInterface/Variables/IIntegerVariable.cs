using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RukisIntegrationTaskhandlerInterface.Variables;

namespace RukisIntegrationTaskhandlerInterface.Variables
{
    public interface IIntegerVariable:IVariable
    {
        int? Value{ set; get;}
    }
}