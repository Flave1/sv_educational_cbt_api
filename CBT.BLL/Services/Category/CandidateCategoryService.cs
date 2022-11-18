using CBT.BLL.Constants;
using CBT.BLL.Utilities;
using CBT.Contracts;
using CBT.Contracts.Authentication;
using CBT.Contracts.Category;
using CBT.Contracts.Common;
using CBT.DAL;
using CBT.DAL.Models.Candidate;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.BLL.Services.Category
{
    public class CandidateCategoryService : ICandidateCategoryService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _accessor;

        public CandidateCategoryService(DataContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
        }
        public async Task<APIResponse<CreateCandidateCategory>> CreateCandidateCategory(CreateCandidateCategory request)
        {
            var res = new APIResponse<CreateCandidateCategory>();

            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());
                var userType = int.Parse(_accessor.HttpContext.Items["userType"].ToString());

                if (_context.CandidateCategory.AsEnumerable().Any(r => UtilTools.ReplaceWhitespace(request.Name) == UtilTools.ReplaceWhitespace(r.Name) && r.Deleted == false && r.ClientId == clientId))
                {
                    res.Message.FriendlyMessage = "Candidate Category Name Already Exist";
                    return res;
                }

                var category = new CandidateCategory
                {
                    Name = request.Name,
                    ClientId = clientId,
                    UserType = userType
                };

                _context.CandidateCategory.Add(category);
                await _context.SaveChangesAsync();
                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Created;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }

        }

        public async Task<APIResponse<bool>> DeleteCandidateCategory(SingleDelete request)
        {
            var res = new APIResponse<bool>();
            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());

                var category = await _context.CandidateCategory.Where(d => d.Deleted != true && d.CandidateCategoryId == Guid.Parse(request.Item) && d.ClientId == clientId).FirstOrDefaultAsync();
                if (category == null)
                {
                    res.Message.FriendlyMessage = "Candidate category does not exist";
                    res.IsSuccessful = true;
                    return res;
                }

                category.Deleted = true;
                await _context.SaveChangesAsync();

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.DeletedSuccess;
                res.Result = true;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        public async Task<APIResponse<List<SelectCandidateCategory>>> GetAllCandidateCategory()
        {
            var res = new APIResponse<List<SelectCandidateCategory>>();
            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());

                var result = await _context.CandidateCategory
                    .OrderByDescending(s => s.CreatedOn)
                    .Where(d => d.Deleted != true && d.ClientId == clientId).Select(a => new SelectCandidateCategory
                    {
                        CandidateCategoryId = a.CandidateCategoryId.ToString(),
                        Name = a.Name,
                        DateCreated = a.CreatedOn.ToString()
                    }).ToListAsync();

                res.IsSuccessful = true;
                res.Result = result;
                res.Message.FriendlyMessage = Messages.GetSuccess;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        public async Task<APIResponse<SelectCandidateCategory>> GetCandidateCategory(Guid Id)
        {
            var res = new APIResponse<SelectCandidateCategory>();
            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());

                var result = await _context.CandidateCategory
                    .Where(d => d.Deleted != true && d.CandidateCategoryId == Id && d.ClientId == clientId).Select(a => new SelectCandidateCategory
                    {
                        CandidateCategoryId = a.CandidateCategoryId.ToString(),
                        Name = a.Name,
                        DateCreated = a.CreatedOn.ToString()
                    }).FirstOrDefaultAsync();
                if (result == null)
                {
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                }
                else
                {
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                }
                res.IsSuccessful = true;
                res.Result = result;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        public async Task<APIResponse<UpdateCandidateCategory>> UpdateCandidateCategory(UpdateCandidateCategory request)
        {
            var res = new APIResponse<UpdateCandidateCategory>();
            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());

                var category = await _context.CandidateCategory.Where(d => d.Deleted != true && d.CandidateCategoryId == request.CandidateCategoryId && d.ClientId == clientId).FirstOrDefaultAsync();
                if (category == null)
                {
                    res.Message.FriendlyMessage = "Candidate category does not exist";
                    res.IsSuccessful = true;
                    return res;
                }

                if (_context.CandidateCategory.AsEnumerable().Any(r => UtilTools.ReplaceWhitespace(request.Name) == UtilTools.ReplaceWhitespace(r.Name)
               && r.CandidateCategoryId != request.CandidateCategoryId && r.ClientId == clientId))
                {
                    res.Message.FriendlyMessage = "Candidate Category Name Already Exist";
                    res.IsSuccessful = true;
                    return res;
                }


                category.Name = request.Name;
                await _context.SaveChangesAsync();

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Updated;
                res.Result = request;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }
    }
}
