using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using RukisIntegrationTaskhandlerInterface;
using RukisIntegrationTaskhandlerInterface.Constants;
using RukisIntegrationTaskhandlerInterface.Enumerations;
using RukisIntegrationTaskhandlerInterface.Exceptions;
using RukisIntegrationTaskhandlerInterface.Variables;
using Common.Logging;

namespace RukisIntegrationTaskhandlerExtension
{
    public class VariablePersister:IVariablePersister
    {
        private Dictionary<string, PersistedVariable> _persistedList;

        public VariablePersister()
        {
            readPersistedVariables();
        }

        #region Persistence

        [Serializable]
        private class PersistedVariable
        {
            public string Name { get; private set; }
            public VariableType Type { get; private set; }
            public int Unused { set; get; }
            public object Value { get; set; }

            public PersistedVariable(IVariable variable)
            {
                Name = variable.VariableName;
                Type = variable.Type;
                Value = getVariableObject(variable);
            }
        }

        public void readPersistedVariables()
        {
            string fullPath = GlobalVariables.getServiceLocation() + Misc.DirectoryEnd + Misc.PersistedVariablesFile;
            if (File.Exists(fullPath))
            {
                try
                {
                    using (Stream stream = File.Open(fullPath, FileMode.Open))
                    {
                        BinaryFormatter bin = new BinaryFormatter();

                        _persistedList = (Dictionary<string, PersistedVariable>)bin.Deserialize(stream);
                        markPersistedAsUnused();

                        //Debug
                        ILog log = LogManager.GetLogger(Logger.SystemLogger);

                        log.Debug("Start reading persisted variables");
                        foreach (string key in _persistedList.Keys)
                        {
                            log.Debug("Key read: " + key);
                        }
                        log.Debug("Finished reading persisted variables");
                    }
                }
                catch (IOException e)
                {
                    throw new RukisIntegrationTaskhandlerException(
                        string.Format("Deserializing file {0} failed", Misc.PersistedVariablesFile), e);
                }
            }
            else
            {
                _persistedList = new Dictionary<string, PersistedVariable>();
            }
        }

        private void markPersistedAsUnused()
        {
            foreach (KeyValuePair<string, PersistedVariable> pair in _persistedList)
            {
                if (pair.Value.Unused <= Misc.PersistenceUnusedMax)
                {
                    pair.Value.Unused = pair.Value.Unused + 1;
                }
                else
                {
                    // Removing old unused variables
                    _persistedList.Remove(pair.Key);
                }
            }
        }

        private void writePersistedVariables()
        {
            string fullPath = GlobalVariables.getServiceLocation() + Misc.DirectoryEnd + Misc.PersistedVariablesFile;
            try
            {
                using (Stream stream = File.Open(fullPath, FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, _persistedList);
                }
            }
            catch (IOException e)
            {
                throw new RukisIntegrationTaskhandlerException(
                    string.Format("Serializing file {0} failed", Misc.PersistedVariablesFile), e);
            }
        }

        public void registerPersistedVariable(IVariable variable)
        {
            if (_persistedList.ContainsKey(variable.VariableFullName))
            {
                PersistedVariable persistedVariable = _persistedList[variable.VariableFullName];
                persistedVariable.Unused = 0;
                variable.setValue(persistedVariable.Value);
            }
            else
            {
                _persistedList.Add(variable.VariableFullName, new PersistedVariable(variable));
                writePersistedVariables();
            }
        }

        public void settingPersistedVariable(IVariable variable)
        {
            if (checkForPersistedValue(variable.VariableFullName))
            {
                PersistedVariable persistedVariable = _persistedList[variable.VariableFullName];
                persistedVariable.Value = getVariableObject(variable);
            }
            else
            {
                _persistedList.Add(variable.VariableFullName, new PersistedVariable(variable));
            }
            writePersistedVariables();
        }

        public bool checkForPersistedValue(string variableFullName)
        {
            if (_persistedList.ContainsKey(variableFullName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //public void gettingPersistedVariable(IVariable variable)
        //{
        //    PersistedVariable persistedVariable;
        //    if (_persistedList.TryGetValue(variable.FullName, out persistedVariable))
        //    {
        //        variable.setValue(persistedVariable.Value);
        //    }
        //    else
        //    {
        //        throw new FactoryException(GetType().Name, "gettingPersistedVariable", string.Format("Trying to get persisted variable {0} but it does not exist", variable.FullName));
        //    }
        //}

        public void gettingPersistedVariable(IVariableConfiguration variableConfiguration)
        {
            if(_persistedList.ContainsKey(variableConfiguration.FullName))
            {
                variableConfiguration.Value = _persistedList[variableConfiguration.FullName].Value;
            }
        }

        public static object getVariableObject(IVariable variable)
        {
            switch (variable.Type)
            {
                case VariableType.Integer:
                    if (variable is IIntegerVariable)
                    {
                        return (variable as IIntegerVariable).Value;
                    }
                    break;
                case VariableType.String:
                    if (variable is IStringVariable)
                    {
                        return (variable as IStringVariable).Value;
                    }
                    break;
                case VariableType.ListOfString:
                    if (variable is IListOfStringVariable)
                    {
                        return (variable as IListOfStringVariable).getStringList();
                    }
                    break;
                case VariableType.DateTime:
                    if (variable is IDateTimeVariable)
                    {
                        return (variable as IDateTimeVariable).Value;
                    }
                    break;
                case VariableType.Boolean:
                    if (variable is IBooleanVariable)
                    {
                        return (variable as IBooleanVariable).Value;
                    }
                    break;
            }
            // Anything coming here is an error
            throw new JobRunnerException(variable.OwnerTask.OwningJobRunner.JobName, "getVariableObject", "Variable does not exist");
        }

        #endregion

    }
}
