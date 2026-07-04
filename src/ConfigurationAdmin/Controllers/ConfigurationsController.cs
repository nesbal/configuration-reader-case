using ConfigurationAdmin.Services;
using ConfigurationReader.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConfigurationAdmin.Controllers;

public class ConfigurationsController : Controller
{
    private readonly IConfigurationAdminService _service;

    public ConfigurationsController(IConfigurationAdminService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _service.GetAllAsync();
        return View(items);
    }

    public IActionResult Create()
    {
        return View(new ConfigurationItem());
    }

    [HttpPost]
    public async Task<IActionResult> Create(ConfigurationItem item)
    {
        if (!ModelState.IsValid)
        {
            return View(item);
        }

        await _service.CreateAsync(item);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string id)
    {
        var item = await _service.GetByIdAsync(id);

        if (item is null)
        {
            return NotFound();
        }

        return View(item);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ConfigurationItem item)
    {
        if (!ModelState.IsValid)
        {
            return View(item);
        }

        await _service.UpdateAsync(item);
        return RedirectToAction(nameof(Index));
    }
}