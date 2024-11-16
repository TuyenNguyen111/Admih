namespace FinalProject.Data;

public partial class UserType
{
    public int UserTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Privilege> Privileges { get; set; } = new List<Privilege>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
