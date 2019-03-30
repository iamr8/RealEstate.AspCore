using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace RealEstate.Extensions
{
    public static class CryptologyExtension
    {
        public static string Token(ClaimsIdentity claimsIdentity)
        {
            if (claimsIdentity == null) return null;

            var securityToken = new JwtSecurityToken(
                claims: claimsIdentity.Claims,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Key),
                    SecurityAlgorithms.HmacSha256Signature),
                notBefore: DateTime.UtcNow,
                expires: DateTime.Now.AddYears(1));

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }

        public static ClaimsIdentity Token(this string token)
        {
            if (string.IsNullOrEmpty(token)) return null;

            var handler = new JwtSecurityTokenHandler();
            if (!(handler.ReadToken(token) is JwtSecurityToken)) return null;

            var validationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Key),
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = true,
                ValidateActor = false,
                ValidateIssuerSigningKey = true,
            };

            var claimsPrincipal = handler.ValidateToken(token, validationParameters, out _);
            if (claimsPrincipal == null) return null;

            if (!(claimsPrincipal.Identity is ClaimsIdentity claimsIdentity) || !claimsIdentity.Claims.Any())
                return null;

            return claimsIdentity;
        }

        private static readonly byte[] Key = { 115, 6, 90, 196, 167, 224, 85, 212, 19, 51, 249, 142, 190, 12, 116, 158 };
        private static readonly byte[] Iv = { 179, 18, 225, 48, 218, 84, 112, 223, 125, 137, 126, 36, 98, 232, 231, 124 };

        public enum CypherMode
        {
            Encryption,
            Decryption
        }

        public static string Cipher(this string data, CypherMode mode)
        {
            var kdf2BytesGenerator = new Kdf2BytesGenerator(new Sha1Digest());
            var kdfParameters = new KdfParameters(Key, Iv);
            kdf2BytesGenerator.Init(kdfParameters);

            var key = new byte[16];
            kdf2BytesGenerator.GenerateBytes(key, 0, key.Length);

            var engine = new AesLightEngine();
            var blockCipher = new CbcBlockCipher(engine);
            var cipher = new PaddedBufferedBlockCipher(blockCipher, new Pkcs7Padding());
            var parameter = new KeyParameter(key);
            var parameterWithIv = new ParametersWithIV(parameter, Iv, 0, 16);

            var targetBytes = mode == CypherMode.Decryption
                ? Convert.FromBase64String(data)
                : Encoding.UTF8.GetBytes(data);

            cipher.Init(mode == CypherMode.Encryption, parameterWithIv);
            var output = new byte[cipher.GetOutputSize(targetBytes.Length)];
            var length = cipher.ProcessBytes(targetBytes, output, 0);
            cipher.DoFinal(output, length);

            return mode == CypherMode.Encryption
                ? Convert.ToBase64String(output)
                : Encoding.UTF8.GetString(output.Where(@byte => @byte != 0).ToArray());
        }
    }
}