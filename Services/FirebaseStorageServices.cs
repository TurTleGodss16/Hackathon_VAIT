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
                    string serviceAccountFilePath =
                        Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS") ?? "invalid-credentials";

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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                throw new Exception($"Error uploading file: {ex.Message}");
            }
        }

        public async Task<List<PdfFileMetaData>> GetResumeHistory(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
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
                        UploadDate = data.ContainsKey("uploadDate")
                            ? ((Timestamp)data["uploadDate"]).ToDateTime()
                            : DateTime.MinValue,
                        FileSize = data.ContainsKey("size") ? Convert.ToInt64(data["size"]) : 0,
                        ContentType = data.ContainsKey("contentType")
                            ? data["contentType"].ToString()
                            : "application/pdf"
                    };
                    resumeList.Add(resumeMetaData);
                }

                if(resumeList != null && resumeList.Any())
                {
                    resumeList = resumeList.OrderByDescending(x => x.UploadDate).ToList();
                }
                
                return resumeList;
            }
            catch (Exception ex)
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

                        if (fileData is Blob blobData)
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

        public async Task createUsers()
        {
            List<User> users = new List<User>();
            users.Add(new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Tony",
                CompanyName = "LCSign",
                Email = "fakeemail@gmail.com",
                DiscordId = "tonylcsign1234",
                ProfilePictureUrl =
                    "https://d1ef7ke0x2i9g8.cloudfront.net/hong-kong/LC-Sign-Tony-interview-Big-Hitter-header.jpg",
                Roles = new List<UserRoleType> { UserRoleType.Reviewer, UserRoleType.User },
                EmploymentStatus = UserEmploymentType.Fulltime,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            });
            users.Add(new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = "John Doe",
                CompanyName = "Elgoog",
                Email = "fakeemail@elgoog.com",
                DiscordId = "elgoog1234",
                ProfilePictureUrl =
                    "https://media.newyorker.com/photos/6228e8e572e438a479e04a0e/16:9/w_1280,c_limit/chayka_google_social_2.jpg",
                Roles = new List<UserRoleType> { UserRoleType.Reviewer, UserRoleType.User },
                EmploymentStatus = UserEmploymentType.Parttime,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            });
            users.Add(new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Jennifer Moore",
                CompanyName = "Atem",
                Email = "fakeemail@atem.com",
                DiscordId = "atem1234",
                ProfilePictureUrl =
                    "https://pbs.twimg.com/profile_images/378800000601297245/e26e1564b85c8f408d4b4ba0c901c758_400x400.jpeg",
                Roles = new List<UserRoleType> { UserRoleType.User },
                EmploymentStatus = UserEmploymentType.Student,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            });
            try
            {
                foreach (var user in users)
                {
                    await _firestoreDb.Collection("Account").AddAsync(
                        user.ToDictionary()
                    );
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error create users: {ex.Message}");
            }
        }

        public async Task<List<User>> GetUsers()
        {
            try
            {
                List<User> users = new List<User>();

                Query q = _firestoreDb.Collection("Account");
                QuerySnapshot snapshot = await q.GetSnapshotAsync();

                foreach (var document in snapshot.Documents)
                {
                    users.Add(User.FromDictionary(document.ToDictionary()));
                }

                return users;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting users: {ex.Message}");
            }
        }

        public async Task<List<User>> GetReviewers()
        {
            try
            {
                List<User> users = new List<User>();

                Query q = _firestoreDb.Collection("Account");
                QuerySnapshot snapshot = await q.GetSnapshotAsync();
                foreach (var document in snapshot.Documents)
                {
                    users.Add(User.FromDictionary(document.ToDictionary()));
                }

                users = users.FindAll(user => user?.Roles?.Contains(UserRoleType.Reviewer) == true);

                return users;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting reviewers: {ex.Message}");
            }
        }

        public async Task<List<User>> GetUsersFromCompany(string companyName)
        {
            try
            {
                List<User> users = new List<User>();

                Query q = _firestoreDb.Collection("Account");

                QuerySnapshot snapshot = await q.GetSnapshotAsync();
                foreach (var document in snapshot.Documents)
                {
                    users.Add(User.FromDictionary(document.ToDictionary()));
                }

                users = users.FindAll(user => user?.CompanyName?.ToLower().Trim() == companyName.ToLower().Trim());
                return users;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting reviewers: {ex.Message}");
            }
        }
    }
}