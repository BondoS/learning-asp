﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
	public class MoviesController : Controller
	{

		private ApplicationDbContext _context;

		public MoviesController()
		{
			_context = new ApplicationDbContext();
		}

		protected override void Dispose(bool disposing)
		{
			_context.Dispose();
		}

		public ActionResult New()
		{   var genres = _context.Genres.ToList();
			var viewModel = new MovieFormViewModel
			{
				Genres = genres
			};
			return View("MovieForm", viewModel);
		}
		public ActionResult Random()
		{
			var movie = new Movie() { Name = "Shrek!" };
			var customers = new List<Customer>{
				new Customer { Name = "Customer 1"},
				new Customer { Name = "Customer 2"}
			};

			var viewModel = new RandomMovieViewModel
			{
				Movie = movie,
				Customers = customers
			};
			//var viewResult = new ViewResult();
			//viewResult.ViewData.Model

			return View(viewModel);
			//return Content("Hellow world!");
			//return HttpNotFound();
			//return new EmptyResult();
			//return RedirectToAction("Index", "Home", new { page = 1, sortBy = "name" });
		}

		// movies
		public ActionResult Index()
		{
			var movies = _context.Movies.Include(m => m.Genre).ToList();
			return  View(movies);
		}

		public ActionResult Edit(int id)
		{
			var movie = _context.Movies.SingleOrDefault(c => c.Id == id);

			if (movie == null)
				return HttpNotFound();

			var viewModel = new MovieFormViewModel
			{
				Movie = movie,
				Genres = _context.Genres.ToList()
			};
			return View("MovieForm",viewModel);
		}
		public ActionResult Details(int id)
		{
			var movie = _context.Movies.Include(m => m.Genre).SingleOrDefault(m => m.Id == id);
			if (movie == null)
				return HttpNotFound();
			return View(movie);
		}

		[Route("movies/released/{year:regex(\\d{4})}/{month:range(1, 12)}")]
		public ActionResult ByReleaseDate(int year, int month)
		{
			return Content(year + "/" + month);
		}

		[HttpPost]
		public ActionResult Save(Movie movie)
		{
			if (movie.Id == 0)
			{
				movie.DateAdded = DateTime.Now;
				_context.Movies.Add(movie);
			}
			else
			{
				var movieInDb = _context.Movies.Single(m => m.Id == movie.Id);
				movieInDb.Name = movie.Name;
				movieInDb.GenreId = movie.GenreId;
				movieInDb.NumberInStock = movie.NumberInStock;
				movieInDb.ReleaseDate = movie.ReleaseDate;
			}

			_context.SaveChanges();

			return RedirectToAction("Index","Movies");
		}
	}
}