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
using Hackathon_VAIT_New.Model;
using Google.Cloud.Firestore.V1;

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

        public async Task<List<PdfFileMetaData>> GetResumeHistory(string userId)
        {
            try
            {
                if(string.IsNullOrEmpty(userId))
                {
                    throw new ArgumentException("User ID cannot be null or empty.");
                }

                var resumeList = new List<PdfFileMetaData>();

                Query q = _firestoreDb.Collection("User")
                              .Document(userId)
                              .Collection("pdfUploads");

                QuerySnapshot snapshot = await q.GetSnapshotAsync();

                foreach (var document in snapshot.Documents)
                {
                    var data = document.ToDictionary();
                    var resumeMetaData = new PdfFileMetaData
                    {
                        Id = document.Id,
                        FileName = data.ContainsKey("fileName") ? data["fileName"].ToString() : "Invalid file name",
                        UploadDate = data.ContainsKey("uploadDate") ? ((Timestamp)data["uploadDate"]).ToDateTime() : DateTime.MinValue,
                        FileSize = data.ContainsKey("size") ? Convert.ToInt64(data["size"]) : 0,
                        ContentType = data.ContainsKey("contentType") ? data["contentType"].ToString() : "application/pdf"
                    };
                    resumeList.Add(resumeMetaData);
                }

                return resumeList;
            }
            catch(Exception ex)
            {
                throw new Exception($"Error fetching resume history: {ex.Message}");
            }
        }

        public async Task<byte[]> DownloadResumePdfFile(string userId, string fileId)
        {
            try
            {
                var resumeRef = _firestoreDb.Collection("User")
                                            .Document(userId)
                                            .Collection("pdfUploads")
                                            .Document(fileId);

                var resumeSnapshot = await resumeRef.GetSnapshotAsync();

                if (resumeSnapshot.Exists)
                {
                    var data = resumeSnapshot.ToDictionary();

                    if (data.ContainsKey("fileData"))
                    {
                        var testData = data["fileData"];
                        var fileData = data["fileData"];

                        if(fileData is Blob blobData)
                        {
                            return blobData.ByteString.ToByteArray();
                        }
                        else if (fileData is byte[] byteArray)
                        {
                            return byteArray;
                        }
                        else
                        {
                            throw new Exception("Invalid file data format.");
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error downloading file", ex);
            }
        }

    }
}