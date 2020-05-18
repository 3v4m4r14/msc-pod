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
    class FileHandler
    {
        private readonly string FilePath;

        public FileHandler(string fileName)
        {
            Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "data"));
            FilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "data", fileName);
        }

        public void WriteToFile(object jsonObject)
        {
            string objectInJson = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
            Console.WriteLine("Object is: " + objectInJson);


            // Write the string array to a new file named "WriteLines.txt".
            using (StreamWriter outputFile = new StreamWriter(FilePath))
            {
                outputFile.WriteLine(objectInJson);
            }
        }


        public Gallery ReadFromFile()
        {
            string strResultJson = String.Empty;

            if (!File.Exists(FilePath))
            {
                return new Gallery();
            }

            strResultJson = File.ReadAllText(FilePath);
            Gallery galleryItem = JsonConvert.DeserializeObject<Gallery>(strResultJson);

            return galleryItem;
        }

        private void CreateFile()
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(FilePath)) { }
        }
    }
}
