using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileManWPF {
    class TestingClass {

        public static void main(String[] args)
        {
            System.Console.WriteLine("Testing Class has started");
        }

        

        public void writeFile(String path, IEnumerable<String> data) {
            StreamWriter writer = new StreamWriter(path);

            IEnumerator<String> iterator = data.GetEnumerator();
            bool hasMore = true;

            while (hasMore) {
                writer.WriteLine(iterator.ToString());
                hasMore = iterator.MoveNext();
            }

            writer.Flush();
            writer.Close();
        }

        public List<String> readFile(String path,int num) 
        {
            System.Console.WriteLine("readFile(String path) has started");
            List<String> result = new List<String>();
            StreamReader reader = new StreamReader(path);

            while (!reader.EndOfStream) 
            {
                String line = reader.ReadLine();
                result.Add(line);

                if (result.Count == num)
                {
                    return result;
                }
            }
            reader.Close();
            System.Console.WriteLine("readFile has finished");
            return result;
        }
    }
}
