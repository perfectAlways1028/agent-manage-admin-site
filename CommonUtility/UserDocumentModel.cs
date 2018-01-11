using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility
{
   public class UserDocumentModel
    {
        public long UserDocumentId { get; set; }
        public string Title { get; set; }
        public string DocumentName { get; set; }
        public string Description { get; set; }
        public long UserId { get; set; }
        public string UploaderName { get; set; }
        public string DocumentExtension { get; set; }
        public string DocType { get; set; }
        public bool IsActive { get; set; }

        public List<UserDocumentModel> UserDocumentList { get; set; }
    }
}
