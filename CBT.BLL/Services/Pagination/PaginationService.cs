using CBT.BLL.Filters;
using CBT.BLL.Wrappers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.BLL.Services.Pagination
{
    public class PaginationService : IPaginationService
    {
        private readonly IUriService uriService;
        private readonly IHttpContextAccessor accessor;

        public PaginationService(IUriService uriService, IHttpContextAccessor accessor)
        {
            this.uriService = uriService;
            this.accessor = accessor;
        }
        public PagedResponse<T> CreatePagedReponse<T>(T pagedData, PaginationFilter valFilter, int totalRecords)
        {
            var route = accessor.HttpContext.Request.Path.Value;
            PaginationFilter validFilter = new PaginationFilter(valFilter.PageNumber, valFilter.PageSize);
            var respose = new PagedResponse<T>(pagedData, validFilter.PageNumber, validFilter.PageSize);
            var totalPages = ((double)totalRecords / (double)validFilter.PageSize);
            int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
            respose.NextPage =
                validFilter.PageNumber >= 1 && validFilter.PageNumber < roundedTotalPages
                ? uriService.GetPageUri(new PaginationFilter(validFilter.PageNumber + 1, validFilter.PageSize), route)
                : null;
            respose.PreviousPage =
                validFilter.PageNumber - 1 >= 1 && validFilter.PageNumber <= roundedTotalPages
                ? uriService.GetPageUri(new PaginationFilter(validFilter.PageNumber - 1, validFilter.PageSize), route)
                : null;
            respose.FirstPage = uriService.GetPageUri(new PaginationFilter(1, validFilter.PageSize), route);
            respose.LastPage = uriService.GetPageUri(new PaginationFilter(roundedTotalPages, validFilter.PageSize), route);
            respose.TotalPages = roundedTotalPages;
            respose.TotalRecords = totalRecords;
            return respose;
        }

        public IQueryable<T> GetPagedResult<T>(IQueryable<T> query, PaginationFilter filter)
        {
            return query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
        }
    }
}
