﻿namespace KOM.Scribere.Web.Areas.Administration.Controllers;

using System.Linq;
using System.Threading.Tasks;

using KOM.Scribere.Data.Common.Repositories;
using KOM.Scribere.Data.Models;
using Microsoft.AspNetCore.Mvc;

public class PagesController : AdministrationController<PagesController>
{
    private readonly IDeletableEntityRepository<Page> pages;

    public PagesController(IDeletableEntityRepository<Page> pages)
    {
        this.pages = pages;
    }

    public IActionResult Index()
    {
        return this.View(this.pages.All().ToList());
    }

    public IActionResult Details(int? id)
    {
        if (id == null)
        {
            return this.BadRequest();
        }

        var page = this.pages.All().FirstOrDefault(x => x.Id == id);
        if (page == null)
        {
            return this.NotFound();
        }

        return this.View(page);
    }

    public IActionResult Create()
    {
        return this.View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Page page)
    {
        if (this.ModelState.IsValid)
        {
            await this.pages.AddAsync(page);
            await this.pages.SaveChangesAsync();
            return this.RedirectToAction("Index");
        }

        return this.View(page);
    }

    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return this.BadRequest();
        }

        var page = this.pages.All().FirstOrDefault(x => x.Id == id);
        if (page == null)
        {
            return this.NotFound();
        }

        return this.View(page);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Page page)
    {
        if (this.ModelState.IsValid)
        {
            this.pages.Update(page);
            await this.pages.SaveChangesAsync();
            return this.RedirectToAction("Index");
        }

        return this.View(page);
    }

    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return this.BadRequest();
        }

        var page = this.pages.All().FirstOrDefault(x => x.Id == id);
        if (page == null)
        {
            return this.NotFound();
        }

        return this.View(page);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var page = this.pages.All().FirstOrDefault(x => x.Id == id);
        this.pages.Delete(page);
        await this.pages.SaveChangesAsync();
        return this.RedirectToAction("Index");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.pages.Dispose();
        }

        base.Dispose(disposing);
    }
}