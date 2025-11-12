using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ImageUploadExample.Data;
using ImageUploadExample.Models;
using ImageUploadExample.ViewModels;

namespace ImageUploadExample.Controllers;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Products
    public async Task<IActionResult> Index()
    {
        return View(await _context.Product.ToListAsync());
    }

    // GET: Products/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Product
            .FirstOrDefaultAsync(m => m.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    // GET: Products/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Products/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductCreateViewModel product)
    {
        if (ModelState.IsValid)
        {
            // Add product to database
            string uniqueProductName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(product.ProductImage.FileName);
            // Ensure the images directory exists
            var imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            Directory.CreateDirectory(imagesDirectory);
            // Save the image to wwwroot/images
            var filePath = Path.Combine(imagesDirectory, uniqueProductName);
            using (var stream = System.IO.File.Create(filePath))
            {
                await product.ProductImage.CopyToAsync(stream);
            }

            Product p = new()
            {
                Name = product.Name,
                ImageUrl = uniqueProductName
            };
            _context.Product.Add(p);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    // GET: Products/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Product.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        var viewModel = new ProductEditViewModel
        {
            Id = product.Id,
            Name = product.Name,
            CurrentImageUrl = product.ImageUrl
        };

        return View(viewModel);
    }

    // POST: Products/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductEditViewModel viewModel)
    {
        if (id != viewModel.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var product = await _context.Product.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                // Update the name
                product.Name = viewModel.Name;

                // Check if a new image was uploaded
                if (viewModel.ProductImage != null)
                {
                    // Delete the old image file if it exists
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", product.ImageUrl);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                    // Save the new image
                    string uniqueProductName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.ProductImage.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", uniqueProductName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await viewModel.ProductImage.CopyToAsync(stream);
                    }

                    // Update the image URL
                    product.ImageUrl = uniqueProductName;
                }
                // If no new image is uploaded, keep the existing ImageUrl

                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(viewModel.Id))
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
        
        // If model state is invalid, preserve the current image URL for display
        viewModel.CurrentImageUrl = (await _context.Product.FindAsync(id))?.ImageUrl ?? viewModel.CurrentImageUrl;
        return View(viewModel);
    }

    // GET: Products/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Product
            .FirstOrDefaultAsync(m => m.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    // POST: Products/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Product.FindAsync(id);
        if (product != null)
        {
            // Delete the image file from the file system
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", product.ImageUrl);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            _context.Product.Remove(product);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ProductExists(int id)
    {
        return _context.Product.Any(e => e.Id == id);
    }
}
