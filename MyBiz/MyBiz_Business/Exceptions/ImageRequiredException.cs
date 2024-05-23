using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBiz_Business.Exceptions
{
    public class ImageRequiredException : Exception
    {
        public string PropertyName { get; set; }
        public ImageRequiredException(string propertyName, string? message) : base(message)
        {
            PropertyName = propertyName;
        }
    }
}
