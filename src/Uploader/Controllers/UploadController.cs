using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Uploader.Models;

namespace Uploader.Controllers
{
    public class UploadController : ApiController
    {
        public string StorageFolder
        {
            get
            {
                return HttpContext
                    .Current
                    .Server
                    .MapPath(ConfigurationManager.AppSettings["StorageFolder"]);
            }
        }
        public long MaxChunkSize
        {
            get
            {
                return long.Parse(HttpContext.Current.Request.QueryString["maxChunkSize"]);
            }
        }
        // GET api/upload
        public IEnumerable<FileStatus> Get()
        {
            return ListCurrentFiles();
        }

        // POST api/upload
        public IEnumerable<FileStatus> Post()
        {
            return UploadFile();
        }

        // PUT api/upload
        public IEnumerable<FileStatus> Put()
        {
            return UploadFile();
        }

        // DELETE api/upload
        public void Delete(string f)
        {
            DeleteFile(f);
        }

        // Delete file from the server
        private void DeleteFile(string f)
        {
            string filePath = Path.Combine(StorageFolder, f);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        private IEnumerable<FileStatus> UploadFile()
        {
            List<FileStatus> statuses = new List<FileStatus>();
            HttpRequestHeaders headers = Request.Headers;
            IEnumerable<string> values;
            HttpContext context = HttpContext.Current;

            if (!headers.TryGetValues("X-File-Name", out values))
            {
                UploadWholeFile(context, statuses);
            }
            else
            {
                UploadPartialFile(values.FirstOrDefault(), context, statuses);
            }

            return statuses;
        }

        private void UploadWholeFile(HttpContext context, List<FileStatus> statuses)
        {
            for (int i = 0; i < context.Request.Files.Count; i++)
            {
                var file = context.Request.Files[i];
                string fullPath = Path.Combine(StorageFolder, Path.GetFileName(file.FileName));
                Directory.CreateDirectory(StorageFolder);
                file.SaveAs(fullPath);
                string fullName = Path.GetFileName(file.FileName);
                statuses.Add(new FileStatus(fullName, file.ContentLength, fullPath));
            }
        }
        private void UploadPartialFile(string fileName, HttpContext context, List<FileStatus> statuses)
        {
            NameValueCollection headers = context.Request.Headers;

            //
            // Retrieve chunks information from the request
            // 
            int chunkSize = int.Parse(headers["Content-Length"]);
            int chunksNumber = int.Parse(headers["X-Chunks-Number"] ?? "0");
            int chunkIndex = int.Parse(headers["X-Chunk-Index"] ?? "0");

            long fileLength = chunkIndex * MaxChunkSize + chunkSize; // in Bytes!
            FileStatus status = new FileStatus(fileName, (int) fileLength, StorageFolder);

            try
            {
                string fullName = Path.Combine(StorageFolder, Path.GetFileName(fileName));
                const int bufferSize = 1024;
                Stream inputStream = context.Request.InputStream;

                // FileShare.Delete for fileuploadfail event
                using (FileStream fileStream =
                    new FileStream(fullName, FileMode.Append, FileAccess.Write, FileShare.Delete))
                {
                    byte[] buffer = new byte[bufferSize];

                    int l;
                    while ((l = inputStream.Read(buffer, 0, bufferSize)) > 0)
                    {
                        fileStream.Write(buffer, 0, l);
                    }

                    fileStream.Flush();
                }
            }
            catch (Exception e)
            {
                status.error = e.Message;
            }

            statuses.Add(status);
        }
        private IEnumerable<FileStatus> ListCurrentFiles()
        {
            FileStatus[] statuses =
                (from file in new DirectoryInfo(StorageFolder).GetFiles("*", SearchOption.TopDirectoryOnly)
                 where !file.Attributes.HasFlag(FileAttributes.Hidden)
                 select new FileStatus(file)).ToArray();
            return statuses;
        }
    } 
}
