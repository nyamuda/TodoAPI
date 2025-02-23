using Firebase.Storage;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Identity.Client.Extensions.Msal;

namespace TodoAPI.Services
{
    public class FirebaseStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;

        public FirebaseStorageService(IConfiguration config)
        {
            //Get the Firebase Storage configuration
            var firebaseConfig = config.GetSection("Authentication:Firebase");

            //Location of the service path
            var serviceAccountPath = firebaseConfig["ServiceAccountPath"] ?? throw new InvalidOperationException("Firebase service account configuration is missing.");

            // Load credentials explicitly from the root folder
            var credential = GoogleCredential.FromFile(
                Path.Combine(AppContext.BaseDirectory, serviceAccountPath)
            );
            _storageClient = StorageClient.Create(credential);

            //The firebase storage bucket name
            _bucketName = firebaseConfig["Bucket"] ?? throw new InvalidOperationException("Firebase bucket configuration is missing.");

        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            //check if folderName ends with a "/"
            if(!folderName.EndsWith("/"))
            {
                folderName += "/";
            }
            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = $"car-wash/{folderName}{fileName}";

            // Upload to Firebase Storage
            using var stream = file.OpenReadStream();
            await _storageClient.UploadObjectAsync(
               bucket: _bucketName,
               objectName: filePath,
               contentType: file.ContentType,
               source: stream
            );

            // Return public URL
            return $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/{Uri.EscapeDataString(filePath)}?alt=media";
        }

    }
}
