using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GalleryOfHeartbeats.Model
{
    class FileReaderWriter
    {
        private readonly string FilePath;

        public FileReaderWriter(string fileName)
        {
            FilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "data", fileName);
        }

        public void WriteToFile(object jsonObject)
        {
            string objectInJson = JsonConvert.SerializeObject(jsonObject);
            Console.WriteLine("Object is: " + objectInJson);

            if (!File.Exists(FilePath))
            {
                CreateFile();
            }

            // Write the string array to a new file named "WriteLines.txt".
            using (StreamWriter outputFile = File.AppendText(FilePath))
            {
                outputFile.WriteLine(objectInJson);
            }
        }

        private void CreateFile()
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(FilePath)) { }
        }

        public Gallery ReadFromFile()
        {
            string strResultJson = String.Empty;

            strResultJson = File.ReadAllText(FilePath);
            Gallery galleryItem = JsonConvert.DeserializeObject<Gallery>(strResultJson);

            return galleryItem;
        }
    }
}
