using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.InputOutputFormatter
{
    public class _CsvOutputFormatter : TextOutputFormatter
    {
        public _CsvOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        protected override bool CanWriteType(Type type)
        {
            if (
                typeof(CompanyDto).IsAssignableFrom(type) ||
                typeof(IEnumerable<CompanyDto>).IsAssignableFrom(type)
                )
                return base.CanWriteType(type);

            return false;
        }
        private static void FormatCsv(StringBuilder buffer, CompanyDto company)
        {
            buffer.AppendLine($"{company.Id},\"{company.Name}\",\"{company.FullAddress}\"");
        }
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var response = context.HttpContext.Response;
            var buffer = new StringBuilder();

            if (context.Object is IEnumerable<CompanyDto> lsCompanyDto)
            {
                foreach (var companyDto in lsCompanyDto)
                {
                    FormatCsv(buffer, companyDto);
                }
            }
            if (context.Object is CompanyDto CompanyDto)
            {
                FormatCsv(buffer, CompanyDto);
            }
           await HttpResponseWritingExtensions.WriteAsync(response, buffer.ToString());
        }
    }
}
