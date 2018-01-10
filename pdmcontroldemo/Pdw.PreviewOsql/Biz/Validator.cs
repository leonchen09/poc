using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pdw.PreviewOsql.Entity;
using ProntoDoc.Framework.CoreObject;
using ProntoDoc.Framework.CoreObject.PdwxObjects;
using ProntoDoc.Framework.CoreObject.Render;
using ProntoDoc.Framework.Render;
using ProntoDoc.Framework.CoreObject.Render.Schema;
using ProntoDoc.Framework.CoreObject.Render.Value;

namespace Pdw.PreviewOsql.Biz
{
    public class Validator
    {
        #region Validate RenderArgumentValue
        public bool ValidateRenderArgumentDomainValue(ChecksumInfoItem csItem, RenderArgDomainValue domainRenderArgValue, ref string msg)
        {
            bool isValid = true;

            if (domainRenderArgValue == null)
            {
                isValid = false;
                msg += "RenderArgumentDomainValue is null or empty !!!";
            }

            //Validate RenderArgument value before render document
            isValid &= ValidateRenderArgValue(csItem.RenderArgument, domainRenderArgValue, ref msg);

            //Validate RenderArgumentX value, RenderArgumentY value before Render Document
            isValid &= ValidateRenderArgumentXYValueBeforeRender(csItem.RenderArgument, domainRenderArgValue, ref msg);

            return isValid;
        }

        public static bool ValidateRenderArgValue(RenderArgDomainSchema renderArg, RenderArgDomainValue argValues, ref string msg)
        {
            //Return if template doesn't need argument value for rendering.
            if (renderArg == null || renderArg.Parameters == null || renderArg.Parameters.Count == 0)
                return true;

            if (argValues == null || argValues.Parameters == null || argValues.Parameters.Count == 0)
            {
                msg += "RenderArgument is null or empty.";
                return false;
            }

            if (renderArg.Parameters.Count > argValues.Parameters.Count)
            {
                msg += string.Format("Argument missing. This template needs value for {0} parameters.", renderArg.Parameters.Count);
                return false;
            }

            Dictionary<long, string> errors = new Dictionary<long, string>();
            bool isValid = ValidateRenderArgType(renderArg, ref msg);
            if (!isValid)
                return false;

            for (int i = 0; i < renderArg.Parameters.Count; i++)
            {
                RenderParameterSchema paramInfo = renderArg.Parameters[i];
                //Get corresponding param value base on Name 
                RenderParameterBase paramValueInfo = (from c in argValues.Parameters
                                                  where string.Compare(c.Name, paramInfo.Name, true) == 0
                                                  select c).FirstOrDefault();
                //If needed parameter isn't found.
                if (paramValueInfo == null || paramValueInfo.Value == null || paramValueInfo.Value.Count() == 0)
                {
                    isValid = false;
                    msg += string.Format("Parameter value is not found: {0}.", paramInfo.Name);
                    continue;
                }

                object paramValue = paramInfo.DataType.IsArray ? paramValueInfo.Value : paramValueInfo.Value[0];

                //Validate value and length
                SQLTypeName dataType = SQLConvertTypeHelper.GetSqlTypeFromString(paramInfo.DataType.Name);
                DataTypeLength length = CoreRenderHelper.GetDataTypeLengthFromSXLength(paramInfo.DataType.Length);
                if (!SQLDBTypeValidator.ValidateValue(dataType, length, paramValue, paramInfo.DataType.IsArray))
                {
                    isValid = false;
                    msg += string.Format("Type of parameter value is invalid: {0}.", paramInfo.Name);
                }
            }

            return isValid;
        }

        public static bool ValidateRenderArgType(RenderArgDomainSchema renderArgument, ref string msg)
        {
            if (renderArgument == null || renderArgument.Parameters == null || renderArgument.Parameters.Count == 0)
                return true;

            bool isValid = true;
            foreach (var arg in renderArgument.Parameters)
            {
                //Validate Type
                SQLTypeName dataType = SQLConvertTypeHelper.GetSqlTypeFromString(arg.DataType.Name);
                isValid &= IsSupportedDataType(dataType, ref msg);
            }

            return isValid;
        }

        public static bool ValidateRenderArgumentXYValueBeforeRender(RenderArgDomainSchema renderArg, RenderArgDomainValue domainValue, ref string msg)
        {
            bool isValid = true;

            if (renderArg.RenderArgumentX)
            {
                if (domainValue.RenderArgumentX == null)
                {
                    msg += string.Format("Parameter value is not found: RenderArgumentX.");
                    isValid = false;
                }
            }

            if (renderArg.RenderArgumentY)
            {
                if (domainValue.RenderArgumentY == null)
                {
                    msg += string.Format("Parameter value is not found: {0}.", "RenderArgumentY");
                    isValid = false;
                }
            }

            return isValid;
        }

        #endregion

        /// <summary>
        /// Check type is supported in RenderArgument by SX or not.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>string.Empty if type is supported. Otherwise, return error message.</returns>
        public static bool IsSupportedDataType(SQLTypeName type, ref string msg)
        {
            switch (type)
            {
                case SQLTypeName.BIGINT:
                case SQLTypeName.INT:
                case SQLTypeName.SMALLINT:
                case SQLTypeName.TINYINT:
                case SQLTypeName.FLOAT:
                case SQLTypeName.DECIMAL:
                case SQLTypeName.NUMERIC:
                case SQLTypeName.MONEY:
                case SQLTypeName.SMALLMONEY:

                case SQLTypeName.UNIQUEIDENTIFIER:
                case SQLTypeName.BIT:
                case SQLTypeName.CHAR:
                case SQLTypeName.NCHAR:
                case SQLTypeName.VARCHAR:
                case SQLTypeName.NVARCHAR:

                case SQLTypeName.BINARY:

                //case SQLTypeName.TIMESTAMP:

                case SQLTypeName.DATETIME:
                case SQLTypeName.SMALLDATETIME:

                case SQLTypeName.TEXT:
                case SQLTypeName.NTEXT:
                    return true;

                default:
                    msg += string.Format("Argument with Type = {0} is not supported by SX.", type);
                    return false;
            }
        }

        #region Validate DBID
        public static bool ValidateDBID(List<ChecksumInfoItem> lstCheckSum, List<string> listAppDB, ref string msg)
        {
            foreach (var item in lstCheckSum)
            {
                if (!listAppDB.Contains(item.DBID.ToUpper()))
                {
                    msg += "DBID : " + item.DBID + " must have in input info !!!";
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
