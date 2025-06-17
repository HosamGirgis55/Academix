using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Newtonsoft.Json;

namespace Academix.Helper
{
    public class FileUploaderHelper
    {
        // في الملف دة هبدا اعمل كلاس علشان يرفع الداتا علي مساحة تخزين
        // تبع الفيربيز وابعت في النهاية لينك ليها 
        private readonly IConfiguration _config;
        private readonly StorageClient _storageClient;
        private readonly string _bucket;

        public FileUploaderHelper(IConfiguration config)
        {
            _config = config;
            var firebaseSection = config.GetSection("Firebase");
            var firebaseJson = JsonConvert.SerializeObject(firebaseSection.GetChildren().ToDictionary(x => x.Key, x => x.Value));
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(firebaseJson));

            var credential = GoogleCredential.FromStream(stream);
            _storageClient = StorageClient.Create(credential);

            _bucket = firebaseSection["bucket"];

        }
     
        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            var obj = await _storageClient.UploadObjectAsync(
            bucket: _bucket,
            objectName: $"uploads/{fileName}",
            contentType: contentType,
            source: fileStream
        );

            return $"https://storage.googleapis.com/{_bucket}/uploads/{fileName}";
        }
    }
}
