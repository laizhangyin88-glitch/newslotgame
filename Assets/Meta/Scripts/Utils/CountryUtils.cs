using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class CountryUtils
    {
        private static List<string> countryKeys = null;
        private static Dictionary<string, string> countryAlpha2Dict = new Dictionary<string, string>()
            {
                { "AD", "Andorra" },
                { "AE", "United Arab Emirates" },
                { "AF", "Afghanistan" },
                { "AG", "Antigua and Barbuda" },
                { "AL", "Albania" },
                { "AM", "Armenia" },
                { "AO", "Angola" },
                { "AR", "Argentina" },
                { "AT", "Austria" },
                { "AU", "Australia" },
                { "AW", "Aruba" },
                { "AZ", "Azerbaijan" },
                { "BS", "Bahamas" },
                { "BH", "Bahrain" },
                { "BD", "Bangladesh" },
                { "BB", "Barbados" },
                { "BY", "Belarus" },
                { "BE", "Belgium" },
                { "BZ", "Belize" },
                { "BJ", "Benin" },
                { "BT", "Bhutan" },
                { "BO", "Bolivia" },
                { "BQ", "Bonaire" },
                { "BA", "Bosnia and Herzegovina" },
                { "BW", "Botswana" },
                { "BR", "Brazil" },
                { "BN", "Brunei Darussalam" },
                { "BG", "Bulgaria" },
                { "BF", "Burkina Faso" },
                { "BI", "Burundi" },
                { "KH", "Cambodia" },
                { "CM", "Cameroon" },
                { "CA", "Canada" },
                { "CF", "Central African Republic" },
                { "CL", "Chile" },
                { "CN", "China" },
                { "CO", "Colombia" },
                { "CR", "Costa Rica" },
                { "CI", "Côte d'Ivoire " },
                { "HR", "Croatia" },
                { "CU", "Cuba" },
                { "CW", "Curaçao" },
                { "CY", "Cyprus" },
                { "CZ", "Czechia" },
                { "DK", "Denmark" },
                { "DJ", "Djibouti" },
                { "DM", "Dominica" },
                { "DO", "Dominican Republic" },
                { "DZ", "Algeria" },
                { "EC", "Ecuador" },
                { "EG", "Egypt" },
                { "SV", "El Salvador" },
                { "GQ", "Equatorial Guinea" },
                { "ER", "Eritrea" },
                { "EE", "Estonia" },
                { "ET", "Ethiopia" },
                { "FJ", "Fiji" },
                { "FI", "Finland" },
                { "FR", "France" },
                { "PF", "French Polynesia" },
                { "GA", "Gabon" },
                { "GM", "Gambia" },
                { "GE", "Georgia" },
                { "DE", "Germany" },
                { "GH", "Ghana" },
                { "GI", "Gibraltar" },
                { "GR", "Greece" },
                { "GL", "Greenland" },
                { "GP", "Guadeloupe" },
                { "GU", "Guam" },
                { "GT", "Guatemala" },
                { "GN", "Guinea" },
                { "GY", "Guyana" },
                { "HT", "Haiti" },
                { "HN", "Honduras" },
                { "HK", "Hong Kong" },
                { "HU", "Hungary" },
                { "IS", "Iceland" },
                { "IN", "India" },
                { "ID", "Indonesia" },
                { "IR", "Iran" },
                { "IQ", "Iraq" },
                { "IE", "Ireland" },
                { "IM", "Isle of Man" },
                { "IL", "Israel" },
                { "IT", "Italy" },
                { "JM", "Jamaica" },
                { "JP", "Japan" },
                { "JE", "Jersey" },
                { "JO", "Jordan" },
                { "KZ", "Kazakhstan" },
                { "KE", "Kenya" },
                { "KI", "Kiribati" },
                { "KW", "Kuwait" },
                { "KG", "Kyrgyzstan" },
                { "LA", "Lao People's Democratic Republic" },
                { "LV", "Latvia" },
                { "LB", "Lebanon" },
                { "LS", "Lesotho" },
                { "LR", "Liberia" },
                { "LY", "Libya" },
                { "LI", "Liechtenstein" },
                { "LT", "Lithuania" },
                { "LU", "Luxembourg" },
                { "MO", "Macao" },
                { "MG", "Madagascar" },
                { "MW", "Malawi" },
                { "MY", "Malaysia" },
                { "MV", "Maldives" },
                { "ML", "Mali" },
                { "MT", "Malta" },
                { "MU", "Mauritius" },
                { "MX", "Mexico" },
                { "MD", "Moldova" },
                { "MC", "Monaco" },
                { "MN", "Mongolia" },
                { "ME", "Montenegro" },
                { "MA", "Morocco" },
                { "MZ", "Mozambique" },
                { "MM", "Myanmar" },
                { "NA", "Namibia" },
                { "NR", "Nauru" },
                { "NP", "Nepal" },
                { "NL", "Netherlands" },
                { "NZ", "New Zealand" },
                { "NI", "Nicaragua" },
                { "NE", "Niger" },
                { "NG", "Nigeria" },
                { "MK", "North Macedonia" },
                { "MP", "Northern Mariana Islands" },
                { "NO", "Norway" },
                { "OM", "Oman" },
                { "PK", "Pakistan" },
                { "PW", "Palau" },
                { "PS", "Palestine" },
                { "PA", "Panama" },
                { "PG", "Papua New Guinea" },
                { "PY", "Paraguay" },
                { "PE", "Peru" },
                { "PH", "Philippines" },
                { "PL", "Poland" },
                { "PT", "Portugal" },
                { "PR", "Puerto Rico" },
                { "QA", "Qatar" },
                { "KR", "Republic of Korea" },
                { "ZA", "Republic of South Africa" },
                { "CD", "Republic of the Congo" },
                { "RE", "Reunion" },
                { "RO", "Romania" },
                { "RU", "Russian Federation" },
                { "RW", "Rwanda" },
                { "SA", "Saudi Arabia" },
                { "SN", "Senegal" },
                { "RS", "Serbia" },
                { "SL", "Sierra Leone" },
                { "SG", "Singapore" },
                { "SK", "Slovakia" },
                { "SI", "Slovenia" },
                { "SO", "Somalia" },
                { "SS", "South Sudan" },
                { "ES", "Spain" },
                { "LK", "Sri Lanka" },
                { "SD", "Sudan" },
                { "SR", "Suriname" },
                { "SE", "Sweden" },
                { "CH", "Switzerland" },
                { "SY", "Syrian Arab Republic" },
                { "TW", "Taiwan" },
                { "TJ", "Tajikistan" },
                { "TZ", "Tanzania" },
                { "TH", "Thailand" },
                { "TL", "Timor-Leste" },
                { "TG", "Togo" },
                { "TO", "Tonga" },
                { "TT", "Trinidad and Tobago" },
                { "TN", "Tunisia" },
                { "TR", "Turkey" },
                { "TM", "Turkmenistan" },
                { "UG", "Uganda" },
                { "UA", "Ukraine" },                
                { "GB", "United Kingdom" },
                { "US", "United States of America" },
                { "VI", "United States Virgin Islands" },
                { "UY", "Uruguay" },
                { "UZ", "Uzbekistan" },
                { "VU", "Vanuatu" },
                { "VE", "Venezuela" },
                { "VN", "VietNam" },
                { "YE", "Yemen" },
                { "ZM", "Zambia" },
                { "ZW", "Zimbabwe" }
            };

        public static string GetDefaultCountryCode()
        {
            return "US";
        }

        public static List<string> GetCountryCodeList()
        {
            if(countryKeys == null)
            {
                countryKeys = countryAlpha2Dict.OrderBy(x => x.Value).Select(x => x.Key).ToList();
            }

            return countryKeys;
        }

        public static bool ExistCountryCode(string countryCode)
        {
            if(string.IsNullOrEmpty(countryCode)) return false;
            
            countryCode = countryCode.ToUpper();

            return countryAlpha2Dict.ContainsKey(countryCode);
        }

        public static string GetCountryName(string countryCode)
        {
            countryCode = countryCode.ToUpper();

            if(countryAlpha2Dict.ContainsKey(countryCode))
                return countryAlpha2Dict[countryCode];

            return countryAlpha2Dict[GetDefaultCountryCode()];
        }
    }
}