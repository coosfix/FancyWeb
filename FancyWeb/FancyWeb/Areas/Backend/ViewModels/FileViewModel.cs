using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FancyWeb.Areas.Backend.ViewModels
{
    public class FileViewModel
    {
        [DisplayName("搜尋檔案")]
        [Required(ErrorMessage = "沒有檔案,不可上傳")]
        public string FileInfo { get; set; }
    }
}