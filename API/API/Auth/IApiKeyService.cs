using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Auth {
    internal interface IApiKeyService {
        string GenerateApiKey();
    }

    internal class ApiKeyService : IApiKeyService {
        private const int _lengthOfKey = 64;

        public string GenerateApiKey() {
            
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[_lengthOfKey];
            rng.GetBytes(bytes);

            string base64String = Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_");
            
            return base64String;
        }
    }
}
