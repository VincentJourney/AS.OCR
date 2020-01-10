using AS.OCR.Commom.Configuration;
using AS.OCR.Extension.SDK.EnterpriseWeChat;
using AS.OCR.Model.Business;
using System;
using System.Linq;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Ocr.V20181119;
using TencentCloud.Ocr.V20181119.Models;

namespace AS.OCR.Extension.SDK.TencentOCR
{
    /// <summary>
    /// 腾讯OCR识别SDK
    /// </summary>
    public class TencentOCR
    {
        private static EnterpriseWeChatHelper enterpriseWeChatHelper = new EnterpriseWeChatHelper(
                ConfigurationUtil.EnterpriseWeChat_AppID,
                ConfigurationUtil.EnterpriseWeChat_AppSecret);
        /// <summary>
        /// OCR 通用高精度印刷体识别（默认url）
        /// </summary>
        /// <param name="Image">图片</param>
        /// <param name="Type">1：url，2：base64</param>
        public static TencentOCRResult GeneralAccurateOCR(string Image, int? Type = 1)
        {
            try
            {
                enterpriseWeChatHelper.OCRFilter();

                Credential cred = new Credential
                {
                    SecretId = ConfigurationUtil.TencentSecretId,
                    SecretKey = ConfigurationUtil.TencentSecretKey
                };
                ClientProfile clientProfile = new ClientProfile();
                HttpProfile httpProfile = new HttpProfile();
                httpProfile.Endpoint = ("ocr.ap-guangzhou.tencentcloudapi.com");
                clientProfile.HttpProfile = httpProfile;

                OcrClient client = new OcrClient(cred, "ap-guangzhou", clientProfile);
                GeneralAccurateOCRRequest req = new GeneralAccurateOCRRequest();
                if (Type == null || Type == 1 || Type == 0)
                    req.ImageUrl = Image;
                else
                    req.ImageBase64 = Image;

                enterpriseWeChatHelper.SetHashIncr(1);//优先扣除库存
                GeneralAccurateOCRResponse resp = client.GeneralAccurateOCR(req).
                    ConfigureAwait(false).GetAwaiter().GetResult();

                return new TencentOCRResult
                {
                    WordList = resp.TextDetections
                     .Select(s => new Word
                     {
                         text = s.DetectedText.Replace(" ", "").ToLower()
                     })
                     .Where(s => !string.IsNullOrEmpty(s.text))
                     .ToList()
                };
            }
            catch (Exception e)
            {
                if (e.Message != "操作Redis失败")//如果请求接口失败则加回来
                    enterpriseWeChatHelper.SetHashIncr(-1);
                throw new Exception(e.Message);
            }

        }


    }
}
