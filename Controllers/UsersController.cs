﻿using MesDoigtsDeFees.Areas.Identity.Data;
using MesDoigtsDeFees.Data;
using MesDoigtsDeFees.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace MesDoigtsDeFees.Controllers
{
    [Authorize(Roles = "SystemAdministrator")]
    public class UsersController : Controller
    {
        private readonly MyDBContext _context;
        public UsersController(MyDBContext context)
        {



            _context = context;
        }
        public IActionResult Index(string userName, string firstName, string lastName, string email)
        {
            List<UserIndexViewModel> vmUsers = new List<UserIndexViewModel>();
            List<MesDoigtsDeFeesUser> users = _context.Users
                                            .Where(u => u.UserName != "Dummy"
                                                    && (u.UserName.Contains(userName) || string.IsNullOrEmpty(userName))
                                                    && (u.FirstName.Contains(firstName) || string.IsNullOrEmpty(firstName))
                                                    && (u.LastName.Contains(lastName) || string.IsNullOrEmpty(lastName))
                                                    && (u.Email.Contains(email) || string.IsNullOrEmpty(email))
                                                   )
                                            .ToList();

            foreach (MesDoigtsDeFeesUser user in users)
            {
                vmUsers.Add(new UserIndexViewModel
                {
                    Blocked = !user.LockoutEnabled,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Roles = (from userRole in _context.UserRoles
                             where userRole.UserId == user.Id
                             orderby userRole.RoleId
                             select userRole.RoleId).ToList()
                });
            }
            ViewData["userName"] = userName;
            ViewData["firstName"] = firstName;
            ViewData["lastName"] = lastName;
            ViewData["email"] = email;
            return View(vmUsers);
        }

        public IActionResult UnBlock(string userName)
        {
            MesDoigtsDeFeesUser user = _context.Users.FirstOrDefault(u => u.UserName == userName);
            user.LockoutEnabled = true;
            _context.Update(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Block(string userName)
        {
            MesDoigtsDeFeesUser user = _context.Users.FirstOrDefault(u => u.UserName == userName);
            user.LockoutEnabled = false;
            _context.Update(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Roles(string userName)
        {
            MesDoigtsDeFeesUser user = _context.Users.FirstOrDefault(u => u.UserName == userName);

            try
            {
                UserRolesViewModel roleViewModel = new UserRolesViewModel
                {
                    UserName = userName,
                    Roles = (from userRole in _context.UserRoles
                             where userRole.UserId == user.Id
                             orderby userRole.RoleId
                             select userRole.RoleId).ToList()
                };

                ViewData["AllRoles"] = new MultiSelectList(_context.Roles.OrderBy(r => r.Name), "Id", "Name", roleViewModel.Roles);

                return View(roleViewModel);
            }
            catch (Exception ex)
            {

                var errorViewModel = new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    // Voeg specifieke foutinformatie toe aan het ErrorViewModel
                    ErrorMessage = ex.Message
                };

                // Stuur het foutenmodel naar de weergave voor foutafhandeling
                return View("Error", errorViewModel);
            }
        }

        [HttpPost]
        public IActionResult Roles([Bind("UserName, Roles")] UserRolesViewModel _model)
        {
            MesDoigtsDeFeesUser user = _context.Users.FirstOrDefault(u => u.UserName == _model.UserName);

            // Bestaande rollen ophalen
            List<IdentityUserRole<string>> roles = _context.UserRoles.Where(ur => ur.UserId == user.Id).ToList();
            foreach (IdentityUserRole<string> role in roles)
                _context.Remove(role);

            // Nieuwe rollen toekennen
            if (_model.Roles != null)
                foreach (string roleId in _model.Roles)
                    _context.UserRoles.Add(new IdentityUserRole<string> { RoleId = roleId, UserId = user.Id });

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
