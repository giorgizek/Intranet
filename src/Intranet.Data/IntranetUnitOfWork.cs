using Intranet.Model.Dictionary;
using Intranet.Model.Document;
using Intranet.Model.HR;
using Zek.Data.Repository;
using Zek.Model.Contact;
using Zek.Model.Dictionary;
using Zek.Model.Identity;
using Zek.Model.Person;
using Zek.Model.Production;

namespace Intranet.Data
{
    public interface IIntranetUnitOfWork : IUnitOfWork
    {
        #region Membership

        IUserRepository<User> Users { get; }
        IRepository<Role> Roles { get; }

        #endregion

        #region Person

        IRepository<Address> Addresses { get; }
        IRepository<Contact> Contacts { get; }
        IRepository<Person> Persons { get; }

        #endregion

        #region HR

        IRepository<Employee> Employees { get; }

        #endregion

        #region Products

        IRepository<Product> Products { get; }

        #endregion

        #region Dictionary

        IRepository<JobTitle> JobTitles { get; }
        IRepository<JobTitleTranslate> JobTitleTranslates { get; }
        IRepository<Department> Departments { get; }
        IRepository<DepartmentTranslate> DepartmentTranslates { get; }

        IRepository<Branch> Branchs { get; }
        IRepository<BranchTranslate> BranchTranslates { get; }


        IRepository<Category> Categories { get; }
        IRepository<CategoryTranslate> CategoryTranslates { get; }



        #endregion


        #region Documents

        IRepository<DocumentTemplate> DocumentTemplates { get; }
        IRepository<DocumentTemplateProduct> DocumentTemplateProducts { get; }


        IRepository<Document> Documents { get; }
        IRepository<DocumentProduct> DocumentProducts { get; }


        #endregion

    }

    public class IntranetUnitOfWork : UnitOfWork, IIntranetUnitOfWork
    {
        public IntranetUnitOfWork(IntranetDbContext context) : base(context)
        {

        }

        #region Membership

        public IUserRepository<User> Users => new UserRepository<User>(Context);
        public IRepository<Role> Roles => new Repository<Role>(Context);

        #endregion

        #region Person

        public IRepository<Address> Addresses => new Repository<Address>(Context);
        public IRepository<Contact> Contacts => new Repository<Contact>(Context);
        public IRepository<Person> Persons => new Repository<Person>(Context);

        #endregion

        #region HR

        public IRepository<Employee> Employees => new Repository<Employee>(Context);

        #endregion


        #region Product

        public IRepository<Product> Products => new Repository<Product>(Context);

        #endregion


        #region Dictionary

        public IRepository<JobTitle> JobTitles => new Repository<JobTitle>(Context);
        public IRepository<JobTitleTranslate> JobTitleTranslates => new Repository<JobTitleTranslate>(Context);
        public IRepository<Department> Departments => new Repository<Department>(Context);
        public IRepository<DepartmentTranslate> DepartmentTranslates => new Repository<DepartmentTranslate>(Context);

        public IRepository<Branch> Branchs => new Repository<Branch>(Context);
        public IRepository<BranchTranslate> BranchTranslates => new Repository<BranchTranslate>(Context);

        public IRepository<Category> Categories => new Repository<Category>(Context);
        public IRepository<CategoryTranslate> CategoryTranslates => new Repository<CategoryTranslate>(Context);

        #endregion


        #region Documents

        public IRepository<DocumentTemplate> DocumentTemplates => new Repository<DocumentTemplate>(Context);
        public IRepository<DocumentTemplateProduct> DocumentTemplateProducts => new Repository<DocumentTemplateProduct>(Context);

        public IRepository<Document> Documents => new Repository<Document>(Context);
        public IRepository<DocumentProduct> DocumentProducts => new Repository<DocumentProduct>(Context);


        #endregion

    }
}
