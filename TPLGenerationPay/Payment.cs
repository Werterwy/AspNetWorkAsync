using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPLGenerationPayments
{
    public class Payment
    {

        public Payment() { }

        public int PaymentID { get; set; }
        public int SumPayment { get; set; }
        public int TimeProcessing { get; set; }

    }
}
