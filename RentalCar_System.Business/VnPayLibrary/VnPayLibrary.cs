using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace RentalCar_System.Business.VnPayLibrary
{
   

    namespace RentalCar_System.Business.VnPayLibrary
    {
        public class VnPayLibrary
        {
            private SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
            private SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

            public void AddRequestData(string key, string value)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _requestData.Add(key, value);
                }
            }

            public void AddResponseData(string key, string value)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _responseData.Add(key, value);
                }
            }

            public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
            {
                string queryString = QueryHelpers.AddQueryString(baseUrl, _requestData);
                string signData = string.Join("&", _requestData.Select(kvp => $"{kvp.Key}={kvp.Value}"));
                string vnp_SecureHash = HmacSHA512(vnp_HashSecret, signData);
                return $"{queryString}&vnp_SecureHash={vnp_SecureHash}";
            }

            public bool ValidateSignature(IQueryCollection vnpayData, string vnp_HashSecret)
            {
                string vnp_SecureHash = vnpayData["vnp_SecureHash"];
                string signData = string.Join("&", _responseData.Select(kvp => $"{kvp.Key}={kvp.Value}"));
                string calculatedHash = HmacSHA512(vnp_HashSecret, signData);
                return vnp_SecureHash == calculatedHash;
            }

            private string HmacSHA512(string key, string data)
            {
                var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }

            public SortedList<string, string> GetRequestData()
            {
                return _requestData;
            }
        }

        public class VnPayCompare : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;
                return string.CompareOrdinal(x, y);
            }
        }
    }


}
