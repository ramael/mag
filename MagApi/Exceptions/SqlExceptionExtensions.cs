using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagApi.Exceptions
{
    public static class SqlExceptionExtensions
    {
        public static bool IsUniqueKeyViolation(this SqlException ex)
        {
            // 2601 - Violation in unique index
            // 2627 - Violation in unique constraint(although it is implemented using unique index)
            return ex.Errors.Cast<SqlError>().Any(e => e.Class == 14 && (e.Number == 2601 || e.Number == 2627));
        }
    }
}
