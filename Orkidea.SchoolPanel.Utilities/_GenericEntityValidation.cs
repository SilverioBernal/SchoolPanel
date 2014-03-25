using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orkidea.SchoolPanel.Utilities
{
    public static class _GenericEntityValidation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="objAssign"></param>
        /// <param name="objWithValues"></param>
        public static void ObjectSetByNamePropertieByObjectBases<T, T2>(this T objAssign, T2 objWithValues)
        {
            try
            {

                PropertyInfo[] objProperties = objAssign.GetType().GetProperties();
                PropertyInfo[] objPropertiesWithValues = objAssign.GetType().GetProperties();

                foreach (PropertyInfo propInfo in objProperties)
                {
                    foreach (var propInfoValues in objPropertiesWithValues)
                    {
                        if (propInfo.Name.ToString().ToUpper() == propInfoValues.Name.ToString().ToUpper())
                        {
                            object value1 = typeof(T).GetProperty(propInfo.Name).GetValue(objAssign, null);
                            object value2 = typeof(T).GetProperty(propInfoValues.Name).GetValue(objWithValues, null);

                            if (value1 != value2 && (value1 == null || !value1.Equals(value2)))
                            {
                                if (value2.GetType().ToString() != "DBNull" && value2 != null)
                                    propInfo.SetValue(objAssign, value2, null);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ObjectGetListPropertiesValues<T>(this T obj)
        {

            PropertyInfo[] oProperties = obj.GetType().GetProperties();
            Dictionary<string, object> oDictionary = new Dictionary<string, object>();

            foreach (PropertyInfo oPropInfo in oProperties)
            {
                object value = typeof(T).GetProperty(oPropInfo.Name).GetValue(obj, null);
                oDictionary.Add(oPropInfo.Name.ToString(), value);
            }
            return oDictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="newValue"></param>
        public static void ObjectSetValue<T>(this T obj, string propertyName, object newValue)
        {
            try
            {
                PropertyInfo oProperty = obj.GetType().GetProperty(propertyName);

                object valueOrg = typeof(T).GetProperty(propertyName).GetValue(obj, null);

                if (oProperty != null)
                {
                    object value = Convert.ChangeType(newValue, Convert.GetTypeCode(valueOrg));
                    oProperty.SetValue(obj, value, null);
                }
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object ObjectGetValue<T>(this T obj, string propertyName)
        {
            try
            {
                return typeof(T).GetProperty(propertyName).GetValue(obj, null);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="OriginalObj"></param>
        /// <param name="ChangedObj"></param>
        public static void EnumeratePropertyDifferences<T>(this T OriginalObj, T ChangedObj)
        {
            try
            {
                PropertyInfo[] properties = ChangedObj.GetType().GetProperties();

                foreach (PropertyInfo pi in properties)
                {
                    try
                    {
                        object OriginalValue = typeof(T).GetProperty(pi.Name).GetValue(OriginalObj, null);
                        object ChangedValue = typeof(T).GetProperty(pi.Name).GetValue(ChangedObj, null);


                        if (OriginalValue != ChangedValue && (OriginalValue == null || !OriginalValue.Equals(ChangedValue)))
                        {
                            if (OriginalValue != null)
                            {
                                if (OriginalValue.GetType().ToString() != "DBNull")
                                    pi.SetValue(OriginalObj, ChangedValue, null);
                            }
                            else
                                pi.SetValue(OriginalObj, ChangedValue, null);
                        }
                    }
                    catch (NullReferenceException) { throw new Exception("The field " + typeof(T).GetProperty(pi.Name).Name + " can't be null or does exists"); }
                    catch (Exception ex) { throw ex; }
                }
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Recursive and dynamacly object initialization only personality types 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static void ObjectInitializateProperties<T>(this T obj)
        {
            if (obj.GetType().Namespace != "System.Collections.Generic" && (obj.GetType().Namespace != "System.Collections.ICollection"))
            {
                PropertyInfo[] ObjectProperties = obj.GetType().GetProperties();

                foreach (PropertyInfo PropInfo in ObjectProperties)
                {
                    if (PropInfo.PropertyType.Namespace.ToString() != "System")
                    {
                        Type proType = PropInfo.PropertyType;
                        PropertyInfo[] Proproperties = PropInfo.PropertyType.GetProperties();
                        Type[] typeParameters = new Type[Proproperties.Count()];

                        for (int i = 0; i < Proproperties.Count(); i++)
                        {
                            foreach (PropertyInfo item in Proproperties)
                            {
                                Type propropertiesType = item.PropertyType;
                                typeParameters[i] = propropertiesType;
                            }
                        }

                        Type[] NewTypeParameters = new Type[0];

                        if (proType.GetType().Namespace.ToString() != "System.Collections.Generic" && proType.GetType().Namespace.ToString() != "System.Collections.ICollection")
                        {
                            var newObj = (Activator.CreateInstance(proType, NewTypeParameters));

                            PropInfo.SetValue(obj, newObj, null);

                            ObjectInitializateProperties(newObj);
                        }
                    }
                }
            }
        }
    }
}
