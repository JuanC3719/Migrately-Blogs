using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrately.Models.Requests
{
    public class BlogIsDeletedUpdateRequest : BlogAddRequest , IModelIdentifier 
    {
        public int Id { get; set; }
        
    }
}
