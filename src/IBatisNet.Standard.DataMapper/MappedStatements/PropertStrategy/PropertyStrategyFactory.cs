#region Apache Notice

/*****************************************************************************
 * $Revision: 374175 $
 * $LastChangedDate: 2006-04-25 19:40:27 +0200 (mar., 25 avr. 2006) $
 * $LastChangedBy: gbayon $
 * 
 * iBATIS.NET Data Mapper
 * Copyright (C) 2006/2005 - The Apache Software Foundation
 *  
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 ********************************************************************************/

#endregion

using System.Collections;
using System.Collections.Generic;
using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.MappedStatements.PropertStrategy;

namespace IBatisNet.DataMapper.MappedStatements.PropertyStrategy
{
    /// <summary>
    ///     Factory to get <see cref="IPropertyStrategy" /> implementation.
    /// </summary>
    public sealed class PropertyStrategyFactory
    {
        private static readonly IPropertyStrategy _defaultStrategy;
        private static readonly IPropertyStrategy _resultMapStrategy;
        private static readonly IPropertyStrategy _groupByStrategy;

        private static readonly IPropertyStrategy _selectArrayStrategy;
        private static readonly IPropertyStrategy _selectGenericListStrategy;
        private static readonly IPropertyStrategy _selectListStrategy;
        private static readonly IPropertyStrategy _selectObjectStrategy;

        /// <summary>
        ///     Initializes the <see cref="PropertyStrategyFactory" /> class.
        /// </summary>
        static PropertyStrategyFactory()
        {
            _defaultStrategy = new DefaultStrategy();
            _resultMapStrategy = new ResultMapStrategy();
            _groupByStrategy = new GroupByStrategy();

            _selectArrayStrategy = new SelectArrayStrategy();
            _selectListStrategy = new SelectListStrategy();
            _selectObjectStrategy = new SelectObjectStrategy();
            _selectGenericListStrategy = new SelectGenericListStrategy();
        }

        /// <summary>
        ///     Finds the <see cref="IPropertyStrategy" />.
        /// </summary>
        /// <param name="mapping">The <see cref="ResultProperty" />.</param>
        /// <returns>The <see cref="IPropertyStrategy" /></returns>
        public static IPropertyStrategy Get(ResultProperty mapping)
        {
            // no 'select' or 'resultMap' attributes
            if (mapping.Select.Length == 0 && mapping.NestedResultMap == null) return _defaultStrategy;

            if (mapping.NestedResultMap != null) // 'resultMap' attribute
            {
                if (mapping.NestedResultMap.GroupByPropertyNames.Count > 0) return _groupByStrategy;

                if (mapping.MemberType.IsGenericType &&
                    typeof(IList<>).IsAssignableFrom(mapping.MemberType.GetGenericTypeDefinition()))
                    return _groupByStrategy;
                if (typeof(IList).IsAssignableFrom(mapping.MemberType))
                    return _groupByStrategy;
                return _resultMapStrategy;
            }

            return new SelectStrategy(mapping,
                _selectArrayStrategy,
                _selectGenericListStrategy,
                _selectListStrategy,
                _selectObjectStrategy);
        }
    }
}