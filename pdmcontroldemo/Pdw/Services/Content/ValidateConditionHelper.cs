
using System.Collections.Generic;

using ProntoDoc.Framework.CoreObject;
using ProntoDoc.Framework.Expression;
using ProntoDoc.Framework.CoreObject.DataSegment;

using Pdw.Core;

namespace Pdw.Services.Content
{
    public class ValidateConditionHelper
    {
        private string _expData;
        private string _messageContent = string.Empty;
        private Dictionary<string, USCItem> _listFields;

        public List<USCItem> FieldsInExpression { get; set; }

        public string MessageContent
        {
            get { return _messageContent; }
            set { _messageContent = value; }
        }

        public ValidateConditionHelper(string expData, Dictionary<string, USCItem> listFields)
        {
            _expData = expData;
            _listFields = listFields;
            FieldsInExpression = new List<USCItem>();
        }

        public ValidateConditionHelper(string expData, Dictionary<string, USCItem> listFields, ref List<USCItem> fields)
        {
            _expData = expData;
            _listFields = listFields;
            FieldsInExpression = fields;
        }

        public bool IsValid()
        {
            if (null == _expData)
            {
                _messageContent = MessageUtils.Expand(Properties.Resources.ipm_M001, Properties.Resources.ipe_M007);
                return false;
            }

            Stack<Token> suffixExpressionStack = new Stack<Token>();
            ExpressionProvider expProvider = new ExpressionProvider();
            ReturnCode retCode = expProvider.Parser(_expData, ref suffixExpressionStack, ref _messageContent);
            if (ReturnCode.OK != retCode)
                return false;

            // Check existing of operand
            List<SqlVariableName> lstSqlFieldsAndParams = new List<SqlVariableName>();
            lstSqlFieldsAndParams = expProvider.GetListFields();

            // Set type for operand before validate
            SetDataTypeForVarialbe(ref suffixExpressionStack);

            if ((!CheckExistsField(lstSqlFieldsAndParams)))
                return false;

            // Validate expression
            retCode = expProvider.Validate(suffixExpressionStack, ref _messageContent);
            if (ReturnCode.OK != retCode)
                return false;

            // Expression must condition expression
            if ( SQLTypeName.BOOLEAN != expProvider.ReturnType.Name)
            {
                _messageContent = MessageUtils.Expand(Properties.Resources.ipm_M001, Properties.Resources.ipe_M008);
                return false;
            }

            return true;
        }

        private bool CheckExistsField(List<SqlVariableName> listExpressFields)
        {
            if (listExpressFields == null || listExpressFields.Count <= 0)
            {
                _messageContent = MessageUtils.Expand(Properties.Resources.ipm_M001, Properties.Resources.ipe_M009);
                return true;
            }

            foreach (SqlVariableName varName in listExpressFields)
            {
                if (!IsExistNameInDomain(_listFields, varName))
                {
                    _messageContent = MessageUtils.Expand(Properties.Resources.ipm_M001, Properties.Resources.ipe_M010, varName.Name);
                    if(FieldsInExpression != null)
                        FieldsInExpression = null;
                    return false;
                }
                else
                    FieldsInExpression.Add(_listFields[TrimBracesSymbol(varName.Name).ToLower()]);
            }

            return true;
        }

        private string TrimBracesSymbol(string src)
        {
            if (string.IsNullOrEmpty(src))
            {
                return string.Empty;
            }
            if ('[' == src[0])
            {
                src = src.Remove(0, 1);
                if (src.Length > 0)
                {
                    src = src.Remove(src.Length - 1, 1);
                }
            }
            
            src = src.Replace("]]", "]");
            return src.ToLower();
        }

        private bool IsExistNameInDomain(Dictionary<string, USCItem> lstFieldInfo, SqlVariableName varName)
        {
            string displayVarName = TrimBracesSymbol(varName.Name);
            if ((string.IsNullOrEmpty(displayVarName)) ||
                (!lstFieldInfo.ContainsKey(displayVarName.ToLower())))
            {
                return false;
            }
            return true;
        }

        private void SetDataTypeForVarialbe(ref Stack<Token> suffixExpressionStack)
        {
            if ((null == _expData) ||
                (null == _listFields) ||
                (null == suffixExpressionStack))
            {
                return;
            }

            Token token = null;
            string fieldName = string.Empty;
            Stack<Token> resultStack = new Stack<Token>();
            while (0 < suffixExpressionStack.Count)
            {
                token = suffixExpressionStack.Pop();
                if (token is OperandInfo)
                {
                    fieldName = TrimBracesSymbol(token.Name);
                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        if (_listFields.ContainsKey(fieldName.ToLower()))
                        {
                            token = SetDataTypeForField((OperandInfo)token, _listFields[fieldName.ToLower()]);
                        }
                        //else if ('@' == token.Name[0])
                        //{
                        //    if (0 < resultStack.Count)
                        //    {
                        //        Token previousToken = resultStack.Peek();
                        //        if (IsSqlArrayOperator(previousToken))
                        //        {
                        //            ((OperandInfo)token).Type = SQLTypeName.ARRAY;
                        //        }
                        //    }
                        //}
                    }
                }

                resultStack.Push(token);
            }
            while (0 < resultStack.Count)
            {
                suffixExpressionStack.Push(resultStack.Pop());
            }
        }

