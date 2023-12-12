using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Universal.DTO.CommonModels.CommonModels;

namespace Services.AWS
{
    public interface IAWSS3FileService
    {
        Task<string> UploadFile(AWSFileUpload files);
        Task<List<string>> FilesList(string folder);
        Task<List<string>> FilesListSearch(string fileName);
        Task<Stream> GetFile(string key);
        // Task<bool> UpdateFile(UploadFileName uploadFileName, string key);
        Task<bool> DeleteFile(string key,int mediaId, int mediaTypeId);
        Task<bool> DeleteMultipleFiles(string[] keys);
    }
    public class AWSS3FileService : IAWSS3FileService
    {
        private readonly IAWSS3BucketHelper _AWSS3BucketHelper;

        public AWSS3FileService(IAWSS3BucketHelper AWSS3BucketHelper)
        {
            this._AWSS3BucketHelper = AWSS3BucketHelper;
        }
        public async Task<string> UploadFile(AWSFileUpload files)
        {
            string imageName = "";
			byte[] fileBytes;
            if (files.Attachments.Count == 0) return null;
            foreach (var file in files.Attachments)
            {

                if (file.Length > 0)
                {
					string timeStamp = GetTimestamp(DateTime.Now);
					string imageNametmp = file.FileName;
                    imageName = "Universal/" + Path.GetFileNameWithoutExtension(imageNametmp) +"_"+ timeStamp + Path.GetExtension(imageNametmp);
                    
					using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }
                    try
                    {
                        Stream stream = new MemoryStream(fileBytes);
                        await _AWSS3BucketHelper.UploadFile(stream, imageName);
                        //File.WriteAllBytes(imgPath, fileBytes);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw e;
                        //return false;
                    }
                }

                //return true;
            }
            return imageName;
        }
        public async Task<List<string>> FilesList(string folder)
        {
            try
            {
                var listObjectsContainer = await _AWSS3BucketHelper.FilesList();
                List<string> keys = new List<string>();
                foreach (var listObjects in listObjectsContainer)
                {
                    keys.AddRange(listObjects.S3Objects.Where(x => folder == null ? true : x.Key.Contains(folder.ToUpper())).Select(c => new { c.LastModified, c.Key }).OrderByDescending(e => e.LastModified)
                    .Select(e => e.Key).ToList());
                }
                return keys;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<string>> FilesListSearch(string fileName)
        {
            try
            {
                return await _AWSS3BucketHelper.FilesListSearch(fileName);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

		public static String GetTimestamp(DateTime value)
		{
			return value.ToString("yyyyMMddHHmmssffff");
		}

		public async Task<Stream> GetFile(string key)
        {
            try
            {
                Stream fileStream = await _AWSS3BucketHelper.GetFile(key);
                if (fileStream == null)
                {
                    Exception ex = new Exception("File Not Found");
                    throw ex;
                }
                else
                {
                    return fileStream;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public async Task<bool> UpdateFile(UploadFileName uploadFileName, string key)
        //{
        //    try
        //    {
        //        var path = Path.Combine("Files", uploadFileName.ToString() + ".png");
        //        using (FileStream fsSource = new FileStream(path, FileMode.Open, FileAccess.Read))
        //        {
        //            return await _AWSS3BucketHelper.UploadFile(fsSource, key);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public async Task<bool> DeleteFile(string key, int mediaId, int mediaTypeId)
        {
            try
            {
                return await _AWSS3BucketHelper.DeleteFile(key, mediaId, mediaTypeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteMultipleFiles(string[] keys)
        {
            try
            {
                return await _AWSS3BucketHelper.DeleteMultipleFiles(keys);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
