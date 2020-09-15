﻿using System;
using System.Collections.Generic;

namespace MagicTheGatheringFinal.Models
{
    public partial class DecksTable
    {
        public DecksTable()
        {
            CardsTable = new HashSet<CardsTable>();
        }

        public int Id { get; set; }
        public int? CardId { get; set; }
        public string AspUserId { get; set; }

        public virtual AspNetUsers AspUser { get; set; }
        public virtual CardsTable Card { get; set; }
        public virtual ICollection<CardsTable> CardsTable { get; set; }
    }
}
