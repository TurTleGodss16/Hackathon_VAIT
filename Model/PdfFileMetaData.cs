namespace Hackathon_VAIT_New.Model;

public class PdfFileMetaData {
    public string Id { get; set; }
    public string FileName { get; set; }
    public DateTime UploadDate { get; set; }
    public long FileSize { get; set; }
    public string ContentType { get; set; }
}