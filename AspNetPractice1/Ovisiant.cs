using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetPractice1
{
    public class Ovisiant
    {
        public Ovisiant() { }

        public Queue<string> queueOrder {  get; set; } = new Queue<string>();

        public bool isAcceptOrder { get; set; }

    }
}
