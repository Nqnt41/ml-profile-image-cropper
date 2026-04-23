namespace ProfileSearch.Server.Data
{
    public class CloudinaryContext
    {
        public string Url { get; }
        public string UploadPreset { get; }

        public CloudinaryContext(string cloudinaryUrl, string cloudinaryUploadPreset)
        {
            Url = cloudinaryUrl;
            UploadPreset = cloudinaryUploadPreset;
        }
    }
}
