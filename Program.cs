using System;
using System.IO;
using System.Text;
using PluginSystem;

namespace MEATranslation
{
    class Program
    {
        public static byte[] rawBuffer;
        //바이너리 파일을 수정하면 PluginSystem에서 꼬임!
        public static byte[] tmpBuffer;
        public static byte[] fileHeader;
        public static TalkTableAsset table;
        public MemoryStream memoryStream;
        
        static void Main(string[] args)
        {
            string path = "mainmenu.res";
            FileStream fileStream = new FileStream("output.res", FileMode.Create);
            BinaryWriter binaryWriter = new BinaryWriter(fileStream);
            
            BinaryReader binaryReader = new BinaryReader(File.Open(Path.GetFullPath("globalmaster.res"), FileMode.Open));
            //BinaryReader binaryReader = new BinaryReader(File.Open(Path.GetFullPath("mainmenu.res"), FileMode.Open));
            
            tmpBuffer = new byte[binaryReader.BaseStream.Length];
            tmpBuffer = binaryReader.ReadBytes(tmpBuffer.Length);
            
            rawBuffer = new byte[tmpBuffer.Length - 16];
            fileHeader = new byte[16];

            Array.Copy(tmpBuffer, 16, rawBuffer,0, (tmpBuffer.Length - 16));
            Array.Copy(tmpBuffer, 0, fileHeader,0, 16);
            
            Console.WriteLine(rawBuffer.Length);
            ReadRawFile(rawBuffer);

            foreach(STR str in table.Strings)
            {
                bool bSuccess;
                if (str.Value != null)
                {
                    bSuccess = EditStringValue(str.ID, str.Value);
                }

            }

            MemoryStream memoryStream = new MemoryStream();
            table.Save(memoryStream);
            
            rawBuffer = memoryStream.ToArray();
            tmpBuffer = new byte[rawBuffer.Length + 16];
            Array.Copy(rawBuffer, 0, tmpBuffer, 16, rawBuffer.Length);
            
            
            Console.WriteLine(rawBuffer.Length);
            
            binaryWriter.Write(tmpBuffer);
            binaryWriter.Close();
        }

        public static void ReadRawFile(byte[] raw)
        {
            //바이트배열을 메모리 스트림에 저장
            table = new TalkTableAsset();
            bool readSuccess = table.Read(new MemoryStream(raw));
            
            if (readSuccess)
            {
                foreach (STR str in table.Strings)
                {
                    Console.WriteLine($"{str.ID}->{str.Value}");
                }
            }
            else
            {
                Console.WriteLine("읽기 실패");
            }
        }

        public static bool EditStringValue(uint ID, string Value)
        {
            foreach (STR str in table.Strings)
            {
                if (str.ID == ID)
                {
                    str.Value = Value;
                    return true;
                }
            }
            return false;
            
        }

        public static void SaveCurrentTable()
        {
            MemoryStream memoryStream = new MemoryStream();
            table.Save(memoryStream);
            rawBuffer = memoryStream.ToArray();
        }
    }
}