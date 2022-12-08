using CBT.BLL.Filters;
using CBT.BLL.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.BLL.Services.Pagination
{
    public interface IPaginationService
    {
        PagedResponse<T> CreatePagedReponse<T>(T pagedData, PaginationFilter valFilter, int totalRecords);
        IQueryable<T> GetPagedResult<T>(IQueryable<T> query, PaginationFilter filter);
    }
}
