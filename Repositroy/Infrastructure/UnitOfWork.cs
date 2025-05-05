using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Core.Entites;

namespace Timesheet.Repositroy.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TimesheetContext _context;

        public UnitOfWork(TimesheetContext context)
        {
            _context = context;
        }

        public int Commit()
        {
            return _context.SaveChanges();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }


    }
}
