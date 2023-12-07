using ErrorsReporting.Models.Domains;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorsReporting.Models.Configurations
{
    public class ErrorConfiguration : EntityTypeConfiguration<Error>
    {
        public ErrorConfiguration()
        {
            ToTable("dbo.Errors");

            HasKey(x => x.Id);

            Property(x => x.Message)
                .HasMaxLength(50)
                .IsRequired();

            Property(x => x.WasSend)
                .IsRequired();

            Property(x => x.Date)
                .IsRequired();
               
        }
    }
}
