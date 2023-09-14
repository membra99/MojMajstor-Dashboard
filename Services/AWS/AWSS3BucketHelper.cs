using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Util;
using Entities.Context;
using Entities.Universal.MainData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using NetTopologySuite.Index.HPRtree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Universal.DTO.CommonModels.CommonModels;

namespace Services.AWS
{
	public interface IAWSS3BucketHelper
	{
		Task<bool> UploadFile(System.IO.Stream inputStream, string fileName);

		Task<List<ListObjectsV2Response>> FilesList();

		Task<List<string>> FilesListSearch(string fileName);

		Task<Stream> GetFile(string key);

		Task<bool> DeleteFile(string key);

		Task<bool> DeleteMultipleFiles(string[] keys);
	}

	public class AWSS3BucketHelper : IAWSS3BucketHelper
	{
		private readonly IAmazonS3 _amazonS3;
		private readonly ServiceConfiguration _settings;
		private readonly MainContext _context;
		private readonly AmazonS3Config _s3Config;

		public AWSS3BucketHelper(IAmazonS3 s3Client, IOptions<ServiceConfiguration> settings, MainContext context)
		{
			_context = context;
			_s3Config = new AmazonS3Config
			{
				RegionEndpoint = RegionEndpoint.GetBySystemName("eu-central-1")
			};
			s3Client = new AmazonS3Client(_s3Config);
			this._amazonS3 = s3Client;
			this._settings = settings.Value;
		}

		public async Task<bool> UploadFile(System.IO.Stream inputStream, string fileName)
		{
			try
			{
				PutObjectRequest request = new PutObjectRequest()
				{
					InputStream = inputStream,
					BucketName = _settings.AWSS3.BucketName,
					Key = fileName
				};
				PutObjectResponse response = await _amazonS3.PutObjectAsync(request);
				if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
					return true;
				else
					return false;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public async Task<List<ListObjectsV2Response>> FilesList()
		{
			List<ListObjectsV2Response> listObjectsV2Responses = new List<ListObjectsV2Response>();
			ListObjectsV2Request request = new ListObjectsV2Request
			{
				BucketName = _settings.AWSS3.BucketName,
				MaxKeys = 1000
			};
			ListObjectsV2Response response;
			do
			{
				response = await _amazonS3.ListObjectsV2Async(request);

				listObjectsV2Responses.Add(response);

				request.ContinuationToken = response.NextContinuationToken;
			} while (response.IsTruncated);

			return listObjectsV2Responses;
		}

		public async Task<List<string>> FilesListSearch(string fileName)
		{
			var data = _amazonS3.ListVersionsAsync(_settings.AWSS3.BucketName).Result;
			ListObjectsV2Request req = new ListObjectsV2Request
			{
				BucketName = _settings.AWSS3.BucketName
			};
			return _amazonS3.ListObjectsV2Async(req).Result.S3Objects.Where(e => e.Key.Contains(fileName)).Select(e => _settings.AWSS3.BucketURL + e.Key).ToList();
		}

		public async Task<Stream> GetFile(string key)
		{
			GetObjectRequest request = new GetObjectRequest
			{
				BucketName = _settings.AWSS3.BucketName,
				Key = key
			};
			GetObjectResponse response = await _amazonS3.GetObjectAsync(request);
			if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
				return response.ResponseStream;
			else
				return null;
		}

		public async Task<bool> DeleteFile(string key)
		{
			try
			{
				var index = key.LastIndexOf('/');
				var newKey = key.Substring(index + 1);
				var deleteRequest = new DeleteObjectRequest
				{
					BucketName = _settings.AWSS3.BucketName,
					Key = "DOT/" + newKey
				};
				DeleteObjectResponse response = await _amazonS3.DeleteObjectAsync(deleteRequest);
				if (response.HttpStatusCode == System.Net.HttpStatusCode.NoContent)
				{
					var media = await _context.Medias.Where(x => x.Src == key).SingleOrDefaultAsync();
					var user = await _context.Users.Where(x => x.MediaId == media.MediaId).SingleOrDefaultAsync();
					user.MediaId = null;
					_context.Entry(user).State = EntityState.Modified;
					await _context.SaveChangesAsync();
					if (media != null)
					{
						_context.Medias.Remove(media);
						await _context.SaveChangesAsync();
					}
					return true;
				}
				else
				{
					return false;
				}
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
				List<KeyVersion> keysProper = new List<KeyVersion>();
				foreach (var item in keys)
				{
					var index = item.LastIndexOf('/');
					var newKey = item.Substring(index + 1);
					KeyVersion keyVersion = new KeyVersion
					{
						Key = "DOT/" + newKey,
						// For non-versioned bucket operations, we only need object key.
					};
					keysProper.Add(keyVersion);
				}

				DeleteObjectsRequest multiObjectDeleteRequest = new DeleteObjectsRequest
				{
					BucketName = _settings.AWSS3.BucketName,
					Objects = keysProper // This includes the object keys and null version IDs.
				};
				DeleteObjectsResponse response = await _amazonS3.DeleteObjectsAsync(multiObjectDeleteRequest);
				if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
				{
					for (int i = 0; i < keys.Length; i++)
					{
						var media = await _context.Medias.Where(x => x.Src == keys[i]).SingleOrDefaultAsync();
						var user = await _context.Users.Where(x => x.MediaId == media.MediaId).SingleOrDefaultAsync();
						user.MediaId = null;
						_context.Entry(user).State = EntityState.Modified;
						await _context.SaveChangesAsync();

						_context.Medias.Remove(media);
						await _context.SaveChangesAsync();
					}
					return true;
				}
				else
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}