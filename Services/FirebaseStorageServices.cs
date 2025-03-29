using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Hackathon_VAIT_New.Services
{
    public class FirebaseStorageService
    {
        private readonly StorageClient _storageClient;
        public FirebaseStorageService()
        {
            try
            {
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
                    });

                    _storageClient = StorageClient.Create();
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> UploadPDFAsync(/*byte[] fileBytes, */string fileName)
        {
            try
            {
                return "Test";
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}