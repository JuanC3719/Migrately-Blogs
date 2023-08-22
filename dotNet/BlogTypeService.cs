using Migrately.Data;
using Migrately.Models.Domain;
using Migrately.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Migrately.Services
{
    public class BlogTypeService : IBlogTypeService
    {
        private readonly IDataProvider _data;

        public BlogTypeService(IDataProvider data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public List<BlogType> GetAll()
        {
            string procName = "[dbo].[BlogTypes_SelectAll]";
            List<BlogType> list = new List<BlogType>();

            _data.ExecuteCmd(
                procName,
                inputParamMapper: null,
                singleRecordMapper: (IDataReader reader, short set) =>
                {
                    BlogType aBlogType = MapBlogType(reader);
                    list.Add(aBlogType);
                });

            return list;
        }

        private static BlogType MapBlogType(IDataReader reader)
        {
            int startingIndex = 0;

            BlogType blogType = new BlogType
            {
                Id = reader.GetSafeInt32(startingIndex++),
                Name = reader.GetSafeString(startingIndex++)
            };

            return blogType;
        }
    }
}
