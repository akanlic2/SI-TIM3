using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConferenceManagement.Application.Services
{
    /// <summary>
    /// Servis za pristup informacijama o trenutnom korisniku (ulogu, ID, itd.)
    /// Koristi se u svim dijelovima aplikacije (servisi, kontroleri, itd.)
    /// </summary>
    public interface IUserContextService
    {
        /// <summary>
        /// Dohvata sve role trenutnog korisnika
        /// </summary>
        IEnumerable<string> GetUserRoles();

        /// <summary>
        /// Provjerava ima li korisnik određenu ulogu
        /// </summary>
        bool HasRole(string role);

        /// <summary>
        /// Provjerava ima li korisnik bilo koju od navedenih ulog
        /// </summary>
        bool HasAnyRole(params string[] roles);

        /// <summary>
        /// Provjerava ima li korisnik sve navedene uloge
        /// </summary>
        bool HasAllRoles(params string[] roles);

        /// <summary>
        /// Dohvata ID trenutnog korisnika (sub claim iz JWT-a)
        /// </summary>
        string GetUserId();

        /// <summary>
        /// Dohvata username trenutnog korisnika
        /// </summary>
        string GetUsername();

        /// <summary>
        /// Provjerava je li korisnik autentificiran
        /// </summary>
        bool IsAuthenticated();
    }
}
