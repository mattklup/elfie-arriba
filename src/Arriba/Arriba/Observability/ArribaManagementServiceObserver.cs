using Arriba.Communication.Server.Application;
using Arriba.Diagnostics.Tracing;
using Arriba.Model;
using Arriba.Model.Column;
using Arriba.Model.Query;
using Arriba.Model.Security;
using Arriba.Types;
using System.Collections.Generic;
using System.Security.Principal;

namespace Arriba.Observability
{
    public class ArribaManagementServiceObserver : IArribaManagementService
    {
        private readonly ArribaLog _logger;
        private readonly IArribaManagementService _innerService;

        public ArribaManagementServiceObserver(ArribaLog logger, IArribaManagementService innerService)
        {
            _logger = logger;
            _innerService = innerService;
        }

        public SecureDatabase GetDatabaseForOwner(IPrincipal user)
        {
            return _innerService.GetDatabaseForOwner(user);
        }

        public IEnumerable<string> GetTables()
        {
            return _innerService.GetTables();
        }

        public IDictionary<string, TableInformation> GetTablesForUser(IPrincipal user)
        {
            return _innerService.GetTablesForUser(user);
        }

        public bool UnloadTableForUser(string tableName, IPrincipal user)
        {
            return _innerService.UnloadTableForUser(tableName, user);
        }

        public bool UnloadAllTableForUser(IPrincipal user)
        {
            return _innerService.UnloadAllTableForUser(user);
        }

        public TableInformation GetTableInformationForUser(string tableName, IPrincipal user)
        {
            return _innerService.GetTableInformationForUser(tableName, user);
        }

        public TableInformation CreateTableForUser(CreateTableRequest table, IPrincipal user)
        {
            return _innerService.CreateTableForUser(table, user);
        }

        public void AddColumnsToTableForUser(string tableName, IList<ColumnDetails> columnDetails, IPrincipal user)
        {
            _innerService.AddColumnsToTableForUser(tableName, columnDetails, user);
        }

        public (bool, ExecutionDetails) SaveTableForUser(string tableName, IPrincipal user, VerificationLevel verificationLevel)
        {
            return _innerService.SaveTableForUser(tableName, user, verificationLevel);
        }

        public void ReloadTableForUser(string tableName, IPrincipal user)
        {
            _innerService.ReloadTableForUser(tableName, user);
        }

        public void DeleteTableForUser(string tableName, IPrincipal user)
        {
            _innerService.DeleteTableForUser(tableName, user);
        }

        public DeleteResult DeleteTableRowsForUser(string tableName, string query, IPrincipal user)
        {
            return _innerService.DeleteTableRowsForUser(tableName, query, user);
        }

        public void GrantAccessForUser(string tableName, SecurityIdentity securityIdentity, PermissionScope scope, IPrincipal user)
        {
            _innerService.GrantAccessForUser(tableName, securityIdentity, scope, user);
        }

        public void RevokeAccessForUser(string tableName, SecurityIdentity securityIdentity, PermissionScope scope, IPrincipal user)
        {
            _innerService.RevokeAccessForUser(tableName, securityIdentity, scope, user);
        }
    }
}
