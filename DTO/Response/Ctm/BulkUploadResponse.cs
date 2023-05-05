namespace DTO.Response.Ctm
{
    public class BulkUploadResponse
    {
        public bool Status { get; set; }
        public List<UserPostResponse> DetailResponse { get; set; }
    }

    public class UserPostResponse
    {
        public string Email { get; set; }
        public bool Status { get; set; }
        public string Reason { get; set; }
    }
}
