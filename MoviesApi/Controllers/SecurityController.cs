using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly IDataProtector _protector;
        private readonly HashService _hashService;
        public SecurityController(IDataProtectionProvider protectionProvider, HashService hashService)
        {
            _hashService = hashService;
            //purpose : هدف
            _protector = protectionProvider.CreateProtector(purpose: "value_secret_and_unique");
        }

        [HttpGet("Hash")]
        public IActionResult GetHash()
        {
            var plainText = "Ali Chavoshi";
            var result = _hashService.Hash(plainText);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult Get()
        {
            //متنی که میخواهیم رمز گذاری کنیم
            var plainText = "Ali Chavoshi";
            //رمز گذاری
            var encryptedText = _protector.Protect(plainText);
            //رمز گشایی
            var decryptedText = _protector.Unprotect(encryptedText);
            return Ok(new { plainText, encryptedText, decryptedText });
        }

        [HttpGet("timeBound")]
        public async Task<IActionResult> GetTimeBound()
        {
            var limitedDataProtector = _protector.ToTimeLimitedDataProtector();
            //متنی که میخواهیم رمز گذاری کنیم
            var plainText = "Ali Chavoshi";
            //lifeTime=عمر این رمز گذاری چقدر است
            //عمر مفید 5 ثانیه بود
            var encryptedText = limitedDataProtector.Protect(plainText, lifetime: TimeSpan.FromSeconds(5));

            //گفتم بعد از 6 ثانیه خروجی را نشان بده که ببینم بعد از 5 ثانیه معتبر هست؟؟
            //باید قبل از اینکه رمز گشایی کنیم 6 ثانیه صبر کنیم
            await Task.Delay(6000);


            string decryptedText = limitedDataProtector.Unprotect(encryptedText);

            return Ok(new { plainText, encryptedText, decryptedText });
        }
    }
}
