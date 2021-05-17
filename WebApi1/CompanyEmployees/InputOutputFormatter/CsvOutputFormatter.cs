using CsvHelper;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.InputOutputFormatter
{
    public class CsvOutputFormatter : TextOutputFormatter
    {
        public CsvOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        protected override bool CanWriteType(Type type)
        {
            return base.CanWriteType(type);
            if (
                typeof(CompanyDto).IsAssignableFrom(type) ||
                typeof(IEnumerable<CompanyDto>).IsAssignableFrom(type)
                )
                return base.CanWriteType(type);

            return false;
        }
        
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            using (var writer = new StringWriter())
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    if (context.Object is IEnumerable<object>)
                    { csv.WriteRecords((IEnumerable<object>)context.Object); }
                    else
                    {
                        var ls = new [] { context.Object };
                        csv.WriteRecords((IEnumerable<object>)ls);
                    }
                }
                await HttpResponseWritingExtensions.WriteAsync(context.HttpContext.Response, writer.ToString());
            }
        }
    }
}
