using System.Collections.Generic;

namespace ChefService
{
    /// <summary>
    /// Helps parse and return arguments in an easily useable fashion
    /// </summary>
    public class Arguments
    {
        private Dictionary<string, string> _args = new Dictionary<string, string>();
        private List<string> argsoriginal;
        private bool UpperCaseParameterName = false;
        private bool UpperCaseParameterValue = false;
        private void GetParameter(string Searcharg, out string ArgName, out string value)
        {
            value = null;
            ArgName = null;
            foreach (string parmInArgs in _args.Keys)
            {
                if (UpperCaseParameterName)
                {
                    if (parmInArgs.ToUpper() == Searcharg.ToUpper())
                    {
                        ArgName = parmInArgs.ToUpper();
                        if (UpperCaseParameterValue)
                        {
                            value = _args[parmInArgs].ToUpper();
                        }
                        else
                        {
                            value = _args[parmInArgs];
                        }
                        return;
                    }
                }
                else
                {
                    if (parmInArgs == Searcharg)
                    {
                        ArgName = parmInArgs;
                        if (UpperCaseParameterValue)
                        {
                            value = _args[parmInArgs].ToUpper();
                        }
                        else
                        {
                            value = _args[parmInArgs];
                        }
                        return;
                    }
                }
            }
        }

        public int Count
        {
            get
            {
                return _args.Count;
            }
        }

        public string this[string param]
        {
            get
            {

                string ignore;
                string value = null;
                GetParameter(param, out ignore, out value);
                return value;
                //return _args[param.ToUpper()];
            }
        }

        public bool ContainsKey(string param)
        {
            string ignore = null;
            string value = null;
            GetParameter(param, out ignore, out value);
            return ignore != null;
        }

        public Arguments(string[] args, bool UpperCaseParamName = true, bool UpperCaseParamValue = true)
        {
            argsoriginal = new List<string>();
            for (int i = 0; i < args.Length; i++)
            {
                argsoriginal.Add(args[i]);
            }
            UpperCaseParameterName = UpperCaseParamName;
            UpperCaseParameterValue = UpperCaseParamValue;
            string curParam = null;

            // Valid arguments are in the form of "/param value"
            foreach (string arg in args)
            {
                // Is this a parameter?
                if (arg != "" && (arg[0] == '/' || arg[0] == '-'))
                {
                    // Do we have an unmatched parameter already? Just set it true
                    if (curParam != null)
                        _args.Add(curParam, "");

                    curParam = arg.Substring(1);
                    continue;
                }
                else
                {
                    // It's a value - do we have a parameter?
                    if (curParam != null)
                    {
                        _args.Add(curParam, arg);
                        curParam = null;
                    }
                }
            }

            // One last check to make sure we didn't end with a value-less param
            if (curParam != null)
                _args.Add(curParam, "");
        }
    }
}
