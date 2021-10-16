using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackOverflowTags.APIs;
using StackOverflowTags.Models;

namespace StackOverflowTags.Controllers
{
    public class TagsController : Controller
    {
        private readonly ILogger<TagsController> _logger;
        private readonly StackOverflowApiAdapter _stackOverflowApiAdapter;
        public TagsController(ILogger<TagsController> logger)
        {
            _logger = logger;
            _stackOverflowApiAdapter = new StackOverflowApiAdapter(logger);


        }
        public async  Task<IActionResult> Index()
        {
            
            var tagsList = await _stackOverflowApiAdapter.GetTagsAsync();
            if (tagsList == null)
            {
                return View("Error");
            }

            var popularitySum = tagsList.Sum(x => x.Count);

            var viewModel = new TagsViewModel()
            {
                Tags = tagsList,
                PopularitySum = popularitySum
            };



            return View(viewModel);
        }
    }
}
