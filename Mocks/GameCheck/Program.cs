using System;
using System.Diagnostics;
using System.IO;

namespace GameCheck
{
    class Program
    {
        // Keys
        #region Keys
        
        struct KeyValues
        {
            public string grp;
            public string gsp;
            public string rf;

            public bool IsValid
            {
                get 
                {
                    return !string.IsNullOrEmpty(grp) &&
                        !string.IsNullOrEmpty(gsp) &&
                        !string.IsNullOrEmpty(rf); 
                }
            }
        }

        private class UnknownKeyException : Exception
        {
            public UnknownKeyException(string message)
                :base(message)
            {}
        }

        #endregion

        // Valid parameter set:
        // "-grp \"{0}\" -gsp \"{1}\" -rf \"{2}\""
        // Where :
        // {0} -- pathProjects = Path.Combine(pathRelease, "projects");
        // {1} -- srcInfo = new DirectoryInfo(pathProjects); gsp = srcInfo.GetDirectories().FirstOrDefault(d => d.Name.StartsWith("Game")).FullName;
        // {2} -- checkResult = Application.LocalUserAppDataPath + "\\gameCheckResult.xml";
        static void Main(string[] args)
        {
            var values = new KeyValues();
            try {
                for (var i = 0; i < 6; ++i) {
                    var key = ((string)args.GetValue(i)).Trim();
                    switch (key) {
                        case "-grp":
                            values.grp = args.GetValue(++i) as string;
                            break;
                        case "-gsp":
                            values.gsp = args.GetValue(++i) as string;
                            break;
                        case "-rf":
                            values.rf = args.GetValue(++i) as string;
                            break;
                        default:
                            throw new UnknownKeyException("Unknown key '" + key + "'");
                    }
                }
            } catch (IndexOutOfRangeException e) {
                Console.WriteLine("Error: " + e.Message);
                return;
            } catch (UnknownKeyException e) {
                Console.WriteLine("Error: " + e.Message);
                return;
            }

            if (!values.IsValid) {
                Console.WriteLine("Error: " + "Not enough keys");
                return;
            }

            // In this version gsp and grp considered always correct
            Debug.Assert(values.rf != null, "values.rf != null");
            File.WriteAllText(values.rf, Properties.Resources.gameCheckResultValid);
        }
    }
}
