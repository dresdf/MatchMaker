using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MatchMaker.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Newtonsoft.Json;

namespace MatchMaker.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment env;
        public HomeController(IHostingEnvironment hostEnv)
        {
            env = hostEnv;
        }
        public IActionResult Index()
        {
            List<Participant> model = ReadJson();
            return View(model);
        }

        public IActionResult AddParticipant(string name)
        {
            //populate list from Json
            List<Participant> model = ReadJson();

            //add new entry
            Participant ret = new Participant
            {
                Name = name
            };
            model.Add(ret);

            //replace file content
            WriteJson(model);
            return RedirectToAction("Index");
        }

        public IActionResult Match()
        {
            List<Participant> model = ReadJson();
            List<Participant> tmp = new List<Participant>();
            tmp.AddRange(model);
            foreach (Participant p in model)
            {
                int count = tmp.Count;
                int index = new Random().Next(0, count);

                while (p.Name.Equals(tmp[index].Name))
                {
                    index = new Random().Next(0, count);
                }
                p.Match = tmp[index].Name;
                tmp.RemoveAt(index);
            }

            WriteJson(model);

            return RedirectToAction("Index");
        }

        public IActionResult ClearList()
        {
            string path = env.WebRootPath + "/json/list.json";
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                FileStream fs = new FileStream(path, FileMode.Create);
                fs.Close();
            }
            return RedirectToAction("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private List<Participant> ReadJson()
        {
            List<Participant> ret = new List<Participant>();
            string path = env.WebRootPath + "/json/list.json";
            if (System.IO.File.Exists(path))
            {
                string content = System.IO.File.ReadAllText(path);
                ret = (List<Participant>)JsonConvert.DeserializeObject(content, typeof(List<Participant>));
            }
            else
            {
                FileStream fs = new FileStream(path, FileMode.Create);
                fs.Close();
            }
            return ret ?? new List<Participant>();
        }

        private void WriteJson(List<Participant> model)
        {
            string path = env.WebRootPath + "/json/list.json";
            FileStream fs = new FileStream(path, FileMode.Create);
            string json = JsonConvert.SerializeObject(model);

            byte[] info = new System.Text.UTF8Encoding(true).GetBytes(json);
            fs.Write(info, 0, info.Length);
            fs.Close();
        }

        private List<Participant> fakeStuff()
        {
            List<Participant> ret = new List<Participant>();
            for (int i = 1; i < 10; i++)
            {
                Participant p = new Participant
                {
                    Name = "Human " + i,
                };
                ret.Add(p);
            }
            return ret;
        }
    }
}
