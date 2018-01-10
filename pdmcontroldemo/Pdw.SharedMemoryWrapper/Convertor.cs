
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Pdw.SharedMemoryWrapper
{
    class Convertor
    {
        #region Methods
        public static object ConvertBinArrayToObj(byte[] data)
        {
            if (data == null) return null;

            BinaryFormatter binForm = new BinaryFormatter();
            MemoryStream memStream = new MemoryStream();
            memStream.Write(data, 0, data.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            memStream.Position = 0;
            object obj = binForm.Deserialize(memStream);
            memStream.Flush();
            memStream.Close();
            return obj;
        }

        public static byte[] ObjectToBinArray(object obj)
        {
            if (obj == null) return null;

            BinaryFormatter binForm = new BinaryFormatter();
            MemoryStream memStream = new MemoryStream();
            binForm.Serialize(memStream, obj);
            byte[] arrBytes = memStream.ToArray();
            byte[] result = new byte[arrBytes.Length];
            Buffer.BlockCopy(arrBytes, 0, result, 0, arrBytes.Length);

            memStream.Flush();
            memStream.Close();
            return result;
        }
        #endregion
    }
}
