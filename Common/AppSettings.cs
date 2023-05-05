namespace Common;

public class AppSettings
{
    public string ClientCcEmail { get; set; }
    public string ContainerName { get; set; }
    public string AzureBlobConnectionString { get; set; }
    public bool IsJwtFlow { get; set; }
    public string PwcKey { get; set; }
}
public class JwtSettings
{
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Secret { get; set; }

}
