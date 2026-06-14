using Microsoft.AspNetCore.Identity;

namespace AdminPanel.Models
{
    // E-posta ve şifre dışında şimdilik ek alan istenmediği için boş bırakıyoruz.
    // İleride ad, soyad, profil fotoğrafı gibi alanlar eklenmek istenirse buraya eklenebilir.
    public class ApplicationUser : IdentityUser
    {
    }
}
