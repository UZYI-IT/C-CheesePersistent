using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheeseMVC.Data;
using CheeseMVC.Models;
using CheeseMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace CheeseMVC.Controllers
{
    public class MenuController : Controller
    {
        private CheeseDbContext context;

        public MenuController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult Index()
        {
            IList<Menu> Menus = context.Menus.ToList();

            return View(Menus);
        }

        public IActionResult Add()
        {
            AddMenuViewModel addMenuViewModel = new AddMenuViewModel();

            return View(addMenuViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddMenuViewModel addMenuViewModel)
        {

            if (ModelState.IsValid)
            {
                Menu newMenu = new Menu
                {
                    Name = addMenuViewModel.Name,
                };

                context.Menus.Add(newMenu);
                context.SaveChanges();

                return Redirect("/Menu/ViewMenu/" + newMenu.ID);
            }

            return View(addMenuViewModel);
        }

        public IActionResult ViewMenu(int id)
        {
            List<CheeseMenu> items = context.CheeseMenus.Include(item => item.Cheese).Where(cm => cm.MenuID == id).ToList();
            Menu menu = context.Menus.Single(c => c.ID == id);

            ViewMenuViewModel viewMenuViewModel = new ViewMenuViewModel
            {
               Menu = menu,
               Items = items

            };

            return View(viewMenuViewModel);
        }

        public IActionResult AddItem(int id)
        {
            
            Menu menu = context.Menus.Single(c => c.ID == id);
            
            IList<Cheese> cheeses = context.Cheeses.ToList();

            AddMenuItemViewModel addMenuItemViewModel = new AddMenuItemViewModel(menu, cheeses);
            //(c => c.Category

            return View(addMenuItemViewModel);
        }

        [HttpPost]
        public IActionResult AddItem(AddMenuItemViewModel addMenuItemViewModel)
        {
            

            if (ModelState.IsValid)
            {
                IList<CheeseMenu> existingItems = context.CheeseMenus.Where(cm => cm.CheeseID == addMenuItemViewModel.cheeseID).Where(cm => cm.MenuID == addMenuItemViewModel.menuID).ToList();

                var MenuID = addMenuItemViewModel.menuID;
                var CheeseID = addMenuItemViewModel.cheeseID;

                if (existingItems.Count == 0)
                {

                    CheeseMenu newCheeseMenu = new CheeseMenu
                    {

                        Menu = context.Menus.Single(m => m.ID == MenuID),
                        Cheese = context.Cheeses.Single(c => c.ID == CheeseID)
                    };

                    context.CheeseMenus.Add(newCheeseMenu);
                    context.SaveChanges();
                }

                return Redirect("/Menu/ViewMenu/" + addMenuItemViewModel.menuID);
            }

            return View(addMenuItemViewModel);
        }
    }
}