        private Token SetDataTypeForField(OperandInfo token, USCItem variableInfo)
        {
            if ((null == token) ||
                (null == variableInfo))
            {
                return token;
            }
            if (variableInfo.Type == DSIconType.Field)
            {
                token.Type = variableInfo.DataType;
            }
            return token;
        }

        private SQLDBType GetSqlTypeFromString(string dataType)
        {
            SQLDBType dbType = new SQLDBType();
            if (string.IsNullOrEmpty(dataType))
            {
                dbType.Name = SQLTypeName.UNKNOW_TYPE;
                return dbType;
            }
            dataType = dataType.ToUpper();
            if (dataType.Equals(SQLTypeName.BIGINT.ToString()))
            {
                dbType.Name = SQLTypeName.BIGINT;
               
            }
            else if (dataType.Equals(SQLTypeName.BINARY.ToString()))
            {
                dbType.Name = SQLTypeName.BINARY;
            }
            else if (dataType.Equals(SQLTypeName.BIT.ToString()))
            {
                dbType.Name = SQLTypeName.BIT;
            }
            else if (dataType.Equals(SQLTypeName.CHAR.ToString()))
            {
                dbType.Name = SQLTypeName.CHAR;
            }
            else if (dataType.Equals(SQLTypeName.DATETIME.ToString()))
            {
                dbType.Name = SQLTypeName.DATETIME;
            }
            else if (dataType.Equals(SQLTypeName.DECIMAL.ToString()))
            {
                dbType.Name = SQLTypeName.DECIMAL;
            }
            else if (dataType.Equals(SQLTypeName.FLOAT.ToString()))
            {
                dbType.Name = SQLTypeName.FLOAT;
            }
            else if (dataType.Equals(SQLTypeName.IMAGE.ToString()))
            {
                dbType.Name = SQLTypeName.IMAGE;
            }
            else if (dataType.Equals(SQLTypeName.INT.ToString()))
            {
                dbType.Name = SQLTypeName.INT;
            }
            else if (dataType.Equals(SQLTypeName.MONEY.ToString()))
            {
                dbType.Name = SQLTypeName.MONEY;
            }
            else if (dataType.Equals(SQLTypeName.NCHAR.ToString()))
            {
                dbType.Name = SQLTypeName.NCHAR;
            }
            else if (dataType.Equals(SQLTypeName.NTEXT.ToString()))
            {
                dbType.Name = SQLTypeName.NTEXT;
            }
            else if (dataType.Equals(SQLTypeName.NUMERIC.ToString()))
            {
                dbType.Name = SQLTypeName.NUMERIC;
            }
            else if (dataType.Equals(SQLTypeName.NVARCHAR.ToString()))
            {
                dbType.Name = SQLTypeName.NVARCHAR;
            }
            else if (dataType.Equals(SQLTypeName.REAL.ToString()))
            {
                dbType.Name = SQLTypeName.REAL;
            }
            else if (dataType.Equals(SQLTypeName.SMALLMONEY.ToString()))
            {
                dbType.Name = SQLTypeName.SMALLMONEY;
            }
            else if (dataType.Equals(SQLTypeName.SMALLDATETIME.ToString()))
            {
                dbType.Name = SQLTypeName.SMALLDATETIME;
            }
            else if (dataType.Equals(SQLTypeName.SMALLINT.ToString()))
            {
                dbType.Name = SQLTypeName.SMALLINT;
            }
            else if (dataType.Equals(SQLTypeName.SQL_VARIANT.ToString()))
            {
                dbType.Name = SQLTypeName.SQL_VARIANT;
            }
            else if (dataType.Equals(SQLTypeName.TEXT.ToString()))
            {
                dbType.Name = SQLTypeName.TEXT;
            }
            else if (dataType.Equals(SQLTypeName.TIMESTAMP.ToString()))
            {
                dbType.Name = SQLTypeName.TIMESTAMP;
            }
            else if (dataType.Equals(SQLTypeName.TINYINT.ToString()))
            {
                dbType.Name = SQLTypeName.TINYINT;
            }
            else if (dataType.Equals(SQLTypeName.UNIQUEIDENTIFIER.ToString()))
            {
                dbType.Name = SQLTypeName.UNIQUEIDENTIFIER;
            }
            else if (dataType.Equals(SQLTypeName.VARBINARY.ToString()))
            {
                dbType.Name = SQLTypeName.VARBINARY;
            }
            else if (dataType.Equals(SQLTypeName.VARCHAR.ToString()))
            {
                dbType.Name = SQLTypeName.VARCHAR;
            }
            else if (dataType.Equals(SQLTypeName.XML.ToString()))
            {
                dbType.Name = SQLTypeName.XML;
            }
            else
            {
                dbType.Name = SQLTypeName.UNKNOW_TYPE;
            }

            return dbType;
        }
    }    
}
