using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RukisIntegrationTaskhandlerInterface.Variables
{
    public interface IListOfStringVariable:IVariable
    {
        void addString(string newString);
        List<string> getStringList();
    }
}
