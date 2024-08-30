namespace Tessa.Models.Filesystem.Directory;

public class Directory : Base.Base
{
    public virtual IList<Base.Base>? Children { get; set; }
}