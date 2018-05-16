using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SkillsBase.WEB.Utils
{
    public class JsonCustomResult: ActionResult
    {
        public Encoding ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public object Data { get; set; }

        public JsonSerializerSettings SerializerSettings { get; set; }
        public Formatting Formatting { get; set; }

        public JsonCustomResult(object data, Formatting formatting)
            : this(data)
        {
            Formatting = formatting;
        }

        public JsonCustomResult(object data) : this()
        {
            Data = data;
        }

        public JsonCustomResult()
        {
            Formatting = Formatting.None;
            SerializerSettings = new JsonSerializerSettings();
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            var response = context.HttpContext.Response;
            response.ContentType = !string.IsNullOrEmpty(ContentType)
              ? ContentType
              : "application/json";
            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (Data == null) return;

            var writer = new JsonTextWriter(response.Output) { Formatting = Formatting };
            var serializer = JsonSerializer.Create(SerializerSettings);
            serializer.Serialize(writer, Data);
            writer.Flush();
        }
    }
}