using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNote.Models
{
    public class Note
    {
        public int Id {get;set;}
        public string Path {get;set;}
        public string Title {get;set;}
        public string Content {get;set;}
        public DateTime ModifiedAt {get;set;} = DateTime.Now; // изначально проставим дату при создании
    }
}