using CBT.DAL.Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Settings
{
    public class SelectSettings
    {
        public bool NotifyByEmail { get; set; }
        public bool NotifyBySMS { get; set; }
        public bool ShowPreviousBtn { get; set; }
        public bool ShowPreviewBtn { get; set; }
        public bool ShowResult { get; set; }
        public bool UseWebCamCapture { get; set; }
        public bool SubmitExamWhenUserLeavesScreen { get; set; }
        public bool ViewCategory { get; set; }
        public bool Calculator { get; set; }
        public bool SendToEmail { get; set; }
        public bool GeoLocation { get; set; }
        public bool ImageCasting { get; set; }
        public bool ScreenRecording { get; set; }
        public bool VideoRecording { get; set; }
        public SelectSettings(Setting setting)
        {
            NotifyByEmail = setting.NotifyByEmail;
            NotifyBySMS = setting.NotifyBySMS;
            ShowPreviousBtn = setting.ShowPreviousBtn;
            ShowPreviewBtn = setting.ShowPreviewBtn;
            ShowResult = setting.ShowResult;
            UseWebCamCapture = setting.UseWebCamCapture;
            SubmitExamWhenUserLeavesScreen = setting.SubmitExamWhenUserLeavesScreen;
            ViewCategory = setting.ViewCategory;
            Calculator = setting.Calculator;
            SendToEmail = setting.SendToEmail;
            GeoLocation = setting.GeoLocation;
            ImageCasting = setting.ImageCasting;
            ScreenRecording = setting.ScreenRecording;
            VideoRecording = setting.VideoRecording;
        }
    }
}
