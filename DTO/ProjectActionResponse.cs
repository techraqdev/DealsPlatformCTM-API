namespace DTO;

public class ProjectActionResponse
{
    public bool Status { get; set; }
    public string? Message { get; set; }
}

public class ProjectActionRequest
{
    public Guid ProjectId { get; set; }
    public string Email { get; set; }
    public string Action { get; set; }
}
