using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidIntegrationTaskApplicationInterface.Variables;

namespace RapidIntegrationTaskApplicationInterface.Variables
{
    public interface IIntegerVariable:IVariable
    {
        int? Value{ set; get;}
    }
}