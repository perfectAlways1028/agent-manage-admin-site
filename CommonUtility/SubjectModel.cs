using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility
{
   public class SubjectModel
    {
        public long SubjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public long CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public List<SubjectModel> SubjectList { get; set; }
    }
}
