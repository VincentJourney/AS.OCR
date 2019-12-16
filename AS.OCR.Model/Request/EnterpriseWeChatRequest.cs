using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AS.OCR.Model.Request
{
    public class SendMessageRequest
    {
        /// <summary>
        /// 成员ID列表（消息接收者，多个接收者用‘|’分隔，最多支持1000个）。
        /// 特殊情况：指定为@all，则向该企业应用的全部成员发送
        /// </summary>
        public string touser { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public string msgtype { get; set; } = "text";
        /// <summary>
        /// 应用Id
        /// </summary>
        public int agentid { get; set; }
        /// <summary>
        /// 消息体
        /// </summary>
        public Content text { get; set; }
    }

    public class Content
    {
        public string content { get; set; }
    }
}
