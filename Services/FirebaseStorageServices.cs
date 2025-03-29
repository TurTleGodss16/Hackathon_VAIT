using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Google.Cloud.Firestore;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Forms;

namespace Hackathon_VAIT_New.Services
{
    public class FirebaseStorageServices
    {
        private readonly StorageClient _storageClient;
        private readonly FirestoreDb _firestoreDb;

        private string firebaseStorageBucket, projectID;

        public FirebaseStorageServices()
        {
            try
            {
                firebaseStorageBucket = Environment.GetEnvironmentVariable("FIREBASE_STORAGE_BUCKET") ?? "invalid name";
                projectID = Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID") ?? "invalid name";

                if (FirebaseApp.DefaultInstance == null)
                {
                    string serviceAccountFilePath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS") ?? "invalid-credentials";
                    
                    if (string.IsNullOrEmpty(serviceAccountFilePath))
                    {
                        throw new Exception("Invalid credentails");
                    }
                    
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromFile(serviceAccountFilePath),
                        ProjectId = projectID
                    });

                    _storageClient = StorageClient.Create();
                    
                    _firestoreDb = FirestoreDb.Create(projectID);
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> UploadPdfAsBinaryAsync(IBrowserFile file, string userId)
        {
            try
            {
                const long maxFileSize = 10 * 1024 * 1024; // 10MB
                if (file.Size > maxFileSize)
                {
                    throw new Exception("File size exceeds the maximum limit of 10MB.");
                }

                byte[] fileBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await file.OpenReadStream(maxAllowedSize: maxFileSize).CopyToAsync(memoryStream);
                    fileBytes = memoryStream.ToArray();
                }

                var document = new Dictionary<string, object>
                {
                    { "fileName", file.Name },
                    { "uploadDate", DateTime.UtcNow },
                    { "size", file.Size },
                    { "contentType", file.ContentType },
                    { "fileData", fileBytes }
                };

                await _firestoreDb.Collection("User")
                                .Document(userId)
                                .Collection("pdfUploads")
                                .AddAsync(document);

                return "File saved successfully";
            }
            catch(Exception ex)
            {
                throw new Exception($"Error uploading file: {ex.Message}");
            }
        }
    }
}