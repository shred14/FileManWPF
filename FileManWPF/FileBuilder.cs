using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;

namespace FileManWPF {
    class FileBuilder {
        List<String> _finalFileData;
        Dictionary<String,List<int>> _sourceFiles;// the key is the path, the value is the column exclusion list
        RawFile _fileA, _fileB;
        char _separator;
        ObservableCollection<object> _fileADataPreview, _fileBDataPreview;

        public FileBuilder() {
            _finalFileData = new List<string>();
            _sourceFiles = new Dictionary<string, List<int>>();
            _separator = ',';
            _fileADataPreview = new ObservableCollection<object>();
            _fileBDataPreview = new ObservableCollection<object>();
        }

        public FileBuilder(char sep){
            _finalFileData = new List<string>();
            _sourceFiles = new Dictionary<string, List<int>>();
            _separator = sep;
            _fileADataPreview = new ObservableCollection<object>();
            _fileBDataPreview = new ObservableCollection<object>();
        }

        public RawFile FileA {
            get { return _fileA; }
            set { _fileA = value;
                   _fileADataPreview = _fileA.GetDataPreview(5);
                }
        }

        public RawFile FileB {
            get { return _fileB; }
            set { _fileB = value;
                _fileBDataPreview = _fileB.GetDataPreview(5);
            }
        }

        public ObservableCollection<object> FileAPreviewData {
            get { return _fileADataPreview;}
        }

        public ObservableCollection<object> FileBPreviewData{
            get { return _fileBDataPreview;}
        }

        public void ManageExclusionChange(string content, bool include) {


            
            char file = content[content.Length - 1];//getting the last character to determine if fileA or fileB
            content = content.Remove(content.Length - 2);//removing so that header can be matched properly

            if (file == 'A') {
                this.FileA.Headers[content] = include;
            }
            else if (file == 'B') {
                this.FileB.Headers[content] = include;
            }
            else {
                MessageBox.Show("There was a problem in changing the exclusion");
            }
        }
        

        //chunkSize says how many lines should be processed of the files, to avoid memory problems if sourceFiles are too big
        public void ProcessData(int chunkSize, string destination) {
            try {
                TextReader readerFileA = _fileA.GetReader();
                

                if(_fileA.HasHeader)
                    readerFileA.ReadLine();

                List<string> headers = _fileA.GetResultHeaders();
                headers.AddRange(_fileB.GetResultHeaders());
                string line = "";

                foreach (string s in headers) {
                    line += s + ",";
                }

                line = line.Remove(line.Length - 1);

                    
                StreamWriter writer = new StreamWriter(destination, true);
                writer.WriteLine(line);
                writer.Flush();
                writer.Close();


                while (readerFileA.Peek() != -1) {
                    
                    string lineA = readerFileA.ReadLine();

                    string idA = _fileA.GenerateID(lineA);
                    TextReader readerFileB = _fileB.GetReader();
                    
                    if (_fileB.HasHeader)
                        readerFileB.ReadLine();

                    while (readerFileB.Peek() != -1) {
                        string lineB = readerFileB.ReadLine();
                        string idB = _fileB.GenerateID(lineB);

                        if (idA == idB) {
                            _finalFileData.Add(_fileA.GenerateResult(lineA) + _fileA.Separator + _fileB.GenerateResult(lineB));

                            if (_finalFileData.Count >= chunkSize) {
                                writeResults(destination);
                            }
                        }
                    }
                }
                writeResults(destination);
                MessageBox.Show("File has been written");
            }
            catch (IOException ex) {
                MessageBox.Show("I have an exception");
                System.Console.Error.WriteLine("IO Error occured in " + ex.Source + Environment.NewLine
                    + ex.Message + Environment.NewLine +
                    "Stack Trace:" + Environment.NewLine
                    + ex.StackTrace);
            }
        }


        private void writeResults(string destination) {
            try {
                StreamWriter writer = new StreamWriter(destination, true);

                while (_finalFileData.Count > 0) {
                    string line = _finalFileData[0];
                    writer.WriteLine(line);
                    _finalFileData.RemoveAt(0);
                }
                writer.Flush();
                writer.Close();
            }
            catch (IOException ex) {
                System.Console.Error.WriteLine("IO Error occured in " + ex.Source + Environment.NewLine 
                    + ex.Message + Environment.NewLine +
                    "Stack Trace:" + Environment.NewLine
                    + ex.StackTrace);
            }
        }




        public void ParseAndSetIDs(string fileAID, string fileBID) {
            setID(_fileA, fileAID);
            setID(_fileB, fileBID);
        }

        private void setID(RawFile file, string p) {
            String[] data = p.Split(',');

            foreach (String entry in data) {
                int n = int.Parse(entry);
                file.IDColumnList.Add(n);
            }
        }
    }
}
