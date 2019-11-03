using System.Collections.Generic;

namespace Intranet.Model.ViewModel.Employee
{
    public class BirthdayViewModel
    {
        public int? Count { get; set; }

        public IEnumerable<BirthdayPersonViewModel> Birthdays { get; set; }
    }

    public class BirthdayPersonViewModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? JobTitleId { get; set; }
        public string JobTitle { get; set; }
        public string ImageUrl { get; set; }
    }
}
