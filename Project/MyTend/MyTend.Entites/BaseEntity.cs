﻿using Castle.ActiveRecord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTend.Entites
{
    public class BaseEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
    }
}
