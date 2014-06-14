using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;

namespace FileManWPF {
    class RawFile{
        private String _path;
        private List<int> _exclusionList;
        private List<int> _idColumnsList;//the column indices that will be unique identifiers for compilation file(so they can map to each other)
        private char _separator = ',';
        private Dictionary<string,bool> _headers;
        private bool _hasHeaders;

        #region Constructors

        public RawFile(String path, bool hasHeaders) {
            _path = path;
            _exclusionList = new List<int>();
            _idColumnsList = new List<int>();
            _hasHeaders = hasHeaders;
            _headers = new Dictionary<string, bool>();
            getHeaders();
        }

        #endregion


        #region Properties

        //default is ','
        public char Separator{
            get{ return _separator;}
            set{_separator = value;}
        }

        public Dictionary<String,bool> Headers {
            get { return _headers; }
            set { _headers = value; }
        }

        public bool HasHeader {
            get { return _hasHeaders; }
        }

        public List<int> ExclusionList {
            get {
                return _exclusionList;
            }
            set { _exclusionList = value; }
        }

        #endregion


        public TextReader GetReader() {
            return new StreamReader(_path);
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

        public void parseAndSetID(String s) {
            String[] data = s.Split(',');
            _idColumnsList.Clear();

            foreach(String entry in data){
                int n = int.Parse(entry);
                _idColumnsList.Add(n);
            }
        }

        //aft is to make sure there are no conflicts between duplicate heading names
        public List<string> GetResultHeaders(string aft) {
            List<string> result = new List<string>();
            for (int index = 0; index < _headers.Count; index++) {
                var item = _headers.ElementAt(index);
                if (item.Value == true) {
                    result.Add(item.Key + aft);
                }
            }
            return result;
        }

        public string GenerateResult(string s) {
            if (s == null)
                throw new ArgumentNullException();
            else if (s.Length == 0)
                throw new ArgumentException();

            List<int> result = new List<int>();
            for (int index = 0; index < _headers.Count; index++) {
                var item = _headers.ElementAt(index);
                if (item.Value == false) {
                    result.Add(index);
                }
                
                this._exclusionList = result;
            }

            StringBuilder sb = new StringBuilder();
            string[] splitInfo = s.Split(_separator);

            for (int i = 0; i < splitInfo.Length; i++) {
                if (!this.ExclusionList.Contains(i)) {
                    if (sb.Length > 0) {
                        sb.Append(_separator);
                    }
                    sb.Append(splitInfo[i]);
                    
                }
            }

            return sb.ToString();
        }

        private void getHeaders() {

            if (!_hasHeaders)
                return;
            TextReader reader = GetReader();
            String line = reader.ReadLine();
            List<String> headers = line.Split(_separator).ToList<String>();
            headers.ForEach(
                delegate(String header) {
                    _headers[header] = true;
                });
            reader.Close();
        }

        public List<List<String>> GetPreview(int numOfRecords) {
            List<List<String>> result = new List<List<String>>();
            TextReader reader = GetReader();

            if (HasHeader)
                reader.ReadLine();

            while (reader.Peek() != -1 && result.Count < numOfRecords) {
                String line = reader.ReadLine();
                String[] data = line.Split(_separator);

                result.Add(data.ToList<String>());
            }

            reader.Close();
            return result;
        }

        public ObservableCollection<object> GetDataPreview(int numOfRecords) {
            ObservableCollection<object> result = new ObservableCollection<object>();

            List<List<String>> preview = GetPreview(5);
            List<String> headers = GetHeaders();

            foreach (List<String> line in preview) {
                dynamic dd = new BindableDynamicDictionary();
                for (int i = 0; i < headers.Count; i++) {
                    String header = headers[i];
                    dd[header] = line[i];
                }

                result.Add(dd);
            }
            return result;
        }

        public List<String> GetHeaders() {
            return this.Headers.Keys.ToList();
        }
    }
}
