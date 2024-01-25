﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MesDoigtsDeFees.Data;
using MesDoigtsDeFees.Models;
using Microsoft.Extensions.Localization;

namespace MesDoigtsDeFees.Controllers
{
    public class LessonsController : Controller
    {
        private readonly MyDBContext _context;
        private readonly IStringLocalizer<LessonsController> _localizer;



        public LessonsController(MyDBContext context, IStringLocalizer<LessonsController> localizer)
        {
            _context = context;
            _localizer = localizer;

        }

        // GET: Lessons


        // GET: Lessons/Details/5
        public async Task<IActionResult> Index(string selectedType = "Alle")
        {

            List<ListItem> groupList = _context.Groups
                    .Select(g => new ListItem() { Text = g.Name, Value = g.Name })
                    .ToList();
            List<ListItem> list = new List<ListItem>()
        {
            new ListItem() { Text = "Theorie", Value = "Theorie" },
            new ListItem() { Text = "Praktijk", Value = "Praktijk" },
        };

            LessonIndexViewModel viewModel = new LessonIndexViewModel();
            viewModel.SelectedType = selectedType;
            viewModel.TypeList = new SelectList(list, "Value", "Text", selectedType);

            if (selectedType == "Alle")
            {
                viewModel.Lessons = await _context.Lessons
                    .OrderBy(l => l.Name)
                    .Include(l => l.Group)
                    .Where(l => l.Ended == DateTime.MaxValue)
                    .ToListAsync();
                return View(viewModel);
            }
            else
            {
                LessonIndexViewModel viewModel2 = new LessonIndexViewModel
                {
                    SelectedType = selectedType,

                    Lessons = await _context.Lessons
                        .Where(l => selectedType == "" || l.Type == selectedType)
                        .Include(l => l.Group)
                        .Where(l => l.Ended == DateTime.MaxValue)
                        .OrderBy(l => l.Name)
                        .ToListAsync()

                };

                return View(viewModel2);
            }

            
        }

    

    // GET: Lessons/Create
    public IActionResult Create()
        {
            ViewBag.TypeList = new List<string> { _localizer["Theorie"], _localizer["Praktijk"] };
            ViewBag.GroupList = new SelectList(_context.Groups, "Id", "Name");


            return View();
        }

        // POST: Lessons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Type,GroupId")] Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                // Controleer of de opgegeven RichtingId geldig is
                if (lesson.RichtingId.HasValue && !_context.Richtings.Any(r => r.Id == lesson.RichtingId))
                {
                    ModelState.AddModelError("RichtingId", "Ongeldige RichtingId");
                    return View(lesson);
                }
                _context.Add(lesson);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Als er validatiefouten zijn, stel de ViewBag opnieuw in met de juiste waarden
            ViewBag.TypeList = new List<string> {  _localizer["Theorie"],_localizer["Praktijk"] };
            ViewBag.GroupList = new SelectList(_context.Groups, "Id", "Name", lesson.GroupId);

            return View(lesson);
        }


        // GET: Lessons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Lessons == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null)
            {
                return NotFound();
            }

            ViewBag.TypeList = new List<string> { _localizer["Theorie"], _localizer["Praktijk"], lesson.Type };
            ViewBag.GroupList = new SelectList(_context.Groups.ToList(), "Id", "GroupName", lesson.GroupId);

            return View(lesson);
        }


        // POST: Lessons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,GroupId,Type")] Lesson lesson)
        {
            if (id != lesson.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lesson);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LessonExists(lesson.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.TypeList = new List<string> { _localizer["Theorie"], _localizer["Praktijk"], lesson.Type };
            ViewBag.GroupList = new SelectList(_context.Groups.ToList(), "Id", "GroupName", lesson.GroupId);

            return View(lesson);
        }

        // GET: Lessons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Lessons == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

        // POST: Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Lessons == null)
            {
                return Problem("Entity set 'MesDoigtsDeFeesContext.Lesson'  is null.");
            }
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson != null)
            {
                lesson.Ended = DateTime.Now;
                _context.Lessons.Update(lesson);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LessonExists(int id)
        {
          return (_context.Lessons?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
