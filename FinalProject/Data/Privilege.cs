namespace FinalProject.Data;

public partial class Privilege
{
    public int PrivilegeId { get; set; }

    public int? UserTypeId { get; set; }

    public int? PageAddressId { get; set; }

    public bool? IsPrivileged { get; set; }

    public virtual PageAddress? PageAddress { get; set; }

    public virtual UserType? UserType { get; set; }
}
