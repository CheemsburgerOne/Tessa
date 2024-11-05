namespace Tessa.Models.Filesystem.File.ResultTypes;

public class UploadFileResultObject
{
    public List<OneItemWrapper>? SuccessfulUploads { get; set; }
    public UploadFileResultType Code { get; set; }
}
public struct OneItemWrapper
{
    public string Name { get; set; }
    public bool Result { get; set; }
}

