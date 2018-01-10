using System;
using System.Collections.Generic;

using Pdw.WKL.Profiler;

namespace Pdw.WKL.DataController
{
    public class BaseController
    {
        private Dictionary<string, BaseProfile> _datas = new Dictionary<string, BaseProfile>();

        public T GetDataObject<T>(string key) where T : class
        {
            BaseProfile tmp = new BaseProfile();
            if (_datas.TryGetValue(key, out tmp))
            {
                return tmp as T;
            }
            return null;
        }

        public void AddDataObject(string key, BaseProfile obj)
        {
            if (_datas.ContainsKey(key))
                _datas.Remove(key);
            
            _datas.Add(key, obj);
        }

        public string AddDataObject(BaseProfile obj)
        {
            string key = Utilities.GuidHelper.GenKey(_datas);
            if (_datas.ContainsKey(key))
                _datas.Remove(key);
            
            _datas.Add(key, obj);

            return key;
        }

        public bool RemoveDataObject(string key)
        {
            if (_datas.ContainsKey(key))
            {
                _datas.Remove(key);
                return true;
            }    

            return false;
        }

        public bool IsExist(string key)
        {
            return _datas.ContainsKey(key);
        }
    }
}
