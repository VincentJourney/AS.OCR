using AS.OCR.Model.Request;
using AS.OCR.Model.Response;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AS.OCR.Service
{
    public class ApplyPointService : Infrastructure
    {
        private ILogger logger;
        public ApplyPointService(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger(typeof(OCRService));
        }

        /// <summary>
        /// 创建积分申请单，校验信息成功并推送
        /// 若原先存在积分申请单，失败的原因：校验失败 所有应该重新赋值
        /// </summary>
        /// <param name="applyPointRequest"></param>
        /// <returns></returns>
        //public CreateApplyPointResponse CreateApplyPoint(CreateApplyPointRequest applyPointRequest)
        //{
        //    //所有广场
        //    List<Mall> MallList = GetCacheFromEntity<Mall, List<Mall>>(
        //        Key: "AllMall",
        //        Hour: ConfigurationUtil.CacheExpiration_Mall);

        //    Guid OrgID = Guid.Empty;
        //    if (Guid.TryParse(applyPointRequest.mallId, out var mid))
        //        OrgID = MallList.Where(s => s.MallID == mid).FirstOrDefault()?.OrgID ?? Guid.Empty;
        //    else
        //        TError("积分申请 【mallId】参数错误");

        //    Store StoreModel = GetCacheFromEntity<Store, Store>(
        //        Key: $"Store{applyPointRequest.receiptOCR.StoreId}",
        //        Hour: ConfigurationUtil.CacheExpiration_Store,
        //        sqlWhere: $"StoreId = '{applyPointRequest.receiptOCR.StoreId}'");

        //    StoreOCR StoreOCRRule = GetCacheFromEntity<StoreOCR, StoreOCR>(
        //        Key: $"StoreOCR{applyPointRequest.receiptOCR.StoreId}",
        //        Hour: ConfigurationUtil.CacheExpiration_StoreOCR,
        //        sqlWhere: $"StoreId = '{applyPointRequest.receiptOCR.StoreId}'");

        //    //增加原始小票
        //    var OriginReceiptNo = applyPointRequest.receiptOCR.ReceiptNo;
        //    var txnDateTime = applyPointRequest.receiptOCR.TransDatetime ?? DateTime.Now;
        //    if (!string.IsNullOrWhiteSpace(applyPointRequest.receiptOCR.ReceiptNo))
        //        applyPointRequest.receiptOCR.ReceiptNo = $"{StoreModel?.StoreCode}01{txnDateTime.ToString("yyyyMMdd")}{applyPointRequest.receiptOCR.ReceiptNo}";

        //    ApplyPoint ApplyPoint = null;
        //    if (!string.IsNullOrWhiteSpace(applyPointRequest.receiptOCR.ReceiptNo) && applyPointRequest.receiptOCR.StoreId != Guid.Empty)
        //        ApplyPoint = applyPointDAO.GetModel($@" and ReceiptNo='{applyPointRequest.receiptOCR.ReceiptNo}' 
        //                                                  and StoreID='{applyPointRequest.receiptOCR.StoreId}'
        //                                                  and TransDate='{applyPointRequest.receiptOCR.TransDatetime}'");

        //    ApplyPictureRecongnize applyPictureRecongnize = applyPictureRecongnizeDAO.Get(applyPointRequest.receiptOCR.RecongnizelId);

        //    if (applyPictureRecongnize == null)
        //        TError("can't find ApplyPictureRecongnize");

        //    if (ApplyPoint != null)
        //        TError("该小票已积分，不可重复提交");

        //    var ApplyPointId = Guid.NewGuid();
        //    ApplyPoint = new ApplyPoint
        //    {
        //        ApplyPointID = ApplyPointId,
        //        MallID = StoreModel?.MallID ?? Guid.Empty,
        //        StoreID = applyPointRequest.receiptOCR.StoreId,
        //        CardID = Guid.Parse(applyPointRequest.cardId),
        //        ReceiptNo = applyPointRequest.receiptOCR.ReceiptNo,
        //        TransDate = applyPointRequest.receiptOCR.TransDatetime,
        //        TransAmt = applyPointRequest.receiptOCR.TranAmount,
        //        VerifyStatus = StoreOCRRule?.needVerify == 0 ? 1 : 0,
        //        ReceiptPhoto = applyPointRequest.receiptOCR.Base64,
        //        AddedOn = DateTime.Now,
        //        OriginReceiptNo = OriginReceiptNo,
        //        OrgID = OrgID
        //    };

        //    applyPictureRecongnize.applyid = ApplyPointId;
        //    applyPictureRecongnizeDAO.Update(applyPictureRecongnize);

        //    var VerifyRecognitionResult = VerifyRecognition(applyPointRequest.receiptOCR);//校验结果
        //    ApplyPoint.AuditDate = DateTime.Now;
        //    if (!VerifyRecognitionResult.Success)//校验失败
        //    {
        //        ApplyPoint.RecongizeStatus = 3;
        //        ApplyPoint.VerifyStatus = 0;
        //        ApplyPoint.Status = 0;
        //        applyPointDAO.Add(ApplyPoint);
        //        return VerifyRecognitionResult;
        //    }

        //    ApplyPoint.RecongizeStatus = 2;
        //    ApplyPoint.VerifyStatus = 1;
        //    applyPointDAO.Add(ApplyPoint);

        //    List<Company> companyList = GetCacheFromEntity<Company, List<Company>>(
        //        Key: "Company",
        //        Hour: ConfigurationUtil.CacheExpiration_Company);
        //    List<OrgInfo> orgList = GetCacheFromEntity<OrgInfo, List<OrgInfo>>(
        //        Key: "OrgInfo",
        //        Hour: ConfigurationUtil.CacheExpiration_OrgInfo);


        //    var StoreCode = StoreModel?.StoreCode;
        //    //自动积分 推送
        //    var arg = new WebPosArg
        //    {
        //        companyID = companyList.Where(s => s.CompanyId == (StoreModel?.CompanyID ?? Guid.Empty)).FirstOrDefault()?.CompanyCode ?? "",
        //        storeID = StoreCode,
        //        cardID = cardDAO.Get(Guid.Parse(applyPointRequest.cardId))?.CardCode ?? "",
        //        cashierID = "CrmAIApplyPoint",
        //        discountPercentage = 0,
        //        orgID = MallList.Where(s => s.MallID == (StoreModel?.MallID ?? Guid.Empty)).FirstOrDefault()?.MallCode ?? "",
        //        receiptNo = applyPointRequest.receiptOCR.ReceiptNo,
        //        txnDateTime = txnDateTime,
        //        amount = applyPointRequest.receiptOCR.TranAmount,
        //    };
        //    var result = WebPosForPoint(arg).Result;
        //    ApplyPoint.Remark = result.Message;
        //    if (!result.Success)
        //    {
        //        ApplyPoint.Status = 2;
        //        applyPointDAO.Update(ApplyPoint);
        //        return SuccessRes<string>(null, "提交积分申请成功，积分失败！");
        //    }

        //    ApplyPoint.Status = 1;
        //    applyPointDAO.Update(ApplyPoint);

        //    return SuccessRes<string>(null, "积分申请成功！");
        //}

        ///// <summary>
        ///// 根据信息校验用户是否篡改信息，是否满足商铺积分规则
        ///// </summary>
        ///// <param name="receiptOCR"></param>
        ///// <returns></returns>
        //private Result<string> VerifyRecognition(ReceiptOCR receiptOCR)
        //{
        //    //识别的原始数据
        //    var RecongnizeModel = applyPictureRecongnizeDAO.Get(receiptOCR.RecongnizelId);
        //    if (RecongnizeModel == null)
        //        return FailRes("校验失败：识别数据丢失，请重新识别小票！");

        //    #region 检查数据是否篡改(Contain 不是绝对)
        //    ReceiptOCR OldReceipt = JsonConvert.DeserializeObject<ReceiptOCR>(RecongnizeModel.OCRResult);
        //    var result = CompareModel<ReceiptOCR>(OldReceipt, receiptOCR);
        //    if (!result.Success)
        //        return FailRes("校验失败：提交数据与小票不一致！");
        //    #endregion

        //    #region 检查数据可靠性

        //    if (string.IsNullOrWhiteSpace(receiptOCR.MallName) ||
        //        string.IsNullOrWhiteSpace(receiptOCR.ReceiptNo) ||
        //        string.IsNullOrWhiteSpace(receiptOCR.StoreCode) ||
        //        string.IsNullOrWhiteSpace(receiptOCR.MallName) ||
        //        receiptOCR.TranAmount == 0 ||
        //        receiptOCR.TransDatetime == DefaultDateTime || receiptOCR.TransDatetime == null)
        //        return FailRes("校验失败：小票未识别完整！");
        //    #endregion

        //    #region 匹配商铺规则
        //    StoreOCR StoreOCRRule = GetCacheFromEntity<StoreOCR, StoreOCR>(
        //       Key: $"StoreOCR{receiptOCR.StoreId}",
        //       Hour: ConfigurationUtil.CacheExpiration_StoreOCR,
        //       sqlWhere: $"and StoreId = '{receiptOCR.StoreId}'");

        //    if (StoreOCRRule == null)
        //        return FailRes("校验失败：未配置积分规则！");

        //    if (StoreOCRRule.Enabled != 1)
        //        return FailRes("校验失败：商铺未启用自动积分");

        //    if (StoreOCRRule.needVerify == 1) //当商铺启用校验规则
        //    {
        //        if (receiptOCR.TranAmount < StoreOCRRule.MinValidReceiptValue || receiptOCR.TranAmount > StoreOCRRule.MaxValidReceiptValue)
        //            return FailRes("校验失败：小票金额不在店铺规则范围之内！");

        //        if (StoreOCRRule.MaxTicketPerDay != 0)  //日自动交易笔数为0时 代表不限制
        //        {
        //            var TicketPerDay = applyPointDAO.GetApplyPointCountByDay(receiptOCR.StoreId); //当日交易笔数
        //            if (TicketPerDay >= StoreOCRRule.MaxTicketPerDay)
        //                return FailRes("校验失败：今日已超过最大自动积分记录数量");
        //        }

        //        Store StoreModel = GetCacheFromEntity<Store, Store>(
        //            Key: $"Store{receiptOCR.StoreId}",
        //            Hour: ConfigurationUtil.CacheExpiration_Store,
        //            sqlWhere: $" and StoreId = '{receiptOCR.StoreId}'");

        //        if ((StoreModel.IsStandardPOS == "1" ? 0 : 1) != StoreOCRRule.POSType)
        //            return FailRes("校验失败：OCR商铺POS类型不一致");

        //        //POS门店代码暂无验证
        //    }
        //    #endregion

        //    return SuccessRes<string>(null, "验证成功");
        //}


        ///// <summary>
        ///// 获取用户积分申请历史
        ///// </summary>
        ///// <param name="applyPointHistoryRequest"></param>
        ///// <returns></returns>
        //public Result<List<ApplyPointModel>> GetApplePointByCardId(ApplyPointHistoryRequest applyPointHistoryRequest)
        //{

        //    if (!Guid.TryParse(applyPointHistoryRequest.cardId, out var CardId))
        //        TError("参数错误！");
        //    return SuccessRes(applyPointDAO.GetApplyPointHistory(CardId));
        //}

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

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="arg"></param>
        ///// <returns></returns>
        //public async Task<Result<CmdResponse>> CommitApplyPoint(WebPosRequest arg)
        //{
        //    //"storecode=13160012&tillid=01&docno=S0020119000195&txdate=20190925&txtime=202208&amount=200.00&orgid=13&cashier=131600121&sign=E82DAADD6105BACFE6B2CC94C99253E2";
        //    logger.LogInformation($"【CommitApplyPoint】 【params】:【{JsonConvert.SerializeObject(arg)}】");
        //    Dictionary<string, string> d = new Dictionary<string, string>();
        //    Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
        //    MatchCollection mc = re.Matches(arg.arg);
        //    foreach (Match m in mc)
        //        d.Add(m.Result("$2").ToLower(), m.Result("$3"));

        //    if (d == null)
        //        TError("参数错误");

        //    #region 缓存取公共信息
        //    Store StoreModel = GetCacheFromEntity<Store, Store>(
        //      Key: $"Store{d["storecode"]}",
        //      Hour: ConfigurationUtil.CacheExpiration_Store,
        //      sqlWhere: $"StoreCode = '{d["storecode"]}'");

        //    List<Company> companyList = GetCacheFromEntity<Company, List<Company>>(
        //           Key: "Company",
        //           Hour: ConfigurationUtil.CacheExpiration_Company);
        //    #endregion

        //    var receiptNo = string.Empty;
        //    DateTime txnDateTime = DefaultDateTime;
        //    var cashierID = string.Empty;
        //    decimal amount = 0M;
        //    var orgid = string.Empty;
        //    var storecode = string.Empty;
        //    var poscode = string.Empty;
        //    try
        //    {
        //        receiptNo = d["docno"];
        //        txnDateTime = DateTime.ParseExact($"{d["txdate"]}{d["txtime"]}", "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture); ;
        //        cashierID = d["cashier"];
        //        amount = decimal.Parse(d["amount"]);
        //        orgid = d["orgid"];
        //        storecode = d["storecode"];
        //        poscode = d["tillid"];
        //    }
        //    catch (Exception ex)
        //    {
        //        TError($"提交到webpos参数异常:{ex.Message}");
        //    }

        //    //自动积分 推送
        //    var webPosArg = new WebPosArg
        //    {
        //        companyID = companyList.Where(s => s.CompanyId == StoreModel.CompanyID).FirstOrDefault()?.CompanyCode ?? "",
        //        storeID = storecode,
        //        cardID = cardDAO.Get(Guid.Parse(arg.cardId))?.CardCode ?? "",
        //        cashierID = cashierID,
        //        discountPercentage = 0,
        //        orgID = orgid,
        //        receiptNo = $"{storecode}{poscode}{d["txdate"]}{receiptNo}",
        //        txnDateTime = txnDateTime,
        //        amount = amount,
        //    };

        //    return await WebPosForPoint(webPosArg);
        //}

    }
}
