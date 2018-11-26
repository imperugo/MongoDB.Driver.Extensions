using System.Threading;
using System.Threading.Tasks;

namespace MongoDB.Driver.Extensions.Abstractions
{
	public interface IAuditRepository
	{
		Task<DbStatus> CheckAsync(CancellationToken cancellationToken = default(CancellationToken));
		DbStatus Check();
	}

	public class DbStatus
	{
		public DbStatus(string dbName, bool running)
		{
			DbName = dbName;
			Running = running;
		}

		public string DbName { get; set; }
		public bool Running { get; set; }
	}
}
