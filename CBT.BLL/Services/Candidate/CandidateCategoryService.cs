using CBT.BLL.Constants;
using CBT.Contracts;
using CBT.Contracts.Authentication;
using CBT.Contracts.Candidate;
using CBT.DAL;
using CBT.DAL.Models.Candidate;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.BLL.Services.Candidate
{
    public class CandidateCategoryService : ICandidateCategoryService
    {
        private readonly DataContext _context;

        public CandidateCategoryService(DataContext context)
        {
            _context = context;
        }
        public async Task<APIResponse<CreateCandidateCategory>> CreateCandidateCategory(CreateCandidateCategory request, Guid clientId, int userType)
        {
            var res = new APIResponse<CreateCandidateCategory>();

            try
            {
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
            catch(Exception ex)
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
                var result = await _context.CandidateCategory
                    .OrderByDescending(s => s.CreatedOn)
                    .Where(d => d.Deleted != true).Select(a => new SelectCandidateCategory
                    {
                        CandidateCategoryId = a.CandidateCategoryId,
                        Name = a.Name,
                        DateCreated = a.CreatedOn
                    }).ToListAsync();

                res.IsSuccessful = true;
                res.Result = result;
                res.Message.FriendlyMessage = Messages.GetSuccess;
                return res;
            }
            catch(Exception ex)
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
                var result = await _context.CandidateCategory
                    .Where(d => d.Deleted != true && d.CandidateCategoryId == Id).Select(a => new SelectCandidateCategory
                    {
                        CandidateCategoryId = a.CandidateCategoryId,
                        Name = a.Name,
                        DateCreated = a.CreatedOn
                    }).FirstOrDefaultAsync();
                if(result == null)
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
    }
}
