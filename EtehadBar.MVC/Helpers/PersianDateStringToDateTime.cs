using MD.PersianDateTime.Standard;
using System;

namespace Helpers
{
    public static class PersianDateStringToDateTime
    {
        /// <summary>
        /// this extension used to convert persian date with yyyy*mm*dd format comes from date picker to datetime format
        /// </summary>
        /// <param name="persianDateString"></param>
        /// <param name="delimiter"></param>
        /// <returns>Datetime</returns>
        public static DateTime ToDateTime(this string persianDateString, string delimiter)
        {
            var dateArray = persianDateString.Split(delimiter);
            return new PersianDateTime(Convert.ToInt32(dateArray[0]), Convert.ToInt32(dateArray[1]), Convert.ToInt32(dateArray[2])).ToDateTime();
        }
    }
}
