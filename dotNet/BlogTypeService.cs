using Migrately.Data;
using Migrately.Data.Providers;
using Migrately.Models.Domain;
using Migrately.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrately.Services
{
    public class BlogTypeService : IBlogTypeService
    {

        private IDataProvider _data = null;
        public BlogTypeService(IDataProvider data)
        {
            _data = data;
        }

        public List<BlogType> GetAll()
        {
            List<BlogType> list = null;

            string procName = "[dbo].[BlogTypes_SelectAll]";

            _data.ExecuteCmd(
                procName,
                inputParamMapper: null,
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    BlogType aBlogType = MapBlogType(reader);

                    if (list == null)
                    {
                        list = new List<BlogType>();
                    }

                    list.Add(aBlogType);
                }
            );

            return list;
        }

        private static BlogType MapBlogType (IDataReader reader)
        {
            BlogType blogType = new BlogType();
            int startingIndex = 0;

            blogType.Id = reader.GetSafeInt32(startingIndex++);
            blogType.Name = reader.GetSafeString(startingIndex++);  

            return blogType;
        }
    }
}
