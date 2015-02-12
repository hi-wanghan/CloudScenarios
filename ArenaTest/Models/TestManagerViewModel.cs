using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArenaTest.Models
{
    public class TestManagerViewModel
    {

        public string TestName { get; set; }
        public string Description { get; set; }

        [DataType(DataType.Upload)]
        [DisplayName("Test Resource")]
        public HttpPostedFileBase FileUpload { get; set; }

    }
}