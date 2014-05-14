using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileManWPF {
    class FileBuilder {
        List<String> _finalFileData;
        Dictionary<String,List<int>> _sourceFiles;// the key is the path, the value is the column exclusion list
        List<int> idFileA;
        List<int> idFileB;
        RawFile _fileA, _fileB;
        char _separator;

        public FileBuilder() {
            _finalFileData = new List<string>();
            _sourceFiles = new Dictionary<string, List<int>>();
            _separator = ',';
        }

        public FileBuilder(char sep, string fileA, string fileB){
            _finalFileData = new List<string>();
            _sourceFiles = new Dictionary<string, List<int>>();
            _separator = sep;
            _fileA = new RawFile(fileA);
            _fileB = new RawFile(fileB);
        }

        public RawFile FileA {
            get { return _fileA; }
            set { _fileA = value; }
        }

        public RawFile FileB {
            get { return _fileB; }
            set { _fileB = value; }
        }

        //chunkSize says how many lines should be processed of the files, to avoid memory problems if sourceFiles are too big
        public void ProcessData(int chunkSize, string destination) {
            try {
                StreamReader readerFileA = _fileA.GetReader();
                StreamReader readerFileB = _fileB.GetReader();

                while (!readerFileA.EndOfStream) {
                    string lineA = readerFileA.ReadLine();
                    string idA = _fileA.GenerateID(lineA);

                    while (!readerFileB.EndOfStream) {
                        string lineB = readerFileB.ReadLine();
                        string idB = _fileB.GenerateID(lineB);

                        if (idA == idB) {
                            _finalFileData.Add(_fileA.GenerateResult(lineA) + _fileA.Separator + _fileB.GenerateResult(lineB));

                            if (_finalFileData.Count == chunkSize) {
                                writeResults(destination);
                            }
                        }
                    }
                }
            }
            catch (IOException ex) {
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


    }
}
