using ErrorsReporting.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace ErrorsReporting
{
    public class DbRepository
    {
        public List<Error> GetNewErrorsFromDb()
        {
            using( var context = new ApplicationDbContext())
            {
                return context.Errors.Where(x => x.WasSend == false).ToList();
            }
        }

        public void MarkErrorAsSend(Error error)
        {
            using (var context = new ApplicationDbContext())
            {
                var errorToMark = context.Errors.Find(error.Id);

                errorToMark.WasSend = true;

                context.SaveChanges();
            }

        }
    }
}
