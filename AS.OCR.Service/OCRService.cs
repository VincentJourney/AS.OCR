using AS.OCR.Commom.Configuration;
using AS.OCR.Extension.SDK.TencentOCR;
using AS.OCR.IDAO;
using AS.OCR.IService;
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
using System.Text.RegularExpressions;

namespace AS.OCR.Service
{
    public class OCRService : Infrastructure, IOCRService
    {
        private ILogger _logger { get; }
        private IOCRLogDAO _iOCRLogDAO { get; }
        private IStoreDAO _storeDAO { get; }

        public OCRService(ILoggerFactory loggerFactory, IOCRLogDAO iOCRLogDAO, IStoreDAO storeDAO)
        {
            _logger = loggerFactory.CreateLogger(typeof(OCRService));
            _iOCRLogDAO = iOCRLogDAO;
            _storeDAO = storeDAO;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oCRRequest"></param>
        /// <returns></returns>
        public ReceiptOCRResponse ReceiptOCR(ReceiptRequest oCRRequest)
        {
            var imageUrl = (oCRRequest.Type == null || oCRRequest.Type == 1) ? oCRRequest.imageUrl : "";
            return RecognitOCRResult(TencentOCR.GeneralAccurateOCR(oCRRequest.imageUrl, oCRRequest.Type),
                                imageUrl);
        }

        /// <summary>
        /// 从OCR接口中 根据规则 获取详细内容 （暂无校验）
        /// </summary>
        /// <param name="OcrResult"></param>
        /// <param name="mallId"></param>
        /// <param name="ImageUrl"></param>
        /// <returns></returns>
        private ReceiptOCRResponse RecognitOCRResult(TencentOCRResult OcrResult, string ImageUrl)
        {
            List<Word> WordList = OcrResult.WordList.Select(s => new Word { text = s.text }).ToList();

            //查询所有的商铺规则并缓存
            List<OCRVerifyRule> OCRVerifyRuleList = GetCacheFromEntity<OCRVerifyRule, List<OCRVerifyRule>>(
                Key: ConfigurationUtil.RedisKeySet_OCRVerifyRuleList,
                Hour: ConfigurationUtil.RedisKeySet_OCRVerifyRuleList_expir);

            Store StoreModel = FindStoreByWords(WordList, OCRVerifyRuleList);//匹配根据文字与规则匹配商铺
            var StoreId = StoreModel?.StoreId ?? Guid.Empty;

            List<OCRVerifyRule> ThisStoreOCRDetail = OCRVerifyRuleList.Where(s => s.StoreId == StoreId).ToList(); //当前店铺的规则明细
            var MatchingResult = Matching(WordList, ThisStoreOCRDetail);//根据当前店铺的规则匹配

            var OCRLog = new OCRLog
            {
                Id = Guid.NewGuid(),
                ApplyId = Guid.Empty,
                ImageUrl = ImageUrl,
                AccountId = AccountInfo.Account.Id,
                StoreId = StoreId,
                OriginalWords = JsonConvert.SerializeObject(OcrResult),
                ReceiptNo = MatchingResult.ReceiptNo,
                StoreName = StoreModel.StoreName,
                TranAmount = MatchingResult.TranAmount.GetValueOrDefault(),
                TransDatetime = MatchingResult.TransDatetime,
                //MallId
                AddedOn = DateTime.Now,
            };
            _iOCRLogDAO.Add(OCRLog);//添加识别日志
            //-----------------------------------------------------------------------------------------------设计上暂未确定还有有问题  店铺应属于广场，这里可以根据商铺查询
            return new ReceiptOCRResponse
            {
                MallName = "",
                StoreId = StoreId,
                StoreName = StoreModel?.StoreName,
                ReceiptNo = MatchingResult.ReceiptNo,
                TranAmount = MatchingResult.TranAmount,
                TransDatetime = MatchingResult.TransDatetime,
                RecongnizelId = OCRLog.Id,
                AccountId = AccountInfo.Account.Id,
                AccountName = AccountInfo.Account.AccountName
            };
        }

        /// <summary>
        /// 根据小票与规则找出符合商铺 2，校验广场 确定商铺
        /// </summary>
        /// <param name="WordList">识别原始信息</param>
        /// <param name="OCRVerifyRuleList">所有规则积分</param>
        /// <returns>广场信息</returns>
        private Store FindStoreByWords(List<Word> WordList, List<OCRVerifyRule> OCRVerifyRuleList)
        {
            var StoreRuleList = OCRVerifyRuleList
                .Where(s => s.OCRKeyType == (int)OCRKeyType.StoreName).ToList();
            List<Store> IdentifiedStoreList = new List<Store>(); //识别的商铺集合
            foreach (var Detail in StoreRuleList)
            {
                foreach (var word in WordList)
                {
                    var key = Detail.OCRKey.ToLower();
                    if (word.text.Contains(key))//只需要小票包含关键字
                    {
                        //根据小票关键字找到规则所在的StoreId，加上前端传来的mallid
                        var IdentifiedStore = _storeDAO.Get(Detail.StoreId);
                        if (IdentifiedStore != null)
                            IdentifiedStoreList.Add(IdentifiedStore);
                    }
                }
            }

            Store StoreModel = null;
            if (IdentifiedStoreList != null && IdentifiedStoreList.Count() > 0)
            {
                foreach (var item in IdentifiedStoreList)
                {
                    var AccountNamekey = OCRVerifyRuleList.Where(s => s.StoreId == item.StoreId
                    && s.OCRKeyType == (int)OCRKeyType.MallName).Select(s => s.OCRKey).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(AccountNamekey))
                    {
                        foreach (var word in WordList)
                        {
                            if (word.text.Contains(AccountNamekey.ToLower()))//只需要小票包含关键字
                            {
                                StoreModel = item;
                            }
                        }
                    }
                }
            }

            return StoreModel;
        }

