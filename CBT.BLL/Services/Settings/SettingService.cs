using CBT.BLL.Constants;
using CBT.BLL.Utilities;
using CBT.Contracts;
using CBT.Contracts.Category;
using CBT.Contracts.Common;
using CBT.Contracts.Settings;
using CBT.DAL;
using CBT.DAL.Models.Candidates;
using CBT.DAL.Models.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.BLL.Services.Settings
{
    public class SettingService : ISettingService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;

        public SettingService(DataContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.accessor = accessor;
        }
        public async Task<APIResponse<CreateSettings>> CreateSettings(CreateSettings request)
        {
            var res = new APIResponse<CreateSettings>();
            
            try
            {
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());

                var setting = await context.Setting
                    .Where(d => d.Deleted != true && d.ClientId == clientId).FirstOrDefaultAsync();

                if(setting == null)
                {
                    var newSetting = new Setting
                    {
                        NotifyByEmail = request.NotifyByEmail,
                        NotifyBySMS = request.NotifyBySMS,
                        ShowPreviousBtn = request.ShowPreviousBtn,
                        ShowPreviewBtn = request.ShowPreviewBtn,
                        ShowResult = request.ShowResult,
                        UseWebCamCapture = request.UseWebCamCapture,
                        SubmitExamWhenUserLeavesScreen = request.SubmitExamWhenUserLeavesScreen,
                        ViewCategory = request.ViewCategory,
                        Calculator = request.Calculator,
                        SendToEmail = request.SendToEmail,
                        UploadToSmpAsAssessment = request.UploadToSmpAsAssessment,
                        UploadToSmpAsExam = request.UploadToSmpAsExam,
                        GeoLocation = request.GeoLocation,
                        ImageCasting = request.ImageCasting,
                        ScreenRecording = request.ScreenRecording,
                        VideoRecording = request.VideoRecording,
                    };
                    context.Setting.Add(newSetting);
                }
                else
                {
                    setting.NotifyByEmail = request.NotifyByEmail;
                    setting.NotifyBySMS = request.NotifyBySMS;
                    setting.ShowPreviousBtn = request.ShowPreviousBtn;
                    setting.ShowPreviewBtn = request.ShowPreviewBtn;
                    setting.ShowResult = request.ShowResult;
                    setting.UseWebCamCapture = request.UseWebCamCapture;
                    setting.SubmitExamWhenUserLeavesScreen = request.SubmitExamWhenUserLeavesScreen;
                    setting.ViewCategory = request.ViewCategory;
                    setting.Calculator = request.Calculator;
                    setting.SendToEmail = request.SendToEmail;
                    setting.UploadToSmpAsAssessment = request.UploadToSmpAsAssessment;
                    setting.UploadToSmpAsExam = request.UploadToSmpAsExam;
                    setting.GeoLocation = request.GeoLocation;
                    setting.ImageCasting = request.ImageCasting;
                    setting.ScreenRecording = request.ScreenRecording;
                    setting.VideoRecording = request.VideoRecording;
                }
                
                await context.SaveChangesAsync();
                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Saved;
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

        public async Task<APIResponse<bool>> DeleteSettings(SingleDelete request)
        {
            var res = new APIResponse<bool>();
            try
            {
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());
                var settings = await context.Setting.Where(d => d.Deleted != true && d.SettingId == Guid.Parse(request.Item) && d.ClientId == clientId).FirstOrDefaultAsync();
                if (settings == null)
                {
                    res.Message.FriendlyMessage = "Settings Id does not exist";
                    res.IsSuccessful = false;
                    return res;
                }

                settings.Deleted = true;
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

        public async Task<APIResponse<SelectSettings>> GetSettings()
        {
            var res = new APIResponse<SelectSettings>();
            try
            {
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());
                var result = await context.Setting
                    .Where(d => d.Deleted != true && d.ClientId == clientId)
                    .Select(db => new SelectSettings(db))
                    .FirstOrDefaultAsync();

                if (result == null)
                {
                    res.Message.FriendlyMessage = "No item found";
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
