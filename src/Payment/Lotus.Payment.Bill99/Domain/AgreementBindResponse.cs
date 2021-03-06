﻿using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    public class AgreementBindResponse : MasMessage
    {
        [XElement("ErrorMsgContent")]
        public ErrorMsgContent ErrorMsgContent { get; set; }

        [XElement("indAuthDynVerifyContent")]
        public IndAuthDynVerifyResponseContent IndAuthDynVerifyContent { get; set; }
    }
}
