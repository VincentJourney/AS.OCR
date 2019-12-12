using AS.OCR.Commom.Util;
using AS.OCR.Dapper.DAO;
using AS.OCR.Extension.SDK.TencentOCR;
using AS.OCR.Model.Business;
using AS.OCR.Model.Entity;
using AS.OCR.Model.Enum;
using AS.OCR.Model.Request;
using AS.OCR.Model.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AS.OCR.Service
{
    public class OCRService : Infrastructure
    {
        /// <summary>
        /// 缓存锁  避免并发设置缓存增加DB压力
        /// </summary>
        private static readonly object cacheLocker = new object();

        private static StoreDAO storeDAO = new StoreDAO();
        /// <summary>
        /// 图片识别
        /// </summary>
        /// <param name="oCRRequest"></param>
        /// <returns>识别结果</returns>
        public static async Task<Result> ReceiptOCR(OCRRequest oCRRequest)
        {
            var imageUrl = "";
            if (oCRRequest.Type == null || oCRRequest.Type == 1)
                imageUrl = oCRRequest.imageUrl;
            return await Task.Run(() =>
                RecognitOCRResult(
                    TencentOCR.GeneralAccurateOCR(oCRRequest.imageUrl, oCRRequest.Type),
                    oCRRequest.mallId,
                    imageUrl));
        }


        /// <summary>
        /// 从OCR接口中 根据规则 获取详细内容 （暂无校验）
        /// </summary>
        /// <param name="OcrResult"></param>
        /// <param name="mallId"></param>
        /// <param name="ImageUrl"></param>
        /// <returns></returns>
        private static Result RecognitOCRResult(TencentOCRResult OcrResult, string mallId, string ImageUrl)
        {
            if (string.IsNullOrWhiteSpace(mallId))
                TError("请选择 Mall");

            List<Word> WordList = OcrResult.WordList
                .Select(s => new Word
                {
                    text = s.text
                }).ToList();
            //查询所有的商铺规则并缓存
            List<StoreOCRDetail> AllStoreOCRDetailRuleList = CacheHandle<List<StoreOCRDetail>>(
                Key: "AllStoreOCRDetailRuleList"
                , Hour: ConfigurationUtil.CacheExpiration_StoreOCRDetailRuleList
                , sqlWhere: "");
            //最后识别结果
            var ReceiptOCRModel = new ReceiptOCR
            {
                StoreId = Guid.Empty,
                StoreName = "",
                StoreCode = "",
                ReceiptNo = "",
                TranAmount = 0,
                TransDatetime = null,
                RecongnizelId = Guid.Empty,
                Base64 = ImageUrl,
                MallName = ""
            };

            //配置广场的规则：识别小票后与规则对比？   
            //1，拿到小票关键字
            //2，确定 Store 
            //3，确定 mall
            //4，拿到规则
            //所有广场
            #region 匹配商铺信息  
            var StoreNameRuleList = AllStoreOCRDetailRuleList
                .Where(s => s.OCRKeyType == (int)OCRKeyType.StoreName).ToList();
            List<Store> recognitionList = new List<Store>();
            foreach (var Detail in StoreNameRuleList)
            {
                foreach (var word in WordList)
                {
                    var key = Detail.OCRKey.ToLower();
                    if (word.text.Contains(key))//只需要小票包含关键字
                    {
                        //根据小票关键字找到规则所在的StoreId，加上前端传来的mallid
                        //var recognitionModel = storeDAO.GetById(Detail.StoreId.ToString(), mallId);
                        var recognitionModel = storeDAO.Get(Detail.StoreId);
                        if (recognitionModel != null)
                            recognitionList.Add(recognitionModel);
                    }
                }
            }

            List<Mall> MallList = CacheHandle<List<Mall>>(Key: "AllMall",
                                                          Hour: int.Parse(ConfigurationUtil.GetSection("ObjectConfig:CacheExpiration:AllMall")),
                                                          sqlWhere: "");
            Store StoreModel = null;
            if (recognitionList != null && recognitionList.Count() > 0)
            {
                foreach (var item in recognitionList)
                {
                    var mallnamekey = AllStoreOCRDetailRuleList.Where(s => s.StoreId == item.StoreId && s.OCRKeyType == (int)OCRKeyType.MallName).Select(s => s.OCRKey).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(mallnamekey))
                    {
                        foreach (var word in WordList)
                        {
                            if (word.words.Contains(mallnamekey.ToLower()))//只需要小票包含关键字
                            {
                                StoreModel = item;
                                Mall thismall = MallList.Where(s => s.MallID == item.MallID).FirstOrDefault();
                                ReceiptOCRModel.MallName = thismall.MallName;
                            }
                        }
                    }
                }
            }

            var StoreId = StoreModel?.StoreId ?? Guid.Empty;
            ReceiptOCRModel.StoreId = StoreId;
            ReceiptOCRModel.StoreName = StoreModel?.StoreName ?? "";
            ReceiptOCRModel.StoreCode = StoreModel?.StoreCode ?? "";

            //当前店铺的规则明细
            var ThisStoreOCRDetail = AllStoreOCRDetailRuleList.Where(s => s.StoreId == StoreId).OrderByDescending(s => s.AddedOn).ToList();
            #endregion

            //根据店铺规则明细 关键字类型 关键字 取值方法 匹配识别结果在·

            for (int i = 0; i < WordList.Count(); i++)
            {
                foreach (var StoreDetailRule in ThisStoreOCRDetail)
                {
                    Result ReturnResult = GetValue(WordList, i, StoreDetailRule); //根据规则取值
                    if (ReturnResult.Success)
                    {
                        var ReturnData = ReturnResult.Data.ToString();
                        if (!string.IsNullOrWhiteSpace(ReturnData))
                        {
                            switch (StoreDetailRule.OCRKeyType) //枚举有注释，根据关键字类型赋值
                            {
                                case (int)OCRKeyType.StoreName:
                                    continue;
                                case (int)OCRKeyType.ReceiptNO:
                                    if (!string.IsNullOrWhiteSpace(ReturnData) && string.IsNullOrWhiteSpace(ReceiptOCRModel.ReceiptNo))
                                    {
                                        ReceiptOCRModel.ReceiptNo = Regex.Replace(ReturnResult.Data.ToString(), "[ \\[ \\] \\^ \\-_*×――(^)（^）$%~!@#$…&%￥—+=<>《》!！??？:：•`·、/ 。，；,.;\"‘’“”_\u4e00-\u9fa5\' ']", "");
                                        continue;
                                    }
                                    break;
                                case (int)OCRKeyType.DateTime:
                                    if (!string.IsNullOrWhiteSpace(ReturnData) && ReceiptOCRModel.TransDatetime == null)
                                    {
                                        try
                                        {
                                            ReturnData = Regex.Replace(ReturnData, @"[^0-9]+", "");
                                            if (ReturnData.Length < 14)
                                            {
                                                var head = ReturnData.Substring(0, 2);
                                                if (head != "20")
                                                    ReturnData = $"{DateTime.Now.Year}{ReturnData}";

                                                ReturnData = ReturnData.PadRight(14, '0');
                                            }
                                            if (ReturnData.Length > 14) //2019 - 01 - 01 12:30:01 - 13:30:01
                                                ReturnData = ReturnData.Substring(0, 8).PadRight(14, '0');

                                            DateTime dt = DateTime.ParseExact(ReturnData, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                                            ReceiptOCRModel.TransDatetime = dt;
                                        }
                                        catch (Exception ex)
                                        {
                                            ReceiptOCRModel.TransDatetime = null;
                                            //TError($"OCR取值时间：{ReturnData},格式转换错误{ex.Message}");
                                        }
                                        continue;
                                    }
                                    break;
                                case (int)OCRKeyType.Amount:
                                    if (!string.IsNullOrWhiteSpace(ReturnData) && ReceiptOCRModel.TranAmount == 0)
                                    {
                                        ReturnData = Regex.Replace(ReturnData, "[ \\[ \\] \\^ \\-_*×――(^)（^）$%~!@#$…&%￥—+=<>《》!！??？:：•`·、/ 。，；,;\"‘’“”_\u4e00-\u9fa5\' ']", "");
                                        if (decimal.TryParse(ReturnData, out var AmountResult))
                                        {
                                            ReceiptOCRModel.TranAmount = AmountResult;
                                            continue;
                                        }
                                    }
                                    break;
                                case (int)OCRKeyType.MallName:
                                    //if (!string.IsNullOrWhiteSpace(ReturnData) && string.IsNullOrWhiteSpace(ReceiptOCRModel.MallName))
                                    //{
                                    //    ReceiptOCRModel.MallName = thisMall?.MallName ?? "";
                                    //    continue;
                                    //}
                                    break;
                                default:
                                    TError($"商铺未设置该关键字类型取值方法：{StoreDetailRule.OCRKeyType}");
                                    break;
                            }
                        }
                    }
                }
            }

            var RecongnizeModelId = Guid.NewGuid();
            var RecongnizeModel = new ApplyPictureRecongnize
            {
                id = RecongnizeModelId,
                applyid = Guid.Empty,
                Lineno = 0,
                LineContent = JsonConvert.SerializeObject(WordList),
                OCRResult = JsonConvert.SerializeObject(ReceiptOCRModel),
                AddedTime = DateTime.Now
            };
            var AddResult = DbContext.Add(RecongnizeModel);//添加原始数据 applyid 等待积分申请
                                                           //添加成功后 出参RecongnizeModelId
            if (AddResult == 0)
                ReceiptOCRModel.RecongnizelId = RecongnizeModelId;
            else
                TError("添加到ApplyPictureRecongnize失败");

            return new Result(true, "识别成功", ReceiptOCRModel);



            //根据规则获取指定识别内容
            Result GetValue(List<Words> words_result, int index, StoreOCRDetail StoreDetailRule)
            {
                var WordValue = "";
                var Key = StoreDetailRule.OCRKey.Trim().Replace("：", "").Replace(":", "").ToLower().Split(',');
                if (Key.Length < 2)
                {
                    var newWord = words_result[index].words.Replace("：", "").Replace(":", "");
                    if (!newWord.Contains(Key[0]))
                        return new Result(false, "", WordValue);
                }
                else
                {
                    if (!words_result[index].words.Contains(Key[0]) || !words_result[index].words.Contains(Key[1]))
                        return new Result(false, "", WordValue);
                }

                switch (StoreDetailRule.GetValueWay)//可查看枚举注释
                {
                    case (int)GetValueWay.OCRKey://在关键字中 用下标取值
                        WordValue = GetValueByIndexArr(words_result[index].words, StoreDetailRule.SkipLines);
                        break;

                    case (int)GetValueWay.AfterOCRKey://关键字后 根据下标取值
                        int IndexOfKey = words_result[index].words.IndexOf(StoreDetailRule.OCRKey) + StoreDetailRule.OCRKey.Length;
                        string AfterKey = words_result[index].words.Substring(IndexOfKey);//关键字之后的文字
                        WordValue = GetValueByIndexArr(AfterKey, StoreDetailRule.SkipLines);
                        break;

                    case (int)GetValueWay.NextLine://隔行： 隔N行后取整行数据
                        if (int.TryParse(StoreDetailRule.SkipLines, out var NextLine))
                        {
                            NextLine = index + int.Parse(StoreDetailRule.SkipLines);
                            if (NextLine <= words_result.Count())
                                WordValue = words_result[NextLine].words;
                        }
                        break;

                    case (int)GetValueWay.BeforeOCRKey://关键字前 根据下标取值
                        int IndexOfKey2 = words_result[index].words.IndexOf(StoreDetailRule.OCRKey);
                        string BeforeKey = words_result[index].words.Substring(0, IndexOfKey2);//关键字之前的文字
                        WordValue = GetValueByIndexArr(BeforeKey, StoreDetailRule.SkipLines);
                        break;

                    case (int)GetValueWay.BetweenOCRKey://关键字之间 并用下标取值
                        var KetArr = StoreDetailRule.OCRKey.Split(',');
                        int afterkeyIndex = words_result[index].words.IndexOf(KetArr[0]) + KetArr[0].Length;
                        string AfterKeyWord = words_result[index].words.Substring(afterkeyIndex);//关键字之后
                        int beforekeyIndex = AfterKeyWord.IndexOf(KetArr[1]);
                        string BeforeKeyWord = AfterKeyWord.Substring(0, beforekeyIndex);//关键字之前
                        WordValue = GetValueByIndexArr(BeforeKeyWord, StoreDetailRule.SkipLines);
                        break;
                    default:
                        TError($"商铺未设置该关键字取值方法：{StoreDetailRule.GetValueWay}");
                        break;
                }

                return new Result(true, "", WordValue);
            }

            //Str: 文字
            //indexArr：取值位置可以逗号分隔
            string GetValueByIndexArr(string Str, string indexArr)
            {
                if (string.IsNullOrWhiteSpace(Str)) return "";
                if (string.IsNullOrWhiteSpace(indexArr)) return Str;
                var Arr = indexArr.Split(',');
                var Result = string.Empty;
                if (Arr.Length == 1)
                    Result = Str.Substring(int.Parse(Arr[0]));//取这个下标之后所有的
                else
                    Result = Str.Substring(int.Parse(Arr[0]), int.Parse(Arr[1]));//取下标之间
                return Result;
            }

        }

        /// <summary>
        /// 根据信息校验用户是否篡改信息，是否满足商铺积分规则
        /// </summary>
        /// <param name="receiptOCR"></param>
        /// <returns></returns>
        private static Result VerifyRecognition(ReceiptOCR receiptOCR)
        {
            //识别的原始数据
            var RecongnizeModel = dal.GetModel<ApplyPictureRecongnize>($" and id ='{receiptOCR.RecongnizelId}'");
            if (RecongnizeModel == null)
                return new Result(false, "校验失败：识别数据丢失，请重新识别小票！", null);

            #region 检查数据是否篡改(Contain 不是绝对)
            ReceiptOCR OldReceipt = JsonConvert.DeserializeObject<ReceiptOCR>(RecongnizeModel.OCRResult);
            var result = CompareModel<ReceiptOCR>(OldReceipt, receiptOCR);
            if (!result.Success)
                return new Result(false, "校验失败：提交数据与小票不一致！", null);
            #endregion

            #region 检查数据可靠性

            if (string.IsNullOrWhiteSpace(receiptOCR.MallName) ||
                string.IsNullOrWhiteSpace(receiptOCR.ReceiptNo) ||
                string.IsNullOrWhiteSpace(receiptOCR.StoreCode) ||
                string.IsNullOrWhiteSpace(receiptOCR.MallName) ||
                receiptOCR.TranAmount == 0 ||
                receiptOCR.TransDatetime == Commom.DefaultDateTime || receiptOCR.TransDatetime == null)
            {
                return new Result(false, "校验失败：小票未识别完整！", null);
            }
            #endregion

            #region 匹配商铺规则
            StoreOCR StoreOCRRule = CacheHandle<StoreOCR>(
               Key: $"StoreOCR{receiptOCR.StoreId}",
               Hour: double.Parse(ConfigurationUtil.GetSection("ObjectConfig:CacheExpiration:StoreOCR")),
               sqlWhere: $"and StoreId = '{receiptOCR.StoreId}'");

            if (StoreOCRRule == null)
                return new Result(false, "校验失败：未配置积分规则！");

            if (StoreOCRRule.Enabled != 1)
                return new Result(false, "校验失败：商铺未启用自动积分");

            if (StoreOCRRule.needVerify == 1) //当商铺启用校验规则
            {
                if (receiptOCR.TranAmount < StoreOCRRule.MinValidReceiptValue || receiptOCR.TranAmount > StoreOCRRule.MaxValidReceiptValue)
                    return new Result(false, "校验失败：小票金额不在店铺规则范围之内！");

                if (StoreOCRRule.MaxTicketPerDay != 0)  //日自动交易笔数为0时 代表不限制
                {
                    var TicketPerDay = DbContext.Query<int>($@"select count(*) from ApplyPoint
                               where StoreID = '{receiptOCR.StoreId}' and VerifyStatus = 1 
                               and SourceType=7 and DATEDIFF(dd,AuditDate,GETDATE())=0").FirstOrDefault(); //当日交易笔数
                    if (TicketPerDay >= StoreOCRRule.MaxTicketPerDay)
                        return new Result(false, "校验失败：今日已超过最大自动积分记录数量");
                }

                Store StoreModel = CacheHandle<Store>(
                    Key: $"Store{receiptOCR.StoreId}",
                    Hour: double.Parse(ConfigurationUtil.GetSection("ObjectConfig:CacheExpiration:Store")),
                    sqlWhere: $" and StoreId = '{receiptOCR.StoreId}'");

                if ((StoreModel.IsStandardPOS == "1" ? 0 : 1) != StoreOCRRule.POSType)
                    return new Result(false, "校验失败：OCR商铺POS类型不一致");

                //POS门店代码暂无验证
            }
            #endregion

            return new Result(true, "验证成功", null);
        }

        /// <summary>
        /// 创建积分申请单，校验信息成功并推送
        /// 若原先存在积分申请单，失败的原因：校验失败 所有应该重新赋值
        /// </summary>
        /// <param name="applyPointRequest"></param>
        /// <returns></returns>
        public static Result CreateApplyPoint(ApplyPointRequest applyPointRequest)
        {
            //所有广场
            List<Mall> MallList = CacheHandle<List<Mall>>(
                Key: "AllMall",
                Hour: int.Parse(ConfigurationUtil.GetSection("ObjectConfig:CacheExpiration:AllMall")),
                sqlWhere: "");

            Guid OrgID = Guid.Empty;
            if (Guid.TryParse(applyPointRequest.mallId, out var mid))
                OrgID = MallList.Where(s => s.MallID == mid).FirstOrDefault()?.OrgID ?? Guid.Empty;
            else
                TError("积分申请 【mallId】参数错误");

            Store StoreModel = CacheHandle<Store>(
                Key: $"Store{applyPointRequest.receiptOCR.StoreId}",
                Hour: double.Parse(ConfigurationUtil.GetSection("ObjectConfig:CacheExpiration:Store")),
                sqlWhere: $" and StoreId = '{applyPointRequest.receiptOCR.StoreId}'");

            StoreOCR StoreOCRRule = CacheHandle<StoreOCR>(
                Key: $"StoreOCR{applyPointRequest.receiptOCR.StoreId}",
                Hour: double.Parse(ConfigurationUtil.GetSection("ObjectConfig:CacheExpiration:StoreOCR")),
                sqlWhere: $"and StoreId = '{applyPointRequest.receiptOCR.StoreId}'");

            //增加原始小票
            var OriginReceiptNo = applyPointRequest.receiptOCR.ReceiptNo;
            var txnDateTime = applyPointRequest.receiptOCR.TransDatetime ?? DateTime.Now;
            if (!string.IsNullOrWhiteSpace(applyPointRequest.receiptOCR.ReceiptNo))
                applyPointRequest.receiptOCR.ReceiptNo = $"{StoreModel?.StoreCode}01{txnDateTime.ToString("yyyyMMdd")}{applyPointRequest.receiptOCR.ReceiptNo}";

            ApplyPoint ApplyPoint = null;
            if (!string.IsNullOrWhiteSpace(applyPointRequest.receiptOCR.ReceiptNo) && applyPointRequest.receiptOCR.StoreId != Guid.Empty)
                ApplyPoint = dal.GetModel<ApplyPoint>($@" and ReceiptNo='{applyPointRequest.receiptOCR.ReceiptNo}' 
                                                          and StoreID='{applyPointRequest.receiptOCR.StoreId}'
                                                          and TransDate='{applyPointRequest.receiptOCR.TransDatetime}'");

            ApplyPictureRecongnize applyPictureRecongnize = dal.GetModel<ApplyPictureRecongnize>($" and id='{applyPointRequest.receiptOCR.RecongnizelId}'");

            if (applyPictureRecongnize == null)
                TError("can't find ApplyPictureRecongnize");

            if (ApplyPoint != null) TError("该小票已积分，不可重复提交");

            var ApplyPointId = Guid.NewGuid();
            ApplyPoint = new ApplyPoint
            {
                ApplyPointID = ApplyPointId,
                MallID = StoreModel?.MallID ?? Guid.Empty,
                StoreID = applyPointRequest.receiptOCR.StoreId,
                CardID = Guid.Parse(applyPointRequest.cardId),
                ReceiptNo = applyPointRequest.receiptOCR.ReceiptNo,
                TransDate = applyPointRequest.receiptOCR.TransDatetime,
                TransAmt = applyPointRequest.receiptOCR.TranAmount,
                VerifyStatus = StoreOCRRule?.needVerify == 0 ? 1 : 0,
                ReceiptPhoto = applyPointRequest.receiptOCR.Base64,
                AddedOn = DateTime.Now,
                OriginReceiptNo = OriginReceiptNo,
                OrgID = OrgID
            };

            applyPictureRecongnize.applyid = ApplyPointId;
            DbContext.Update(applyPictureRecongnize);

            var VerifyRecognitionResult = VerifyRecognition(applyPointRequest.receiptOCR);//校验结果
            ApplyPoint.AuditDate = DateTime.Now;
            if (!VerifyRecognitionResult.Success)//校验失败
            {
                ApplyPoint.RecongizeStatus = 3;
                ApplyPoint.VerifyStatus = 0;
                ApplyPoint.Status = 0;
                DbContext.Add(ApplyPoint);
                return VerifyRecognitionResult;
            }

            ApplyPoint.RecongizeStatus = 2;
            ApplyPoint.VerifyStatus = 1;
            DbContext.Add<ApplyPoint>(ApplyPoint);

            List<Company> companyList = CacheHandle<List<Company>>(
                Key: "Company",
                Hour: double.Parse(ConfigurationUtil.GetSection("ObjectConfig:CacheExpiration:Company")),
                sqlWhere: "");
            List<OrgInfo> orgList = CacheHandle<List<OrgInfo>>(
                Key: "OrgInfo",
                Hour: double.Parse(ConfigurationUtil.GetSection("ObjectConfig:CacheExpiration:OrgInfo")),
                sqlWhere: "");


            var StoreCode = StoreModel?.StoreCode;
            //自动积分 推送
            var arg = new WebPosArg
            {
                companyID = companyList.Where(s => s.CompanyId == (StoreModel?.CompanyID ?? Guid.Empty)).FirstOrDefault()?.CompanyCode ?? "",
                storeID = StoreCode,
                cardID = dal.GetModel<Card>($" and CardID='{applyPointRequest.cardId}'")?.CardCode ?? "",
                cashierID = "CrmAIApplyPoint",
                discountPercentage = 0,
                orgID = MallList.Where(s => s.MallID == (StoreModel?.MallID ?? Guid.Empty)).FirstOrDefault()?.MallCode ?? "",
                receiptNo = applyPointRequest.receiptOCR.ReceiptNo,
                txnDateTime = txnDateTime,
                amount = applyPointRequest.receiptOCR.TranAmount,
            };
            var result = WebPosForPoint(arg).Result;
            ApplyPoint.Remark = result.Message;
            if (!result.Success)
            {
                ApplyPoint.Status = 2;
                DbContext.Update<ApplyPoint>(ApplyPoint);
                return new Result(true, "提交积分申请成功，积分失败！", null);
            }

            ApplyPoint.Status = 1;
            DbContext.Update<ApplyPoint>(ApplyPoint);

            return new Result(true, "积分申请成功！", null);
        }

        /// <summary>
        /// 获取用户积分申请历史
        /// </summary>
        /// <param name="applyPointHistoryRequest"></param>
        /// <returns></returns>
        public static Result GetApplePointByCardId(ApplyPointHistoryRequest applyPointHistoryRequest)
        {

            if (!Guid.TryParse(applyPointHistoryRequest.cardId, out var CardId))
                TError("参数错误！");
            List<ApplyPointModel> applyPointList = dal.GetApplyPointHistory(CardId);
            return new Result(true, "", applyPointList);
        }








        /// <summary>
        /// 自动积分并微信推送接口
        /// </summary>
        /// <param name="webPosArg"></param>
        /// <returns></returns>
        public async static Task<Result> WebPosForPoint(WebPosArg webPosArg)
        {
            Log.Warn($@"WebPosForPoint 參數:
                      {JsonConvert.SerializeObject(webPosArg)}", null);
            //int amount = new Random(((unchecked((int)DateTime.Now.Millisecond + (int)DateTime.Now.Ticks)))).Next(10, 2000);
            string testString = $@"<cmd type=""SALES"" appCode=""POS"">
                                        <shared offline=""true"">
                                                <companyID>{webPosArg.companyID}</companyID>
                                                <orgID>{webPosArg.orgID}</orgID>
                                                <storeID>{webPosArg.storeID}</storeID>
                                                <cashierID>{webPosArg.cashierID}</cashierID>
                                        </shared>
                                        <sales discountPercentage=""{webPosArg.discountPercentage}"" 
                                        cardID=""{webPosArg.cardID}"" 
                                        txnDateTime=""{webPosArg.txnDateTime}"" 
                                        receiptNo=""{webPosArg.receiptNo}"" 
                                        actualAmount=""{webPosArg.amount}"" 
                                        payableAmount=""{webPosArg.amount}"" 
                                        verificationCode="""" />
                                   </cmd>";

            WebPos.CmdResponse cmdResult = null;
            try
            {
                WebPos.WebPOSSoapClient ws = new WebPos.WebPOSSoapClient(WebPos.WebPOSSoapClient.EndpointConfiguration.WebPOSSoap);
                Log.Warn($@"ws address:{ws.Endpoint.Address.Uri.AbsoluteUri}", null);
                WebPos.WebPOSCredentials mwc = new WebPos.WebPOSCredentials();


                mwc.Username = ConfigurationUtil.GetSection("ObjectConfig:WebPosService:UserName");
                var password = ConfigurationUtil.GetSection("ObjectConfig:WebPosService:PassWord");
                mwc.Password = ws.GetEncryptedCharAsync(password).Result;
                cmdResult = await ws.CmdAsync(mwc, testString);
                Log.Warn($@"cmdResult :{cmdResult}", null);
            }
            catch (Exception ex)
            {
                TError($"WebPosForPoint异常：{ex.Message}");
            }
            if (!string.IsNullOrWhiteSpace(cmdResult.CmdResult) && cmdResult.CmdResult.Contains("hasError=\"false\""))
            {
                return new Result(true, "识别成功，完成自动积分且进行微信推送", cmdResult);
            }
            return new Result(false, $"验证通过，积分失败。", cmdResult);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public async static Task<Result> CommitApplyPoint(WebPosRequest arg)
        {
            //"storecode=13160012&tillid=01&docno=S0020119000195&txdate=20190925&txtime=202208&amount=200.00&orgid=13&cashier=131600121&sign=E82DAADD6105BACFE6B2CC94C99253E2";
            Dictionary<string, string> d = new Dictionary<string, string>();
            Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
            MatchCollection mc = re.Matches(arg.arg);
            foreach (Match m in mc)
                d.Add(m.Result("$2").ToLower(), m.Result("$3"));

            if (d == null)
                TError("参数错误");

            #region 缓存取公共信息
            Store StoreModel = CacheHandle<Store>(
              Key: $"Store{d["storecode"]}",
              Hour: double.Parse(ConfigurationUtil.GetSection("ObjectConfig:CacheExpiration:Store")),
              sqlWhere: $" and StoreCode = '{d["storecode"]}'");

            List<Company> companyList = CacheHandle<List<Company>>(
                   Key: "Company",
                   Hour: double.Parse(ConfigurationUtil.GetSection("ObjectConfig:CacheExpiration:Company")),
                   sqlWhere: "");
            #endregion

            var receiptNo = string.Empty;
            DateTime txnDateTime = Commom.DefaultDateTime;
            var cashierID = string.Empty;
            decimal amount = 0M;
            var orgid = string.Empty;
            var storecode = string.Empty;
            var poscode = string.Empty;
            try
            {
                receiptNo = d["docno"];
                txnDateTime = DateTime.ParseExact($"{d["txdate"]}{d["txtime"]}", "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture); ;
                cashierID = d["cashier"];
                amount = decimal.Parse(d["amount"]);
                orgid = d["orgid"];
                storecode = d["storecode"];
                poscode = d["tillid"];
            }
            catch (Exception ex)
            {
                TError($"提交到webpos参数异常:{ex.Message}");
            }

            //自动积分 推送
            var webPosArg = new WebPosArg
            {
                companyID = companyList.Where(s => s.CompanyId == StoreModel.CompanyID).FirstOrDefault()?.CompanyCode ?? "",
                storeID = storecode,
                cardID = dal.GetModel<Card>($" and CardID='{arg.cardId}'")?.CardCode ?? "",
                cashierID = cashierID,
                discountPercentage = 0,
                orgID = orgid,
                receiptNo = $"{storecode}{poscode}{d["txdate"]}{receiptNo}",
                txnDateTime = txnDateTime,
                amount = amount,
            };

            return WebPosForPoint(webPosArg).Result;

        }
    }
}
