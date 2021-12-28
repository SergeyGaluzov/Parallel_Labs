using System;
using System.Collections.Generic;
using System.Text;

namespace lab2.Models
{
    public class Client
    {
        public string Id { get; }
        public bool isChangeQueue { get; set; }

        public Client(string id)
        {
            Id = id;
            isChangeQueue = false;
        }
    }
}
