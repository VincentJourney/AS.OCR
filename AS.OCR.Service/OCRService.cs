using AS.OCR.Commom.Util;
using AS.OCR.Dapper.DAO;
using AS.OCR.Extension.SDK.TencentOCR;
using AS.OCR.Model.Business;
using AS.OCR.Model.Entity;
using AS.OCR.Model.Enum;
using AS.OCR.Model.Request;
using AS.OCR.Model.Response;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebPos;

namespace AS.OCR.Service
{
    public class OCRService : Infrastructure
    {
        private ILogger logger;
        private OCRLogDAO OCRLogDAO = new OCRLogDAO();

        public OCRService(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger(typeof(OCRService));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oCRRequest"></param>
        ///// <returns></returns>
        //public async Task<ReceiptOCRResponse> ReceiptOCR(ReceiptRequest oCRRequest)
        //{
        //    var imageUrl = (oCRRequest.Type == null || oCRRequest.Type == 1) ? oCRRequest.imageUrl : "";
        //    return await Task.Run(() =>
        //        RecognitOCRResult(TencentOCR.GeneralAccurateOCR(oCRRequest.imageUrl, oCRRequest.Type),
        //                          imageUrl));
        //}

        ///// <summary>
        ///// 从OCR接口中 根据规则 获取详细内容 （暂无校验）
        ///// </summary>
        ///// <param name="OcrResult"></param>
        ///// <param name="mallId"></param>
        ///// <param name="ImageUrl"></param>
        ///// <returns></returns>
        //private ReceiptOCRResponse RecognitOCRResult(TencentOCRResult OcrResult, string ImageUrl)
        //{
        //    List<Word> WordList = OcrResult.WordList.Select(s => new Word { text = s.text }).ToList();
        //    //查询所有的商铺规则并缓存
        //    List<OCRVerifyRule> OCRVerifyRuleList = GetCacheFromEntity<OCRVerifyRule, List<OCRVerifyRule>>(
        //        Key: "OCRVerifyRuleList",
        //        Hour: ConfigurationUtil.CacheExpiration_StoreOCRDetailRuleList);
        //    //最后识别结果
        //    var ReceiptOCRModel = new ReceiptOCRResponse
        //    {
        //        StoreId = Guid.Empty,
        //        StoreName = "",
        //        ReceiptNo = "",
        //        TranAmount = 0,
        //        TransDatetime = null,
        //        RecongnizelId = Guid.Empty,
        //        AccountId = AccountInfo.Account.Id,
        //        AccountName = AccountInfo.Account.AccountName
        //    };

        //    //配置广场的规则：识别小票后与规则对比？   
        //    //1，拿到小票关键字
        //    //2，确定 Store 
        //    //3，确定 mall
        //    //4，拿到规则
        //    //所有广场
        //    #region 匹配 商铺 广场 信息  
        //    var StoreRuleList = OCRVerifyRuleList
        //        .Where(s => s.KeyType == (int)OCRKeyType.StoreName).ToList();
        //    List<Store> IdentifiedStoreList = new List<Store>(); //识别的商铺集合
        //    foreach (var Detail in StoreRuleList)
        //    {
        //        foreach (var word in WordList)
        //        {
        //            var key = Detail.OCRKey.ToLower();
        //            if (word.text.Contains(key))//只需要小票包含关键字
        //            {
        //                //根据小票关键字找到规则所在的StoreId，加上前端传来的mallid
        //                var IdentifiedStore = storeDAO.Get(Detail.StoreId);
        //                if (IdentifiedStore != null)
        //                    IdentifiedStoreList.Add(IdentifiedStore);
        //            }
        //        }
        //    }

        //    List<Mall> MallList = null;
        //    Store StoreModel = null;
        //    if (IdentifiedStoreList != null && IdentifiedStoreList.Count() > 0)
        //    {
        //        foreach (var item in IdentifiedStoreList)
        //        {
        //            var mallnamekey = OCRVerifyRuleList.Where(s => s.StoreId == item.StoreId
        //            && s.OCRKeyType == (int)OCRKeyType.MallName).Select(s => s.OCRKey).FirstOrDefault();
        //            if (!string.IsNullOrWhiteSpace(mallnamekey))
        //            {
        //                foreach (var word in WordList)
        //                {
        //                    if (word.text.Contains(mallnamekey.ToLower()))//只需要小票包含关键字
        //                    {
        //                        StoreModel = item;
        //                        Mall thismall = MallList.Where(s => s.MallID == item.MallID).FirstOrDefault();
        //                        ReceiptOCRModel.MallName = thismall.MallName;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    var StoreId = StoreModel?.StoreId ?? Guid.Empty;
        //    ReceiptOCRModel.StoreId = StoreId;
        //    ReceiptOCRModel.StoreName = StoreModel?.StoreName ?? "";
        //    ReceiptOCRModel.StoreCode = StoreModel?.StoreCode ?? "";

        //    //当前店铺的规则明细
        //    var ThisStoreOCRDetail = OCRVerifyRuleList.Where(s => s.StoreId == StoreId).OrderByDescending(s => s.AddedOn).ToList();
        //    #endregion

        //    //根据店铺规则明细 关键字类型 关键字 取值方法 匹配识别结果
        //    ReceiptOCRModel = Matching(WordList, ReceiptOCRModel, ThisStoreOCRDetail);

        //    var RecongnizeModelId = Guid.NewGuid();
        //    var RecongnizeModel = new ApplyPictureRecongnize
        //    {
        //        id = RecongnizeModelId,
        //        applyid = Guid.Empty,
        //        Lineno = 0,
        //        LineContent = JsonConvert.SerializeObject(WordList),
        //        OCRResult = JsonConvert.SerializeObject(ReceiptOCRModel),
        //        AddedTime = DateTime.Now
        //    };
        //    //添加原始数据 applyid 等待积分申请
        //    if (!applyPictureRecongnizeDAO.Add(RecongnizeModel))
        //        TError("添加到ApplyPictureRecongnize失败");

        //    ReceiptOCRModel.RecongnizelId = RecongnizeModelId;

        //    return ReceiptOCRModel;

        //    //匹配
        //    ReceiptOCRResponse Matching(List<Word> WordList, ReceiptOCRResponse ReceiptOCRModel, List<OCRVerifyRule> ThisStoreOCRDetail)
        //    {
        //        for (int i = 0; i < WordList.Count(); i++)
        //        {
        //            foreach (var StoreDetailRule in ThisStoreOCRDetail)
        //            {
        //                Result<string> ReturnResult = GetValue(WordList, i, StoreDetailRule); //根据规则取值
        //                if (ReturnResult.Success)
        //                {
        //                    var ReturnData = ReturnResult.Data.ToString();
        //                    if (!string.IsNullOrWhiteSpace(ReturnData))
        //                    {
        //                        switch (StoreDetailRule.OCRKeyType) //枚举有注释，根据关键字类型赋值
        //                        {
        //                            case (int)OCRKeyType.StoreName:
        //                                continue;
        //                            case (int)OCRKeyType.ReceiptNO:
        //                                if (!string.IsNullOrWhiteSpace(ReturnData) && string.IsNullOrWhiteSpace(ReceiptOCRModel.ReceiptNo))
        //                                {
        //                                    ReceiptOCRModel.ReceiptNo = Regex.Replace(ReturnResult.Data.ToString(), "[ \\[ \\] \\^ \\-_*×――(^)（^）$%~!@#$…&%￥—+=<>《》!！??？:：•`·、/ 。，；,.;\"‘’“”_\u4e00-\u9fa5\' ']", "");
        //                                    continue;
        //                                }
        //                                break;
        //                            case (int)OCRKeyType.DateTime:
        //                                if (!string.IsNullOrWhiteSpace(ReturnData) && ReceiptOCRModel.TransDatetime == null)
        //                                {
        //                                    try
        //                                    {
        //                                        ReturnData = Regex.Replace(ReturnData, @"[^0-9]+", "");
        //                                        if (ReturnData.Length < 14)
        //                                        {
        //                                            var head = ReturnData.Substring(0, 2);
        //                                            if (head != "20")
        //                                                ReturnData = $"{DateTime.Now.Year}{ReturnData}";

        //                                            ReturnData = ReturnData.PadRight(14, '0');
        //                                        }
        //                                        if (ReturnData.Length > 14) //2019 - 01 - 01 12:30:01 - 13:30:01
        //                                            ReturnData = ReturnData.Substring(0, 8).PadRight(14, '0');

        //                                        DateTime dt = DateTime.ParseExact(ReturnData, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
        //                                        ReceiptOCRModel.TransDatetime = dt;
        //                                    }
        //                                    catch (Exception ex)
        //                                    {
        //                                        ReceiptOCRModel.TransDatetime = null;
        //                                        logger.LogError(eventId: 1, ex, "时间转换报错");
        //                                        //TError($"OCR取值时间：{ReturnData},格式转换错误{ex.Message}");
        //                                    }
        //                                    continue;
        //                                }
        //                                break;
        //                            case (int)OCRKeyType.Amount:
        //                                if (!string.IsNullOrWhiteSpace(ReturnData) && ReceiptOCRModel.TranAmount == 0)
        //                                {
        //                                    ReturnData = Regex.Replace(ReturnData, "[ \\[ \\] \\^ \\-_*×――(^)（^）$%~!@#$…&%￥—+=<>《》!！??？:：•`·、/ 。，；,;\"‘’“”_\u4e00-\u9fa5\' ']", "");
        //                                    if (decimal.TryParse(ReturnData, out var AmountResult))
        //                                    {
        //                                        ReceiptOCRModel.TranAmount = AmountResult;
        //                                        continue;
        //                                    }
        //                                }
        //                                break;
        //                            case (int)OCRKeyType.MallName:
        //                                //if (!string.IsNullOrWhiteSpace(ReturnData) && string.IsNullOrWhiteSpace(ReceiptOCRModel.MallName))
        //                                //{
        //                                //    ReceiptOCRModel.MallName = thisMall?.MallName ?? "";
        //                                //    continue;
        //                                //}
        //                                break;
        //                            default:
        //                                TError($"商铺未设置该关键字类型取值方法：{StoreDetailRule.OCRKeyType}");
        //                                break;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        return ReceiptOCRModel;
        //    }

        //    //根据规则获取指定识别内容
        //    Result<string> GetValue(List<Word> words_result, int index, OCRVerifyRule StoreDetailRule)
        //    {
        //        var WordValue = "";
        //        var Key = StoreDetailRule.OCRKey.Trim().Replace("：", "").Replace(":", "").ToLower().Split(',');
        //        if (Key.Length < 2)
        //        {
        //            var newWord = words_result[index].text.Replace("：", "").Replace(":", "");
        //            if (!newWord.Contains(Key[0]))
        //                return FailRes();
        //        }
        //        else
        //        {
        //            if (!words_result[index].text.Contains(Key[0]) || !words_result[index].text.Contains(Key[1]))
        //                return FailRes();
        //        }

        //        switch (StoreDetailRule.GetValueWay)//可查看枚举注释
        //        {
        //            case (int)GetValueWay.OCRKey://在关键字中 用下标取值
        //                WordValue = GetValueByIndexArr(words_result[index].text, StoreDetailRule.SkipLines);
        //                break;

        //            case (int)GetValueWay.AfterOCRKey://关键字后 根据下标取值
        //                int IndexOfKey = words_result[index].text.IndexOf(StoreDetailRule.OCRKey) + StoreDetailRule.OCRKey.Length;
        //                string AfterKey = words_result[index].text.Substring(IndexOfKey);//关键字之后的文字
        //                WordValue = GetValueByIndexArr(AfterKey, StoreDetailRule.SkipLines);
        //                break;

        //            case (int)GetValueWay.NextLine://隔行： 隔N行后取整行数据
        //                if (int.TryParse(StoreDetailRule.SkipLines, out var NextLine))
        //                {
        //                    NextLine = index + int.Parse(StoreDetailRule.SkipLines);
        //                    if (NextLine <= words_result.Count())
        //                        WordValue = words_result[NextLine].text;
        //                }
        //                break;

        //            case (int)GetValueWay.BeforeOCRKey://关键字前 根据下标取值
        //                int IndexOfKey2 = words_result[index].text.IndexOf(StoreDetailRule.OCRKey);
        //                string BeforeKey = words_result[index].text.Substring(0, IndexOfKey2);//关键字之前的文字
        //                WordValue = GetValueByIndexArr(BeforeKey, StoreDetailRule.SkipLines);
        //                break;

        //            case (int)GetValueWay.BetweenOCRKey://关键字之间 并用下标取值
        //                var KetArr = StoreDetailRule.OCRKey.Split(',');
        //                int afterkeyIndex = words_result[index].text.IndexOf(KetArr[0]) + KetArr[0].Length;
        //                string AfterKeyWord = words_result[index].text.Substring(afterkeyIndex);//关键字之后
        //                int beforekeyIndex = AfterKeyWord.IndexOf(KetArr[1]);
        //                string BeforeKeyWord = AfterKeyWord.Substring(0, beforekeyIndex);//关键字之前
        //                WordValue = GetValueByIndexArr(BeforeKeyWord, StoreDetailRule.SkipLines);
        //                break;
        //            default:
        //                TError($"商铺未设置该关键字取值方法：{StoreDetailRule.GetValueWay}");
        //                break;
        //        }

        //        return SuccessRes(WordValue);
        //    }

        //    //Str: 文字
        //    //indexArr：取值位置可以逗号分隔
        //    string GetValueByIndexArr(string Str, string indexArr)
        //    {
        //        if (string.IsNullOrWhiteSpace(Str)) return "";
        //        if (string.IsNullOrWhiteSpace(indexArr)) return Str;
        //        var Arr = indexArr.Split(',');
        //        var Result = string.Empty;
        //        if (Arr.Length == 1)
        //            Result = Str.Substring(int.Parse(Arr[0]));//取这个下标之后所有的
        //        else
        //            Result = Str.Substring(int.Parse(Arr[0]), int.Parse(Arr[1]));//取下标之间
        //        return Result;
        //    }

        //}
    }
}
