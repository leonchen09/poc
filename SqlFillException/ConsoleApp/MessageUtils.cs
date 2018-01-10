using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProntoDoc.Configurer.Utils
{
    public class MessageUtils
    {
        /// <summary>
        /// Template have one replace value0 only 
        /// Template : 
        /// [XXX] <{0}> [XXX]
        /// </summary>
        /// <param name="Template">Template message</param>
        /// <param name="value0">Value for replace</param>
        /// <returns>Message content</returns>
        /// <exception cref="">Null Exception, Format Exception</exception>
        public static string Expand(string Template, string value)
        {
            // Build Message content
            return string.Format(Template, value);
        }

        /// <summary>
        /// Build message content with multi value0
        /// </summary>
        /// <param name="Template">Template</param>
        /// <param name="values">List value0</param>
        /// <returns>Message content</returns>
        /// <remarks>If number of values least than number prameter then method throw exception</remarks>
        public static string Expand(string Template, string[] values)
        {
            // Build Message content
            return string.Format(Template, values);
        }

        /// <summary>
        /// Combine two template ( Destination/Source ) for building message content
        /// Template Des/Src : 
        /// [XXX] <{0}> [XXX]
        /// </summary>
        /// <param name="TemplateDes">Template Destination</param>
        /// <param name="TemplateSrc">Template Source</param>
        /// <param name="value0">Value for replace</param>
        /// <returns>Message content</returns>
        /// <exception cref="">Null Exception, Format Exception</exception>
        public static string Expand(string TemplateDes, string TemplateSrc, string value)
        {
            // Build new template
            string newTemplateMsg = Expand(TemplateDes, TemplateSrc);

            // Build Message content
            return Expand(newTemplateMsg, value);
        }

        /// <summary>
        /// Template have one parameter, generate message with plural
        /// [XXX] <{0}> [XXX]
        /// <{0}>/s : The index of string value0
        /// /s appear depend on the input number, have /s if number greater than 1 otherwise disappear
        /// </summary>
        /// <param name="TemplateRoot">Template</param>        
        /// <param name="number">Number value0 for replace</param>
        /// <param name="value">Template for build plural value0</param>
        /// <param name="pluralCharacter">Plural character</param>
        /// <returns>Message content</returns>
        /// <example> "1 DD loaded" or "Scope in 5 DDs changed" </example>
        public static string ExpandS(string TemplateRoot, int number, string value, string pluralCharacter)
        {
            // Generate plural character if need
            string sNumber = number.ToString();
            if (number <= 1)
            {
                pluralCharacter = string.Empty;
                if (number < 1)
                {
                    sNumber = SharedComponent.CommonResource.ipm_HaveNothing;
                }
            }

            // Build new content with plural
            value = sNumber + ProntoDoc.Configurer.SharedComponent.Constants.Generator.Space + value + pluralCharacter;
            
            // Build message content
            return string.Format(TemplateRoot, value);
        }

        /// <summary>
        /// Build message content with template as 
        /// [XXX]<{0}>[XXX]<{1}>[XXX]
        /// </summary>
        /// <param name="TemplateRoot">Template</param>
        /// <param name="number1">The number at index 0</param>
        /// <param name="value1">The value0 at index 0</param>
        /// <param name="number2">The number at index 1</param>
        /// <param name="value2">The value0 at index 1</param>
        /// <param name="pluralCharacter">plural character</param>
        /// <returns>Message content</returns>
        public static string ExpandS(string TemplateDes, string TemplateSrc, string value0, int number1, string value1, int number2, string value2, string pluralCharacter)
        { 
            // Generate plural character
            string plural1 = string.Empty;
            string plural2 = string.Empty;
            if (number1 > 1)
            {
                plural1 = pluralCharacter;
            }

            if (number2 > 1)
            {
                plural2 = pluralCharacter;
            }

            // Generate value
            value1 = number1.ToString() + ProntoDoc.Configurer.SharedComponent.Constants.Generator.Space + value1 + plural1;
            value2 = number2.ToString() + ProntoDoc.Configurer.SharedComponent.Constants.Generator.Space + value2 + plural2;

            // Build new Template
            TemplateDes = string.Format(TemplateDes, TemplateSrc);

            // Build message content
            return string.Format(TemplateDes, value0, value1, value2);
        }
    }
}
