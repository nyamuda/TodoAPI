using Firebase.Storage;
using FirebaseAdmin;
using Google.Cloud.Storage.V1;
using Microsoft.Identity.Client.Extensions.Msal;

namespace TodoAPI.Services
{
    public class FirebaseStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;

        public FirebaseStorageService()
        {
            // Initialize Google Cloud Storage client
            _storageClient = StorageClient.Create();
            _bucketName = "drivingschool-7c02e.appspot.com"; // Your Firebase Storage bucket name

        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            // Generate unique filename (optional)
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = $"car-wash/{fileName}"; 

            // Upload to Firebase Storage
            using var stream = file.OpenReadStream();
            await _storageClient.UploadObjectAsync(
                _bucketName,
                filePath,
                file.ContentType,
                stream
            );

            // Return public URL
            return $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/{Uri.EscapeDataString(filePath)}?alt=media";
        }

    }
}
