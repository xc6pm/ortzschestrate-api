using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Ortzschestrate.Data.Models
{
    public class User : IdentityUser
    {
        [StringLength(40)] public string UnverifiedWalletAddress { get; set; }
        [StringLength(40)] public string WalletAddress { get; set; }
    }
}