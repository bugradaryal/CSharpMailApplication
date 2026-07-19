using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mail.DTO
{
    public class mail_file_transfer
    {
        public string file_name { get; set; }
        public string bodyfile_string { get; set; }
        public byte[] bodyfile_byte { get; set; }
    }
}
