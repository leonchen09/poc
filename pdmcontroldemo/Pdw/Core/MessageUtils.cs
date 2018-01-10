
namespace Pdw.Core
{
    public class MessageUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="template">Adding failed. Error: {0}</param>
        /// <param name="value">object reference not set</param>
        /// <returns>Adding failed. Error: object reference not set</returns>
        public static string Expand(string template, object value)
        {
            return string.Format(template, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootTemplate">Adding failed. Error: {0}</param>
        /// <param name="template1">{0} is not existed</param>
        /// <param name="value1">FieldName</param>
        /// <returns>Adding failed. Error: FieldName is not existed</returns>
        public static string Expand(string rootTemplate, string template1, object value1)
        {
            string message = Expand(rootTemplate, template1);
            message = Expand(message, value1);

            return message;
        }
    }
}
