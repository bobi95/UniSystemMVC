using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UniversitySystemMVC.Extensions
{
    public enum FlashMessageTypeEnum
    {
        Red, Blue, Yellow, Green
    }

    public class FlashMessage
    {
        public string Message { get; set; }

        public string Title { get; set; }

        public FlashMessageTypeEnum Type { get; set; }
    }

    public static class FlashMessageExtensions
    {
        public static void FlashMessage(this TempDataDictionary data, string message, string title = null, FlashMessageTypeEnum type = FlashMessageTypeEnum.Green)
        {
            data.Add("FlashMessage", new FlashMessage
            {
                Message = message,
                Title = title,
                Type = type
            });
        }

        public static bool FlashExists(this TempDataDictionary data)
        {
            return data.ContainsKey("FlashMessage");
        }

        public static string GetFlashMessage(this TempDataDictionary data)
        {
            return ((FlashMessage)data["FlashMessage"]).Message;
        }

        public static string GetFlashTitle(this TempDataDictionary data)
        {
            return ((FlashMessage)data["FlashMessage"]).Title;
        }

        public static FlashMessageTypeEnum GetFlashType(this TempDataDictionary data)
        {
            return ((FlashMessage)data["FlashMessage"]).Type;
        }

        public static void ResetFlash(this TempDataDictionary data)
        {
            data.Remove("FlashMessage");
        }
    }
}