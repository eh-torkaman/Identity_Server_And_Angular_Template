using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace IdP.Models
{
    public enum MsgTypeEnum
    {
        Info='i',
        Error='e',
        Success='s',
        Warning='w',
        Simple=' '
    }
    public class CustomMessage
    {
        [JsonIgnore]
        public MsgTypeEnum MsgTypeEnum
        {
            set
            {
                switch (value)
                {
                    case MsgTypeEnum.Info:
                        tileTemp = "پیام";
                        MsgType = "Info";
                        break;
                    case MsgTypeEnum.Error:
                        tileTemp = "خطا";
                        MsgType = "Error";
                        break; ;
                    case MsgTypeEnum.Warning:
                        tileTemp = "هشدار";
                        MsgType = "Warning";
                        break; ;
                    case MsgTypeEnum.Simple:
                        tileTemp = "پیام";
                        MsgType = "Simple";
                        break; ;
                    default:
                        tileTemp = "پیام موفقیت";
                        MsgType = "Success";
                        break; ;
                }
               if ( this.Title == "" )
                    this.Title= this.tileTemp;
            }
        }
        private string tileTemp = "";
        private PersianCalendar pc = new PersianCalendar();

        public string Message = "";
        public string Title = "";
        public string MsgType {  get; private set; }
        public string Name = "";
        public string Stack = "";


        public DateTime GeneraterdTime = DateTime.Now;
        public string GeneraterdTimePersianStr
        {
            get
            {
                return "" + pc.GetYear (GeneraterdTime) + "/" + pc.GetMonth(GeneraterdTime) + "/" + pc.GetDayOfMonth(GeneraterdTime) + " "
                    + pc.GetHour(GeneraterdTime) + ":" + pc.GetMinute(GeneraterdTime);
            }
        }
        public bool ShouldLog = false;

        public CustomMessage()
        {

        }
        public CustomMessage(MsgTypeEnum msgType = MsgTypeEnum.Info,String message ="",string name="",string title="",string stack="" ,bool shouldLog=false)
        {
           this.MsgTypeEnum = msgType;
            this.Title = title!=""?title: this.tileTemp;
            this.Message = message;
            this.Name = name;
            this.Stack = stack;
            this.ShouldLog = shouldLog;
        }

        public CustomMessage(Exception ee)
        {
            this.Title = "خطا";
            this.Message = ee.Message;
            this.Name = ee.Source;
            this.MsgTypeEnum = MsgTypeEnum.Error;
        }

    }

    public class ListOfCustomMessages:List<CustomMessage>
    {
        public ListOfCustomMessages()
        {

        }
        public ListOfCustomMessages(string msg, MsgTypeEnum msgTypeEnum= MsgTypeEnum.Info)
        {
            this.Add(new CustomMessage() {Message=msg , MsgTypeEnum= msgTypeEnum });
        }
        public ListOfCustomMessages(Exception ee)
        {
            this.Add(new CustomMessage(ee) );
        }

        public ListOfCustomMessages(CustomMessage msg)
        {
            this.Add(msg);
        }
        public ListOfCustomMessages(ListOfCustomMessages msgs)
        {
            this.AddRange(msgs);
        }
    }
}
