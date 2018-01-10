using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

namespace testSQL
{
    [Serializable]
    public class FileBufferedStream : Stream, ISerializable
    {
        /// <summary>
        /// if the string size exceed this value, we use file to buffer it to aviod out of memory exception.
        /// </summary>
        public const long MaxMemoryStringLength = 10 * 1024 * 1024; //10m, 

        private string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
        private string FileName;

        /// <summary>
        /// identify whether the string is file or memory.
        /// </summary>
        private bool useFile = false;

        private MemoryStream msData;
        private FileStream fileData;

        public FileBufferedStream()
        {
            Init();
        }
        #region custom serialize, it will be raise out of memory exception while the content is large size.
        //for custom deserialize.
        private FileBufferedStream(SerializationInfo info, StreamingContext ctxt)
        {
            Init();
            String allData = (string)info.GetValue("allData", typeof(string));
            Append(allData);
        }
        //custom serialize
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            string allData = Reader().ReadToEnd();
            info.AddValue("allData", allData);
        }
        #endregion
        private void Init()
        {
            //win7, c:\user\username\appdata\local\temp
            tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
            msData = new MemoryStream();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        public void Append(string content)
        {
            //convert to byte[] then write to stream.
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Get stream which represent current content. Make sure to call Dispose() after this method.
        /// </summary>
        /// <returns></returns>
        public Stream GetStream()
        {
            if (useFile)
            {
                fileData.Seek(0, SeekOrigin.Begin);
                return fileData;
            }
            else
            {
                msData.Seek(0, SeekOrigin.Begin);
                return msData;
            }
        }
        /// <summary>
        /// return a reader which represent current content.
        /// NOTICE: If you want to use this reader as parameter of XmlSerializer, do not create 
        /// new xmlreader from this, just use it.
        /// </summary>
        /// <returns></returns>
        public StreamReader Reader()
        {
            if (useFile)
            {
                fileData.Seek(0, SeekOrigin.Begin);
                return new StreamReader(fileData, encoding: Encoding.UTF8);
            }
            else
            {
                msData.Seek(0, SeekOrigin.Begin);
                return new StreamReader(msData, encoding: Encoding.UTF8);
            }
        }

        public override void Close()
        {
            if (useFile)
            {
                fileData.Close();
            }
            else
            {
                msData.Close();
            }
        }
        /// <summary>
        /// Clear object after read finish. It is import to call this method, otherwise there are 
        /// many death-wood files in tempory folder of System. Please call this method on FileBufferdStream object NOT Stream object.
        /// </summary>
        public void Dispose()
        {
            if (useFile)
            {
                fileData.Dispose();
                if (File.Exists(FileName))
                    File.Delete(FileName);
            }
            else
            {
                msData.Dispose();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Dispose();
        }

        /// <summary>
        /// create a temp file to store the content.
        /// </summary>
        /// <returns></returns>
        private FileStream InitTempFile()
        {
            FileName = tempFolder + "\\" + Guid.NewGuid().ToString() + ".pdcache";
            if (File.Exists(FileName))
                File.Delete(FileName);
            FileStream result = new FileStream(FileName, FileMode.CreateNew, FileAccess.ReadWrite);
            return result;
        }

        public override bool CanRead
        {
            get
            {
                if (useFile)
                    return fileData.CanRead;
                else
                    return msData.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                if (useFile)
                    return fileData.CanSeek;
                else
                    return msData.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                if (useFile)
                    return fileData.CanWrite;
                else
                    return msData.CanWrite;
            }
        }

        public override void Flush()
        {
            if (useFile)
                fileData.Flush();
            else
                msData.Flush();
        }

        public override long Length
        {
            get
            {
                if (useFile)
                    return fileData.Length;
                else
                    return msData.Length;
            }
        }

        public override long Position
        {
            get
            {
                if (useFile)
                    return fileData.Position;
                else
                    return msData.Position;
            }
            set
            {
                if (useFile)
                    fileData.Position = value;
                else
                    msData.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (useFile)
            {
                return fileData.Read(buffer, offset, count);
            }
            else
            {
                return msData.Read(buffer, offset, count);
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (useFile)
            {
                return fileData.Seek(offset, origin);
            }
            else
            {
                return msData.Seek(offset, origin);
            }
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            //first check use file or not.
            if (!useFile)
            {
                if (msData.Length >= MaxMemoryStringLength)//change to use file stream
                {
                    useFile = true;
                    fileData = InitTempFile();
                    //write all the existed content to file and close memory stream.
                    msData.WriteTo(fileData);
                    msData.Close();
                    msData.Dispose();
                }
            }

            if (useFile)
            {
                fileData.Write(buffer, offset, count);
            }
            else
            {
                msData.Write(buffer, offset, count);
            }
        }
    }
}
