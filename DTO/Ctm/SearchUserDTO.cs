namespace DTO.Ctm
{
    public class SearchUserDTO
    {
        public PageQueryModelDTO PageQueryModel = new();
        public string? Name { get; set; }
    }
}
