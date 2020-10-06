using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Saint.Seiya.Infrastructure
{
    internal static class SqlQueryGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentCollection"></param>
        /// <param name="conditionsObject"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        internal static string GenerateSqlQuery(string documentCollection, JObject conditionsObject, IEnumerable<string> fields)
        {
            var selectStatement = fields != null && fields.Any() ? GenerateSelectStatement(fields, documentCollection) : $"Select * from {documentCollection} d";
            var conditions = conditionsObject != null && conditionsObject.HasValues ? GenerateWhereStatement(conditionsObject) : string.Empty;

            return $"{selectStatement} {conditions}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        private static string GenerateWhereStatement(JObject conditions)
        {
            var stringConditions = GenerateConditions(conditions).ToList();
            return $"WHERE {string.Join(" AND ", stringConditions)}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        private static IEnumerable<string> GenerateConditions(JObject conditions)
        {
            foreach (var condition in conditions)
            {
                var isArrayToken = condition.Value.Type == JTokenType.Array;
                if (isArrayToken)
                {
                    yield return GenerateConditionValue(condition.Key, condition.Value);
                }
                else
                {
                    yield return $"d.{condition.Key} {GenerateConditionValue(condition.Key, condition.Value)}";
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string GenerateConditionValue(string fieldName, JToken value)
        {
            var actions = new Dictionary<JTokenType, Func<string>>
            {
                [JTokenType.String] = () => { return $"= \"{value.ToString()}\""; },
                [JTokenType.Boolean] = () => { return $"= {value.Value<bool>().ToString().ToLower(System.Globalization.CultureInfo.InvariantCulture)}"; },
                [JTokenType.Date] = () => { return $"= \"{value.Value<DateTime>().ToString("o")}\""; },
                [JTokenType.Array] = () => { return GenerateArrayCondition(fieldName, value); }
            };

            return actions[value.Type]();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="documentCollection"></param>
        /// <returns></returns>
        private static string GenerateSelectStatement(IEnumerable<string> conditions, string documentCollection)
        {
            return $"Select {string.Join($", ", conditions.Select(c => $"d.{c}").ToList())} from {documentCollection} d";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        private static string GenerateArrayCondition(string fieldName, JToken array)
        {
            var conditionArray = new List<string>();
            var isStringArray = array.Values().First().Type == JTokenType.String;

            foreach (var arrayIten in array)
            {
                var conditionValue = isStringArray ? $"\"{arrayIten.Value<string>()}\"" : arrayIten.Value<string>();
                conditionArray.Add($"ARRAY_CONTAINS(d.{fieldName}, {conditionValue})");
            }
            return $"({string.Join(" AND ", conditionArray)})";
        }
    }
}
