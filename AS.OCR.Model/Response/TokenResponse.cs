﻿using System;

namespace AS.OCR.Model.Response
{
    public class TokenResponse
    {
        public string token { get; set; }
        public DateTime expiryTime { get; set; }
    }
}
