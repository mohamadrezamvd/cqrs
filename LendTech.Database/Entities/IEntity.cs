using LendTech.Database.Entities;

namespace LendTech.Database.Entities;

public interface IEntity
{
	Guid Id { get; set; }
}
public partial class User : IEntity { }
public partial class Role : IEntity { }
public partial class AuditLog : IEntity { }
public partial class Currency : IEntity { }
public partial class CurrencyRate : IEntity { }
public partial class Organization : IEntity { }
public partial class OutboxEvent : IEntity { }
public partial class InboxEvent : IEntity { }
public partial class Permission : IEntity { }
public partial class PermissionGroup : IEntity { }
public partial class User : IEntity { }
public partial class UserToken : IEntity { }