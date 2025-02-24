using Firebase.Storage;
using FirebaseAdmin;
using Google.Api.Gax.ResourceNames;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensions.Msal;
using TodoAPI.Models;

namespace TodoAPI.Services
{
    public class FirebaseStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;
        private readonly string _rootFolder = "car-wash";

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

        public async Task<string> UploadFileAsync(IFormFile file, string? category)
        {
            // Generate unique filename
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.Name);
            var filePath = string.Empty;
            //Check if category is empty or not
            //If empty, the file will be saved in the root folder on Firebase
            if (string.IsNullOrEmpty(category))
            {
                filePath = $"{_rootFolder}/{fileName}";
            }
            //If not empty, the file will be save inside the folder with the given category name
            else
            {
                if(category.EndsWith("/"))
                {
                    filePath = $"{_rootFolder}/{category}{fileName}";
                }
                else
                {
                    filePath = $"{_rootFolder}/{category}/{fileName}";
                }
            }
           

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

        //Remove a file from Firebase using its filePath
        public async Task DeleteImageAsync(string filePath)
        {
           
            await _storageClient.DeleteObjectAsync(_bucketName, filePath);
         
        }

    }
}
