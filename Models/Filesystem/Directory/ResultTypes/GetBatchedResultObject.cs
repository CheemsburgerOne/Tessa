namespace Tessa.Models.Filesystem.Directory.ResultTypes;

public struct GetBatchedResultObject
{
    public DirectoryBatchedDto? Value { get; set; }
    public GetBatchedResultType Code { get; set; }
}