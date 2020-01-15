using AS.OCR.Commom.Configuration;
using AS.OCR.IDAO;
using AS.OCR.IService;
using AS.OCR.Model.Business;
using AS.OCR.Model.Entity;
using AS.OCR.Model.Request;
using AS.OCR.Model.Response;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AS.OCR.Service
{
    public class ApplyPointService : Infrastructure, IApplyPointService
    {
        private ILogger logger;
        private IApplyPointDAO _applyPointDAO;
        private IOCRLogDAO _oCRLogDAO;
        public ApplyPointService(ILoggerFactory loggerFactory, IApplyPointDAO applyPointDAO, IOCRLogDAO oCRLogDAO)
        {
            logger = loggerFactory.CreateLogger(typeof(OCRService));
            _applyPointDAO = applyPointDAO;
            _oCRLogDAO = oCRLogDAO;
        }

        /// <summary>
        /// 创建积分申请单，校验信息成功并推送
        /// 若原先存在积分申请单，失败的原因：校验失败 所有应该重新赋值
        /// </summary>
        /// <param name="applyPointRequest"></param>
        /// <returns></returns>
        public CreateApplyPointResponse CreateApplyPoint(CreateApplyPointRequest applyPointRequest)
        {
            ////所有广场
            //List<Mall> MallList = GetCacheFromEntity<Mall, List<Mall>>(
            //    Key: "AllMall",
            //    Hour: ConfigurationUtil.CacheExpiration_Mall);

            //Guid OrgID = Guid.Empty;
            //if (Guid.TryParse(applyPointRequest.mallId, out var mid))
            //    OrgID = MallList.Where(s => s.MallID == mid).FirstOrDefault()?.OrgID ?? Guid.Empty;
            //else
            //    TError("积分申请 【mallId】参数错误");

            Store StoreModel = GetCacheFromEntity<Store, Store>(
                Key: $"{ConfigurationUtil.RedisKeySet_Store}_{applyPointRequest.receiptOCR.StoreId}",
                Hour: ConfigurationUtil.RedisKeySet_Store_expir,
                sqlWhere: $"StoreId = '{applyPointRequest.receiptOCR.StoreId}'");

            OCRVerifyRule StoreOCRRule = GetCacheFromEntity<OCRVerifyRule, OCRVerifyRule>(
                Key: $"{ConfigurationUtil.RedisKeySet_OCRVerifyRuleList}_{applyPointRequest.receiptOCR.StoreId}",
                Hour: ConfigurationUtil.RedisKeySet_OCRVerifyRuleList_expir,
                sqlWhere: $"StoreId = '{applyPointRequest.receiptOCR.StoreId}'");

            if (_applyPointDAO.Exsist(applyPointRequest.receiptOCR.ReceiptNo,
                applyPointRequest.receiptOCR.StoreId, applyPointRequest.receiptOCR.TransDatetime))
                throw new Exception("该小票已积分，不可重复提交");

            OCRLog ocrLog = _oCRLogDAO.Get(applyPointRequest.receiptOCR.RecongnizelId);
            if (ocrLog == null)
                TError("小票未经识别");


            var ApplyPointId = Guid.NewGuid();
            //--------------------------------------------------------------------------------------
            var ApplyPoint = new ApplyPoint
            {
                //Id = ApplyPointId,
                //MallId = null,
                //s
                //s = applyPointRequest.receiptOCR.StoreId,
                //CardID = Guid.Parse(applyPointRequest.cardId),
                //ReceiptNo = applyPointRequest.receiptOCR.ReceiptNo,
                //TransDate = applyPointRequest.receiptOCR.TransDatetime,
                //TransAmt = applyPointRequest.receiptOCR.TranAmount,
                //VerifyStatus = StoreOCRRule?.needVerify == 0 ? 1 : 0,
                //ReceiptPhoto = applyPointRequest.receiptOCR.Base64,
                //AddedOn = DateTime.Now,
                //OriginReceiptNo = OriginReceiptNo,
                //OrgID = OrgID
            };

            ocrLog.ApplyId = ApplyPointId;
            _oCRLogDAO.Update(ocrLog);

            var VerifyRecognitionResult = VerifyRecognition(applyPointRequest.receiptOCR);//校验结果
            ApplyPoint.AuditDate = DateTime.Now;
            if (VerifyRecognitionResult.HasError())//校验失败
            {
                //ApplyPoint.RecongizeStatus = 3;
                //ApplyPoint.VerifyStatus = 0;
                //ApplyPoint.Status = 0;
                _applyPointDAO.Add(ApplyPoint);
                //return VerifyRecognitionResult;
            }

            //ApplyPoint.RecongizeStatus = 2;
            //ApplyPoint.VerifyStatus = 1;
            _applyPointDAO.Add(ApplyPoint);

            //List<Company> companyList = GetCacheFromEntity<Company, List<Company>>(
            //    Key: "Company",
            //    Hour: ConfigurationUtil.CacheExpiration_Company);
            //List<OrgInfo> orgList = GetCacheFromEntity<OrgInfo, List<OrgInfo>>(
            //    Key: "OrgInfo",
            //    Hour: ConfigurationUtil.CacheExpiration_OrgInfo);


            //var StoreCode = StoreModel?.StoreCode;
            ////自动积分 推送
            //var arg = new WebPosArg
            //{
            //    companyID = companyList.Where(s => s.CompanyId == (StoreModel?.CompanyID ?? Guid.Empty)).FirstOrDefault()?.CompanyCode ?? "",
            //    storeID = StoreCode,
            //    cardID = cardDAO.Get(Guid.Parse(applyPointRequest.cardId))?.CardCode ?? "",
            //    cashierID = "CrmAIApplyPoint",
            //    discountPercentage = 0,
            //    orgID = MallList.Where(s => s.MallID == (StoreModel?.MallID ?? Guid.Empty)).FirstOrDefault()?.MallCode ?? "",
            //    receiptNo = applyPointRequest.receiptOCR.ReceiptNo,
            //    txnDateTime = txnDateTime,
            //    amount = applyPointRequest.receiptOCR.TranAmount,
            //};
            //var result = WebPosForPoint(arg).Result;
            //ApplyPoint.Remark = result.Message;
            //if (!result.Success)
            //{
            //    ApplyPoint.Status = 2;
            //    applyPointDAO.Update(ApplyPoint);
            //    return SuccessRes<string>(null, "提交积分申请成功，积分失败！");
            //}

            //ApplyPoint.Status = 1;
            //applyPointDAO.Update(ApplyPoint);

            //return SuccessRes<string>(null, "积分申请成功！");

            return new CreateApplyPointResponse();
        }

        /// <summary>
        /// 根据信息校验用户是否篡改信息，是否满足商铺积分规则
        /// </summary>
        /// <param name="receiptOCR"></param>
        /// <returns></returns>
        private Result<string> VerifyRecognition(ReceiptOCRResponse receiptOCR)
        {
            //识别的原始数据
            var RecongnizeModel = _applyPointDAO.Get(receiptOCR.RecongnizelId);
            if (RecongnizeModel == null)
                return FailRes("校验失败：识别数据丢失，请重新识别小票！");

            #region 检查数据可靠性

            if (string.IsNullOrWhiteSpace(receiptOCR.MallName) ||
                string.IsNullOrWhiteSpace(receiptOCR.ReceiptNo) ||
                receiptOCR.TranAmount == 0 || receiptOCR.TransDatetime == null ||
                receiptOCR.StoreId == null || receiptOCR.StoreId == Guid.Empty ||
                receiptOCR.AccountId == null || receiptOCR.AccountId == Guid.Empty)
                return FailRes("校验失败：小票未识别完整！");
            #endregion

            #region 匹配商铺规则
            ApplyPointRule ApplyPointRule = GetCacheFromEntity<ApplyPointRule, ApplyPointRule>(
               Key: $"StoreOCR{receiptOCR.StoreId}",
               Hour: ConfigurationUtil.RedisKeySet_OCRVerifyRuleList_expir,
               sqlWhere: $"and StoreId = '{receiptOCR.StoreId}'");

            if (ApplyPointRule == null)
                return FailRes("校验失败：未配置积分规则！");

            if (ApplyPointRule.Status == 0)
                return FailRes("校验失败：商铺未启用自动积分");

            if (ApplyPointRule.NeedVerify == 1) //当商铺启用校验规则
            {
                if (receiptOCR.TranAmount < ApplyPointRule.MinValue || receiptOCR.TranAmount > ApplyPointRule.MaxValue)
                    return FailRes("校验失败：小票金额不在店铺规则范围之内！");

                if (ApplyPointRule.TotalMaxForDay != 0)  //日自动交易笔数为0时 代表不限制
                {
                    var TicketPerDay = _applyPointDAO.GetApplyPointCountByDay(receiptOCR.StoreId); //当日交易笔数
                    if (TicketPerDay >= ApplyPointRule.TotalMaxForDay)
                        return FailRes("校验失败：今日已超过最大自动积分记录数量");
                }
            }
            #endregion

            return SuccessRes<string>(null, "验证成功");
        }



        ///// <summary>
        ///// 自动积分并微信推送接口
        ///// </summary>
        ///// <param name="webPosArg"></param>
        ///// <returns></returns>
        //public async Task<Result<CmdResponse>> WebPosForPoint(WebPosArg webPosArg)
        //{
        //    string param = $@"<cmd type=""SALES"" appCode=""POS"">
        //                                <shared offline=""true"">
        //                                        <companyID>{webPosArg.companyID}</companyID>
        //                                        <orgID>{webPosArg.orgID}</orgID>
        //                                        <storeID>{webPosArg.storeID}</storeID>
        //                                        <cashierID>{webPosArg.cashierID}</cashierID>
        //                                </shared>
        //                                <sales discountPercentage=""{webPosArg.discountPercentage}"" 
        //                                cardID=""{webPosArg.cardID}"" 
        //                                txnDateTime=""{webPosArg.txnDateTime}"" 
        //                                receiptNo=""{webPosArg.receiptNo}"" 
        //                                actualAmount=""{webPosArg.amount}"" 
        //                                payableAmount=""{webPosArg.amount}"" 
        //                                verificationCode="""" />
        //                           </cmd>";
        //    logger.LogInformation($"【自动积分】调用webpos接口的参数 | 【{param}】 ");
        //    CmdResponse cmdResult = null;
        //    try
        //    {
        //        WebPos.WebPOSSoapClient ws = new WebPos.WebPOSSoapClient(WebPos.WebPOSSoapClient.EndpointConfiguration.WebPOSSoap);
        //        WebPos.WebPOSCredentials mwc = new WebPos.WebPOSCredentials();
        //        mwc.Username = ConfigurationUtil.WebPosUserName;
        //        var password = ConfigurationUtil.WebPosPassWord;
        //        mwc.Password = ws.GetEncryptedCharAsync(password).Result;
        //        cmdResult = await ws.CmdAsync(mwc, param);
        //        logger.LogInformation($"【自动积分】webpos接口返回参数 | 【{JsonConvert.SerializeObject(cmdResult)}】 ");
        //    }
        //    catch (Exception ex)
        //    {
        //        TError($"WebPosForPoint异常：{ex.Message}");
        //    }
        //    if (!string.IsNullOrWhiteSpace(cmdResult.CmdResult) && cmdResult.CmdResult.Contains("hasError=\"false\""))
        //        return SuccessRes<CmdResponse>(cmdResult, "识别成功，完成自动积分且进行微信推送");
        //    return FailRes<CmdResponse>($"验证通过，积分失败。");
        //}




        /// <summary>
        /// 获取用户积分申请历史
        /// </summary>
        /// <param name="applyPointHistoryRequest"></param>
        /// <returns></returns>
        public List<ApplyPointModel> GetApplePointByCardId(string UnionId) =>
            _applyPointDAO.GetApplyPointHistory(UnionId);
    }
}
