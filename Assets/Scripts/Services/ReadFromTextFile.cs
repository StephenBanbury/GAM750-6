using UnityEngine;

namespace Assets.Scripts.Services
{
    public static class GetVideosExternal
    {
        public static string[] GetFromTextFile()
        {
            // Read each line of the file into a string array
            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Public\TestFolder\VideoFiles.txt");

            for (var i = 0; i <= lines.Length - 1; i++)
            {
                Debug.Log($"ReadAllLines: {lines[i]}");
                lines[i] = lines[i].Replace("www.dropbox.com", "dl.dropbox.com").Replace("?dl=0", "");
            }

            return lines;
        }
    }
}
