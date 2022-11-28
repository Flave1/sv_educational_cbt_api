using CBT.BLL.Constants;
using CBT.BLL.Utilities;
using CBT.Contracts;
using CBT.Contracts.Category;
using CBT.Contracts.Common;
using CBT.DAL;
using CBT.DAL.Models.Candidates;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CBT.BLL.Services.Category
{
    public class CandidateCategoryService : ICandidateCategoryService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;

        public CandidateCategoryService(DataContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.accessor = accessor;
        }
        public async Task<APIResponse<CreateCandidateCategory>> CreateCandidateCategory(CreateCandidateCategory request)
        {
            var res = new APIResponse<CreateCandidateCategory>();

            try
            {
                var clientId = Guid.Parse(accessor.HttpContext.Items["smsClientId"].ToString());
                if (context.CandidateCategory.AsEnumerable().Any(r => UtilTools.ReplaceWhitespace(request.Name) == UtilTools.ReplaceWhitespace(r.Name) && r.Deleted == false && r.ClientId == clientId))
                {
                    res.Message.FriendlyMessage = "Candidate Category Name Already Exist";
                    return res;
                }

                var category = new CandidateCategory
                {
                    Name = request.Name,
                };

                context.CandidateCategory.Add(category);
                await context.SaveChangesAsync();
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
                var clientId = Guid.Parse(accessor.HttpContext.Items["smsClientId"].ToString());
                var category = await context.CandidateCategory.Where(d => d.Deleted != true && d.CandidateCategoryId == Guid.Parse(request.Item) && d.ClientId == clientId).FirstOrDefaultAsync();
                if (category == null)
                {
                    res.Message.FriendlyMessage = "Candidate category does not exist";
                    res.IsSuccessful = true;
                    return res;
                }

                category.Deleted = true;
                await context.SaveChangesAsync();

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
                var clientId = Guid.Parse(accessor.HttpContext.Items["smsClientId"].ToString());
                var result = await context.CandidateCategory
                    .Where(d => d.Deleted != true && d.ClientId == clientId)
                    .OrderByDescending(s => s.CreatedOn)
                    .Select(db => new SelectCandidateCategory(db))
                    .ToListAsync();

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
                var clientId = Guid.Parse(accessor.HttpContext.Items["smsClientId"].ToString());
                var result = await context.CandidateCategory
                    .Where(d => d.Deleted != true && d.CandidateCategoryId == Id && d.ClientId == clientId)
                    .Select(db => new SelectCandidateCategory(db))
                    .FirstOrDefaultAsync();

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
                var clientId = Guid.Parse(accessor.HttpContext.Items["smsClientId"].ToString());
                var category = await context.CandidateCategory.Where(d => d.Deleted != true && d.CandidateCategoryId == request.CandidateCategoryId && d.ClientId == clientId).FirstOrDefaultAsync();
                if (category == null)
                {
                    res.Message.FriendlyMessage = "Candidate category does not exist";
                    res.IsSuccessful = true;
                    return res;
                }

                if (context.CandidateCategory.AsEnumerable().Any(r => UtilTools.ReplaceWhitespace(request.Name) == UtilTools.ReplaceWhitespace(r.Name)
               && r.CandidateCategoryId != request.CandidateCategoryId && r.ClientId == clientId))
                {
                    res.Message.FriendlyMessage = "Candidate Category Name Already Exist";
                    res.IsSuccessful = true;
                    return res;
                }

                category.Name = request.Name;
                await context.SaveChangesAsync();

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
