namespace Common;
public class Constants
{
    public static Dictionary<string, string> DuplicateExceptionDictionary = new Dictionary<string, string>()
     {
        {"Project", "Project is already exists with given Project Code"},  //  add CustomStatusCode and Custom errorDescription
        {"User", "User already exists with given Email"},  //  add CustomStatusCode and Custom errorDescription
        {"ProjectExcel", "Invalid Header Excel Format"},  //  add CustomStatusCode and Custom errorDescription
        {"1001", "Uploaded Excel Sheet Should Contain Atleast One Record"},
        //{"1002", "Provided Sbu or LegalEntity not valid"},
        {"1002", "Errors exists in the uploaded file. Please check the downloaded error report"},
     };

    public const string SortDirectionAsc = "asc";
    public const string SortDirectionDesc = "desc";
    public const string UserDataClaimsType = "UserData";
    public const string MailToLinkContent = "mailto:{0}?subject={1}&cc={2}&bcc={3}";
    public const string PptHeaderTargetRequest = "Target";

    public static readonly Dictionary<int, Dictionary<string, string>> CustomExceptionDictionary = new()
    {
        { 1000, DuplicateExceptionDictionary }
    };
}