        /// <summary>
        /// 匹配
        /// </summary>
        /// <param name="WordList">识别原始信息</param>
        /// <param name="ReceiptOCRModel"></param>
        /// <param name="thisRule"></param>
        /// <returns></returns>
        private ReceiptOCRResponse Matching(List<Word> WordList, List<OCRVerifyRule> thisRule)
        {
            ReceiptOCRResponse ReceiptOCRModel = new ReceiptOCRResponse();
            for (int i = 0; i < WordList.Count(); i++)
            {
                foreach (var StoreDetailRule in thisRule)
                {
                    Result<string> ReturnResult = GetValue(WordList, i, StoreDetailRule); //根据规则取值
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
                                        ReceiptOCRModel.TransDatetime = DateTimeFormat(ReturnData);
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
            return ReceiptOCRModel;
        }

        /// <summary>
        /// 根据规则获取指定识别内容
        /// </summary>
        /// <param name="words_result">解析原文</param>
        /// <param name="index">文字所在行数</param>
        /// <param name="StoreDetailRule">该商铺规则</param>
        /// <returns></returns>
        private Result<string> GetValue(List<Word> words_result, int index, OCRVerifyRule OCRVerifyRuleModel)
        {
            var WordValue = "";
            var Key = OCRVerifyRuleModel.OCRKey.Trim().Replace("：", "").Replace(":", "").ToLower().Split(',');
            if (Key.Length < 2)
            {
                var newWord = words_result[index].text.Replace("：", "").Replace(":", "");
                if (!newWord.Contains(Key[0]))
                    return FailRes();
            }
            else
            {
                if (!words_result[index].text.Contains(Key[0]) || !words_result[index].text.Contains(Key[1]))
                    return FailRes();
            }

            switch (OCRVerifyRuleModel.GetValueWay)//可查看枚举注释
            {
                case (int)GetValueWay.OCRKey://在关键字中 用下标取值
                    WordValue = GetValueByIndexArr(words_result[index].text, OCRVerifyRuleModel.SkipLines);
                    break;

                case (int)GetValueWay.AfterOCRKey://关键字后 根据下标取值
                    int IndexOfKey = words_result[index].text.IndexOf(OCRVerifyRuleModel.OCRKey) + OCRVerifyRuleModel.OCRKey.Length;
                    string AfterKey = words_result[index].text.Substring(IndexOfKey);//关键字之后的文字
                    WordValue = GetValueByIndexArr(AfterKey, OCRVerifyRuleModel.SkipLines);
                    break;

                case (int)GetValueWay.NextLine://隔行： 隔N行后取整行数据
                    if (int.TryParse(OCRVerifyRuleModel.SkipLines, out var NextLine))
                    {
                        NextLine = index + int.Parse(OCRVerifyRuleModel.SkipLines);
                        if (NextLine <= words_result.Count())
                            WordValue = words_result[NextLine].text;
                    }
                    break;

                case (int)GetValueWay.BeforeOCRKey://关键字前 根据下标取值
                    int IndexOfKey2 = words_result[index].text.IndexOf(OCRVerifyRuleModel.OCRKey);
                    string BeforeKey = words_result[index].text.Substring(0, IndexOfKey2);//关键字之前的文字
                    WordValue = GetValueByIndexArr(BeforeKey, OCRVerifyRuleModel.SkipLines);
                    break;

                case (int)GetValueWay.BetweenOCRKey://关键字之间 并用下标取值
                    var KetArr = OCRVerifyRuleModel.OCRKey.Split(',');
                    int afterkeyIndex = words_result[index].text.IndexOf(KetArr[0]) + KetArr[0].Length;
                    string AfterKeyWord = words_result[index].text.Substring(afterkeyIndex);//关键字之后
                    int beforekeyIndex = AfterKeyWord.IndexOf(KetArr[1]);
                    string BeforeKeyWord = AfterKeyWord.Substring(0, beforekeyIndex);//关键字之前
                    WordValue = GetValueByIndexArr(BeforeKeyWord, OCRVerifyRuleModel.SkipLines);
                    break;
                default:
                    TError($"商铺未设置该关键字取值方法：{OCRVerifyRuleModel.GetValueWay}");
                    break;
            }

            return SuccessRes(WordValue);
        }

        /// <summary>
        /// 时间格式转换
        /// </summary>
        /// <param name="ReturnData"></param>
        /// <returns></returns>
        private DateTime? DateTimeFormat(string ReturnData)
        {
            try
            {

                ReturnData = ReturnData.Replace(" ", "").Replace("'", "");
                var strArr = new string[] { "am", "pm", "上午", "下午" };
                var result = false;
                foreach (var item in strArr)
                {
                    if (ReturnData.Contains(item))
                    {
                        ReturnData = ReturnData.Replace(item, "");
                        result = true;
                        break;
                    }
                }
                if (result)//如果是带英文的
                {
                    var monthStr = new string[] { "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec" };
                    if (ReturnData.Contains("0ct"))
                        monthStr[9] = "0ct";
                    int month = DateTime.Now.Month;
                    int date = DateTime.Now.Day;
                    int year = DateTime.Now.Year;
                    int hour = 0;
                    int minute = 0;
                    for (int i = 0; i < monthStr.Length; i++)
                    {
                        if (ReturnData.Contains(monthStr[i]))
                        {
                            var dateStr = ReturnData.Substring(0, ReturnData.IndexOf(monthStr[i]));
                            month = i + 1;
                            date = int.Parse(dateStr);
                            ReturnData = ReturnData.Replace(dateStr, "").Replace(monthStr[i], "");
                            var ystr = ReturnData.Substring(0, 2);
                            var yearStr = $"20{ystr}";
                            year = int.Parse(yearStr);
                            var timess = ReturnData.Replace(ystr, "");
                            hour = int.Parse(timess.Split(':')[0]);
                            minute = int.Parse(timess.Split(':')[1]);
                            var newDATE = new DateTime(year, month, date, hour, minute, 0);
                            ReturnData = newDATE.ToString("yyyyMMddHHmmss");
                            monthStr[9] = "oct";
                            break;
                        }
                    }
                }
                else
                {
                    ReturnData = Regex.Replace(ReturnData, @"[^0-9]+", "");
                    var head = ReturnData.Substring(0, 2);
                    if (ReturnData.Length == 14)
                    {
                        if (head != "20")
                            ReturnData = ReturnData.Remove(4, 4);
                    }

                    if (ReturnData.Length < 14)
                    {
                        if (head != "20")
                            ReturnData = $"{DateTime.Now.Year}{ReturnData}";

                        ReturnData = ReturnData.PadRight(14, '0');
                    }
                    if (ReturnData.Length > 14) //2019 - 01 - 01 12:30:01 - 13:30:01
                        ReturnData = ReturnData.Substring(0, 8).PadRight(14, '0');
                }
                DateTime dt = DateTime.ParseExact(ReturnData, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                return dt;

            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据取值位置解析字符串
        /// </summary>
        /// <param name="Str">文字</param>
        /// <param name="indexArr">取值位置可以逗号分隔</param>
        /// <returns></returns>
        private string GetValueByIndexArr(string Str, string indexArr)
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
}
