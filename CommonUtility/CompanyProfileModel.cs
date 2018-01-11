using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility
{
   public class CompanyProfileModel
    {
        public long ProfileId { get; set; }
        public string Logo { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<CompanyProfileModel> CompanyProfileList { get; set; }
    }
}
