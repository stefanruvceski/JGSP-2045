using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebApp.Models;
using WebApp.Persistence;
using WebApp.Persistence.Repository;

namespace WebApp.Controllers
{
    [Authorize]
    [RoutePrefix("api/User")]
    public class UsersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IUserRepository userRepository;
        private IAgeGroupRepository ageGroupRepository;
        
        public UsersController(IUserRepository userRepository, IAgeGroupRepository ageGroupRepository)
        {
            this.userRepository = userRepository;
            this.ageGroupRepository = ageGroupRepository;
        }

        // POST api/User/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(UserRegistrationBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // provera da li vec postoji korisnik sa tim email-om
            if (userRepository.Find(x => x.Email == model.Email).Count() != 0)
            {
                return BadRequest("Email already in use...");
            }
            // provera da li vec postoji korisnik sa tim username-om
            if (userRepository.Find(x => x.Username == model.Username).Count() != 0)
            {
                return BadRequest("Username already in use...");
            }

            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);

            //ApplicationUser user =  db.Users.Find(User.Identity.Name);

            DateTime birthday = DateTime.Parse(model.Birthday);
            int ageGroup = 1;
            switch (model.AgeGroup.ToUpper())
            {
                case "REGULAR":
                    ageGroup = 1;
                    break;
                case "STUDENT":
                    ageGroup = 2;
                    break;
                case "PENSIONER":
                    ageGroup = 3;
                    break;
                default:
                    ageGroup = 1;
                    break;
            }

            // kreiran User, treba ga dodati u bazu
            //string passwordHash = ApplicationUser.HashPassword(model.Password);

            Passenger user = new Passenger(model.Email, model.Password, model.FirstName, model.LastName, model.Username, birthday, model.Address, model.Document);
            //user.AgeGroup = ageGroupRepository.Get(ageGroup);
            user.AgeGroupId = ageGroup;
            user.Tickets = new List<Ticket>();
            user.VerificationStatus = VerificationStatus.Pending;

            // dodavanje u bazu koriscenjem repository klase
            //userRepository.Add(user);
            db.AppUsers.Add(user);
            db.SaveChanges();

            // kreiram ApplicationUser-a
            var appUser = new ApplicationUser() { Id = user.Id.ToString(), UserName = model.Username, Email = model.Email, PasswordHash = ApplicationUser.HashPassword(model.Password) };
            appUser.UserId = user.Id;       // dodavanje stranog kljuca na Id iz moje User tabele
            IdentityResult result = await userManager.CreateAsync(appUser, user.Password);
            userManager.AddToRole(appUser.Id, "AppUser");

            user.Password = appUser.PasswordHash;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET: api/User/GetInfo
        [Authorize(Roles = "AppUser")]
        [Route("GetInfo")]
        [ResponseType(typeof(UserRegistrationBindingModel))]
        public IHttpActionResult GetUserInfo(string username)
        {
            Passenger user = (Passenger)userRepository.GetAll().Where(x => x.Username == username).ToList().First();
            if (user == null)
            {
                return NotFound();
            }

            string ageGroup;
            switch (user.AgeGroupId)
            {
                case 1:
                    ageGroup = "Regular";
                    break;
                case 2:
                    ageGroup = "Student";
                    break;
                case 3:
                    ageGroup = "Pensioner";
                    break;
                default:
                    ageGroup = "None";
                    break;
            }

            UserRegistrationBindingModel userRetval = new UserRegistrationBindingModel()
            {
                Email = user.Email,
                Password = user.Password,
                ConfirmPassword = user.Password,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Birthday = user.Birthday.ToString(),
                AgeGroup = ageGroup,
                Document = user.Document,
                Address = user.Address
            };

            return Ok(userRetval);
        }

        // POST api/User/ChangeInfo
        [Authorize(Roles = "AppUser")]
        [ResponseType(typeof(UserRegistrationBindingModel))]
        [Route("ChangeInfo")]
        public IHttpActionResult Edit(UserRegistrationBindingModel model)
        {
            // validacija
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // username i email se sigurno ne menjaju
            // nalazenje podataka o User-u preko username-a
            Passenger user = (Passenger)userRepository.GetAll().Where(x => x.Username == model.Username).ToList().First();
            if (user == null)
            {
                return NotFound();
            }

            // parsiranje ageGroup-a
            int ageGroup = 1;
            switch (model.AgeGroup)
            {
                case "Regular":
                    ageGroup = 1;
                    break;
                case "Student":
                    ageGroup = 2;
                    break;
                case "Pensioner":
                    ageGroup = 3;
                    break;
                default:
                    ageGroup = 1;
                    break;
            }

            // parsiranje datuma
            DateTime birthday = DateTime.Parse(model.Birthday);

            // ako se promenila vrednost ageGroup-e
            if (user.AgeGroupId != ageGroup)
            {
                // i ta nova vrednost nije Regular => treba vratiti status na pending, i obrisati sliku (ako nije promenjena)
                if (ageGroup != 1)
                {
                    user.VerificationStatus = VerificationStatus.Pending;
                }
                // i ta nova vrednost je Regular => treba postaviti status na succecssfull, i obrisati sliku (ako nije promenjena)
                else
                {
                    user.VerificationStatus = VerificationStatus.Successful;
                }

                // brisanje slike, ako nije promenjena (i ako je uopste pre toga imao sliku)
                // ako je promenio grupu, a nije promenio sliku, treba obrisati njegovu sliku (obrisati i ne postaviti opet istu sliku)
                if (user.Document != null)
                {
                    if (user.Document.SequenceEqual(model.Document))
                    {
                        user.Document = null;
                    }
                    else
                    {
                        // promenio je starosnu grupu, i postavio novi dokument => novi dokument se smesta u bazu i ceka se kontroler da potvrdi/odbije
                        user.Document = model.Document;
                    }
                }
                else
                {
                    user.Document = model.Document;
                    user.VerificationStatus = VerificationStatus.Pending;
                }
            }
            // ako nije promenio grupu, a promenio je sliku, treba sacuvati novu sliku i promeniti status na Pending
            else
            {
                if (user.Document != null)
                {
                    if (!user.Document.SequenceEqual(model.Document))
                    {
                        user.Document = model.Document;
                        user.VerificationStatus = VerificationStatus.Pending;
                    }
                }
                else
                {
                    user.Document = model.Document;
                    user.VerificationStatus = VerificationStatus.Pending;
                }
            }

            // izmena zeljenih propertija
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address;
            user.AgeGroupId = ageGroup;
            user.Birthday = birthday;

            // izmena u bazi
            //userRepository.Update(user);                      // ne radi kad koristim Repository metodu...
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            string ageGroupString;
            switch (user.AgeGroupId)
            {
                case 1:
                    ageGroupString = "Regular";
                    break;
                case 2:
                    ageGroupString = "Student";
                    break;
                case 3:
                    ageGroupString = "Pensioner";
                    break;
                default:
                    ageGroupString = "None";
                    break;
            }
            UserRegistrationBindingModel userRetVal = new UserRegistrationBindingModel()
            {
                Email = user.Email,
                Password = user.Password,
                ConfirmPassword = user.Password,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Birthday = user.Birthday.ToString(),
                AgeGroup = ageGroupString,
                Document = user.Document,
                Address = user.Address
            };
            return Ok(userRetVal);
        }

        // GET: api/User/GetUsers
        [Authorize(Roles = "Controller")]
        [Route("GetUsers")]
        [ResponseType(typeof(List<UserStatusBindingModel>))]
        public List<UserStatusBindingModel> GetAppUsers(VerificationStatus userStatus)
        {
            List<Passenger> passengers = new List<Passenger>();

            foreach (AppUser u in userRepository.GetAll().ToList())
            {
                if(u.Type == UserType.Passenger)
                {
                    passengers.Add((Passenger)u);
                }              
            }

            var query = (from user in passengers
                         join ag in ageGroupRepository.GetAll().ToList() on user.AgeGroupId equals ag.Id
                         where user.VerificationStatus == userStatus
                         select new { user.Username,user.Type, user.FirstName, user.LastName, ag.GroupName, user.Document, user.Birthday, Status = user.VerificationStatus }).ToList();


            List<UserStatusBindingModel> retVal = new List<UserStatusBindingModel>();
            foreach (var temp in query)
            {
                if(temp.Type == UserType.Passenger)
                    retVal.Add(new UserStatusBindingModel() { Username = temp.Username, FirstName = temp.FirstName, LastName = temp.LastName, AgeGroup = temp.GroupName, Birthday = temp.Birthday.ToString(), Document = temp.Document, Status = temp.Status.ToString() });
            }

            return retVal;
        }

        // POST api/User/ChangeUserStatus
        [Authorize(Roles = "Controller")]
        [ResponseType(typeof(UserRegistrationBindingModel))]
        [Route("ChangeUserStatus")]
        public IHttpActionResult ChangeUserStatus(UserStatusBindingModel model)
        {
            // nalazenje user-a kom se menja status
            Passenger user = (Passenger)userRepository.GetAll().Where(x => x.Username == model.Username).ToList().First();
            string body;
            string ageGroup = ageGroupRepository.GetAll().Where(x => x.Id == user.AgeGroupId).ToList().First().GroupName;

            // promena statusa na osnovu sadrzaja propertija Status iz prosledjenog objekta
            if (model.Status.ToUpper() == "YES")
            {
                user.VerificationStatus = VerificationStatus.Successful;
                body = String.Format($"Dear {user.FirstName} {user.LastName}\n\n\tYour status is verificated. Now you can buy tickets as {ageGroup} user.\n\nSincerely,\nLondon Bus Service\n");
            }
            else
            {
                user.VerificationStatus = VerificationStatus.Unsuccessful;
                body = String.Format($"Dear {user.FirstName} {user.LastName}\n\n\tYour status isn't verificated. Now you Now you have to buy tickets as Regular user.\n\nSincerely,\nLondon Bus Service\n");
            }

            // izmena u bazi
            //userRepository.Update(user);                      // ne radi kad koristim Repository metodu...
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            // slanje obavestenja mejlom
            MailHandler.SendMail(user.Email, "Verification status", body);

            return Ok();
        }

        // GET: api/Users/5
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult GetUser(int id)
        {
            AppUser user = db.AppUsers.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser(int id, AppUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Users
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult PostUser(AppUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AppUsers.Add(user);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult DeleteUser(int id)
        {
            AppUser user = db.AppUsers.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            db.AppUsers.Remove(user);
            db.SaveChanges();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.AppUsers.Count(e => e.Id == id) > 0;
        }

        #region Helpers

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        #endregion
    }
}