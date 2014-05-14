using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileManWPF {
    class RawFile{
        private String _path;
        private List<int> _exclusionList;
        private List<int> _idColumnsList;//the column indices that will be unique identifiers for compilation file(so they can map to each other)
        private char _separator = ',';
        private List<string> _dataTypes;

        #region Constructors

        public RawFile(String path) {
            _path = path;
            _exclusionList = new List<int>();
            _idColumnsList = new List<int>();
        }

        #endregion


        #region Properties

        //default is ','
        public char Separator{
            get{ return _separator;}
            set{_separator = value;}
        }

        public List<int> ExclusionList {
            get { return _exclusionList; }
            set { _exclusionList = value; }
        }

        public List<int> IDColumnList {
            get { return _idColumnsList; }
            set { _idColumnsList = value; }
        }

        #endregion


        #region Methods to manipulate properties

        public void addIDColumnNumber(int n) {
            if (n < 0)
                throw new ArgumentException();

            _idColumnsList.Add(n);
        }

        public void addExclusion(int n) {
            if (n < 0)
                throw new ArgumentException();

            _exclusionList.Add(n);
        }

        #endregion


        public StreamReader GetReader() {
            return new StreamReader(_path);
        }

        public List<List<String>> GetSample(int size) {
            List<List<String>> result = new List<List<string>>();
            StreamReader reader = GetReader();

            while (!reader.EndOfStream && result.Count != size) {
                string line = reader.ReadLine();
                string[] data = line.Split(_separator);
                result.Add(data.ToList<String>());
            }


            return result;
        }

        public string GenerateID(string s) {
            
            if (s == null)
                throw new ArgumentNullException();
            else if (s.Length == 0)
                throw new ArgumentException();

            StringBuilder sb = new StringBuilder();
            string[] data = s.Split(_separator);

            foreach(int i in _idColumnsList){
                sb.Append(data[i]);
            }

            return sb.ToString();

        }

        public string GenerateResult(string s) {
            if (s == null)
                throw new ArgumentNullException();
            else if (s.Length == 0)
                throw new ArgumentException();

            StringBuilder sb = new StringBuilder();
            string[] splitInfo = s.Split(_separator);

            for (int i = 0; i < splitInfo.Length; i++) {
                if (!_exclusionList.Contains(i)) {
                    if (sb.Length > 0) {
                        sb.Append(_separator);
                    }
                    sb.Append(splitInfo[i]);
                    
                }
            }

            return sb.ToString();
        }


        private List<String> DetermineDataTypes() {
            List<String> result = new List<String>();

            try {
                StreamReader reader = new StreamReader(_path);

                while (!reader.EndOfStream) {
                    String line = reader.ReadLine();
                    string[] data = line.Split(_separator);

                    if (result.Count == 0) {
                        result = new List<string>(data.Length);
                    }

                    for (int i = 0; i < data.Length; i++) {
                        string s = data[i];
                        double attempt = 0.0;
                        bool isANumber = double.TryParse(s, out attempt);

                        if (!isANumber) {
                            if (result[i] == "numeric" || result[i] == "") {
                                result[i] = "nominal";
                            }
                        }
                        else {
                            if (result[i] == "") {
                                result[i] = "numeric";
                            }
                        }
                    }
                }
                reader.Close();
            }
            catch (IOException ex) {
                System.Console.Error.WriteLine("IO Error occured in " + ex.Source + Environment.NewLine
                    + ex.Message + Environment.NewLine +
                    "Stack Trace:" + Environment.NewLine
                    + ex.StackTrace);
            }

            return result;
        }
    }
}
