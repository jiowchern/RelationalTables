﻿using Regulus.RelationalTables.Raw;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Regulus.RelationalTables
{
    
    public class FieldValue
    {
        private readonly FieldInfo _Field;
        private readonly IRowQueryable _Row;
        private readonly ITableFindable _Finder;

        public readonly object Instance;
        public FieldValue(FieldInfo field, IRowQueryable row, ITableFindable findable)
        {

            this._Field = field;
            this._Row = row;
            this._Finder = findable;
            Instance = _Create();
        }

        private object _Create()
        {
            object instance;
            if(_TryCreateArray(out instance))
            {
                return instance;
            }
            return _CreateNormal();
        }

        private bool _TryCreateArray(out object instance)
        {
            instance = null;
            var array = _Field.GetCustomAttributes<Regulus.RelationalTables.Array>().FirstOrDefault() ;
            if (array == null)
                return false;

            instance = array.Create(_Field,_Row);
            return true;
        }

        private object _CreateNormal()
        {
            var fieldName = _Field.Name;
            var values = from col in _Row.GetColumns() where col.Name == fieldName select col.Value;
            return Regulus.Utility.ValueHelper.StringConvert(_Field.FieldType, values.Single());            
            
        }
    }
}