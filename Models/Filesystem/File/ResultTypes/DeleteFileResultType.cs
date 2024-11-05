using System.Reflection;

namespace Tessa.Models.Filesystem.File.ResultTypes;

public enum DeleteFileResultType
{
    InvalidFile,
    NotLogged,
    NotOwner,
    DriveError,
    Ok
}