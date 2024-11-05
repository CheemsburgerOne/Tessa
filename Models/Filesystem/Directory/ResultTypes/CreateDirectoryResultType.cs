namespace Tessa.Models.Filesystem.Directory.ResultTypes;

public enum CreateDirectoryResultType
{
    NotLogged,
    NotOwner,
    DataInvalid,
    ParentInvalid,
    Duplicate,
    DriveError,
    Ok
}