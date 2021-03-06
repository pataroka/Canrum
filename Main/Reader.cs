﻿using System;
using System.IO;
using System.Text;

namespace Main
{
    public static class Reader
    {
        public static string[][] GetMissionData(string filePath)
        {
            string[][] data = null;

            try
            {
                using (StreamReader sr = new StreamReader(filePath, Encoding.Unicode))
                {
                    string readTheFile = sr.ReadToEnd();
                    string[] dataParts = readTheFile.Split(new[] { '§' }, StringSplitOptions.RemoveEmptyEntries);
                    data = new string[3][];
                    data[0] = new[] {dataParts[0].Replace("%", "\n")};
                    data[1] = dataParts[1].Replace("\r\n", "").Replace("\n", "").Replace("%", "\n").Split('Ẋ');
                    data[2] = dataParts[2].Replace("\r\n", "").Replace("\n", "").Replace("%", "\r\n").Split('Ẋ');
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found!");
            }

            return data;
        }
    }
}

