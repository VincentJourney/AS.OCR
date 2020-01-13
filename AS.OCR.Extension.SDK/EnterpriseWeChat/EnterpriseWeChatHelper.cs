using AS.OCR.Commom.Configuration;
using System;
using AS.OCR.Model.Response;
using Newtonsoft.Json;
using AS.OCR.Model.Request;
using AS.OCR.Commom.Http;
using AS.OCR.Model.Business;

namespace AS.OCR.Extension.SDK.EnterpriseWeChat
{
    public class EnterpriseWeChatHelper
    {
        private string corpid;
        private string corpsecret;
        private string token;
        public EnterpriseWeChatHelper(string CorpId, string CorpSecret)
        {
            corpid = CorpId;
            corpsecret = CorpSecret;
            if (!PooledRedisClientHelper.ContainsKey("WeChat_Token"))
            {
                var result = GetToken(corpid, corpsecret);
                if (result == null)
                    throw new Exception("请求企业微信token失败");
                if (result.errcode != 0)
                    throw new Exception(result.errmsg);
                if (string.IsNullOrWhiteSpace(result.access_token))
                    throw new Exception("请求企业微信token失败");

                token = result.access_token;
                PooledRedisClientHelper.Set<string>("WeChat_Token", result.access_token, TimeSpan.FromSeconds(result.expires_in));
            }
            else
                token = PooledRedisClientHelper.GetValueString("WeChat_Token");

        }
        /// <summary>
        /// 获取企业微信Token
        /// </summary>
        /// <param name="corpid"></param>
        /// <param name="corpsecret"></param>
        /// <returns></returns>
        public static GetTokenResponse GetToken(string corpid, string corpsecret) =>
            JsonConvert.DeserializeObject<GetTokenResponse>(
                HttpHelper.HttpGet($" https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={corpid}&corpsecret={corpsecret}"));


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public SendMessageResponse Send(SendMessageRequest req) =>
            JsonConvert.DeserializeObject<SendMessageResponse>(
                HttpHelper.HttpPost(
                    $"https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={token}",
                    JsonConvert.SerializeObject(req)));


        public SendMessageResponse Send(string Content) =>
            Send(new SendMessageRequest
            {
                touser = ConfigurationUtil.EnterpriseWeChat_Touser,
                agentid = ConfigurationUtil.EnterpriseWeChat_AgentId,
                text = new Content { content = Content }
            });


        public void OCRFilter()
        {

            var Total = AccountInfo.Account.Total;//购买总数
            var Warning = AccountInfo.Account.Warning;//预警次数
            var RedisKey = AccountInfo.Account.RedisKey;
            var HashId = AccountInfo.Account.HashId;
            var WarningHz = AccountInfo.Account.WarningHz;//预警频率
            PooledRedisClientHelper.SetEntryInHashIfNotExists(HashId, RedisKey, "0");
            var OCRUseCount = int.Parse(PooledRedisClientHelper.GetHashField(HashId, RedisKey));//已用次数
            var Balance = Total - OCRUseCount;//剩余次数
            if (Balance <= 0)
            {
                Send($@"【余额不足】
 购买的OCR次数不足。
【已用次数】：{OCRUseCount}
【可用次数】：{Balance}
【总购买数】：{Total}");
                throw new Exception("余额不足");
            }

            if (Balance > 0 && Balance <= Warning && Balance % WarningHz == 0)//可用，次数小于预警，1000次提醒一次
            {
                Send($@"【警告】购买的OCR次数预警。
【已用次数】：{OCRUseCount}
【可用次数】：{Balance}
【总购买数】：{Total}");
            }
        }


        public void SetHashIncr(int value)
        {
            var HashId = AccountInfo.Account.HashId;
            var RedisKey = AccountInfo.Account.RedisKey;
            try
            {
                PooledRedisClientHelper.SetEntryInHashIfNotExists(HashId, RedisKey, "0");
                PooledRedisClientHelper.SetHashIncr(HashId, RedisKey, value);
            }
            catch (Exception ex)
            {
                Send($@"【操作Redis失败】 ：
【HashId】：【 {HashId} 】
【redisKey】：【 {RedisKey} 】
【异常消息】：【 {ex.Message} 】
【堆栈信息】：【 {ex.StackTrace} 】");

                throw new Exception("操作Redis失败");
            }
        }

    }
}
