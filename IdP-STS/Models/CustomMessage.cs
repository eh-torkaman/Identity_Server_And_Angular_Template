using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdP.Models
{
    public class CustomMessage
    {
        public string Message = "";
        public string Name = "";
        public string Stack = "";
        public bool IsError = false;
        public bool ShouldLog = false;


        public CustomMessage(bool isError=false,String message ="",string name="",string stack="" ,bool shouldLog=fale)
        {
            this.Message = message;
            this.Name = name;
            this.Stack = stack;
            this.IsError = isError; 
            this.ShouldLog = shouldLog;
        }

        public CustomMessage(Exception ee)
        {
            this.Message = ee.Message;
            this.Name = ee.Source;
            this.IsError = true;
        }

    }

    public class CustomMessages:List<CustomMessage>
    {
        public CustomMessages()
        {

        }
        public CustomMessages(CustomMessage msg)
        {
            this.Add(msg);
        }
        public CustomMessages(CustomMessages msgs)
        {
            this.AddRange(msgs);
        }
    }
}
