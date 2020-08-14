using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CluedIn.Crawling.Kafka.Core.Models
{
    public class Contact
    {
        public string AccountId { get; set; }
        public string AccountIdName { get; set; }
        public string AccountIdYomiName { get; set; }
        public string AccountRoleCode { get; set; }
        public string AccountRoleCodeName { get; set; }
        public string ContactId { get; set; }
        public string Description { get; set; }
        public string FullName { get; set; }
        public string NickName { get; set; }

    }
}
