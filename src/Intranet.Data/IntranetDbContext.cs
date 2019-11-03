using Intranet.Model.Dictionary;
using Intranet.Model.Document;
using Intranet.Model.HR;
using Microsoft.EntityFrameworkCore;
using Zek.Data.Entity;
using Zek.Data.Repository;
using Zek.Model.Contact;
using Zek.Model.Dictionary;
using Zek.Model.Identity;
using Zek.Model.Person;
using Zek.Model.Production;

namespace Intranet.Data
{
    public class IntranetDbContext : IdentityDbContext<User, Role>, IDbContext
    {
        public IntranetDbContext(DbContextOptions options)
            : base(options)
        {
        }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ReSharper disable ObjectCreationAsStatement

            new PersonMap(builder);
            new PersonTitleMap(builder);
            new PersonTitleTranslateMap(builder);
            //todo emploeeppersonMap

            new AddressMap(builder);
            //new MapMap(builder);
            new ContactMap(builder);



            new EmployeeMap(builder);




            new DepartmentMap(builder);
            new DepartmentTranslateMap(builder);
            new JobTitleMap(builder);
            new JobTitleTranslateMap(builder);
            new BranchMap(builder);
            new BranchTranslateMap(builder);




            new ProductMap(builder);
            new CategoryMap(builder);
            new CategoryTranslateMap(builder);




            new DocumentTemplateMap(builder);
            new DocumentTemplateProductMap(builder);

            new DocumentMap(builder);
            new DocumentProductMap(builder);

            // ReSharper restore ObjectCreationAsStatement





            builder.InitConventions();
        }




    }
}
